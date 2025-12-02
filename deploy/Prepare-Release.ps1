<#
.SYNOPSIS
    準備 EasyBrailleEdit 發布檔案，將建置輸出複製到 Inno Setup 安裝程式目錄。

.DESCRIPTION
    此腳本會從 Release 建置輸出目錄複製必要的檔案到 deploy/InnoSetup/Files/app 目錄，
    以便後續使用 Inno Setup 製作安裝程式。

    主要功能：
    - 複製所有可執行檔（.exe）和函式庫（.dll）
    - 複製設定檔和資源檔
    - 複製多語言資源目錄
    - 複製 Windows 平台的 runtimes 目錄（排除其他平台）
    - 排除不需要的檔案（.pdb 除錯符號）
    - 清空目標目錄（保留手動維護的文件）

.NOTES
    File Name      : Prepare-Release.ps1
    Prerequisite   : PowerShell 5.1 或更高版本
    
.EXAMPLE
    .\deploy\Prepare-Release.ps1
    執行此腳本以準備發布檔案
#>

[CmdletBinding()]
param()

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# =============================================================================
# 設定路徑
# =============================================================================

$scriptDir = $PSScriptRoot
$projectRoot = Split-Path $scriptDir -Parent

# 建置輸出目錄
$sourceDir = Join-Path $projectRoot "output\EasyBrailleEdit\Release\net10.0-windows10.0.17763.0"

# Inno Setup 目標目錄
$targetDir = Join-Path $scriptDir "InnoSetup\Files\app"

# =============================================================================
# 檔案篩選規則
# =============================================================================

# 需要排除的檔案擴展名
$excludeExtensions = @('.pdb')

# 需要保留的手動維護檔案（不會被清空）
$preserveFiles = @('LICENSE.md', 'ReleaseNote.txt')

# 需要排除的 runtimes 平台（只保留 Windows 平台）
$excludeRuntimePlatforms = @(
    'android-*', 'linux-*', 'osx-*', 'maccatalyst-*', 'unix'
)

# =============================================================================
# 函式定義
# =============================================================================

function Write-ColorMessage {
    param(
        [string]$Message,
        [string]$Color = 'White'
    )
    Write-Host $Message -ForegroundColor $Color
}

function Test-Prerequisites {
    Write-ColorMessage "`n[檢查前置條件]" -Color Cyan
    
    # 檢查來源目錄是否存在
    if (-not (Test-Path $sourceDir)) {
        throw "找不到建置輸出目錄: $sourceDir`n請先執行 Release 建置。"
    }
    
    # 檢查必要的可執行檔是否存在
    $requiredFiles = @('EasyBrailleEdit.exe', 'Txt2Brl.exe')
    foreach ($file in $requiredFiles) {
        $filePath = Join-Path $sourceDir $file
        if (-not (Test-Path $filePath)) {
            throw "找不到必要檔案: $file`n請確認 Release 建置是否成功。"
        }
    }
    
    Write-ColorMessage "✓ 前置條件檢查通過" -Color Green
}

function Clear-TargetDirectory {
    Write-ColorMessage "`n[清理目標目錄]" -Color Cyan
    
    # 建立目標目錄（如果不存在）
    if (-not (Test-Path $targetDir)) {
        New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
        Write-ColorMessage "✓ 已建立目標目錄: $targetDir" -Color Green
        return
    }
    
    # 刪除所有檔案和目錄，但保留手動維護的檔案
    Get-ChildItem -Path $targetDir -Recurse | ForEach-Object {
        $relativePath = $_.FullName.Substring($targetDir.Length + 1)
        
        # 檢查是否為需要保留的檔案
        $shouldPreserve = $false
        foreach ($preserveFile in $preserveFiles) {
            if ($relativePath -eq $preserveFile) {
                $shouldPreserve = $true
                Write-ColorMessage "  保留: $relativePath" -Color Yellow
                break
            }
        }
        
        if (-not $shouldPreserve) {
            Remove-Item $_.FullName -Recurse -Force
        }
    }
    
    Write-ColorMessage "✓ 目標目錄已清理" -Color Green
}

function Copy-BuildOutput {
    Write-ColorMessage "`n[複製建置輸出]" -Color Cyan
    
    $copiedCount = 0
    $skippedCount = 0
    
    # 取得所有檔案（不包含目錄）
    Get-ChildItem -Path $sourceDir -File | ForEach-Object {
        $fileName = $_.Name
        $extension = $_.Extension
        
        # 檢查是否需要排除
        if ($excludeExtensions -contains $extension) {
            Write-Verbose "  跳過: $fileName ($extension 檔案)"
            $skippedCount++
            return
        }
        
        # 複製檔案
        $targetPath = Join-Path $targetDir $fileName
        Copy-Item $_.FullName -Destination $targetPath -Force
        Write-Verbose "  複製: $fileName"
        $copiedCount++
    }
    
    Write-ColorMessage "✓ 已複製 $copiedCount 個檔案（跳過 $skippedCount 個）" -Color Green
}

function Copy-Subdirectories {
    Write-ColorMessage "`n[複製子目錄]" -Color Cyan
    
    # 複製所有子目錄（除了 runtimes）
    Get-ChildItem -Path $sourceDir -Directory | Where-Object { $_.Name -ne 'runtimes' } | ForEach-Object {
        $subDirName = $_.Name
        $targetSubDir = Join-Path $targetDir $subDirName
        
        Copy-Item $_.FullName -Destination $targetSubDir -Recurse -Force
        Write-Verbose "  複製目錄: $subDirName"
    }
}

function Copy-RuntimesDirectory {
    Write-ColorMessage "`n[複製 runtimes 目錄（僅 Windows 平台）]" -Color Cyan
    
    $runtimesSource = Join-Path $sourceDir 'runtimes'
    
    if (-not (Test-Path $runtimesSource)) {
        Write-ColorMessage "  找不到 runtimes 目錄，跳過" -Color Yellow
        return
    }
    
    $runtimesTarget = Join-Path $targetDir 'runtimes'
    New-Item -ItemType Directory -Path $runtimesTarget -Force | Out-Null
    
    $copiedPlatforms = 0
    $skippedPlatforms = 0
    
    # 複製 Windows 平台的 runtimes
    Get-ChildItem -Path $runtimesSource -Directory | ForEach-Object {
        $platformName = $_.Name
        
        # 檢查是否為 Windows 平台
        $isWindowsPlatform = $platformName.StartsWith('win')
        
        if ($isWindowsPlatform) {
            $targetPlatformDir = Join-Path $runtimesTarget $platformName
            Copy-Item $_.FullName -Destination $targetPlatformDir -Recurse -Force
            Write-Verbose "  複製: runtimes\$platformName"
            $copiedPlatforms++
        }
        else {
            Write-Verbose "  跳過: runtimes\$platformName (非 Windows 平台)"
            $skippedPlatforms++
        }
    }
    
    Write-ColorMessage "✓ 已複製 $copiedPlatforms 個 Windows 平台（跳過 $skippedPlatforms 個其他平台）" -Color Green
}

function Show-Summary {
    Write-ColorMessage "`n[完成摘要]" -Color Cyan
    
    $totalSize = (Get-ChildItem -Path $targetDir -Recurse | Measure-Object -Property Length -Sum).Sum
    $totalSizeMB = [math]::Round($totalSize / 1MB, 2)
    $fileCount = (Get-ChildItem -Path $targetDir -File -Recurse | Measure-Object).Count
    $dirCount = (Get-ChildItem -Path $targetDir -Directory -Recurse | Measure-Object).Count
    
    Write-Host ""
    Write-ColorMessage "來源目錄: $sourceDir" -Color White
    Write-ColorMessage "目標目錄: $targetDir" -Color White
    Write-ColorMessage "檔案總數: $fileCount" -Color White
    Write-ColorMessage "目錄總數: $dirCount" -Color White
    Write-ColorMessage "總大小: $totalSizeMB MB" -Color White
    Write-Host ""
    Write-ColorMessage "✓ 發布檔案準備完成！" -Color Green
    Write-ColorMessage "  下一步：使用 Inno Setup 開啟 Setup.iss 並編譯安裝程式。" -Color Yellow
    Write-Host ""
}

# =============================================================================
# 主要執行流程
# =============================================================================

try {
    Write-ColorMessage "========================================" -Color Cyan
    Write-ColorMessage "EasyBrailleEdit 發布檔案準備工具" -Color Cyan
    Write-ColorMessage "========================================" -Color Cyan
    
    Test-Prerequisites
    Clear-TargetDirectory
    Copy-BuildOutput
    Copy-Subdirectories
    Copy-RuntimesDirectory
    Show-Summary
    
    exit 0
}
catch {
    Write-ColorMessage "`n[錯誤]" -Color Red
    Write-ColorMessage $_.Exception.Message -Color Red
    Write-Host ""
    exit 1
}
