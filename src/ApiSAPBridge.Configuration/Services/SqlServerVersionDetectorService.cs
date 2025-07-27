// SqlServerVersionDetectorService.cs
// Ubicación: src\ApiSAPBridge.Configuration\Services\SqlServerVersionDetectorService.cs

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ApiSAPBridge.Configuration.Services
{
    public interface ISqlServerVersionDetectorService
    {
        Task<SqlServerVersionInfo> DetectVersionAsync(string serverName, bool useIntegratedSecurity = true, string username = null, string password = null);
        Task<ConnectionStringOptimizationResult> OptimizeConnectionStringAsync(string baseConnectionString);
        bool ShouldUseTrustServerCertificate(SqlServerVersion version);
    }

    public class SqlServerVersionDetectorService : ISqlServerVersionDetectorService
    {
        private readonly ILogger<SqlServerVersionDetectorService> _logger;

        public SqlServerVersionDetectorService(ILogger<SqlServerVersionDetectorService> logger)
        {
            _logger = logger;
        }

        public async Task<SqlServerVersionInfo> DetectVersionAsync(string serverName, bool useIntegratedSecurity = true, string username = null, string password = null)
        {
            try
            {
                _logger.LogInformation("🔍 Detectando versión de SQL Server: {Server}", serverName);

                // Primero intentar sin TrustServerCertificate (para versiones antiguas)
                var versionInfo = await TryDetectVersionAsync(serverName, useIntegratedSecurity, username, password, false);

                if (versionInfo.DetectionSuccessful)
                {
                    _logger.LogInformation("✅ Versión detectada sin TrustServerCertificate: {Version}", versionInfo.VersionString);
                    return versionInfo;
                }

                _logger.LogDebug("🔄 Reintentando con TrustServerCertificate=true...");

                // Si falla, intentar con TrustServerCertificate=true (para versiones nuevas)
                versionInfo = await TryDetectVersionAsync(serverName, useIntegratedSecurity, username, password, true);

                if (versionInfo.DetectionSuccessful)
                {
                    _logger.LogInformation("✅ Versión detectada con TrustServerCertificate: {Version}", versionInfo.VersionString);
                    versionInfo.RequiresTrustServerCertificate = true;
                    return versionInfo;
                }

                _logger.LogWarning("⚠️ No se pudo detectar la versión de SQL Server");
                return new SqlServerVersionInfo
                {
                    DetectionSuccessful = false,
                    ErrorMessage = "No se pudo conectar al servidor para detectar la versión"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al detectar versión de SQL Server");
                return new SqlServerVersionInfo
                {
                    DetectionSuccessful = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task<SqlServerVersionInfo> TryDetectVersionAsync(string serverName, bool useIntegratedSecurity, string username, string password, bool trustServerCertificate)
        {
            try
            {
                var connectionBuilder = new SqlConnectionStringBuilder
                {
                    DataSource = serverName,
                    InitialCatalog = "master", // Conectar a master para obtener versión
                    IntegratedSecurity = useIntegratedSecurity,
                    ConnectTimeout = 10, // Timeout corto para detección rápida
                    TrustServerCertificate = trustServerCertificate
                };

                if (!useIntegratedSecurity)
                {
                    connectionBuilder.UserID = username;
                    connectionBuilder.Password = password;
                }

                using var connection = new SqlConnection(connectionBuilder.ConnectionString);
                await connection.OpenAsync();

                // Obtener información de versión
                var versionQuery = @"
                    SELECT 
                        @@VERSION as VersionString,
                        SERVERPROPERTY('ProductVersion') as ProductVersion,
                        SERVERPROPERTY('ProductLevel') as ProductLevel,
                        SERVERPROPERTY('Edition') as Edition,
                        SERVERPROPERTY('ProductMajorVersion') as MajorVersion,
                        SERVERPROPERTY('ProductMinorVersion') as MinorVersion
                ";

                using var command = new SqlCommand(versionQuery, connection);
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var versionString = reader["VersionString"].ToString();
                    var productVersion = reader["ProductVersion"].ToString();
                    var majorVersion = Convert.ToInt32(reader["MajorVersion"]);
                    var minorVersion = Convert.ToInt32(reader["MinorVersion"]);

                    var version = DetermineVersion(majorVersion, minorVersion);

                    return new SqlServerVersionInfo
                    {
                        DetectionSuccessful = true,
                        VersionString = versionString,
                        ProductVersion = productVersion,
                        Edition = reader["Edition"].ToString(),
                        ProductLevel = reader["ProductLevel"].ToString(),
                        MajorVersion = majorVersion,
                        MinorVersion = minorVersion,
                        Version = version,
                        RequiresTrustServerCertificate = trustServerCertificate
                    };
                }

                return new SqlServerVersionInfo
                {
                    DetectionSuccessful = false,
                    ErrorMessage = "No se pudo obtener información de versión"
                };
            }
            catch (Exception ex)
            {
                return new SqlServerVersionInfo
                {
                    DetectionSuccessful = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<ConnectionStringOptimizationResult> OptimizeConnectionStringAsync(string baseConnectionString)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(baseConnectionString);

                _logger.LogInformation("🔧 Optimizando connection string para servidor: {Server}", builder.DataSource);

                // Detectar versión del servidor
                var versionInfo = await DetectVersionAsync(
                    builder.DataSource,
                    builder.IntegratedSecurity,
                    builder.UserID,
                    builder.Password
                );

                if (!versionInfo.DetectionSuccessful)
                {
                    return new ConnectionStringOptimizationResult
                    {
                        Success = false,
                        ErrorMessage = $"No se pudo detectar la versión del servidor: {versionInfo.ErrorMessage}",
                        OriginalConnectionString = baseConnectionString
                    };
                }

                // Optimizar connection string basado en la versión
                var optimizedBuilder = new SqlConnectionStringBuilder(baseConnectionString);

                // Configurar TrustServerCertificate basado en la versión
                if (ShouldUseTrustServerCertificate(versionInfo.Version))
                {
                    optimizedBuilder.TrustServerCertificate = true;
                    _logger.LogInformation("✅ TrustServerCertificate=true aplicado para {Version}", versionInfo.Version);
                }
                else
                {
                    // Para versiones antiguas, remover la propiedad si existe
                    if (optimizedBuilder.TrustServerCertificate)
                    {
                        optimizedBuilder.Remove("TrustServerCertificate");
                        _logger.LogInformation("✅ TrustServerCertificate removido para {Version}", versionInfo.Version);
                    }
                }

                // Otras optimizaciones basadas en versión
                ApplyVersionSpecificOptimizations(optimizedBuilder, versionInfo.Version);

                return new ConnectionStringOptimizationResult
                {
                    Success = true,
                    OriginalConnectionString = baseConnectionString,
                    OptimizedConnectionString = optimizedBuilder.ConnectionString,
                    VersionInfo = versionInfo,
                    OptimizationsApplied = GetOptimizationsSummary(versionInfo.Version)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error al optimizar connection string");
                return new ConnectionStringOptimizationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    OriginalConnectionString = baseConnectionString
                };
            }
        }

        public bool ShouldUseTrustServerCertificate(SqlServerVersion version)
        {
            // SQL Server 2019 (15.x) y posteriores generalmente requieren TrustServerCertificate
            return version >= SqlServerVersion.SqlServer2019;
        }

        private SqlServerVersion DetermineVersion(int majorVersion, int minorVersion)
        {
            return majorVersion switch
            {
                16 => SqlServerVersion.SqlServer2022,
                15 => SqlServerVersion.SqlServer2019,
                14 => SqlServerVersion.SqlServer2017,
                13 => SqlServerVersion.SqlServer2016,
                12 => SqlServerVersion.SqlServer2014,
                11 => SqlServerVersion.SqlServer2012,
                10 when minorVersion >= 50 => SqlServerVersion.SqlServer2008R2,
                10 => SqlServerVersion.SqlServer2008,
                9 => SqlServerVersion.SqlServer2005,
                _ => SqlServerVersion.Unknown
            };
        }

        private void ApplyVersionSpecificOptimizations(SqlConnectionStringBuilder builder, SqlServerVersion version)
        {
            // Configuraciones específicas por versión
            switch (version)
            {
                case SqlServerVersion.SqlServer2022:
                case SqlServerVersion.SqlServer2019:
                    builder.MultipleActiveResultSets = true;
                    builder.ApplicationName = "ApiSAPBridge.Configuration";
                    break;

                case SqlServerVersion.SqlServer2017:
                case SqlServerVersion.SqlServer2016:
                    builder.MultipleActiveResultSets = true;
                    break;

                case SqlServerVersion.SqlServer2014:
                case SqlServerVersion.SqlServer2012:
                    // Configuraciones más conservadoras para versiones antiguas
                    builder.MultipleActiveResultSets = false;
                    break;
            }

            _logger.LogDebug("🔧 Optimizaciones aplicadas para {Version}", version);
        }

        private string GetOptimizationsSummary(SqlServerVersion version)
        {
            var optimizations = new List<string>();

            if (ShouldUseTrustServerCertificate(version))
            {
                optimizations.Add("TrustServerCertificate=true (requerido para SQL 2019+)");
            }
            else
            {
                optimizations.Add("TrustServerCertificate removido (no compatible con versiones antiguas)");
            }

            optimizations.Add($"Configuraciones específicas para {version}");
            optimizations.Add("MultipleActiveResultSets optimizado");

            return string.Join("; ", optimizations);
        }
    }

    public class SqlServerVersionInfo
    {
        public bool DetectionSuccessful { get; set; }
        public string VersionString { get; set; } = string.Empty;
        public string ProductVersion { get; set; } = string.Empty;
        public string Edition { get; set; } = string.Empty;
        public string ProductLevel { get; set; } = string.Empty;
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public SqlServerVersion Version { get; set; }
        public bool RequiresTrustServerCertificate { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public string GetFriendlyVersionName()
        {
            return Version switch
            {
                SqlServerVersion.SqlServer2022 => "SQL Server 2022",
                SqlServerVersion.SqlServer2019 => "SQL Server 2019",
                SqlServerVersion.SqlServer2017 => "SQL Server 2017",
                SqlServerVersion.SqlServer2016 => "SQL Server 2016",
                SqlServerVersion.SqlServer2014 => "SQL Server 2014",
                SqlServerVersion.SqlServer2012 => "SQL Server 2012",
                SqlServerVersion.SqlServer2008R2 => "SQL Server 2008 R2",
                SqlServerVersion.SqlServer2008 => "SQL Server 2008",
                SqlServerVersion.SqlServer2005 => "SQL Server 2005",
                _ => $"Versión desconocida ({MajorVersion}.{MinorVersion})"
            };
        }
    }

    public class ConnectionStringOptimizationResult
    {
        public bool Success { get; set; }
        public string OriginalConnectionString { get; set; } = string.Empty;
        public string OptimizedConnectionString { get; set; } = string.Empty;
        public SqlServerVersionInfo? VersionInfo { get; set; }
        public string OptimizationsApplied { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public enum SqlServerVersion
    {
        Unknown = 0,
        SqlServer2005 = 9,
        SqlServer2008 = 10,
        SqlServer2008R2 = 11,
        SqlServer2012 = 12,
        SqlServer2014 = 13,
        SqlServer2016 = 14,
        SqlServer2017 = 15,
        SqlServer2019 = 16,
        SqlServer2022 = 17
    }
}