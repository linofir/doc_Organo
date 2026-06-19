# HTTP smoke — Atendimento Minimal API (TASK-008 helper)
# Requires DocAPI running: dotnet run --project DocAPI/DocAPI.csproj
# Usage: .\scripts\api-smoke-atendimento.ps1
# Optional: .\scripts\api-smoke-atendimento.ps1 -BaseUrl "https://localhost:7004"

param(
    [string]$BaseUrl = "https://localhost:7004"
)

$ErrorActionPreference = "Stop"

function Write-JsonFileNoBom {
    param(
        [string]$Path,
        [object]$Object
    )

    $json = $Object | ConvertTo-Json -Depth 6 -Compress
    $utf8NoBom = New-Object System.Text.UTF8Encoding $false
    [System.IO.File]::WriteAllText($Path, $json, $utf8NoBom)
}

function Assert-Status {
    param(
        [string]$Label,
        [int]$Expected,
        [object]$Response
    )

    if ($Response.StatusCode -ne $Expected) {
        throw "$Label expected HTTP $Expected but got $($Response.StatusCode)"
    }

    Write-Host "PASS $Label ($Expected)"
}

$tempDir = Join-Path $env:TEMP "docorgano-smoke-atendimento"
New-Item -ItemType Directory -Force -Path $tempDir | Out-Null

$pacienteBody = @{
    nome = "Paciente Smoke Test"
    dataNascimento = "1990-01-15"
    cpf = "{0}{1}" -f (Get-Random -Minimum 100000000 -Maximum 999999999), (Get-Random -Minimum 10 -Maximum 99)
    email = "smoke.{0}@example.com" -f ([guid]::NewGuid().ToString("N").Substring(0, 8))
    telefone = "31990001111"
    rg = "MG123"
    plano = "Plano Smoke"
    carteira = "CART001"
    endereco = @{
        logradouro = "Rua Smoke"
        numero = "1"
        bairro = "Teste"
        cidade = "Belo Horizonte"
        uf = "MG"
        cep = "30130000"
    }
}

$pacientePath = Join-Path $tempDir "paciente.json"
Write-JsonFileNoBom -Path $pacientePath -Object $pacienteBody

Write-Host "Smoke base URL: $BaseUrl"

try {
    $swagger = Invoke-WebRequest -Uri "$BaseUrl/swagger/index.html" -SkipCertificateCheck
    Assert-Status "Swagger reachable" 200 $swagger

    $pacienteResponse = curl.exe -s -w "`n%{http_code}" -X POST "$BaseUrl/Paciente" `
        -H "Content-Type: application/json" `
        --data-binary "@$pacientePath"
    $pacienteLines = $pacienteResponse -split "`n"
    $pacienteStatus = [int]$pacienteLines[-1]
    if ($pacienteStatus -ne 201) { throw "Create Paciente expected 201 but got $pacienteStatus" }
    $pacienteJson = ($pacienteLines[0..($pacienteLines.Length - 2)] -join "`n") | ConvertFrom-Json
    $pacienteId = $pacienteJson.id
    Write-Host "PASS Create Paciente (201)"

    $invalidAtendPath = Join-Path $tempDir "invalid-atend.json"
    Write-JsonFileNoBom -Path $invalidAtendPath -Object @{
        pacienteId = [guid]::NewGuid().ToString()
        mensagemParaMedico = "invalid fk"
    }
    $invalidStatus = curl.exe -s -o NUL -w "%{http_code}" -X POST "$BaseUrl/Atendimento" `
        -H "Content-Type: application/json" `
        --data-binary "@$invalidAtendPath"
    if ([int]$invalidStatus -ne 404) { throw "Invalid PacienteId expected 404 but got $invalidStatus" }
    Write-Host "PASS Invalid PacienteId (404)"

    $atendPath = Join-Path $tempDir "atend.json"
    Write-JsonFileNoBom -Path $atendPath -Object @{
        pacienteId = $pacienteId
        mensagemParaMedico = "Mensagem smoke"
    }
    $createAtend = curl.exe -s -w "`n%{http_code}" -X POST "$BaseUrl/Atendimento" `
        -H "Content-Type: application/json" `
        --data-binary "@$atendPath"
    $createLines = $createAtend -split "`n"
    $createStatus = [int]$createLines[-1]
    if ($createStatus -ne 201) { throw "Create Atendimento expected 201 but got $createStatus" }
    $atendJson = ($createLines[0..($createLines.Length - 2)] -join "`n") | ConvertFrom-Json
    if ($atendJson.etapaAtual -ne "Consulta") { throw "Create Atendimento expected etapaAtual Consulta" }
    $atendimentoId = $atendJson.id
    Write-Host "PASS Create Atendimento (201, Consulta)"

    $getStatus = curl.exe -s -o NUL -w "%{http_code}" "$BaseUrl/Atendimento/$atendimentoId"
    if ([int]$getStatus -ne 200) { throw "GET Atendimento expected 200 but got $getStatus" }
    Write-Host "PASS GET by id (200)"

    $missingId = [guid]::NewGuid().ToString()
    $getMissing = curl.exe -s -o NUL -w "%{http_code}" "$BaseUrl/Atendimento/$missingId"
    if ([int]$getMissing -ne 404) { throw "GET missing expected 404 but got $getMissing" }
    Write-Host "PASS GET missing (404)"

    $listStatus = curl.exe -s -o NUL -w "%{http_code}" "$BaseUrl/Atendimento/paciente/$pacienteId"
    if ([int]$listStatus -ne 200) { throw "List by paciente expected 200 but got $listStatus" }
    Write-Host "PASS List by paciente (200)"

    $pageStatus = curl.exe -s -o NUL -w "%{http_code}" "$BaseUrl/Atendimento?skip=0&take=10"
    if ([int]$pageStatus -ne 200) { throw "Paginated list expected 200 but got $pageStatus" }
    Write-Host "PASS Paginated list (200)"

    $atend2Path = Join-Path $tempDir "atend2.json"
    Write-JsonFileNoBom -Path $atend2Path -Object @{
        pacienteId = $pacienteId
        mensagemParaMedico = "Second journey"
    }
    $create2Status = curl.exe -s -o NUL -w "%{http_code}" -X POST "$BaseUrl/Atendimento" `
        -H "Content-Type: application/json" `
        --data-binary "@$atend2Path"
    if ([int]$create2Status -ne 201) { throw "Second create expected 201 but got $create2Status" }
    Write-Host "PASS Second active journey (201)"

    $updatePath = Join-Path $tempDir "update-atend.json"
    Write-JsonFileNoBom -Path $updatePath -Object @{ mensagemParaMedico = "Updated smoke" }
    $putStatus = curl.exe -s -o NUL -w "%{http_code}" -X PUT "$BaseUrl/Atendimento/$atendimentoId" `
        -H "Content-Type: application/json" `
        --data-binary "@$updatePath"
    if ([int]$putStatus -ne 204) { throw "PUT expected 204 but got $putStatus" }
    Write-Host "PASS PUT (204)"

    $putMissing = curl.exe -s -o NUL -w "%{http_code}" -X PUT "$BaseUrl/Atendimento/$missingId" `
        -H "Content-Type: application/json" `
        --data-binary "@$updatePath"
    if ([int]$putMissing -ne 404) { throw "PUT missing expected 404 but got $putMissing" }
    Write-Host "PASS PUT missing (404)"

    $report501 = curl.exe -s -o NUL -w "%{http_code}" "$BaseUrl/Atendimento/report-id/$atendimentoId"
    if ([int]$report501 -ne 501) { throw "Report route expected 501 but got $report501" }
    Write-Host "PASS Report route (501)"

    $follow501 = curl.exe -s -o NUL -w "%{http_code}" "$BaseUrl/Atendimento/followUp-id/$atendimentoId"
    if ([int]$follow501 -ne 501) { throw "FollowUp route expected 501 but got $follow501" }
    Write-Host "PASS FollowUp route (501)"

    $deleteStatus = curl.exe -s -o NUL -w "%{http_code}" -X DELETE "$BaseUrl/Atendimento/$atendimentoId"
    if ([int]$deleteStatus -ne 204) { throw "DELETE expected 204 but got $deleteStatus" }
    Write-Host "PASS DELETE (204)"

    $getDeleted = curl.exe -s -o NUL -w "%{http_code}" "$BaseUrl/Atendimento/$atendimentoId"
    if ([int]$getDeleted -ne 404) { throw "GET after delete expected 404 but got $getDeleted" }
    Write-Host "PASS Soft delete exclusion GET (404)"

    $deleteMissing = curl.exe -s -o NUL -w "%{http_code}" -X DELETE "$BaseUrl/Atendimento/$missingId"
    if ([int]$deleteMissing -ne 404) { throw "DELETE missing expected 404 but got $deleteMissing" }
    Write-Host "PASS DELETE missing (404)"

    Write-Host "`nAll Atendimento smoke checks passed."
}
finally {
    if (Test-Path $tempDir) {
        Remove-Item -Recurse -Force $tempDir
    }
}
