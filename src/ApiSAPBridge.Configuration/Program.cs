using ApiSAPBridge.Configuration.Forms;
using ApiSAPBridge.Configuration.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.Logging;
using Serilog;
using System.Windows.Forms.Design;

namespace ApiSAPBridge.Configuration
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Configurar logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File("logs/configuration-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                // Configurar servicios
                var services = new ServiceCollection();
                ConfigureServices(services);

                var serviceProvider = services.BuildServiceProvider();

                // Configurar aplicación Windows Forms
                ApplicationConfiguration.Initialize();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Ejecutar aplicación principal
                var mainForm = serviceProvider.GetRequiredService<MainForm>();
                Application.Run(mainForm);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Error fatal al iniciar la aplicación de configuración");
                MessageBox.Show($"Error fatal: {ex.Message}", "ApiSAPBridge Configuration",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            // Logging
            services.AddLogging(builder => builder.AddSerilog());

            // Servicios de configuración
            services.AddSingleton<ConfigurationService>();
            services.AddSingleton<SqlConnectionService>();
            services.AddSingleton<ApiService>();

            // Forms
            services.AddTransient<MainForm>();
            services.AddTransient<LoginForm>();
        }
    }
}