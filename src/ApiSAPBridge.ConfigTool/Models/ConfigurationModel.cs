using System.ComponentModel;

namespace ApiSAPBridge.ConfigTool.Models
{
    public class ConfigurationModel
    {
        public SqlConnectionConfig SqlConfig { get; set; } = new SqlConnectionConfig();
        public MethodsConfig MethodsConfig { get; set; } = new MethodsConfig();
        public SwaggerConfig SwaggerConfig { get; set; } = new SwaggerConfig();
        public DateTime LastUpdated { get; set; } = DateTime.Now;
        public string Version { get; set; } = "1.0.0";
    }

    public class SqlConnectionConfig
    {
        [DisplayName("Servidor SQL")]
        public string Server { get; set; } = "localhost";

        [DisplayName("Base de Datos")]
        public string Database { get; set; } = "ApiSAP";

        [DisplayName("Usuario")]
        public string Username { get; set; } = "";

        [DisplayName("Contraseña")]
        public string Password { get; set; } = "";

        [DisplayName("Usar Windows Authentication")]
        public bool UseWindowsAuth { get; set; } = true;

        [DisplayName("Puerto")]
        public int Port { get; set; } = 1433;

        [DisplayName("Timeout (segundos)")]
        public int ConnectionTimeout { get; set; } = 30;

        [DisplayName("Pool de Conexiones")]
        public bool EnableConnectionPooling { get; set; } = true;

        [DisplayName("Tamaño Mínimo Pool")]
        public int MinPoolSize { get; set; } = 5;

        [DisplayName("Tamaño Máximo Pool")]
        public int MaxPoolSize { get; set; } = 100;

        public string GetConnectionString()
        {
            if (UseWindowsAuth)
            {
                return $"Server={Server},{Port};Database={Database};Trusted_Connection=true;Connection Timeout={ConnectionTimeout};Min Pool Size={MinPoolSize};Max Pool Size={MaxPoolSize};Pooling={EnableConnectionPooling.ToString().ToLower()};";
            }
            else
            {
                return $"Server={Server},{Port};Database={Database};User Id={Username};Password={Password};Connection Timeout={ConnectionTimeout};Min Pool Size={MinPoolSize};Max Pool Size={MaxPoolSize};Pooling={EnableConnectionPooling.ToString().ToLower()};";
            }
        }
    }

    public class MethodsConfig
    {
        [DisplayName("Automatización Habilitada")]
        public bool AutomationEnabled { get; set; } = false;

        [DisplayName("Intervalo de Ejecución (minutos)")]
        public int ExecutionIntervalMinutes { get; set; } = 60;

        [DisplayName("Hora de Inicio")]
        public TimeSpan StartTime { get; set; } = new TimeSpan(8, 0, 0); // 8:00 AM

        [DisplayName("Hora de Fin")]
        public TimeSpan EndTime { get; set; } = new TimeSpan(18, 0, 0); // 6:00 PM

        [DisplayName("Ejecutar Fines de Semana")]
        public bool RunOnWeekends { get; set; } = false;

        [DisplayName("Reintentos por Error")]
        public int RetryAttempts { get; set; } = 3;

        [DisplayName("Delay entre Reintentos (segundos)")]
        public int RetryDelaySeconds { get; set; } = 30;

        public List<MethodExecutionConfig> Methods { get; set; } = new List<MethodExecutionConfig>();
    }

    public class MethodExecutionConfig
    {
        public string MethodName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Endpoint { get; set; } = "";
        public string HttpMethod { get; set; } = "POST";
        public bool IsEnabled { get; set; } = false;
        public int ExecutionOrder { get; set; } = 0;
        public bool RequiresAuthentication { get; set; } = true;
        public string Description { get; set; } = "";
        public int TimeoutSeconds { get; set; } = 300;
        public string Category { get; set; } = "General";
    }

    public class SwaggerConfig
    {
        [DisplayName("Swagger Habilitado")]
        public bool SwaggerEnabled { get; set; } = true;

        [DisplayName("Mostrar en Producción")]
        public bool ShowInProduction { get; set; } = false;

        [DisplayName("Requerir Autenticación")]
        public bool RequireAuthentication { get; set; } = true;

        [DisplayName("Título de la API")]
        public string ApiTitle { get; set; } = "API SAP Bridge";

        [DisplayName("Versión de la API")]
        public string ApiVersion { get; set; } = "v1";

        [DisplayName("Descripción")]
        public string ApiDescription { get; set; } = "API para integración con SAP";

        public List<EndpointVisibilityConfig> Endpoints { get; set; } = new List<EndpointVisibilityConfig>();
    }

    public class EndpointVisibilityConfig
    {
        public string Controller { get; set; } = "";
        public string Action { get; set; } = "";
        public string HttpMethod { get; set; } = "";
        public string FullPath { get; set; } = "";
        public bool IsVisible { get; set; } = true;
        public bool RequiresAuth { get; set; } = false;
        public string Category { get; set; } = "";
        public string Description { get; set; } = "";
    }
}