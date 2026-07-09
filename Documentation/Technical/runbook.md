# Runbook — Local Development

## Prerequisites

- .NET 7 SDK
- Docker Desktop (WSL2 on Windows recommended)
- Cursor / Cline / VS Code

## SQL Server

Use a single local credential source: repo-root `.env` (gitignored). Docker Compose loads it automatically.

```powershell
# First time only
Copy-Item .env.example .env
# Edit .env and set SA_PASSWORD to the password used by docorgano-sql

docker compose up -d
```

Container: `docorgano-sql` on port `1433`.

**Important:** If the container volume already exists, `SA_PASSWORD` in `.env` must match the password used when the volume was first created. To reset SQL data and adopt a new password: `docker compose down -v`, then update `.env`, then `docker compose up -d`.

For manual shell sessions (dotnet test / ef / run without relying on auto-discovery):

```powershell
. .\scripts\load-env.ps1
```

## Database migrations

```bash
dotnet ef database update --project DocAPI/DocAPI.csproj
```

Connection resolution order: `DOCORGANO_TEST_CONNECTION` override → `SA_PASSWORD` from environment or repo-root `.env` → `ConnectionStrings:DefaultConnection` from user-secrets (legacy fallback).

## Run API

```bash
dotnet run --project DocAPI/DocAPI.csproj
```

Swagger (Development): `/swagger`

## Run frontend

```bash
dotnet run --project DocFront.Web/DocFront.Web.csproj
```

Ensure `ApiSettings:BaseUrl` matches API HTTPS port.

## Tests

```bash
dotnet test
```

### SQL integration tests

Integration tests in `DocAPI.Tests/Integration/` require a reachable SQL Server instance. They skip (do not fail) when prerequisites are missing.

**Prerequisites:**

1. `.env` exists at repo root with `SA_PASSWORD` (copy from `.env.example`)
2. Docker container `docorgano-sql` running
3. Database migrated: `dotnet ef database update --project DocAPI/DocAPI.csproj`

**Recommended one-command workflow:**

```powershell
.\scripts\sql-integration-test.ps1
```

Filter example:

```powershell
.\scripts\sql-integration-test.ps1 -Filter "FullyQualifiedName~AtendimentoSql"
```

**Manual workflow:**

```powershell
. .\scripts\load-env.ps1
docker compose up -d
dotnet ef database update --project DocAPI/DocAPI.csproj
dotnet test DocAPI.Tests/DocAPI.Tests.csproj --filter FullyQualifiedName~Sql
```

**Optional override:** set `DOCORGANO_TEST_CONNECTION` to a full SQL Server connection string when not using the Docker default.

Expected when Docker and `.env` are aligned: all unit tests pass and SQL integration tests pass. If `.env` is missing or `SA_PASSWORD` does not match the container volume, integration tests skip with the setup hint from `SqlConnectionResolver`. If Docker is running but login fails, skip messages distinguish **credential mismatch** from **unreachable host** (see `SqlIntegrationTestGate`).

### Credential probe (before Execute)

Quick check before SQL-backed SDD Execute:

```powershell
. .\scripts\load-env.ps1
docker compose ps
dotnet ef database update --project DocAPI/DocAPI.csproj
dotnet test DocAPI.Tests/DocAPI.Tests.csproj --filter "FullyQualifiedName~Sql" --verbosity minimal
```

If tests skip with a login/password message, align `.env` with the Docker volume — not a Docker outage.

### API HTTP smoke (TASK-008)

DocAPI must be running (`dotnet run --project DocAPI/DocAPI.csproj`).

```powershell
.\scripts\api-smoke-atendimento.ps1
# Optional base URL:
.\scripts\api-smoke-atendimento.ps1 -BaseUrl "https://localhost:7004"
```

Record durable results in `verification.md` during Verify. For manual curl smoke, write JSON with **UTF-8 without BOM**:

```powershell
$path = "payload.json"
$json = '{"pacienteId":"...","mensagemParaMedico":"test"}'
$utf8NoBom = New-Object System.Text.UTF8Encoding $false
[System.IO.File]::WriteAllText($path, $json, $utf8NoBom)
curl.exe -X POST "https://localhost:7004/Atendimento" -H "Content-Type: application/json" --data-binary "@$path"
```

Do **not** use `Set-Content -Encoding UTF8` for API JSON on Windows — it adds a BOM and breaks ASP.NET model binding.

## CLI — PDF extract

```bash
dotnet run --project DocAPI/DocAPI.csproj -- --extract
```

## Troubleshooting

| Issue | Check |
|-------|-------|
| Cannot connect SQL | Docker running, `.env` `SA_PASSWORD` matches container volume, port 1433 free |
| Integration tests skip | Read skip message: credential hint → fix `.env`; login failure → password/volume mismatch; unreachable host → start Docker |
| API POST returns 400 on valid JSON | PowerShell wrote UTF-8 BOM — use `[System.IO.File]::WriteAllText` with UTF8Encoding `$false` or `scripts/api-smoke-*.ps1` |
| `dotnet ef` works but tests skip | User-secrets had a different connection; align `.env` and remove conflicting user-secrets override |
| API DI error | `Program.cs` repository registrations and migration status |
| Front cannot reach API | CORS/HTTPS URL in `appsettings.json` |
