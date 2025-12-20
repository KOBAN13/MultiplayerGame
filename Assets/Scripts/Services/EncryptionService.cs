using System;
using System.IO;
using System.Security.Cryptography;
using Services.Interface;
using System.Text;

namespace Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public EncryptionService(string key)
        {
            _key = Encoding.UTF8.GetBytes(key.PadRight(32)[..32]);
            _iv = Encoding.UTF8.GetBytes("1234567890123456");
        }
        
        public string Encrypt(string plainText)
        {
            var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using var sw = new StreamWriter(cs);
            
            sw.Write(plainText);
            sw.Flush();
            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            
            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            
            using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            
            return sr.ReadToEnd();
        }
    }
}