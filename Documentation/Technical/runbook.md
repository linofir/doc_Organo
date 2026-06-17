# Runbook — Local Development

## Prerequisites

- .NET 7 SDK
- Docker Desktop (WSL2 on Windows recommended)
- Cursor / VS Code

## SQL Server

```powershell
$env:SA_PASSWORD = "YourStrong!Passw0rd"
docker compose up -d
```

Container: `docorgano-sql` on port `1433`.

## Database migrations

```bash
dotnet ef database update --project DocAPI/DocAPI.csproj
```

Connection string: `DocAPI/appsettings.json` (uses `${SA_PASSWORD}`).

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

### SQL integration tests (Paciente)

Integration tests in `DocAPI.Tests/Integration/` require a reachable SQL Server instance. They skip (do not fail) when prerequisites are missing.

**Prerequisites:**

1. Docker container `docorgano-sql` running
2. `SA_PASSWORD` set (same value used for `docker compose` and API)
3. Database migrated: `dotnet ef database update --project DocAPI/DocAPI.csproj`

**Optional override:** set `DOCORGANO_TEST_CONNECTION` to a full SQL Server connection string when not using the Docker default.

```powershell
$env:SA_PASSWORD = "YourStrong!Passw0rd"
# Optional:
# $env:DOCORGANO_TEST_CONNECTION = "Server=localhost,1433;Database=DocOrgano;User Id=sa;Password=...;TrustServerCertificate=True"

docker compose up -d
dotnet ef database update --project DocAPI/DocAPI.csproj
dotnet test DocAPI.Tests/DocAPI.Tests.csproj --filter PacienteSql
```

Expected: 14 unit tests pass; 1 SQL integration test passes when Docker is available, or skips with reason when not.

## CLI — PDF extract

```bash
dotnet run --project DocAPI/DocAPI.csproj -- --extract
```

## Troubleshooting

| Issue | Check |
|-------|-------|
| Cannot connect SQL | Docker running, `SA_PASSWORD` set, port 1433 free |
| API DI error | `Program.cs` repository registrations and migration status |
| Front cannot reach API | CORS/HTTPS URL in `appsettings.json` |
