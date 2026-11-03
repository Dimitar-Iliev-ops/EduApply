using EduApply.Data;
using EduApply.Models;
using Microsoft.EntityFrameworkCore;

namespace EduApply.Services;

public sealed class CampaignService(AdmissionDbContext db) : ICampaignService
{
    public async Task<IReadOnlyList<CampaignModel>> ListAsync(CancellationToken ct = default) =>
        await db.Campaigns.AsNoTracking().OrderByDescending(x => x.SchoolYear)
            .Select(x => new CampaignModel(x.Id, x.Name, x.SchoolYear, x.IsActive)).ToListAsync(ct);

    public async Task<CampaignDetailsModel?> DetailsAsync(int id, CancellationToken ct = default)
    {
        var campaign = await db.Campaigns.AsNoTracking().Include(x => x.Classes).ThenInclude(x => x.ScoreFormula)
            .Include(x => x.Stages).AsSplitQuery().SingleOrDefaultAsync(x => x.Id == id, ct);
        return campaign is null ? null : new(campaign.Name,
            campaign.Classes.OrderBy(x => x.Name).Select(x => new SchoolClassModel(x.Name, x.Capacity, x.ScoreFormula.Name)).ToList(),
            campaign.Stages.OrderBy(x => x.Number).Select(x => new StageModel(x.Number, x.ApplicationsOpenUtc, x.ApplicationsCloseUtc)).ToList());
    }
}
