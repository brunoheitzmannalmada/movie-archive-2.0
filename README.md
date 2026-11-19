# Movie Archive 2.0

A portfolio and learning project built incrementally with **C# / ASP.NET Core
(.NET 10 LTS)** and, later, **React + TypeScript**. The long-term goal is a
catalog for personal media preservation (titles, editions, image files,
metadata and their relationships).

## What's included

- Four project boundaries: Domain, Application, Infrastructure and API.
- A single diagnostic endpoint and one liveness health check.
- Structured JSON logs on stdout.
- OpenTelemetry HTTP and custom tracing/metrics, with console traces in Development.
- Optional OTLP trace/metric export; no collector or AWS required.
- Dockerfile, Docker Compose, a basic smoke-test script and build CI.
- ADRs and a guided first exercise.

## Prerequisites

- .NET SDK 10.x for running locally, or Docker with Compose.
- `curl` for smoke tests.

## Run locally

```bash
dotnet --version
ASPNETCORE_ENVIRONMENT=Development ASPNETCORE_URLS=http://localhost:8080 \
  dotnet run --project backend/src/MovieArchive.Api
```

Or run the same API in a container:

```bash
docker compose up --build
```

In another terminal:

```bash
bash scripts/smoke-test.sh
```

### Endpoints

| URL | Purpose |
| --- | --- |
| `GET http://localhost:8080/health/live` | Liveness (`Healthy`) |
| `GET http://localhost:8080/api/v1/system/ping` | Diagnostic JSON, log, trace and counter |

## Observability

The API emits structured JSON logs to stdout. In `Development`, tracing is
exported to the console for immediate inspection. HTTP latency metrics and
a custom ping counter are instrumented; to **collect/view their values**, an
OTLP receiver/metrics backend must be added in a later exercise.

You can optionally point the app to an existing OTLP receiver by setting
`OTEL_EXPORTER_OTLP_ENDPOINT` (gRPC port 4317 by default). Without this variable,
nothing tries to connect to an external telemetry system. Grafana, Tempo and
Prometheus are **not** prerequisites for this version.

## Repository structure

```text
backend/src/
  MovieArchive.Api/             HTTP, configuration, observability
  MovieArchive.Application/     future use cases
  MovieArchive.Domain/          future entities and business rules
  MovieArchive.Infrastructure/  future persistence and adapters
frontend/                     reserved for React + TypeScript
infra/docker/                 portable .NET container image
docs/                         architecture, ADRs, learning steps
scripts/                      smoke test
.github/workflows/            build verification
```

## What is deliberately missing

No Movie entity, catalog API, database, migrations, frontend implementation,
authentication, scheduled jobs, cloud services or dashboards. Each is a
separate increment with its own goal and verification.

**Status:** foundation prepared, but no .NET build or Docker execution was
performed in the authoring environment. Verify `dotnet build MovieArchive.slnx`
and the first run on your own .NET 10 environment before adding features.
