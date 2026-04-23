# AGENTS.md

This file is the operating contract for any agent — human or AI — making changes in this repository. Read it before making a change. If it conflicts with a user instruction, surface the conflict and ask before proceeding.

## 1. Mission

This repository hosts two concerns that must not be conflated:

1. **Primary product:** the semantic test mining platform specified in [docs/requirements.md](docs/requirements.md), with conceptual background in [docs/concept.md](docs/concept.md). New application work targets this product.
2. **Retained tooling:** Symphony (`SPEC.md`, `IMPLEMENTATION_PLAN.md`, `WORKFLOW.md`, `src/Symphony.*`, `tests/Symphony.*`, `symphony_docs/`). Symphony is preserved because it is used *by* this project; it is not the product described by `docs/requirements.md`.

Every change must declare, in its pull request description, which of the two concerns it targets. Cross-boundary changes are split into separate pull requests.

## 2. Sources of Truth

In order of precedence, for test mining platform work:

1. `docs/requirements.md` (product specification)
2. `docs/concept.md` (design rationale; informative, not normative)
3. `README.md` (user-facing framing)

For Symphony work:

1. `SPEC.md`
2. `IMPLEMENTATION_PLAN.md`
3. `WORKFLOW.md`

If a spec and a plan disagree, the spec wins and the plan is updated. If requirements and Symphony docs disagree about repository direction, `docs/requirements.md` wins for product work; Symphony docs continue to govern Symphony work.

## 3. Non-Negotiable Guardrails

These are hard stops. Do not weaken them without an explicit, documented decision.

1. **Never persist, log, or emit captured secrets.** This includes `GITHUB_TOKEN`, workflow secrets, target-application credentials, session cookies, Playwright storage states, bearer tokens, and anything classified as `Sensitive`. If you are unsure whether a value is sensitive, treat it as sensitive.
2. **Never launch a Playwright browser context against a target URL absent from the administrator-managed allow-list** (requirements §12.4). The allow-list check is server-side and runs before context creation.
3. **Never encrypt-at-rest bypass.** Storage states, cookies, cached auth material, and sensitive scenario variables are encrypted at rest (requirements §12.5). No code path may write them plaintext.
4. **Never edit generated source files by hand and commit the result.** Generated output is a derived artefact. Regenerate from scenario data and templates (requirements FR-GEN-001).
5. **Never auto-apply healing to a persisted scenario in v1.** Healing produces proposals; humans approve (requirements FR-HEAL-003).
6. **Never let AI be the runtime-critical path.** AI assists; deterministic logic decides (requirements principle 3).
7. **Never mutate Symphony files from a test-mining task, or vice versa.** Scope discipline is enforced at PR review.
8. **Never parse stderr as protocol events, and never parse secrets out of logs.** (Inherited Symphony constraint retained for Symphony work.)

## 4. Architecture Rules

1. Clear boundaries. Core domain and scenario contracts must not depend on concrete infrastructure APIs. Infrastructure adapters implement interfaces declared in core.
2. Ports and adapters for the big four external systems: Playwright, persistence, artefact storage, identity provider. Each has a core-defined interface and an isolated adapter project.
3. Persistence:
   - EF Core with migrations.
   - PostgreSQL is the v1 default provider (requirements §6.2). SQL Server is a deferred option behind the same abstraction.
   - No provider-specific SQL in domain or application layers. Isolated, gated, and justified in infrastructure only.
   - Optimistic concurrency (`RowVersion`) on mutable aggregates; soft delete on `Scenario`, `RecordingSession`, `GenerationArtifact`, and artefact records (requirements §7.2.0).
4. Recording engine:
   - Playwright-native instrumentation hooks (`AddInitScriptAsync` and exposed bindings) are the primary event transport. Console scraping is a bounded fallback only (FR-REC-009).
   - Recorder script ships from source control with a pinned `RecorderVersion` surfaced in captures (requirements §22.4). No runtime fetch from third-party origins.
5. Replay engine:
   - Isolated browser contexts per run (FR-REP-005).
   - Deterministic heuristics first; AI advisory at most.
6. Generation:
   - Scenarios are the source of truth. Output is reproducible (FR-GEN-001, FR-GEN-010).
   - Snapshot/approval tests for generator output (FR-GEN, §14.4).

## 5. Coding Standards

- C# language version must remain compatible with the repository's configured .NET build settings. Do not change SDK expectations without an explicit, reviewed change.
- Nullable reference types enabled.
- Async all the way for I/O; cancellation tokens propagated through polling, subprocess, and HTTP.
- Analyzer warnings treated as errors in core projects.
- Keep classes focused and small. Split large orchestration into feature services.
- Prefer built-in ASP.NET Core and .NET primitives over third-party packages unless a concrete justification is recorded.

## 6. Security Hygiene

1. Secret-like strings must not appear in diffs. CI secret-scanning runs on every PR (requirements §22.3).
2. Any new captured data field or outbound network call is called out explicitly in the PR description.
3. Dependency vulnerability checks run before merge.
4. Structured logs include correlation IDs (recording session, scenario version, replay run, user) but never raw event payloads that could contain sensitive values.
5. Masking policy changes are retroactive: changing a classification to `Sensitive` must re-mask prior previews and re-generation (requirements §12.5).

## 7. Testing Expectations

Minimum bar for any non-trivial change:

1. Unit tests for business logic and state transitions.
2. Integration tests for infrastructure boundaries touched. Use real PostgreSQL (Testcontainers) for persistence integration — not an in-memory provider.
3. Conformance tests tagged with the requirement identifier (e.g. `FR-REC-004`) they cover.
4. Generator changes require snapshot tests (FR-GEN, §14.4).
5. Flaky tests are fixed or quarantined within one working day; persistent quarantine is not acceptable.
6. For Symphony work, keep or extend the `SPEC.md` section-17 conformance tests.

## 8. Configuration and Options

- Read target URL allow-lists, retention policies, and generation defaults from the configuration surface described in requirements §11.
- Resolve `$ENV_VAR` values in configuration.
- Fail fast on invalid required configuration at startup or on administrative save.
- Keep configurable defaults aligned with requirements §22 (build) and §7.4 (retention).

## 9. Observability

- Structured logs with recording session id, scenario id, scenario version id, replay run id, user id, browser context id (requirements §13.1).
- Operational metrics per §13.2 — sufficient to verify the performance targets in §15.2.
- Snapshot/status surfaces derived from orchestrator or scenario engine state, not ad hoc caches.
- Artefact traceability: every screenshot, trace, DOM snapshot, and generated bundle links back to the session, version, or replay run that produced it.

## 10. Delivery Workflow

1. Cite the relevant requirement identifier(s) (e.g. `FR-REC-004`) in the PR description.
2. State which of the two concerns from §1 this change targets.
3. Implement the smallest vertical slice that demonstrates the requirement.
4. Add or update tests mapped to the cited requirements.
5. Run `dotnet build` and `dotnet test` locally before handing off.
6. Update `README.md` or `docs/` if user-visible behaviour changed.
7. Keep PRs scoped. Refactors that cross the Symphony/test-mining boundary are split.

## 11. Suggested Commands

```powershell
dotnet restore
dotnet build
dotnet test
```

Database migrations (platform persistence project, once introduced):

```powershell
dotnet ef migrations add <Name> --project src/TestMining.Platform.Persistence
dotnet ef database update --project src/TestMining.Platform.Persistence
```

Symphony migrations (retained tooling):

```powershell
dotnet ef migrations add <Name> --project src/Symphony.Infrastructure/Persistence.Sqlite
dotnet ef database update --project src/Symphony.Infrastructure/Persistence.Sqlite
```

## 12. Definition of Done (Per Change)

1. Behaviour aligns with `docs/requirements.md` (or `SPEC.md` for Symphony work).
2. Tests prove the behaviour or failure mode, tagged with requirement identifiers.
3. Logging and error paths are explicit. No raw sensitive values in logs.
4. No secrets, keys, or tokens in diffs or output.
5. Reviewer can trace the change from requirement clause to implementation to test.
6. Retention, security, or captured-data surface changes are explicitly called out in the PR.
7. Generated code is reproducible from scenario data plus template version; no hand-edits committed.

## 13. Escalation

If a requirement is ambiguous, silent on an edge case, or appears to contradict another section:

1. Stop. Do not guess.
2. Raise the question in the PR description or as a clarifying note.
3. Record the resolution either by updating `docs/requirements.md` under the originating section or by adding an architecture decision record under `docs/adr/` once that directory exists.

Shortcuts that trade safety, auditability, or reproducibility for speed are not acceptable. When an obstacle appears, identify the root cause rather than bypassing a guardrail.
