# Template Architecture

## Purpose

This document defines the template ecosystem for Doc Organo SDD, verification, and reporting artifacts. It establishes ownership, dependencies, creation order, validation order, and the canonical folder structure for Feature SDDs.

All generated governance documents, templates, SDD artifacts, verification handoffs, session handoffs, feature reports, and lessons learned should be written in English.

## Canonical Feature SDD Location

Feature SDDs live under:

```text
Documentation/SDD/
```

Each feature should use a stable folder name:

```text
Documentation/SDD/<feature-slug>/
```

Recommended feature folder contents:

```text
Documentation/SDD/<feature-slug>/
├── specify.md
├── design.md
├── tasks.md
├── verification.md
├── teacher-guide.md
└── reports/
    ├── session-handoff.md
    ├── feature-report.md
    ├── sdd-pilot-report-v*.md
    └── governance-improvement-plan.md
```

`specify.md`, `design.md`, and `tasks.md` are SDD artifacts. `verification.md` is the verifier handoff/output for the feature. `teacher-guide.md` is the knowledge-transfer artifact when generated. `reports/` contains communication artifacts and must not replace State, SDD, or Verifier authority.

**Pre-calibration reference:** Paciente SQL Stabilization SDD predates post-pilot template calibration. Use it as a behavioral reference, not as a structural conformance example. Prontuario forward SDD is the first post-calibration consumer.

Do not create feature folders until a feature actually enters retrospective calibration, forward SDD, implementation, verification, or reporting.

## Template Families

| Family | Template path | Output location | Owner |
|--------|---------------|-----------------|-------|
| SDD Specify | `Documentation/AI-Harness/template/sdd/specify.md` | `Documentation/SDD/<feature-slug>/specify.md` | SDD |
| SDD Design | `Documentation/AI-Harness/template/sdd/design.md` | `Documentation/SDD/<feature-slug>/design.md` | SDD |
| SDD Tasks | `Documentation/AI-Harness/template/sdd/tasks.md` | `Documentation/SDD/<feature-slug>/tasks.md` | SDD |
| Verification | `Documentation/AI-Harness/template/verification/verification.md` | `Documentation/SDD/<feature-slug>/verification.md` | Verifier |
| Session Handoff | `Documentation/AI-Harness/template/reporting/session-handoff.md` | `Documentation/SDD/<feature-slug>/reports/session-handoff.md` or a session-specific reporting location when needed | Reporting |
| Feature Report | `Documentation/AI-Harness/template/reporting/feature-report.md` | `Documentation/SDD/<feature-slug>/reports/feature-report.md` | Reporting |
| SDD Pilot Report | `Documentation/AI-Harness/template/reporting/sdd-pilot-report.md` | `Documentation/SDD/<feature-slug>/reports/sdd-pilot-report-v*.md` | Harness calibration |
| Governance Improvement Plan | `Documentation/AI-Harness/template/governance/governance-improvement-plan.md` | `Documentation/SDD/<feature-slug>/reports/governance-improvement-plan.md` | Harness calibration |
| Teacher Guide | `Documentation/AI-Harness/template/knowledge/teacher-guide.md` | `Documentation/SDD/<feature-slug>/teacher-guide.md` | Knowledge transfer |

## Ownership

SDD templates own reusable structure for feature scope, design, and task planning. They do not own verification decisions, reporting narrative, State updates, or ADR acceptance.

The verification template owns the structure of a verification handoff. It does not select gates by itself; the Verifier selects gates for each change.

Reporting templates own continuity and communication structure. They do not own current truth, feature scope, gate decisions, or durable architecture decisions.

The governance improvement plan template owns harness calibration backlog structure after a feature pilot. It does not implement governance changes by itself; Wave execution updates templates, skills, rules, and Harness Design docs through Documentation Update routing.

## Dependencies

Template dependencies:

- SDD templates depend on SDD Operational Governance.
- Verification template depends on Verification Governance and the verifier skill.
- Reporting templates depend on Reporting Strategy.
- Governance improvement plan template depends on Reporting Strategy, CONTRIBUTING-AI Harness Calibration Workflow, and pilot report / feature report evidence.
- Feature SDD folder structure depends on ADR-003 documentation taxonomy and the accepted `Documentation/SDD/` destination.
- Template updates after pilot evidence depend on Documentation Update routing.

## Creation Order

Create v1 templates in this order:

1. SDD templates: `specify.md`, `design.md`, `tasks.md`.
2. Verification template: `verification.md`.
3. Reporting templates: `session-handoff.md`, `feature-report.md`, `sdd-pilot-report.md`.
4. Governance calibration template: `governance-improvement-plan.md` — after second pilot validates the feedback loop (GOV-10).
5. Knowledge template: `teacher-guide.md`.

The v1 templates should be good enough to run the first pilot, not final. They should be adjusted after Paciente retrospective calibration and the ProntuarioRepository forward SDD pilot reveal real friction.

## Validation Order

Validate templates in this order:

1. Check against SDD Operational Governance for phase responsibilities, sizing, SDD Pre-Execution Review, traceability, Definition of Done, ADR evaluation, Execution Prerequisites, Backend Stabilization Rule, and brownfield expectations.
2. Check against Verification Governance for verifier handoff quality, Environment-Dependent Evidence policy, and residual-risk boundaries.
3. Check against Reporting Strategy for State/Verifier/SDD boundary protection and Reporting-after-Documentation-Follow-Up ordering.
4. Use Paciente retrospective calibration as pre-calibration reference; do not retrofit Paciente SDD folder to match updated templates.
5. Use the ProntuarioRepository forward SDD pilot to test full Specify -> Design -> Tasks -> SDD Pre-Execution Review -> Execute -> Verify -> Documentation Follow-Up flow.
6. Review pilot findings before updating templates, rules, skills, or governance.

## Current Execution Boundary

This preparation execution creates governance and template v1 artifacts only.

It does not implement Paciente retrospective calibration.
It does not implement the ProntuarioRepository forward SDD pilot.
It does not create feature-specific SDD folders for Paciente or Prontuario.
It does not implement application code.

## Template Change Policy

Templates should remain practical and short. Add structure only when it prevents a real failure mode:

- Lost requirements.
- Missing verification handoff.
- Unclear residual risk.
- Undocumented ADR candidate.
- Unrouted documentation follow-up.
- Legacy behavior preserve/adapt/abandon ambiguity.
- Reporting that duplicates State, SDD, or Verifier outputs.
- Harness calibration findings with no routed backlog or wave plan.

After each pilot, update templates only through Documentation Update routing.
