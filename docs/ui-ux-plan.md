# Test Mining Platform UI/UX Plan

## 1. Purpose

This document turns the UI-related requirements in [docs/requirements.md](./requirements.md) into a delivery plan for the Blazor Server experience.

Primary requirement anchors:

- Section 4 user roles and use cases
- Section 8.1 through 8.7
- Section 9 user interface requirements
- Section 10.2 real-time updates
- Section 12.1 access control
- Section 15.6 accessibility

## 2. UX Principles

The UI should reflect the product principles from requirements Section 2.3:

1. Semantic over raw technical detail
2. Structured scenario over generated code
3. Deterministic evidence before automation or AI suggestions
4. Review before approval
5. Diagnostics always available

Practical interpretation:

- show business-meaningful labels first, raw technical payload second
- surface confidence, evidence, and warnings wherever the system infers behaviour
- keep every destructive or source-of-truth-changing action explicit
- preserve continuity between recording, editing, generation, and replay
- keep the scenario editor as the primary authoring surface; generated code preview is a downstream review surface, not the editing source of truth
- deliver the UI early enough that developers can visually test the real workflows during implementation, not only after backend completion

## 3. User Roles and UX Focus

- `Administrator`: environment configuration, policy management, audit visibility
- `Author`: recording, editing, approval, generation, replay, healing review
- `Viewer`: read-only review of scenarios, replay runs, and generated artefact metadata

Permissions should shape visible actions, not just backend responses.

Role visibility rules:

- hide or disable actions the current role cannot complete
- explain why an action is unavailable when doing so helps workflow comprehension
- never show approval or export affordances that the current user is not authorized to invoke

## 4. Primary Navigation Model

Recommended top-level navigation:

- Dashboard
- Recordings
- Scenarios
- Replays
- Artefacts
- Administration

Recommended dashboard widgets:

- recent recordings
- draft scenarios awaiting review
- recent replay failures
- healing proposals awaiting approval
- retention or configuration warnings

This navigation shell should exist early in development so the product can be exercised visually in a local environment even while deeper capability slices are still being completed.

## 5. Information Architecture

### 5.1 Recording Workspace

Supports `FR-REC-001` through `FR-REC-010` and Section 9.2.

Core regions:

- session setup panel
- browser status and connection state
- live step feed
- inline note and importance controls
- event detail drawer

Critical states:

- ready to start
- launching browser
- recording active
- paused
- stopping/finalizing
- failed with recovery guidance

### 5.2 Timeline and Scenario Editor

Supports `FR-AUTH-001` through `FR-AUTH-006` and Section 9.3.

Core regions:

- ordered step timeline
- step detail inspector
- locator candidates panel
- assertion suggestions panel
- variable classification panel
- validation summary banner

Critical states:

- draft dirty
- validation warning
- blocking validation error
- approval pending for suggested assertions
- ready to version
- version committed

### 5.3 Code Preview Workspace

Supports `FR-GEN-001` through `FR-GEN-010` and Section 9.4.

Core regions:

- file tree / manifest list
- generated code viewer
- warning panel for low-confidence decisions
- generation profile selector
- export summary

Critical states:

- no approved version selected
- generating
- generated successfully
- generation warning
- generation failed

### 5.4 Replay Workspace

Supports `FR-REP-001` through `FR-REP-005`, `FR-HEAL-001` through `FR-HEAL-005`, and Section 9.5.

Core regions:

- replay launch panel
- live step execution stream
- diagnostics sidebar
- failure evidence tabs
- healing proposal review card

Critical states:

- queued/starting
- running
- passed
- failed with diagnostics
- healing available
- healing approved/rejected

### 5.5 Administration Area

Supports `FR-ADM-001` through `FR-ADM-004`, Sections 11 and 12.7.

Core regions:

- environments and base URL allow-lists
- browser and replay defaults
- masking and sensitive-data rules
- retention policies
- authentication bootstrap options
- audit activity feed

## 6. Screen-Level Plan

### 6.1 Dashboard

Purpose:

- orient users quickly
- highlight work in progress and required action

Recommended modules:

- active recordings
- scenarios needing approval
- recent generations
- replay health summary
- outstanding healing reviews

### 6.2 New Recording Flow

User journey:

1. enter scenario name
2. choose environment
3. enter target URL
4. choose browser and recording options
5. choose authentication bootstrap mode
6. pass server-side allow-list validation
7. start recording

Validation expectations:

- invalid URL format
- URL not in environment allow-list
- missing required environment settings
- unsupported authentication bootstrap selection

### 6.3 Live Recording Screen

Priority interactions:

- pause/resume/stop without losing progress
- mark important or ignorable steps
- add inline notes at time of capture
- inspect current URL and recorder health

Design note:

The live feed should focus on meaningful user actions and collapse noise by default.

### 6.4 Scenario Editor

The editor should make system inference reviewable rather than magical.

Recommended step row content:

- sequence number
- semantic label
- step type badge
- locator confidence indicator
- assertion indicator
- variable/sensitive badges
- suppression state

Recommended inspector tabs:

- Overview
- Locator Candidates
- Assertions
- Data Strategy
- Context Evidence
- Notes and Audit

Approval checkpoints in this screen:

- suggested assertions remain visually distinct from approved assertions
- locator candidate changes should show previous and newly selected strategy
- sensitive-value classifications should update every preview surface immediately after save

### 6.5 Generation Preview

Users should see:

- which scenario version is being generated
- which generation profile and template version were used
- file manifest with warnings
- source preview
- export destination summary

Warnings should clearly separate:

- low-confidence locator use
- unapproved suggestions excluded from output
- sensitive values replaced with secure references

### 6.6 Replay and Healing

Replay diagnostics should be layered:

1. human-readable failure summary first
2. step-level timeline second
3. raw diagnostics and traces third

Healing review should always show:

- original locator
- proposed locator
- evidence for proposal
- confidence and deterministic basis
- explicit approval or rejection action

Healing review must never imply automatic application in v1. The UI language should explicitly state that approval creates a new immutable scenario version.

## 7. UX Rules for Long-Running Operations

Because recording, generation, replay, export, and cleanup are asynchronous, the UI should:

- use SignalR updates for live progress
- persist operation status so page reload does not lose state
- show elapsed time and current stage
- provide retry guidance when failures occur
- avoid blocking the entire app shell for operation-specific failures

## 8. Local Visual Testing Expectations

Because the product is intended to be developed and validated through its actual UI, the local developer environment should support visual testing of:

- sign-in and application shell navigation
- recording session creation and recording status
- timeline editing and scenario validation feedback
- generation preview and warnings
- replay execution, diagnostics, and healing review surfaces

Local visual testing should use the same Blazor Server UI that will ship, not a separate mock frontend.

## 9. Accessibility Plan

Required to support Section 15.6:

- keyboard access for all recording, editing, approval, and replay actions
- semantic landmarks and headings
- live region announcements for recording/replay status changes
- sufficient contrast for confidence states and warnings
- non-color indicators for pass/warn/fail/confidence levels
- focus management when drawers, dialogs, or review panels open

## 10. Responsive Behaviour

The UI is desktop-first for v1, but should degrade gracefully.

Recommended breakpoints:

- desktop: multi-panel workflow surfaces
- tablet/small laptop: collapsible side panels
- narrow widths: stacked panels, reduced preview width, preserved key actions

Do not hide critical validation, approval, or security warnings on smaller layouts.

## 11. Design System Guidance

Suggested component set:

- status badge
- confidence pill
- timeline row
- evidence drawer
- split-pane code/diagnostic viewer
- validation summary banner
- audit event list

Suggested state taxonomy:

- neutral
- in-progress
- success
- warning
- blocking
- sensitive

## 12. Source-of-Truth UX Rules

To stay aligned with Sections 7.1 and 8.4:

1. Users edit scenarios, not generated code.
2. Generated code views must clearly show the source scenario version and generation profile.
3. Replay and healing screens must always show which scenario version they are derived from.
4. Any action that changes persisted scenario behaviour must route through scenario versioning, not ad hoc direct mutation.

## 13. UX Acceptance Checks

The following checks should be true before UI slices are considered complete:

1. An author can complete the Phase 1 recording-to-replay path without leaving the web UI.
2. Every inferred locator or assertion can be inspected with evidence before approval.
3. Sensitive values never appear in clear text in preview surfaces.
4. Long-running operations recover gracefully from refresh or reconnect.
5. Role-restricted actions are hidden or disabled with clear rationale.
6. A developer can run the local environment and visually exercise the primary UI workflows before pushing changes.
