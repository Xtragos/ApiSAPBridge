using System.Text.Json;
using ApiSAPBridge.Configuration.Models;
using System.Data.SqlClient;
using Serilog;

namespace ApiSAPBridge.Configuration.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly string _configPath;
        private readonly string _securityConfigPath;
        private readonly ILogger _logger;

        public ConfigurationService()
        {
            _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationConstants.CONFIG_FILE_NAME);
            _securityConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationConstants.SECURITY_CONFIG_FILE);

            _logger = new LoggerConfiguration()
                .WriteTo.File("logs/configuration-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public async Task<AppConfiguration> LoadConfigurationAsync()
        {
            try
            {
                if (!File.Exists(_configPath))
                {
                    _logger.Information("Archivo de configuración no existe, creando configuración por defecto");
                    var defaultConfig = CreateDefaultConfiguration();
                    await SaveConfigurationAsync(defaultConfig);
                    return defaultConfig;
                }

                var jsonContent = await File.ReadAllTextAsync(_configPath);
                var configuration = JsonSerializer.Deserialize<AppConfiguration>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                });

                _logger.Information("Configuración cargada exitosamente");
                return configuration ?? CreateDefaultConfiguration();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error al cargar la configuración");
                return CreateDefaultConfiguration();
            }
        }

        public async Task SaveConfigurationAsync(AppConfiguration configuration)
        {
            try
            {
                configuration.LastModified = DateTime.Now;

                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var jsonContent = JsonSerializer.Serialize(configuration, jsonOptions);
                await File.WriteAllTextAsync(_configPath, jsonContent);

                _logger.Information("Configuración guardada exitosamente en {Path}", _configPath);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error al guardar la configuración");
                throw;
            }
        }

        public async Task<bool> TestSqlConnectionAsync(SqlServerConfiguration sqlConfig)
        {
            try
            {
                using var connection = new SqlConnection(sqlConfig.GetConnectionString());

                // Configurar timeout personalizado
                //connection.ConnectionTimeout = sqlConfig.ConnectionTimeout;

                await connection.OpenAsync();

                // Ejecutar una consulta simple para verificar permisos
                using var command = new SqlCommand("SELECT 1", connection);
                await command.ExecuteScalarAsync();

                _logger.Information("Prueba de conexión SQL exitosa para servidor: {Server}", sqlConfig.Server);
                return true;
            }
            catch (SqlException sqlEx)
            {
                _logger.Error(sqlEx, "Error SQL en prueba de conexión para servidor: {Server}. Código: {ErrorNumber}",
                    sqlConfig.Server, sqlEx.Number);
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error general en prueba de conexión SQL para servidor: {Server}", sqlConfig.Server);
                return false;
            }
        }
        public async Task<SecurityConfiguration> LoadSecurityConfigurationAsync()
        {
            try
            {
                if (!File.Exists(_securityConfigPath))
                {
                    var defaultSecurity = new SecurityConfiguration();
                    await SaveSecurityConfigurationAsync(defaultSecurity);
                    return defaultSecurity;
                }

                var jsonContent = await File.ReadAllTextAsync(_securityConfigPath);
                var securityConfig = JsonSerializer.Deserialize<SecurityConfiguration>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return securityConfig ?? new SecurityConfiguration();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error al cargar configuración de seguridad");
                return new SecurityConfiguration();
            }
        }

        public async Task SaveSecurityConfigurationAsync(SecurityConfiguration securityConfig)
        {
            try
            {
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var jsonContent = JsonSerializer.Serialize(securityConfig, jsonOptions);
                await File.WriteAllTextAsync(_securityConfigPath, jsonContent);

                _logger.Information("Configuración de seguridad guardada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error al guardar configuración de seguridad");
                throw;
            }
        }

        public string GetConfigurationFilePath()
        {
            return _configPath;
        }

        public bool ConfigurationExists()
        {
            return File.Exists(_configPath);
        }

        private AppConfiguration CreateDefaultConfiguration()
        {
            return new AppConfiguration
            {
                SqlServer = new SqlServerConfiguration
                {
                    Server = ConfigurationConstants.DefaultValues.DEFAULT_SERVER,
                    Database = ConfigurationConstants.DefaultValues.DEFAULT_DATABASE,
                    UseWindowsAuthentication = true,
                    ConnectionTimeout = ConfigurationConstants.DefaultValues.DEFAULT_CONNECTION_TIMEOUT
                },
                SapAutomation = new SapAutomationConfiguration
                {
                    SyncIntervalMinutes = ConfigurationConstants.DefaultValues.DEFAULT_SYNC_INTERVAL,
                    EnableAutomaticSync = true
                },
                Swagger = new SwaggerConfiguration
                {
                    EnableSwagger = true,
                    EnableSwaggerUI = true,
                    AllowedMethods = new List<string> { "GET", "POST" }
                }
            };
        }
    }
}