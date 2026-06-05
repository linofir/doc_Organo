# AGENTS.md Strategy

## Purpose

This document defines the target responsibility of `AGENTS.md` inside the Doc Organo AI Harness. `AGENTS.md` should be the stable entry point that orients an agent quickly without becoming a long methodology document, daily status log, or replacement for product and architecture documentation.

## Responsibilities

`AGENTS.md` should own:

- Stable product identity: what Doc Organo is and the controlled clinical MVP context.
- Current stack facts that rarely change: ASP.NET Core, EF Core, SQL Server, Blazor Server, Docker SQL.
- Branch model: `main` as Google Sheets reference, `feature/base_DB` as SQL migration branch while that remains true.
- Repository layout and ownership map.
- Bounded context summary with deferral guidance for Financial work.
- High-level architecture guardrails: Clean Architecture light, repository boundaries, legacy behavior reference, abstraction gate.
- Commands that are safe and useful for most agents.
- Task-to-document routing: which durable document to read first for domain, backend, schema, product, migration, or harness work.
- SDD entry rule: when to use specify/design/tasks and where feature folders belong.
- Verification ladder summary.
- Short pointer to the verifier role once it becomes operational.
- Short pointer to ADR governance when a task may create a durable decision.
- Recommended MCP categories at a high level.

`AGENTS.md` should not own:

- Daily branch status or detailed blockers. That belongs in `Documentation/State.md`.
- Full RPI methodology or AI study notes. That belongs in `Documentation/AI-Harness/research/playbook.md` and `Documentation/AI-Harness/research/AI-Research.md`.
- Full domain rules. Those belong in `Documentation/Architecture/Domain_Overview_Business_Rules.md`.
- Full SQL migration details. Those belong in `Documentation/Technical/migration-sql.md`.
- Full review checklists. Those belong in `Documentation/AI-Harness/review-prompts/`.
- Long workflow procedures. Those belong in `.cursor/skills/`.
- Implementation tasks or feature specifications. Those belong in SDD feature folders or PM planning.

## Inputs

Primary inputs:

- `Documentation/State.md` for the current branch and runtime truth.
- `Documentation/Product/PRD.md` for stable product intent.
- `Documentation/Architecture/Architecture_Overview.md` for backend structure.
- `Documentation/Architecture/Domain_Overview_Business_Rules.md` for bounded contexts and ubiquitous language.
- `Documentation/Architecture/ADR/` for accepted decisions.
- `Documentation/Technical/migration-sql.md`, `front-architecture.md`, and `runbook.md` for technical operating context.
- `Documentation/AI-Harness/documentation-index.md` for canonical documentation taxonomy.
- `.cursor/rules/` and `.cursor/skills/` as machine-facing harness assets.

Secondary inputs:

- `Documentation/AI-Harness/research/AI-Research.md` when harness conclusions change.
- `Documentation/AI-Harness/research/playbook.md` when the RPI or harness methodology changes.
- PR template and review prompts when verification expectations change.

## Outputs

Expected outputs from `AGENTS.md` are behavioral, not feature-specific:

- Agents start from the correct branch and stack assumptions.
- Agents choose the right documentation path for the task.
- Agents avoid common architectural mistakes such as re-enabling Sheets on the SQL branch, expanding Financial early, or adding premature abstractions.
- Agents understand that Legacy Sheets code is a behavior reference, not an implementation to revive by default.
- Agents know when SDD, review prompts, and verification gates apply.
- Agents know when a decision may require ADR review instead of being buried in implementation notes.

## Update Triggers

Update `AGENTS.md` when:

- The canonical branch model changes.
- The stack, runtime commands, ports, or repository layout change.
- A bounded context boundary or major architecture rule changes.
- A new documentation category becomes canonical.
- SDD paths or MCP policy change.
- Verification gates become more concrete or enforced.
- An ADR changes a stable project assumption.
- The artifact authority hierarchy changes.
- The verifier role becomes operationally enforced.

Do not update `AGENTS.md` for:

- Routine implementation progress.
- Temporary blockers.
- One-off task notes.
- Detailed feature plans.
- Research conclusions that have not yet become stable harness policy.

## Relationship With Other Harness Components

| Component | Relationship |
|-----------|--------------|
| `Documentation/State.md` | `AGENTS.md` should instruct agents to read State first, but State owns the current truth. |
| `.cursor/rules/` | Rules enforce concise guardrails referenced by `AGENTS.md`; `AGENTS.md` should not duplicate every rule. |
| `.cursor/skills/` | Skills execute workflows that are too long for `AGENTS.md`; `AGENTS.md` can route agents to them. |
| Review prompts | `AGENTS.md` should list review prompts as verification sensors, not embed their checklists. |
| Verifier role | `AGENTS.md` may summarize that verification ownership exists, but detailed workflow belongs in a verifier skill/checklist or contribution guide. |
| SDD | `AGENTS.md` defines when SDD is required and where artifacts live; SDD owns feature-specific planning. |
| ADRs | `AGENTS.md` should point to ADR governance for durable decisions; it should not duplicate ADR criteria in full. |
| MCPs | `AGENTS.md` names recommended MCP categories; detailed tool usage belongs in skills or operational docs. |
| Auxiliary docs | `AGENTS.md` is a map to durable docs, not the durable doc itself. |

## Authority And Conflict Rules

`AGENTS.md` is a bootstrap map, not the highest authority for every kind of truth.

- If `AGENTS.md` conflicts with an accepted ADR, the ADR wins for durable architecture decisions.
- If `AGENTS.md` conflicts with `Documentation/State.md`, State wins for current branch/runtime/blocker status.
- If `AGENTS.md` conflicts with a Harness Design document, the design document wins for target governance until the operational refactor updates `AGENTS.md`.
- If `AGENTS.md` conflicts with an active SDD, SDD wins for that feature's scope and verification expectations, unless it contradicts an accepted ADR.
- If `AGENTS.md` conflicts with a rule, the rule wins for concise machine-facing guardrails in its scope.

Future updates to `AGENTS.md` should make these relationships easy to discover without turning the file into a governance manual.

## ADR And Verifier Touchpoints

`AGENTS.md` should eventually include concise routing guidance for two governance concepts:

- ADR governance: create or update ADRs for durable decisions involving architecture boundaries, persistence, schema/data lifecycle, security/auth/PHI, API contracts, major technology choices, or durable exceptions.
- Verifier role: after meaningful implementation, a verifier workflow should decide applicable gates, record skipped gates, apply review prompts when relevant, and summarize residual risk.

The full ADR policy and verifier role definition belong in `harness-architecture.md`; `AGENTS.md` should only route agents to them.

## Current Gaps To Address Later

These are recommendations for a future operational update pass:

- Normalize stale path references from `docs/` to `Documentation/`.
- Resolve remaining `Documentation/studies.md` references by pointing methodology references to `Documentation/AI-Harness/research/playbook.md`.
- Align SDD references with the actual template location and intended feature folder location.
- Keep the `docs/` folder out of the repository layout unless compatibility redirects are intentionally restored.
- Treat `Documentation/AI-Harness/research/AI-Research.md` and `Documentation/AI-Harness/research/playbook.md` as canonical research sources.

## Governance Rules

1. `AGENTS.md` should stay short enough to read every session.
2. A new line in `AGENTS.md` should represent a stable project invariant or navigation rule.
3. If a proposed addition is procedural and repeatable, create or update a skill instead.
4. If a proposed addition is a warning that should always shape behavior, create or update a rule instead.
5. If a proposed addition records a durable decision and consequences, apply the ADR Governance Policy and create or update an ADR when the threshold is met.
6. If a proposed addition describes current progress, update `State.md` instead.
