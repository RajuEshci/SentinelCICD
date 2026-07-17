using Microsoft.Extensions.Configuration;
using SentinelApp.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace SentinelApp.Infrastructure
{
    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _cryptKey;
        private readonly byte[] _initVector;

        public EncryptionService(IConfiguration configuration)
        {
            var cryptKey = configuration["EncryptionSettings:CryptKey"];
            var initVector = configuration["EncryptionSettings:InitVector"];

            if (string.IsNullOrWhiteSpace(cryptKey))
                throw new ArgumentNullException(nameof(cryptKey));

            if (string.IsNullOrWhiteSpace(initVector))
                throw new ArgumentNullException(nameof(initVector));

            _cryptKey = Encoding.ASCII.GetBytes(cryptKey);
            _initVector = Encoding.ASCII.GetBytes(initVector);
        }

        public string Encrypt(string plainText)
        {
            using var rijndael = new RijndaelManaged
            {
                Key = _cryptKey,
                IV = _initVector,
                Mode = CipherMode.CBC
            };

            using var memoryStream = new MemoryStream();

            using (var cryptoStream = new CryptoStream(
                memoryStream,
                rijndael.CreateEncryptor(_cryptKey, _initVector),
                CryptoStreamMode.Write))
            using (var writer = new StreamWriter(cryptoStream))
            {
                writer.Write(plainText);
            }

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            using var rijndael = new RijndaelManaged
            {
                Key = _cryptKey,
                IV = _initVector,
                Mode = CipherMode.CBC
            };

            using var memoryStream = new MemoryStream(Convert.FromBase64String(cipherText));

            using var cryptoStream = new CryptoStream(
                memoryStream,
                rijndael.CreateDecryptor(_cryptKey, _initVector),
                CryptoStreamMode.Read);

            using var reader = new StreamReader(cryptoStream);

            return reader.ReadToEnd();
        }

        public byte[] EncryptStringToBytes_Aes(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));

            using Aes aes = Aes.Create();

            aes.Key = _cryptKey;
            aes.IV = _initVector;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using MemoryStream msEncrypt = new MemoryStream();

            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }

            return msEncrypt.ToArray();
        }

        public string DecryptStringFromBytes_Aes(byte[] cipherText)
        {
            if (cipherText == null || cipherText.Length == 0)
                throw new ArgumentNullException(nameof(cipherText));

            using Aes aes = Aes.Create();

            aes.Key = _cryptKey;
            aes.IV = _initVector;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using MemoryStream msDecrypt = new MemoryStream(cipherText);

            using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

            using StreamReader srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }

        public string ComputeSha256Hash(string rawData)
        {
            using SHA256 sha256 = SHA256.Create();

            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            StringBuilder builder = new StringBuilder();

            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}