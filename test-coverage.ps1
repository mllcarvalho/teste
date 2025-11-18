# Caminho do projeto de testes unitários
$testProject = "tests/Oficina.Tests/Oficina.Tests.csproj"

# Diretórios de resultado
$resultsDir = "tests/TestResults"
$coverageReportDir = "$resultsDir/CoverageReport"

# Limpa resultados antigos
if (Test-Path $resultsDir) {
    Write-Host "🧹 Limpando resultados anteriores..."
    Remove-Item -Recurse -Force $resultsDir
}

# Cria diretórios
New-Item -ItemType Directory -Path $resultsDir -Force | Out-Null
New-Item -ItemType Directory -Path $coverageReportDir -Force | Out-Null

Write-Host "`n🧪 Executando testes unitários com cobertura..."

# Arquivo de cobertura
$projectName = [System.IO.Path]::GetFileNameWithoutExtension($testProject)
$coverageFile = Join-Path (Resolve-Path $resultsDir) "$($projectName)_coverage.cobertura.xml"

dotnet test $testProject `
    --logger "trx;LogFileName=$($projectName)_results.trx" `
    /p:CollectCoverage=true `
    /p:CoverletOutputFormat=cobertura `
    /p:CoverletOutput=$coverageFile

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Falha ao executar testes em $projectName." -ForegroundColor Red
    exit $LASTEXITCODE
}

if (-not (Test-Path $coverageFile)) {
    Write-Host "❌ Arquivo de cobertura não encontrado: $coverageFile" -ForegroundColor Red
    exit 1
}

# Diretórios a excluir da cobertura
$excludes = @(
    "Oficina.Estoque.Infrastructure",
    "Oficina.Common.Infrastructure",
    "Oficina.Atendimento.Infrastructure",
    "Oficina.Estoque.Infrastructure\Data",
    "Oficina.Estoque.Infrastructure\Migrations",
    "Oficina.Common.Infrastructure\Data",
    "Oficina.Common.Infrastructure\Migrations",
    "Oficina.Atendimento.Infrastructure\Data",
    "Oficina.Atendimento.Infrastructure\Migrations",
    "Oficina.Common.Infrastructure\Repository\GenericRepository"
)

# Função para filtrar arquivos indesejados de cobertura
function Filter-CoverageXml($xmlPath, $excludes) {
    [xml]$xml = Get-Content $xmlPath
    $removedFiles = @()

    foreach ($class in $xml.SelectNodes("//class")) {
        foreach ($pattern in $excludes) {
            if ($class.filename -like "*$pattern*") {
                $removedFiles += $class.filename
                $null = $class.ParentNode.RemoveChild($class)
                break
            }
        }
    }

    return @{ Xml = $xml; RemovedFiles = $removedFiles }
}

Write-Host "`n🔗 Aplicando filtros na cobertura..."
$result = Filter-CoverageXml $coverageFile $excludes
$combinedXml = $result.Xml
$removedFiles = $result.RemovedFiles

# Recalcular cobertura
$allLines = $combinedXml.SelectNodes("//line")
$linesCovered = ($allLines | Where-Object { [int]$_.'hits' -gt 0 }).Count
$linesValid   = $allLines.Count
$totalPercent = if ($linesValid -gt 0) { [math]::Round(($linesCovered / $linesValid) * 100, 2) } else { 0 }

# Salvar XML filtrado
$filteredCoverage = Join-Path (Resolve-Path $coverageReportDir) "coverage_filtered.cobertura.xml"
$combinedXml.Save($filteredCoverage)
Write-Host "`n✅ Arquivo filtrado salvo em: $filteredCoverage" -ForegroundColor Green

# Gerar relatório HTML
reportgenerator `
    "-reports:$filteredCoverage" `
    "-targetdir:$coverageReportDir" `
    "-reporttypes:Html" `
    | Out-Null

Write-Host "`n📊 Relatório HTML final disponível em:"
Write-Host "   $coverageReportDir/index.html" -ForegroundColor Cyan

# Mostrar cobertura no terminal
function Write-Coverage($text, $percent) {
    if ($percent -ge 80) { Write-Host $text -ForegroundColor Green }
    elseif ($percent -ge 50) { Write-Host $text -ForegroundColor Yellow }
    else { Write-Host $text -ForegroundColor Red }
}

Write-Host "`n📈 Cobertura total combinada (apenas arquivos válidos):"
Write-Coverage " - $totalPercent%" $totalPercent

# Threshold global
$threshold = 80
if ($totalPercent -lt $threshold) {
    Write-Host "`n❌ Cobertura abaixo do limite mínimo ($threshold%)" -ForegroundColor Red
    exit 1
}

Write-Host "`n✅ Testes unitários e verificação de cobertura concluídos com sucesso!" -ForegroundColor Green
