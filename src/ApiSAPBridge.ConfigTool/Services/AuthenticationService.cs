using ApiSAPBridge.ConfigTool.Utils;
using System.Security.Cryptography;
using System.Text;

namespace ApiSAPBridge.ConfigTool.Services
{
    public class AuthenticationService
    {
        private readonly Dictionary<string, string> _sectionPasswords;
        private readonly EncryptionHelper _encryption;

        public AuthenticationService()
        {
            _encryption = new EncryptionHelper();
            _sectionPasswords = LoadSectionPasswords();
        }

        public bool ValidatePassword(string section, string password)
        {
            if (!_sectionPasswords.ContainsKey(section.ToLower()))
            {
                return false;
            }

            var expectedHash = _sectionPasswords[section.ToLower()];
            var passwordHash = HashPassword(password);

            return passwordHash == expectedHash;
        }

        private Dictionary<string, string> LoadSectionPasswords()
        {
            // Por defecto, usar contraseñas predefinidas
            // En producción, estas deberían cargarse de un archivo seguro
            return new Dictionary<string, string>
            {
                ["métodos"] = HashPassword("Admin2024!"),
                ["swagger"] = HashPassword("Config2024!")
            };
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "APISAP_SALT"));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public void ChangePassword(string section, string newPassword)
        {
            _sectionPasswords[section.ToLower()] = HashPassword(newPassword);
            SaveSectionPasswords();
        }

        private void SaveSectionPasswords()
        {
            // Implementar guardado seguro de contraseñas
            // Por ahora, mantener en memoria
        }
    }
}