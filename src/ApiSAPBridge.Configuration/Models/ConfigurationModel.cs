using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ApiSAPBridge.Configuration.Models
{
    public class ConfigurationModel
    {
        public SqlConfiguration SqlConfig { get; set; } = new();
        public MethodConfiguration MethodConfig { get; set; } = new();
        public SwaggerConfiguration SwaggerConfig { get; set; } = new();
        public SecurityConfiguration SecurityConfig { get; set; } = new();
    }

    public class SqlConfiguration
    {
        [DisplayName("Servidor SQL")]
        [Description("Dirección del servidor SQL Server")]
        public string Server { get; set; } = "localhost";

        [DisplayName("Base de Datos")]
        [Description("Nombre de la base de datos ApiSAP")]
        public string Database { get; set; } = "ApiSAP";

        [DisplayName("Usuario")]
        [Description("Usuario para conexión SQL")]
        public string Username { get; set; } = "";

        [DisplayName("Contraseña")]
        [Description("Contraseña para conexión SQL")]
        [JsonIgnore]
        public string Password { get; set; } = "";

        [DisplayName("Autenticación Windows")]
        [Description("Usar autenticación integrada de Windows")]
        public bool UseWindowsAuthentication { get; set; } = true;

        [DisplayName("Timeout Conexión")]
        [Description("Timeout en segundos para conexión SQL")]
        public int ConnectionTimeout { get; set; } = 30;

        [DisplayName("Habilitar Pooling")]
        [Description("Habilitar pooling de conexiones")]
        public bool EnablePooling { get; set; } = true;

        [DisplayName("Tamaño Mínimo Pool")]
        [Description("Tamaño mínimo del pool de conexiones")]
        public int MinPoolSize { get; set; } = 5;

        [DisplayName("Tamaño Máximo Pool")]
        [Description("Tamaño máximo del pool de conexiones")]
        public int MaxPoolSize { get; set; } = 100;

        public string GetConnectionString()
        {
            var builder = new System.Data.SqlClient.SqlConnectionStringBuilder
            {
                DataSource = Server,
                InitialCatalog = Database,
                ConnectTimeout = ConnectionTimeout,
                Pooling = EnablePooling,
                MinPoolSize = MinPoolSize,
                MaxPoolSize = MaxPoolSize
            };

            if (UseWindowsAuthentication)
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

    public class MethodConfiguration
    {
        [DisplayName("Intervalo Sincronización (minutos)")]
        [Description("Intervalo en minutos para ejecutar sincronización automática")]
        public int SyncIntervalMinutes { get; set; } = 60;

        [DisplayName("Habilitar Sincronización Automática")]
        [Description("Habilitar la sincronización automática con SAP")]
        public bool EnableAutoSync { get; set; } = true;

        [DisplayName("Hora Inicio Sincronización")]
        [Description("Hora de inicio de sincronización automática")]
        public TimeSpan StartTime { get; set; } = new TimeSpan(8, 0, 0);

        [DisplayName("Hora Fin Sincronización")]
        [Description("Hora de fin de sincronización automática")]
        public TimeSpan EndTime { get; set; } = new TimeSpan(18, 0, 0);

        [DisplayName("Ejecutar Fines de Semana")]
        [Description("Ejecutar sincronización en fines de semana")]
        public bool RunOnWeekends { get; set; } = false;

        public Dictionary<string, EndpointConfiguration> Endpoints { get; set; } = new()
        {
            ["Departamentos"] = new EndpointConfiguration { Name = "Departamentos", Enabled = true, Priority = 1 },
            ["Secciones"] = new EndpointConfiguration { Name = "Secciones", Enabled = true, Priority = 2 },
            ["Familias"] = new EndpointConfiguration { Name = "Familias", Enabled = true, Priority = 3 },
            ["Vendedores"] = new EndpointConfiguration { Name = "Vendedores", Enabled = true, Priority = 4 },
            ["Impuestos"] = new EndpointConfiguration { Name = "Impuestos", Enabled = true, Priority = 5 },
            ["FormasPago"] = new EndpointConfiguration { Name = "Formas de Pago", Enabled = true, Priority = 6 },
            ["Clientes"] = new EndpointConfiguration { Name = "Clientes", Enabled = true, Priority = 7 },
            ["Tarifas"] = new EndpointConfiguration { Name = "Tarifas", Enabled = true, Priority = 8 },
            ["Articulos"] = new EndpointConfiguration { Name = "Artículos", Enabled = true, Priority = 9 },
            ["ArticuloLineas"] = new EndpointConfiguration { Name = "Líneas de Artículos", Enabled = true, Priority = 10 },
            ["Precios"] = new EndpointConfiguration { Name = "Precios", Enabled = true, Priority = 11 },
            ["Facturas"] = new EndpointConfiguration { Name = "Facturas", Enabled = false, Priority = 12 }
        };
    }

    public class EndpointConfiguration
    {
        public string Name { get; set; } = "";
        public bool Enabled { get; set; } = true;
        public int Priority { get; set; } = 1;
        public string SapEndpoint { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime LastSync { get; set; } = DateTime.MinValue;
        public int BatchSize { get; set; } = 100;
        public int MaxRetries { get; set; } = 3;
    }

    public class SwaggerConfiguration
    {
        [DisplayName("Habilitar Swagger")]
        [Description("Habilitar interfaz Swagger UI")]
        public bool EnableSwagger { get; set; } = true;

        [DisplayName("Mostrar en Producción")]
        [Description("Mostrar Swagger en ambiente de producción")]
        public bool ShowInProduction { get; set; } = false;

        [DisplayName("Título API")]
        [Description("Título mostrado en Swagger")]
        public string Title { get; set; } = "ApiSAPBridge API";

        [DisplayName("Descripción")]
        [Description("Descripción de la API")]
        public string Description { get; set; } = "API Bridge para sincronización con SAP";

        [DisplayName("Versión")]
        [Description("Versión de la API")]
        public string Version { get; set; } = "v1";

        [DisplayName("URL Contacto")]
        [Description("URL de contacto para soporte")]
        public string ContactUrl { get; set; } = "";

        public Dictionary<string, ControllerVisibility> Controllers { get; set; } = new()
        {
            ["Departamentos"] = new ControllerVisibility { Name = "Departamentos", Visible = true, RequireAuth = false },
            ["Secciones"] = new ControllerVisibility { Name = "Secciones", Visible = true, RequireAuth = false },
            ["Familias"] = new ControllerVisibility { Name = "Familias", Visible = true, RequireAuth = false },
            ["Vendedores"] = new ControllerVisibility { Name = "Vendedores", Visible = true, RequireAuth = false },
            ["Impuestos"] = new ControllerVisibility { Name = "Impuestos", Visible = true, RequireAuth = false },
            ["FormasPago"] = new ControllerVisibility { Name = "Formas de Pago", Visible = true, RequireAuth = false },
            ["Clientes"] = new ControllerVisibility { Name = "Clientes", Visible = true, RequireAuth = false },
            ["Tarifas"] = new ControllerVisibility { Name = "Tarifas", Visible = true, RequireAuth = false },
            ["Articulos"] = new ControllerVisibility { Name = "Artículos", Visible = true, RequireAuth = false },
            ["ArticuloLineas"] = new ControllerVisibility { Name = "Líneas de Artículos", Visible = true, RequireAuth = false },
            ["Precios"] = new ControllerVisibility { Name = "Precios", Visible = true, RequireAuth = false },
            ["Facturas"] = new ControllerVisibility { Name = "Facturas", Visible = true, RequireAuth = true }
        };

        public Dictionary<string, bool> HttpMethods { get; set; } = new()
        {
            ["GET"] = true,
            ["POST"] = true,
            ["PUT"] = false,
            ["DELETE"] = false,
            ["PATCH"] = false
        };
    }

    public class ControllerVisibility
    {
        public string Name { get; set; } = "";
        public bool Visible { get; set; } = true;
        public bool RequireAuth { get; set; } = false;
        public string Description { get; set; } = "";
        public List<string> AllowedRoles { get; set; } = new();
    }

    public class SecurityConfiguration
    {
        public string AdminPasswordHash { get; set; } = "";
        public DateTime LastPasswordChange { get; set; } = DateTime.Now;
        public int PasswordExpirationDays { get; set; } = 90;
        public bool RequirePasswordChange { get; set; } = true;
    }
}