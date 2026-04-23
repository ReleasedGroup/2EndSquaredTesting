# @EndSquareTesting Requirements Specification

## 1. Executive Summary

The application defined by this specification is not a raw click recorder. It is a semantic test mining and stabilisation platform.

The platform shall:

- Record browser sessions performed by a human user against a target web application.
- Observe DOM state, accessibility metadata, navigation changes, network outcomes, and runtime context around each meaningful interaction.
- Transform raw browser events into a structured, intent-rich scenario model.
- Infer resilient selectors, variable data strategies, and business-level assertions.
- Generate readable C# Playwright test code and supporting assets from the scenario model.
- Replay generated tests and attempt deterministic healing when selectors or data drift.
- Preserve enough artefacts, metadata, and diagnostics to let users review, edit, approve, and regenerate scenarios over time.

The platform shall be built with the following solution model:

- Backend: ASP.NET Core
- Frontend: Blazor Server
- Database: PostgreSQL
- Browser automation: Microsoft.Playwright for .NET
- Code generation: Roslyn or Scriban templates
- Desktop packaging later: .NET MAUI Hybrid or Electron wrapper

The single most important product decision is this: the system shall store structured scenarios as the source of truth, not generated code. Generated code is a derived artefact and must always be reproducible from persisted scenario data plus generation settings.

## 2. Product Vision

### 2.1 Problem Statement

Teams frequently want fast automated coverage of real user workflows, but hand-writing end-to-end tests is expensive and generic recorders produce brittle output. Raw click streams, unstable CSS selectors, exact-text assertions, and timing-based playback produce tests that quickly fail after routine UI changes.

The platform shall solve this by treating recording as a data capture step and test generation as a compilation step. It shall infer user intent, promote resilient selectors, classify dynamic data, and generate maintainable Playwright tests that align with how users experience the application.

### 2.2 Product Goal

Enable product owners, testers, developers, and analysts to record business workflows once, refine them in a guided UI, and produce resilient C# Playwright tests that can be replayed, diagnosed, and healed as applications evolve.

### 2.3 Product Principles

The product shall be designed around these principles:

1. Semantic first.
The system shall prefer user-facing meaning such as roles, labels, accessible names, nearby headings, and business outcomes over raw DOM structure.

2. Structured source of truth.
The scenario model shall be the canonical artefact from which tests, page objects, diagnostics, and healing recommendations are derived.

3. Deterministic before intelligent.
The system shall rely on deterministic heuristics and Playwright-native behaviour first. AI-assisted suggestions may augment the system, but must not be the only execution path for runtime-critical flows.

4. User review is part of the workflow.
The platform shall support manual review and editing before generated code is considered ready.

5. Diagnostics are mandatory.
Every recording, generation, replay, and healing attempt shall preserve sufficient evidence for supportability and debugging.

## 3. Scope

### 3.1 In Scope

The v1 platform shall support:

- Guided browser session recording
- DOM and accessibility metadata capture
- Playwright-based environment observation
- Post-recording scenario review and editing
- Locator ranking and locator candidate persistence
- Assertion suggestion and manual approval
- Variable data extraction and classification
- C# Playwright code generation
- Generated helper library emission
- Replay execution and diagnostics
- Deterministic selector healing workflows
- Persistent storage of sessions, scenarios, steps, artefacts, and replay history
- Multi-user authenticated web administration experience through Blazor Server
- Export of generated tests into repository-friendly file structures

### 3.2 Out of Scope for v1

The following are explicitly out of scope unless later prioritised:

- Full low-code test authoring without any review step
- Support for native mobile app automation
- AI-only locator resolution at execution time
- Canvas-heavy, game-like, or non-DOM-centric application automation as a first-class supported scenario
- Rich collaborative review workflows such as branching, inline comments, or merge conflict handling between multiple editors
- Fine-grained enterprise RBAC beyond foundational role separation
- General-purpose workflow orchestration unrelated to browser test mining
- Electron or .NET MAUI desktop packaging in the initial release

### 3.3 Deferred but Anticipated Scope

The architecture shall allow later addition of:

- Page object refactoring generation
- CI/CD export packs and pipeline templates
- Browser extension-based capture
- AI-assisted scenario summarisation and healing proposals
- Team collaboration and scenario versioning
- Desktop packaging for offline-friendly operator experiences

## 4. Target Users and Primary Use Cases

### 4.1 User Roles

The system shall recognise these primary personas:

1. Test Author
Records workflows, reviews steps, approves assertions, and generates tests.

2. Developer
Consumes generated code, integrates it into solution structures, and refines helpers or templates.

3. QA Engineer
Validates workflow coverage, replay reliability, and failure diagnostics.

4. Platform Administrator
Configures browser runners, storage, security, code-generation defaults, and environment connections.

### 4.2 Core Use Cases

The platform shall support these end-to-end use cases:

1. Record a new business workflow from a live application.
2. Review and edit the recorded timeline before generation.
3. Mark inputs as fixed, variable, generated, lookup-driven, or sensitive.
4. Accept or reject suggested assertions.
5. Generate a readable Playwright C# test suite and helper files.
6. Replay the generated scenario against the target application.
7. Investigate replay failures using screenshots, traces, DOM context, and locator diagnostics.
8. Accept a healing suggestion and regenerate the derived code.

## 5. High-Level System Overview

### 5.1 Product Capabilities

The application shall consist of these major capability areas:

1. Recording
Browser session launch, event capture, instrumentation injection, and artefact creation.

2. Interpretation
DOM intelligence, locator ranking, variable classification, widget recognition, and assertion inference.

3. Scenario Authoring
Timeline editing, step suppression, annotation, data strategy configuration, and acceptance of inferred outputs.

4. Generation
Transformation of scenario models into C# Playwright tests, helpers, fixtures, and optional page object scaffolding.

5. Replay and Healing
Execution, diagnostics, candidate re-resolution, deterministic healing, and controlled regeneration.

6. Administration
Environment configuration, browser configuration, template settings, project export settings, and user management.

### 5.2 Logical Architecture

The solution shall be decomposed into the following logical components:

1. Blazor Server UI
- Hosts recording workflows, timeline editing, generation preview, replay diagnostics, and admin surfaces.

2. ASP.NET Core Application Host
- Provides HTTP endpoints, SignalR backplane for live session updates, background workers, authentication, orchestration, and API surfaces for the UI.

3. Recording Engine
- Launches Playwright browsers and coordinates injected capture scripts, event streams, screenshots, traces, and browser-side metadata collection.

4. Capture and Analysis Engine
- Normalises raw browser events, enriches them with DOM context, identifies semantic widgets, ranks locators, and produces assertion candidates.

5. Scenario Engine
- Stores, edits, versions, validates, and materialises structured scenarios.

6. Code Generation Engine
- Produces C# Playwright test source files, helper classes, optional page objects, and configuration assets using Roslyn or Scriban.

7. Replay and Healing Engine
- Replays scenarios or generated tests, captures outcomes, diagnoses failures, evaluates fallback locators, and records healing proposals.

8. Persistence Layer
- Stores structured entities, audit history, artefact references, configuration, and operational logs in PostgreSQL or SQL Server.

9. Artefact Storage
- Stores screenshots, traces, exported code bundles, DOM snapshots, and replay attachments on a filesystem or object-backed abstraction.

## 5.3 Operational Lifecycles

### 5.3.1 Recording Lifecycle

The recording lifecycle shall follow this logical sequence:

1. User creates a recording session with target URL and settings.
2. The backend provisions a Playwright browser context and applies configured bootstrap settings.
3. Recorder instrumentation is injected before or at page startup.
4. The user performs actions in the target application.
5. Raw browser events and Playwright observations are streamed to the backend.
6. The backend normalises events and persists session artefacts incrementally.
7. The user stops recording.
8. The backend finalises the session and generates an initial scenario draft for review.

### 5.3.2 Scenario Authoring Lifecycle

The authoring lifecycle shall support:

1. Reviewing the recorded step timeline.
2. Removing or suppressing noise.
3. Promoting meaningful events into semantic actions.
4. Approving or rejecting inferred assertions.
5. Classifying test data and secrets.
6. Choosing generation profile settings.
7. Saving an immutable scenario version ready for generation or replay.

### 5.3.3 Generation Lifecycle

The generation lifecycle shall support:

1. Selecting an approved scenario version.
2. Applying generation profile and template settings.
3. Producing output files and a manifest.
4. Storing a checksum and file inventory.
5. Presenting the generated output for preview and export.

### 5.3.4 Replay and Healing Lifecycle

The replay and healing lifecycle shall support:

1. Selecting a scenario version or generated artefact for execution.
2. Launching an isolated Playwright replay context.
3. Executing steps while recording diagnostics.
4. Classifying failures when they occur.
5. Attempting deterministic healing when drift is suspected.
6. Presenting a proposal with supporting evidence.
7. Applying approved healing changes to a new scenario version or updating approved locator preferences under controlled rules.

## 6. Mandatory Technology Requirements

### 6.1 Application Stack

The implementation shall use:

- ASP.NET Core for backend hosting and service orchestration
- Blazor Server for the primary web UI
- Microsoft.Playwright for .NET for browser control, observation, replay, trace capture, and browser-context management
- EF Core for relational persistence
- PostgreSQL or SQL Server as the durable database
- Roslyn and/or Scriban for code generation

### 6.2 Database Provider Portability

The application shall support PostgreSQL and SQL Server as first-class database providers. To make this feasible:

- The persistence layer shall use EF Core abstractions and migrations.
- Provider-specific SQL shall be avoided unless isolated behind clearly bounded infrastructure components.
- Data types, indexing strategies, JSON storage, and concurrency rules shall be designed to work on both supported providers.
- One provider may be chosen as the default local-development profile, but the schema and data-access patterns must remain portable.

### 6.3 Browser Support

The initial supported execution browser shall be Chromium through Playwright. The architecture shall not prevent later support for WebKit and Firefox, but those browsers are not required for v1 unless explicitly enabled by configuration.

## 7. Information Architecture and Domain Model

### 7.1 Canonical Principle

The canonical persisted business artefact shall be `Scenario`. Generated tests, replay runs, and healing decisions shall reference the scenario version they were derived from.

### 7.2 Core Entities

The persistence model shall include, at minimum, these entities.

#### 7.2.1 RecordingSession

Represents an interactive capture session started by a user.

Required fields:

- `Id`
- `Name`
- `TargetBaseUrl`
- `EnvironmentName`
- `Status`
- `StartedByUserId`
- `StartedAtUtc`
- `EndedAtUtc`
- `BrowserKind`
- `BrowserChannel`
- `Headless`
- `AuthenticationMode`
- `RecordAssertionsAutomatically`
- `MaskedFieldPolicyJson`
- `SettingsJson`

#### 7.2.2 Scenario

Represents a business workflow compiled from one or more recording sessions and maintained as the product source of truth.

Required fields:

- `Id`
- `Name`
- `Description`
- `Status`
- `PrimaryRecordingSessionId`
- `TargetApplicationName`
- `TargetBaseUrl`
- `EnvironmentName`
- `CreatedByUserId`
- `CreatedAtUtc`
- `UpdatedAtUtc`
- `CurrentVersionNumber`
- `DefaultGenerationProfileId`
- `TagsJson`

#### 7.2.3 ScenarioVersion

Represents an immutable version of the scenario structure used for generation or replay traceability.

In-progress edits shall not mutate an immutable `ScenarioVersion`. Any draft editing workflow shall
store draft state separately and create a new immutable scenario version when changes are committed.

Required fields:

- `Id`
- `ScenarioId`
- `VersionNumber`
- `ParentScenarioVersionId`
- `CreatedAtUtc`
- `CreatedByUserId`
- `ChangeSummary`
- `StructureJson`
- `InferenceMetadataJson`
- `ApprovedForGeneration`

#### 7.2.4 RecordedStep

Represents a normalised action or assertion candidate in sequence order.

Required fields:

- `Id`
- `ScenarioVersionId`
- `Sequence`
- `StepType`
- `SemanticActionType`
- `DisplayName`
- `UserNote`
- `Status`
- `UrlBefore`
- `UrlAfter`
- `TimestampUtc`
- `DurationMs`
- `RawValue`
- `NormalisedValue`
- `VariableStrategy`
- `SensitivityClassification`
- `TargetElementSnapshotId`
- `ParentContainerSnapshotId`
- `ExpectedOutcomeJson`
- `GeneratedCodeHintJson`

#### 7.2.5 ElementSnapshot

Represents the known properties of an element at record or replay time.

Required fields:

- `Id`
- `CapturePhase`
- `RecordingSessionId` (nullable; populated for recording-time capture)
- `ReplayRunId` (nullable; populated for replay-time capture)
- `RecordedStepId`
- `TagName`
- `InputType`
- `Role`
- `AccessibleName`
- `LabelText`
- `Placeholder`
- `TextContent`
- `TitleAttribute`
- `NameAttribute`
- `ValuePreview`
- `TestId`
- `AriaAttributesJson`
- `DataAttributesJson`
- `CssPath`
- `XPath`
- `BoundingBoxJson`
- `VisibilityState`
- `EnabledState`
- `ParentContextJson`
- `SiblingContextJson`
- `WidgetKind`
- `DomHash`

#### 7.2.6 LocatorCandidate

Represents one potential locator strategy for a target element.

Required fields:

- `Id`
- `RecordedStepId`
- `StrategyType`
- `Expression`
- `ScopeExpression`
- `Priority`
- `Confidence`
- `UniquenessAtRecordTime`
- `VisibilityAtRecordTime`
- `Recommended`
- `FailureReasonIfRejected`

#### 7.2.7 AssertionCandidate

Represents a suggested or approved expected outcome.

Required fields:

- `Id`
- `RecordedStepId`
- `AssertionType`
- `ExpressionJson`
- `Priority`
- `Confidence`
- `SourceSignal`
- `Approved`
- `RejectedReason`

#### 7.2.8 ReplayRun

Represents one execution attempt of a scenario or generated test.

Required fields:

- `Id`
- `ScenarioId`
- `ScenarioVersionId`
- `ReplaySourceKind`
- `GenerationArtifactId` (nullable; populated when replay uses a generated artefact bundle)
- `Status`
- `StartedAtUtc`
- `EndedAtUtc`
- `TriggeredByUserId`
- `ExecutionEnvironment`
- `BrowserKind`
- `Headless`
- `FailureCategory`
- `FailureSummary`
- `DiagnosticsJson`

#### 7.2.9 HealingSuggestion

Represents a proposed change when replay detects drift.

Required fields:

- `Id`
- `ReplayRunId`
- `RecordedStepId`
- `SuggestionType`
- `CurrentFailureEvidenceJson`
- `ProposedLocatorJson`
- `DeterministicConfidence`
- `AiConfidence`
- `ApprovalStatus`
- `AppliedAtUtc`
- `AppliedByUserId`

#### 7.2.10 GenerationArtifact

Represents a generated bundle or file set.

Required fields:

- `Id`
- `ScenarioVersionId`
- `GeneratorKind`
- `GeneratorVersion`
- `TemplateProfile`
- `TemplateVersion`
- `OutputRoot`
- `ManifestJson`
- `CreatedAtUtc`
- `CreatedByUserId`
- `Checksum`

### 7.3 Structured Scenario Contract

The application shall define a first-class structured scenario contract that can represent:

- Navigation steps
- Click actions
- Fill actions
- Select actions
- Checkbox and toggle interactions
- Keyboard actions
- Dialog interactions
- Grid and list interactions
- Widget-scoped semantic actions
- Assertions
- Control flow metadata such as waits inferred from user-facing events
- Variable definitions
- Data generation strategies
- Replay and healing hints

The scenario contract shall be expressive enough that:

- Tests can be regenerated without rerecording.
- Helper libraries can evolve independently of the raw recording.
- Multiple code-generation styles can be supported later.
- Healing decisions can update the scenario model without directly editing generated output.

## 7.4 Artefact Model

The application shall treat the following as first-class artefacts:

- Screenshots
- Playwright traces
- DOM snapshots
- Session logs
- Generated source files
- Replay diagnostics packages

Each artefact shall have:

- A stable identifier
- A logical owner such as recording session, scenario version, or replay run
- Content type
- Retention classification
- Storage path or provider-specific handle
- Created timestamp
- Optional checksum

## 7.5 State Model Expectations

The system shall define explicit statuses for at least:

- Recording sessions
- Scenarios
- Scenario versions
- Replay runs
- Healing suggestions
- Generation artefacts

Status transitions shall be explicit, validated, and auditable.

## 8. Functional Requirements

### 8.1 Recording Requirements

#### FR-REC-001 Guided Recording Creation

The system shall let a user start a recording by providing:

- Scenario name
- Target application URL
- Optional environment label
- Browser selection
- Whether automatic assertion suggestions are enabled
- Authentication or session bootstrap mode

#### FR-REC-002 Browser Session Launch

The system shall launch a Playwright browser context for recording with configurable:

- Headless or headed mode
- Viewport
- Storage state
- Trace settings
- Video settings, if enabled
- Network capture settings

#### FR-REC-003 Early Instrumentation Injection

The system shall inject recorder logic before application content loads wherever technically possible, using Playwright-supported initialisation hooks such as `AddInitScriptAsync`.

#### FR-REC-004 Event Capture

The recorder shall capture, at minimum, these event types when user meaningful:

- Page navigation
- Click
- Double click when distinguishable
- Input
- Change
- Form submit
- Key press
- Dialog open and close
- File download start
- Popup open
- Checkbox or toggle change
- Select and autocomplete selection

The recorder shall suppress noise events such as incidental mouse movement unless explicitly enabled for diagnostics.

#### FR-REC-005 Element Context Capture

For each meaningful event, the system shall capture contextual element metadata including, where available:

- HTML tag
- Input type
- Role
- Accessible name
- Label text
- Placeholder
- Visible text
- Title attribute
- Name attribute
- Test id or equivalent stable data attribute
- Bounding box
- Enabled and visible state
- Nearby heading
- Parent form, region, modal, or table context
- Candidate CSS path
- Candidate XPath as a last-resort fallback

#### FR-REC-006 Observer Correlation

The recording engine shall correlate user events with surrounding Playwright-observed browser state including:

- URL changes
- Page load phases
- Network requests and responses
- Console errors and warnings, if enabled
- Dialog lifecycle
- Download lifecycle
- Popup lifecycle
- Screenshots
- Trace segments

#### FR-REC-007 Sensitive Data Handling

The user shall be able to mark fields or values as sensitive during or after recording. Sensitive values shall be masked in persisted logs, previews, and generated artefacts unless the user explicitly authorises retention in a secure variable form.

#### FR-REC-008 Recording Controls

The UI shall allow the user to:

- Start
- Pause
- Resume
- Stop
- Cancel
- Mark current step as important
- Mark current step as ignorable
- Add an inline note

#### FR-REC-009 Recorder Transport

The recorder shall use a reliable browser-to-host transport mechanism for event delivery. Preferred mechanisms include Playwright-exposed bindings or a similarly explicit channel. Console scraping may be used only as a bounded fallback or diagnostic technique, not as the primary durable transport contract.

#### FR-REC-010 Incremental Persistence

Long-running recording sessions shall persist meaningful progress incrementally so that unexpected host failures do not necessarily lose the entire session.

### 8.2 Post-Processing and Inference Requirements

#### FR-INF-001 Event Normalisation

The system shall transform raw captured events into a cleaned event stream by:

- Removing technical noise
- Merging related input bursts
- Folding navigation waits into surrounding intent
- Linking actions to the most likely observed outcomes

#### FR-INF-002 Semantic Action Inference

The system shall infer higher-order action meaning where possible, including but not limited to:

- Open details page
- Save form
- Submit search
- Open modal
- Confirm dialog
- Select row in grid
- Apply filter
- Choose date
- Dismiss notification

#### FR-INF-003 Widget Recognition

The system shall recognise common UI widget patterns where feasible, including:

- Forms
- Tables and grids
- Modals and dialogs
- Dropdowns and combo boxes
- Tabs
- Toast notifications
- Date pickers
- Autocomplete controls

The recognised widget type shall be stored as metadata usable by generation and healing logic.

#### FR-INF-004 Locator Ranking

For each targetable step, the system shall create a ranked set of locator candidates. The ranking algorithm shall prioritise:

1. Explicit test id or configured stable data attribute
2. Role plus accessible name
3. Label-based targeting
4. Visible text
5. Placeholder
6. Scoped parent-region plus child-role targeting
7. Stable attribute combinations
8. CSS selector fallback
9. XPath fallback

#### FR-INF-005 Locator Validation

Each locator candidate shall be validated at record time when possible for uniqueness, visibility, and scope confidence.

#### FR-INF-006 Variable Classification

The system shall support classifying entered or observed values into at least these categories:

- Fixed test data
- Scenario variable
- Generated data
- Lookup value
- Sensitive value
- Incidental value that should not become an assertion target

#### FR-INF-007 Assertion Suggestion

The system shall infer candidate assertions from post-action signals including:

- Success toast visibility
- URL pattern change
- Heading change
- Dialog closure
- Newly visible row, card, or detail view
- Relevant network success response
- Count changes where meaningful

#### FR-INF-008 Fuzzy Outcome Strategies

The system shall support resilience strategies for data-sensitive assertions including:

- Exact text to contains matching
- Regex matching for identifiers
- Date and time range validation
- Row existence by key column
- Partial heading matching
- Minimum count assertions

#### FR-INF-009 Confidence Scoring

The system shall assign confidence scores to inferred locators, assertions, widget classifications, and healing suggestions. Confidence values shall be explainable and based on deterministic evidence where possible.

### 8.3 Scenario Authoring Requirements

#### FR-AUTH-001 Timeline Review

The UI shall present recorded scenarios as an ordered timeline of meaningful steps with human-readable labels.

#### FR-AUTH-002 Step Editing

The user shall be able to:

- Delete a step
- Suppress a step from generation
- Reorder steps where valid
- Convert a step into an assertion
- Change a step to use a different locator candidate
- Add notes and rationale
- Mark a step as variable-driven
- Mark a step as sensitive

#### FR-AUTH-003 Assertion Approval

Suggested assertions shall not be treated as final until approved by the user or by a configured auto-approval rule.

#### FR-AUTH-004 Data Strategy Editing

The author shall be able to review every captured value and choose whether it becomes:

- A literal
- A scenario parameter
- A generated value
- A fixture reference
- A secret
- Ignored data

#### FR-AUTH-005 Scenario Validation

Before generation, the system shall validate that the scenario is structurally coherent, including:

- Ordered step sequence
- Supported step types
- Required locator presence
- Required variable definitions
- No unresolved sensitive-data policy violations

#### FR-AUTH-006 Version Creation

Saving a material scenario change after review shall create a new immutable scenario version.
If the product supports draft editing, the draft state shall be stored separately from immutable
scenario versions until the user commits the change. The application shall preserve enough history
to identify what changed between versions.

### 8.4 Code Generation Requirements

#### FR-GEN-001 Derived Output Principle

Generated source files shall always be reproducible from scenario data and template settings. Manual edits to generated code shall not silently become the source of truth.

#### FR-GEN-002 Supported Output

The v1 system shall generate:

- C# Playwright test classes
- Shared locator-resolution helpers
- Optional data model or fixture classes
- Supporting test project files or export manifests where needed

#### FR-GEN-003 Readability

Generated code shall prioritise readability for human developers. It shall:

- Use clear method names
- Avoid unreadable deeply nested locator code where helper abstraction is appropriate
- Separate data setup from action steps
- Use Playwright web-first assertions rather than fixed sleeps

#### FR-GEN-004 LocatorResolver Helper

The generated runtime helper library shall include a locator-resolution abstraction capable of:

- Trying candidates in ranked order
- Validating uniqueness before acting
- Optionally scoping search to a parent container
- Emitting useful diagnostics when no candidate resolves uniquely

#### FR-GEN-005 Assertion Generation

Generated assertions shall prefer user-facing outcome checks over implementation-detail checks.

#### FR-GEN-006 Variable and Fixture Generation

When configured, the generator shall emit strongly typed classes or structures representing scenario variables, generated data, and fixture data.

#### FR-GEN-007 Generation Profiles

The system shall support generation profiles so teams can choose between styles such as:

- Flat test methods
- Test plus helper methods
- Test plus page object scaffolding

#### FR-GEN-008 Template Technology

The implementation may use Roslyn, Scriban, or a hybrid approach, but the chosen approach shall support:

- Deterministic output
- Repeatable formatting
- Testable generation logic
- Easy future extension

#### FR-GEN-009 Repository-Friendly Output

The generation system shall support output layouts that are easy to place under source control, including predictable file names, stable folder structure, and manifests that identify which files belong to each generation run.

#### FR-GEN-010 Runtime Helper Compatibility

Generated helper libraries shall remain versioned and compatible with the generator profile that emitted them. Breaking helper changes shall be traceable through generation manifests or template version metadata.

### 8.5 Replay Requirements

#### FR-REP-001 Scenario Replay

The system shall replay a scenario using Playwright and the currently approved scenario version or the chosen generated artefact.

#### FR-REP-002 Replay Diagnostics

Each replay run shall capture:

- Start and end timestamps
- Browser and environment settings
- Screenshots at key failure points
- Playwright traces when enabled
- Step-level pass or fail state
- Locator resolution attempts
- Assertion evaluation results
- Failure categorisation

#### FR-REP-003 Failure Categories

Replay failures shall be classified into categories including:

- Locator resolution failure
- Timeout
- Unexpected navigation
- Assertion failure
- Authentication failure
- Environment availability failure
- Script or generation bug

#### FR-REP-004 Targeted Rerun

The UI shall allow rerunning:

- The full scenario
- The current step onward
- A single step for diagnostic purposes where valid

#### FR-REP-005 Replay Isolation

Replay runs shall execute in isolated browser contexts and shall not share mutable session state unless explicitly configured for a trusted use case.

### 8.6 Healing Requirements

#### FR-HEAL-001 Deterministic Healing First

When replay fails due to locator drift, the system shall first attempt deterministic recovery using:

- Alternate persisted locator candidates
- Scoped searches
- Fuzzy accessible-name or text matching
- Parent context anchoring
- Nearby heading anchoring

#### FR-HEAL-002 Healing Evidence

Every healing proposal shall include evidence showing:

- The original locator strategy
- Why it failed
- The proposed replacement
- What contextual signals supported the proposal
- Confidence scores

#### FR-HEAL-003 Human Approval

Healing changes that alter the persisted scenario shall require explicit approval unless an administrator enables a controlled auto-apply policy for deterministic high-confidence matches.

#### FR-HEAL-004 AI as Adviser

AI-assisted healing, if enabled later, shall be advisory and reviewable. It shall not bypass deterministic diagnostics or obscure the basis for a proposal.

#### FR-HEAL-005 Healing Traceability

When a healing proposal is approved and applied, the system shall retain a clear audit record linking the replay failure, the proposal, the approver, and the resulting scenario or locator change.

### 8.7 Administration Requirements

#### FR-ADM-001 Environment Configuration

Administrators shall be able to configure:

- Application environments
- Browser execution settings
- Base URLs
- Storage settings
- Database provider and connection
- Artefact retention rules
- Code-generation defaults

#### FR-ADM-002 Authentication Bootstrap Options

The system shall support configurable authentication bootstrap approaches such as:

- Manual login during recording
- Stored Playwright storage state
- Cookie import under controlled administration

#### FR-ADM-003 Auditability

Administrative changes shall be auditable with who, when, and what changed.

#### FR-ADM-004 Retention Policies

Administrators shall be able to configure retention policies for logs, screenshots, traces, generated artefacts, and stale replay diagnostics.

## 9. User Interface Requirements

### 9.1 Overall UI Structure

The Blazor Server application shall provide at least these primary work areas:

1. Recording workspace
2. Timeline and scenario editor
3. Generated code preview
4. Replay and diagnostics workspace
5. Administration area

### 9.2 Recording Workspace

The recording workspace shall display:

- Browser session status
- Start, pause, resume, and stop controls
- Current URL
- Live step feed
- Ability to flag importance or ignore noise
- Inline note entry

### 9.3 Timeline Workspace

The timeline workspace shall provide:

- Ordered list of steps
- Step type badges
- Locator confidence indicators
- Assertion suggestion indicators
- Variable classification editing
- Reorder and suppress controls

### 9.4 Code Preview Workspace

The code preview workspace shall provide:

- Generated C# preview
- File list or manifest view
- Warnings for low-confidence locators or assertions
- Regenerate action

### 9.5 Replay Workspace

The replay workspace shall provide:

- Replay status
- Step-by-step result stream
- Failure summary
- Locator diagnostics
- Screenshot and trace links
- Healing proposal review and approval actions

### 9.6 Responsiveness

The application shall be usable on standard desktop browser sizes and shall degrade gracefully on smaller widths, but desktop-first layout is acceptable for v1.

## 10. API and Integration Requirements

### 10.1 Internal API Surface

The ASP.NET Core backend shall expose internal or authenticated HTTP APIs sufficient to support:

- Recording lifecycle control
- Scenario CRUD
- Timeline editing
- Replay triggering
- Healing approval
- Artefact retrieval
- Administrative configuration

### 10.2 Real-Time Updates

The application shall provide real-time UI updates for long-running recording and replay operations. SignalR is the preferred approach.

### 10.3 Artefact Export

The system shall support export of generated assets into a chosen output directory structure that is suitable for committing to a repository or copying into another solution.

### 10.4 External Target Application Interaction

The platform shall treat the target application as an external system under test. It shall not require modifications to the target application, but it shall provide guidance when better accessibility or testability hooks would improve recording quality.

### 10.5 API Design Expectations

Backend APIs should:

- Use explicit resource identifiers
- Return actionable validation errors
- Support long-running operation status retrieval
- Avoid leaking secrets or raw sensitive payloads
- Be versionable from the outset

## 11. Configuration Requirements

### 11.1 Configuration Sources

The application shall support configuration from:

- App settings files
- Environment variables
- Secure secret providers or equivalent deployment-time injection
- Database-backed administrative settings where appropriate

### 11.2 Configurable Areas

The following areas shall be configurable:

- Database provider and connection string
- Artefact storage root or provider
- Browser launch defaults
- Headless defaults
- Base URL allowlists
- Session timeout and retention policies
- Masking rules
- Default generation profile
- Replay diagnostics level
- Authentication provider settings

### 11.3 Validation

Invalid required configuration shall fail fast at application startup or at the point of administrative save, depending on whether the setting is environment-level or runtime-editable.

## 12. Security and Privacy Requirements

### 12.1 Authentication and Access Control

The platform shall require authenticated access for non-public environments. Baseline role separation shall distinguish at least:

- Administrator
- Author
- Viewer

### 12.2 Secret Protection

Secrets shall never be written to plain logs, screenshots metadata, or generated code by default. This includes:

- Target application credentials
- Session cookies
- Tokens
- Secret variable values

### 12.3 Sensitive Data Masking

The system shall allow masking rules by field name, selector metadata, or manual marking. Masked values shall remain masked in:

- UI previews
- Diagnostic logs
- Stored event payloads
- Generation previews

### 12.4 Browser Session Isolation

Each recording or replay session shall execute in an isolated Playwright browser context unless explicitly configured otherwise by an administrator.

### 12.5 Audit Logging

The system shall maintain an audit trail for:

- Login and administrative access
- Scenario creation and approval
- Healing approval
- Artefact export
- Configuration changes

## 13. Observability and Diagnostics Requirements

### 13.1 Structured Logging

The backend shall emit structured logs with identifiers such as:

- Recording session id
- Scenario id
- Scenario version id
- Replay run id
- User id
- Browser context id

### 13.2 Operational Metrics

The system shall capture metrics useful for support and scaling, including:

- Active recordings
- Active replays
- Average recording duration
- Replay success rate
- Most common failure categories
- Healing proposal frequency
- Generation duration

### 13.3 Artefact Traceability

Screenshots, traces, DOM snapshots, and generated outputs shall remain traceable to the session, scenario version, or replay run that created them.

## 14. Verification and Testing Requirements

### 14.1 Product Test Strategy

The platform implementation shall include automated coverage for:

- Domain and inference rules
- Scenario validation
- Locator ranking
- Assertion suggestion rules
- Code-generation output shape
- Replay orchestration
- Healing evaluation
- Persistence mappings and migrations

### 14.2 Test Layers

At minimum, the repository should include:

- Unit tests for core business rules and model transformations
- Integration tests for Playwright coordination, persistence, and artefact storage
- End-to-end tests against controlled sample web applications

### 14.3 Regression Fixtures

The implementation should maintain representative sample applications or fixture pages that cover:

- Basic forms
- Grid interactions
- Modal dialogs
- Login flows
- Dynamic identifiers
- Toast-based success notifications

### 14.4 Generator Verification

Generated output shall be verified through snapshot-style approval tests or equivalent structural tests so template changes do not unintentionally degrade code readability or behaviour.

## 15. Non-Functional Requirements

### 15.1 Reliability

The platform shall favour deterministic execution. Generated tests and replay flows shall use Playwright waiting behaviour and retryable assertions rather than fixed delays except where no better alternative exists.

### 15.2 Performance

The system shall feel responsive for interactive authoring. Initial targets:

- Start a recording session within 10 seconds under normal local conditions
- Render timeline updates to the UI within 2 seconds of capture under normal load
- Generate code for a typical scenario of up to 100 meaningful steps within 15 seconds

### 15.3 Scalability

The v1 system shall support multiple concurrent users and background replays on a single server instance. The architecture shall allow future scale-out of non-UI background execution services if demand grows.

### 15.4 Maintainability

The implementation shall keep clear boundaries between:

- UI concerns
- Application services
- Playwright execution
- Scenario/inference logic
- Persistence
- Generation
- Healing

### 15.5 Testability

The system itself shall be designed for automated testing using unit, integration, and end-to-end coverage. The product shall not rely on untestable static global state for core flows.

### 15.6 Accessibility

Because the product explicitly depends on accessibility metadata from target applications, the platform UI should also model good practice. The Blazor UI should use accessible semantics, keyboard support, and meaningful announcements for long-running operations where practical.

## 16. Deployment and Operational Requirements

### 16.1 Host Model

The initial application shall run as an ASP.NET Core server-hosted web application with Blazor Server integrated into the host process or solution boundary.

### 16.2 Environment Profiles

The implementation shall support at least:

- Local developer execution
- Shared non-production environment
- Production-like server deployment

### 16.3 Packaging Direction

Desktop packaging through .NET MAUI Hybrid or Electron is explicitly a future option. The initial architecture shall therefore keep the backend and UI boundaries clean enough that later packaging can host the same application surfaces without re-implementing core recording, generation, or replay services.

### 16.4 Operational Safety

Operational processes shall account for:

- Browser process cleanup
- Expired artefact cleanup
- Failed replay recovery
- Application restarts during long-running operations

## 17. Suggested Repository Architecture

The application should be introduced into this repository as a new vertical slice rather than by mutating the existing Symphony specification. A suggested project layout is:

```text
/src
  /TestMining.Platform.Host            (ASP.NET Core host, Blazor Server UI, auth, APIs)
  /TestMining.Platform.Core            (domain models, interfaces, scenario contracts)
  /TestMining.Platform.Recording       (Playwright launch, injected recorder, capture coordination)
  /TestMining.Platform.Analysis        (DOM intelligence, inference, locator ranking, assertions)
  /TestMining.Platform.Generation      (Roslyn/Scriban generation)
  /TestMining.Platform.Replay          (replay execution and diagnostics)
  /TestMining.Platform.Healing         (deterministic healing services)
  /TestMining.Platform.Persistence     (EF Core, PostgreSQL/SQL Server adapters, repositories)
  /TestMining.Platform.Artefacts       (artefact storage abstraction)
/tests
  /TestMining.Platform.Core.Tests
  /TestMining.Platform.Integration.Tests
  /TestMining.Platform.E2E.Tests
/docs
  concept.md
  requirements.md
```

## 18. Delivery Phasing

### 18.1 Phase 1: Core Proof

Phase 1 shall target:

- Recording of navigation, click, fill, select, checkbox, and simple assertion-relevant actions
- Timeline UI
- Locator ranking
- Basic scenario persistence with immutable scenario version creation
- C# Playwright generation
- Basic replay

### 18.2 Phase 2: Robustness

Phase 2 shall add:

- Assertion inference
- Variable extraction
- Regex and partial matching
- Scoped locators
- Replay diagnostics
- Screenshot capture per important step

### 18.3 Phase 3: Product Hardening

Phase 3 shall add:

- Login and session bootstrap management
- Export packaging
- Trace viewer integration
- Advanced healing review workflow and approval ergonomics
- Scenario history comparison and version-diff views

### 18.4 Phase 4: AI Assistance

Phase 4 may add:

- Scenario naming suggestions
- Assertion suggestion assistance
- Healing proposal assistance
- Page object refactoring suggestions

## 19. Acceptance Criteria

The implementation shall be considered to satisfy this specification only when all of the following are demonstrably true:

1. A user can record a real browser workflow through the UI and the system persists a structured scenario.
2. The scenario can be reviewed, edited, and approved without direct database manipulation.
3. The system generates readable C# Playwright artefacts from the approved scenario.
4. The generated or scenario-derived replay executes through Playwright and produces useful diagnostics.
5. Locator candidates are ranked and persisted rather than reduced to a single opaque selector.
6. Sensitive values can be masked and remain masked across previews and logs.
7. Assertions are outcome-oriented and can be approved or rejected before generation.
8. A locator drift failure can produce a deterministic healing proposal with evidence.
9. The application runs on ASP.NET Core with a Blazor Server frontend and a PostgreSQL or SQL Server-backed persistence layer.
10. Generated output remains reproducible from scenario data and generation settings.

## 20. Risks and Constraints

### 20.1 Known Challenging Target Apps

The product shall acknowledge reduced reliability for:

- Canvas-heavy applications
- Highly virtualised lists or grids
- Poorly accessible React or SPA interfaces
- UIs with unstable text and random generated IDs
- Drag-and-drop heavy workflows
- Rich text editors

### 20.2 Product Response to Poor Testability

When recording quality is degraded by target-application design, the platform should surface actionable recommendations such as:

- Add stable `data-testid` attributes
- Improve ARIA labels
- Expose predictable success messages
- Add stable row identifiers
- Reduce dependence on random element IDs

## 21. Open Design Decisions

The following decisions remain implementation-level choices unless later locked by an approved architecture note:

- Whether Roslyn, Scriban, or a hybrid is the default generator engine
- Whether artefact storage is local filesystem first or abstracted for object storage from day one
- Whether replay executes in-process or via a dedicated background worker boundary
- Which authentication provider is used for the host application
- Which database provider is used as the primary development default

## 22. Final Requirement Statement

This repository’s new application shall be a C#-based semantic test mining platform built on ASP.NET Core, Blazor Server, Microsoft.Playwright for .NET, and PostgreSQL or SQL Server. It shall record browser interactions, infer structured scenarios, rank resilient locators, generate maintainable Playwright C# tests, replay them with strong diagnostics, and support deterministic healing while keeping structured scenarios as the enduring source of truth.
