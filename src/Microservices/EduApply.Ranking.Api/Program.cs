var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var api = app.MapGroup("/api/ranking");
api.MapGet("/health", () => TypedResults.Ok(new { service = "ranking", status = "ok" }));

// Future boundary: ranking runs and results based on approved applications.
// Do not compute or publish rankings until the scoring and tie-break rules are confirmed.
app.Run();
