# Rules Inventory

Harness rules (Cursor: `.cursor/rules/*.mdc`; Cline: `.clinerules/*.md`) are short, persistent guardrails. This inventory is governance metadata, not a copy of rule content.

Rules own short, persistent guardrails. Workflows belong in Harness skills (Cursor: `.cursor/skills/`; Cline: `.cline/skills/`); durable decisions belong in ADRs; current operational truth belongs in `Documentation/State.md`.

| Rule file | Purpose | Scope | Ownership | Classification | Update trigger |
|-----------|---------|-------|-----------|----------------|----------------|
| `security-phi.mdc` | Prevent PHI, credential, secret, and unsafe logging exposure | All sessions | Security invariants | Global Safety | New recurring security risk or accepted security/auth ADR |
| `token-economy.mdc` | Keep context loading focused and route workflows to skills | All sessions | Context loading guardrails | Global Context | New recurring context bloat or path-routing issue |
| `update-doc.mdc` | Route current truth, durable decisions, and documentation placement | All sessions | Documentation and ADR routing | Global Governance | ADR policy, authority hierarchy, or State ownership changes |
| `backend-architecture.mdc` | Preserve backend architecture and domain boundaries | `DocAPI/**/*.cs` | Backend architecture guardrails | Scoped Architecture | Backend boundary, domain, or accepted ADR change |
| `ef-migrations.mdc` | Preserve EF mapping, soft-delete, and migration safety invariants | `DocAPI/Infrastructure/SqlDb/**/*`, `DocAPI/Migrations/**/*` | EF persistence guardrails | Scoped Persistence | EF mapping, schema lifecycle, or migration ADR change |
| `blazor-front.mdc` | Preserve Blazor Server frontend conventions | `DocFront.Web/**/*` | Frontend architecture guardrails | Scoped Frontend | Front architecture or API integration convention change |
/Technical/api-contract.md | Public API contract documentation (future or when created) |
## Rule Governance

- Add a rule only for stable guidance that should shape agent behavior repeatedly.
- Remove or demote a rule when it becomes project-specific workflow, checklist, or temporary guidance.
- Keep rules concise and workflow-free.
- Link to canonical docs for deeper context instead of embedding long explanations.
- Move multi-step procedures, checklists, gate selection, and review orchestration to skills.
- Use review prompts as sensors; verifier workflows are responsible for selecting and applying the appropriate sensors and gates.
