namespace ApiSAPBridge.Models.Configuration
{
    public class DatabaseConfig
    {
        public string ServerName { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool UseWindowsAuth { get; set; } = true;
        public bool TrustServerCertificate { get; set; } = true;
        public bool MultipleActiveResultSets { get; set; } = true;

        public string GetConnectionString()
        {
            if (UseWindowsAuth)
            {
                return $"Server={ServerName};Database={DatabaseName};" +
                       $"User Id={Username};Password={Password};" +
                       $"MultipleActiveResultSets={MultipleActiveResultSets};" +
                       $"TrustServerCertificate={TrustServerCertificate};";
            }
            else
            {
                return $"Server={ServerName};Database={DatabaseName};" +
                       $"User Id={Username};Password={Password};" +
                       $"MultipleActiveResultSets={MultipleActiveResultSets};" +
                       $"TrustServerCertificate={TrustServerCertificate};";
            }
        }
    }

    public class ApiKeyConfig
    {
        public string SAPApiKey { get; set; } = string.Empty;
        public string AuthToken { get; set; } = string.Empty;
    }
}