using FluxOps.Application;
using FluxOps.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configuration and logging are configured by default.
// Add logging & configuration-bound services
builder.Services.AddLogging();
builder.Services.AddFluxOps(builder.Configuration);

var app = builder.Build();

// Health endpoint
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// Run pipeline with a posted ingestion spec.
// Pipeline run endpoint
app.MapPost("/pipeline/run", async (IMLOpsPipeline pipeline, IngestionSpec spec, CancellationToken ct) =>
{
    var result = await pipeline.RunAsync(spec, ct);
    return Results.Ok(new { result.Succeeded, result.Message, result.Metrics });
});

app.Run();

// Expose Program for WebApplicationFactory integration testing
public partial class Program { }
