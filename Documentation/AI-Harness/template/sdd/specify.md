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

## Sizing

| Field | Decision |
|-------|----------|
| Size | Small / Medium / Large / Complex |
| Rationale | ... |
| Required phases | Specify / Design / Tasks / Execute / Verify |
| Escalation triggers | ... |

## Assumptions And Constraints

- ...

## Open Questions

- ...

## Domain Language

List relevant terms and confirm they match `Documentation/Architecture/Domain_Overview_Business_Rules.md`.

- ...

## Legacy Behavior

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
