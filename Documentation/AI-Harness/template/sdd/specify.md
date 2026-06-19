# Specify - [Feature Name]

> Copy this template to `Documentation/SDD/<feature-slug>/specify.md`.
> Generated SDD artifacts should be written in English.

## Context

Briefly describe why this feature or calibration exists now.

- PM item:
- Product or domain source:
- Current operational source: `Documentation/State.md`
- Related SDD, ADR, technical, or architecture docs:

## Problem

Describe the current pain, risk, missing behavior, migration gap, or governance gap.

## Goals

- [ ] `REQ-001` - ...
- [ ] `REQ-002` - ...

Use simple requirement IDs for Medium, Large, and Complex work when traceability helps verification.

## Out Of Scope

- ...

## Users And Scenarios

| Actor | Scenario | Outcome |
|-------|----------|---------|
| ... | ... | ... |

## Acceptance Criteria

- [ ] `REQ-001` - ...
- [ ] `REQ-002` - ...

## Execution Prerequisites

Document infrastructure, credentials, and baseline evidence before Execute. Required for Large and Complex work.

| Prerequisite | Status | Notes |
|--------------|--------|-------|
| Docker SQL Server (`docorgano-sql`) | Required / N/A | ... |
| `SA_PASSWORD` available | Required / N/A | ... |
| `DOCORGANO_TEST_CONNECTION` when SQL integration tests apply | Required / N/A | ... |
| Baseline `dotnet build` | Pass / Not run | ... |
| Baseline `dotnet test` count | ... tests | Capture count before Execute for measurable progress |

Cross-reference `Documentation/Technical/runbook.md` for local setup steps.

### Credential Probe

Run before Execute when SQL integration tests or local SQL gates apply. Record results in Execution Prerequisites.

| Probe step | Pass criteria | Failure action |
|------------|---------------|----------------|
| Repo-root `.env` exists with `SA_PASSWORD` | File present and non-empty | Copy from `.env.example`; align password with Docker volume |
| `scripts/load-env.ps1` loads variables | `$env:SA_PASSWORD` set in shell | Run probe from repo root |
| Docker container `docorgano-sql` running | `docker compose ps` shows healthy | `docker compose up -d` |
| SQL connect with resolved credentials | `SqlConnectionResolver` or `dotnet test` filter `Sql` passes | If login fails, password mismatch — see runbook volume note; do not assume "Docker unreachable" |
| Database migrated | `dotnet ef database update --project DocAPI/DocAPI.csproj` succeeds | Apply migrations before Execute |
| Optional one-command check | `.\scripts\sql-integration-test.ps1` passes or skips with credential hint only when `.env` missing | Fix credentials before treating integration as blocked |

If the probe fails on **login** but Docker is running, treat as **credential misalignment**, not infrastructure unavailability.

## Backend Stabilization

Use when this feature is a backend aggregate stabilization or SQL migration vertical slice.

- Frontend validation in scope: Yes / No
- Default for backend stabilization: **No** — exclude Blazor smoke and UI contract work unless explicitly required
- Frontend/API drift follow-up: ...

## Sizing

| Field | Decision |
|-------|----------|
| Size | Small / Medium / Large / Complex |
| Rationale | ... |
| Migration vertical note | For first SQL migration vertical pilots, prefer **Large** even when code effort appears modest |
| Required phases | Specify / Design / Tasks / SDD Pre-Execution Review / Execute / Verify / Documentation Follow-Up / Reporting / Teacher Guide when warranted |
| Escalation triggers | ... |

## Assumptions And Constraints

- ...

## Open Questions

Record unresolved decisions here. Resolve during SDD Pre-Execution Review or escalate.

| ID | Question | Options | Decision | Date | Owner |
|----|----------|---------|----------|------|-------|
| DQ-001 | ... | ... | Open / Resolved | | |

- ...

## Domain Language

List relevant terms and confirm they match `Documentation/Architecture/Domain_Overview_Business_Rules.md`.

- ...

## Legacy Behavior

**Recommended for Legacy-behavior migration SDDs; required when material Legacy behavior is in scope.**

Use this section when existing Legacy behavior is being preserved, adapted, or intentionally abandoned.

| Behavior | Source | Decision | Notes |
|----------|--------|----------|-------|
| ... | `DocAPI/Legacy/_LegacySheetsDb/...` | Preserve / Adapt / Abandon / Not applicable | ... |

## Security And PHI

State whether the feature touches patient identity, clinical data, files, logs, prompts, tests, auth assumptions, API exposure, or secrets.

- Security/PHI review needed: Yes / No
- Rationale:

## Initial Verification Expectations

List expected evidence at the requirement level. The Verifier will select final gates later.

| Requirement | Expected evidence |
|-------------|-------------------|
| `REQ-001` | ... |

## References

- `Documentation/State.md`
- `AGENTS.md`
- `Documentation/Product/PM_DocOrgano.md`
- `Documentation/AI-Harness/Harness-Design/sdd-operational.md`
