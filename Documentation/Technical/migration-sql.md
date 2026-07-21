# SQL Migration Plan — Sheets to SQL Server

Branch: `feature/base_DB`  
Reference behavior: `main` (Google Sheets via `Legacy/_LegacySheetsDb/`)

## Order of migration

Approved sequencing (2026-06 — Research consolidation: Paciente, Atendimento Minimal/Workflow, Prontuario):

| Order | Aggregate / slice | Repository | Legacy reference | Done criteria | Status |
|-------|-------------------|------------|------------------|---------------|--------|
| 1 | **Paciente** | `PacienteRepository` | `PacienteSheetsRepository.cs` | CRUD + SQL test + Swagger | **Backend verified** (2026-06) — WS07 frontend alignment pending |
| 2 | **Atendimento Minimal** | `AtendimentoRepository` | `AtendimentoSheetsRepository.cs` (CRUD only) | CRUD + Paciente FK + Guid + SQL test + Swagger | **Backend verified** (2026-06) — WS07 frontend alignment pending |
| 3 | **Prontuario** | `ProntuarioRepository` | `ProntuarioSheetsRepository.cs` | CRUD + versioning + nested children + SQL test | **Backend verified** (2026-07-09) — 79 tests (11 unit + 27 controller + 1 SQL integration); dual-mode versioning API (ADR-006); REQ-001–REQ-018 — [verification.md](../SDD/prontuario-sql-stabilization/verification.md) |
| 4 | **Agendamento** | `AgendamentoRepository` | `AgendamentoSheetsRepository.cs` | CRUD + SQL test | **Backend verified** (2026-07-20) — 30 tests (entity + repository + controller + SQL integration); 6 endpoints with Guid routing, 3-FK validation, PHI-safe controller; Approved with Findings (F-01, F-02 low severity). WS07 frontend alignment deferred. — [verification.md](../SDD/Agendamento_Stabilization/verification.md) |
| 5 | **Atendimento Workflow** | `AtendimentoRepository` (extended) | `AtendimentoSheetsRepository.cs` (`ValidacaoEtapa*`) | Rules ported + pendências/events + workflow endpoint + characterization tests | Not started — **all prerequisites verified (Minimal + Prontuario + Agendamento)** — [research.md](../SDD/atendimento-workflow-stabilization/research.md) |

> **Prerequisite clarification:** Only **Atendimento Minimal** (order 2) is required before Prontuario and Agendamento. **Atendimento Workflow** (order 5) is **not** a prerequisite for Prontuario or Agendamento — it extends persistence with journey orchestration after those aggregates are SQL-stable.

## Migration vertical patterns (calibrated)

Reusable backend stabilization pattern after Paciente (#1) and Atendimento Minimal (#2):

| Pattern | Paciente | Atendimento Minimal | Forward aggregates (Prontuario, Agendamento) |
|---------|----------|---------------------|-----------------------------------------------|
| Repository | Persistence-only CRUD + soft delete | + list-by-parent (`GetByPacienteIdAsync`) | Adapt list/query to aggregate |
| Controller FK validation | N/A (root aggregate) | Validates `PacienteId` before create | Validate `AtendimentoId` (and other FKs) in controller |
| API contract location | Section below + SDD | Section below + SDD | Add section here when verified |
| SQL integration test | `PacienteSqlIntegrationTests` | `AtendimentoSqlIntegrationTests` | One round-trip class per aggregate |
| Runtime smoke | Verify session / manual | `scripts/api-smoke-atendimento.ps1` or Verify HTTP table | Add aggregate script or Verify table |
| Credential probe | Required before Execute | Required before Execute | Required before Execute |
| Optional test debt | Controller HTTP gap accepted | Mapping tests + soft-deleted Paciente negative | Document in SDD design Optional Test Debt |

Standalone `api-contract.md` remains deferred — migration-sql sections are authoritative until a multi-aggregate contract doc is justified.

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

### Atendimento Minimal (order #2) — verified 2026-06

```markdown
- [x] EF config reviewed (Fluent API) — `AtendimentoConfiguration`
- [x] Repository uses DocDbContext — CRUD + soft delete + list by PacienteId
- [x] Registered in Program.cs DI
- [x] Controller IDs aligned (Guid)
- [x] Paciente FK validation on create
- [x] Integration test(s) — `AtendimentoSqlIntegrationTests` with skip policy
- [x] Legacy behavior reviewed — CRUD preserve/adapt/abandon in atendimento-minimal-sql-stabilization SDD
- [x] PHI-safe controller (no Console.WriteLine)
- [x] Swagger smoke checklist — recorded in verification.md (Verify session)
- [x] Report/followUp routes explicitly out of scope (501; routes preserved)
- [ ] Front service smoke — deferred to WS07
- [x] State.md updated
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

### Prontuario (Aggregate #3) — verified 2026-07-09

```markdown
- [x] EF config reviewed (Fluent API) — `ProntuarioConfig`, child configs
- [x] Repository uses DocDbContext — CRUD + version lookup + includes for nested graph
- [x] Registered in Program.cs DI
- [x] Controller IDs aligned (Guid)
- [x] Integration test(s) — `ProntuarioSqlIntegrationTests` with skip policy; synthetic CID seed
- [x] Legacy behavior reviewed — preserve/adapt/abandon table in prontuario-sql-stabilization SDD
- [x] Atendimento Minimal prerequisite satisfied (valid AtendimentoId FK)
- [x] ADR-006 dual-mode versioning API (PUT correction + POST `/versoes` evolution) honored
- [x] D-01 structural correction contract (`UpdateProntuarioDto` — correction-safe fields only)
- [x] D-05 no-merge snapshot semantics for evolution child collections
- [x] Patient-wide Versao numbering per `PacienteId` (DQ-007 A)
- [x] PHI-safe controller (no Console.WriteLine)
- [x] Soft delete per ADR-001 — single version only
- [x] Swagger smoke checklist — **TASK-009 pending** (F-01, low severity; 27 controller unit tests mitigate)
- [ ] Front service smoke — deferred to WS07
- [x] State.md updated
```

### Agendamento (Aggregate #4) — verified 2026-07-20

```markdown
- [x] EF config reviewed (Fluent API) — `AgendamentoConfig`
- [x] Repository uses DocDbContext — CRUD + soft delete + query by name/pacienteId
- [x] Registered in Program.cs DI
- [x] Controller IDs aligned (Guid)
- [x] Integration test(s) — `AgendamentoSqlIntegrationTests` with skip policy
- [x] Legacy behavior reviewed — preserve/adapt/abandon in agendamento-sql-stabilization SDD
- [x] Atendimento Minimal prerequisite satisfied (valid AtendimentoId FK)
- [x] PHI-safe controller (no Console.WriteLine)
- [x] FK validation on create (AtendimentoId, InternacaoId, PacienteId) — 404 with structured body
- [x] Soft delete per ADR-001
- [x] Two low-severity findings (F-01, F-02) — Paciente navigation + EF relationship accepted
- [ ] Front service smoke — deferred to WS07
- [x] State.md updated
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

## Atendimento Minimal API contract (backend verified)

Single-resource retrieval:

| Method | Route | Success | Not found |
|--------|-------|---------|-----------|
| GET | `/Atendimento/{id}` | 200 + `ReadAtendimentoDto` | 404 |

Collection retrieval:

| Method | Route | Success |
|--------|-------|---------|
| GET | `/Atendimento?skip=&take=` | 200 + paginated array |
| GET | `/Atendimento/paciente/{pacienteId}` | 200 + array (multiple active journeys allowed) |

Mutations:

| Method | Route | Success | Notes |
|--------|-------|---------|-------|
| POST | `/Atendimento` | 201 + `ReadAtendimentoDto` | Invalid or missing PacienteId → 404; `etapaAtual` set to Consulta via factory |
| PUT | `/Atendimento/{id}` | 204 | Updates `MensagemParaMedico` only; missing → 404 |
| DELETE | `/Atendimento/{id}` | 204 | Soft delete (ADR-001); missing → 404 |

Deferred (routes preserved, 501 Not Implemented):

| Method | Route | Response |
|--------|-------|----------|
| GET | `/Atendimento/report-id/{id}` | 501 |
| GET | `/Atendimento/followUp-id/{id}` | 501 |

**Not in scope:** stage advancement, workflow DTOs, `ValidacaoEtapa*` — owned by atendimento-workflow-stabilization SDD.

Full design rationale: [design.md](../SDD/atendimento-minimal-sql-stabilization/design.md).

## Feature flag

```json
"Persistence": {
  "Provider": "Sql"
}
```

Not implemented yet. On `feature/base_DB`, SQL is the active target and Sheets is reference only.

## Infrastructure

```powershell
# First time: copy .env.example to .env and set SA_PASSWORD
docker compose up -d
dotnet ef database update --project DocAPI/DocAPI.csproj
dotnet test --filter FullyQualifiedName~Sql
```

Credential source: repo-root `.env` (`SA_PASSWORD`, `DB_NAME`). Docker Compose, EF tools, API, and SQL integration tests resolve the same connection via `SqlConnectionResolver`. Convenience script: `scripts/sql-integration-test.ps1`.

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
2. Atendimento Minimal slice complete — valid `AtendimentoId` FK for downstream aggregates. **Backend criteria met**; WS07 frontend smoke pending.
3. Prontuario + Agendamento CRUD on SQL.
4. Atendimento Workflow rules ported and tested (characterization + integration).
5. Security/PHI review completed for touched flows.
