param(
    [string]$DeepFaceProject = (Join-Path $PSScriptRoot '../../DeepFace'),
    # 启用时复制已有活体权重，不触发网络下载。
    [switch]$IncludeLiveness
)
$ErrorActionPreference = 'Stop'
$sourceProject = (Resolve-Path -LiteralPath $DeepFaceProject).Path
$targetRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../ClientApi/FaceRecognition'))
$sourcePython = Join-Path $sourceProject '.runtime/python/cpython-3.11-windows-x86_64-none'
$sourcePackages = Join-Path $sourceProject '.venv/Lib/site-packages'
$sourceWeights = Join-Path $sourceProject 'data/models/.deepface/weights/face_recognition_sface_2021dec.onnx'
foreach ($required in @((Join-Path $sourcePython 'python.exe'), $sourcePackages, $sourceWeights)) {
    if (-not (Test-Path -LiteralPath $required)) { throw "Missing runtime component: $required" }
}
$livenessWeightNames = @('face_detection_yunet_2023mar.onnx', '2.7_80x80_MiniFASNetV2.h5', '4_0_0_80x80_MiniFASNetV1SE.h5')
if ($IncludeLiveness) {
    foreach ($name in $livenessWeightNames) {
        $weightPath = Join-Path $sourceProject "data/models/.deepface/weights/$name"
        if (-not (Test-Path -LiteralPath $weightPath -PathType Leaf)) { throw "Missing liveness weight: $weightPath" }
    }
}
# 只复制运行环境与选定权重，不复制身份库、照片或演示页面。
New-Item -ItemType Directory -Path $targetRoot -Force | Out-Null
function Copy-RuntimeDirectory([string]$Source, [string]$Destination) {
    & robocopy.exe $Source $Destination /E /XD __pycache__ /XF *.pyc /R:1 /W:1 /NFL /NDL /NJH /NJS /NP
    if ($LASTEXITCODE -ge 8) { throw "Runtime copy failed: $Source" }
}
Copy-RuntimeDirectory $sourcePython (Join-Path $targetRoot 'runtime')
Copy-RuntimeDirectory $sourcePackages (Join-Path $targetRoot 'runtime/Lib/site-packages')
# backend 是本项目维护的源码，不从相邻 DeepFace 项目覆盖。
$weights = Join-Path $targetRoot 'data/models/.deepface/weights'
New-Item -ItemType Directory -Path $weights -Force | Out-Null
Copy-Item -LiteralPath $sourceWeights -Destination $weights -Force
if ($IncludeLiveness) {
    foreach ($name in $livenessWeightNames) {
        Copy-Item -LiteralPath (Join-Path $sourceProject "data/models/.deepface/weights/$name") -Destination $weights -Force
    }
}
Copy-Item -LiteralPath (Join-Path $sourceProject 'requirements.lock.txt') -Destination (Join-Path $targetRoot 'requirements.lock.txt') -Force
# 打包后的解释器必须从自己的路径加载依赖。
& (Join-Path $targetRoot 'runtime/python.exe') -X utf8 -c 'import sys, flask, waitress, cv2, tensorflow; print("Bundled runtime verified:", sys.executable)'
if ($LASTEXITCODE -ne 0) { throw 'Bundled Python dependency verification failed.' }
Write-Output 'Face runtime prepared. Start ClientApi normally; dotnet publish includes this runtime.'
exit 0
