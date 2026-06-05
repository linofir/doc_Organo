# ADR-001: Soft Delete with Global Query Filters

## Status

Accepted (with caveats)

## Context

Doc Organo uses soft delete (`Deletado`, `DeletadoEm`) on clinical aggregates to preserve history. EF Core global query filters hide deleted rows on Paciente, Prontuario, Agendamento and Atendimento.

During `InitialCreate` migration, EF warned about query filters with required relationships and owned types.

Original note: `Documentation/Thechnical/note_001.md`

## Decision

Keep soft delete + global query filters for clinical aggregates. Use `.IgnoreQueryFilters()` only in audit/report code paths, with a comment explaining why.

## Consequences

**Positive**

- Clinical history preserved.
- Consistent default queries exclude deleted rows.

**Negative**

- Risk of hidden parent when a related entity is soft-deleted.
- Developers must remember `IgnoreQueryFilters()` for full audits.

## Actions

| Horizon | Action |
|---------|--------|
| Short | Integration test validating filter behavior with relationships |
| Medium | Spike: shadow properties vs history table if issues appear |

## References

- `DocAPI/Infrastructure/SqlDb/DbContext/DbContext.cs` — `ApplySoftDeleteQueryFilter`
