# Feature SDDs

This folder contains feature-specific Spec-Driven Design artifacts for Doc Organo.

Feature SDDs are created only when a feature enters retrospective calibration, forward SDD planning, implementation, verification, or reporting. Do not create placeholder feature folders before the work exists.

## Authoritative Templates

**SDD structure is defined by templates**, not by existing feature folders:

- `Documentation/AI-Harness/template/sdd/specify.md`
- `Documentation/AI-Harness/template/sdd/design.md`
- `Documentation/AI-Harness/template/sdd/tasks.md`
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
    └── feature-report.md
```

## Artifact Ownership

- `specify.md` owns feature scope, requirements, acceptance criteria, assumptions, constraints, and sizing.
- `design.md` owns the feature-level technical approach, risks, ADR candidates, reuse analysis, and integration impact.
- `tasks.md` owns executable implementation slices, dependencies, requirement mapping, and expected verification evidence.
- `verification.md` records Verifier output for the feature. It does not replace the verifier skill.
- `teacher-guide.md` owns post-Reporting pedagogical content when generated. See knowledge-strategy.
- `reports/` contains lightweight communication artifacts. Reports do not replace State, SDD, Verifier, ADRs, PM, or RoadMap.

## Language

All generated SDD artifacts, verification handoffs, reports, and lessons learned should be written in English.

## Pre-Calibration Reference

| Feature | Folder | Status |
|---------|--------|--------|
| Paciente SQL Stabilization | `paciente-sql-stabilization/` | **Historical pre-calibration reference** — pilot complete; structural drift from current templates is expected |

Do **not** copy Paciente SDD structure or Required phases as the template for new features. Use template files under `Documentation/AI-Harness/template/sdd/`.

Prontuario forward SDD will be the first post-calibration structural instance when that work begins.
