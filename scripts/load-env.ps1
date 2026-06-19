# Loads repo-root .env into the current PowerShell session (process scope).
# Usage: . .\scripts\load-env.ps1

$repoRoot = Split-Path $PSScriptRoot -Parent
$envFile = Join-Path $repoRoot ".env"

if (-not (Test-Path $envFile)) {
    Write-Error @"
.env not found at $envFile
Copy .env.example to .env and set SA_PASSWORD to the password used by docorgano-sql.
"@
    exit 1
}

Get-Content $envFile | ForEach-Object {
    $line = $_.Trim()
    if ([string]::IsNullOrWhiteSpace($line) -or $line.StartsWith("#")) {
        return
    }

    $separatorIndex = $line.IndexOf("=")
    if ($separatorIndex -le 0) {
        return
    }

    $name = $line.Substring(0, $separatorIndex).Trim()
    $value = $line.Substring($separatorIndex + 1).Trim().Trim('"')
    Set-Item -Path "Env:$name" -Value $value
}

if ([string]::IsNullOrWhiteSpace($env:SA_PASSWORD)) {
    Write-Error "SA_PASSWORD is missing in .env"
    exit 1
}

Write-Host "Loaded .env from $envFile (SA_PASSWORD is set; DB_NAME=$($env:DB_NAME))"
