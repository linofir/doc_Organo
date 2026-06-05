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
