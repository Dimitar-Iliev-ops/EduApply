using EduApply.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace EduApply.Web.Controllers;
[Authorize(Roles = "Administrator")]
public sealed class AdministrationController : Controller
{
    public IActionResult Index() => View(new ModuleModel("Администрация", "Управление на кампании, паралелки, етапи и формули."));
}
