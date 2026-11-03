"""Local HTTPS smoke checks for the stage-one skeleton; no third-party packages."""
import html
import http.cookiejar
import json
import os
from pathlib import Path
import re
import ssl
import urllib.error
import urllib.parse
import urllib.request

BASE = "https://localhost:7143"


class NoRedirect(urllib.request.HTTPRedirectHandler):
    def redirect_request(self, req, fp, code, msg, headers, newurl):
        return None


def client():
    # The only target is this machine's development server with its self-signed certificate.
    return urllib.request.build_opener(
        urllib.request.HTTPSHandler(context=ssl._create_unverified_context()),
        urllib.request.HTTPCookieProcessor(http.cookiejar.CookieJar()), NoRedirect())


def request(opener, path, fields=None):
    data = None if fields is None else urllib.parse.urlencode(fields).encode()
    try:
        response = opener.open(BASE + path, data=data, timeout=20)
    except urllib.error.HTTPError as error:
        response = error
    return response.code, response.headers, response.read().decode("utf-8")


def check(condition, label):
    if not condition:
        raise AssertionError(label)
    print("PASS:", label)


if __name__ == "__main__":
    anonymous = client()
    status, _, body = request(anonymous, "/")
    check(status == 200 and "Демонстрационна кампания" in html.unescape(body), "home reads seeded campaign")
    check(request(anonymous, "/Campaigns/Details/1")[0] == 200, "campaign details")
    check(request(anonymous, "/Campaigns/Details/2147483647")[0] == 404, "unknown campaign returns 404")
    for path in ["/Applications", "/Preferences", "/Documents", "/Ranking", "/Enrollment", "/Administration"]:
        status, headers, _ = request(anonymous, path)
        check(status == 302 and "/Account/Login" in headers["Location"], "anonymous blocked: " + path)
    check(request(anonymous, "/Identity/Account/Register")[0] == 200, "Identity registration page renders")
    secret_path = Path(os.environ["APPDATA"]) / "Microsoft/UserSecrets/eduapply-stage1-local-settings/secrets.json"
    secrets = json.loads(secret_path.read_text(encoding="utf-8-sig"))
    accounts = [
        ("Applicant", "applicant", ["/Applications", "/Preferences", "/Ranking", "/Enrollment"], ["/Documents", "/Administration"]),
        ("Committee", "committee", ["/Documents", "/Ranking"], ["/Administration", "/Applications"]),
        ("Administrator", "admin", ["/Administration", "/Documents", "/Ranking"], ["/Applications"]),
    ]
    for role, name, allowed, denied in accounts:
        opener = client()
        status, _, body = request(opener, "/Identity/Account/Login")
        check(status == 200, role + " login form")
        token = re.search(r'name="__RequestVerificationToken"[^>]*value="([^"]+)"', body)
        check(token is not None, role + " login anti-forgery token")
        status, headers, _ = request(opener, "/Identity/Account/Login", {
            "Input.Email": name + "@eduapply.example",
            "Input.Password": secrets["Seed:Passwords:" + role],
            "__RequestVerificationToken": html.unescape(token.group(1)),
        })
        check(status == 302 and headers.get("Location") == "/", role + " signs in")
        cookie = " ".join(headers.get_all("Set-Cookie", [])).lower()
        check("httponly" in cookie and "secure" in cookie and "samesite=lax" in cookie, role + " secure cookie flags")
        for path in allowed:
            check(request(opener, path)[0] == 200, role + " allowed: " + path)
        for path in denied:
            status, headers, _ = request(opener, path)
            check(status == 302 and "/Account/AccessDenied" in headers["Location"], role + " denied: " + path)
        check(request(opener, "/Identity/Account/Logout", {})[0] == 400, role + " rejects logout without anti-forgery token")
    print("Stage-one smoke checks passed. No passwords were printed.")
