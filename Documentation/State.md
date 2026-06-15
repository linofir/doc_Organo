# State — Doc Organo

Operational snapshot for agents and developers. Update at the end of each work session or merged PR.

**Last updated:** 2026-06-11

## Current branch

`feature/harness`

## Runtime status

| Component | Status |
|-----------|--------|
| DocAPI (SQL) | **Paciente** EF repository implemented + unit tests; Prontuario/Agendamento/Atendimento stubbed (DI resolves, throws until migrated) |
| DocAPI (`main`) | Functional with Google Sheets (reference only) |
| DocFront.Web | Blazor Server; expects API at `https://localhost:7004` |
| SQL Server | Docker `docorgano-sql`; requires `SA_PASSWORD` env var |
| AI harness | SDD pilot preparation artifacts created; governance and v1 templates live under [AI-Harness](AI-Harness/documentation-index.md); Feature SDDs live under [SDD/](SDD/) |

## Active epic

**Infraestrutura SQL** — vertical slice **Paciente** first, then Prontuario → Agendamento → Atendimento.

Current harness phase: **SDD pilot preparation**. Governance and template v1 artifacts are prepared for review before pilot execution.

## Recent decisions

| Date | Decision |
|------|----------|
| 2026-05 | Entities refactor + `InitialCreate` EF migration (`b550477`) |
| 2026-05 | ERD versioned as `Documentation/Architecture/erd.dbml` |
| 2026-05 | AI harness foundation: AGENTS.md, rules, skills, SDD templates |
| 2026-05 | Legacy Sheets repos moved to `Legacy/_LegacySheetsDb/` (commented) |
| 2026-06 | SDD Operational Governance established as the canonical governance document |
| 2026-06 | Feature SDD location set to `Documentation/SDD/<feature-slug>/`; generated SDD, verification, reporting, and lessons-learned artifacts should be written in English |
| 2026-06 | Verification Governance v1, Reporting Strategy v1, Template Architecture, and v1 verification/reporting templates prepared for first SDD pilot |

## Known blockers

- Atendimento business rules only in commented Legacy (~700 LOC) — must port to domain/use cases.
- Financial tables in schema — out of scope until clinical migration completes.

## Next steps

1. Review the SDD pilot preparation artifacts and v1 templates.
2. Run Paciente retrospective calibration in a separate execution session.
3. Run `ProntuarioRepository` forward SDD pilot in a separate execution session.
4. After the pilot, adjust and validate templates based on real friction and verifier/reporting evidence.

Out of scope for the current preparation execution:

- Do not implement Paciente retrospective calibration yet.
- Do not implement the `ProntuarioRepository` forward SDD pilot yet.
- Do not create feature-specific Paciente or Prontuario SDD folders yet.
- Do not implement application code as part of this preparation pass.

## Task management

- **Now:** Git branches + [PM_DocOrgano.md](Product/PM_DocOrgano.md)
- **Future:** GitHub Projects or Linear (note here when switched)

## Harness files

- [AGENTS.md](../AGENTS.md)
- [.cursor/rules/](../.cursor/rules/)
- [.cursor/skills/](../.cursor/skills/)
- [Documentation/SDD/](SDD/)
- [Documentation/Technical/migration-sql.md](Technical/migration-sql.md)
