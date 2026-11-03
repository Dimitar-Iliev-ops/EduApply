using EduApply.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace EduApply.Web.Controllers;
[Authorize(Roles = "Committee,Administrator")]
public sealed class DocumentsController : Controller
{
    public IActionResult Index() => View(new ModuleModel("Проверка на документи", "Преглед на документи от приемната комисия."));
}
