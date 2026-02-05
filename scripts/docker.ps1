param(
    [ValidateSet('build','run','stop')]
    [string]$Action = 'build',
    [string]$ImageName = 'fluxops-api:local'
)

$ErrorActionPreference = 'Stop'

function Build-Image {
    Write-Host "Building Docker image $ImageName..."
    docker build -f ./FluxOps.Api/Dockerfile -t $ImageName .
}

function Run-Container {
    Write-Host "Running container from $ImageName on port 8080..."
    docker run --rm -d -p 8080:8080 --name fluxops-api $ImageName
}

function Stop-Container {
    Write-Host "Stopping container fluxops-api..."
    docker stop fluxops-api | Out-Null
}

switch ($Action) {
    'build' { Build-Image }
    'run'   { Run-Container }
    'stop'  { Stop-Container }
}
