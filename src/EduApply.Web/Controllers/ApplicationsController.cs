using EduApply.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace EduApply.Web.Controllers;
[Authorize(Roles = "Applicant")]
public sealed class ApplicationsController : Controller
{
    public IActionResult Index() => View(new ModuleModel("Заявления", "Подаване и проследяване на заявления."));
}
