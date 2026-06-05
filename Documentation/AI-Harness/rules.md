# Cursor Rules Inventory

Rules live in [.cursor/rules/](../../.cursor/rules/). This inventory is governance metadata, not a copy of rule content.

Rules own short, persistent guardrails. Workflows belong in skills under `.cursor/skills/`; durable decisions belong in ADRs; current operational truth belongs in `Documentation/State.md`.

| Rule file | Purpose | Scope | Ownership | Classification | Update trigger |
|-----------|---------|-------|-----------|----------------|----------------|
| `security-phi.mdc` | Prevent PHI, credential, secret, and unsafe logging exposure | All sessions | Security invariants | Always-on safety | New recurring security risk or accepted security/auth ADR |
| `token-economy.mdc` | Keep context loading focused and route workflows to skills | All sessions | Context loading guardrails | Always-on context | New recurring context bloat or path-routing issue |
| `update-doc.mdc` | Route current truth, durable decisions, and documentation placement | All sessions | Documentation and ADR routing | Always-on governance | ADR policy, authority hierarchy, or State ownership changes |
| `backend-architecture.mdc` | Preserve backend architecture and domain boundaries | `DocAPI/**/*.cs` | Backend architecture guardrails | Scoped architecture | Backend boundary, domain, or accepted ADR change |
| `ef-migrations.mdc` | Preserve EF mapping, soft-delete, and migration safety invariants | `DocAPI/Infrastructure/SqlDb/**/*`, `DocAPI/Migrations/**/*` | EF persistence guardrails | Scoped persistence | EF mapping, schema lifecycle, or migration ADR change |
| `blazor-front.mdc` | Preserve Blazor Server frontend conventions | `DocFront.Web/**/*` | Frontend architecture guardrails | Scoped frontend | Front architecture or API integration convention change |

## Rule Governance

- Add a rule only for stable guidance that should shape agent behavior repeatedly.
- Keep rules concise and workflow-free.
- Link to canonical docs for deeper context instead of embedding long explanations.
- Move multi-step procedures, checklists, gate selection, and review orchestration to skills.
- Use review prompts as sensors and the verifier concept to decide which sensors apply.
