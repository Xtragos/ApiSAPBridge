// ApiSAPBridge.Data/Extensions/ServiceCollectionExtensions.cs - VERSIÓN CORREGIDA
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ApiSAPBridge.Models.Configuration;

namespace ApiSAPBridge.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiSAPBridgeData(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Obtener la cadena de conexión
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "La cadena de conexión 'DefaultConnection' no está configurada.");
            }

            // Registrar DbContext
            services.AddDbContext<ApiSAPBridgeDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);

                    sqlOptions.CommandTimeout(120); // 2 minutos
                });

                // Solo en desarrollo
                options.EnableSensitiveDataLogging(false);
                options.EnableDetailedErrors(false);
            });

            // Registrar configuraciones - MÉTODO ALTERNATIVO QUE SIEMPRE FUNCIONA
            services.Configure<DatabaseConfig>(options =>
            {
                var databaseSection = configuration.GetSection("DatabaseConfig");

                options.ServerName = databaseSection.GetValue<string>("ServerName") ?? "localhost";
                options.DatabaseName = databaseSection.GetValue<string>("DatabaseName") ?? "ApiSAP";
                options.Username = databaseSection.GetValue<string>("Username");
                options.Password = databaseSection.GetValue<string>("Password");
                options.UseWindowsAuth = databaseSection.GetValue<bool>("UseWindowsAuth", true);
                options.TrustServerCertificate = databaseSection.GetValue<bool>("TrustServerCertificate", true);
                options.MultipleActiveResultSets = databaseSection.GetValue<bool>("MultipleActiveResultSets", true);
            });

            return services;
        }

        public static IServiceCollection AddApiSAPBridgeDataWithConnectionString(
            this IServiceCollection services,
            string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException(
                    "La cadena de conexión no puede estar vacía.",
                    nameof(connectionString));
            }

            services.AddDbContext<ApiSAPBridgeDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);

                    sqlOptions.CommandTimeout(120);
                });

                options.EnableSensitiveDataLogging(false);
                options.EnableDetailedErrors(false);
            });

            return services;
        }

        // Método adicional para registrar DatabaseConfig desde código
        public static IServiceCollection AddDatabaseConfig(
            this IServiceCollection services,
            DatabaseConfig config)
        {
            services.Configure<DatabaseConfig>(options =>
            {
                options.ServerName = config.ServerName;
                options.DatabaseName = config.DatabaseName;
                options.Username = config.Username;
                options.Password = config.Password;
                options.UseWindowsAuth = config.UseWindowsAuth;
                options.TrustServerCertificate = config.TrustServerCertificate;
                options.MultipleActiveResultSets = config.MultipleActiveResultSets;
            });

            return services;
        }
    }
}