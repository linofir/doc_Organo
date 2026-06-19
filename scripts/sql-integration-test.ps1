# Local SQL integration workflow: load .env, ensure Docker SQL, migrate, run SQL integration tests.
# Usage: .\scripts\sql-integration-test.ps1
# Optional filter: .\scripts\sql-integration-test.ps1 -Filter "FullyQualifiedName~AtendimentoSql"

param(
    [string]$Filter = "FullyQualifiedName~Sql"
)

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path $PSScriptRoot -Parent

. (Join-Path $PSScriptRoot "load-env.ps1")

function Invoke-RepoCommand {
    param(
        [string]$Command
    )

    Push-Location $repoRoot
    try {
        Invoke-Expression $Command
        if ($LASTEXITCODE -ne 0) {
            throw "Command failed ($LASTEXITCODE): $Command"
        }
    }
    finally {
        Pop-Location
    }
}

$previousEap = $ErrorActionPreference
$ErrorActionPreference = "Continue"
try {
    Invoke-RepoCommand "docker compose up -d"
    Invoke-RepoCommand "dotnet ef database update --project DocAPI/DocAPI.csproj"
    Invoke-RepoCommand "dotnet test DocAPI.Tests/DocAPI.Tests.csproj --filter $Filter --verbosity minimal"
}
finally {
    $ErrorActionPreference = $previousEap
}
