using EduApply.Data;
using EduApply.Data.Entities;
using EduApply.Data.Seeding;
using EduApply.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Set ConnectionStrings:DefaultConnection in User Secrets. See README.md.");
builder.Services.AddDbContext<AdmissionDbContext>(o => o.UseSqlServer(connection));
builder.Services.AddDefaultIdentity<ApplicationUser>(o => {
    o.SignIn.RequireConfirmedAccount = false; // No email sender in stage 1.
    o.User.RequireUniqueEmail = true;
    o.Password.RequiredLength = 12;
    o.Lockout.MaxFailedAccessAttempts = 5;
    o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
}).AddRoles<IdentityRole>().AddEntityFrameworkStores<AdmissionDbContext>();
builder.Services.ConfigureApplicationCookie(o => {
    o.Cookie.HttpOnly = true;
    o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    o.Cookie.SameSite = SameSiteMode.Lax;
    o.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    o.SlidingExpiration = true;
});
builder.Services.AddAntiforgery(o => o.Cookie.SecurePolicy = CookieSecurePolicy.Always);
builder.Services.AddControllersWithViews(o => o.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute()));
builder.Services.AddRazorPages();
builder.Services.AddScoped<ICampaignService, CampaignService>();
builder.Services.AddScoped<DatabaseSeeder>();

var app = builder.Build();
if (args.Contains("--initialize-database"))
{
    if (!app.Environment.IsDevelopment())
        throw new InvalidOperationException("Demo initialization is available only in Development.");
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<AdmissionDbContext>().Database.MigrateAsync();
    if (app.Configuration.GetValue<bool>("Seed:Enabled"))
        await scope.ServiceProvider.GetRequiredService<DatabaseSeeder>().SeedAsync();
    return;
}
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();

public partial class Program { }
