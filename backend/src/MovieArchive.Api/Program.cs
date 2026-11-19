using System.Diagnostics;
using MovieArchive.Api.Observability;

var builder = WebApplication.CreateBuilder(args);

// Structured, machine-readable logs, written to stdout.
builder.Logging.ClearProviders();
builder.Logging.Configure(options =>
    options.ActivityTrackingOptions = ActivityTrackingOptions.TraceId | ActivityTrackingOptions.SpanId);
builder.Logging.AddJsonConsole(options =>
{
    options.IncludeScopes = true;
    options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
    options.UseUtcTimestamp = true;
});

builder.Services.AddHealthChecks();
builder.Services.AddMovieArchiveTelemetry(builder.Configuration, builder.Environment);

var app = builder.Build();

// Infrastructure-only endpoint: no movie entities or business rules yet.
app.MapGet("/api/v1/system/ping", (ILogger<Program> logger) =>
{
    using var activity = TelemetrySource.Activities.StartActivity("system.ping");
    TelemetrySource.PingRequests.Add(1);

    logger.LogInformation("System ping handled: {Operation}", "system.ping");

    return Results.Ok(new
    {
        status = "ok",
        service = "movie-archive-api",
        traceId = Activity.Current?.TraceId.ToString()
    });
});

// Liveness only. Readiness dependencies (database, etc.) will come later.
app.MapHealthChecks("/health/live");

app.Run();

// Supports future in-process API integration tests.
public partial class Program { }
