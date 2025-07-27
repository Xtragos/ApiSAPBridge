@echo off
echo ============================================
echo SCRIPT PARA ARREGLAR MIGRACIONES EF CORE
echo ApiSAPBridge - Configuration Project  
echo ============================================
echo.

REM Verificar si estamos en el directorio correcto
if not exist "src" (
    echo ❌ No se encontró la carpeta 'src'. 
    echo Asegurate de ejecutar este script desde la raiz del proyecto ApiSAPBridge.
    echo.
    echo Estructura esperada:
    echo ApiSAPBridge\
    echo ├── src\
    echo ├── tests\
    echo └── docs\
    echo.
    pause
    exit /b 1
)

if not exist "src\ApiSAPBridge.Data" (
    echo ❌ No se encontró el proyecto 'src\ApiSAPBridge.Data'.
    echo Verificando carpetas en src:
    dir src /ad /b
    echo.
    pause
    exit /b 1
)

echo ✅ Estructura del proyecto verificada.
echo 📁 Proyecto EF encontrado: src\ApiSAPBridge.Data
echo.

REM Advertencia sobre pérdida de datos
echo ⚠️  ADVERTENCIA IMPORTANTE:
echo Este script eliminara TODOS los datos existentes en la base de datos.
echo Si tienes datos importantes, haz un backup antes de continuar.
echo.
set /p confirm=¿Continuar? (s/N): 

if /i "%confirm%" neq "s" (
    echo Operación cancelada por el usuario.
    pause
    exit /b 0
)

echo.
echo 🔧 Iniciando proceso de reparación...
echo.

REM Paso 1: Listar migraciones actuales
echo 📋 Listando migraciones existentes...
dotnet ef migrations list --project "src\ApiSAPBridge.Data"
echo.

REM Paso 2: Remover migraciones conflictivas
echo 🗑️ Removiendo migraciones conflictivas...
:remove_migrations
dotnet ef migrations remove --project "src\ApiSAPBridge.Data" >nul 2>&1
if %errorlevel% equ 0 (
    echo ✅ Migración removida.
    goto remove_migrations
) else (
    echo ✅ No hay más migraciones para remover.
)
echo.

REM Paso 3: Eliminar base de datos
echo 🗄️ Eliminando base de datos existente...
dotnet ef database drop --project "src\ApiSAPBridge.Data" --force
if %errorlevel% equ 0 (
    echo ✅ Base de datos eliminada.
) else (
    echo ⚠️ La base de datos no existía o no pudo ser eliminada.
)
echo.

REM Paso 4: Limpiar y reconstruir
echo 🔨 Limpiando y recompilando solución...
dotnet clean
if %errorlevel% neq 0 (
    echo ❌ Error al limpiar la solución.
    pause
    exit /b 1
)

dotnet build
if %errorlevel% neq 0 (
    echo ❌ Error al compilar la solución.
    echo Revisa los errores de compilación y vuelve a intentar.
    pause
    exit /b 1
)
echo ✅ Solución compilada exitosamente.
echo.

REM Paso 5: Crear nueva migración
echo 📦 Creando nueva migración...
for /f "tokens=2 delims==" %%a in ('wmic OS Get localdatetime /value') do set "dt=%%a"
set "timestamp=%dt:~0,14%"
set "migrationName=InitialCreateFixed_%timestamp%"

dotnet ef migrations add %migrationName% --project "src\ApiSAPBridge.Data"
if %errorlevel% neq 0 (
    echo ❌ Error al crear la migración.
    echo.
    echo 💡 Posibles soluciones:
    echo    1. Verificar que el DbContext esté correctamente configurado
    echo    2. Verificar el connection string en appsettings.json
    echo    3. Verificar que todas las referencias entre proyectos estén correctas
    echo.
    pause
    exit /b 1
)
echo ✅ Migración '%migrationName%' creada exitosamente.
echo.

REM Paso 6: Aplicar migración
echo ⬆️ Aplicando migración a la base de datos...
dotnet ef database update --project "src\ApiSAPBridge.Data"
if %errorlevel% neq 0 (
    echo ❌ Error al aplicar la migración.
    echo Revisa la configuración de conexión en appsettings.json
    echo.
    
    REM Generar script SQL como fallback
    echo 📝 Generando script SQL para revisión manual...
    set "scriptName=migration_script_%timestamp%.sql"
    dotnet ef migrations script --project "src\ApiSAPBridge.Data" --output "%scriptName%"
    echo 📄 Script generado: %scriptName%
    echo.
    pause
    exit /b 1
)
echo ✅ Migración aplicada exitosamente.
echo.

REM Paso 7: Verificación final
echo ✅ Verificación final...
echo.
echo 📋 Migraciones actuales:
dotnet ef migrations list --project "src\ApiSAPBridge.Data"
echo.

echo 📊 Información del contexto:
dotnet ef dbcontext info --project "src\ApiSAPBridge.Data"
echo.

echo ============================================
echo 🎉 ¡PROCESO COMPLETADO EXITOSAMENTE!
echo ============================================
echo.
echo 📋 Resumen de acciones realizadas:
echo    • Migraciones conflictivas removidas
echo    • Base de datos eliminada y recreada  
echo    • Nueva migración creada: %migrationName%
echo    • Migración aplicada a la base de datos
echo    • Verificaciones completadas
echo.
echo 🚀 Tu proyecto está listo para continuar el desarrollo!
echo.
echo 📚 Próximos pasos recomendados:
echo    1. Verificar que todos los modelos estén correctos
echo    2. Probar las operaciones CRUD básicas
echo    3. Implementar los formularios de configuración
echo    4. Configurar las conexiones a SAP
echo.
pause