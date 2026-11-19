# Architecture — foundation v0.1

## Goals

- Cloud-ready, not cloud-dependent: run without vendor accounts or cloud services.
- Small verifiable steps: avoid premature business functionality.
- A modular monolith with explicit dependency direction.
- Observable from day one, without a mandatory monitoring server.

## Logical boundaries

```text
[React + TypeScript — future]
              |
              | HTTP/JSON — future
              v
MovieArchive.Api              HTTP endpoints and composition root
   |                 \
   v                  v
MovieArchive.Application   MovieArchive.Infrastructure
   |                  |     (future implementations)
   v                  v
MovieArchive.Domain <-------+

Observability: API -> ILogger + ActivitySource + Meter
                 -> stdout logs / console traces
                 -> optional OTLP collector
```

Allowed project references:

- `Domain`: none.
- `Application`: Domain.
- `Infrastructure`: Application and Domain.
- `Api`: Application and Infrastructure.

These are **compile-time boundaries**, not separate network services. Business
modules will be added only when the first use case requires them.

## Health endpoints

- `/health/live`: process liveness; no external dependencies.
- No `/health/ready` yet: there is no database or other dependency to check.

## Observability

- Logs: `ILogger` -> JSON stdout, including Activity correlation when available.
- Tracing: OpenTelemetry instrumentation of ASP.NET Core + custom ActivitySource.
- Metrics: ASP.NET Core HTTP instrumentation + a custom ping counter.
- Development: trace Console exporter, no Collector required.
- Optional: set `OTEL_EXPORTER_OTLP_ENDPOINT` to export traces and metrics
  to an OTLP receiver. This never changes the business API or domain.
- Monitoring backends (Grafana, Prometheus, Tempo) are **not** installed yet.

## Cloud portability

Local: .NET process (or Docker) + stdout + optional OTLP endpoint.

Later AWS: same container, environment settings, managed relational database
and an OTLP-compatible collector, if desired. Avoid AWS SDK references in Domain
or Application. Storage integrations will be introduced behind interfaces when
the first real use case needs them.

## Out of scope v0.1

Movies, CRUD, migrations, PostgreSQL, authentication, media scanners, jobs,
React UI, business tests, dashboards and deployment to AWS.
