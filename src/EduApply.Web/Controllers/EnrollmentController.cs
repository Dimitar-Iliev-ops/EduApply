using EduApply.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace EduApply.Web.Controllers;
[Authorize(Roles = "Applicant")]
public sealed class EnrollmentController : Controller
{
    public IActionResult Index() => View(new ModuleModel("Записване", "Потвърждаване на записването в определения срок."));
}
