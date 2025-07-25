using ApiSAPBridge.Configuration.Models;
using System.Security.Cryptography;
using System.Text;

namespace ApiSAPBridge.Configuration.Helpers
{
    public static class SecurityHelper
    {
        private static readonly Dictionary<string, DateTime> _loginAttempts = new();
        private static readonly Dictionary<string, int> _attemptCounts = new();

        /// <summary>
        /// Hashea una contraseña usando SHA256
        /// </summary>
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        /// <summary>
        /// Verifica si una contraseña coincide con el hash
        /// </summary>
        public static bool VerifyPassword(string password, string hash)
        {
            var passwordHash = HashPassword(password);
            return passwordHash.Equals(hash, StringComparison.Ordinal);
        }

        /// <summary>
        /// Registra un intento de login fallido
        /// </summary>
        public static void RegisterFailedAttempt(string identifier = "default")
        {
            _loginAttempts[identifier] = DateTime.Now;
            _attemptCounts[identifier] = _attemptCounts.GetValueOrDefault(identifier, 0) + 1;
        }

        /// <summary>
        /// Verifica si un usuario está bloqueado
        /// </summary>
        public static bool IsLockedOut(string identifier = "default", int maxAttempts = 3, int lockoutMinutes = 15)
        {
            if (!_attemptCounts.ContainsKey(identifier) || !_loginAttempts.ContainsKey(identifier))
                return false;

            var attemptCount = _attemptCounts[identifier];
            var lastAttempt = _loginAttempts[identifier];

            if (attemptCount >= maxAttempts)
            {
                var lockoutExpiry = lastAttempt.AddMinutes(lockoutMinutes);
                if (DateTime.Now < lockoutExpiry)
                    return true;

                // Reset después de que expire el bloqueo
                _attemptCounts.Remove(identifier);
                _loginAttempts.Remove(identifier);
            }

            return false;
        }

        /// <summary>
        /// Limpia los intentos después de un login exitoso
        /// </summary>
        public static void ClearAttempts(string identifier = "default")
        {
            _attemptCounts.Remove(identifier);
            _loginAttempts.Remove(identifier);
        }

        /// <summary>
        /// Obtiene el tiempo restante de bloqueo
        /// </summary>
        public static TimeSpan GetLockoutTimeRemaining(string identifier = "default", int lockoutMinutes = 15)
        {
            if (!_loginAttempts.ContainsKey(identifier))
                return TimeSpan.Zero;

            var lastAttempt = _loginAttempts[identifier];
            var lockoutExpiry = lastAttempt.AddMinutes(lockoutMinutes);
            var remaining = lockoutExpiry - DateTime.Now;

            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }
        /// <summary>
        /// Valida que la configuración sea segura
        /// </summary>
        public static List<string> ValidateSecurityConfiguration(SecurityConfiguration config)
        {
            var warnings = new List<string>();

            if (config.ConfigurationPassword == "admin123")
            {
                warnings.Add("⚠️ Se recomienda cambiar la contraseña por defecto");
            }

            if (config.ConfigurationPassword.Length < 6)
            {
                warnings.Add("⚠️ La contraseña debe tener al menos 6 caracteres");
            }

            if (config.MaxLoginAttempts > 5)
            {
                warnings.Add("⚠️ Se recomienda un máximo de 5 intentos de login");
            }

            if (config.LockoutMinutes < 5)
            {
                warnings.Add("⚠️ Se recomienda un bloqueo mínimo de 5 minutos");
            }

            return warnings;
        }

        /// <summary>
        /// Genera una contraseña segura aleatoria
        /// </summary>
        public static string GenerateSecurePassword(int length = 12)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}