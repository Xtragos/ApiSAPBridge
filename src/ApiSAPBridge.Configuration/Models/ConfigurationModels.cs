using System.ComponentModel.DataAnnotations;

namespace ApiSAPBridge.Configuration.Models
{
    /// <summary>
    /// Configuración de conexión a SQL Server
    /// </summary>
    public class SqlServerConfiguration
    {
        [Required(ErrorMessage = "El servidor es requerido")]
        public string Server { get; set; } = "localhost";

        [Required(ErrorMessage = "La base de datos es requerida")]
        public string Database { get; set; } = "ApiSAPBridge";

        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool UseWindowsAuthentication { get; set; } = true;
        public int ConnectionTimeout { get; set; } = 30;
        public bool TrustServerCertificate { get; set; } = true;

        /// <summary>
        /// Genera la cadena de conexión
        /// </summary>
        public string GetConnectionString()
        {
            if (UseWindowsAuthentication)
            {
                return $"Server={Server};Database={Database};Integrated Security=true;Connection Timeout={ConnectionTimeout};TrustServerCertificate={TrustServerCertificate};";
            }
            else
            {
                return $"Server={Server};Database={Database};User Id={Username};Password={Password};Connection Timeout={ConnectionTimeout};TrustServerCertificate={TrustServerCertificate};";
            }
        }
    }

    /// <summary>
    /// Configuración de automatización SAP
    /// </summary>
    public class SapAutomationConfiguration
    {
        public int SyncIntervalMinutes { get; set; } = 30;
        public bool EnableAutomaticSync { get; set; } = true;
        public DateTime LastSyncTime { get; set; }
        public List<SapEndpointConfiguration> Endpoints { get; set; } = new();
    }

    /// <summary>
    /// Configuración de endpoints SAP
    /// </summary>
    public class SapEndpointConfiguration
    {
        public string Name { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string Method { get; set; } = "POST";
        public bool IsEnabled { get; set; } = true;
        public int Priority { get; set; } = 1;
        public DateTime LastExecuted { get; set; }
        public bool RequiresAuthentication { get; set; } = true;
    }

    /// <summary>
    /// Configuración de Swagger
    /// </summary>
    public class SwaggerConfiguration
    {
        public bool EnableSwagger { get; set; } = true;
        public bool EnableSwaggerUI { get; set; } = true;
        public List<SwaggerEndpointConfiguration> HiddenEndpoints { get; set; } = new();
        public List<string> AllowedMethods { get; set; } = new() { "GET", "POST" };
    }

    /// <summary>
    /// Configuración de endpoints en Swagger
    /// </summary>
    public class SwaggerEndpointConfiguration
    {
        public string Controller { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public bool IsHidden { get; set; } = false;
    }

    /// <summary>
    /// Configuración principal de la aplicación
    /// </summary>
    public class AppConfiguration
    {
        public SqlServerConfiguration SqlServer { get; set; } = new();
        public SapAutomationConfiguration SapAutomation { get; set; } = new();
        public SwaggerConfiguration Swagger { get; set; } = new();
        public DateTime LastModified { get; set; } = DateTime.Now;
        public string Version { get; set; } = "1.0.0";
    }

    /// <summary>
    /// Configuración de seguridad
    /// </summary>
    public class SecurityConfiguration
    {
        public string ConfigurationPassword { get; set; } = "admin123";
        public bool RequirePasswordForMethods { get; set; } = true;
        public bool RequirePasswordForSwagger { get; set; } = true;
        public int MaxLoginAttempts { get; set; } = 3;
        public int LockoutMinutes { get; set; } = 15;
    }
}