# Local Developer Environment

This document defines the initial production-like developer bootstrap introduced for issue `#7`.

## Concern

This change targets the **primary product** described in [requirements.md](./requirements.md). It does not alter retained `Symphony.*` tooling assets.

## Requirement Links

- Sections 14.1, 14.2, 14.3
- Sections 16.2, 18.1, 22.2, 22.5
- Acceptance Criterion 13

## What Is Included

- `src/TestMining.Platform.Host`
  - ASP.NET Core host with a real Blazor Server shell
  - local environment diagnostics for PostgreSQL, artefact storage, and fixture availability
- `src/TestMining.Platform.Fixtures`
  - representative fixture flows for forms, editable grids, modal dialogs, login, dynamic ids, and toast feedback
  - deterministic reset hook at `POST /api/test/reset`
- `tests/TestMining.Platform.Host.Tests`
  - host smoke coverage for the local developer shell
- `tests/TestMining.Platform.Fixtures.Tests`
  - deterministic fixture reset and route coverage
- `deploy/local/docker-compose.platform-dev.yml`
  - PostgreSQL dependency for the local production-like profile

## Prerequisites

1. Docker Desktop or another local Docker runtime
2. `.NET SDK 10`
3. An environment variable named `PLATFORM_POSTGRES_PASSWORD`

PowerShell example:

```powershell
$env:PLATFORM_POSTGRES_PASSWORD = "<set-a-local-dev-password>"
```

## Startup Workflow

1. Start PostgreSQL:

```powershell
docker compose -f deploy/local/docker-compose.platform-dev.yml up -d postgres
```

2. Export the PostgreSQL connection string for the host:

```powershell
$env:ConnectionStrings__PlatformPostgres = "Host=localhost;Port=5432;Database=testmining_platform;Username=testmining_platform;Password=$env:PLATFORM_POSTGRES_PASSWORD"
```

3. Run the fixture harness:

```powershell
dotnet run --project src/TestMining.Platform.Fixtures
```

4. Run the host shell:

```powershell
dotnet run --project src/TestMining.Platform.Host
```

5. Open `http://localhost:5174` and confirm:
   - artefact storage reports ready
   - PostgreSQL reports ready
   - fixture harness reports ready

## Fixture Routes

- `/forms`
- `/grid`
- `/modal`
- `/login`

The fixture app root (`/`) links to each representative flow and exposes deterministic seed data for later recording and replay scenarios.

## Reset Hooks

- `POST /api/test/reset`
  - restores the seeded customer list
  - restores the seeded audit trail
- `GET /api/test/state`
  - returns the current deterministic fixture snapshot for test assertions

## Notes

- The host only performs a lightweight PostgreSQL probe in this slice. Full persistence and migrations remain separate product work.
- The fixture harness intentionally uses simple server-owned endpoints and static pages so later Playwright coverage can exercise predictable DOM patterns without introducing app-specific complexity too early.
