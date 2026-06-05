# ADR-002: SQL Server and EF Core Migration Strategy

## Status

Accepted for MVP2

## Context

MVP1 used Google Sheets as persistence to validate the product quickly. MVP2 needs a more reliable persistence model for clinical data, query consistency, relationships, tests and future growth.

The active branch `feature/base_DB` migrates persistence to SQL Server with EF Core. The `main` branch remains the behavioral reference for the Google Sheets implementation.

## Decision

Migrate incrementally from Google Sheets to SQL Server + EF Core by vertical clinical slices:

1. Paciente.
2. Prontuario.
3. Agendamento.
4. Atendimento.

Do not expand the Financial domain until the clinical migration is stable.

## Consequences

**Positive**

- Lower risk than rewriting all repositories at once.
- Each aggregate can be validated against legacy behavior.
- SQL integration tests can grow incrementally.

**Negative**

- During migration, some repositories may be implemented while others remain stubs.
- Documentation and `State.md` must be kept current to avoid agents assuming everything is production-ready.

## Actions

| Horizon | Action |
|---------|--------|
| Short | Validate Paciente against Docker SQL and tests |
| Short | Implement Prontuario and Agendamento repositories |
| Medium | Port Atendimento rules from Legacy before marking Atendimento stable |
| Medium | Add SQL integration tests for each migrated aggregate |

## References

- `Documentation/Technical/migration-sql.md`
- `Documentation/State.md`
- `DocAPI/Legacy/_LegacySheetsDb/`
