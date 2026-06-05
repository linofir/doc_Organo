# ADR-005: Legacy Sheets as Behavioral Reference

## Status

Accepted for MVP2 migration

## Context

MVP1 validated behavior using Google Sheets repositories. On `feature/base_DB`, SQL Server and EF Core are replacing this persistence model.

Some important business behavior, especially Atendimento stage validation, still exists only in commented legacy Sheets repositories under `DocAPI/Legacy/_LegacySheetsDb/`.

## Decision

Treat Legacy Sheets code as behavioral reference until SQL replacements and tests cover the same behavior.

Do not re-enable Google Sheets on `feature/base_DB` unless explicitly requested and documented. Do not delete legacy code until equivalent SQL behavior is implemented and verified.

## Consequences

**Positive**

- Preserves validated MVP1 behavior during migration.
- Supports characterization tests before rewriting risky rules.
- Reduces chance of losing clinical workflow rules.

**Negative**

- Legacy commented code can confuse agents and developers if not clearly marked.
- Requires discipline to avoid copying repository-level business rules into new EF repositories.

## Actions

| Horizon | Action |
|---------|--------|
| Short | Use Legacy only as reference, not runtime dependency |
| Short | Create characterization tests for Atendimento validations |
| Medium | Port rules to domain methods or application use cases |
| Medium | Remove legacy code only after replacements are verified |

## References

- `DocAPI/Legacy/_LegacySheetsDb/`
- `Documentation/Technical/migration-sql.md`
- `Documentation/Architecture/Domain_Overview_Business_Rules.md`
