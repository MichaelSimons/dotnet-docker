#!/usr/bin/env pwsh
param(
    # Paths to the Dockerfiles to generate.
    [string[]]$Paths
)

if ($Paths) {
    $pathArgs = " --path " + ($Paths -join " --path ")
}

& $PSScriptRoot/common/Invoke-ImageBuilder.ps1 `
    -ImageBuilderArgs "generateDockerfiles --architecture * --os-type *$pathArgs"
