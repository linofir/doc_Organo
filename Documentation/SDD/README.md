# Feature SDDs

This folder contains feature-specific Spec-Driven Design artifacts for Doc Organo.

Feature SDDs are created only when a feature enters retrospective calibration, forward SDD planning, implementation, verification, or reporting. Do not create placeholder feature folders before the work exists.

## Folder Structure

Use one folder per feature:

```text
Documentation/SDD/<feature-slug>/
├── specify.md
├── design.md
├── tasks.md
├── verification.md
└── reports/
    ├── session-handoff.md
    └── feature-report.md
```

## Artifact Ownership

- `specify.md` owns feature scope, requirements, acceptance criteria, assumptions, constraints, and sizing.
- `design.md` owns the feature-level technical approach, risks, ADR candidates, reuse analysis, and integration impact.
- `tasks.md` owns executable implementation slices, dependencies, requirement mapping, and expected verification evidence.
- `verification.md` records Verifier output for the feature. It does not replace the verifier skill.
- `reports/` contains lightweight communication artifacts. Reports do not replace State, SDD, Verifier, ADRs, PM, or RoadMap.

## Language

All generated SDD artifacts, verification handoffs, reports, and lessons learned should be written in English.

## Current Boundary

| Feature | Folder | Phase |
|---------|--------|-------|
| Paciente SQL Stabilization | `paciente-sql-stabilization/` | Refinement complete — ready for Execute |

ProntuarioRepository forward SDD artifacts are not created yet.
