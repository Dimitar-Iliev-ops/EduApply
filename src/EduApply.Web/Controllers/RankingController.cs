using EduApply.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace EduApply.Web.Controllers;
[Authorize(Roles = "Applicant,Committee,Administrator")]
public sealed class RankingController : Controller
{
    public IActionResult Index() => View(new ModuleModel("Класиране", "Резултати по етапи и желания."));
}
