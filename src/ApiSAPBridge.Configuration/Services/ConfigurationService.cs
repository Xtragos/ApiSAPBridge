
using ApiSAPBridge.Configuration.Models.DTOs;
using ApiSAPBridge.Data;
using ApiSAPBridge.Models.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ApiSAPBridge.Configuration.Services
{
    public interface IConfigurationService
    {
        Task<SqlConfiguration?> GetSqlConfigurationAsync();
        Task<OperationResult> SaveSqlConfigurationAsync(SqlConfiguration config);
        Task<List<MethodConfiguration>> GetMethodConfigurationsAsync();
        Task<OperationResult> SaveMethodConfigurationAsync(MethodConfiguration config);
        Task<List<SwaggerConfiguration>> GetSwaggerConfigurationsAsync();
        Task<OperationResult> SaveSwaggerConfigurationAsync(SwaggerConfiguration config);
        Task<string?> GetSystemConfigurationAsync(string key);
        Task<OperationResult> SaveSystemConfigurationAsync(string key, string value);
    }

    public class ConfigurationService : IConfigurationService
    {
        private readonly ApiSAPBridgeDbContext _context;

        public ConfigurationService(ApiSAPBridgeDbContext context)
        {
            _context = context;
        }

        public async Task<SqlConfiguration?> GetSqlConfigurationAsync()
        {
            try
            {
                return await _context.SqlConfigurations.FirstOrDefaultAsync();
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 208)
            {
                // Tabla no existe aún, retornar null
                return null;
            }
        }

        public async Task<OperationResult> SaveSqlConfigurationAsync(SqlConfiguration config)
        {
            try
            {
                var existing = await _context.SqlConfigurations.FirstOrDefaultAsync();

                if (existing != null)
                {
                    existing.Server = config.Server;
                    existing.Database = config.Database;
                    existing.Username = config.Username;
                    existing.Password = config.Password;
                    existing.UseIntegratedSecurity = config.UseIntegratedSecurity;
                    existing.ConnectionTimeout = config.ConnectionTimeout;
                    existing.UpdatedAt = DateTime.UtcNow;

                    _context.SqlConfigurations.Update(existing);
                }
                else
                {
                    _context.SqlConfigurations.Add(config);
                }

                await _context.SaveChangesAsync();
                return OperationResult.Success("Configuración SQL guardada correctamente");
            }
            catch (Exception ex)
            {
                return OperationResult.Failure("Error al guardar configuración SQL", ex.Message);
            }
        }

        public async Task<List<MethodConfiguration>> GetMethodConfigurationsAsync()
        {
            return await _context.MethodConfigurations.ToListAsync();
        }

        public async Task<OperationResult> SaveMethodConfigurationAsync(MethodConfiguration config)
        {
            try
            {
                var existing = await _context.MethodConfigurations
                    .FirstOrDefaultAsync(m => m.Id == config.Id);

                if (existing != null)
                {
                    existing.IsEnabled = config.IsEnabled;
                    existing.IsAutomaticSync = config.IsAutomaticSync;
                    existing.SyncIntervalMinutes = config.SyncIntervalMinutes;
                    existing.UpdatedAt = DateTime.UtcNow;

                    _context.MethodConfigurations.Update(existing);
                }
                else
                {
                    _context.MethodConfigurations.Add(config);
                }

                await _context.SaveChangesAsync();
                return OperationResult.Success("Configuración de método guardada correctamente");
            }
            catch (Exception ex)
            {
                return OperationResult.Failure("Error al guardar configuración de método", ex.Message);
            }
        }

        public async Task<List<SwaggerConfiguration>> GetSwaggerConfigurationsAsync()
        {
            return await _context.SwaggerConfigurations.ToListAsync();
        }

        public async Task<OperationResult> SaveSwaggerConfigurationAsync(SwaggerConfiguration config)
        {
            try
            {
                var existing = await _context.SwaggerConfigurations
                    .FirstOrDefaultAsync(s => s.Id == config.Id);

                if (existing != null)
                {
                    existing.IsVisible = config.IsVisible;
                    existing.UpdatedAt = DateTime.UtcNow;

                    _context.SwaggerConfigurations.Update(existing);
                }
                else
                {
                    _context.SwaggerConfigurations.Add(config);
                }

                await _context.SaveChangesAsync();
                return OperationResult.Success("Configuración Swagger guardada correctamente");
            }
            catch (Exception ex)
            {
                return OperationResult.Failure("Error al guardar configuración Swagger", ex.Message);
            }
        }

        public async Task<string?> GetSystemConfigurationAsync(string key)
        {
            var config = await _context.SystemConfigurations
                .FirstOrDefaultAsync(s => s.Key == key);
            return config?.Value;
        }

        public async Task<OperationResult> SaveSystemConfigurationAsync(string key, string value)
        {
            try
            {
                var existing = await _context.SystemConfigurations
                    .FirstOrDefaultAsync(s => s.Key == key);

                if (existing != null)
                {
                    existing.Value = value;
                    existing.UpdatedAt = DateTime.UtcNow;
                    _context.SystemConfigurations.Update(existing);
                }
                else
                {
                    _context.SystemConfigurations.Add(new SystemConfiguration
                    {
                        Key = key,
                        Value = value
                    });
                }

                await _context.SaveChangesAsync();
                return OperationResult.Success("Configuración del sistema guardada correctamente");
            }
            catch (Exception ex)
            {
                return OperationResult.Failure("Error al guardar configuración del sistema", ex.Message);
            }
        }
    }
}
