# Doc Organo AI Harness Architecture

## Purpose

This document defines the target architecture and governance model for the Doc Organo AI Harness. It is the design foundation for future changes to `AGENTS.md`, `.cursor/rules/`, `.cursor/skills/`, `Documentation/State.md`, review prompts, SDD artifacts, MCP usage, and auxiliary documentation.

It does not rewrite operational artifacts and does not create feature implementation tasks. Its role is to define what each harness component should own, how context should be loaded, and how verification feedback should return to durable documentation.

## Architecture Principles

1. **Codebase is context.** Agents should learn from canonical project files instead of repeated prompt explanations.
2. **RPI remains the core workflow.** Research creates durable markdown, Plan converts it into decisions and designs, Implement uses focused context and verification gates.
3. **Static and dynamic context must be separate.** Stable project facts belong in `AGENTS.md` and durable docs; current branch truth belongs in `Documentation/State.md`.
4. **Rules are guardrails, skills are workflows.** Rules should stay short and persistent; skills should handle repeatable multi-step work.
5. **Verification is part of the harness, not an afterthought.** Build, test, smoke checks, review prompts, and PR checklists are sensors that feed back into `State.md`, ADRs, or future rules.
6. **Clinical data is sensitive by default.** Security and PHI constraints apply to code, docs, prompts, logs, examples, tests, and commits.
7. **Adoption should stay incremental.** The harness should mature around the active SQL migration and clinical MVP, not introduce broad process overhead before it is useful.
8. **Authority must be explicit.** When harness artifacts conflict, agents should know which source owns architecture truth, operational truth, feature truth, and machine-facing guidance.

## Current State Assessment

The harness has completed first-pilot calibration (Paciente SQL Stabilization) and Wave 1–2 governance alignment. It is operational for forward SDD work; CI enforcement and some secondary docs remain incremental improvements.

Existing strengths:

- `AGENTS.md` gives a compact project bootstrap: stack, branch split, repository layout, bounded contexts, commands, SDD entry point, and verification entry points.
- `.cursor/rules/` separates always-on security/token guidance from scoped backend, EF, and Blazor conventions.
- `.cursor/skills/` includes operational workflows: verifier, documentation-update, not-a-teacher, sql-migration, codebase decomposition, and harness planning.
- `Documentation/AI-Harness/` contains calibrated SDD operational governance, verification governance, reporting and knowledge strategy, templates, and contribution workflow including Harness Calibration Workflow.
- First feature SDD instance exists: `Documentation/SDD/paciente-sql-stabilization/` (historical pre-calibration reference).
- Product, architecture, technical, and ADR docs provide durable context for AI-assisted development.

Main architecture risks:

- Path drift may remain in older research or auxiliary docs referencing pre-taxonomy paths.
- `Documentation/State.md` can become stale if Documentation Follow-Up is skipped after Verify.
- Harness secondary docs can drift from `sdd-operational.md` if calibration workflow is not followed after future pilots.
- Review prompt coverage for EF migrations and API contracts remains incomplete.
- CI workflow enforcing at least `dotnet build` and `dotnet test` is not yet in place.
- `Documentation/Technical/api-contract.md` does not exist yet.

## Component Responsibilities

| Component | Purpose | Responsibilities | Inputs | Outputs | Update triggers | Relationship with other components |
|-----------|---------|------------------|--------|---------|-----------------|------------------------------------|
| `AGENTS.md` | Static agent bootstrap | Stable project facts, stack, branch model, repo layout, bounded contexts, commands, task-to-doc map, high-level verification and SDD entry points | PRD, architecture docs, technical docs, ADRs, `State.md` pointers | Consistent session orientation | Stack, layout, commands, architecture rules, or canonical doc map changes | Points agents to `State.md`, docs, rules, skills, SDD, MCPs; should not duplicate full methodology |
| `.cursor/rules/` | Persistent behavior guardrails | Always-on or glob-scoped constraints for PHI, token economy, DDD, EF, Blazor, docs updates | Architecture decisions, security policy, repeated review findings | Runtime guidance injected into agent context | New persistent invariant, recurring defect, new file scope | Rules link to durable docs and may recommend skills for deeper workflows |
| `.cursor/skills/` | Repeatable task workflows | Step-by-step procedures for onboarding, decomposition, harness planning, future SQL slices and legacy rule porting | Rules, docs, codebase patterns, known risky workflows | Structured analysis or execution plan | Workflow repeats, requires tool sequence, or exceeds rule size | Skills consume `AGENTS.md`, `State.md`, architecture docs, and produce markdown handoffs |
| `Documentation/State.md` | Current operational truth | Branch, runtime status, active epic, blockers, next steps, recent decisions, harness pointers | Latest merged work, session outcomes, verification results | Bootstrap state for next session | End of relevant session, PR merge, runtime status change, blocker resolution | Read before large tasks; feeds `AGENTS.md` context indirectly without becoming historical narrative |
| Review prompts | Pre-merge review sensors | Domain, security/PHI, documentation, EF/API contract checklists | Diff, domain docs, security rules, ADRs, PR template | Findings by severity, missing checks, gate result | New review class, recurring bug, new compliance concern | Complement rules; deeper than always-on guidance; should feed PR review and future rules |
| Verifier role | Verification ownership model | Decide applicable gates, run or request checks, apply review prompts, record skipped gates, summarize residual risk | Diff, SDD tasks, review prompts, PR template, ADRs, security rules, `State.md` | Verification summary, skipped-gate reasons, residual risks, State/doc/ADR follow-up signal | Change reaches review/PR boundary, risky work completes, SDD tasks claim done | Orchestrates review prompts and test/smoke gates; does not replace human review or CI |
| SDD | Feature design workflow | Per-feature specify, design, tasks for features larger than a small change | PRD, PM, RoadMap, Domain Overview, architecture docs, State, research | Scoped feature contract, design decisions, executable tasks, verification plan | Feature > half day, cross-module change, risky clinical/security behavior | Bridges Plan to Execute and post-Verify completion chain; tasks should reference review prompts and Follow-Up |
| MCPs | External tool and runtime bridge | GitHub PR/issues, browser smoke tests, future docs/db access | Tool schemas, repo state, browser or GitHub data | Runtime evidence, PR metadata, smoke-test observations | Need external state or browser verification | MCP results should be summarized into docs or PRs, not become hidden chat-only context |
| Auxiliary docs | Durable knowledge base | PRD, PM, RoadMap, Architecture, ADRs, Technical docs, AI research, playbook, documentation index | Product decisions, architecture decisions, implementation evidence | Canonical context for agents and humans | Product, architecture, technical, methodology, or taxonomy change | Referenced by `AGENTS.md`, skills, SDD, and review prompts |

## Artifact Authority Hierarchy

The harness should use this hierarchy to resolve conflicts between artifacts. A conflict should be fixed in the stale artifact during an operational update pass, but agents need a temporary decision rule while working.

| Authority area | Primary source | Secondary source | Conflict rule |
|----------------|----------------|------------------|---------------|
| Architecture truth | Accepted ADRs | Architecture and Technical docs | ADRs win for accepted durable decisions. Architecture docs explain the current model and should be updated when an ADR changes the model. |
| Harness governance | Harness Design docs | `AGENTS.md`, rules, skills, contribution guides | Design docs define the target model. Operational artifacts implement it later and should not invent conflicting policy. |
| Operational truth | `Documentation/State.md` | PM, PRs, recent verification reports | State wins for current branch, runtime status, blockers, and next steps, even if bootstrap docs are stale. |
| Feature truth | Active SDD feature folder | User request, PM item, implementation notes | SDD owns feature scope, acceptance criteria, design, tasks, and verification expectations once created. It cannot override ADRs without proposing a new ADR. |
| Product truth | PRD | PM and RoadMap | PRD owns stable product intent. PM and RoadMap own sequencing and future horizons. |
| Machine-facing guidance | Rules and skills | `AGENTS.md`, review prompts | Rules enforce concise guardrails; skills own repeatable workflows. Review prompts detect issues but do not redefine project policy by themselves. |

If a conflict exposes a new durable decision, create or update an ADR. If it exposes stale current status, update `Documentation/State.md`. If it exposes recurring agent behavior risk, update a rule or skill in a later operational pass.

## Harness Lifecycle Flow

The target harness flow is:

```text
Research
->
Plan
->
SDD when risk, size, or cross-module scope requires it
  (Specify -> Design -> Tasks -> SDD Pre-Execution Review for Large/Complex)
->
Execute
->
Verify
->
Documentation Follow-Up
->
Reporting
->
Teacher Guide when warranted
->
State / ADR / Documentation Updates
```

| Phase | Purpose | Typical inputs | Required output |
|-------|---------|----------------|-----------------|
| Research | Understand codebase, docs, risks, and existing decisions | Code, docs, ADRs, research baseline, targeted searches | Durable findings or a short handoff, not long chat history |
| Plan | Convert research into a proposed path | Research output, PRD/PM, architecture docs, `State.md` | Plan, design note, or decision that can be reviewed |
| SDD | Capture feature-specific scope and design when risk warrants it | Plan, PRD/PM, Domain Overview, ADRs, technical docs | `specify.md`, `design.md`, `tasks.md`; SDD Pre-Execution Review before Execute for Large/Complex work |
| Execute | Make scoped changes using focused context | SDD or plan, relevant files, rules, skills | Code or documentation changes within the approved scope |
| Verify | Produce evidence that the work is correct enough to continue | Diff, SDD tasks, review prompts, test/smoke commands | Verification summary, skipped-gate reasons, residual risks |
| Documentation Follow-Up | Synchronize operational and technical truth after Verify | Verifier follow-up targets, active SDD | Updated State, PM, migration-sql, runbook, and routed docs |
| Reporting | Preserve continuity and feature summary | Verified implementation, synchronized docs | Session handoff, feature report, embedded lessons learned |
| Teacher Guide | Transfer verified implementation knowledge | Finalized code, verification, reporting | `teacher-guide.md` when knowledge strategy criteria apply |
| Updates | Feed durable learning back into the harness | Verification results, merged work, new decisions | State update, ADR, docs update, or future rule/skill recommendation |

Research and Plan may produce only design documentation. Implementation should not begin just because research exists; it begins when scope, risk, and verification expectations are clear enough.

## Harness Calibration Workflow

Feature SDD completion does not automatically update harness governance. When a pilot produces repeatable harness friction, use the calibration workflow defined operationally in `Documentation/AI-Harness/CONTRIBUTING-AI.md`.

```text
Pilot Execution -> Pilot Report -> Implementation Plan (Proven only)
  -> Review / Approval -> Phased Governance Update -> Consistency Audit -> Next Pilot
```

Pilot reports under `Documentation/AI-Harness/research/` are calibration inputs, not feature truth, verification output, or operational truth. They do not override `sdd-operational.md`, `verification-governance.md`, or `Documentation/State.md`.

Finding maturity for adoption: **Experimental** → **Pilot-Proven** (Proven in report) → **Adopted** (implemented in governance) → **Canonical** (stable authority). Preliminary and Deferred findings require second-pilot validation or explicit approval before adoption.

First calibration instance: Paciente SQL Stabilization → `sdd-pilot-report-v1.0.md` → Wave 1–2 governance updates.

## ADR Governance Policy

ADRs record durable decisions that future agents and developers must understand to avoid re-litigating or accidentally reversing architectural direction.

Create an ADR when a decision:

- Changes bounded context ownership, aggregate boundaries, or where business rules live.
- Changes persistence strategy, schema lifecycle, migration approach, soft-delete behavior, or data history handling.
- Establishes or changes security, PHI, auth, authorization, logging, or secret-handling architecture.
- Changes public API contracts, ID strategy, DTO shape policy, or compatibility expectations across API/front boundaries.
- Chooses or replaces significant infrastructure, runtime, framework, integration, MCP, or deployment strategy.
- Creates a durable exception to an existing architecture rule.
- Resolves a recurring architectural disagreement that would otherwise keep appearing in plans or reviews.
- Is discovered during SDD design and affects more than the local feature implementation.

Do not create an ADR for:

- Local bug fixes or implementation details that do not change future architecture guidance.
- Mechanical refactors with no behavioral or ownership change.
- Temporary branch status, blockers, or next steps. Use `Documentation/State.md`.
- Feature-specific scope, acceptance criteria, or task sequencing. Use SDD.
- Routine documentation cleanup or path correction unless it changes documentation taxonomy.
- Decisions already covered by an accepted ADR, unless the decision revises or supersedes it.

An ADR should state the status, context, decision, consequences, and affected artifacts. Accepted ADRs may require follow-up updates to architecture docs, rules, skills, review prompts, SDD templates, or `AGENTS.md`, but those updates should happen in the appropriate operational pass.

## Verifier Role

The verifier is a harness role, not necessarily a separate person or tool at first. Its purpose is to prevent agents from self-certifying completion without evidence.

Responsibilities:

- Decide which verification gates apply based on risk, touched areas, SDD tasks, and PR expectations.
- Run or request applicable checks: build, tests, SQL integration tests, Swagger/API smoke, Blazor smoke, review prompts, and PR checklist items.
- Apply security/PHI review whenever clinical data, identity fields, logs, prompts, generated examples, auth assumptions, files, or secrets are touched.
- Record skipped gates with concrete reasons.
- Summarize residual risk and unresolved blockers.
- Recommend whether results should update `Documentation/State.md`, create/update an ADR, change durable docs, or inform future rules/skills.

Inputs:

- Diff or planned change scope.
- SDD `tasks.md` verification section when present.
- Relevant review prompts.
- PR template expectations.
- Security/PHI rule and accepted ADRs.
- Current `Documentation/State.md`.

Outputs:

- Verification summary with commands/checks run.
- Skipped-gate list with reasons.
- Findings grouped by severity when review prompts are used.
- Residual risk statement.
- State, ADR, documentation, rule, or skill follow-up recommendation.

Relationship with review prompts:

- Review prompts are sensors.
- The verifier chooses which sensors apply, runs or requests them, and converts results into a concise gate result.
- A review prompt finding may recommend a future rule or skill, but only after the governance owner decides it is recurring or durable.

Relationship with SDD:

- `specify.md` defines acceptance criteria.
- `design.md` identifies testability, ADR, and review risks.
- `tasks.md` names concrete verification gates.
- The verifier checks whether those gates were satisfied or explicitly skipped with reasons.

## Context Loading Policy

Always load or keep available:

- `Documentation/State.md`
- `AGENTS.md`
- always-on security/PHI and token economy rules

Load by task:

- Domain work: `Documentation/Architecture/Domain_Overview_Business_Rules.md`, relevant entity/controller/repository files, `domain-review.md`.
- SQL/EF migration: `Documentation/Technical/migration-sql.md`, ADR-001, ADR-002, EF rule, relevant Fluent configs/migrations.
- Frontend work: `Documentation/Technical/front-architecture.md`, Blazor rule, relevant page/state/service/mapper files.
- Product planning: `Documentation/Product/PRD.md`, `Documentation/Product/PM_DocOrgano.md`, `Documentation/Product/RoadMap.md`.
- Harness planning: `Documentation/AI-Harness/research/AI-Research.md`, `Documentation/AI-Harness/research/playbook.md`, `Documentation/AI-Harness/*`, `.cursor/rules/*`, `.cursor/skills/*`.
- Legacy behavior: targeted methods in `DocAPI/Legacy/_LegacySheetsDb/`, not the whole folder.

Avoid loading by default:

- Full Legacy folders.
- Full research chats.
- Large methodology docs in implementation sessions unless the task is harness architecture or governance.
- Financial domain files unless the task is explicitly scoped to future planning.

## Feedback And Verification Loop

The target loop is:

1. `AGENTS.md`, rules, skills, SDD, and auxiliary docs guide the agent before changes.
2. The agent produces a small plan or SDD-backed implementation path.
3. Verification gates run according to risk: build, tests, SQL integration, Swagger/Blazor smoke, review prompts, PR checklist.
4. Findings feed back into one of:
   - `Documentation/State.md` for current status or blockers.
   - ADRs for durable architecture decisions.
   - Rules for recurring guardrails.
   - Skills for repeatable workflows.
   - SDD templates when feature planning quality improves.

## Missing Or Weak Architecture Pieces

Missing or incomplete:

- Documentation path normalization guidance for stale `docs/` links and absent `Documentation/AI-Harness/Research/*` paths.
- Ongoing documentation path normalization for renamed or moved harness artifacts.
- Review prompt coverage for EF migrations and API contracts.
- Continued alignment between verification governance, the verifier skill, review prompts, and future CI.
- Skills for legacy Atendimento rule porting beyond sql-migration-workflow.
- CI workflow enforcing at least `dotnet build` and `dotnet test`.
- `Documentation/Technical/api-contract.md`.
- Clear MCP usage policy beyond GitHub and browser recommendations.

Overlaps to govern:

- `AGENTS.md`, `CONTRIBUTING-AI.md`, and `doc-organo-context` repeat session bootstrap guidance. This is acceptable if one canonical sequence is maintained.
- `security-phi.mdc`, `security-phi-review.md`, and PR checklist security items overlap intentionally at different depths.
- `dotnet-ddd.mdc`, Domain Overview, and Architecture Overview overlap intentionally between concise enforcement and durable explanation.
- `AI-Research.md` and `playbook.md` should remain research/methodology inputs, not daily operational truth.
- Duplicate or stale skill paths should be resolved by policy before adding more skills.

## Adoption Strategy

Testing:

- Treat tests as verification sensors, not a bounded context.
- Start with gates already documented: `dotnet build`, `dotnet test`, then SQL integration tests for the active migration slice.
- Add characterization tests before porting critical Legacy Atendimento behavior.
- Defer E2E until API and Blazor flows stabilize.

Verification gates:

- Keep a risk-based ladder: build, test, smoke, review prompts, PR checklist.
- Make the verifier role explicit before relying on agents to self-certify completion.
- Use PR template and review prompts as the human-readable gate until CI is available.

Security reviews:

- Keep PHI/security as always-on guidance plus pre-merge review.
- Require security/PHI review whenever clinical data, logs, files, auth assumptions, API fields, or tests are touched.
- Promote recurring security findings into rules only when they are short and persistent.

SDD workflow:

- Use SDD for features larger than a small change, cross-module work, or clinical/security-sensitive behavior.
- Keep `specify.md` focused on problem, goals, out-of-scope, acceptance criteria, and ubiquitous language.
- Keep `design.md` focused on architecture choices, reuse, data/API/front changes, risks, and ADR needs.
- Keep `tasks.md` focused on executable steps, sequencing, verification, and State update.

MCP usage:

- Phase 1: GitHub for issues/PRs and browser for Swagger/Blazor smoke tests.
- Phase 2: consider docs search or DB introspection only after local docs, verification, and SQL migration practices stabilize.
- MCP evidence should be converted into durable documentation or PR notes when it affects decisions.

## Recommendations For Future Operational Updates

These are design recommendations only; they should be applied in a later operational update pass.

For `AGENTS.md`:

- Align all documentation and SDD paths with the canonical `Documentation/` taxonomy.
- Replace references to missing `Documentation/studies.md` with the chosen canonical methodology or research path.
- Keep the file as a bootstrap, not a methodology manual.
- Add only short stable guidance for verifier expectations if the verification model is adopted.

For `Documentation/AI-Harness/CONTRIBUTING-AI.md`:

- Clarify the canonical SDD template path and feature SDD folder path.
- Add a concise verifier step between implementation and commit/PR.
- Link review prompts as expected review sensors, not optional reading.
- Keep commit guidance brief and avoid duplicating PR template details.

For `Documentation/AI-Harness/rules.md`:

- Include every active rule, including `update-doc.mdc`.
- Add rule ownership metadata: purpose, scope, load policy, update trigger.
- State when not to add a rule and when to create a skill instead.
- Keep it as an index, not a copy of rule contents.
