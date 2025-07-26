using System.Security.Cryptography;
using System.Text;

namespace ApiSAPBridge.Configuration.Utils
{
    public static class CryptoHelper
    {
        private static readonly byte[] Salt = Encoding.UTF8.GetBytes("ApiSAPBridge2024!");

        /// <summary>
        /// Encripta una cadena usando AES
        /// </summary>
        public static string Encrypt(string plainText, string password)
        {
            using var aes = Aes.Create();
            var key = new Rfc2898DeriveBytes(password, Salt, 10000, HashAlgorithmName.SHA256).GetBytes(32);
            aes.Key = key;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            using var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }

            var iv = aes.IV;
            var encrypted = msEncrypt.ToArray();
            var result = new byte[iv.Length + encrypted.Length];

            Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
            Buffer.BlockCopy(encrypted, 0, result, iv.Length, encrypted.Length);

            return Convert.ToBase64String(result);
        }

        /// <summary>
        /// Desencripta una cadena usando AES
        /// </summary>
        public static string Decrypt(string cipherText, string password)
        {
            var buffer = Convert.FromBase64String(cipherText);
            using var aes = Aes.Create();

            var key = new Rfc2898DeriveBytes(password, Salt, 10000, HashAlgorithmName.SHA256).GetBytes(32);
            aes.Key = key;

            var iv = new byte[16];
            var encrypted = new byte[buffer.Length - 16];

            Buffer.BlockCopy(buffer, 0, iv, 0, 16);
            Buffer.BlockCopy(buffer, 16, encrypted, 0, encrypted.Length);

            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            using var msDecrypt = new MemoryStream(encrypted);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }
    }
}