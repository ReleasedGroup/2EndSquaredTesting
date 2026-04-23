# Test Mining Platform Security Plan

## 1. Purpose

This document defines the implementation security plan for the platform described in [docs/requirements.md](./requirements.md), with emphasis on Sections 12, 16.4, 19, 22.3, and 23.

This application handles sensitive material by construction, including cookies, storage state, captured inputs, target URLs, generated artefacts, and operator identities. Security controls must therefore be built into the default architecture rather than added later.

## 2. Security Objectives

1. Prevent unauthorised access to the platform and its artefacts.
2. Prevent secret leakage to logs, previews, diagnostics, and generated code.
3. Constrain browser automation to approved target URLs and isolated sessions.
4. Preserve auditable traceability for privileged and source-of-truth-changing actions.
5. Keep deterministic product features available even when optional AI or convenience features are disabled.

## 3. Requirement Mapping

Primary source requirements:

- Section 11 configuration validation
- Section 12.1 through 12.7
- Section 13 observability
- Section 16.4 operational safety
- Section 19 acceptance criteria 6 and 11
- Section 22.3 CI secret scanning
- Section 23.1 prohibited actions

## 4. Trust Boundaries

Primary trust boundaries in v1:

1. Browser under test vs platform host
2. Browser recorder script vs server-side recording engine
3. Blazor UI connection vs authenticated backend APIs
4. Application services vs persistence and artefact storage
5. Platform operators vs administrator-only configuration surfaces
6. Generated output vs canonical scenario storage

Each cross-boundary interaction should be explicit, authenticated where applicable, validated, and logged without leaking sensitive payloads.

## 5. Authentication and Authorization

### 5.1 Authentication

The platform shall require authenticated access outside public local development cases, consistent with Section 12.1.

Recommended v1 approach:

- local development: developer-friendly local auth toggle
- shared environments: external identity provider via ASP.NET Core authentication
- production-like environments: enforced external identity provider and secure cookie/session configuration

The local developer environment should still exercise the real application shell and the core security-sensitive code paths wherever practical. Local convenience mode should reduce friction, not create a separate unrepresentative application path.

### 5.2 Authorization

Minimum role model:

- `Administrator`
- `Author`
- `Viewer`

Control expectations:

- `Viewer` cannot start recording, edit scenarios, generate, replay, approve healing, or export sensitive artefacts.
- `Author` can perform workflow actions but cannot change environment security policy.
- `Administrator` can manage allow-lists, masking rules, retention, auth bootstrap settings, and key rotation workflows.

## 6. Sensitive Data Classification

Data should be classified at minimum as:

- Public operational metadata
- Internal non-sensitive workflow data
- Sensitive captured data
- Secret authentication material

Examples that must be treated as sensitive or secret:

- credentials
- cookies
- Playwright storage state
- tokens
- manually marked sensitive input values
- scenario variables classified as `Sensitive`

## 7. Core Security Controls

### 7.1 Target URL Allow-List Enforcement

Required by Section 12.4.

Control plan:

- store allow-list entries per environment
- validate requested target URL server-side before browser launch
- reject launch if URL does not match the selected environment policy
- audit every allow-list change

Implementation note:

This must happen before any Playwright browser context is created.

### 7.2 Browser Session Isolation

Required by Section 12.6 and `FR-REP-005`.

Control plan:

- create isolated browser contexts for recording and replay by default
- never share mutable session state across runs unless explicitly configured
- clean up contexts, temp files, traces, and storage state after completion or failure

### 7.3 Secret Protection and Masking

Required by Sections 12.2, 12.3, 12.5 and `FR-REC-007`.

Control plan:

- apply masking rules before persistence for previewable/raw event fields
- never log sensitive raw values
- never render secret values in UI previews
- never emit sensitive plaintext into generated source
- replace secret-backed values with secure variable references in generator output
- if a field is reclassified as sensitive after recording, future previews and regenerated output must use the updated masking rule

### 7.4 Encryption at Rest

Required by Section 12.5.

Encrypt at rest:

- Playwright storage state
- cookies
- stored authentication material
- sensitive scenario variables
- any captured raw payload fields configured for encryption

Key management expectations:

- keys loaded from secure deployment-time secret provider
- keys never committed to source control
- support controlled key rotation with re-encryption job
- re-encryption operations must be auditable and scoped so failed runs can be resumed safely

### 7.5 Audit Logging

Required by Section 12.7 and `FR-ADM-003`.

Audit at minimum:

- logins and access failures
- recording start/stop/cancel
- scenario version creation
- assertion approval/rejection where material
- generation export
- replay trigger
- healing approval/rejection
- configuration changes
- retention cleanup deletions
- allow-list changes
- authentication bootstrap configuration changes

## 8. Secure Design by Capability

### 8.1 Recording

Threats:

- recording against disallowed targets
- secret capture in raw payloads
- recorder transport spoofing or malformed payloads

Controls:

- allow-list gate
- strict payload validation
- masking before persistence
- authenticated session ownership checks

### 8.2 Scenario Authoring

Threats:

- exposure of masked values in editing surfaces
- unauthorized source-of-truth changes

Controls:

- field-level masking in inspector and previews
- optimistic concurrency
- role-based edit permissions
- audit trail for version creation

### 8.3 Generation

Threats:

- secret leakage to generated code
- export of unauthorised artefacts

Controls:

- generator redaction rules
- manifest review before export
- export authorization checks
- secret scanning in CI for generated fixtures/templates

### 8.4 Replay and Healing

Threats:

- reusing mutable session state unsafely
- healing proposals altering source of truth without approval
- AI suggestions obscuring deterministic evidence

Controls:

- isolated replay context
- approval gate for persisted healing
- explicit provenance and evidence display
- AI disabled or advisory only by policy

## 9. Logging and Telemetry Restrictions

Structured logs must include correlation identifiers from Section 13.1, but must not contain:

- tokens
- cookies
- storage state blobs
- secret values
- full sensitive captured field payloads

Recommended practice:

- structured properties for IDs and categories only
- masked previews when troubleshooting requires representative value shape
- explicit log sanitization tests
- do not place secret-bearing values in exception messages or validation summaries that may later be logged

## 10. Secure Configuration Plan

Configuration controls:

- fail startup on missing required connection strings, auth config, or key references
- validate admin-managed policies before save
- separate secret configuration from ordinary app settings
- prevent unauthorised edits to allow-lists, masking policies, or retention policy
- distinguish environment-level immutable configuration from runtime-editable administrative settings

## 11. CI/CD Security Controls

Required by Section 22.3 and 23.3.

Pipeline controls:

- dependency vulnerability scanning
- secret scanning on diffs
- tests for masking and encryption rules
- policy/lint checks for retention and security-sensitive configuration

Recommended additions:

- SBOM generation
- container image scanning if container deployment is adopted
- restricted deployment secrets by environment

## 12. Security Testing Plan Summary

Security-specific tests should cover:

- allow-list rejection
- sensitive-field masking at record and preview time
- encrypted persistence of storage state and sensitive variables
- authorization failures for admin-only actions
- audit event creation for privileged actions
- replay isolation guarantees
- healing approval enforcement

Detailed testing ownership is defined in [docs/testing-plan.md](./testing-plan.md).

## 13. Incident and Operational Response

Operational readiness should include:

- ability to revoke or rotate encryption keys
- ability to disable recording or replay by environment
- audit review of recent exports and configuration changes
- retention cleanup monitoring
- procedure for purging incorrectly retained sensitive artefacts

## 14. Non-Negotiable Security Rules

The following must never be relaxed in v1:

1. No secret values in logs, previews, or generated code.
2. No recording or replay outside the configured allow-list.
3. No persisted healing changes without explicit approval.
4. No runtime-critical dependency on AI suggestions.
5. No key material in source control.
