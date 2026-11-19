using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace MovieArchive.Api.Observability;

public static class TelemetryRegistration
{
    public static IServiceCollection AddMovieArchiveTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        // OTLP is optional: the application works without any collector or cloud account.
        var useOtlp = !string.IsNullOrWhiteSpace(
            configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("movie-archive-api"))
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation(options =>
                {
                    // Avoid diagnostic noise from periodic health probes.
                    options.Filter = context =>
                        !context.Request.Path.StartsWithSegments("/health");
                });
                tracing.AddSource(TelemetrySource.Name);

                // Development traces are visible even without Grafana / Tempo.
                if (environment.IsDevelopment())
                {
                    tracing.AddConsoleExporter();
                }

                if (useOtlp)
                {
                    tracing.AddOtlpExporter();
                }
            })
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation();
                metrics.AddMeter(TelemetrySource.Name);

                if (useOtlp)
                {
                    metrics.AddOtlpExporter();
                }
            });

        return services;
    }
}
