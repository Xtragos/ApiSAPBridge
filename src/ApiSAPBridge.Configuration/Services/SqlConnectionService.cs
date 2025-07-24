using ApiSAPBridge.Configuration.Models;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace ApiSAPBridge.Configuration.Services
{
    public class SqlConnectionService
    {
        private readonly ILogger<SqlConnectionService> _logger;

        public SqlConnectionService(ILogger<SqlConnectionService> logger)
        {
            _logger = logger;
        }

        public async Task<(bool Success, string Message)> TestConnectionAsync(SqlConfiguration config)
        {
            try
            {
                using var connection = new SqlConnection(config.GetConnectionString());
                await connection.OpenAsync();

                _logger.LogInformation("Conexión SQL exitosa a {Server}/{Database}", config.Server, config.Database);
                return (true, "Conexión exitosa");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al conectar con SQL Server");
                return (false, $"Error: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> TestDatabaseExistsAsync(SqlConfiguration config)
        {
            try
            {
                var masterConfig = new SqlConfiguration
                {
                    Server = config.Server,
                    Database = "master",
                    Username = config.Username,
                    Password = config.Password,
                    UseWindowsAuthentication = config.UseWindowsAuthentication,
                    ConnectionTimeout = config.ConnectionTimeout
                };

                using var connection = new SqlConnection(masterConfig.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand(
                    "SELECT COUNT(*) FROM sys.databases WHERE name = @dbName", connection);
                command.Parameters.AddWithValue("@dbName", config.Database);

                var result = await command.ExecuteScalarAsync();
                var exists = Convert.ToInt32(result) > 0;

                if (exists)
                {
                    _logger.LogInformation("Base de datos {Database} existe", config.Database);
                    return (true, $"Base de datos '{config.Database}' encontrada");
                }
                else
                {
                    _logger.LogWarning("Base de datos {Database} no existe", config.Database);
                    return (false, $"Base de datos '{config.Database}' no existe");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar base de datos");
                return (false, $"Error: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> CreateDatabaseAsync(SqlConfiguration config)
        {
            try
            {
                var masterConfig = new SqlConfiguration
                {
                    Server = config.Server,
                    Database = "master",
                    Username = config.Username,
                    Password = config.Password,
                    UseWindowsAuthentication = config.UseWindowsAuthentication,
                    ConnectionTimeout = config.ConnectionTimeout
                };

                using var connection = new SqlConnection(masterConfig.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand($"CREATE DATABASE [{config.Database}]", connection);
                await command.ExecuteNonQueryAsync();

                _logger.LogInformation("Base de datos {Database} creada exitosamente", config.Database);
                return (true, $"Base de datos '{config.Database}' creada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear base de datos");
                return (false, $"Error: {ex.Message}");
            }
        }

        public async Task<List<string>> GetAvailableServersAsync()
        {
            try
            {
                var servers = new List<string>();

                // Agregar servidores comunes
                servers.Add("localhost");
                servers.Add("(local)");
                servers.Add(".");
                servers.Add($"{Environment.MachineName}\\SQLEXPRESS");
                servers.Add($"{Environment.MachineName}\\MSSQLSERVER");

                // Intentar enumerar servidores de red (puede tomar tiempo)
                await Task.Run(() =>
                {
                    try
                    {
                        var dataSources = SqlDataSourceEnumerator.Instance.GetDataSources();
                        foreach (System.Data.DataRow row in dataSources.Rows)
                        {
                            var serverName = row["ServerName"]?.ToString() ?? "";
                            var instanceName = row["InstanceName"]?.ToString() ?? "";

                            if (!string.IsNullOrEmpty(serverName))
                            {
                                var fullName = string.IsNullOrEmpty(instanceName) ?
                                    serverName : $"{serverName}\\{instanceName}";

                                if (!servers.Contains(fullName))
                                {
                                    servers.Add(fullName);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning("No se pudo enumerar servidores SQL: {Message}", ex.Message);
                    }
                });

                return servers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener servidores disponibles");
                return new List<string> { "localhost", "(local)", "." };
            }
        }

        public async Task<List<string>> GetDatabasesAsync(SqlConfiguration config)
        {
            try
            {
                var masterConfig = new SqlConfiguration
                {
                    Server = config.Server,
                    Database = "master",
                    Username = config.Username,
                    Password = config.Password,
                    UseWindowsAuthentication = config.UseWindowsAuthentication,
                    ConnectionTimeout = config.ConnectionTimeout
                };

                using var connection = new SqlConnection(masterConfig.GetConnectionString());
                await connection.OpenAsync();

                using var command = new SqlCommand(
                    "SELECT name FROM sys.databases WHERE database_id > 4 ORDER BY name", connection);

                var databases = new List<string>();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    databases.Add(reader.GetString(0));
                }

                return databases;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener bases de datos");
                return new List<string>();
            }
        }
    }
}