# Test Mining Platform Sprint Plan

## 1. Purpose

This sprint plan converts the implementation guidance in:

- [requirements.md](./requirements.md)
- [technical-specification.md](./technical-specification.md)
- [ui-ux-plan.md](./ui-ux-plan.md)
- [security-plan.md](./security-plan.md)
- [testing-plan.md](./testing-plan.md)
- [implementation-roadmap.md](./implementation-roadmap.md)

into an ordered execution plan for delivering the v1 application.

This plan is subordinate to `requirements.md`. If any sprint task conflicts with the requirements, update the sprint plan rather than weakening the requirements.

## 2. Planning Assumptions

To keep the plan correct and implementation-ready, these assumptions are used:

1. This sprint plan targets v1 completion, not the optional AI-assistance phase.
2. PostgreSQL is the required v1 persistence provider path.
3. Blazor Server is the required primary UI.
4. Structured `Scenario` plus immutable `ScenarioVersion` remain the source of truth throughout every sprint.
5. Security, auditability, observability, and automated verification are built into each sprint rather than deferred to the end.
6. Developers need a local production-like environment and usable UI surfaces early so workflow changes can be tested visually before push.

## 3. Sprint Structure

The plan uses eight implementation sprints:

1. Sprint 0: Foundation and Architecture Decisions
2. Sprint 1: Host Shell, Persistence, and Admin Baseline
3. Sprint 2: Recording Pipeline MVP
4. Sprint 3: Scenario Authoring and Locator Intelligence
5. Sprint 4: Deterministic Generation and Export
6. Sprint 5: Replay MVP and Failure Diagnostics
7. Sprint 6: Semantic Robustness and Review Depth
8. Sprint 7: Product Hardening and v1 Completion

Each sprint contains:

- a sprint goal
- feature issues
- core requirement links
- explicit exit criteria

## 4. Sprint 0: Foundation and Architecture Decisions

### Goal

Establish the project skeleton, architectural decisions, CI/testing baseline, and shared conventions needed to deliver the rest of the product without rework.

### Issues

1. Scaffold `TestMining.Platform.*` solution structure and shared project conventions.
2. Write ADRs for generator choice, authentication provider direction, replay execution boundary, and draft persistence shape.
3. Establish CI baseline for restore, build, test, secret scanning, and PostgreSQL-backed integration execution.
4. Create local production-like developer environment bootstrap plus fixture application/test harness foundation for later recording and replay coverage.

### Requirement Links

- Sections 6, 14, 17, 22
- Sections 23.1 through 23.3

### Exit Criteria

- New platform projects exist with clear boundaries.
- CI runs on the repository and enforces build/test/security baseline.
- Developers can start a local environment that mirrors the intended application topology closely enough for real workflow testing.
- Fixture harness exists for later sprints.
- Deferred design choices that block implementation have ADR direction recorded.

## 5. Sprint 1: Host Shell, Persistence, and Admin Baseline

### Goal

Deliver the application host, authentication shell, PostgreSQL persistence baseline, admin configuration, and security-critical policy surfaces.

### Issues

1. Implement ASP.NET Core host and Blazor Server shell with role-aware navigation, usable for visual local testing from this sprint onward.
2. Implement PostgreSQL persistence baseline, EF Core migrations, and core domain entities.
3. Implement environment configuration, target URL allow-list management, and audit logging baseline.
4. Implement artefact storage abstraction, retention-policy model, and encrypted secret-storage plumbing.

### Requirement Links

- Sections 5.2, 6.1, 6.2, 7.2 through 7.5, 9.1, 10, 11, 12.1 through 12.7, 13, 16.1
- `FR-ADM-001`, `FR-ADM-003`, `FR-ADM-004`

### Exit Criteria

- Users can sign in and reach a Blazor shell with role-aware access.
- PostgreSQL migrations succeed from a clean checkout.
- Core persistence entities exist for scenarios, versions, recordings, artefacts, replay runs, healing suggestions, and audit events.
- Allow-list and retention policies are managed server-side and audited.
- The local environment is usable for visually testing the host shell and admin configuration flows.

## 6. Sprint 2: Recording Pipeline MVP

### Goal

Deliver the first end-to-end recording flow that can safely launch a browser, capture meaningful events, and persist recording progress incrementally.

### Issues

1. Implement recording session creation, validation, and browser launch with allow-list enforcement.
2. Implement recorder script delivery and init-script injection through Playwright.
3. Implement meaningful event capture, Playwright observation correlation, and reliable recorder transport.
4. Implement incremental recording persistence with pause, resume, stop, cancel, importance, ignore, and inline notes.

### Requirement Links

- `FR-REC-001` through `FR-REC-010`
- Sections 5.3.1, 9.2, 10.2, 10.4, 12.4, 12.6

### Exit Criteria

- A user can start and control a recording from the UI.
- Browser launch is blocked for disallowed target URLs.
- Meaningful events are captured and persisted incrementally.
- Sensitive inputs are masked per active policy during capture and preview.
- The recording workflow is visually testable through the local UI.

## 7. Sprint 3: Scenario Authoring and Locator Intelligence

### Goal

Transform recordings into structured drafts, expose a usable timeline editor, and persist ranked locator intelligence plus immutable scenario versions.

### Issues

1. Implement event normalisation and semantic action inference for the Phase 1 step set.
2. Implement element snapshots, locator candidate generation, ranking, and record-time validation.
3. Implement scenario draft creation, timeline workspace, and step editing workflows.
4. Implement scenario validation and immutable `ScenarioVersion` creation with change history.

### Requirement Links

- `FR-INF-001` through `FR-INF-005`
- `FR-AUTH-001`, `FR-AUTH-002`, `FR-AUTH-005`, `FR-AUTH-006`
- Sections 7.1 through 7.5, 9.3

### Exit Criteria

- Recorded sessions compile into structured scenario drafts.
- Locator candidates are ranked and persisted for every targetable step.
- Users can review, suppress, reorder, annotate, and validate steps in the timeline UI.
- Material edits create new immutable scenario versions rather than mutating committed history.
- Developers can visually test timeline editing and versioning locally.

## 8. Sprint 4: Deterministic Generation and Export

### Goal

Generate readable and reproducible C# Playwright output from approved scenario versions, with manifesting and repository-friendly export.

### Issues

1. Implement deterministic generation pipeline and generation profile model for the first supported output style.
2. Implement readable C# Playwright test generation plus runtime helper compatibility metadata.
3. Implement `LocatorResolver` helper generation and low-confidence warning surfacing in preview.
4. Implement generated artefact persistence, manifesting, preview, and repository-friendly export workflow.

### Requirement Links

- `FR-GEN-001` through `FR-GEN-010`
- Sections 5.3.3, 9.4, 10.3, 14.4, 19

### Exit Criteria

- Approved scenario versions generate deterministic file sets.
- Generated code is previewable in the UI and exportable to a repository-friendly layout.
- Generation manifests include template version, helper compatibility, and checksum details.
- Generator output is verified by snapshot/approval tests.
- Developers can visually validate generation previews and warnings locally.

## 9. Sprint 5: Replay MVP and Failure Diagnostics

### Goal

Replay scenario versions or generated artefacts in isolated contexts and provide actionable step-by-step diagnostics for failures.

### Issues

1. Implement replay execution orchestration from scenario version or generation artefact.
2. Implement step-level replay status streaming and replay workspace UI.
3. Implement replay diagnostics capture, failure categorisation, and artefact linking.
4. Implement targeted rerun support for full replay, current-step onward, and single-step diagnostics where valid.

### Requirement Links

- `FR-REP-001` through `FR-REP-005`
- Sections 5.3.4, 9.5, 10.2, 13.3

### Exit Criteria

- Users can run replay from the UI against the approved scenario version or chosen generated artefact.
- Replay runs use isolated browser contexts.
- Failures produce categorized diagnostics with step-level evidence.
- Replay status survives long-running execution and remains observable.
- Developers can visually validate replay progress and diagnostics through the local UI.

## 10. Sprint 6: Semantic Robustness and Review Depth

### Goal

Increase generated test resilience and author review quality with assertions, variables, fuzzy strategies, scoped locators, and richer diagnostic evidence.

### Issues

1. Implement variable classification model and UI editing for literals, parameters, generated values, fixtures, secrets, and ignored data.
2. Implement assertion suggestion engine, approval workflows, and outcome-oriented assertion persistence.
3. Implement fuzzy assertion strategies, scoped locators, and confidence explanation surfaces.
4. Implement richer replay diagnostics packages including screenshots and locator-resolution evidence.

### Requirement Links

- `FR-INF-006` through `FR-INF-009`
- `FR-AUTH-003`, `FR-AUTH-004`
- `FR-REP-002`, `FR-REP-003`
- Sections 9.3 through 9.5, 14.1 through 14.4

### Exit Criteria

- Authors can classify all captured data with masking-safe behavior.
- Suggested assertions are reviewable and approval-gated before generation.
- Replay diagnostics include richer artefacts and locator evidence.
- Regeneration preserves approved variable and assertion choices.
- Developers can visually validate the deeper authoring and diagnostics flows locally.

## 11. Sprint 7: Product Hardening and v1 Completion

### Goal

Finish the remaining hardening work needed to satisfy the v1 requirements and acceptance criteria: authentication bootstrap, healing approval, history/diff visibility, retention cleanup, and export/review polish.

### Issues

1. Implement authentication bootstrap options for manual login, stored storage state, and controlled cookie import.
2. Implement deterministic healing proposal generation, approval workflow, and linked immutable version creation.
3. Implement scenario history, version comparison, and trace viewer/export review surfaces.
4. Implement retention cleanup scheduling, artefact lifecycle enforcement, and operational safety/cleanup observability.

### Requirement Links

- `FR-ADM-002`
- `FR-HEAL-001` through `FR-HEAL-005`
- Sections 7.4, 12.5, 12.7, 16.4, 18.5, 19

### Exit Criteria

- Authentication bootstrap flows work end to end under controlled administration.
- Healing proposals are deterministic, evidence-backed, and approval-gated.
- Approved healing creates a new immutable scenario version linked to the originating replay run.
- Retention cleanup is scheduled, observable, and audited.
- The v1 acceptance criteria in Section 19 are demonstrably satisfied.
- The full core workflow is visually testable locally before push or deployment.

## 12. Cross-Sprint Working Agreements

These apply in every sprint:

1. Every implementation issue must cite the requirement IDs it satisfies.
2. Every behaviour change must include automated coverage linked to those requirements.
3. Security-sensitive work must document masking, encryption, audit, and retention impact.
4. Generated code is always derived output, never the editing source of truth.
5. `Symphony.*` tooling assets remain outside the change scope unless explicitly requested.

## 13. GitHub Execution Model

To keep GitHub planning easy to manage:

- each sprint should be represented by a GitHub milestone
- each deliverable issue should be assigned to exactly one sprint milestone
- issue titles should stay vertical-slice oriented instead of layer-only
- issue bodies should include scope, requirement links, dependencies, and exit expectations

Recommended labels for later use:

- `area/host`
- `area/recording`
- `area/analysis`
- `area/generation`
- `area/replay`
- `area/healing`
- `area/security`
- `area/testing`

## 14. Definition of Done Per Sprint

A sprint is only done when:

1. Its milestone issues are closed.
2. The sprint exit criteria in this document are met.
3. Build, test, and security checks pass for the implemented scope.
4. New behaviour is documented where needed.
5. No sprint introduces drift from `requirements.md`.

## 15. Post-v1 Backlog

The optional AI-assistance phase remains post-v1 and should not be treated as required for initial application completion. If scheduled later, it should be tracked as a separate milestone series with feature flags and advisory-only controls.
