# Test Mining Platform Requirements Traceability Matrix

## 1. Purpose

This matrix links the requirements in [docs/requirements.md](./requirements.md) to the implementation planning documents added for the repository.

Use this document to keep future code changes, ADRs, test cases, and pull requests traceable to the source requirements.

## 2. Planning Document Index

- [Documentation Index](./README.md)
- [Technical Specification](./technical-specification.md)
- [UI/UX Plan](./ui-ux-plan.md)
- [Security Plan](./security-plan.md)
- [Testing Plan](./testing-plan.md)
- [Implementation Roadmap](./implementation-roadmap.md)
- [Sprint Plan](./sprint-plan.md)

## 3. Requirement Coverage Matrix

| Requirement Area | Requirement Source | Primary Planning Docs |
| --- | --- | --- |
| Product intent and canonical scenario principle | Sections 1, 2, 7.1, 24 | Technical Specification, Implementation Roadmap |
| Scope and phase boundaries | Sections 3, 18, 21 | Technical Specification, Implementation Roadmap, Sprint Plan |
| Roles and primary use cases | Section 4 | UI/UX Plan, Technical Specification |
| Logical architecture and lifecycles | Sections 5, 17 | Technical Specification, Implementation Roadmap, Sprint Plan |
| Technology stack and provider strategy | Section 6 | Technical Specification |
| Domain entities and artefacts | Sections 7.2 through 7.5 | Technical Specification, Testing Plan |
| Recording requirements | `FR-REC-001` through `FR-REC-010` | Technical Specification, UI/UX Plan, Security Plan, Testing Plan, Implementation Roadmap, Sprint Plan |
| Inference requirements | `FR-INF-001` through `FR-INF-009` | Technical Specification, UI/UX Plan, Testing Plan, Sprint Plan |
| Scenario authoring requirements | `FR-AUTH-001` through `FR-AUTH-006` | Technical Specification, UI/UX Plan, Testing Plan, Sprint Plan |
| Generation requirements | `FR-GEN-001` through `FR-GEN-010` | Technical Specification, UI/UX Plan, Testing Plan, Implementation Roadmap, Sprint Plan |
| Replay requirements | `FR-REP-001` through `FR-REP-005` | Technical Specification, UI/UX Plan, Security Plan, Testing Plan, Sprint Plan |
| Healing requirements | `FR-HEAL-001` through `FR-HEAL-005` | Technical Specification, UI/UX Plan, Security Plan, Testing Plan, Implementation Roadmap, Sprint Plan |
| Administration requirements | `FR-ADM-001` through `FR-ADM-004` | Technical Specification, UI/UX Plan, Security Plan, Testing Plan, Sprint Plan |
| UI requirements | Section 9 | UI/UX Plan, Technical Specification |
| API and real-time requirements | Section 10 | Technical Specification, UI/UX Plan |
| Configuration requirements | Section 11 | Technical Specification, Security Plan |
| Security and privacy requirements | Section 12 | Security Plan, Testing Plan, Technical Specification |
| Observability and diagnostics | Section 13 | Technical Specification, Security Plan, Testing Plan |
| Verification and testing requirements | Section 14 | Testing Plan |
| Non-functional requirements | Section 15 | Technical Specification, UI/UX Plan, Testing Plan |
| Deployment and operational safety | Section 16 | Technical Specification, Security Plan, Implementation Roadmap |
| Local production-like developer environment and visual workflow validation | Sections 9.1, 14.1, 16.2, 18.1, 19 | Technical Specification, UI/UX Plan, Testing Plan, Implementation Roadmap, Sprint Plan |
| Acceptance criteria | Section 19 | Testing Plan, Technical Specification, Implementation Roadmap, Sprint Plan |
| Risks and constraints | Section 20 | Technical Specification, Implementation Roadmap |
| Build, test, and delivery requirements | Section 22 | Testing Plan, Technical Specification, Security Plan |
| Implementer guardrails | Section 23 | Security Plan, Technical Specification, Implementation Roadmap |

## 4. Acceptance Criteria Mapping

| Acceptance Criterion | Requirements Reference | Planned Evidence |
| --- | --- | --- |
| Record a workflow and persist a structured scenario | `FR-REC-001`, `FR-REC-004`, `FR-REC-010` | Technical Specification workflow design, Testing Plan Phase 1 path |
| Review, edit, and approve through UI | `FR-AUTH-001` through `FR-AUTH-006` | UI/UX Plan scenario editor, Testing Plan scenario authoring coverage |
| Generate readable C# Playwright artefacts | `FR-GEN-001` through `FR-GEN-010` | Technical Specification generation engine, Testing Plan generator verification |
| Replay with useful diagnostics | `FR-REP-001`, `FR-REP-002`, `FR-REP-003` | Technical Specification replay engine, UI/UX Plan replay workspace, Testing Plan replay coverage |
| Rank and persist locator candidates | `FR-INF-004`, `FR-INF-005` | Technical Specification analysis engine, Testing Plan inference coverage |
| Mask and encrypt sensitive values | `FR-REC-007`, Sections 12.3, 12.5 | Security Plan controls, Testing Plan security coverage |
| Approve/reject outcome-oriented assertions | `FR-INF-007`, `FR-AUTH-003` | UI/UX Plan inspector design, Testing Plan assertion approval coverage |
| Healing is deterministic, evidenced, and approval-based | `FR-HEAL-001` through `FR-HEAL-005` | Security Plan and Technical Specification healing workflow, Testing Plan healing coverage |
| Stack is ASP.NET Core + Blazor Server + PostgreSQL | Sections 6.1, 6.2 | Technical Specification solution shape |
| Generated output remains reproducible | `FR-GEN-001`, `FR-GEN-010` | Technical Specification generation design, Testing Plan snapshot strategy |
| Allow-list blocks disallowed targets | Section 12.4 | Security Plan allow-list control, Testing Plan allow-list tests |
| Observability supports performance verification | Sections 13.1, 15.2 | Technical Specification observability design, Testing Plan performance instrumentation |
| Developers can validate the real UI locally before push | Sections 9.1, 14.1, 16.2, 19 | UI/UX Plan local visual testing expectations, Testing Plan local validation environment, Sprint Plan sprint exit criteria |

## 5. Future Use

When code work begins, this matrix should be extended with:

- implementation project/file locations
- test case IDs
- pull request references
- ADR references for open decisions
- links to evidence for phase exit criteria
