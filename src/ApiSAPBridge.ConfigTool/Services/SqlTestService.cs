using System.Data.SqlClient;
using ApiSAPBridge.ConfigTool.Models;

namespace ApiSAPBridge.ConfigTool.Services
{
    public class SqlTestService
    {
        public async Task<SqlTestResult> TestConnectionAsync(SqlConnectionConfig config)
        {
            try
            {
                var connectionString = config.GetConnectionString();

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Probar consulta básica
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT @@VERSION as Version, DB_NAME() as DatabaseName, USER_NAME() as Username, GETDATE() as ServerTime";
                        command.CommandTimeout = config.ConnectionTimeout;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var version = reader["Version"].ToString();
                                var dbName = reader["DatabaseName"].ToString();
                                var username = reader["Username"].ToString();
                                var serverTime = reader["ServerTime"].ToString();

                                return new SqlTestResult
                                {
                                    Success = true,
                                    Details = $"Servidor: {config.Server}:{config.Port}\n" +
                                             $"Base de Datos: {dbName}\n" +
                                             $"Usuario: {username}\n" +
                                             $"Hora del Servidor: {serverTime}\n" +
                                             $"Versión SQL Server: {version?.Split('\n')[0]}"
                                };
                            }
                        }
                    }
                }

                return new SqlTestResult { Success = false, ErrorMessage = "No se pudo obtener información del servidor" };
            }
            catch (Exception ex)
            {
                return new SqlTestResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    Details = $"Tipo de Error: {ex.GetType().Name}\nDetalles técnicos: {ex.Message}"
                };
            }
        }
    }

    public class SqlTestResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = "";
        public string Details { get; set; } = "";
    }
}