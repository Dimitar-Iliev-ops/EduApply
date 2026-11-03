using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EduApply.Data.Entities;

public sealed class ApplicationUser : IdentityUser { }
public static class Roles
{
    public const string Applicant = "Applicant";
    public const string Committee = "Committee";
    public const string Administrator = "Administrator";
    public static readonly string[] All = [Applicant, Committee, Administrator];
}
public enum ApplicationStatus { Draft, Submitted, Returned, Approved, Rejected }
public enum DocumentStatus { Pending, Approved, Returned, Rejected }
public enum EnrollmentStatus { Pending, Confirmed, Withdrawn, Expired }

public sealed class Campaign
{
    public int Id { get; set; }
    [MaxLength(150)] public string Name { get; set; } = "";
    public int SchoolYear { get; set; }
    public bool IsActive { get; set; }
    public ICollection<AdmissionStage> Stages { get; set; } = new List<AdmissionStage>();
    public ICollection<SchoolClass> Classes { get; set; } = new List<SchoolClass>();
}
public sealed class AdmissionStage
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public Campaign Campaign { get; set; } = null!;
    public int Number { get; set; }
    public DateTime ApplicationsOpenUtc { get; set; }
    public DateTime ApplicationsCloseUtc { get; set; }
    public DateTime RankingUtc { get; set; }
    public DateTime EnrollmentCloseUtc { get; set; }
}
public sealed class ScoreFormula
{
    public int Id { get; set; }
    [MaxLength(100)] public string Name { get; set; } = "";
    [MaxLength(50)] public string StrategyKey { get; set; } = "WeightedExams";
    public decimal BulgarianWeight { get; set; }
    public decimal MathematicsWeight { get; set; }
}
public sealed class SchoolClass
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public Campaign Campaign { get; set; } = null!;
    [MaxLength(150)] public string Name { get; set; } = "";
    public int Capacity { get; set; }
    public int ScoreFormulaId { get; set; }
    public ScoreFormula ScoreFormula { get; set; } = null!;
}
public sealed class Applicant
{
    public int Id { get; set; }
    [MaxLength(450)] public string UserId { get; set; } = "";
    public ApplicationUser User { get; set; } = null!;
    [MaxLength(150)] public string FullName { get; set; } = "";
}
public sealed class Application
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public Campaign Campaign { get; set; } = null!;
    public int ApplicantId { get; set; }
    public Applicant Applicant { get; set; } = null!;
    public ApplicationStatus Status { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? SubmittedUtc { get; set; }
    [MaxLength(1000)] public string? ReviewComment { get; set; }
    [Timestamp] public byte[] RowVersion { get; set; } = [];
    public ICollection<Preference> Preferences { get; set; } = new List<Preference>();
}
public sealed class Preference
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public int ApplicationId { get; set; }
    public Application Application { get; set; } = null!;
    public int SchoolClassId { get; set; }
    public SchoolClass SchoolClass { get; set; } = null!;
    public int Priority { get; set; }
}
public sealed class ExamResult
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public Application Application { get; set; } = null!;
    [MaxLength(50)] public string Subject { get; set; } = "";
    public decimal Points { get; set; }
}
public sealed class ApplicationDocument
{
    public int Id { get; set; }
    public int ApplicationId { get; set; }
    public Application Application { get; set; } = null!;
    [MaxLength(255)] public string OriginalName { get; set; } = "";
    [MaxLength(255)] public string StorageKey { get; set; } = "";
    public DocumentStatus Status { get; set; }
    [MaxLength(1000)] public string? ReviewComment { get; set; }
}
public sealed class RankingResult
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public int AdmissionStageId { get; set; }
    public AdmissionStage AdmissionStage { get; set; } = null!;
    public int ApplicationId { get; set; }
    public Application Application { get; set; } = null!;
    public int SchoolClassId { get; set; }
    public SchoolClass SchoolClass { get; set; } = null!;
    public decimal Score { get; set; }
    public int Position { get; set; }
}
public sealed class Enrollment
{
    public int Id { get; set; }
    public int RankingResultId { get; set; }
    public RankingResult RankingResult { get; set; } = null!;
    public EnrollmentStatus Status { get; set; }
    public DateTime? ConfirmedUtc { get; set; }
}
public sealed class Notification
{
    public int Id { get; set; }
    [MaxLength(450)] public string UserId { get; set; } = "";
    public ApplicationUser User { get; set; } = null!;
    [MaxLength(1000)] public string Message { get; set; } = "";
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }
}
