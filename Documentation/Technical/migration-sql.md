# SQL Migration Plan — Sheets to SQL Server

Branch: `feature/base_DB`  
Reference behavior: `main` (Google Sheets via `Legacy/_LegacySheetsDb/`)

## Order of migration

Approved sequencing (2026-06 — Research consolidation: Paciente, Atendimento Minimal/Workflow, Prontuario):

| Order | Aggregate / slice | Repository | Legacy reference | Done criteria | Status |
|-------|-------------------|------------|------------------|---------------|--------|
| 1 | **Paciente** | `PacienteRepository` | `PacienteSheetsRepository.cs` | CRUD + SQL test + Swagger | **Backend verified** (2026-06) — WS07 frontend alignment pending |
| 2 | **Atendimento Minimal** | `AtendimentoRepository` | `AtendimentoSheetsRepository.cs` (CRUD only) | CRUD + Paciente FK + Guid + SQL test + Swagger | Not started — [research.md](../SDD/atendimento-minimal-sql-stabilization/research.md) |
| 3 | **Prontuario** | `ProntuarioRepository` | `ProntuarioSheetsRepository.cs` | CRUD + versioning + nested children + SQL test | Not started — **requires Atendimento Minimal verified** — [research.md](../SDD/prontuario-sql-stabilization/research.md) |
| 4 | **Agendamento** | `AgendamentoRepository` | `AgendamentoSheetsRepository.cs` | CRUD + SQL test | Not started — **requires Atendimento Minimal verified** |
| 5 | **Atendimento Workflow** | `AtendimentoRepository` (extended) | `AtendimentoSheetsRepository.cs` (`ValidacaoEtapa*`) | Rules ported + pendências/events + workflow endpoint + characterization tests | Not started — **requires Minimal + Prontuario + Agendamento verified** — [research.md](../SDD/atendimento-workflow-stabilization/research.md) |

> **Prerequisite clarification:** Only **Atendimento Minimal** (order 2) is required before Prontuario and Agendamento. **Atendimento Workflow** (order 5) is **not** a prerequisite for Prontuario or Agendamento — it extends persistence with journey orchestration after those aggregates are SQL-stable.

## Per-entity checklist

### Paciente (Aggregate #1) — verified 2026-06

```markdown
- [x] EF config reviewed (Fluent API)
- [x] Repository uses DocDbContext
- [x] Registered in Program.cs DI
- [x] Controller IDs aligned (Guid)
- [x] Integration test(s) — `PacienteSqlIntegrationTests` with skip policy
- [x] Legacy behavior reviewed — REQ-008 table in paciente-sql-stabilization SDD
- [ ] Front service smoke (if applicable) — deferred to WS07
- [x] State.md updated
```

### Atendimento Minimal (order #2)

```markdown
- [ ] EF config reviewed (Fluent API) — `AtendimentoConfiguration`
- [ ] Repository uses DocDbContext — CRUD + soft delete + list by PacienteId
- [ ] Registered in Program.cs DI
- [ ] Controller IDs aligned (Guid)
- [ ] Paciente FK validation on create
- [ ] Integration test(s) — `AtendimentoSqlIntegrationTests` with skip policy
- [ ] Legacy behavior reviewed — CRUD preserve/adapt/abandon in atendimento-minimal-sql-stabilization SDD
- [ ] PHI-safe controller (no Console.WriteLine)
- [ ] Swagger smoke checklist
- [ ] Report/followUp routes explicitly out of scope (501 or undocumented)
- [ ] Front service smoke — deferred to WS07
- [ ] State.md updated
```

### Atendimento Workflow (order #5 — extension of order #2)

```markdown
- [ ] Prerequisites verified — Atendimento Minimal, Prontuario, Agendamento
- [ ] Application Use Cases — stage evaluators (not repository, not aggregate methods)
- [ ] Pendencias + ClinicalEvents persistence on workflow refresh
- [ ] Journey projection DTO — legacy nested stage objects abandoned
- [ ] Workflow endpoint — explicit invocation (e.g. POST atualizar-jornada)
- [ ] ValidacaoEtapaPosProcedimento — net-new design (not legacy migration)
- [ ] Characterization tests for Consulta, PreProcedimento, Procedimento stages
- [ ] Cross-aggregate SQL integration tests (Paciente → Atendimento → Prontuario → Agendamento → workflow)
- [ ] Legacy behavior table — preserve/adapt/abandon per stage
- [ ] PHI-safe workflow path
- [ ] Swagger smoke for workflow endpoint
- [ ] Front service smoke — deferred to WS07 (journey projection required)
- [ ] State.md updated
```

### Template for remaining aggregates (Prontuario, Agendamento)

```markdown
- [ ] EF config reviewed (Fluent API)
- [ ] Repository uses DocDbContext
- [ ] Registered in Program.cs DI
- [ ] Controller IDs aligned (Guid)
- [ ] Integration test(s)
- [ ] Legacy behavior reviewed
- [ ] Atendimento Minimal prerequisite satisfied (valid AtendimentoId FK)
- [ ] Front service smoke (if applicable) — deferred to WS07
- [ ] State.md updated
```

## Paciente API contract (backend verified)

Single-resource retrieval:

| Method | Route | Success | Not found | Conflict |
|--------|-------|---------|-----------|----------|
| GET | `/Paciente/{id}` | 200 + `ReadPacienteDto` | 404 | — |
| GET | `/Paciente/cpf/{cpf}` | 200 + `ReadPacienteDto` | 404 | 409 if duplicate rows |

Collection search (empty result = 200 + `[]`):

| Method | Route | Success |
|--------|-------|---------|
| GET | `/Paciente?skip=&take=` | 200 + paginated array |
| GET | `/Paciente/search?nome=&skip=&take=` | 200 + partial nome match array |

Mutations:

| Method | Route | Success | Notes |
|--------|-------|---------|-------|
| POST | `/Paciente` | 201 + `ReadPacienteDto` | Duplicate CPF → 409 |
| PUT | `/Paciente/{id}` | 204 | Missing → 404 |
| DELETE | `/Paciente/{id}` | 204 | Soft delete (ADR-001); missing → 404 |

**Retired:** `GET /Paciente/nome/{nome}` single-result semantics. Frontend must adopt collection search (WS07).

Full design rationale: [design.md](../SDD/paciente-sql-stabilization/design.md).

## Feature flag

```json
"Persistence": {
  "Provider": "Sql"
}
```

Not implemented yet. On `feature/base_DB`, SQL is the active target and Sheets is reference only.

## Infrastructure

```bash
# Set password (PowerShell)
$env:SA_PASSWORD = "YourStrong!Passw0rd"

docker compose up -d
dotnet ef database update --project DocAPI/DocAPI.csproj
```

Connection: `DocAPI/appsettings.json` → `DefaultConnection`

## Atendimento rules port (Workflow slice only — order #5)

Source: `DocAPI/Legacy/_LegacySheetsDb/AtendimentoSheetsRepository.cs`

Owned by SDD: `Documentation/SDD/atendimento-workflow-stabilization/`

| Method | Target |
|--------|--------|
| `ValidacaoEtapaConsulta` | `Application/UseCases/` (stage evaluator) |
| `ValidacaoPreProcedimento` | Same |
| `ValidacaoEtapaProcedimento` | Same |
| `ValidacaoEtapaPosProcedimento` | Net-new domain design — not legacy migration |

Do not leave validation in repository. Do not implement as aggregate root methods (`atendimento.ValidacaoEtapa*()`).

**Not in scope for Atendimento Minimal (order #2):** any `ValidacaoEtapa*` logic.

## Merge criteria for `main`

Do not merge `feature/base_DB` into `main` until:

1. Paciente slice complete with SQL validation and tests. **Backend criteria met**; WS07 frontend smoke pending.
2. Atendimento Minimal slice complete — valid `AtendimentoId` FK for downstream aggregates.
3. Prontuario + Agendamento CRUD on SQL.
4. Atendimento Workflow rules ported and tested (characterization + integration).
5. Security/PHI review completed for touched flows.
