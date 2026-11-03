using EduApply.Services;
using Microsoft.AspNetCore.Mvc;

namespace EduApply.Web.Controllers;

public sealed class CampaignsController(ICampaignService campaigns) : Controller
{
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var model = await campaigns.DetailsAsync(id, ct);
        return model is null ? NotFound() : View(model);
    }
}
