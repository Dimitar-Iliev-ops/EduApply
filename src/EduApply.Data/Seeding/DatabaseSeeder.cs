using EduApply.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EduApply.Data.Seeding;

public sealed class DatabaseSeeder(AdmissionDbContext db, RoleManager<IdentityRole> roles,
    UserManager<ApplicationUser> users, IConfiguration configuration)
{
    public async Task SeedAsync(CancellationToken ct = default)
    {
        var accounts = new[] {
            (Roles.Applicant, "applicant@eduapply.example"),
            (Roles.Committee, "committee@eduapply.example"),
            (Roles.Administrator, "admin@eduapply.example")
        };
        // Passwords never enter migrations, source files, or command output.
        foreach (var (role, _) in accounts)
        {
            var password = configuration[$"Seed:Passwords:{role}"];
            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException($"Set Seed:Passwords:{role} in User Secrets before seeding.");
            var candidate = new ApplicationUser { UserName = "validation@eduapply.example" };
            foreach (var validator in users.PasswordValidators)
                Ensure(await validator.ValidateAsync(users, candidate, password));
        }

        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        foreach (var role in Roles.All)
            if (!await roles.RoleExistsAsync(role)) Ensure(await roles.CreateAsync(new IdentityRole(role)));

        foreach (var (role, email) in accounts)
        {
            var user = await users.FindByEmailAsync(email);
            if (user is null)
            {
                user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
                Ensure(await users.CreateAsync(user, configuration[$"Seed:Passwords:{role}"]!));
            }
            if (!await users.IsInRoleAsync(user, role)) Ensure(await users.AddToRoleAsync(user, role));
            if (role == Roles.Applicant && !await db.Applicants.AnyAsync(x => x.UserId == user.Id, ct))
                db.Applicants.Add(new Applicant { UserId = user.Id, FullName = "Демонстрационен кандидат" });
        }
        if (!await db.Campaigns.AnyAsync(x => x.SchoolYear == 2027, ct))
        {
            var formula = new ScoreFormula { Name = "Демо: 2 × БЕЛ + 3 × математика", BulgarianWeight = 2, MathematicsWeight = 3 };
            db.Campaigns.Add(new Campaign {
                Name = "Демонстрационна кампания 2027/2028", SchoolYear = 2027, IsActive = false,
                Stages = [new AdmissionStage { Number = 1,
                    ApplicationsOpenUtc = new DateTime(2027, 7, 1, 6, 0, 0, DateTimeKind.Utc),
                    ApplicationsCloseUtc = new DateTime(2027, 7, 8, 15, 0, 0, DateTimeKind.Utc),
                    RankingUtc = new DateTime(2027, 7, 9, 6, 0, 0, DateTimeKind.Utc),
                    EnrollmentCloseUtc = new DateTime(2027, 7, 12, 15, 0, 0, DateTimeKind.Utc) }],
                Classes = [new SchoolClass { Name = "Приложно програмиране", Capacity = 26, ScoreFormula = formula },
                    new SchoolClass { Name = "Компютърни мрежи", Capacity = 26, ScoreFormula = formula }]
            });
        }
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
    }

    private static void Ensure(IdentityResult result)
    {
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => $"{x.Code}: {x.Description}")));
    }
}
