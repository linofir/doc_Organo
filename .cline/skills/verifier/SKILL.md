---
name: verifier
description: Orchestrates Doc Organo verification after implementation or before completion. Use when checking whether work is complete, selecting build/test/SQL/API/UI/security/documentation gates, applying review prompts as sensors, summarizing residual risk, or preparing State/ADR/documentation follow-up.
---

# Verifier - Doc Organo

## Purpose

Use this workflow to decide whether Doc Organo work has enough evidence to be considered complete.

The verifier is a workflow skill. It selects and evaluates verification gates, applies review prompts as sensors, records skipped gates, summarizes residual risk, and identifies follow-up needs.

The verifier is not a rule, not a review prompt, not a test runner, and not an architecture authority.

## Start Here

1. Read `Documentation/State.md` for current operational truth.
2. Read `AGENTS.md` for authority hierarchy, commands, and routing.
3. Review the diff, task request, implementation summary, PR, or active SDD artifacts.
4. Read only the task-specific docs needed to classify the change.
5. Select applicable gates and review sensors before declaring completion.

Use the `documentation-update` skill only when routing follow-up updates is needed. The verifier identifies follow-up needs; the documentation-update skill decides the owning artifact and checks authority boundaries.

## Change Classification

Classify the change by touched behavior and risk, not just by file path.

Use these labels as applicable:

- Documentation changes: docs, rules, skills, SDD, prompts, indexes, State, ADRs.
- Domain changes: entities, business rules, validation, ubiquitous language, clinical workflows.
- Persistence changes: EF configuration, repositories, migrations, schema, SQL behavior, data lifecycle.
- Frontend changes: Blazor pages, components, state, services, mappers, UI flow.
- API changes: controllers, DTOs, routes, IDs, response shapes, API/front compatibility.
- Security-sensitive changes: PHI, patient identity, clinical data exposure, auth/authz, logs, files, prompts, tests, secrets.
- Legacy behavior porting: behavior copied or reinterpreted from `DocAPI/Legacy/_LegacySheetsDb/`.
- Cross-layer features: changes spanning domain, persistence, API, frontend, or docs.

If an active SDD exists, compare classification against `specify.md`, `design.md`, and `tasks.md`. The SDD owns feature-specific scope and verification expectations unless it conflicts with an accepted ADR.

## Gate Selection

Do not assume every gate applies. Select gates from the classification, active SDD, rules, accepted ADRs, and implementation risk.

Baseline gates:

- Build: `dotnet build`.
- Automated tests: `dotnet test`.
- SQL / Persistence: EF migration review, repository or integration checks, `dotnet ef database update --project DocAPI/DocAPI.csproj` when schema/runtime migration is involved.
- API: Swagger or targeted endpoint smoke when controllers, DTOs, routes, IDs, or response behavior change.
- UI: Blazor smoke when pages, state, services, mappers, or user flows change.
- Security: security/PHI review when clinical data, identity fields, logs, files, generated examples, auth assumptions, API exposure, tests, or secrets are touched.
- Documentation: documentation review when operational truth, architecture/technical explanation, SDD, rules, skills, prompts, paths, or governance change.
- ADR evaluation: apply ADR criteria when a durable decision may affect boundaries, persistence, schema lifecycle, security/auth, API contracts, cross-context ownership, major runtime choices, or irreversible migration decisions.
- Legacy characterization: targeted characterization tests or explicit behavior comparison when Legacy behavior is ported.

When a gate is skipped, record a concrete reason. "Not needed" is acceptable only when tied to the classification, such as "No runtime or code behavior changed."

### Environment-Dependent Evidence

When SQL integration, Docker, or credential gates cannot run during Verify, apply `Documentation/AI-Harness/Harness-Design/verification-governance.md` Environment-Dependent Evidence policy.

- Record gate status as **Skipped (environment unavailable)**, not Passed without evidence.
- Accept Execute evidence only when Execute ran the gate successfully with documented prerequisites satisfied.
- Classify residual risk when accepting Execute-only evidence.
- Example: Docker unavailable during Verify re-run; SQL integration passed on Execute with `SA_PASSWORD` and `DOCORGANO_TEST_CONNECTION` documented.

## SDD-Backed Work — Documentation Review Default

For SDD-backed feature work, apply `check-docs.md` unless explicitly skipped with reason tied to classification. Documentation review is expected when operational truth, SDD alignment, or governance docs may have drifted during Execute/Verify.

When `check-docs.md` reports **stale `Documentation/State.md`**, `migration-sql.md` checklist drift, or active SDD misalignment with verified implementation, treat documentation review as **blocking** until Documentation Follow-Up executes or the user explicitly accepts residual risk. Do not mark Verify complete while operational truth contradicts verified feature status without documenting the gap and mandatory Follow-Up target.

## Runtime Validation (TASK-008)

When the active SDD defines runtime validation (Swagger/HTTP smoke):

- **Execute** may record intent or partial notes; missing structured Execute notes is not automatically a critical defect.
- **Verify** owns the durable evidence table in `verification.md` § Runtime Validation.
- If Execute did not provide smoke notes, run equivalent HTTP checks during Verify and record results in `verification.md`.
- Prefer `scripts/api-smoke-<aggregate>.ps1` when available; otherwise use curl with **UTF-8 no-BOM** JSON files (see runbook).

## Review Sensor Selection

Review prompts are sensors. They detect issues; they do not redefine policy.

Use prompts under `Documentation/AI-Harness/review-prompts/` when relevant:

- `domain-review.md`: domain language, clinical business rules, entity/use-case placement, Financial deferral, Legacy behavior porting.
- `security-phi-review.md`: PHI exposure, credentials, secrets, logs, files, auth/authz assumptions, synthetic test data.
- `check-docs.md`: State, ADR, durable docs, SDD alignment, path drift, authority boundaries.
- `test-strategy.md`: automated coverage for new behavior, meaningful test names, synthetic data, explicit missing-test debt.

For each selected sensor, record why it applies and whether findings are Critical, Suggestion, or OK.

Critical findings block completion until fixed or explicitly accepted by the user with residual risk. Suggestions may be deferred if the residual risk is documented.

## Test Coverage Policy

Treat testing as implementation quality, not optional cleanup.

For behavior changes, evaluate:

- Existing tests passed.
- New behavior has automated coverage.
- Critical business rules are not covered only by manual testing.
- SQL or Legacy behavior has integration or characterization coverage when risk warrants it.
- Missing coverage is documented as technical debt.
- Deferred coverage has an owner, reason, and residual risk.

For new features, test creation is part of implementation, not a post-implementation task.

## Residual Risk

Classify residual risk after gates and sensors:

- Low: applicable gates passed, no critical findings, missing checks are low impact and justified.
- Medium: important checks were skipped or deferred, behavior is partly manual-verified, or suggestions remain with clear containment.
- High: critical findings remain, required gates failed or were not run, clinical/security/persistence risk is unresolved, or completion depends on unverified assumptions.

Always justify the risk rating in one or two sentences.

## Follow-up Identification

Identify whether follow-up may be needed for:

- `Documentation/State.md`: current branch, runtime status, blockers, verification status, next steps.
- ADR evaluation: durable decisions or revisions to accepted decisions.
- Architecture docs: durable architecture explanation.
- Technical docs: operational technical guidance, migration, frontend, API guidance.
- Rules: recurring concise guardrails.
- Skills: repeatable workflows or verification gaps.
- SDD artifacts: feature scope, design, tasks, or verification expectations.
- Review prompts: recurring review sensors that are missing or obsolete.

The verifier **identifies** follow-up needs. Documentation Follow-Up **executes** mandatory updates via the `documentation-update` skill. Verify completion does not substitute for Documentation Follow-Up.

Do not route or rewrite documentation inside this skill unless the user asked for documentation changes. Use the `documentation-update` skill for routing, path drift checks, authority validation, and mandatory post-Verify checklist execution.

## Task And Checkbox Ownership (SDD-Backed Work)

| Phase | Owns |
|-------|------|
| Execute | Implementation complete; `TASK-*` done in code and handoff; may mark `TASK-*` in `tasks.md` |
| Verify | Requirement acceptance; runtime evidence in `verification.md`; confirms `TASK-*` acceptance |
| Documentation Follow-Up | Operational doc sync (`State`, PM, migration-sql, runbook); marks `VP-*` / `DF-*` — not default owner for `TASK-*` implementation status |

## Completion Policy

Work is not complete until:

1. Applicable gates have been selected and evaluated.
2. Automated test expectations have been evaluated.
3. Required review sensors have been applied or explicitly skipped with reasons.
4. Failed gates and critical findings are resolved or accepted as residual risk by the user.
5. Missing coverage and deferred checks are documented.
6. Residual risk is classified and justified.
7. Documentation follow-up has been evaluated.
8. ADR evaluation has been considered when applicable.

If any required evidence is missing, report the work as not fully verified rather than complete.

## Output Format

Use this format for verification handoffs:

```markdown
# Verification Summary: [scope]

## Change Classification
- Labels:
- Evidence:

## Gates
| Gate | Status | Evidence / Reason |
|------|--------|-------------------|
| Build | Passed / Failed / Skipped / Blocked | ... |
| Automated Tests | Passed / Failed / Skipped / Blocked | ... |
| SQL / Persistence | Passed / Failed / Skipped / Blocked | ... |
| API | Passed / Failed / Skipped / Blocked | ... |
| UI | Passed / Failed / Skipped / Blocked | ... |
| Security | Passed / Failed / Skipped / Blocked | ... |
| Documentation | Passed / Failed / Skipped / Blocked | ... |
| ADR Evaluation | Needed / Not Needed / Blocked | ... |
| Legacy Characterization | Passed / Failed / Skipped / Blocked | ... |

## Review Sensors
- Applied:
- Skipped:
- Findings:

## Test Coverage
- Existing tests:
- New behavior covered:
- Missing or deferred coverage:

## Residual Risk
- Rating: Low / Medium / High
- Justification:

## Follow-up Needed
- State:
- ADR:
- Architecture / Technical docs:
- Rules:
- Skills:
- SDD:
- Review prompts:

## Completion Decision
- Complete / Not complete / Complete with accepted residual risk:
- Reason:
```

## Anti-patterns

- Do not claim completion only because `dotnet build` or `dotnet test` passed.
- Do not run every possible gate when the classification shows it is irrelevant.
- Do not silently skip failed or unavailable gates.
- Do not duplicate the documentation-update skill's routing and authority checks.
- Do not create ADRs automatically; identify ADR candidates and apply the ADR policy.
- Do not turn review prompts into rules unless recurring findings justify a concise guardrail.
- Do not load full Legacy folders; read targeted methods only.