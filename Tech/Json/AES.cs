using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace Tech.Json
{
    public class AES : IEncryption
    {
        private static byte[] key;
        private static byte[] iv;
        public AES(string key64string, string iv64string)
        {
            key = Convert.FromBase64String(key64string);
            iv = Convert.FromBase64String(iv64string);
        }
        
        public string Encrypt(string text)
        {
            byte[] cipheredtext;
          
            using (Aes aes = Aes.Create())
            {
                ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(text);
                        }

                        cipheredtext = memoryStream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(cipheredtext);
        }

        public string Decrypt(string text)
        {
            var plaintext = Convert.FromBase64String(text);
            var result = string.Empty;
            using (Aes aes = Aes.Create())
            {
                ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
                using (MemoryStream memoryStream = new MemoryStream(plaintext))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            result = streamReader.ReadToEnd();
                        }
                    }
                }
            }
            return result;
        }
    }
}