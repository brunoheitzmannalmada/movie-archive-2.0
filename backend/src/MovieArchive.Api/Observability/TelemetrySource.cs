using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace MovieArchive.Api.Observability;

public static class TelemetrySource
{
    public const string Name = "MovieArchive.Api";

    public static readonly ActivitySource Activities = new(Name);
    public static readonly Meter Metrics = new(Name);
    public static readonly Counter<long> PingRequests =
        Metrics.CreateCounter<long>("moviearchive.system.ping.count");
}
