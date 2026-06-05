---
name: documentation-update
description: Documentation governance workflow for Doc Organo. Use when updating State, ADRs, documentation indexes, path references, or when deciding where a discovered fact belongs.
---

# Documentation Update — Doc Organo

## Purpose

Use this skill to route documentation changes to the right artifact and validate that paths and authority boundaries stay consistent.

This is a workflow skill. Do not convert it into an always-on rule.

## Start Here

1. Read `Documentation/State.md` for current operational truth.
2. Read `AGENTS.md` for the authority hierarchy.
3. Read `Documentation/AI-Harness/documentation-index.md` for navigation.
4. If a durable decision may be involved, read `Documentation/AI-Harness/Harness-Design/harness-architecture.md`.

## Routing Rules

| Change type | Owning artifact |
|-------------|-----------------|
| Current branch, runtime status, blockers, next steps | `Documentation/State.md` |
| Durable architecture decision | `Documentation/Architecture/ADR/` |
| Domain model or business rules | `Documentation/Architecture/Domain_Overview_Business_Rules.md` |
| Backend architecture explanation | `Documentation/Architecture/Architecture_Overview.md` |
| SQL migration and runtime technical guidance | `Documentation/Technical/` |
| Product intent | `Documentation/Product/PRD.md` |
| Backlog or sequencing | `Documentation/Product/PM_DocOrgano.md` |
| Harness governance design | `Documentation/AI-Harness/Harness-Design/` |
| Contribution lifecycle | `Documentation/AI-Harness/CONTRIBUTING-AI.md` |
| Rule inventory | `Documentation/AI-Harness/rules.md` |
| Feature scope, design, tasks | Active SDD |

## ADR Routing

Evaluate ADR need when a durable decision affects architecture boundaries, persistence strategy, schema lifecycle, security/auth model, public API contracts, cross-context ownership, major technology/runtime choices, or irreversible migration decisions.

Do not create ADRs for routine bug fixes, mechanical refactors, temporary branch decisions, local implementation details, or decisions already covered by accepted ADRs.

## Path Drift Check

Check touched docs, rules, and skills for:

- Old lowercase documentation-root paths that should point under `Documentation/`.
- Old studies or root-level AI research references.
- Old uppercase AI Harness research folder references.
- Old SDD template folder references.
- Old ADR folder references.
- Deprecated rule names that were replaced by current rule files.

## Authority Validation

- ADRs override `AGENTS.md`, rules, skills, and SDD.
- `Documentation/State.md` overrides stale bootstrap context.
- Active SDD owns feature-specific truth but cannot contradict accepted ADRs.
- Rules stay concise and workflow-free.
- Skills own repeatable procedures and handoffs.
- Review prompts are sensors; verifier responsibility selects and applies them.

## Output Format

```markdown
# Documentation Update Handoff

## Routing
- State:
- ADR:
- Architecture/Technical/Product:
- Harness:
- SDD:

## Path Drift
- Fixed:
- Remaining:

## Authority Check
- Conflicts found:
- Follow-up:
```
