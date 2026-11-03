using EduApply.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace EduApply.Web.Controllers;
[Authorize(Roles = "Applicant")]
public sealed class PreferencesController : Controller
{
    public IActionResult Index() => View(new ModuleModel("Желания", "Подреждане на паралелките по приоритет."));
}
