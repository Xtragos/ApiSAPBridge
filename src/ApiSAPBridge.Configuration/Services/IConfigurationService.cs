using ApiSAPBridge.Configuration.Models;

namespace ApiSAPBridge.Configuration.Services
{
    public interface IConfigurationService
    {
        Task<AppConfiguration> LoadConfigurationAsync();
        Task SaveConfigurationAsync(AppConfiguration configuration);
        Task<bool> TestSqlConnectionAsync(SqlServerConfiguration sqlConfig);
        Task<SecurityConfiguration> LoadSecurityConfigurationAsync();
        Task SaveSecurityConfigurationAsync(SecurityConfiguration securityConfig);
        string GetConfigurationFilePath();
        bool ConfigurationExists();
    }
}