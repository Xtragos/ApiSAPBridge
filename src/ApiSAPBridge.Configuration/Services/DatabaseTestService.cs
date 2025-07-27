// DatabaseTestService.cs - Versión mejorada con detección automática
// Ubicación: src\ApiSAPBridge.Configuration\Services\DatabaseTestService.cs

using ApiSAPBridge.Configuration.Models.DTOs;
using ApiSAPBridge.Models.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ApiSAPBridge.Configuration.Services
{
    public interface IDatabaseTestService
    {
        Task<ConnectionTestResult> TestConnectionAsync(string server, string database, string username = null, string password = null, bool useIntegratedSecurity = true);
        Task<ConnectionTestResult> TestConnectionWithAutoDetectionAsync(string server, string database, string username = null, string password = null, bool useIntegratedSecurity = true);
    }

    public class DatabaseTestService : IDatabaseTestService
    {
        private readonly ILogger<DatabaseTestService> _logger;
        private readonly ISqlServerVersionDetectorService _versionDetectorService;

        public DatabaseTestService(
            ILogger<DatabaseTestService> logger,
            ISqlServerVersionDetectorService versionDetectorService)
        {
            _logger = logger;
            _versionDetectorService = versionDetectorService;
        }

        public async Task<ConnectionTestResult> TestConnectionAsync(string server, string database, string username = null, string password = null, bool useIntegratedSecurity = true)
        {
            // Método original mantenido para compatibilidad
            return await TestConnectionWithAutoDetectionAsync(server, database, username, password, useIntegratedSecurity);
        }

        public async Task<ConnectionTestResult> TestConnectionWithAutoDetectionAsync(string server, string database, string username = null, string password = null, bool useIntegratedSecurity = true)
        {
            try
            {
                _logger.LogInformation("🔧 Iniciando prueba de conexión inteligente para {Server}/{Database}", server, database);

                // Paso 1: Detectar versión del servidor SQL
                _logger.LogDebug("🔍 Detectando versión de SQL Server...");
                var versionInfo = await _versionDetectorService.DetectVersionAsync(server, useIntegratedSecurity, username, password);

                if (!versionInfo.DetectionSuccessful)
                {
                    _logger.LogWarning("⚠️ No se pudo detectar la versión, intentando conexión básica");
                    return await FallbackConnectionTest(server, database, username, password, useIntegratedSecurity, versionInfo.ErrorMessage);
                }

                // Paso 2: Construir connection string optimizado
                var baseConnectionString = BuildBaseConnectionString(server, database, username, password, useIntegratedSecurity);
                var optimizationResult = await _versionDetectorService.OptimizeConnectionStringAsync(baseConnectionString);

                if (!optimizationResult.Success)
                {
                    _logger.LogWarning("⚠️ No se pudo optimizar connection string, usando configuración básica");
                    return await FallbackConnectionTest(server, database, username, password, useIntegratedSecurity, optimizationResult.ErrorMessage);
                }

                // Paso 3: Probar conexión optimizada
                _logger.LogInformation("✅ Usando connection string optimizado para {Version}", versionInfo.GetFriendlyVersionName());
                var testResult = await ExecuteConnectionTest(optimizationResult.OptimizedConnectionString, versionInfo);

                if (testResult.IsSuccess)
                {
                    // Agregar información de la versión detectada al resultado
                    testResult.Message += $"\n\n📊 Información del Servidor:\n" +
                                        $"• Versión: {versionInfo.GetFriendlyVersionName()}\n" +
                                        $"• Edición: {versionInfo.Edition}\n" +
                                        $"• Nivel: {versionInfo.ProductLevel}\n" +
                                        $"• TrustServerCertificate: {(versionInfo.RequiresTrustServerCertificate ? "Requerido" : "No necesario")}\n" +
                                        $"• Optimizaciones: {optimizationResult.OptimizationsApplied}";
                }

                return testResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en prueba de conexión inteligente");
                return new ConnectionTestResult
                {
                    IsSuccess = false,
                    Message = "Error en la prueba de conexión",
                    ErrorDetails = $"Error inesperado: {ex.Message}"
                };
            }
        }

        private async Task<ConnectionTestResult> FallbackConnectionTest(string server, string database, string username, string password, bool useIntegratedSecurity, string detectionError)
        {
            _logger.LogInformation("🔄 Ejecutando prueba de conexión con fallback automático...");

            // Intentar primero sin TrustServerCertificate (para versiones antiguas)
            var connectionString1 = BuildConnectionString(server, database, username, password, useIntegratedSecurity, false);
            var result1 = await ExecuteConnectionTest(connectionString1, null);

            if (result1.IsSuccess)
            {
                _logger.LogInformation("✅ Conexión exitosa sin TrustServerCertificate (probablemente SQL 2017 o anterior)");
                result1.Message += "\n\n🔧 Configuración detectada: Compatible con SQL Server 2017 o anterior";
                return result1;
            }

            _logger.LogDebug("🔄 Primera conexión falló, intentando con TrustServerCertificate=true...");

            // Intentar con TrustServerCertificate=true (para versiones nuevas)
            var connectionString2 = BuildConnectionString(server, database, username, password, useIntegratedSecurity, true);
            var result2 = await ExecuteConnectionTest(connectionString2, null);

            if (result2.IsSuccess)
            {
                _logger.LogInformation("✅ Conexión exitosa con TrustServerCertificate=true (probablemente SQL 2019+)");
                result2.Message += "\n\n🔧 Configuración detectada: Compatible con SQL Server 2019 o posterior\n" +
                                  "⚠️ Nota: TrustServerCertificate=true es requerido para este servidor";
                return result2;
            }

            // Si ambos fallan, devolver el error más informativo
            _logger.LogError("❌ Ambas configuraciones de conexión fallaron");
            return new ConnectionTestResult
            {
                IsSuccess = false,
                Message = "No se pudo establecer conexión con ninguna configuración",
                ErrorDetails = $"Error de detección: {detectionError}\n\n" +
                              $"Error sin TrustServerCertificate: {result1.ErrorDetails}\n\n" +
                              $"Error con TrustServerCertificate: {result2.ErrorDetails}\n\n" +
                              $"💡 Sugerencias:\n" +
                              $"• Verificar que SQL Server esté ejecutándose\n" +
                              $"• Verificar credenciales de usuario\n" +
                              $"• Verificar que la base de datos existe\n" +
                              $"• Verificar configuración de red/firewall"
            };
        }

        private async Task<ConnectionTestResult> ExecuteConnectionTest(string connectionString, SqlServerVersionInfo versionInfo)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);

                _logger.LogDebug("📡 Abriendo conexión...");
                await connection.OpenAsync();

                _logger.LogInformation("✅ Conexión establecida exitosamente");

                // Ejecutar consultas de prueba
                var serverInfo = await GetServerInfo(connection);

                var message = $"✅ Conexión exitosa!\n\n" +
                             $"🖥️ Servidor: {connection.DataSource}\n" +
                             $"🗄️ Base de datos: {connection.Database}\n" +
                             $"👤 Usuario conectado: {serverInfo.CurrentUser}\n" +
                             $"⏰ Hora del servidor: {serverInfo.ServerTime}\n" +
                             $"🔢 Versión del servidor: {serverInfo.ServerVersion}";

                return new ConnectionTestResult
                {
                    IsSuccess = true,
                    Message = message
                };
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "🚫 Error de SQL Server: {ErrorNumber}", sqlEx.Number);

                var analysis = AnalyzeSqlError(sqlEx);
                return new ConnectionTestResult
                {
                    IsSuccess = false,
                    Message = analysis.UserMessage,
                    ErrorDetails = $"Error SQL {sqlEx.Number}: {sqlEx.Message}\n\n{analysis.DetailedSuggestions}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error general de conexión");
                return new ConnectionTestResult
                {
                    IsSuccess = false,
                    Message = "Error de conexión",
                    ErrorDetails = ex.Message
                };
            }
        }

        private async Task<ServerInfo> GetServerInfo(SqlConnection connection)
        {
            var query = @"
                SELECT 
                    GETDATE() as ServerTime,
                    USER_NAME() as CurrentUser,
                    @@VERSION as ServerVersion,
                    SERVERPROPERTY('ServerName') as ServerName,
                    SERVERPROPERTY('ProductVersion') as ProductVersion
            ";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new ServerInfo
                {
                    ServerTime = Convert.ToDateTime(reader["ServerTime"]),
                    CurrentUser = reader["CurrentUser"].ToString(),
                    ServerVersion = reader["ServerVersion"].ToString(),
                    ServerName = reader["ServerName"].ToString(),
                    ProductVersion = reader["ProductVersion"].ToString()
                };
            }

            return new ServerInfo();
        }

        private (string UserMessage, string DetailedSuggestions) AnalyzeSqlError(SqlException sqlEx)
        {
            return sqlEx.Number switch
            {
                2 => ("No se puede conectar al servidor",
                      "• Verificar que SQL Server esté ejecutándose\n" +
                      "• Verificar el nombre del servidor\n" +
                      "• Verificar configuración de red y puertos\n" +
                      "• Verificar configuración del firewall"),

                18456 => ("Error de autenticación",
                          "• Verificar usuario y contraseña\n" +
                          "• Verificar que el usuario existe en SQL Server\n" +
                          "• Verificar permisos de acceso\n" +
                          "• Intentar con autenticación de Windows\n" +
                          "• Verificar que SQL Server permite autenticación mixta"),

                4060 => ("No se puede acceder a la base de datos",
                         "• Verificar que la base de datos existe\n" +
                         "• Verificar permisos del usuario en esta base de datos\n" +
                         "• Intentar conectar a la base de datos 'master' primero\n" +
                         "• Crear la base de datos si no existe"),

                18487 => ("Contraseña expirada",
                          "• Cambiar la contraseña del usuario\n" +
                          "• Configurar política de contraseñas\n" +
                          "• Contactar administrador del sistema"),

                20 => ("Error de certificado SSL/TLS",
                       "• Para SQL Server 2019+: Agregar TrustServerCertificate=true\n" +
                       "• Para versiones anteriores: Remover TrustServerCertificate\n" +
                       "• Verificar configuración de certificados SSL\n" +
                       "• Verificar configuración de encriptación"),

                _ => ($"Error SQL {sqlEx.Number}",
                      $"Descripción: {sqlEx.Message}\n" +
                      $"Consultar documentación de SQL Server para este error específico")
            };
        }

        private string BuildBaseConnectionString(string server, string database, string username, string password, bool useIntegratedSecurity)
        {
            return BuildConnectionString(server, database, username, password, useIntegratedSecurity, false);
        }

        private string BuildConnectionString(string server, string database, string username, string password, bool useIntegratedSecurity, bool trustServerCertificate)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = database,
                IntegratedSecurity = useIntegratedSecurity,
                ConnectTimeout = 30,
                MultipleActiveResultSets = true,
                ApplicationName = "ApiSAPBridge.Configuration",
                TrustServerCertificate = trustServerCertificate
            };

            if (!useIntegratedSecurity)
            {
                builder.UserID = username;
                builder.Password = password;
            }

            return builder.ConnectionString;
        }

        private class ServerInfo
        {
            public DateTime ServerTime { get; set; }
            public string CurrentUser { get; set; } = string.Empty;
            public string ServerVersion { get; set; } = string.Empty;
            public string ServerName { get; set; } = string.Empty;
            public string ProductVersion { get; set; } = string.Empty;
        }
    }
}