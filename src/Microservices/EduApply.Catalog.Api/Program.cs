var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var api = app.MapGroup("/api/catalog");
api.MapGet("/health", () => TypedResults.Ok(new { service = "catalog", status = "ok" }));

api.MapGet("/campaigns", () => TypedResults.Ok(new[]
{
    new CampaignSummary(
        Id: "demo-2027",
        Name: "Примерен прием 2027/2028",
        SchoolYear: "2027/2028",
        IsDemo: true,
        Programs:
        [
            new("Приложно програмиране", 26),
            new("Компютърни мрежи", 26)
        ])
}));

app.Run();

internal sealed record CampaignSummary(
    string Id, string Name, string SchoolYear, bool IsDemo, ProgramSummary[] Programs);

internal sealed record ProgramSummary(string Name, int Capacity);
