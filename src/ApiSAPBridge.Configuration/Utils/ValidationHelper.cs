using System.Text.RegularExpressions;

namespace ApiSAPBridge.Configuration.Utils
{
    public static class ValidationHelper
    {
        /// <summary>
        /// Valida formato de servidor SQL
        /// </summary>
        public static bool IsValidSqlServer(string server)
        {
            if (string.IsNullOrWhiteSpace(server))
                return false;

            // Permite localhost, IP, servidor\instancia, puerto
            var pattern = @"^(localhost|(\d{1,3}\.){3}\d{1,3}|[\w\-\.]+)(\\[\w\-]+)?(\,\d+)?$";
            return Regex.IsMatch(server, pattern, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Valida nombre de base de datos
        /// </summary>
        public static bool IsValidDatabaseName(string database)
        {
            if (string.IsNullOrWhiteSpace(database))
                return false;

            // Nombres válidos de SQL Server
            var pattern = @"^[a-zA-Z_@#$][a-zA-Z0-9_@#$]*$";
            return Regex.IsMatch(database, pattern) && database.Length <= 128;
        }

        /// <summary>
        /// Valida nombre de usuario SQL
        /// </summary>
        public static bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return true; // Puede ser null para autenticación Windows

            return username.Length <= 128 && !string.IsNullOrWhiteSpace(username);
        }

        /// <summary>
        /// Valida fortaleza de contraseña
        /// </summary>
        public static (bool IsValid, string Message) ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return (false, "La contraseña es requerida");

            if (password.Length < 6)
                return (false, "La contraseña debe tener al menos 6 caracteres");

            if (password.Length > 50)
                return (false, "La contraseña no puede exceder 50 caracteres");

            return (true, "Contraseña válida");
        }

        /// <summary>
        /// Valida intervalo de sincronización
        /// </summary>
        public static bool IsValidSyncInterval(int minutes)
        {
            return minutes >= 5 && minutes <= 1440; // Entre 5 minutos y 24 horas
        }
    }
}
