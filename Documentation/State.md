# State — Doc Organo

Operational snapshot for agents and developers. Update at the end of each work session or merged PR.

**Last updated:** 2026-06-17 (documentation alignment — Atendimento split + migration sequencing)

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

**Infraestrutura SQL** — vertical slice **Paciente** backend verified.

**Approved migration sequencing:**

```text
Paciente → Atendimento Minimal → Prontuario → Agendamento → Atendimento Workflow → WS07 Frontend
```

**Next planned implementation:** Atendimento Minimal SQL Stabilization.

Current harness phase: **Harness calibration complete (Wave 1 + Wave 2)**. Forward SDD pilots: Atendimento Minimal (next), then Prontuario, Agendamento, Atendimento Workflow.

## SDD research status

| Feature SDD | Research | Specify |
|-------------|----------|---------|
| [paciente-sql-stabilization](SDD/paciente-sql-stabilization/) | Complete (pre-calibration) | Complete — verified |
| [atendimento-minimal-sql-stabilization](SDD/atendimento-minimal-sql-stabilization/research.md) | **Complete** | Ready |
| [atendimento-workflow-stabilization](SDD/atendimento-workflow-stabilization/research.md) | **Complete** | Not ready — blocked on Minimal + Prontuario + Agendamento Verify |
| [prontuario-sql-stabilization](SDD/prontuario-sql-stabilization/research.md) | **Complete** | Ready with conditions — requires Atendimento Minimal verified |

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
| 2026-06 | Atendimento split into two SDDs: **Minimal SQL** (FK prerequisite for Prontuario/Agendamento) and **Workflow** (journey orchestration after Prontuario + Agendamento). Approved order documented in [migration-sql.md](Technical/migration-sql.md) and [PM_DocOrgano.md](Product/PM_DocOrgano.md) |

## Known blockers

- `AtendimentoRepository` stubbed — Prontuario and Agendamento cannot persist without **Atendimento Minimal** slice (valid `AtendimentoId` FK).
- Atendimento workflow rules (~700 LOC Legacy) port deferred to **atendimento-workflow-stabilization** — requires Prontuario + Agendamento SQL verified first.
- Financial tables in schema — out of scope until clinical migration completes.
- Frontend/API contract drift on Paciente nome search until WS07 aligns with collection search route.

## Verification status

| Feature | Decision | Reference |
|---------|----------|-----------|
| Paciente SQL Stabilization | Complete with accepted residual risk | [verification.md](SDD/paciente-sql-stabilization/verification.md) |

Residual risk (accepted): SQL integration environment-dependent; no controller-level API tests; WS07 frontend alignment pending; `AtualizadoPor` unset; CPF checksum not validated.

## Next steps

1. Run **Atendimento Minimal SQL Stabilization** forward SDD (Specify → Design → Tasks → Execute → Verify).
2. Re-run full 15-test Paciente suite with Docker SQL + `SA_PASSWORD` before merge (if not yet done).
3. Plan WS07 frontend alignment for retired `GET /Paciente/nome/{nome}` → collection search.
4. After Atendimento Minimal verified — run **Prontuario SQL Stabilization** forward SDD.

## Task management

- **Now:** Git branches + [PM_DocOrgano.md](Product/PM_DocOrgano.md)
- **Future:** GitHub Projects or Linear (note here when switched)

## Harness files

- [AGENTS.md](../AGENTS.md)
- [.cursor/rules/](../.cursor/rules/)
- [.cursor/skills/](../.cursor/skills/)
- [Documentation/SDD/](SDD/)
- [Documentation/Technical/migration-sql.md](Technical/migration-sql.md)
