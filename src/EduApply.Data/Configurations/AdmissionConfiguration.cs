using EduApply.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EduApply.Data.Configurations;

public sealed class AdmissionConfiguration :
    IEntityTypeConfiguration<Campaign>, IEntityTypeConfiguration<AdmissionStage>,
    IEntityTypeConfiguration<SchoolClass>, IEntityTypeConfiguration<ScoreFormula>,
    IEntityTypeConfiguration<Applicant>, IEntityTypeConfiguration<Application>,
    IEntityTypeConfiguration<Preference>, IEntityTypeConfiguration<ExamResult>,
    IEntityTypeConfiguration<RankingResult>, IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Campaign> b)
    {
        b.HasIndex(x => x.SchoolYear).IsUnique();
        b.ToTable(t => t.HasCheckConstraint("CK_Campaign_Year", "[SchoolYear] BETWEEN 2020 AND 2100"));
    }
    public void Configure(EntityTypeBuilder<AdmissionStage> b)
    {
        b.HasAlternateKey(x => new { x.Id, x.CampaignId });
        b.HasIndex(x => new { x.CampaignId, x.Number }).IsUnique();
        b.ToTable(t => {
            t.HasCheckConstraint("CK_Stage_Number", "[Number] > 0");
            t.HasCheckConstraint("CK_Stage_Dates", "[ApplicationsOpenUtc] < [ApplicationsCloseUtc] AND [ApplicationsCloseUtc] <= [RankingUtc] AND [RankingUtc] < [EnrollmentCloseUtc]");
        });
    }
    public void Configure(EntityTypeBuilder<SchoolClass> b)
    {
        b.HasAlternateKey(x => new { x.Id, x.CampaignId });
        b.HasIndex(x => new { x.CampaignId, x.Name }).IsUnique();
        b.ToTable(t => t.HasCheckConstraint("CK_Class_Capacity", "[Capacity] > 0"));
    }
    public void Configure(EntityTypeBuilder<ScoreFormula> b)
    {
        b.Property(x => x.BulgarianWeight).HasPrecision(6, 2);
        b.Property(x => x.MathematicsWeight).HasPrecision(6, 2);
        b.ToTable(t => t.HasCheckConstraint("CK_Formula_Weights", "[BulgarianWeight] >= 0 AND [MathematicsWeight] >= 0 AND ([BulgarianWeight] + [MathematicsWeight]) > 0"));
    }
    public void Configure(EntityTypeBuilder<Applicant> b) => b.HasIndex(x => x.UserId).IsUnique();
    public void Configure(EntityTypeBuilder<Application> b)
    {
        b.HasAlternateKey(x => new { x.Id, x.CampaignId });
        b.HasIndex(x => new { x.CampaignId, x.ApplicantId }).IsUnique();
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
    }
    public void Configure(EntityTypeBuilder<Preference> b)
    {
        b.HasOne(x => x.Application).WithMany(x => x.Preferences)
            .HasForeignKey(x => new { x.ApplicationId, x.CampaignId }).HasPrincipalKey(x => new { x.Id, x.CampaignId });
        b.HasOne(x => x.SchoolClass).WithMany()
            .HasForeignKey(x => new { x.SchoolClassId, x.CampaignId }).HasPrincipalKey(x => new { x.Id, x.CampaignId });
        b.HasIndex(x => new { x.ApplicationId, x.Priority }).IsUnique();
        b.HasIndex(x => new { x.ApplicationId, x.SchoolClassId }).IsUnique();
        b.ToTable(t => t.HasCheckConstraint("CK_Preference_Priority", "[Priority] > 0"));
    }
    public void Configure(EntityTypeBuilder<ExamResult> b)
    {
        b.Property(x => x.Points).HasPrecision(5, 2);
        b.HasIndex(x => new { x.ApplicationId, x.Subject }).IsUnique();
        b.ToTable(t => t.HasCheckConstraint("CK_Exam_Points", "[Points] BETWEEN 0 AND 100"));
    }
    public void Configure(EntityTypeBuilder<RankingResult> b)
    {
        b.HasOne(x => x.Application).WithMany().HasForeignKey(x => new { x.ApplicationId, x.CampaignId }).HasPrincipalKey(x => new { x.Id, x.CampaignId });
        b.HasOne(x => x.SchoolClass).WithMany().HasForeignKey(x => new { x.SchoolClassId, x.CampaignId }).HasPrincipalKey(x => new { x.Id, x.CampaignId });
        b.HasOne(x => x.AdmissionStage).WithMany().HasForeignKey(x => new { x.AdmissionStageId, x.CampaignId }).HasPrincipalKey(x => new { x.Id, x.CampaignId });
        b.HasIndex(x => new { x.AdmissionStageId, x.ApplicationId }).IsUnique();
        b.Property(x => x.Score).HasPrecision(10, 2);
        b.ToTable(t => {
            t.HasCheckConstraint("CK_Ranking_Score", "[Score] >= 0");
            t.HasCheckConstraint("CK_Ranking_Position", "[Position] > 0");
        });
    }
    public void Configure(EntityTypeBuilder<Enrollment> b) => b.HasIndex(x => x.RankingResultId).IsUnique();
}
