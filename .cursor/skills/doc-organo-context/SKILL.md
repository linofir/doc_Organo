---
name: doc-organo-context
description: Loads Doc Organo project context for AI sessions. Use when starting work on Doc Organo, onboarding to the codebase, or when the user asks about project structure, stack, or documentation map.
---

# Doc Organo — Session Context

## Purpose

Use this skill to load the minimum useful project context at the start of a Doc Organo session. It routes to authoritative documents and should not duplicate mutable project truth.

## Start Here

1. Read [Documentation/State.md](Documentation/State.md) for current branch, runtime status, blockers, and next steps.
2. Read [AGENTS.md](AGENTS.md) for stable bootstrap, authority hierarchy, commands, and routing.
3. Use [Documentation/AI-Harness/documentation-index.md](Documentation/AI-Harness/documentation-index.md) to choose task-specific docs.

## Route By Task

| Task | Read |
|------|------|
| Domain or business rules | `Documentation/Architecture/Domain_Overview_Business_Rules.md` |
| Backend architecture | `Documentation/Architecture/Architecture_Overview.md` |
| Accepted decisions | `Documentation/Architecture/ADR/` |
| SQL migration | `Documentation/Technical/migration-sql.md` and `.cursor/skills/sql-migration-workflow/SKILL.md` |
| Frontend | `Documentation/Technical/front-architecture.md` |
| Harness operations | `Documentation/AI-Harness/CONTRIBUTING-AI.md` |
| Documentation routing | `.cursor/skills/documentation-update/SKILL.md` |

## Guardrails

- `Documentation/State.md` wins over stale bootstrap information.
- ADRs win for durable architecture decisions.
- Active SDD owns feature-specific scope and verification expectations.
- Rules are guardrails; skills are workflows.
- Legacy Sheets code is behavioral reference only unless the user explicitly asks otherwise.

## Output

Summarize only the context needed for the current task: current status from State, relevant authoritative docs, applicable rules/skills, and any known verification expectations.
