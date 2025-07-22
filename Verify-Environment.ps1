# ⚡ Script de Verificación Rápida - API SAP Bridge
# Ejecutar en PowerShell como Administrador

Write-Host "🔍 VERIFICANDO ENTORNO PARA API SAP BRIDGE" -ForegroundColor Cyan
Write-Host "=" * 60

# 1. Verificar .NET 8.0
Write-Host "`n📦 Verificando .NET 8.0..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    if ($dotnetVersion -like "8.*") {
        Write-Host "✅ .NET 8.0 encontrado: $dotnetVersion" -ForegroundColor Green
    } else {
        Write-Host "❌ .NET 8.0 no encontrado. Versión actual: $dotnetVersion" -ForegroundColor Red
        Write-Host "   Descargar: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
    }
} catch {
    Write-Host "❌ .NET no está instalado" -ForegroundColor Red
}

# 2. Verificar SQL Server LocalDB
Write-Host "`n🗄️  Verificando SQL Server LocalDB..." -ForegroundColor Yellow
try {
    $localdbInfo = sqllocaldb info 2>$null
    if ($localdbInfo) {
        Write-Host "✅ LocalDB encontrado:" -ForegroundColor Green
        sqllocaldb info | ForEach-Object { Write-Host "   - $_" -ForegroundColor Gray }
        
        # Verificar si MSSQLLocalDB está corriendo
        $status = sqllocaldb info MSSQLLocalDB
        if ($status -like "*State: Running*") {
            Write-Host "✅ MSSQLLocalDB está corriendo" -ForegroundColor Green
        } else {
            Write-Host "⚠️  Iniciando MSSQLLocalDB..." -ForegroundColor Yellow
            sqllocaldb start MSSQLLocalDB
        }
    } else {
        Write-Host "❌ LocalDB no encontrado" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Error verificando LocalDB: $($_.Exception.Message)" -ForegroundColor Red
}

# 3. Test de conexión a base de datos
Write-Host "`n🔗 Probando conexión a base de datos..." -ForegroundColor Yellow
$connectionString = "Server=(localdb)\mssqllocaldb;Database=master;Trusted_Connection=true;Connection Timeout=5;"

try {
    Add-Type -AssemblyName System.Data
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    Write-Host "✅ Conexión a SQL Server exitosa" -ForegroundColor Green
    $connection.Close()
    
    # Verificar si la base de datos ApiSAP_Development existe
    $checkDbConnection = "Server=(localdb)\mssqllocaldb;Database=ApiSAP_Development;Trusted_Connection=true;Connection Timeout=5;"
    try {
        $dbConnection = New-Object System.Data.SqlClient.SqlConnection($checkDbConnection)
        $dbConnection.Open()
        Write-Host "✅ Base de datos ApiSAP_Development existe" -ForegroundColor Green
        $dbConnection.Close()
    } catch {
        Write-Host "⚠️  Base de datos ApiSAP_Development no existe - se creará automáticamente" -ForegroundColor Yellow
    }
    
} catch {
    Write-Host "❌ Error de conexión: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "   Intentando instalar/configurar LocalDB..." -ForegroundColor Yellow
    
    # Intentar crear instancia LocalDB
    try {
        sqllocaldb create MSSQLLocalDB 2>$null
        sqllocaldb start MSSQLLocalDB
        Write-Host "✅ LocalDB configurado" -ForegroundColor Green
    } catch {
        Write-Host "❌ No se pudo configurar LocalDB automáticamente" -ForegroundColor Red
    }
}

# 4. Verificar estructura de proyecto
Write-Host "`n📁 Verificando estructura de proyecto..." -ForegroundColor Yellow

$projectFiles = @(
    "src/ApiSAPBridge.API/ApiSAPBridge.API.csproj",
    "src/ApiSAPBridge.Data/ApiSAPBridge.Data.csproj",
    "src/ApiSAPBridge.Models/ApiSAPBridge.Models.csproj"
)

foreach ($file in $projectFiles) {
    if (Test-Path $file) {
        Write-Host "✅ $file" -ForegroundColor Green
    } else {
        Write-Host "❌ Falta: $file" -ForegroundColor Red
    }
}

# 5. Verificar archivos críticos
Write-Host "`n📋 Verificando archivos críticos..." -ForegroundColor Yellow

$criticalFiles = @(
    @{ Path = "src/ApiSAPBridge.API/appsettings.json"; Required = $true },
    @{ Path = "src/ApiSAPBridge.Data/ApiSAPBridgeDbContext.cs"; Required = $true },
    @{ Path = "src/ApiSAPBridge.Data/DesignTimeDbContextFactory.cs"; Required = $true }
)

foreach ($fileInfo in $criticalFiles) {
    if (Test-Path $fileInfo.Path) {
        Write-Host "✅ $($fileInfo.Path)" -ForegroundColor Green
    } else {
        if ($fileInfo.Required) {
            Write-Host "❌ FALTA ARCHIVO CRÍTICO: $($fileInfo.Path)" -ForegroundColor Red
        } else {
            Write-Host "⚠️  $($fileInfo.Path)" -ForegroundColor Yellow
        }
    }
}

# 6. Crear archivos faltantes automáticamente
Write-Host "`n🔧 Creando archivos faltantes..." -ForegroundColor Yellow

# Crear DesignTimeDbContextFactory si no existe
$designTimeFactory = "src/ApiSAPBridge.Data/DesignTimeDbContextFactory.cs"
if (!(Test-Path $designTimeFactory)) {
    $factoryContent = @"
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ApiSAPBridge.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApiSAPBridgeDbContext>
    {
        public ApiSAPBridgeDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApiSAPBridgeDbContext>();
            
            var connectionString = "Server=(localdb)\\mssqllocaldb;Database=ApiSAP_Development;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true;";
            
            optionsBuilder.UseSqlServer(connectionString, options =>
            {
                options.MigrationsAssembly("ApiSAPBridge.Data");
            });

            return new ApiSAPBridgeDbContext(optionsBuilder.Options);
        }
    }
}
"@
    
    New-Item -Path (Split-Path $designTimeFactory -Parent) -ItemType Directory -Force | Out-Null
    Set-Content -Path $designTimeFactory -Value $factoryContent -Encoding UTF8
    Write-Host "✅ Creado: DesignTimeDbContextFactory.cs" -ForegroundColor Green
}

# Crear appsettings.json básico si no existe
$appsettings = "src/ApiSAPBridge.API/appsettings.json"
if (!(Test-Path $appsettings)) {
    $appsettingsContent = @"
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ApiSAP_Development;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
"@
    
    New-Item -Path (Split-Path $appsettings -Parent) -ItemType Directory -Force | Out-Null
    Set-Content -Path $appsettings -Value $appsettingsContent -Encoding UTF8
    Write-Host "✅ Creado: appsettings.json" -ForegroundColor Green
}

# 7. Crear base de datos automáticamente
Write-Host "`n🗄️  Creando base de datos si no existe..." -ForegroundColor Yellow
try {
    $createDbConnection = "Server=(localdb)\mssqllocaldb;Database=master;Trusted_Connection=true;"
    $connection = New-Object System.Data.SqlClient.SqlConnection($createDbConnection)
    $connection.Open()
    
    $command = $connection.CreateCommand()
    $command.CommandText = @"
        IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ApiSAP_Development')
        BEGIN
            CREATE DATABASE [ApiSAP_Development]
        END
"@
    $command.ExecuteNonQuery()
    $connection.Close()
    Write-Host "✅ Base de datos ApiSAP_Development lista" -ForegroundColor Green
} catch {
    Write-Host "❌ Error creando base de datos: $($_.Exception.Message)" -ForegroundColor Red
}

# 8. Test final de migración
Write-Host "`n🧪 Probando comando de migración..." -ForegroundColor Yellow
try {
    $currentLocation = Get-Location
    Set-Location "src/ApiSAPBridge.Data"
    
    # Intentar generar migración
    Write-Host "Ejecutando: dotnet ef migrations add TestMigration --startup-project ../ApiSAPBridge.API" -ForegroundColor Gray
    $result = dotnet ef migrations add TestMigration --startup-project ../ApiSAPBridge.API 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Comando de migración funciona correctamente" -ForegroundColor Green
        
        # Limpiar migración de prueba
        dotnet ef migrations remove --startup-project ../ApiSAPBridge.API --force 2>$null
    } else {
        Write-Host "❌ Error en comando de migración:" -ForegroundColor Red
        Write-Host $result -ForegroundColor Red
    }
    
    Set-Location $currentLocation
} catch {
    Write-Host "❌ Error probando migración: $($_.Exception.Message)" -ForegroundColor Red
    Set-Location $currentLocation
}

# 9. Resumen final
Write-Host "`n📋 RESUMEN FINAL" -ForegroundColor Magenta
Write-Host "=" * 60

if ($LASTEXITCODE -eq 0) {
    Write-Host "🎉 ¡TODO LISTO! Puedes ejecutar las migraciones:" -ForegroundColor Green
    Write-Host ""
    Write-Host "Comandos para ejecutar:" -ForegroundColor Yellow
    Write-Host "1. cd src/ApiSAPBridge.Data" -ForegroundColor White
    Write-Host "2. dotnet ef migrations add InitialCreate --startup-project ../ApiSAPBridge.API" -ForegroundColor White
    Write-Host "3. dotnet ef database update --startup-project ../ApiSAPBridge.API" -ForegroundColor White
    Write-Host ""
} else {
    Write-Host "⚠️  ATENCIÓN: Hay problemas que resolver:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Revisa los elementos marcados con ❌ arriba" -ForegroundColor Red
    Write-Host ""
    Write-Host "Opciones:" -ForegroundColor Yellow
    Write-Host "1. Instalar SQL Server Express: https://www.microsoft.com/sql-server/sql-server-downloads" -ForegroundColor White
    Write-Host "2. Usar SQLite en lugar de SQL Server (más simple)" -ForegroundColor White
    Write-Host "3. Usar base de datos en memoria para testing" -ForegroundColor White
}

Write-Host "`n🔗 Enlaces útiles:" -ForegroundColor Cyan
Write-Host "- .NET 8.0 Download: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor White
Write-Host "- SQL Server Express: https://www.microsoft.com/sql-server/sql-server-downloads" -ForegroundColor White
Write-Host "- LocalDB Install: https://docs.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb" -ForegroundColor White

Write-Host "`n✅ Verificación completada" -ForegroundColor Green