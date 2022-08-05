using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MEI.SPDocuments
{
    public interface IEncryptor
    {
        string GenerateKey();

        string GenerateIv();

        string Encrypt(string data);

        string Decrypt(string data);

        string Encrypt(string data, string key, string iv);

        string Encrypt(string data, byte[] key, byte[] iv);

        string Decrypt(string data, string key, string iv);

        string Decrypt(string data, byte[] key, byte[] iv);
    }

    public class TripleDESEncryptor
        : IEncryptor
    {
        private const string defaultKey = "fcJrwYHTIvbgTdcEVxkW6PO3TnyfQSPT";
        private const string defaultIv = "GPDDvfCnKDc=";

        private readonly TripleDESCryptoServiceProvider _encryptor;

        private byte[] _decodedDefaultIv;
        private byte[] _decodedDefaultKey;

        public TripleDESEncryptor()
        {
            _encryptor = new TripleDESCryptoServiceProvider
                    {
                        Mode = CipherMode.CBC
                    };

            _decodedDefaultIv = Convert.FromBase64String(defaultIv);
            _decodedDefaultKey = Convert.FromBase64String(defaultKey);
        }

        public string GenerateKey()
        {
            // Length is 24
            _encryptor.GenerateKey();

            return Convert.ToBase64String(_encryptor.Key);
        }

        public string GenerateIv()
        {
            // Length is 8
            _encryptor.GenerateIV();

            return Convert.ToBase64String(_encryptor.IV);
        }

        public string Encrypt(string data)
        {
            return Encrypt(data, _decodedDefaultKey, _decodedDefaultIv);
        }

        public string Encrypt(string data, string key, string iv)
        {
            byte[] decodedKey = Convert.FromBase64String(key);
            byte[] decodedIv = Convert.FromBase64String(iv);

            return Encrypt(data, decodedKey, decodedIv);
        }

        public string Encrypt(string data, byte[] key, byte[] iv)
        {
            byte[] encodedData = Encoding.UTF8.GetBytes(data);

            using (var stream = new MemoryStream())
            {
                using (var encStream = new CryptoStream(stream, _encryptor.CreateEncryptor(key, iv), CryptoStreamMode.Write))
                {
                    encStream.Write(encodedData, 0, encodedData.Length);
                    encStream.FlushFinalBlock();
                }

                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public string Decrypt(string data)
        {
            return Decrypt(data, defaultKey, defaultIv);
        }

        public string Decrypt(string data, string key, string iv)
        {
            byte[] decodedKey = Convert.FromBase64String(key);
            byte[] decodedIv = Convert.FromBase64String(iv);

            return Decrypt(data, decodedKey, decodedIv);
        }

        public string Decrypt(string data, byte[] key, byte[] iv)
        {
            byte[] decodedData = Convert.FromBase64String(data);

            using (var stream = new MemoryStream())
            {
                using (var encStream = new CryptoStream(stream, _encryptor.CreateDecryptor(key, iv), CryptoStreamMode.Write))
                {
                    encStream.Write(decodedData, 0, decodedData.Length);
                    encStream.FlushFinalBlock();
                }

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}
