---
name: codebase-decomposition
description: Analyzes Doc Organo codebase for DDD boundaries, technical debt, and refactoring targets. Use when researching architecture, planning refactors, porting Legacy Sheets logic, or decomposing features before SDD specify/design.
---

# Codebase Decomposition — Doc Organo

## When to use

- Before large refactors or SQL migration steps.
- When porting rules from `Legacy/_LegacySheetsDb/`.
- When user asks "analyze the codebase" or "what should we refactor first".
- When preparing SDD research or design inputs.

## Start Here

1. Read `Documentation/State.md` for current operational truth.
2. Read `AGENTS.md` for authority hierarchy and routing.
3. Read the task-specific architecture or technical document from `Documentation/AI-Harness/documentation-index.md`.
4. Use targeted code reads; avoid loading full Legacy folders.

## Process

Copy this checklist and track progress:

```
- [ ] Step 1: Identify bounded context (Clinical / Financial / Support)
- [ ] Step 2: Map aggregates and entry points (Controllers → Repository → Entity)
- [ ] Step 3: Compare with Domain_Overview doc
- [ ] Step 4: Scan Legacy for business rules not in domain
- [ ] Step 5: List debt with priority (P0 blocks run, P1 blocks feature, P2 quality)
- [ ] Step 6: Recommend minimal next slice (one aggregate end-to-end)
- [ ] Step 7: Flag durable decisions for ADR evaluation
- [ ] Step 8: Flag verification expectations for the verifier
```

## Bounded Context Routing

- Clinical work should use `Documentation/Architecture/Domain_Overview_Business_Rules.md`.
- Persistence work should use `Documentation/Technical/migration-sql.md`.
- Financial work is deferred unless the user explicitly scopes future planning.
- Current implementation status must come from `Documentation/State.md`, not this skill.

## Entry Point Mapping

For each aggregate or module, map:

- Controller or UI entry point.
- Application service, use case, mapper, or DTO.
- Entity and invariants.
- Repository or persistence configuration.
- Tests and review prompts that should apply.

## Legacy Reference

When porting Legacy behavior, read targeted methods in `DocAPI/Legacy/_LegacySheetsDb/`, not the entire folder.

Known Atendimento validation methods include `ValidacaoEtapaConsulta`, `ValidacaoPreProcedimento`, `ValidacaoEtapaProcedimento`, and `ValidacaoEtapaPosProcedimento`.

Destination for ported clinical rules should be entity behavior or thin application use cases, not repositories or controllers.

## Output format

```markdown
# Decomposition: [area]

## Scope
...

## Findings
| Item | Location | Severity | Recommendation |
|------|----------|----------|----------------|

## Suggested next slice
1. ...

## ADR candidates
- ...

## Verification handoff
- Applicable gates:
- Review prompts:
- Residual risks:
```

## Governance Handoff

- Route durable changes to ADR evaluation using `Documentation/AI-Harness/Harness-Design/harness-architecture.md`.
- Route gate selection and skipped-gate reasoning to verifier responsibility.
- Route current branch/runtime updates to `Documentation/State.md`.
- If a feature SDD exists, treat it as the feature source of truth.

## Abstraction Gate

Before proposing new interfaces, answer:
1. Will dependency change soon?
2. Will code be reused?
3. Could shared code pollute another domain?

If any "no", prefer concrete classes or entity behavior.
