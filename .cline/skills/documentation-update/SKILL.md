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
- References to renamed, moved, or deleted artifacts.

### Multi-Tool Path Drift

Extended check for governance docs that describe Harness concepts:

- Flag `.cursor/` references in governance docs that describe Harness concepts (not Cursor-specific implementation details).
- Harness concept references should use tool-agnostic naming + parenthetical disambiguation (e.g., "Harness rules (Cursor: `.cursor/rules/*.mdc`; Cline: `.clinerules/*.md`)").
- Cursor-specific implementation descriptions (e.g., "Cursor uses `alwaysApply: true`") are correctly Cursor-specific — do not flag.
- This check is advisory — it flags potential issues for human review, not enforcement.

## Authority Validation

- ADRs override `AGENTS.md`, rules, skills, and SDD.
- `Documentation/State.md` overrides stale bootstrap context.
- Active SDD owns feature-specific truth but cannot contradict accepted ADRs.
- Rules stay concise and workflow-free.
- Skills own repeatable procedures and handoffs.
- Review prompts are sensors; verifier responsibility selects and applies them.

## Post-Verify Documentation Follow-Up (Mandatory)

After Verify for SDD-backed work, execute this checklist — not merely evaluate routing candidates. Documentation Follow-Up is mandatory execution per `Documentation/AI-Harness/Harness-Design/sdd-operational.md`.

| Document | Update when |
|----------|-------------|
| `Documentation/State.md` | Branch, runtime status, blockers, verification status, active epic, or next steps changed |
| `Documentation/Product/PM_DocOrgano.md` | Backlog item status or sequencing changed |
| `Documentation/Technical/migration-sql.md` | Persistence, migration, or aggregate status changed |
| `Documentation/Technical/runbook.md` | Runtime prerequisites, credentials, or local setup changed |

For each checklist item:

1. Read the current document.
2. Determine whether the verified feature changed operational truth.
3. Update the document or record **No update needed** with reason.

Additional follow-up beyond this checklist still routes through Routing Rules above. Routing evaluation alone is insufficient when mandatory items apply.

## Follow-Up vs Routing Evaluation

- **Routing evaluation** — decide which artifact owns a discovered fact.
- **Follow-Up execution** — apply updates to mandatory checklist documents and routed candidates.

Do not treat Documentation Follow-Up as a suggestion list. When Verify completes SDD-backed work, run the mandatory checklist unless the user explicitly defers with documented reason.

## Ownership Validation

Check whether information is stored in the correct artifact.

Common violations:

- Operational status in ADRs
- Workflow procedures in rules
- Architecture decisions in State.md
- Feature-specific truth outside an active SDD
- Durable decisions outside ADRs

## Output Format

```markdown
# Documentation Update Handoff

## Mandatory Follow-Up Checklist
- State.md: Updated / No update needed — reason
- PM_DocOrgano.md: Updated / No update needed — reason
- migration-sql.md: Updated / No update needed — reason
- runbook.md: Updated / No update needed — reason

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

## Ownership Check
- Violations:
- Follow-up: