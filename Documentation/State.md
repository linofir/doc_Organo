# State — Doc Organo

Operational snapshot for agents and developers. Update at the end of each work session or merged PR.

**Last updated:** 2026-05-19

## Current branch

`feature/base_DB`

## Runtime status

| Component | Status |
|-----------|--------|
| DocAPI (SQL) | **Paciente** EF repository implemented + unit tests; Prontuario/Agendamento/Atendimento stubbed (DI resolves, throws until migrated) |
| DocAPI (`main`) | Functional with Google Sheets (reference only) |
| DocFront.Web | Blazor Server; expects API at `https://localhost:7004` |
| SQL Server | Docker `docorgano-sql`; requires `SA_PASSWORD` env var |
| AI harness | AGENTS.md, rules, skills, SDD templates, Documentation/AI-Harness — see [documentation-index.md](AI-Harness/documentation-index.md) |

## Active epic

**Infraestrutura SQL** — vertical slice **Paciente** first, then Prontuario → Agendamento → Atendimento.

## Recent decisions

| Date | Decision |
|------|----------|
| 2026-05 | Entities refactor + `InitialCreate` EF migration (`b550477`) |
| 2026-05 | ERD versioned as `Documentation/Architecture/erd.dbml` |
| 2026-05 | AI harness foundation: AGENTS.md, rules, skills, SDD templates |
| 2026-05 | Legacy Sheets repos moved to `Legacy/_LegacySheetsDb/` (commented) |

## Known blockers

- Atendimento business rules only in commented Legacy (~700 LOC) — must port to domain/use cases.
- Financial tables in schema — out of scope until clinical migration completes.
- TLC skills (`tlc-spec-driven`, `domain-analysis`) — install manually: [docs/tlc-skills-install.md](../docs/tlc-skills-install.md)

## Next steps

1. Run Paciente against Docker SQL (`docker compose up`, `dotnet ef database update`, Swagger).
2. Implement `ProntuarioRepository` (EF).
3. Port `ValidacaoEtapa*` from Legacy Atendimento repository.

## Task management

- **Now:** Git branches + [PM_DocOrgano.md](Product/PM_DocOrgano.md)
- **Future:** GitHub Projects or Linear (note here when switched)

## Harness files

- [AGENTS.md](../AGENTS.md)
- [.cursor/rules/](../.cursor/rules/)
- [.cursor/skills/](../.cursor/skills/)
- [Documentation/Technical/migration-sql.md](Technical/migration-sql.md)
