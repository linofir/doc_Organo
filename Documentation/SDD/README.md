# Feature SDDs

This folder contains feature-specific Spec-Driven Design artifacts for Doc Organo.

Feature SDDs are created only when a feature enters retrospective calibration, forward SDD planning, implementation, verification, or reporting. Do not create placeholder feature folders before the work exists.

## Authoritative Templates

**SDD structure is defined by templates**, not by existing feature folders:

- `Documentation/AI-Harness/template/sdd/specify.md`
- `Documentation/AI-Harness/template/sdd/design.md`
- `Documentation/AI-Harness/template/sdd/tasks.md`
- `Documentation/AI-Harness/template/verification/verification.md`
- `Documentation/AI-Harness/template/reporting/` and `template/governance/governance-improvement-plan.md`
- `Documentation/AI-Harness/Harness-Design/sdd-pilot-report-governance.md` (pilot report rules)
- `Documentation/AI-Harness/Harness-Design/template-architecture.md`

## Folder Structure

Use one folder per feature:

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

## Artifact Ownership

- `specify.md` owns feature scope, requirements, acceptance criteria, assumptions, constraints, and sizing.
- `design.md` owns the feature-level technical approach, risks, ADR candidates, reuse analysis, and integration impact.
- `tasks.md` owns executable implementation slices, dependencies, requirement mapping, and expected verification evidence.
- `verification.md` records Verifier output for the feature. It does not replace the verifier skill.
- `teacher-guide.md` owns post-Reporting pedagogical content when generated. See knowledge-strategy.
- `reports/` contains lightweight communication artifacts. Reports do not replace State, SDD, Verifier, ADRs, PM, or RoadMap.
- `reports/sdd-pilot-report-v*.md` consolidates harness calibration evidence for a feature pilot. See [sdd-pilot-report-governance.md](../AI-Harness/Harness-Design/sdd-pilot-report-governance.md).
- `reports/governance-improvement-plan.md` turns pilot evidence into Wave 1/2/3 harness execution backlog when calibration is warranted.

## Language

All generated SDD artifacts, verification handoffs, reports, and lessons learned should be written in English.

## Pre-Calibration Reference

| Feature | Folder | Status |
|---------|--------|--------|
| Paciente SQL Stabilization | `paciente-sql-stabilization/` | Verified — first full lifecycle pilot; [pilot report v1.0](paciente-sql-stabilization/reports/sdd-pilot-report-v1.0.md) |
| Atendimento Minimal SQL Stabilization | `atendimento-minimal-sql-stabilization/` | Verified — forward SDD #2; [pilot report v0.6](atendimento-minimal-sql-stabilization/reports/sdd-pilot-report-v0.6.md) |
| Prontuario SQL Stabilization | `prontuario-sql-stabilization/` | Research complete — [pilot report v0.1](prontuario-sql-stabilization/reports/sdd-pilot-report-v0.1.md); Specify next |

Do **not** copy Paciente SDD structure or Required phases as the template for new features. Use template files under `Documentation/AI-Harness/template/sdd/`.
