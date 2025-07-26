using Microsoft.Data.SqlClient;
using ApiSAPBridge.Configuration.Models.DTOs;

namespace ApiSAPBridge.Configuration.Services
{
    public interface IDatabaseTestService
    {
        Task<ConnectionTestResult> TestConnectionAsync(string connectionString);
        Task<ConnectionTestResult> TestConnectionAsync(string server, string database, string? username = null, string? password = null, bool useIntegratedSecurity = true);
    }

    public class DatabaseTestService : IDatabaseTestService
    {
        public async Task<ConnectionTestResult> TestConnectionAsync(string connectionString)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                stopwatch.Stop();

                return new ConnectionTestResult
                {
                    IsSuccess = true,
                    Message = "Conexión exitosa",
                    ConnectionTime = stopwatch.Elapsed
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                return new ConnectionTestResult
                {
                    IsSuccess = false,
                    Message = "Error de conexión",
                    ConnectionTime = stopwatch.Elapsed,
                    ErrorDetails = ex.Message
                };
            }
        }

        public async Task<ConnectionTestResult> TestConnectionAsync(string server, string database, string? username = null, string? password = null, bool useIntegratedSecurity = true)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = database,
                ConnectTimeout = 10
            };

            if (useIntegratedSecurity)
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.UserID = username;
                builder.Password = password;
            }

            return await TestConnectionAsync(builder.ConnectionString);
        }
    }
}