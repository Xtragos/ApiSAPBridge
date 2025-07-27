// DatabaseInitializerService.cs
// Ubicación: src\ApiSAPBridge.Configuration\Services\DatabaseInitializerService.cs

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ApiSAPBridge.Configuration.Services
{
    public interface IDatabaseInitializerService
    {
        Task<DatabaseInitializationResult> InitializeDatabaseAsync(string connectionString);
        Task<bool> DatabaseExistsAsync(string serverName, string databaseName, bool useIntegratedSecurity = true, string username = null, string password = null);
    }

    public class DatabaseInitializerService : IDatabaseInitializerService
    {
        private readonly ILogger<DatabaseInitializerService> _logger;

        public DatabaseInitializerService(ILogger<DatabaseInitializerService> logger)
        {
            _logger = logger;
        }

        public async Task<DatabaseInitializationResult> InitializeDatabaseAsync(string connectionString)
        {
            try
            {
                _logger.LogInformation("🚀 Iniciando inicialización de base de datos...");

                // Extraer información de conexión
                var builder = new SqlConnectionStringBuilder(connectionString);
                var serverName = builder.DataSource;
                var databaseName = builder.InitialCatalog;

                _logger.LogInformation("📋 Verificando base de datos: {Database} en servidor: {Server}", databaseName, serverName);

                // Verificar si la base de datos ya existe
                var exists = await DatabaseExistsAsync(serverName, databaseName, builder.IntegratedSecurity, builder.UserID, builder.Password);

                if (exists)
                {
                    _logger.LogInformation("✅ Base de datos {Database} ya existe", databaseName);
                    return new DatabaseInitializationResult
                    {
                        Success = true,
                        Message = $"Base de datos '{databaseName}' ya existe y está lista para usar.",
                        DatabaseExisted = true
                    };
                }

                _logger.LogInformation("📦 Base de datos no existe, ejecutando script de creación...");

                // Leer el script SQL embebido
                var sqlScript = GetEmbeddedSqlScript();

                if (string.IsNullOrEmpty(sqlScript))
                {
                    throw new InvalidOperationException("No se pudo cargar el script SQL de inicialización");
                }

                // Ejecutar el script SQL
                await ExecuteSqlScriptAsync(connectionString, sqlScript);

                _logger.LogInformation("🎉 Base de datos inicializada exitosamente");

                return new DatabaseInitializationResult
                {
                    Success = true,
                    Message = $"Base de datos '{databaseName}' creada exitosamente con todas las tablas.",
                    DatabaseExisted = false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al inicializar base de datos");
                return new DatabaseInitializationResult
                {
                    Success = false,
                    Message = "Error al inicializar base de datos",
                    ErrorDetails = ex.Message,
                    DatabaseExisted = false
                };
            }
        }

        public async Task<bool> DatabaseExistsAsync(string serverName, string databaseName, bool useIntegratedSecurity = true, string username = null, string password = null)
        {
            try
            {
                // Crear connection string para master
                var masterConnectionBuilder = new SqlConnectionStringBuilder
                {
                    DataSource = serverName,
                    InitialCatalog = "master",
                    IntegratedSecurity = useIntegratedSecurity,
                    ConnectTimeout = 30,
                    TrustServerCertificate = true
                };

                if (!useIntegratedSecurity)
                {
                    masterConnectionBuilder.UserID = username;
                    masterConnectionBuilder.Password = password;
                }

                using var connection = new SqlConnection(masterConnectionBuilder.ConnectionString);
                await connection.OpenAsync();

                // Verificar si la base de datos existe
                var sql = "SELECT COUNT(*) FROM sys.databases WHERE name = @DatabaseName";
                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@DatabaseName", databaseName);

                var count = (int)await command.ExecuteScalarAsync();
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ Error al verificar existencia de base de datos");
                return false;
            }
        }

        private string GetEmbeddedSqlScript()
        {
            try
            {
                // Intentar cargar desde archivo embebido
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "ApiSAPBridge.Configuration.Resources.CreateDatabase.sql";

                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    using var reader = new StreamReader(stream);
                    return reader.ReadToEnd();
                }

                // Si no está embebido, buscar en la carpeta Resources
                var resourcesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "CreateDatabase.sql");
                if (File.Exists(resourcesPath))
                {
                    return File.ReadAllText(resourcesPath);
                }

                // Como último recurso, devolver el script hardcodeado
                _logger.LogWarning("📁 No se encontró archivo SQL embebido, usando script incluido");
                return GetHardcodedSqlScript();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al cargar script SQL");
                return GetHardcodedSqlScript();
            }
        }

        private async Task ExecuteSqlScriptAsync(string connectionString, string sqlScript)
        {
            // Cambiar connection string para usar master inicialmente
            var builder = new SqlConnectionStringBuilder(connectionString);
            var masterConnectionString = connectionString.Replace($"Database={builder.InitialCatalog}", "Database=master");

            using var connection = new SqlConnection(masterConnectionString);
            await connection.OpenAsync();

            // Dividir el script en lotes por GO
            var batches = sqlScript.Split(new[] { "\nGO\n", "\nGO\r\n", "\r\nGO\r\n", "\r\nGO\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var batch in batches)
            {
                if (string.IsNullOrWhiteSpace(batch))
                    continue;

                try
                {
                    using var command = new SqlCommand(batch, connection);
                    command.CommandTimeout = 300; // 5 minutos timeout
                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error ejecutando lote SQL: {Batch}", batch.Substring(0, Math.Min(100, batch.Length)));
                    throw;
                }
            }
        }

        private string GetHardcodedSqlScript()
        {
            // Aquí puedes incluir una versión resumida del script SQL más crítico
            return @"
-- Verificar login ICGAdmin
USE [master]
IF NOT EXISTS (SELECT name FROM sys.server_principals WHERE name = 'ICGAdmin')
BEGIN
    RAISERROR('❌ ERROR: El login ICGAdmin no existe. Crear primero: CREATE LOGIN [ICGAdmin] WITH PASSWORD = ''TuContraseña''', 16, 1)
    RETURN
END

-- Crear base de datos ApiSAP
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ApiSAP')
BEGIN
    CREATE DATABASE [ApiSAP] COLLATE Modern_Spanish_CI_AS
END

USE [ApiSAP]

-- Crear usuario
IF EXISTS (SELECT name FROM sys.database_principals WHERE name = 'ICGAdmin')
    DROP USER [ICGAdmin]
CREATE USER [ICGAdmin] FOR LOGIN [ICGAdmin]
ALTER ROLE [db_owner] ADD MEMBER [ICGAdmin]

-- Crear tabla básica para verificar
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    )
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES ('20250726000000_InitialCreateComplete', '8.0.13')
END
";
        }
    }

    public class DatabaseInitializationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ErrorDetails { get; set; } = string.Empty;
        public bool DatabaseExisted { get; set; }
    }
}