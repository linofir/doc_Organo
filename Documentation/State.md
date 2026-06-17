# State — Doc Organo

Operational snapshot for agents and developers. Update at the end of each work session or merged PR.

**Last updated:** 2026-06-17 (harness calibration complete)

## Current branch

`feature/harness`

## Runtime status

| Component | Status |
|-----------|--------|
| DocAPI (SQL) | **Paciente** backend stabilized — full-field CRUD, collection search, soft delete, duplicate CPF 409, PHI-safe controller, 14 unit + 1 SQL integration test (15 total). Prontuario/Agendamento/Atendimento stubbed (DI resolves, throws until migrated) |
| DocAPI (`main`) | Functional with Google Sheets (reference only) |
| DocFront.Web | Blazor Server; expects API at `https://localhost:7004`. Still calls retired `paciente/nome/{nome}` — WS07 debt |
| SQL Server | Docker `docorgano-sql`; requires `SA_PASSWORD` env var |
| AI harness | First SDD pilot complete (Paciente SQL Stabilization): Execute + Verify done; Documentation Follow-Up complete |

## Active epic

**Infraestrutura SQL** — vertical slice **Paciente** backend verified; next: Prontuario → Agendamento → Atendimento.

Current harness phase: **Harness calibration complete (Wave 1 + Wave 2)**. Governance, templates, skills, reporting templates, and Harness Calibration Workflow aligned from [sdd-pilot-report-v1.0.md](AI-Harness/research/sdd-pilot-report-v1.0.md). Paciente SDD is pre-calibration reference. **Ready for Prontuario forward SDD.**

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
| 2026-06 | Paciente SQL Stabilization verified (REQ-001–REQ-008); `GET /Paciente/nome/{nome}` retired; collection search adopted; ADR-001 referenced, no new ADR |
| 2026-06 | Wave 1 harness calibration: SDD Pre-Execution Review, post-Verify lifecycle, Execution Prerequisites, Environment-Dependent Evidence, mandatory Documentation Follow-Up |
| 2026-06 | Wave 2 harness calibration: Definition of Done alignment, CONTRIBUTING-AI post-Verify chain, reporting templates, SDD README, Harness Calibration Workflow |

## Known blockers

- Atendimento business rules only in commented Legacy (~700 LOC) — must port to domain/use cases.
- Financial tables in schema — out of scope until clinical migration completes.
- Frontend/API contract drift on Paciente nome search until WS07 aligns with collection search route.

## Verification status

| Feature | Decision | Reference |
|---------|----------|-----------|
| Paciente SQL Stabilization | Complete with accepted residual risk | [verification.md](SDD/paciente-sql-stabilization/verification.md) |

Residual risk (accepted): SQL integration environment-dependent; no controller-level API tests; WS07 frontend alignment pending; `AtualizadoPor` unset; CPF checksum not validated.

## Next steps

1. Commit Paciente SQL stabilization implementation changes (if not yet committed).
2. Re-run full 15-test suite with Docker SQL + `SA_PASSWORD` before merge.
3. Plan WS07 frontend alignment for retired `GET /Paciente/nome/{nome}` → collection search.
4. Run `ProntuarioRepository` forward SDD (harness calibration complete; use templates under `Documentation/AI-Harness/template/sdd/`).

## Task management

- **Now:** Git branches + [PM_DocOrgano.md](Product/PM_DocOrgano.md)
- **Future:** GitHub Projects or Linear (note here when switched)

## Harness files

- [AGENTS.md](../AGENTS.md)
- [.cursor/rules/](../.cursor/rules/)
- [.cursor/skills/](../.cursor/skills/)
- [Documentation/SDD/](SDD/)
- [Documentation/Technical/migration-sql.md](Technical/migration-sql.md)
