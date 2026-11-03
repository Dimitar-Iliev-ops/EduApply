using EduApply.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EduApply.Data;

public sealed class AdmissionDbContext(DbContextOptions<AdmissionDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Campaign> Campaigns => Set<Campaign>();
    public DbSet<AdmissionStage> AdmissionStages => Set<AdmissionStage>();
    public DbSet<SchoolClass> SchoolClasses => Set<SchoolClass>();
    public DbSet<ScoreFormula> ScoreFormulas => Set<ScoreFormula>();
    public DbSet<Applicant> Applicants => Set<Applicant>();
    public DbSet<Application> Applications => Set<Application>();
    public DbSet<Preference> Preferences => Set<Preference>();
    public DbSet<ExamResult> ExamResults => Set<ExamResult>();
    public DbSet<ApplicationDocument> ApplicationDocuments => Set<ApplicationDocument>();
    public DbSet<RankingResult> RankingResults => Set<RankingResult>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        b.ApplyConfigurationsFromAssembly(typeof(AdmissionDbContext).Assembly);
        // Avoid SQL Server multiple cascade paths; deletion will be explicit in services.
        foreach (var fk in b.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            fk.DeleteBehavior = DeleteBehavior.Restrict;
    }
}
