using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ApiSAPBridge.Data;
using ApiSAPBridge.Configuration.Services;
using ApiSAPBridge.Configuration.UI.Forms;

namespace ApiSAPBridge.Configuration
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        [STAThread]
        static async Task Main()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File("logs/configuration-.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                ApplicationConfiguration.Initialize();

                var services = ConfigureServices();
                ServiceProvider = services.BuildServiceProvider();

                // Ejecutar migración para agregar tablas de configuración
                await EnsureDatabaseCreatedAsync(ServiceProvider);

                var mainForm = ServiceProvider.GetRequiredService<MainForm>();
                Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "La aplicación falló al iniciar");
                MessageBox.Show($"Error crítico al iniciar la aplicación:\n{ex.Message}",
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Log.CloseAndFlush();
                if (ServiceProvider is IDisposable disposableProvider)
                {
                    disposableProvider.Dispose();
                }
            }
        }

        private static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();

            // Configuración - USAR LA MISMA QUE EL PROYECTO PRINCIPAL
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // USAR LA MISMA CONNECTION STRING DEL PROYECTO PRINCIPAL
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Server=ORION-LUIS;Database=ApiSAP;User Id=ICGAdmin;Password=masterkey;MultipleActiveResultSets=true;TrustServerCertificate=true;ConnectRetryCount=0;";

            // USAR EL CONTEXTO EXISTENTE
            services.AddDbContext<ApiSAPBridgeDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Servicios actualizados
            services.AddScoped<IConfigurationService, ConfigurationService>();
            services.AddScoped<IDatabaseTestService, DatabaseTestService>();
            services.AddScoped<ISecurityService, SecurityService>();

            // Formularios
            services.AddTransient<MainForm>();
            services.AddTransient<SqlConfigForm>();
            services.AddTransient<MethodsConfigForm>();
            services.AddTransient<SwaggerConfigForm>();
            services.AddTransient<LoginForm>();

            // Logging
            services.AddLogging(builder => builder.AddSerilog());

            return services;
        }

        private static async Task EnsureDatabaseCreatedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApiSAPBridgeDbContext>();

            try
            {
                // Aplicar migraciones automáticamente
                await context.Database.MigrateAsync();
                Log.Information("Migraciones aplicadas correctamente");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al aplicar migraciones");
                throw;
            }
        }
    }
}
