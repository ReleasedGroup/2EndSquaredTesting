# Test Mining Platform Testing Plan

## 1. Purpose

This document defines the verification strategy for the platform described in [docs/requirements.md](./requirements.md). It expands on Sections 14, 15.5, 18.5, 19, and 22.3 through 22.5.

## 2. Testing Goals

The test strategy must prove that:

1. structured scenarios are persisted and versioned correctly
2. recorder and analysis pipelines transform browser interactions into meaningful scenario steps
3. generated Playwright output is deterministic and readable
4. replay diagnostics and healing workflows are trustworthy
5. security and masking rules are enforced continuously

## 3. Test Principles

Derived from requirements:

- unit tests are the default
- integration tests use real infrastructure where it matters
- end-to-end tests are limited but representative
- flaky tests must be fixed or quarantined quickly
- every acceptance criterion must trace to automated coverage
- the PostgreSQL-backed path is the required v1 provider path in CI and local integration testing

## 4. Repository Test Layout

Recommended test projects:

```text
/tests
  /TestMining.Platform.Core.Tests
  /TestMining.Platform.Application.Tests
  /TestMining.Platform.Integration.Tests
  /TestMining.Platform.Generator.Tests
  /TestMining.Platform.E2E.Tests
  /TestMining.Platform.Fixtures
```

Purpose:

- `Core.Tests`: entities, invariants, domain services, deterministic ranking/scoring rules
- `Application.Tests`: command/query handlers, validation, workflow orchestration with mocked boundaries
- `Integration.Tests`: PostgreSQL, EF Core mappings, artefact storage, Playwright coordination, encryption/masking integration
- `Generator.Tests`: snapshot/approval verification of generated output
- `E2E.Tests`: thin end-to-end flows against shipped fixture apps
- `Fixtures`: sample web apps/pages covering required target patterns

## 5. Coverage by Requirement Area

### 5.1 Recording

Coverage target:

- `FR-REC-001` through `FR-REC-010`

Test themes:

- session creation validation
- allow-list enforcement before launch
- recorder init script injection
- event capture for meaningful event types
- sensitive field masking during capture
- pause/resume/stop behaviour
- incremental persistence recovery

### 5.2 Analysis and Inference

Coverage target:

- `FR-INF-001` through `FR-INF-009`

Test themes:

- event stream normalisation
- semantic action inference
- widget recognition
- locator ranking order
- locator uniqueness validation
- variable classification rules
- assertion suggestion and fuzzy matching strategies
- confidence scoring explainability

### 5.3 Scenario Authoring

Coverage target:

- `FR-AUTH-001` through `FR-AUTH-006`

Test themes:

- step editing rules
- locator candidate switching
- assertion approval workflows
- sensitive-data policy validation
- immutable scenario version creation
- draft vs committed version behaviour

### 5.4 Generation

Coverage target:

- `FR-GEN-001` through `FR-GEN-010`

Test themes:

- deterministic file output
- readable code structure assertions
- helper compatibility metadata
- variable/fixture generation
- repository-friendly layout and manifest generation
- generator warnings for low-confidence decisions

### 5.5 Replay and Healing

Coverage target:

- `FR-REP-001` through `FR-REP-005`
- `FR-HEAL-001` through `FR-HEAL-005`

Test themes:

- scenario replay pass/fail execution
- failure categorisation
- targeted rerun rules
- isolated replay contexts
- deterministic healing proposal creation
- human approval gate enforcement
- traceability from replay failure to approved healing version

### 5.6 Administration and Security

Coverage target:

- `FR-ADM-001` through `FR-ADM-004`
- Sections 11, 12, 13, 16.4

Test themes:

- environment configuration validation
- retention policy persistence
- authentication bootstrap configuration
- audit event creation
- encryption and masking enforcement
- log sanitization
- retention cleanup behaviour and artefact traceability

## 6. Test Layers

### 6.1 Unit Tests

Scope:

- no browser
- no database
- deterministic logic only

Examples:

- locator ranking priority
- scenario validation rules
- assertion inference based on synthetic signals
- code generation model transformations before rendering

### 6.2 Integration Tests

Scope:

- real PostgreSQL
- EF Core migrations
- real artefact storage abstraction against local filesystem
- Playwright coordination against fixture pages where appropriate

Examples:

- migration application from clean database
- persistence of scenario versions and related steps
- encryption-at-rest round trip
- recorder event flow into persistent draft scenario
- replay diagnostics package creation
- retention cleanup deleting expired artefacts while preserving audit evidence

### 6.3 End-to-End Tests

Scope:

- run the host app
- exercise core user journeys through the UI or internal APIs
- use representative fixture applications

Required Phase 1 path:

1. authenticate
2. record simple CRUD workflow
3. review timeline
4. commit scenario version
5. generate C# output
6. replay and inspect result

## 7. Fixture Application Plan

Based on Section 14.3, fixtures should cover:

- simple forms
- editable grids
- modal dialogs
- login flows
- unstable/dynamic IDs
- toast notifications

Recommended implementation:

- small ASP.NET Core fixture apps or static pages served from test host
- deterministic seed data
- stable routes and reset hooks for test runs

## 8. Generator Verification Strategy

Required by Section 14.4.

Recommended approach:

- snapshot or approval tests for generated files
- normalize line endings and timestamp placeholders before comparison
- assert both full-file snapshots and structural invariants

Structural invariants should include:

- file names
- namespaces
- helper references
- absence of fixed sleeps by default
- redaction of sensitive values

## 9. Performance and Reliability Testing

Support Sections 15.1 through 15.3.

Measure:

- recording startup readiness
- event-to-timeline latency
- generation duration for 100-step scenarios
- replay startup overhead

Recommended practice:

- capture metrics during integration suites
- run periodic benchmark job outside fast PR loop
- keep thresholds visible but avoid fragile benchmark tests in ordinary CI

CI should still fail when basic instrumentation needed to measure Section 15.2 is removed or broken.

## 10. Security Testing

Must include:

- allow-list rejection tests
- authorization tests for admin-only settings
- secret redaction and masking tests
- encryption persistence tests
- no-secret-in-generated-output tests
- audit trail creation tests

## 11. Traceability and Conformance

Every requirement-backed feature should include:

- test case IDs linked to `FR-*` requirements or section numbers
- location of automated coverage
- acceptance-criteria mapping

The repository should maintain this mapping in [docs/requirements-traceability-matrix.md](./requirements-traceability-matrix.md).

## 12. CI Pipeline Expectations

Pull request CI should:

1. restore packages
2. build the solution
3. run unit and integration tests
4. run generator snapshot tests
5. publish test results and coverage
6. run secret scanning and policy checks
7. run PostgreSQL-backed integration tests using a containerized database, per Section 22.3

Suggested command groups:

- `dotnet restore`
- `dotnet build`
- `dotnet test`

## 13. Exit Criteria by Phase

### Phase 1

- record-to-scenario flow proven against fixture CRUD app
- locator candidates persisted
- generated output compiles
- replay reports per-step pass/fail
- allow-list and encryption rules tested

### Phase 2

- assertion inference and variable classification round-trip tested
- replay diagnostics bundle tested
- screenshot-on-failure behaviour tested

### Phase 3

- authentication bootstrap tested end to end
- healing approval flow creates new immutable version
- retention cleanup job tested and observable

## 14. Conformance Matrix Maintenance

When implementation begins, every new feature PR should add or update:

- the linked `FR-*` identifiers
- the automated tests proving the requirement
- any changed acceptance-criteria evidence
- any new retained artefact or sensitive-data handling impact

### Phase 4

- AI-assisted suggestions verified as advisory only
- deterministic diagnostics still present alongside AI output
