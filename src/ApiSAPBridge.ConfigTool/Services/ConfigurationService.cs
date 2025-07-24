using System.Text.Json;
using ApiSAPBridge.ConfigTool.Models;
using ApiSAPBridge.ConfigTool.Utils;

namespace ApiSAPBridge.ConfigTool.Services
{
    public class ConfigurationService
    {
        private readonly string _configPath;
        private readonly EncryptionHelper _encryption;

        public ConfigurationService()
        {
            _configPath = Path.Combine(Application.StartupPath, "config", "apisap-config.json");
            _encryption = new EncryptionHelper();
            EnsureConfigDirectory();
        }

        private void EnsureConfigDirectory()
        {
            var configDir = Path.GetDirectoryName(_configPath);
            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
            }
        }

        public ConfigurationModel LoadConfiguration()
        {
            if (!File.Exists(_configPath))
            {
                return CreateDefaultConfiguration();
            }

            try
            {
                var json = File.ReadAllText(_configPath);
                var config = JsonSerializer.Deserialize<ConfigurationModel>(json, GetJsonOptions());

                // Desencriptar contraseñas
                if (!string.IsNullOrEmpty(config.SqlConfig.Password))
                {
                    config.SqlConfig.Password = _encryption.Decrypt(config.SqlConfig.Password);
                }

                return config ?? CreateDefaultConfiguration();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar configuración: {ex.Message}");
            }
        }

        public ConfigurationModel LoadConfigurationFromFile(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                var config = JsonSerializer.Deserialize<ConfigurationModel>(json, GetJsonOptions());
                return config ?? CreateDefaultConfiguration();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar configuración desde archivo: {ex.Message}");
            }
        }

        public void SaveConfiguration(ConfigurationModel config)
        {
            try
            {
                // Encriptar contraseña antes de guardar
                var configToSave = CloneConfig(config);
                if (!string.IsNullOrEmpty(configToSave.SqlConfig.Password))
                {
                    configToSave.SqlConfig.Password = _encryption.Encrypt(configToSave.SqlConfig.Password);
                }

                var json = JsonSerializer.Serialize(configToSave, GetJsonOptions());
                File.WriteAllText(_configPath, json);

                // También crear una copia de respaldo
                var backupPath = _configPath.Replace(".json", $"-backup-{DateTime.Now:yyyyMMdd-HHmmss}.json");
                File.WriteAllText(backupPath, json);

                // Limpiar backups antiguos (mantener solo los últimos 5)
                CleanupOldBackups();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar configuración: {ex.Message}");
            }
        }

        private ConfigurationModel CreateDefaultConfiguration()
        {
            var config = new ConfigurationModel();

            // Métodos por defecto para automatización
            config.MethodsConfig.Methods = new List<MethodExecutionConfig>
            {
                new MethodExecutionConfig
                {
                    MethodName = "SyncDepartamentos",
                    DisplayName = "Sincronizar Departamentos",
                    Endpoint = "/api/departamentos",
                    HttpMethod = "POST",
                    IsEnabled = false,
                    ExecutionOrder = 1,
                    Category = "Maestros",
                    Description = "Sincroniza departamentos desde SAP",
                    TimeoutSeconds = 120
                },
                new MethodExecutionConfig
                {
                    MethodName = "SyncSecciones",
                    DisplayName = "Sincronizar Secciones",
                    Endpoint = "/api/secciones",
                    HttpMethod = "POST",
                    IsEnabled = false,
                    ExecutionOrder = 2,
                    Category = "Maestros",
                    Description = "Sincroniza secciones desde SAP"
                },
                new MethodExecutionConfig
                {
                    MethodName = "SyncFamilias",
                    DisplayName = "Sincronizar Familias",
                    Endpoint = "/api/familias",
                    HttpMethod = "POST",
                    IsEnabled = false,
                    ExecutionOrder = 3,
                    Category = "Maestros",
                    Description = "Sincroniza familias desde SAP"
                },
                new MethodExecutionConfig
                {
                    MethodName = "SyncClientes",
                    DisplayName = "Sincronizar Clientes",
                    Endpoint = "/api/clientes",
                    HttpMethod = "POST",
                    IsEnabled = false,
                    ExecutionOrder = 4,
                    Category = "Maestros",
                    Description = "Sincroniza clientes desde SAP"
                },
                new MethodExecutionConfig
                {
                    MethodName = "SyncArticulos",
                    DisplayName = "Sincronizar Artículos",
                    Endpoint = "/api/articulos/completo",
                    HttpMethod = "POST",
                    IsEnabled = false,
                    ExecutionOrder = 5,
                    Category = "Productos",
                    Description = "Sincroniza artículos completos desde SAP",
                    TimeoutSeconds = 600
                },
                new MethodExecutionConfig
                {
                    MethodName = "SyncPrecios",
                    DisplayName = "Sincronizar Precios",
                    Endpoint = "/api/precios/masivos",
                    HttpMethod = "POST",
                    IsEnabled = false,
                    ExecutionOrder = 6,
                    Category = "Productos",
                    Description = "Sincroniza precios desde SAP",
                    TimeoutSeconds = 300
                }
            };

            // Endpoints por defecto para Swagger
            config.SwaggerConfig.Endpoints = new List<EndpointVisibilityConfig>
            {
                new EndpointVisibilityConfig { Controller = "Departamentos", Action = "GetDepartamentos", HttpMethod = "GET", IsVisible = true, Category = "Consultas" },
                new EndpointVisibilityConfig { Controller = "Departamentos", Action = "CreateDepartamentos", HttpMethod = "POST", IsVisible = false, RequiresAuth = true, Category = "Administración" },
                new EndpointVisibilityConfig { Controller = "Articulos", Action = "GetArticulos", HttpMethod = "GET", IsVisible = true, Category = "Productos" },
                new EndpointVisibilityConfig { Controller = "Articulos", Action = "CreateArticulos", HttpMethod = "POST", IsVisible = false, RequiresAuth = true, Category = "Administración" },
                new EndpointVisibilityConfig { Controller = "Precios", Action = "GetPrecios", HttpMethod = "GET", IsVisible = true, Category = "Precios" },
                new EndpointVisibilityConfig { Controller = "Precios", Action = "CreatePrecios", HttpMethod = "POST", IsVisible = false, RequiresAuth = true, Category = "Administración" }
                // ... más endpoints
            };

            return config;
        }

        private ConfigurationModel CloneConfig(ConfigurationModel config)
        {
            var json = JsonSerializer.Serialize(config, GetJsonOptions());
            return JsonSerializer.Deserialize<ConfigurationModel>(json, GetJsonOptions());
        }

        private JsonSerializerOptions GetJsonOptions()
        {
            return new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
        }

        private void CleanupOldBackups()
        {
            try
            {
                var configDir = Path.GetDirectoryName(_configPath);
                var backupFiles = Directory.GetFiles(configDir, "*-backup-*.json")
                    .OrderByDescending(f => File.GetCreationTime(f))
                    .Skip(5);

                foreach (var file in backupFiles)
                {
                    File.Delete(file);
                }
            }
            catch
            {
                // Ignorar errores de limpieza
            }
        }
    }
}