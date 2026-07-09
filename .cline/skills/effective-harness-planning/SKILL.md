---
name: effective-harness-planning
description: Reviews and plans the Doc Organo AI harness across AGENTS.md, rules, skills, PRD, ADRs, SDD templates, State.md, MCPs, and verification gates. Use when improving AI-Harness research, creating harness tasks, reviewing documentation structure, or turning research/playbook context into actionable planning artifacts.
---

# Effective Harness Planning — Doc Organo

## Purpose

Use this skill to convert research and playbook notes into a practical AI harness for Doc Organo without overloading implementation sessions.

The harness should help agents answer:

1. What context is always needed?
2. What context is task-specific?
3. What should be planned before code changes?
4. What must be verified before work is considered done?

## Start Here

Read these first:

1. `Documentation/State.md` — current branch, runtime status, blockers, next steps.
2. `AGENTS.md` — stable project context, stack, commands, architecture rules.
3. `Documentation/AI-Harness/research/AI-Research.md` — research baseline and harness roadmap.
4. `Documentation/AI-Harness/research/playbook.md` — AI methodology, RPI, context engineering, SDD.

Do not paste full research chats into implementation sessions. Use Markdown artifacts as the handoff.

## Harness Map

| Artifact | Role | Load policy |
|----------|------|-------------|
| `AGENTS.md` | Static project context | Always useful |
| `Documentation/State.md` | Current operational truth | Always read first |
| Harness rules (Cursor: `.cursor/rules/`; Cline: `.clinerules/`) | Persistent behavior and guardrails | Always or file-scoped |
| Harness skills (Cursor: `.cursor/skills/`; Cline: `.cline/skills/`) | Specialized workflows | Load by task |
| `Documentation/Product/PRD.md` | Stable WHY and product alignment | Planning/product tasks |
| `Documentation/Product/PM_DocOrgano.md` | Operational backlog and sequencing | Planning/tasks |
| `Documentation/AI-Harness/template/sdd/` | Specify/design/tasks templates | Features > half day |
| `Documentation/Architecture/ADR/` | Historical technical decisions | Architecture changes |
| `Documentation/Technical/migration-sql.md` | Sheets to SQL migration plan | Persistence tasks |
| `Documentation/AI-Harness/review-prompts/` | Review checklists | Before PR/merge |

## Multi-Tool Awareness Check

During harness reviews, evaluate whether governance documentation assumes Cursor as the only tool:

- **Tool-specific titles:** "Cursor Rules" or "Cursor Skills" where Harness concept is intended → flag for tool-agnostic naming ("Harness Rules", "Harness Skills").
- **Tool-specific paths as canonical:** `.cursor/` references describing Harness concepts without Cline disambiguation → flag for parenthetical disambiguation.
- **Single-tool assumptions in strategies:** Rules or skills strategies that assume only Cursor format → flag for generalization.
- **Context assumptions:** Context loading descriptions that assume Cursor-specific injection → flag for tool-agnostic strategy descriptions.

## Planning Workflow

Copy this checklist when reviewing or extending the harness:

```markdown
- [ ] Confirm current status from `Documentation/State.md`
- [ ] Identify whether the work is Research, Plan, or Implement
- [ ] Map the task to the minimum docs needed
- [ ] Check whether a rule, skill, ADR, PRD update, or SDD artifact is the right output
- [ ] Preserve existing information; if unclear, add a suggested clearer version
- [ ] Separate mature/static context from evolving task context
- [ ] Add verification gates appropriate to risk
- [ ] Update `Documentation/AI-Harness/research/AI-Research.md` only when research conclusions change
- [ ] Check for multi-tool assumptions: Cursor-only titles, paths, or assumptions in governance docs
```

## Decision Rules

- Use `AGENTS.md` for stable architecture, commands, task-to-doc mapping, and project facts.
- Use rules for short, persistent guardrails: security/PHI, DDD, EF migrations, front conventions, token economy.
- Use skills for repeatable workflows: SQL migration slices, legacy rule porting, codebase decomposition, harness planning.
- Use ADRs for decisions that will matter later: soft delete, SQL migration strategy, auth/RBAC, .NET upgrade.
- Use PRD for product intent and out-of-scope decisions; use PM for backlog and sequencing.
- Use `State.md` for what is true now, not for historical narrative.

## Doc Organo Priorities

Treat these as current harness risks:

- Security/PHI review is not fully enforced; avoid logging patient or clinical data.
- Auth/RBAC is a supporting capability for the MVP, not a bounded context yet.
- Tests are verification sensors, not a bounded context.
- Paciente SQL slice is the current anchor; Prontuario, Agendamento, and Atendimento are migration follow-ups.
- Legacy Sheets code is behavioral reference until SQL replacements and characterization tests exist.
- MCPs should start with GitHub and browser; custom MCPs come after the local harness is reliable.

## Output Format

When producing a harness review, use:

```markdown
# Harness Review: [scope]

## Current State
[What exists and what is stale]

## Gaps
| Priority | Gap | Evidence | Recommendation |
|----------|-----|----------|----------------|

## Recommended Artifacts
| Artifact | Action | Reason |
|----------|--------|--------|

## Next Plan Tasks
1. ...
```

## Anti-Patterns

- Do not turn every idea into an always-loaded rule.
- Do not create skills before the workflow repeats or has high risk.
- Do not let `Documentation/AI-Harness/research/AI-Research.md` become the daily state file.
- Do not use PRD, PM, RoadMap, and `State.md` interchangeably.
- Do not preserve legacy code forever; preserve behavior until replacements are tested.