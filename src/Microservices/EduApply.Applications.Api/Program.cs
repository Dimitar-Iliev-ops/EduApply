var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var api = app.MapGroup("/api/applications");
api.MapGet("/health", () => TypedResults.Ok(new { service = "applications", status = "ok" }));

// Future boundary: authenticated application drafts, preferences, and document metadata.
// Writes stay disabled until identity and service-owned storage are connected.
app.Run();
