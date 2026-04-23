# Test Mining Platform Technical Specification

## 1. Purpose

This document translates [docs/requirements.md](./requirements.md) into an implementation-oriented technical specification for the new test mining platform described in the repository [README.md](../README.md).

It is subordinate to `docs/requirements.md`. If this document conflicts with the requirements, the requirements win.

## 2. Source Inputs

- Product source of truth: `docs/requirements.md`
- Repository framing: `README.md`
- Repository boundary guidance: Requirements Sections 17 and 22.1

## 2.1 Decision Status Legend

To remove ambiguity, this document uses the following terms consistently:

- `Locked`: directly required by `docs/requirements.md` for v1
- `Recommended`: strong implementation guidance that fits the requirements and should be followed unless an ADR records a different choice
- `Deferred`: intentionally left open by the requirements and should be resolved by ADR before implementation hardens

## 3. Product Intent

The system is a semantic test mining and stabilisation platform, not a raw click recorder. Its durable source of truth is the structured `Scenario` model, and generated Playwright code is a reproducible derived artefact.

This technical plan primarily supports:

- recording guided browser workflows
- enriching captured events with DOM, accessibility, and runtime context
- compiling recordings into immutable scenario versions
- generating readable C# Playwright output
- replaying scenarios with diagnostics
- producing deterministic healing suggestions under human approval

Relevant requirements:

- Sections 1, 2, 5, 7, 8, 18, 19, 24
- `FR-REC-*`, `FR-INF-*`, `FR-AUTH-*`, `FR-GEN-*`, `FR-REP-*`, `FR-HEAL-*`, `FR-ADM-*`

## 4. Solution Shape

The implementation should introduce a new vertical slice using the `TestMining.Platform.*` naming convention from Section 17 and must not repurpose retained `Symphony.*` assets.

Proposed solution structure:

```text
/src
  /TestMining.Platform.Host
  /TestMining.Platform.Core
  /TestMining.Platform.Application
  /TestMining.Platform.Recording
  /TestMining.Platform.Analysis
  /TestMining.Platform.Generation
  /TestMining.Platform.Replay
  /TestMining.Platform.Healing
  /TestMining.Platform.Persistence
  /TestMining.Platform.Artefacts
  /TestMining.Platform.Contracts
  /TestMining.Platform.Recorder
/tests
  /TestMining.Platform.Core.Tests
  /TestMining.Platform.Application.Tests
  /TestMining.Platform.Integration.Tests
  /TestMining.Platform.Generator.Tests
  /TestMining.Platform.E2E.Tests
  /TestMining.Platform.Fixtures
```

Notes:

- `Host` owns ASP.NET Core hosting, Blazor Server UI, DI, authentication, SignalR, HTTP endpoints, and background services.
- `Core` owns domain entities, enums, invariants, scenario contracts, and interfaces.
- `Application` owns use-case orchestration, commands, queries, validation, and DTO mapping.
- `Recording`, `Analysis`, `Generation`, `Replay`, and `Healing` own the product capability implementations.
- `Persistence` owns EF Core, PostgreSQL implementation, migrations, repositories, and provider abstractions.
- `Artefacts` owns storage abstraction and retention cleanup.
- `Contracts` owns shared API and SignalR contracts to keep host/UI boundaries explicit.
- `Recorder` owns the browser-side recorder script and deterministic build output required by Section 22.4.

## 5. Technology Decisions

Locked from requirements:

- Backend: ASP.NET Core
- UI: Blazor Server
- Browser automation: Playwright for .NET
- ORM: EF Core
- Primary database for v1: PostgreSQL
- Future secondary provider: SQL Server
- Code generation: Roslyn, Scriban, or hybrid

Recommended defaults for v1:

- ASP.NET Core server app with integrated Blazor Server host
- EF Core migrations targeting PostgreSQL first
- Scriban templates for first-pass generation readability and snapshot testing
- Roslyn reserved for later structural generation/refactoring needs
- Filesystem artefact storage behind an abstraction so object storage can be added later

Deferred by requirements and requiring ADRs before hardening:

- exact authentication provider
- exact generator implementation split between Scriban and Roslyn
- whether replay remains in-process in v1 or moves behind a worker boundary
- how much draft scenario state is normalized versus document-shaped

## 6. Architecture Principles

Derived from requirements Sections 5, 6, 15, 16, 22, and 23:

1. `Scenario` and immutable `ScenarioVersion` are the canonical business artefacts.
2. Generated code is never edited as the source of truth.
3. Browser orchestration, inference, generation, replay, healing, persistence, and UI remain separate modules.
4. Deterministic logic is mandatory on runtime-critical paths.
5. Sensitive data is masked or encrypted before persistence and before display.
6. Long-running operations are observable, resumable where reasonable, and auditable.
7. Every significant workflow is traceable to user, scenario version, replay run, and artefacts.

## 7. Logical Components

### 7.1 Host and UI

Responsibilities:

- route handling
- Blazor Server pages/components
- authentication and authorization
- internal HTTP APIs
- SignalR hubs for recording/replay progress
- background cleanup and replay workers

Key requirement links:

- Sections 5.2, 9, 10, 11, 12.1, 13, 16

### 7.2 Recording Engine

Responsibilities:

- start browser contexts with validated settings
- inject recorder script via Playwright init hooks
- capture meaningful events and correlated Playwright observations
- incrementally persist recording progress

Key requirement links:

- `FR-REC-001` through `FR-REC-010`
- Sections 5.3.1, 10.4, 12.4, 12.6

### 7.3 Analysis Engine

Responsibilities:

- clean and normalise raw event streams
- infer semantic actions and widget types
- create, rank, and validate locator candidates
- propose assertion candidates and data strategies
- assign explainable confidence scores

Key requirement links:

- `FR-INF-001` through `FR-INF-009`

### 7.4 Scenario Engine

Responsibilities:

- manage draft edits and immutable version commits
- enforce structural validation before generation
- preserve change summaries and version lineage

Key requirement links:

- Sections 7.1 through 7.5
- `FR-AUTH-001` through `FR-AUTH-006`

### 7.5 Generation Engine

Responsibilities:

- compile scenario version plus generation profile into a deterministic file set
- emit test code, runtime helpers, fixture/data types, and manifest
- persist manifest, checksum, template version, and helper compatibility metadata

Key requirement links:

- `FR-GEN-001` through `FR-GEN-010`
- Sections 5.3.3, 14.4, 19

### 7.6 Replay Engine

Responsibilities:

- execute scenario or generated artefact in isolated browser context
- stream step results and diagnostics
- classify failures
- support full, onward, and targeted reruns where valid

Key requirement links:

- `FR-REP-001` through `FR-REP-005`

### 7.7 Healing Engine

Responsibilities:

- evaluate replay failures for deterministic recovery options
- produce evidence-backed healing suggestions
- require approval before persisted scenario changes
- create linked immutable scenario versions when healing is applied

Key requirement links:

- `FR-HEAL-001` through `FR-HEAL-005`

### 7.8 Persistence and Artefacts

Responsibilities:

- EF Core entity mapping and repository access
- migrations and provider portability discipline
- encrypted data storage for sensitive material
- artefact storage, retention, and cleanup jobs
- audit trail persistence

Key requirement links:

- Sections 7, 11, 12.5, 12.7, 13.3, 16.4, 22.3

## 8. Domain Model Summary

The data model should directly implement the entities in requirements Section 7.2:

- `RecordingSession`
- `Scenario`
- `ScenarioVersion`
- `RecordedStep`
- `ElementSnapshot`
- `LocatorCandidate`
- `AssertionCandidate`
- `ReplayRun`
- `HealingSuggestion`
- `UserAccount`
- `GenerationArtifact`

Additional supporting entities recommended for v1:

- `ScenarioDraft`
- `ScenarioVariableDefinition`
- `ArtefactRecord`
- `AuditEvent`
- `EnvironmentConfiguration`
- `GenerationProfile`
- `MaskedFieldPolicy`
- `RetentionPolicy`
- `ReplayStepResult`

Design rules:

- Use application-assigned GUID/UUID keys.
- Use optimistic concurrency tokens for editable entities.
- Store immutable scenario content as versioned JSON plus normalized child tables where query value exists.
- Preserve audit metadata and UTC timestamps everywhere required by Section 7.2.0.
- Keep `ScenarioVersion` immutable after commit; any editable state lives in draft-specific storage only.
- Treat generated files, replay runs, and healing decisions as downstream artefacts that always reference a scenario version, never replace it.

### 8.1 Retention and Cleanup Defaults

The implementation should encode the Section 7.4 defaults directly in configuration and migrations-facing seed data where practical:

- screenshots and DOM snapshots: at least 30 days retention, 10 MB default item cap
- Playwright traces: at least 14 days retention, 200 MB default item cap
- session logs and replay diagnostic bundles: at least 30 days retention
- generated source bundles: retained until the owning scenario is deleted

Administrative overrides must remain auditable and server-enforced.

## 9. Main Workflows

### 9.1 Recording Workflow

1. User creates recording request with scenario name, environment, target URL, browser, and auth bootstrap mode.
2. Server validates user permissions and allow-list before launching Playwright.
3. Recording engine creates isolated browser context and injects recorder.
4. Browser events stream to host through explicit transport.
5. Engine correlates events with navigation, network, dialog, popup, trace, and screenshot signals.
6. Incremental persistence stores session progress, steps, snapshots, and artefact references.
7. Stop action finalizes the recording and triggers initial scenario draft creation.

### 9.2 Authoring Workflow

1. User opens timeline workspace for a draft scenario.
2. UI shows step list, locator confidence, assertion suggestions, and variable strategy metadata.
3. User edits, suppresses, reorders, approves assertions, and marks sensitive or variable values.
4. Scenario validator enforces coherent order, locator presence, variable definitions, and masking rules.
5. Commit action creates a new immutable `ScenarioVersion`.

### 9.3 Generation Workflow

1. User selects approved scenario version and generation profile.
2. Generator compiles scenario structure into deterministic output.
3. Host stores manifest, checksum, generator version, template version, and helper version.
4. UI previews files and warnings before export.

### 9.4 Replay and Healing Workflow

1. User launches replay from scenario version or generation artefact.
2. Engine executes steps in isolated context and streams step states.
3. Failure categorizer assigns a replay failure class.
4. Healing engine evaluates persisted candidates and contextual anchors.
5. UI shows evidence-backed proposal.
6. Approval creates a linked healing record and new immutable scenario version.

## 10. Internal API Surface

The internal authenticated API should be versioned from day one, for example `/api/v1/...`.

Suggested resource groups:

- `/recordings`
- `/recordings/{id}/events`
- `/recordings/{id}/controls`
- `/scenarios`
- `/scenarios/{id}/draft`
- `/scenario-versions/{id}`
- `/scenario-versions/{id}/generate`
- `/replays`
- `/replays/{id}`
- `/healing-suggestions/{id}`
- `/artefacts/{id}`
- `/admin/environments`
- `/admin/generation-profiles`
- `/admin/retention-policies`

API rules:

- every mutating endpoint must use explicit resource identifiers
- validation failures must return actionable error details
- long-running operations must expose status retrieval and correlation IDs
- API contracts must avoid returning sensitive raw payloads even to authenticated clients by default

SignalR channels should cover:

- recording session state
- live step feed
- replay step stream
- generation status
- background cleanup notifications where useful

## 11. Background Processing

Recommended hosted services:

- recording session supervisor
- replay execution queue/dispatcher
- artefact retention cleanup job
- encryption key rotation job runner
- diagnostics aggregation job

Recommended v1 execution boundary:

- keep replay execution in-process as a hosted service for v1 simplicity
- isolate the execution behind an application interface so it can move to a dedicated worker later without rewriting domain or UI layers

These services should operate on explicit persisted state rather than in-memory-only state.

## 12. Configuration Model

Configuration sources must support Section 11:

- `appsettings.*.json`
- environment variables
- secure secret providers
- database-backed admin settings

Configuration areas:

- connection strings
- artefact storage root/provider
- allowed target base URLs per environment
- browser defaults
- trace/video/network capture defaults
- masking rules
- retention windows and size limits
- generation defaults
- authentication provider configuration

Validation rules:

- environment-level invalid configuration fails startup
- runtime-editable invalid configuration fails save with actionable validation

## 13. Security-Critical Technical Controls

This technical specification depends on the detailed controls in [docs/security-plan.md](./security-plan.md). At minimum:

- server-side allow-list check before browser launch
- isolated browser contexts by default
- encrypted storage state, cookies, and sensitive variables at rest
- masking before persistence for previewable fields
- no secret leakage to logs, previews, or generated code
- audit trail for security-sensitive actions

## 14. Observability

The host must emit structured logs and metrics around:

- recording start/stop/failure
- generation start/complete/failure
- replay start/step/failure/complete
- healing proposal creation/approval/rejection
- artefact cleanup and retention deletions

Every event should include the identifiers required by Section 13.1 where applicable.

Metrics should be sufficient to verify:

- recording startup latency
- event-to-timeline publication latency
- generation duration
- replay startup latency
- replay pass/fail rate by failure category
- healing proposal rate and approval rate

## 15. Performance and Scaling Targets

Implementation should be designed so Section 15.2 is measurable:

- record launch readiness
- event-to-timeline latency
- generation duration
- replay startup overhead

Recommended approach:

- capture timings in application services and background workers
- expose metrics through OpenTelemetry-compatible instrumentation
- measure per environment and per browser kind

## 16. Open Technical Decisions

The following remain deferred and should be resolved in ADRs before implementation hardens:

1. Generator default: Scriban, Roslyn, or hybrid
2. Draft editing persistence shape: JSON document, normalized tables, or mixed
3. Replay execution boundary: in-process hosted service or dedicated worker process
4. Authentication provider choice for host application
5. Artefact storage abstraction depth in v1

## 17. Recommended First Vertical Slices

To align with Phase 1 in Section 18:

1. Host shell plus authenticated Blazor layout and PostgreSQL persistence bootstrap
2. Recording session creation plus allow-list validation
3. Recorder transport and incremental persistence for navigation/click/fill/select
4. Timeline review UI with draft editing
5. Locator ranking and scenario version creation
6. Deterministic generation preview and export
7. Basic replay with pass/fail diagnostics

## 18. Definition of Ready for Implementation

Implementation should not start until these are agreed:

- project layout and naming
- PostgreSQL as the active v1 provider
- generator approach for Phase 1
- fixture web applications for automated testing
- authentication approach for non-local environments
