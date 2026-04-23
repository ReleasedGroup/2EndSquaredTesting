# Test Mining Platform

This repository is being used to build a C#-based semantic test mining platform that records browser interactions, converts them into structured scenarios, and generates maintainable Playwright tests with replay and healing support.

The current source of truth for the planned product is [docs/requirements.md](/mnt/c/Users/kenne/Desktop/ReleasedGroup/2EndSquaredTesting/docs/requirements.md). The concept document that informed it is [docs/concept.md](/mnt/c/Users/kenne/Desktop/ReleasedGroup/2EndSquaredTesting/docs/concept.md).

In addition to being production-capable, the application is expected to support a local, production-like developer environment so changes can be exercised safely before they are pushed. The Blazor Server UI is also expected to be built early enough that major workflows can be visually tested during development rather than only through backend or CLI flows.

## Product Summary

The application described in this repository is not intended to be a raw click recorder. It is intended to be a semantic test mining and stabilisation platform that:

- records real user workflows in a browser
- captures DOM, accessibility, navigation, and runtime context around meaningful actions
- transforms raw events into structured, intent-rich scenarios
- infers resilient locators, variable data strategies, and outcome-oriented assertions
- generates readable C# Playwright tests and supporting helpers
- replays generated scenarios and provides deterministic healing when selectors drift

The core design rule is that structured scenarios are the source of truth. Generated code is a derived artefact and must remain reproducible from scenario data and generation settings.

## Intended Stack

The requirements currently define this implementation model:

- Backend: ASP.NET Core
- Frontend: Blazor Server
- Database: PostgreSQL or SQL Server
- Browser automation: Microsoft.Playwright for .NET
- Code generation: Roslyn or Scriban templates
- Desktop packaging later: .NET MAUI Hybrid or Electron wrapper if needed

## Planned Capabilities

The platform is expected to include these major areas:

- Recording engine for guided browser-session capture
- DOM and accessibility analysis
- Scenario authoring and timeline review
- Locator ranking and assertion suggestion
- C# Playwright code generation
- Replay diagnostics
- Deterministic healing workflows
- Persistent storage for sessions, scenarios, artefacts, and replay history
- Administrative configuration for environments, browser settings, and generation defaults

## Repository Guidance

This repository currently contains existing Symphony-related files and scaffolding. Those files are being retained in the repository because Symphony is a tool used within this project context.

Symphony guidance for users and agents:

- Treat Symphony-related files as retained tool files, not as the application being described in `docs/requirements.md`.
- Do not rewrite, repurpose, or delete Symphony files unless a task explicitly asks for Symphony/tooling work.
- Keep Symphony files in the repository even while building the new application.
- New product work should align to `docs/requirements.md`, not the legacy Symphony README content that previously occupied this file.

Examples of retained Symphony-related content include:

- `SPEC.md`
- `IMPLEMENTATION_PLAN.md`
- `WORKFLOW.md`
- `src/Symphony.*`
- `tests/Symphony.*`
- `symphony_docs/`

## Expected Solution Direction

A likely repository shape for the new application is described in the requirements document and includes logical areas such as:

- host and Blazor Server UI
- core domain and scenario contracts
- recording and Playwright orchestration
- analysis and inference
- code generation
- replay and healing
- persistence and artefact storage
- automated tests

## Current Status

The repository is in a planning and transition stage:

- the requirements specification exists in `docs/requirements.md`
- the concept source exists in `docs/concept.md`
- the new application implementation may coexist with retained Symphony tooling content during development

## Working Rule

If there is a conflict between older Symphony-oriented repository documentation and the new product direction, treat `docs/requirements.md` as the authoritative description of the application to be built, while still preserving Symphony-related files as tooling assets unless explicitly instructed otherwise.
