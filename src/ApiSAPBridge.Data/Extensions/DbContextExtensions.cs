using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApiSAPBridge.Data.Extensions
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// Asegura que la base de datos esté creada y aplica migraciones pendientes
        /// </summary>
        public static async Task EnsureDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApiSAPBridgeDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApiSAPBridgeDbContext>>();

            try
            {
                logger.LogInformation("Verificando estado de la base de datos...");

                // Verificar si hay migraciones pendientes
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

                if (pendingMigrations.Any())
                {
                    logger.LogInformation(
                        "Aplicando {Count} migraciones pendientes: {Migrations}",
                        pendingMigrations.Count(),
                        string.Join(", ", pendingMigrations));

                    await context.Database.MigrateAsync();
                    logger.LogInformation("Migraciones aplicadas exitosamente.");
                }
                else
                {
                    logger.LogInformation("La base de datos está actualizada.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al verificar o aplicar migraciones de la base de datos.");
                throw;
            }
        }

        /// <summary>
        /// Verifica la conectividad con la base de datos
        /// </summary>
        public static async Task<bool> TestConnectionAsync(this ApiSAPBridgeDbContext context)
        {
            try
            {
                return await context.Database.CanConnectAsync();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene información sobre el estado de la base de datos
        /// </summary>
        public static async Task<DatabaseInfo> GetDatabaseInfoAsync(this ApiSAPBridgeDbContext context)
        {
            var info = new DatabaseInfo();

            try
            {
                info.CanConnect = await context.Database.CanConnectAsync();

                if (info.CanConnect)
                {
                    info.DatabaseExists = true;
                    info.PendingMigrations = await context.Database.GetPendingMigrationsAsync();
                    info.AppliedMigrations = await context.Database.GetAppliedMigrationsAsync();
                }
            }
            catch (Exception ex)
            {
                info.Error = ex.Message;
            }

            return info;
        }
    }

    public class DatabaseInfo
    {
        public bool CanConnect { get; set; }
        public bool DatabaseExists { get; set; }
        public IEnumerable<string> PendingMigrations { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> AppliedMigrations { get; set; } = Enumerable.Empty<string>();
        public string? Error { get; set; }
    }
}