namespace EduApply.Models;

public sealed record CampaignModel(int Id, string Name, int SchoolYear, bool IsActive);
public sealed record SchoolClassModel(string Name, int Capacity, string Formula);
public sealed record StageModel(int Number, DateTime OpensUtc, DateTime ClosesUtc);
public sealed record HomeModel(IReadOnlyList<CampaignModel> Campaigns);
public sealed record CampaignDetailsModel(string Name, IReadOnlyList<SchoolClassModel> Classes, IReadOnlyList<StageModel> Stages);
public sealed record ModuleModel(string Title, string Description);
