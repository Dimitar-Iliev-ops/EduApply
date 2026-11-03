using EduApply.Models;
using EduApply.Services;
using Microsoft.AspNetCore.Mvc;

namespace EduApply.Web.Controllers;

public sealed class HomeController(ICampaignService campaigns) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct) => View(new HomeModel(await campaigns.ListAsync(ct)));
    public IActionResult Error() => View();
}
