using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;


namespace ApiSAPBridge.Models.Configuration
{
    /// <summary>
    /// Modelo para configuración de conexión SQL Server
    /// </summary>
    public class SqlConfiguration
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El servidor es requerido")]
        public string Server { get; set; } = string.Empty;

        [Required(ErrorMessage = "La base de datos es requerida")]
        public string Database { get; set; } = "ApiSAP";

        public string? Username { get; set; }

        public string? Password { get; set; }

        public bool UseIntegratedSecurity { get; set; } = true;

        public int ConnectionTimeout { get; set; } = 30;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Genera la cadena de conexión basada en la configuración
        /// </summary>
        public string GetConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = Server,
                InitialCatalog = Database,
                ConnectTimeout = ConnectionTimeout,
                MultipleActiveResultSets = true,
                TrustServerCertificate = true
            };

            if (UseIntegratedSecurity)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.UserID = Username;
                builder.Password = Password;
            }

            return builder.ConnectionString;
        }
    }

    /// <summary>
    /// Modelo para configuración de métodos SAP
    /// </summary>
    public class MethodConfiguration
    {
        public int Id { get; set; }

        [Required]
        public string MethodName { get; set; } = string.Empty;

        [Required]
        public string HttpMethod { get; set; } = "POST";

        [Required]
        public string Endpoint { get; set; } = string.Empty;

        public bool IsEnabled { get; set; } = true;

        public bool IsAutomaticSync { get; set; } = false;

        public int SyncIntervalMinutes { get; set; } = 30;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastExecuted { get; set; }
    }

    /// <summary>
    /// Modelo para configuración de Swagger
    /// </summary>
    public class SwaggerConfiguration
    {
        public int Id { get; set; }

        [Required]
        public string MethodName { get; set; } = string.Empty;

        [Required]
        public string HttpMethod { get; set; } = string.Empty;

        [Required]
        public string Endpoint { get; set; } = string.Empty;

        public bool IsVisible { get; set; } = true;

        public string? Category { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Modelo para configuración general del sistema
    /// </summary>
    public class SystemConfiguration
    {
        public int Id { get; set; }

        [Required]
        public string Key { get; set; } = string.Empty;

        [Required]
        public string Value { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Modelo para configuración de seguridad
    /// </summary>
    public class SecurityConfiguration
    {
        public int Id { get; set; }

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLogin { get; set; }

        public int LoginAttempts { get; set; } = 0;

        public DateTime? LockedUntil { get; set; }
    }
}