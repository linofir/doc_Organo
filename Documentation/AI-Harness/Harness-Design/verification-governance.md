# Verification Governance

## Purpose

This document defines the governance boundaries for verification in the Doc Organo AI Harness. It explains what verification must prove, who owns each decision, and how verification evidence flows back into SDD, State, ADR, documentation, rules, skills, and review prompts.

This document is governance, not a command checklist. The operational workflow remains in `.cursor/skills/verifier/SKILL.md`.

## Scope

Verification governance applies when work changes behavior, persistence, API contracts, frontend flows, clinical rules, security/PHI handling, documentation authority, harness governance, or feature SDD artifacts.

It also applies to documentation-only governance changes when those changes alter how future agents plan, execute, verify, or report work.

## Responsibilities

Verification governance owns:

- The rule that implementation is not complete without evaluated evidence.
- The relationship between feature requirements, implementation evidence, tests, review sensors, and residual risk.
- The authority boundary between SDD, Verifier, Documentation Update, ADRs, review prompts, and templates.
- The minimum categories of evidence expected for SDD-backed work.
- The requirement that skipped gates have concrete reasons.

Verification governance does not own:

- Running every command for every change.
- Selecting exact gates for a specific diff.
- Writing feature-specific acceptance criteria.
- Routing documentation updates.
- Accepting or rejecting ADRs.
- Replacing CI, human review, or the verifier skill.

## Authority Boundaries

| Area | Owner | Boundary |
|------|-------|----------|
| Feature acceptance criteria | Active SDD | Defines what must be true, but does not self-certify completion. |
| Gate selection | Verifier | Selects applicable gates from risk, diff, active SDD, rules, ADRs, and review prompts. |
| Review sensors | Review prompts | Detect issues; they do not redefine governance. |
| Documentation routing | Documentation Update | Routes State, ADR, architecture, technical, rules, skills, prompt, and SDD follow-up. |
| Durable decisions | ADRs | Own accepted architecture decisions and override SDD when in conflict. |
| Current operational truth | `Documentation/State.md` | Owns current branch, runtime status, blockers, verification status, and next steps. |

## Minimum Viable Verification v1

For this v1, verification is intentionally lightweight and risk-based. Every meaningful change should produce a verification handoff that answers:

- What changed?
- Which risk labels apply?
- Which gates were selected?
- Which gates passed, failed, were skipped, or were blocked?
- Which review sensors were applied or skipped?
- What test evidence exists?
- What residual risk remains?
- What follow-up may be needed?
- Is the work complete, incomplete, or complete only with accepted residual risk?

## Gate Categories

The verifier chooses from these categories based on the change:

- Build.
- Automated tests.
- SQL, EF, migration, or repository checks.
- API smoke checks.
- UI smoke checks.
- Security/PHI review.
- Domain review.
- Documentation review.
- Test strategy review.
- ADR evaluation.
- Legacy behavior characterization or behavior comparison.

Not every gate applies to every change. Skipped gates must include reasons tied to the change classification.

## Relationship With SDD

SDD prepares verification; it does not complete it.

- `specify.md` provides requirements, acceptance criteria, constraints, and sizing.
- `design.md` identifies architecture, persistence, API, frontend, security, testability, and ADR risks.
- `tasks.md` lists expected tests, smoke checks, review prompts, and known residual risks.
- Verification evaluates the resulting evidence and decides whether completion is justified.

For Large or Complex SDD work, requirement IDs should be traceable through design, tasks, tests, and verification evidence.

## Relationship With Documentation Update

The Verifier identifies follow-up needs after gates and review sensors are evaluated. Documentation Update owns routing and authority validation.

Examples:

- State update when current runtime, blockers, verification status, or next steps changed.
- ADR evaluation when durable architecture, persistence, schema, security, API, ownership, runtime, or irreversible decisions were introduced.
- Technical or architecture updates when durable explanation changed.
- Template, rule, skill, or review prompt updates when the workflow reveals a recurring gap.

## Relationship With Reporting

Verification outputs are evidence. Reporting outputs are communication artifacts.

A session handoff or feature report may summarize verification status, but it must not become the authority for gate selection, residual risk acceptance, or current operational truth.

## Completion Policy

Work is not complete until:

- Applicable gates were selected and evaluated.
- Failed or blocked gates were resolved or explicitly carried as residual risk.
- Skipped gates have concrete reasons.
- Required review sensors were applied or skipped with reasons.
- Automated test expectations were evaluated.
- Security/PHI implications were considered when clinical data, identity fields, logs, files, prompts, tests, auth assumptions, API exposure, or secrets are touched.
- Documentation follow-up and ADR need were evaluated.
- Residual risk is classified and justified.

Verify completion does not substitute for Documentation Follow-Up execution. After Verify declares completion, Documentation Follow-Up remains mandatory for SDD-backed work.

## Environment-Dependent Evidence Policy

Some gates require infrastructure or credentials that may be unavailable during Verify even when Execute produced valid evidence. SQL integration tests with Docker and `SA_PASSWORD` are the primary example in Doc Organo v1.

### Acceptance rules

When a gate is environment-dependent:

1. Execute evidence may be accepted during Verify if Execute ran the gate successfully with documented environment prerequisites satisfied.
2. Verify must record the gate as **Skipped (environment unavailable)** with a concrete reason, not as Passed without evidence.
3. Residual risk must be classified and justified when Verify accepts Execute-only evidence.
4. The skip reason must name the missing prerequisite, such as Docker unavailable or `SA_PASSWORD` not set.

### Documentation requirements

When accepting Execute-only evidence for an environment-dependent gate:

- Record the Execute command, timestamp or session reference, and pass/fail outcome in the verification summary.
- Reference Execution Prerequisites from the active SDD `specify.md` when present.
- State whether re-running the gate during Verify was attempted and why it failed or was not attempted.

### Residual risk guidance

| Situation | Typical residual risk | Acceptable when |
|-----------|----------------------|-----------------|
| SQL integration passed on Execute; Docker unavailable on Verify | Medium | Execute evidence is documented; prerequisite gap is environmental, not implementation |
| SQL integration never run on Execute or Verify | High | Not acceptable for completion without explicit user acceptance |
| Manual runtime smoke only in session notes | Medium | Acceptable for pilot slices with documented checks; prefer structured Runtime Validation Environment in design |

This policy aligns with `.cursor/skills/verifier/SKILL.md` skip/accept behavior. Governance and skill must use consistent terminology: **Skipped (environment unavailable)** with Execute evidence accepted.

## v1 Limitations

This v1 intentionally does not define CI architecture, full release gates, metrics, dashboards, external tool integrations, or automated enforcement. Those should follow real pilot evidence from Paciente retrospective calibration and the ProntuarioRepository forward SDD pilot.
