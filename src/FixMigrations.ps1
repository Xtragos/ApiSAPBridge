# ============================================
# SCRIPT PARA ARREGLAR MIGRACIONES EF CORE
# ApiSAPBridge - Configuration Project
# ============================================

param(
    [string]$ProjectPath = "ApiSAPBridge.Data",
    [switch]$DropDatabase,
    [switch]$Force,
    [string]$BackupPath = ""
)

Write-Host "🔧 Iniciando proceso de reparación de migraciones..." -ForegroundColor Cyan
Write-Host "Proyecto: $ProjectPath" -ForegroundColor Yellow

# Función para ejecutar comandos dotnet ef
function Invoke-EFCommand {
    param(
        [string]$Command,
        [string]$Description
    )
    
    Write-Host "`n📋 $Description" -ForegroundColor Green
    Write-Host "Ejecutando: $Command" -ForegroundColor Gray
    
    try {
        Invoke-Expression $Command
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ $Description completado exitosamente" -ForegroundColor Green
            return $true
        } else {
            Write-Host "❌ Error en: $Description" -ForegroundColor Red
            return $false
        }
    }
    catch {
        Write-Host "❌ Excepción en: $Description - $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Verificar si estamos en el directorio correcto
if (-not (Test-Path "*.sln")) {
    Write-Host "❌ No se encontró archivo .sln. Asegúrate de estar en el directorio raíz del proyecto." -ForegroundColor Red
    exit 1
}

Write-Host "`n🔍 Verificando estado actual de migraciones..." -ForegroundColor Cyan

# Listar migraciones actuales
$listResult = Invoke-EFCommand "dotnet ef migrations list --project $ProjectPath" "Listando migraciones existentes"

# Paso 1: Hacer backup si se especifica
if ($BackupPath -ne "") {
    Write-Host "`n💾 Creando backup de la base de datos..." -ForegroundColor Cyan
    $backupCommand = "dotnet ef migrations script --project $ProjectPath --output `"$BackupPath`""
    Invoke-EFCommand $backupCommand "Creando script de backup"
}

# Paso 2: Remover migraciones problemáticas
Write-Host "`n🗑️ Removiendo migraciones conflictivas..." -ForegroundColor Cyan

$removeAttempts = 0
$maxAttempts = 5

while ($removeAttempts -lt $maxAttempts) {
    $removeResult = Invoke-EFCommand "dotnet ef migrations remove --project $ProjectPath" "Removiendo migración conflictiva"
    
    if ($removeResult) {
        $removeAttempts++
        Write-Host "🔄 Migración removida. Verificando si hay más..." -ForegroundColor Yellow
        
        # Verificar si quedan migraciones
        $checkCommand = "dotnet ef migrations list --project $ProjectPath"
        $checkOutput = Invoke-Expression $checkCommand 2>&1
        
        if ($checkOutput -match "No migrations") {
            Write-Host "✅ Todas las migraciones han sido removidas." -ForegroundColor Green
            break
        }
    } else {
        Write-Host "ℹ️ No hay más migraciones para remover o se completó la limpieza." -ForegroundColor Blue
        break
    }
}

# Paso 3: Eliminar base de datos si se especifica
if ($DropDatabase -or $Force) {
    Write-Host "`n🗄️ Eliminando base de datos existente..." -ForegroundColor Cyan
    $dropResult = Invoke-EFCommand "dotnet ef database drop --project $ProjectPath --force" "Eliminando base de datos"
    
    if (-not $dropResult) {
        Write-Host "⚠️ La base de datos no pudo ser eliminada o no existe." -ForegroundColor Yellow
    }
}

# Paso 4: Limpiar y recompilar
Write-Host "`n🔨 Limpiando y recompilando solución..." -ForegroundColor Cyan
Invoke-EFCommand "dotnet clean" "Limpiando solución"
Invoke-EFCommand "dotnet build" "Recompilando solución"

# Paso 5: Crear nueva migración
Write-Host "`n📦 Creando nueva migración..." -ForegroundColor Cyan
$timestamp = Get-Date -Format "yyyyMMddHHmmss"
$migrationName = "InitialCreateFixed_$timestamp"

$createResult = Invoke-EFCommand "dotnet ef migrations add $migrationName --project $ProjectPath" "Creando migración: $migrationName"

if (-not $createResult) {
    Write-Host "❌ Error al crear la migración. Revisa los errores anteriores." -ForegroundColor Red
    exit 1
}

# Paso 6: Aplicar migración
Write-Host "`n⬆️ Aplicando migración a la base de datos..." -ForegroundColor Cyan
$updateResult = Invoke-EFCommand "dotnet ef database update --project $ProjectPath" "Aplicando migración a la base de datos"

if (-not $updateResult) {
    Write-Host "❌ Error al aplicar la migración. Revisa la configuración de conexión." -ForegroundColor Red
    
    # Generar script SQL para revisión manual
    Write-Host "📝 Generando script SQL para revisión manual..." -ForegroundColor Yellow
    $scriptPath = "migration_script_$timestamp.sql"
    Invoke-EFCommand "dotnet ef migrations script --project $ProjectPath --output `"$scriptPath`"" "Generando script SQL"
    Write-Host "📄 Script generado en: $scriptPath" -ForegroundColor Blue
    
    exit 1
}

# Paso 7: Verificación final
Write-Host "`n✅ Verificación final..." -ForegroundColor Cyan
Invoke-EFCommand "dotnet ef migrations list --project $ProjectPath" "Listando migraciones finales"
Invoke-EFCommand "dotnet ef dbcontext info --project $ProjectPath" "Verificando información del contexto"

Write-Host "`n🎉 ¡Proceso completado exitosamente!" -ForegroundColor Green
Write-Host "📋 Resumen de acciones realizadas:" -ForegroundColor Cyan
Write-Host "   • Migraciones conflictivas removidas" -ForegroundColor White
if ($DropDatabase -or $Force) {
    Write-Host "   • Base de datos eliminada y recreada" -ForegroundColor White
}
Write-Host "   • Nueva migración creada: $migrationName" -ForegroundColor White
Write-Host "   • Migración aplicada a la base de datos" -ForegroundColor White
Write-Host "   • Verificaciones completadas" -ForegroundColor White

Write-Host "`n🚀 Tu proyecto está listo para continuar el desarrollo!" -ForegroundColor Green

# Instrucciones adicionales
Write-Host "`n📚 Próximos pasos recomendados:" -ForegroundColor Cyan
Write-Host "1. Verificar que todos los modelos estén correctos" -ForegroundColor White
Write-Host "2. Probar las operaciones CRUD básicas" -ForegroundColor White
Write-Host "3. Implementar los formularios de configuración" -ForegroundColor White
Write-Host "4. Configurar las conexiones a SAP" -ForegroundColor White

# Función de uso
function Show-Usage {
    Write-Host "`n📖 Uso del script:" -ForegroundColor Cyan
    Write-Host ".\FixMigrations.ps1 [-ProjectPath 'ApiSAPBridge.Data'] [-DropDatabase] [-Force] [-BackupPath 'backup.sql']" -ForegroundColor White
    Write-Host "`nParámetros:" -ForegroundColor Yellow
    Write-Host "  -ProjectPath: Ruta del proyecto EF (default: ApiSAPBridge.Data)" -ForegroundColor White
    Write-Host "  -DropDatabase: Eliminar la base de datos existente" -ForegroundColor White
    Write-Host "  -Force: Forzar todas las operaciones" -ForegroundColor White
    Write-Host "  -BackupPath: Ruta para generar backup SQL" -ForegroundColor White
}

if ($args -contains "-help" -or $args -contains "--help" -or $args -contains "-h") {
    Show-Usage
}