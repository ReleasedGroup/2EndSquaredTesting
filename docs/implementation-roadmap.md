# Test Mining Platform Implementation Roadmap

## 1. Purpose

This roadmap converts the delivery phasing in [docs/requirements.md](./requirements.md) Section 18 into implementation slices that fit the repository and reduce delivery risk.

## 2. Roadmap Principles

1. Build vertical slices, not disconnected layers.
2. Preserve `Symphony.*` tooling assets untouched unless explicitly requested.
3. Prove the canonical `Scenario` workflow early.
4. Treat security and observability as phase-entry requirements, not polish.
5. Keep each slice traceable to `FR-*` requirements and acceptance criteria.

## 3. Foundation Track

This track should start before Phase 1 feature depth expands.

Deliverables:

- `TestMining.Platform.*` project scaffolding
- PostgreSQL persistence bootstrap and migrations pipeline
- authentication shell and role model
- internal API versioning convention
- SignalR infrastructure
- artefact storage abstraction
- local production-like developer environment bootstrap
- fixture app test harness
- baseline observability and audit primitives
- ADRs for generator choice, authentication provider, replay execution boundary, and draft persistence shape

Requirement anchors:

- Sections 6, 10, 11, 12.1, 13, 14, 16, 17, 22

## 4. Phase 1: Core Proof

Objective:

Prove the end-to-end path from recording to generated code to replay on a simple fixture application.

Suggested slices:

1. Local production-like developer startup workflow with Blazor UI and PostgreSQL
2. Application shell and navigation suitable for visual testing
3. Recording session creation plus allow-list validation
4. Playwright browser launch and recorder injection
5. Meaningful event capture for navigation, click, fill, select, and checkbox
6. Incremental recording persistence
7. Initial timeline UI with human-readable steps
8. Locator candidate creation and ranking
9. Draft scenario editing and immutable version creation
10. Deterministic C# generator for one profile
11. Basic replay execution and step-level result reporting

Phase 1 exit should match Section 18.5 Phase 1 exit criteria.

Phase 1 is not complete unless URL allow-list enforcement and encryption of stored auth/session material are demonstrably working, because they are part of the stated exit criteria rather than optional hardening.

Phase 1 is also not complete unless a developer can run the real UI locally against a production-like stack shape and visually test the core workflow before push.

## 5. Phase 2: Robustness

Objective:

Increase semantic quality and diagnostic usefulness without changing the core source-of-truth model.

Suggested slices:

1. assertion suggestion engine
2. variable classification editing
3. fuzzy/resilient assertion strategies
4. scoped locator support
5. replay failure screenshots
6. richer replay diagnostics packaging
7. confidence explanation surfaces

Requirement anchors:

- `FR-INF-006` through `FR-INF-009`
- `FR-AUTH-003`, `FR-AUTH-004`
- `FR-REP-002`, `FR-REP-003`

## 6. Phase 3: Product Hardening

Objective:

Harden the system for multi-user and longer-lived operational use.

Suggested slices:

1. authentication bootstrap modes
2. export packaging workflow
3. trace viewer integration
4. healing approval workflow and version lineage
5. scenario history and diff views
6. retention cleanup scheduling and admin controls

Requirement anchors:

- `FR-ADM-002`
- `FR-HEAL-001` through `FR-HEAL-005`
- Sections 7.4, 12.7, 16.4

## 7. Phase 4: AI Assistance

Objective:

Add advisory AI features without weakening deterministic execution or approval rules.

Suggested slices:

1. AI-assisted naming suggestions
2. AI-assisted assertion suggestion refinement
3. AI-assisted healing proposal augmentation
4. page object refactoring suggestions

Guardrails:

- feature flagged
- disabled by default
- advisory only
- deterministic evidence always shown
- no mutation of persisted scenarios without the same approval and traceability rules required for deterministic healing

## 8. Cross-Cutting Backlog

These items should run across phases:

- security hardening and audits
- performance instrumentation
- accessibility improvements in Blazor UI
- documentation updates
- CI pipeline maturation

## 9. Recommended Work Breakdown Structure

Suggested implementation order:

1. scaffold projects and shared contracts
2. establish persistence, auth shell, and audit primitives
3. build recording pipeline
4. build timeline authoring and versioning
5. build generation
6. build replay
7. add diagnostics depth
8. add healing workflow
9. add advanced admin and AI-adjacent features

## 10. Dependencies and Risks

Key dependencies:

- stable fixture applications for test coverage
- Playwright recorder script build strategy
- PostgreSQL dev/test environment availability
- final authentication provider selection

Key risks:

- over-investing in raw recording before scenario editing is solid
- leaking sensitive data through early diagnostics
- making generator output non-deterministic
- coupling replay too tightly to generated code instead of scenario model

## 11. Milestone Definition

Each roadmap slice should only be closed when it has:

- linked requirement IDs
- passing automated coverage
- documented artefact/security impact
- structured logs and diagnostics where applicable
- no mutation of retained `Symphony.*` assets
