using EduApply.Models;

namespace EduApply.Services;

public interface ICampaignService
{
    Task<IReadOnlyList<CampaignModel>> ListAsync(CancellationToken ct = default);
    Task<CampaignDetailsModel?> DetailsAsync(int id, CancellationToken ct = default);
}
