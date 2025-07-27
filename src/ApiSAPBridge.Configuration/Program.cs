using ApiSAPBridge.Configuration.Services;
using ApiSAPBridge.Configuration.UI.Forms;
using ApiSAPBridge.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

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
                Log.Information("🚀 Iniciando ApiSAPBridge Configuration Manager...");

                ApplicationConfiguration.Initialize();

                var services = ConfigureServices();
                ServiceProvider = services.BuildServiceProvider();

                Log.Information("✅ Servicios configurados correctamente");

                // Ejecutar migración para agregar tablas de configuración
                await EnsureDatabaseCreatedAsync(ServiceProvider);

                Log.Information("🎮 Iniciando interfaz de usuario...");
                var mainForm = ServiceProvider.GetRequiredService<MainForm>();
                Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "💥 La aplicación falló al iniciar");
                MessageBox.Show($"Error crítico al iniciar la aplicación:\n{ex.Message}",
                               "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Log.Information("🏁 Cerrando aplicación");
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

            Log.Information("⚙️ Configurando servicios...");

            // Configuración - USAR LA MISMA QUE EL PROYECTO PRINCIPAL
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // USAR LA MISMA CONNECTION STRING DEL PROYECTO PRINCIPAL
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Server=ORION-LUIS;Database=ApiSAP;User Id=ICGAdmin;Password=masterkey;MultipleActiveResultSets=true;TrustServerCertificate=true;ConnectRetryCount=0;";

            Log.Information("🔗 Connection string configurado para: {Server}", "ORION-LUIS");

            // USAR EL CONTEXTO EXISTENTE
            services.AddDbContext<ApiSAPBridgeDbContext>(options =>
                options.UseSqlServer(connectionString));

            // ============================================
            // SERVICIOS PRINCIPALES DE CONFIGURACIÓN
            // ============================================

            // Servicios de configuración base
            services.AddScoped<IConfigurationService, ConfigurationService>();
            services.AddScoped<ISecurityService, SecurityService>();

            // ============================================
            // SERVICIOS DE BASE DE DATOS (NUEVOS Y MEJORADOS)
            // ============================================

            // 🆕 Servicio de detección automática de versión SQL Server
            services.AddTransient<ISqlServerVersionDetectorService, SqlServerVersionDetectorService>();

            // Servicio de prueba de base de datos (mejorado con detección automática)
            services.AddScoped<IDatabaseTestService, DatabaseTestService>();

            // Servicio de inicialización de base de datos
            services.AddTransient<IDatabaseInitializerService, DatabaseInitializerService>();

            // ============================================
            // FORMULARIOS DE LA APLICACIÓN
            // ============================================

            // Formulario principal
            services.AddTransient<MainForm>();

            // Formulario de login/seguridad
            services.AddTransient<LoginForm>();

            // 🆕 Formulario SQL con detección automática de versión
            services.AddTransient<SqlConfigForm>(provider => new SqlConfigForm(
                provider.GetRequiredService<ILogger<SqlConfigForm>>(),
                provider.GetRequiredService<IConfigurationService>(),
                provider.GetRequiredService<IDatabaseTestService>(),
                provider.GetRequiredService<IDatabaseInitializerService>(),
                provider.GetRequiredService<ISqlServerVersionDetectorService>() // ← Nuevo servicio
            ));

            // Otros formularios de configuración
            services.AddTransient<MethodsConfigForm>();
            services.AddTransient<SwaggerConfigForm>();

            // ============================================
            // LOGGING
            // ============================================

            services.AddLogging(builder => builder.AddSerilog());

            Log.Information("✅ Servicios registrados correctamente:");
            Log.Information("   • ConfigurationService - Gestión de configuraciones");
            Log.Information("   • SecurityService - Gestión de seguridad");
            Log.Information("   • SqlServerVersionDetectorService - 🆕 Detección automática de versión SQL");
            Log.Information("   • DatabaseTestService - Pruebas de conexión inteligentes");
            Log.Information("   • DatabaseInitializerService - Inicialización automática de BD");
            Log.Information("   • SqlConfigForm - 🆕 Con detección automática de versión");

            return services;
        }

        private static async Task EnsureDatabaseCreatedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApiSAPBridgeDbContext>();

            try
            {
                Log.Information("🗄️ Verificando estado de la base de datos...");

                // Verificar si la base de datos existe
                var canConnect = await context.Database.CanConnectAsync();

                if (!canConnect)
                {
                    Log.Warning("⚠️ No se puede conectar a la base de datos, intentando crearla...");
                }

                // Aplicar migraciones automáticamente
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

                if (pendingMigrations.Any())
                {
                    Log.Information("📦 Aplicando {Count} migraciones pendientes...", pendingMigrations.Count());
                    await context.Database.MigrateAsync();
                    Log.Information("✅ Migraciones aplicadas correctamente");
                }
                else
                {
                    Log.Information("✅ Base de datos actualizada, no hay migraciones pendientes");
                }

                // Verificar conectividad final
                var finalCheck = await context.Database.CanConnectAsync();
                if (finalCheck)
                {
                    Log.Information("✅ Conexión a base de datos verificada exitosamente");
                }
                else
                {
                    Log.Warning("⚠️ Problemas de conectividad con la base de datos");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "❌ Error al configurar la base de datos");

                // No lanzar excepción aquí para permitir que la aplicación inicie
                // El usuario podrá usar el formulario SQL para crear/configurar la BD
                Log.Warning("⚠️ La aplicación continuará, pero será necesario configurar la base de datos manualmente");
            }
        }
    }
}