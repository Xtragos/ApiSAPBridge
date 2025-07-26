using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ApiSAPBridge.Configuration.Data;
using ApiSAPBridge.Configuration.Services;
using ApiSAPBridge.Configuration.UI.Forms;

namespace ApiSAPBridge.Configuration
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main()
        {
            // Configurar Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File("logs/configuration-.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                ApplicationConfiguration.Initialize();

                // Configurar servicios
                var services = ConfigureServices();
                ServiceProvider = services.BuildServiceProvider();

                // Ejecutar migraciones
                await EnsureDatabaseCreatedAsync(ServiceProvider);

                // Iniciar aplicación
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

            // Configuración
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // Base de datos
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ApiSAPBridgeConfig;Integrated Security=True;";

            services.AddDbContext<ConfigurationContext>(options =>
                options.UseSqlServer(connectionString));

            // Servicios
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
            var context = scope.ServiceProvider.GetRequiredService<ConfigurationContext>();

            try
            {
                await context.Database.MigrateAsync();
                Log.Information("Base de datos configurada correctamente");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error al configurar la base de datos");
                throw;
            }
        }
    }
}