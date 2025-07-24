using System.Security.Cryptography;
using System.Text;

namespace ApiSAPBridge.ConfigTool.Utils
{
    public class EncryptionHelper
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public EncryptionHelper()
        {
            // En producción, estas claves deberían generarse de forma más segura
            _key = SHA256.HashData(Encoding.UTF8.GetBytes("APISAP_CONFIG_ENCRYPTION_KEY_2024"));
            _iv = MD5.HashData(Encoding.UTF8.GetBytes("APISAP_CONFIG_IV"));
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;

            try
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = _key;
                    aes.IV = _iv;

                    using (var encryptor = aes.CreateEncryptor())
                    using (var msEncrypt = new MemoryStream())
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
            catch
            {
                return plainText; // Si falla el cifrado, devolver texto plano
            }
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return string.Empty;

            try
            {
                var cipherBytes = Convert.FromBase64String(cipherText);

                using (var aes = Aes.Create())
                {
                    aes.Key = _key;
                    aes.IV = _iv;

                    using (var decryptor = aes.CreateDecryptor())
                    using (var msDecrypt = new MemoryStream(cipherBytes))
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
            catch
            {
                return cipherText; // Si falla el descifrado, asumir que es texto plano
            }
        }
    }
}