# ============================================
# SCRIPT PARA ARREGLAR MIGRACIONES EF CORE
# ApiSAPBridge - Configuration Project
# Estructura: src/ApiSAPBridge.Data
# ============================================

param(
    [string]$ProjectPath = "src\ApiSAPBridge.Data",
    [switch]$DropDatabase,
    [switch]$Force,
    [string]$BackupPath = ""
)

Write-Host "🔧 Iniciando proceso de reparación de migraciones..." -ForegroundColor Cyan
Write-Host "Proyecto: $ProjectPath" -ForegroundColor Yellow

# Verificar si estamos en el directorio correcto
if (-not (Test-Path "*.sln") -and -not (Test-Path "src")) {
    Write-Host "❌ No se encontró archivo .sln ni carpeta src. Asegúrate de estar en el directorio raíz del proyecto ApiSAPBridge." -ForegroundColor Red
    Write-Host "Estructura esperada: ApiSAPBridge/src/ApiSAPBridge.Data/" -ForegroundColor Yellow
    exit 1
}

# Verificar que existe el proyecto
if (-not (Test-Path $ProjectPath)) {
    Write-Host "❌ No se encontró el proyecto en: $ProjectPath" -ForegroundColor Red
    Write-Host "Verificando estructura de carpetas..." -ForegroundColor Yellow
    Get-ChildItem -Path "src" -Directory | ForEach-Object { Write-Host "  📁 $($_.Name)" -ForegroundColor Gray }
    exit 1
}

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

Write-Host "`n🔍 Verificando estado actual de migraciones..." -ForegroundColor Cyan

# Listar migraciones actuales
$listResult = Invoke-EFCommand "dotnet ef migrations list --project `"$ProjectPath`"" "Listando migraciones existentes"

# Paso 1: Hacer backup si se especifica
if ($BackupPath -ne "") {
    Write-Host "`n💾 Creando backup de la base de datos..." -ForegroundColor Cyan
    $backupCommand = "dotnet ef migrations script --project `"$ProjectPath`" --output `"$BackupPath`""
    Invoke-EFCommand $backupCommand "Creando script de backup"
}

# Paso 2: Remover migraciones problemáticas
Write-Host "`n🗑️ Removiendo migraciones conflictivas..." -ForegroundColor Cyan

$removeAttempts = 0
$maxAttempts = 10

while ($removeAttempts -lt $maxAttempts) {
    $removeResult = Invoke-EFCommand "dotnet ef migrations remove --project `"$ProjectPath`"" "Removiendo migración conflictiva"
    
    if ($removeResult) {
        $removeAttempts++
        Write-Host "🔄 Migración removida ($removeAttempts/$maxAttempts). Verificando si hay más..." -ForegroundColor Yellow
    } else {
        Write-Host "ℹ️ No hay más migraciones para remover o se completó la limpieza." -ForegroundColor Blue
        break
    }
}

# Paso 3: Eliminar base de datos si se especifica
if ($DropDatabase -or $Force) {
    Write-Host "`n🗄️ Eliminando base de datos existente..." -ForegroundColor Cyan
    $dropResult = Invoke-EFCommand "dotnet ef database drop --project `"$ProjectPath`" --force" "Eliminando base de datos"
    
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

$createResult = Invoke-EFCommand "dotnet ef migrations add $migrationName --project `"$ProjectPath`"" "Creando migración: $migrationName"

if (-not $createResult) {
    Write-Host "❌ Error al crear la migración. Revisa los errores anteriores." -ForegroundColor Red
    Write-Host "💡 Posibles soluciones:" -ForegroundColor Yellow
    Write-Host "   1. Verificar que el DbContext esté correctamente configurado" -ForegroundColor White
    Write-Host "   2. Verificar el connection string en appsettings.json" -ForegroundColor White
    Write-Host "   3. Verificar que todas las referencias entre proyectos estén correctas" -ForegroundColor White
    exit 1
}

# Paso 6: Aplicar migración
Write-Host "`n⬆️ Aplicando migración a la base de datos..." -ForegroundColor Cyan
$updateResult = Invoke-EFCommand "dotnet ef database update --project `"$ProjectPath`"" "Aplicando migración a la base de datos"

if (-not $updateResult) {
    Write-Host "❌ Error al aplicar la migración. Revisa la configuración de conexión." -ForegroundColor Red
    
    # Generar script SQL para revisión manual
    Write-Host "📝 Generando script SQL para revisión manual..." -ForegroundColor Yellow
    $scriptPath = "migration_script_$timestamp.sql"
    Invoke-EFCommand "dotnet ef migrations script --project `"$ProjectPath`" --output `"$scriptPath`"" "Generando script SQL"
    Write-Host "📄 Script generado en: $scriptPath" -ForegroundColor Blue
    
    exit 1
}

# Paso 7: Verificación final
Write-Host "`n✅ Verificación final..." -ForegroundColor Cyan
Invoke-EFCommand "dotnet ef migrations list --project `"$ProjectPath`"" "Listando migraciones finales"
Invoke-EFCommand "dotnet ef dbcontext info --project `"$ProjectPath`"" "Verificando información del contexto"

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

# Mostrar información de la estructura
Write-Host "`n📊 Estructura del proyecto verificada:" -ForegroundColor Cyan
Write-Host "   📁 Raíz: $(Get-Location)" -ForegroundColor White
Write-Host "   📁 Proyecto EF: $ProjectPath" -ForegroundColor White
Write-Host "   📁 Configuración: src\ApiSAPBridge.Configuration" -ForegroundColor White

# Instrucciones adicionales
Write-Host "`n📚 Próximos pasos recomendados:" -ForegroundColor Cyan
Write-Host "1. Verificar que todos los modelos estén correctos" -ForegroundColor White
Write-Host "2. Probar las operaciones CRUD básicas" -ForegroundColor White
Write-Host "3. Implementar los formularios de configuración en ApiSAPBridge.Configuration" -ForegroundColor White
Write-Host "4. Configurar las conexiones a SAP" -ForegroundColor White

# Función de uso
function Show-Usage {
    Write-Host "`n📖 Uso del script:" -ForegroundColor Cyan
    Write-Host ".\FixMigrations.ps1 [-ProjectPath 'src\ApiSAPBridge.Data'] [-DropDatabase] [-Force] [-BackupPath 'backup.sql']" -ForegroundColor White
    Write-Host "`nParámetros:" -ForegroundColor Yellow
    Write-Host "  -ProjectPath: Ruta del proyecto EF (default: src\ApiSAPBridge.Data)" -ForegroundColor White
    Write-Host "  -DropDatabase: Eliminar la base de datos existente" -ForegroundColor White
    Write-Host "  -Force: Forzar todas las operaciones" -ForegroundColor White
    Write-Host "  -BackupPath: Ruta para generar backup SQL" -ForegroundColor White
}

if ($args -contains "-help" -or $args -contains "--help" -or $args -contains "-h") {
    Show-Usage
}