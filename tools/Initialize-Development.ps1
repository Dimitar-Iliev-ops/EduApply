param([string]$DatabaseName = 'EduApplyStage1')
$ErrorActionPreference = 'Stop'
if ($DatabaseName -notmatch '^[A-Za-z][A-Za-z0-9_]{0,63}$') { throw 'Invalid local database name.' }
Push-Location (Split-Path $PSScriptRoot -Parent)
try {
    $project = 'src/EduApply.Web'
    $secretText = dotnet user-secrets list --project $project --json
    if ($LASTEXITCODE -ne 0) { throw 'Cannot read User Secrets.' }
    # The CLI can wrap JSON with comment lines.
    $jsonText = ($secretText | Where-Object { $_ -notmatch '^\s*//' }) -join "`n"
    $existing = $jsonText | ConvertFrom-Json
    $connection = "Server=(localdb)\MSSQLLocalDB;Database=$DatabaseName;Trusted_Connection=True;TrustServerCertificate=True"
    dotnet user-secrets set 'ConnectionStrings:DefaultConnection' $connection --project $project | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'Cannot save the local connection.' }
    foreach ($role in @('Applicant', 'Committee', 'Administrator')) {
        $key = "Seed:Passwords:$role"
        if (-not $existing.$key) {
            $buffer = New-Object byte[] 24
            $random = [System.Security.Cryptography.RandomNumberGenerator]::Create()
            try { $random.GetBytes($buffer) } finally { $random.Dispose() }
            $password = [Convert]::ToBase64String($buffer) + 'aA1!'
            dotnet user-secrets set $key $password --project $project | Out-Null
            if ($LASTEXITCODE -ne 0) { throw "Cannot save password for $role." }
        }
    }
    dotnet user-secrets set 'Seed:Enabled' 'true' --project $project | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'Cannot enable development seeding.' }
    SqlLocalDB start MSSQLLocalDB | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'Cannot start SQL Server LocalDB.' }
    $previousEnvironment = $env:ASPNETCORE_ENVIRONMENT
    try {
        $env:ASPNETCORE_ENVIRONMENT = 'Development'
        dotnet run --no-build --project $project -- --initialize-database
        if ($LASTEXITCODE -ne 0) { throw 'Database initialization failed.' }
    } finally { $env:ASPNETCORE_ENVIRONMENT = $previousEnvironment }
    Write-Host 'Local database and demo accounts are ready. Passwords are in User Secrets.'
} finally { Pop-Location }
