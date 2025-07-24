using ApiSAPBridge.Configuration.Models;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ApiSAPBridge.Configuration.Services
{
    public class ConfigurationService
    {
        private readonly ILogger<ConfigurationService> _logger;
        private readonly string _configFilePath;
        private ConfigurationModel _configuration;

        public ConfigurationService(ILogger<ConfigurationService> logger)
        {
            _logger = logger;
            _configFilePath = Path.Combine(Application.StartupPath, "appsettings.json");
            _configuration = LoadConfiguration();
        }

        public ConfigurationModel Configuration => _configuration;

        public event EventHandler<ConfigurationModel>? ConfigurationChanged;

        public ConfigurationModel LoadConfiguration()
        {
            try
            {
                if (File.Exists(_configFilePath))
                {
                    var json = File.ReadAllText(_configFilePath);
                    var config = JsonSerializer.Deserialize<ConfigurationModel>(json) ?? new ConfigurationModel();

                    // Desencriptar contraseña SQL
                    if (!string.IsNullOrEmpty(config.SqlConfig.Password))
                    {
                        config.SqlConfig.Password = DecryptPassword(config.SqlConfig.Password);
                    }

                    _logger.LogInformation("Configuración cargada exitosamente");
                    return config;
                }
                else
                {
                    _logger.LogWarning("Archivo de configuración no encontrado, creando configuración por defecto");
                    var defaultConfig = CreateDefaultConfiguration();
                    SaveConfiguration(defaultConfig);
                    return defaultConfig;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar configuración");
                return CreateDefaultConfiguration();
            }
        }

        public void SaveConfiguration(ConfigurationModel configuration)
        {
            try
            {
                _configuration = configuration;

                // Crear copia para serialización
                var configToSave = JsonSerializer.Deserialize<ConfigurationModel>(
                    JsonSerializer.Serialize(configuration));

                // Encriptar contraseña SQL
                if (!string.IsNullOrEmpty(configToSave.SqlConfig.Password))
                {
                    configToSave.SqlConfig.Password = EncryptPassword(configToSave.SqlConfig.Password);
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(configToSave, options);
                File.WriteAllText(_configFilePath, json);

                _logger.LogInformation("Configuración guardada exitosamente");
                ConfigurationChanged?.Invoke(this, _configuration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar configuración");
                throw;
            }
        }

        public void UpdateSqlConfiguration(SqlConfiguration sqlConfig)
        {
            _configuration.SqlConfig = sqlConfig;
            SaveConfiguration(_configuration);
        }

        public void UpdateMethodConfiguration(MethodConfiguration methodConfig)
        {
            _configuration.MethodConfig = methodConfig;
            SaveConfiguration(_configuration);
        }

        public void UpdateSwaggerConfiguration(SwaggerConfiguration swaggerConfig)
        {
            _configuration.SwaggerConfig = swaggerConfig;
            SaveConfiguration(_configuration);
        }

        public bool ValidateAdminPassword(string password)
        {
            var hash = HashPassword(password);
            return hash == _configuration.SecurityConfig.AdminPasswordHash;
        }

        public void SetAdminPassword(string password)
        {
            _configuration.SecurityConfig.AdminPasswordHash = HashPassword(password);
            _configuration.SecurityConfig.LastPasswordChange = DateTime.Now;
            SaveConfiguration(_configuration);
        }

        public void ExportConfiguration(string filePath)
        {
            try
            {
                var configCopy = JsonSerializer.Deserialize<ConfigurationModel>(
                    JsonSerializer.Serialize(_configuration));

                // Limpiar información sensible para exportación
                configCopy.SqlConfig.Password = "[PROTEGIDO]";
                configCopy.SecurityConfig.AdminPasswordHash = "[PROTEGIDO]";

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(configCopy, options);
                File.WriteAllText(filePath, json);

                _logger.LogInformation("Configuración exportada a {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al exportar configuración");
                throw;
            }
        }

        public void ImportConfiguration(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                var importedConfig = JsonSerializer.Deserialize<ConfigurationModel>(json);

                if (importedConfig != null)
                {
                    // Mantener configuración de seguridad actual
                    importedConfig.SecurityConfig = _configuration.SecurityConfig;

                    SaveConfiguration(importedConfig);
                    _logger.LogInformation("Configuración importada desde {FilePath}", filePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al importar configuración");
                throw;
            }
        }

        private ConfigurationModel CreateDefaultConfiguration()
        {
            var config = new ConfigurationModel();

            // Configurar contraseña por defecto (admin123)
            config.SecurityConfig.AdminPasswordHash = HashPassword("admin123");
            config.SecurityConfig.RequirePasswordChange = true;

            return config;
        }

        private string EncryptPassword(string password)
        {
            try
            {
                var data = Encoding.UTF8.GetBytes(password);
                var encrypted = ProtectedData.Protect(data, null, DataProtectionScope.LocalMachine);
                return Convert.ToBase64String(encrypted);
            }
            catch
            {
                return password; // Fallback a texto plano si no se puede encriptar
            }
        }

        private string DecryptPassword(string encryptedPassword)
        {
            try
            {
                var data = Convert.FromBase64String(encryptedPassword);
                var decrypted = ProtectedData.Unprotect(data, null, DataProtectionScope.LocalMachine);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch
            {
                return encryptedPassword; // Fallback a texto plano si no se puede desencriptar
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "ApiSAPBridge_Salt"));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}