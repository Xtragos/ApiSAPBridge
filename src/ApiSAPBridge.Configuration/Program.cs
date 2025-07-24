using ApiSAPBridge.Configuration.Forms;
using ApiSAPBridge.Configuration.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ApiSAPBridge.Configuration
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Configurar Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs/configuration-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                // Configurar servicios
                var services = new ServiceCollection();
                ConfigureServices(services);
                var serviceProvider = services.BuildServiceProvider();

                // Configurar aplicación - CORREGIDO
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Iniciar aplicación
                var mainForm = serviceProvider.GetRequiredService<MainForm>();
                Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Error fatal al iniciar la aplicación");
                MessageBox.Show($"Error al iniciar la aplicación: {ex.Message}",
                    "Error Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            // Registrar servicios
            services.AddSingleton<IConfigurationService, ConfigurationService>();

            // Registrar formularios
            services.AddTransient<MainForm>();
        }
    }
}