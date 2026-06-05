# SQL Migration Plan — Sheets to SQL Server

Branch: `feature/base_DB`  
Reference behavior: `main` (Google Sheets via `Legacy/_LegacySheetsDb/`)

## Order of migration

| Order | Aggregate | Repository | Legacy reference | Done criteria |
|-------|-----------|------------|------------------|---------------|
| 1 | **Paciente** | `PacienteRepository` | `PacienteSheetsRepository.cs` | CRUD + SQL test + Swagger |
| 2 | Prontuario | `ProntuarioRepository` | `ProntuarioSheetsRepository.cs` | CRUD + front tabs |
| 3 | Agendamento | `AgendamentoRepository` | `AgendamentoSheetsRepository.cs` | CRUD + front |
| 4 | Atendimento | `AtendimentoRepository` | `AtendimentoSheetsRepository.cs` | Rules ported + CRUD |

## Per-entity checklist

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

1. Paciente slice complete with SQL validation and tests.
2. Prontuario + Agendamento CRUD on SQL.
3. Atendimento rules ported and tested.
4. Security/PHI review completed for touched flows.
