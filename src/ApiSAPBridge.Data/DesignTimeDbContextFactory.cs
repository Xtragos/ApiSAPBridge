// ApiSAPBridge.Data/DesignTimeDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ApiSAPBridge.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApiSAPBridgeDbContext>
    {
        public ApiSAPBridgeDbContext CreateDbContext(string[] args)
        {
            // Configuración para design-time (migraciones)
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApiSAPBridgeDbContext>();

            // Obtener connection string del appsettings.json
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                // Connection string por defecto si no se encuentra en appsettings.json
                connectionString = "Server=ORION-LUIS;Database=ApiSAP;User Id=ICGAdmin;Password=masterkey;MultipleActiveResultSets=true;TrustServerCertificate=true;ConnectRetryCount=0;";
            }

            Console.WriteLine($"[DesignTime] Using connection string: {MaskPassword(connectionString)}");

            optionsBuilder.UseSqlServer(connectionString, options =>
            {
                options.MigrationsAssembly("ApiSAPBridge.Data");
                options.CommandTimeout(120);
            });

            // Configurar para design-time
            optionsBuilder.EnableSensitiveDataLogging(false);
            optionsBuilder.EnableServiceProviderCaching(false);

            return new ApiSAPBridgeDbContext(optionsBuilder.Options);
        }

        private static string MaskPassword(string connectionString)
        {
            // Ocultar password en logs
            return System.Text.RegularExpressions.Regex.Replace(
                connectionString,
                @"(Password|Pwd)=([^;]*)",
                "$1=***",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        }
    }
}