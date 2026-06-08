# Documentation Index — Doc Organo

This index is navigation, not authority. When documents conflict, use the authority hierarchy in `AGENTS.md` and the Harness Design docs.

Canonical documentation lives under `Documentation/`.

## Folder Roles

| Path | Role |
|------|------|
| `Documentation/Product/` | Product intent, backlog, roadmap |
| `Documentation/Architecture/` | Domain model, architecture, schema, ADRs |
| `Documentation/Technical/` | Runtime, migration, frontend, API technical guidance |
| `Documentation/AI-Harness/` | Harness design, contribution workflow, research, rules index, review prompts, SDD templates |
| `.cursor/` | Machine-facing rules and skills |

## Read First

| Doc | Purpose |
|-----|---------|
| [../State.md](../State.md) | Operational truth: current branch, runtime status, blockers, next steps |
| [../../AGENTS.md](../../AGENTS.md) | Stable agent bootstrap and routing |
| [../Product/PRD.md](../Product/PRD.md) | Stable product intent |
| [../Product/PM_DocOrgano.md](../Product/PM_DocOrgano.md) | Backlog and sequencing |

## Product

| Doc | Purpose |
|-----|---------|
| [../Product/PRD.md](../Product/PRD.md) | Product requirements and stable WHY |
| [../Product/PM_DocOrgano.md](../Product/PM_DocOrgano.md) | Operational planning and MVP backlog |
| [../Product/RoadMap.md](../Product/RoadMap.md) | Future product horizons |
| [../../Readme.md](../../Readme.md) | Repository onboarding overview |

## Architecture

| Doc | Purpose |
|-----|---------|
| [../Architecture/Domain_Overview_Business_Rules.md](../Architecture/Domain_Overview_Business_Rules.md) | Canonical domain language and business rules |
| [../Architecture/Architecture_Overview.md](../Architecture/Architecture_Overview.md) | Current architecture overview |
| [../Architecture/erd.dbml](../Architecture/erd.dbml) | Database schema DBML |
| [../Architecture/ADR/](../Architecture/ADR/) | Accepted architecture decision records |

## Technical

| Doc | Purpose |
|-----|---------|
| [../Technical/migration-sql.md](../Technical/migration-sql.md) | Sheets to SQL migration plan |
| [../Technical/front-architecture.md](../Technical/front-architecture.md) | Blazor Server frontend architecture |
| [../Technical/runbook.md](../Technical/runbook.md) | Local development runbook |
| `../Technical/api-contract.md | Public API contract documentation (future or when created) |

## ## Harness Design (Governance Source)

| Doc | Purpose |
|-----|---------|
| [Harness-Design/harness-architecture.md](Harness-Design/harness-architecture.md) | Target harness architecture, authority hierarchy, ADR policy, verifier model |
| [Harness-Design/agents-strategy.md](Harness-Design/agents-strategy.md) | `AGENTS.md` responsibility model |
| [Harness-Design/rules-strategy.md](Harness-Design/rules-strategy.md) | Rule ownership and growth policy |
| [Harness-Design/skills-strategy.md](Harness-Design/skills-strategy.md) | Skill ownership and candidate workflows |
| [Harness-Design/sdd-adoption-roadmap.md](Harness-Design/sdd-adoption-roadmap.md) | SDD, testing, verification, security, MCP adoption |

## AI Harness Operations

| Doc | Purpose |
|-----|---------|
| [research/AI-Research.md](research/AI-Research.md) | Research baseline and adoption risks |
| [research/playbook.md](research/playbook.md) | RPI and context engineering methodology |
| [CONTRIBUTING-AI.md](CONTRIBUTING-AI.md) | AI contribution lifecycle |
| [rules.md](rules.md) | Cursor rule inventory and governance metadata |
| [review-prompts/domain-review.md](review-prompts/domain-review.md) | Domain review sensor |
| [review-prompts/security-phi-review.md](review-prompts/security-phi-review.md) | Security and PHI review sensor |
| [review-prompts/check-docs.md](review-prompts/check-docs.md) | Documentation review sensor |
| [template/sdd/specify.md](template/sdd/specify.md) | SDD specify template |
| [template/sdd/design.md](template/sdd/design.md) | SDD design template |
| [template/sdd/tasks.md](template/sdd/tasks.md) | SDD tasks template |

## Cursor Artifacts

| Path | Purpose |
|------|---------|
| [../../.cursor/rules/](../../.cursor/rules/) | Short persistent guardrails |
| [../../.cursor/skills/](../../.cursor/skills/) | Repeatable workflows |

## Ownership Reminder

- Use ADRs for durable decisions.
- Use `Documentation/State.md` for current operational truth.
- Use SDD for feature-specific truth.
- Use rules for guardrails.
- Use skills for workflows.
- Do not store operational truth in AGENTS.md, rules, skills, or contribution guides.


