using ApiSAPBridge.Configuration.Models;

namespace ApiSAPBridge.Configuration.Helpers
{
    public static class ValidationHelper
    {
        public static List<string> ValidateAppConfiguration(AppConfiguration config)
        {
            var errors = new List<string>();

            // Validar configuración SQL
            errors.AddRange(ValidateSqlConfiguration(config.SqlServer));

            // Validar configuración SAP
            errors.AddRange(ValidateSapConfiguration(config.SapAutomation));

            // Validar configuración Swagger
            errors.AddRange(ValidateSwaggerConfiguration(config.Swagger));

            return errors;
        }

        private static List<string> ValidateSqlConfiguration(SqlServerConfiguration config)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(config.Server))
                errors.Add("SQL: Servidor es requerido");

            if (string.IsNullOrWhiteSpace(config.Database))
                errors.Add("SQL: Base de datos es requerida");

            if (!config.UseWindowsAuthentication)
            {
                if (string.IsNullOrWhiteSpace(config.Username))
                    errors.Add("SQL: Usuario es requerido para autenticación SQL");
            }

            if (config.ConnectionTimeout < 5 || config.ConnectionTimeout > 300)
                errors.Add("SQL: Timeout debe estar entre 5 y 300 segundos");

            return errors;
        }

        private static List<string> ValidateSapConfiguration(SapAutomationConfiguration config)
        {
            var errors = new List<string>();

            if (config.SyncIntervalMinutes < 5)
                errors.Add("SAP: Intervalo de sincronización debe ser mínimo 5 minutos");

            if (config.SyncIntervalMinutes > 1440)
                errors.Add("SAP: Intervalo de sincronización debe ser máximo 1440 minutos (24 horas)");

            if (!config.Endpoints.Any())
                errors.Add("SAP: Debe tener al menos un endpoint configurado");

            var enabledEndpoints = config.Endpoints.Where(e => e.IsEnabled).ToList();
            if (config.EnableAutomaticSync && !enabledEndpoints.Any())
                errors.Add("SAP: Sincronización automática activada pero no hay endpoints habilitados");

            return errors;
        }

        private static List<string> ValidateSwaggerConfiguration(SwaggerConfiguration config)
        {
            var errors = new List<string>();

            if (config.EnableSwaggerUI && !config.EnableSwagger)
                errors.Add("Swagger: No se puede habilitar UI sin habilitar Swagger");

            if (!config.AllowedMethods.Any())
                errors.Add("Swagger: Debe permitir al menos un método HTTP");

            if (config.AllowedMethods.Contains("DELETE") && config.HiddenEndpoints.All(e => !e.IsHidden))
                errors.Add("Swagger: Se recomienda ocultar endpoints DELETE por seguridad");

            return errors;
        }

        public static bool IsValidConnectionString(string connectionString)
        {
            try
            {
                var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
                return !string.IsNullOrWhiteSpace(builder.DataSource) &&
                       !string.IsNullOrWhiteSpace(builder.InitialCatalog);
            }
            catch
            {
                return false;
            }
        }
    }
}