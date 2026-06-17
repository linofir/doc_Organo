# SQL Migration Plan — Sheets to SQL Server

Branch: `feature/base_DB`  
Reference behavior: `main` (Google Sheets via `Legacy/_LegacySheetsDb/`)

## Order of migration

| Order | Aggregate | Repository | Legacy reference | Done criteria | Status |
|-------|-----------|------------|------------------|---------------|--------|
| 1 | **Paciente** | `PacienteRepository` | `PacienteSheetsRepository.cs` | CRUD + SQL test + Swagger | **Backend verified** (2026-06) — WS07 frontend alignment pending |
| 2 | Prontuario | `ProntuarioRepository` | `ProntuarioSheetsRepository.cs` | CRUD + front tabs | Not started |
| 3 | Agendamento | `AgendamentoRepository` | `AgendamentoSheetsRepository.cs` | CRUD + front | Not started |
| 4 | Atendimento | `AtendimentoRepository` | `AtendimentoSheetsRepository.cs` | Rules ported + CRUD | Not started |

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

### Template for remaining aggregates

```markdown
- [ ] EF config reviewed (Fluent API)
- [ ] Repository uses DocDbContext
- [ ] Registered in Program.cs DI
- [ ] Controller IDs aligned (Guid)
- [ ] Integration test(s)
- [ ] Legacy behavior reviewed
- [ ] Front service smoke (if applicable)
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

## Atendimento rules port

Source: `DocAPI/Legacy/_LegacySheetsDb/AtendimentoSheetsRepository.cs`

| Method | Target |
|--------|--------|
| `ValidacaoEtapaConsulta` | Domain or `Application/UseCases/` |
| `ValidacaoPreProcedimento` | Same |
| `ValidacaoEtapaProcedimento` | Same |
| `ValidacaoEtapaPosProcedimento` | Same |

Do not leave validation in repository.

## Merge criteria for `main`

Do not merge `feature/base_DB` into `main` until:

1. Paciente slice complete with SQL validation and tests. **Backend criteria met**; WS07 frontend smoke pending.
2. Prontuario + Agendamento CRUD on SQL.
3. Atendimento rules ported and tested.
4. Security/PHI review completed for touched flows.
