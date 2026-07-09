---
paths:
  - "DocAPI/**/*.cs"
---

# EF Core — Doc Organo

- Preserve soft delete from `Documentation/Architecture/ADR/ADR-001-soft-delete.md`: `Deletado` plus global query filters.
- Use `.IgnoreQueryFilters()` only for explicit audit/report behavior and document the reason in code.
- Keep Fluent configuration files as `.cs` files so `ApplyConfigurationsFromAssembly` can discover them.
- Preserve `Endereco` as an owned Paciente type with `Endereco_*` columns unless changed by an accepted ADR or migration plan.
- Never commit real connection strings, `${SA_PASSWORD}` values, secrets, or local credentials.
- Use the `sql-migration-workflow` skill for migration procedure, characterization-test guidance, commands, verification, and documentation routing.

## Additional Context

For migration workflow:
- The `sql-migration-workflow` skill

For architecture decisions:
- `Documentation/Architecture/ADR/`