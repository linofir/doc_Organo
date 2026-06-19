# Contributing With AI — Doc Organo

This guide describes the contribution workflow. It does not replace `AGENTS.md`, rules, skills, SDD artifacts, ADRs, or `Documentation/State.md`.

## Authority

- ADRs own durable architecture decisions.
- Architecture and Technical docs explain the durable model.
- `Documentation/State.md` owns current operational truth.
- Active SDD owns feature scope, design, tasks, and verification expectations.
- Rules are concise guardrails; skills are repeatable workflows.
- Review prompts are sensors; the verifier orchestrates which sensors and gates apply.

## Lifecycle

The Harness supports the following lifecycle. SDD phase detail is authoritative in `Documentation/AI-Harness/Harness-Design/sdd-operational.md`.

```text
Research -> Plan -> SDD when required
  (Specify -> Design -> Tasks -> SDD Pre-Execution Review for Large/Complex)
  -> Execute -> Verify -> Documentation Follow-Up -> Reporting -> Teacher Guide
  -> State / ADR / Documentation Updates
```

A session may start at any phase.

Do not repeat completed phases when authoritative artifacts already exist. Use existing research, SDDs, ADRs, plans, and documentation as inputs and continue from the appropriate lifecycle stage.

### Phase Ownership (SDD-Backed Work)

| Phase | Owner | Not terminal because |
|-------|-------|----------------------|
| SDD Pre-Execution Review | SDD Review session | Execute must not start until review exit criteria pass |
| Execute | Implementation session | Verify evaluates evidence; Execute does not self-certify |
| Verify | Verifier (`.cursor/skills/verifier/SKILL.md`) | Follow-Up, Reporting, and Teacher Guide remain |
| Documentation Follow-Up | Documentation Update (`.cursor/skills/documentation-update/SKILL.md`) | Reporting must reflect synchronized operational truth |
| Reporting | Reporting artifacts (`feature-report.md`, `session-handoff.md`) | Teacher Guide depends on finalized reporting inputs |
| Teacher Guide | `not-a-teacher` skill per knowledge-strategy | Final learning artifact when criteria apply |

Verify is **not** the end of SDD-backed feature work.

## Before Coding

1. Read `Documentation/State.md` for current branch truth.
2. Read `AGENTS.md` for stable bootstrap and routing.
3. Use `.cursor/skills/doc-organo-context/SKILL.md` when a session needs guided orientation.
4. Load task-specific docs from `Documentation/AI-Harness/documentation-index.md`.

## Research

Research should produce durable findings or a short handoff, not depend on long chat history.

Research should not be repeated when authoritative findings already exist. Existing research artifacts should be treated as inputs to planning, SDD creation, implementation, or verification unless new information is required.

Use targeted code reads and relevant skills such as `.cursor/skills/codebase-decomposition/SKILL.md`.

Canonical research context lives in `Documentation/AI-Harness/research/AI-Research.md` and `Documentation/AI-Harness/research/playbook.md`.

Generated SDD artifacts, verification handoffs, reports, and lessons learned should be written in English.

## Plan And SDD

Use SDD when the change is large, cross-layer, clinical/security-sensitive, persistence-related, API-contract-changing, Legacy-behavior-porting, or likely to require an ADR.

An existing approved SDD should be treated as the authoritative feature plan unless new requirements or risks justify revisiting the design.

SDD templates live in `Documentation/AI-Harness/template/sdd/`. **Template files are authoritative.** Feature SDD folders under `Documentation/SDD/<feature-slug>/` are instances — Paciente SQL Stabilization is a historical pre-calibration reference, not a structural template.

Feature folders should keep:

- `specify.md` for feature scope and acceptance criteria.
- `design.md` for technical approach, risks, and ADR trigger analysis.
- `tasks.md` for implementation steps and verification expectations.
- `verification.md` for Verifier output.
- `teacher-guide.md` when knowledge strategy criteria apply.
- `reports/session-handoff.md` for session continuation.
- `reports/feature-report.md` after Documentation Follow-Up.
- `reports/sdd-pilot-report-v*.md` for harness calibration retrospectives.
- `reports/governance-improvement-plan.md` when calibration findings drive harness updates.

## SDD Pre-Execution Review

For Large and Complex work, complete SDD Pre-Execution Review after Tasks and before Execute.

Resolve API contract ambiguities, validate Execution Prerequisites, confirm Backend Stabilization scope when applicable, and declare Execute readiness. See `sdd-operational.md` SDD Pre-Execution Review entry and exit criteria.

## Execute

Keep implementation scoped to the accepted plan or active SDD. Use rules for guardrails and skills for workflows. Do not move workflow procedures into rules or stable project facts into skills.

Execute does not produce verification artifacts, reporting, project documentation updates, or Teacher Guides unless explicitly in scope.

## Verification

Apply verifier responsibility when implementation is ready for evidence review:

- Select applicable gates from the diff, active SDD, rules, ADRs, and review prompts.
- Run or request build, tests, SQL/API/UI smoke checks, and review prompts when relevant.
- Record skipped gates with concrete justification, including environment-dependent gates per verification-governance.
- Summarize residual risk.
- Identify follow-up targets for Documentation Follow-Up.

Baseline gates are:

1. `dotnet build`
2. `dotnet test`
3. SQL integration or characterization checks when persistence or Legacy behavior is touched
4. Swagger/API or Blazor smoke checks when behavior changes
5. Review prompts under `Documentation/AI-Harness/review-prompts/`
6. PR checklist in `.github/pull_request_template.md`

**Verify completion does not complete the feature.** Proceed to Documentation Follow-Up for SDD-backed work.

## Documentation Follow-Up

Mandatory post-Verify phase for SDD-backed work. Execute — do not merely evaluate routing.

Use `.cursor/skills/documentation-update/SKILL.md` to run the mandatory checklist:

1. `Documentation/State.md` — highest priority when branch, runtime, blockers, verification status, or next steps changed
2. `Documentation/Product/PM_DocOrgano.md`
3. `Documentation/Technical/migration-sql.md` when persistence or migration changed
4. `Documentation/Technical/runbook.md` when runtime prerequisites changed

Route additional follow-up beyond the checklist when warranted. Sync active SDD artifacts when implementation or verification changed scope or evidence.

## Reporting

Run **after Documentation Follow-Up**, not after Verify alone.

`Documentation/State.md` and routed project docs must reflect the final verified state before generating:

- `reports/feature-report.md` — feature summary and embedded lessons learned
- `reports/session-handoff.md` — session continuity when needed

See `Documentation/AI-Harness/Harness-Design/reporting-strategy.md`.

## Teacher Guide

Run **after Reporting** when knowledge strategy criteria apply.

Use `.cursor/skills/not-a-teacher/SKILL.md` to generate `teacher-guide.md`. See `Documentation/AI-Harness/Harness-Design/knowledge-strategy.md`.

Record skip rationale in Documentation Follow-Up or feature report when an SDD existed but no Teacher Guide was produced.

## Updates

Harness calibration and durable governance changes route through Documentation Update and may require ADR evaluation.

For feature work, State and project doc updates belong in **Documentation Follow-Up**, not ad hoc after Verify.

Evaluate ADR need when a durable decision affects architecture boundaries, persistence, schema lifecycle, security/auth, public API contracts, cross-context ownership, major technology/runtime choices, or irreversible migration decisions.

Not every change requires documentation updates. Documentation should only be updated when the corresponding source of truth changes.

## Harness Calibration Workflow

Harness calibration is **distinct from feature SDD lifecycle**. Feature SDD completion does not automatically update harness governance. Use this workflow when a pilot reveals repeatable harness friction.

Authority reference: `Documentation/AI-Harness/Harness-Design/harness-architecture.md`. Operational procedure: this section.

### When To Calibrate

Calibrate the harness when a feature SDD pilot produces governance findings that would affect future pilots — not for one-off feature decisions.

### Workflow

```text
Pilot Execution (feature SDD lifecycle)
  -> Pilot Report (consolidated retrospective — prefer Documentation/SDD/<feature-slug>/reports/sdd-pilot-report-v*.md)
  -> Governance Improvement Plan (template: `Documentation/AI-Harness/template/governance/governance-improvement-plan.md`; instance: `Documentation/SDD/<feature-slug>/reports/governance-improvement-plan.md`)
  -> Governance Review / Approval
  -> Phased Governance Update (Wave 1 critical path, Wave 2 secondary, Wave 3 optional)
  -> Consistency Audit
  -> Next Pilot (forward SDD)
```

**Pilot report supersession:** When Verify or post-Follow-Up Reporting completes, publish a new pilot report version that **explicitly supersedes** prior Execute-only snapshots (e.g. v0.5 Execute → v0.6 Execute + Verify). Template: `Documentation/AI-Harness/template/reporting/sdd-pilot-report.md`.

**First example:** Paciente SQL Stabilization → [sdd-pilot-report-v1.0.md](../SDD/paciente-sql-stabilization/reports/sdd-pilot-report-v1.0.md) → Wave 1 governance update → consistency review → Prontuario forward SDD.

**Second example:** Atendimento Minimal SQL Stabilization → pilot report v0.3–v0.6 → Wave 3 harness calibration (credential probe, TASK-008 ownership, sql-migration-workflow skill, smoke scripts) → Prontuario forward SDD.

### Pilot Report Role (META-02)

Pilot reports are calibration inputs. Preferred location: `Documentation/SDD/<feature-slug>/reports/sdd-pilot-report-v*.md`. Historical Paciente report may also appear under `Documentation/AI-Harness/research/`.

They are:

- **Input** to harness calibration and implementation planning
- **Not** feature SDD truth, verification output, or operational truth

They do not override `sdd-operational.md`, `verification-governance.md`, or `Documentation/State.md`. They feed governance updates through an improvement plan and review. Use version supersession when phases advance — do not leave stale Execute-only reports as current status.

### Finding Maturity And Promotion (META-03)

Tag every calibration finding by confidence before implementation:

| Level | Meaning | Implementation gate |
|-------|---------|---------------------|
| **Experimental** | Hypothesis; not yet validated in a pilot | Do not adopt into governance |
| **Pilot-Proven** | Observed in one pilot; same as **Proven** in pilot reports | May enter Wave 1 implementation plan after review |
| **Adopted** | Implemented in harness governance, templates, or skills | Active for new SDDs |
| **Canonical** | Stable authority in Harness Design docs | Treated as harness truth |

**Promotion criteria (Pilot-Proven → Adopted):**

- Finding appears in consolidated pilot report with **Proven** confidence
- Included in governance implementation plan restricted to Proven findings only
- Review confirms no conflict with accepted ADRs or authority hierarchy
- Phased rollout completes for the finding's wave
- Consistency audit passes for affected artifacts

Preliminary and Deferred pilot findings require a second pilot or explicit approval override before adoption.

### Review And Approval

1. Produce or update consolidated pilot report.
2. Extract **Proven** findings into a governance improvement plan using `Documentation/AI-Harness/template/governance/governance-improvement-plan.md`.
3. Review for effort, breaking change, phase DoD, and rollout wave assignment.
4. Approve phased implementation before editing authority docs.
5. Record calibration completion in `Documentation/State.md`.

### Where Decisions Are Recorded

| Decision type | Owner |
|---------------|-------|
| Harness lifecycle and SDD phase policy | `sdd-operational.md` |
| Verification policy | `verification-governance.md` |
| Reporting and knowledge transfer policy | `reporting-strategy.md`, `knowledge-strategy.md` |
| Template structure | `template-architecture.md`, templates under `template/` |
| Operational procedure | This document |
| Current calibration status | `Documentation/State.md` |
| Durable architecture decisions | ADRs |

Do not create standalone governance backlog files unless a future pilot proves the need. Use `template/governance/governance-improvement-plan.md` after calibration pilots (validated by Atendimento retrospective, GOV-10).

## Commits

Keep commits atomic and focused. Do not commit secrets, `.env` files, credentials, real patient data, or clinical data.
