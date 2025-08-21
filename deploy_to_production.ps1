# 🚀 PRODUCTION DEPLOYMENT SCRIPT
# Subscription Tools Module - User Interface Enhancements
# Date: August 20, 2025
# 
# IMPORTANT: Update the variables below before running this script!

param(
    [Parameter(Mandatory=$true)]
    [string]$ProductionSitePath,
    
    [Parameter(Mandatory=$true)]
    [string]$BackupLocation,
    
    [Parameter(Mandatory=$false)]
    [string]$AppPoolName = ""
)

# 🚨 STOP EXECUTION IF NOT RUNNING AS ADMINISTRATOR
if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
    Write-Host "❌ ERROR: This script must be run as Administrator!" -ForegroundColor Red
    Write-Host "Please right-click PowerShell and select 'Run as Administrator'" -ForegroundColor Yellow
    exit 1
}

# 📋 VALIDATE PATHS
if (-not (Test-Path $ProductionSitePath)) {
    Write-Host "❌ ERROR: Production site path not found: $ProductionSitePath" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $BackupLocation)) {
    Write-Host "❌ ERROR: Backup location not found: $BackupLocation" -ForegroundColor Red
    exit 1
}

# 🔍 VALIDATE SOURCE FILES
$sourceDll = "bin\Ventrian.SubscriptionTools.dll"
$sourceEditReceipt = "EditReceipt.ascx"
$sourceEditReceipts = "EditReceipts.ascx"
$sourceResx = "App_LocalResources\EditReceipt.ascx.resx"

if (-not (Test-Path $sourceDll)) {
    Write-Host "❌ ERROR: Source DLL not found: $sourceDll" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $sourceEditReceipt)) {
    Write-Host "❌ ERROR: Source EditReceipt.ascx not found: $sourceEditReceipt" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $sourceEditReceipts)) {
    Write-Host "❌ ERROR: Source EditReceipts.ascx not found: $sourceEditReceipts" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $sourceResx)) {
    Write-Host "❌ ERROR: Source resource file not found: $sourceResx" -ForegroundColor Red
    exit 1
}

# 📊 DISPLAY DEPLOYMENT INFORMATION
Write-Host "🚀 PRODUCTION DEPLOYMENT - Subscription Tools Module" -ForegroundColor Cyan
Write-Host "==================================================" -ForegroundColor Cyan
Write-Host "Production Site: $ProductionSitePath" -ForegroundColor White
Write-Host "Backup Location: $BackupLocation" -ForegroundColor White
Write-Host "Date: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor White
Write-Host ""

# ⚠️ CONFIRMATION PROMPT
Write-Host "⚠️  WARNING: This will deploy changes to PRODUCTION!" -ForegroundColor Yellow
Write-Host "Are you sure you want to continue? (y/N)" -ForegroundColor Yellow
$confirmation = Read-Host

if ($confirmation -ne "y" -and $confirmation -ne "Y") {
    Write-Host "❌ Deployment cancelled by user" -ForegroundColor Red
    exit 0
}

Write-Host ""

# 🔄 STEP 1: BACKUP EXISTING FILES
Write-Host "📦 STEP 1: Creating backups..." -ForegroundColor Green

try {
    $backupTimestamp = Get-Date -Format "yyyyMMdd_HHmmss"
    
    # Backup DLL
    $backupDll = "$BackupLocation\Ventrian.SubscriptionTools.dll.backup_$backupTimestamp"
    Copy-Item "$ProductionSitePath\bin\Ventrian.SubscriptionTools.dll" $backupDll -Force
    Write-Host "✅ DLL backed up to: $backupDll" -ForegroundColor Green
    
    # Backup ASCX files
    $backupEditReceipt = "$BackupLocation\EditReceipt.ascx.backup_$backupTimestamp"
    Copy-Item "$ProductionSitePath\DesktopModules\SubscriptionSignup\EditReceipt.ascx" $backupEditReceipt -Force
    Write-Host "✅ EditReceipt.ascx backed up to: $backupEditReceipt" -ForegroundColor Green
    
    $backupEditReceipts = "$BackupLocation\EditReceipts.ascx.backup_$backupTimestamp"
    Copy-Item "$ProductionSitePath\DesktopModules\SubscriptionSignup\EditReceipts.ascx" $backupEditReceipts -Force
    Write-Host "✅ EditReceipts.ascx backed up to: $backupEditReceipts" -ForegroundColor Green
    
    # Backup resource file if it exists
    if (Test-Path "$ProductionSitePath\DesktopModules\SubscriptionSignup\App_LocalResources\EditReceipt.ascx.resx") {
        $backupResx = "$BackupLocation\EditReceipt.ascx.resx.backup_$backupTimestamp"
        Copy-Item "$ProductionSitePath\DesktopModules\SubscriptionSignup\App_LocalResources\EditReceipt.ascx.resx" $backupResx -Force
        Write-Host "✅ Resource file backed up to: $backupResx" -ForegroundColor Green
    }
    
} catch {
    Write-Host "❌ ERROR: Backup failed - $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host ""

# 🚀 STEP 2: DEPLOY COMPILED ASSEMBLY
Write-Host "🔧 STEP 2: Deploying compiled assembly..." -ForegroundColor Green

try {
    Copy-Item $sourceDll "$ProductionSitePath\bin\Ventrian.SubscriptionTools.dll" -Force
    Write-Host "✅ DLL deployed successfully" -ForegroundColor Green
    
    # Verify deployment
    $deployedDll = Get-Item "$ProductionSitePath\bin\Ventrian.SubscriptionTools.dll"
    Write-Host "   Size: $($deployedDll.Length) bytes" -ForegroundColor Gray
    Write-Host "   Modified: $($deployedDll.LastWriteTime)" -ForegroundColor Gray
    
} catch {
    Write-Host "❌ ERROR: DLL deployment failed - $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host ""

# 🎨 STEP 3: DEPLOY USER INTERFACE FILES
Write-Host "🎨 STEP 3: Deploying user interface files..." -ForegroundColor Green

try {
    # Deploy EditReceipt.ascx
    Copy-Item $sourceEditReceipt "$ProductionSitePath\DesktopModules\SubscriptionSignup\EditReceipt.ascx" -Force
    Write-Host "✅ EditReceipt.ascx deployed successfully" -ForegroundColor Green
    
    # Deploy EditReceipts.ascx
    Copy-Item $sourceEditReceipts "$ProductionSitePath\DesktopModules\SubscriptionSignup\EditReceipts.ascx" -Force
    Write-Host "✅ EditReceipts.ascx deployed successfully" -ForegroundColor Green
    
    # Deploy resource file
    Copy-Item $sourceResx "$ProductionSitePath\DesktopModules\SubscriptionSignup\App_LocalResources\EditReceipt.ascx.resx" -Force
    Write-Host "✅ Resource file deployed successfully" -ForegroundColor Green
    
} catch {
    Write-Host "❌ ERROR: UI files deployment failed - $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host ""

# 🔄 STEP 4: RECYCLE APPLICATION POOL
Write-Host "🔄 STEP 4: Recycling application pool..." -ForegroundColor Green

try {
    # Touch web.config to trigger recycle
    (Get-Item "$ProductionSitePath\web.config").LastWriteTime = Get-Date
    Write-Host "✅ Application pool recycle triggered" -ForegroundColor Green
    
    Write-Host "   ⏳ Waiting 10 seconds for recycle to complete..." -ForegroundColor Yellow
    Start-Sleep -Seconds 10
    
} catch {
    Write-Host "❌ ERROR: Application pool recycle failed - $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "   Please manually recycle the application pool in IIS Manager" -ForegroundColor Yellow
}

Write-Host ""

# ✅ DEPLOYMENT COMPLETE
Write-Host "🎉 DEPLOYMENT COMPLETED SUCCESSFULLY!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Next Steps:" -ForegroundColor Cyan
Write-Host "1. Test the new user interface in production" -ForegroundColor White
Write-Host "2. Verify user dropdowns populate correctly" -ForegroundColor White
Write-Host "3. Test receipt creation with different users" -ForegroundColor White
Write-Host "4. Verify role assignment works as expected" -ForegroundColor White
Write-Host "5. Test auto-search functionality in EditReceipts" -ForegroundColor White
Write-Host ""
Write-Host "📁 Backup files created in: $BackupLocation" -ForegroundColor Cyan
Write-Host "🕐 Deployment completed at: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor Cyan
Write-Host ""

# 📝 CREATE DEPLOYMENT LOG
$logFile = "$BackupLocation\deployment_log_$backupTimestamp.txt"
$logContent = @"
PRODUCTION DEPLOYMENT LOG
=========================
Date: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
Production Site: $ProductionSitePath
Backup Location: $BackupLocation

FILES DEPLOYED:
- Ventrian.SubscriptionTools.dll
- EditReceipt.ascx
- EditReceipts.ascx
- EditReceipt.ascx.resx

BACKUP FILES:
- $backupDll
- $backupEditReceipt
- $backupEditReceipts
- $backupResx

STATUS: SUCCESS
"@

$logContent | Out-File -FilePath $logFile -Encoding UTF8
Write-Host "📝 Deployment log saved to: $logFile" -ForegroundColor Cyan

Write-Host ""
Write-Host "🚀 Deployment script completed successfully!" -ForegroundColor Green
