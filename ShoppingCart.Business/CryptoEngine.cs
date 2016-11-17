using System;
using System.IO;
using System.Security.Cryptography;

namespace ShoppingCart.Business
{
    public class CryptoEngine : ICryptoEngine
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public CryptoEngine(string key, string iv)
        {
            _key = Convert.FromBase64String(key);
            _iv = Convert.FromBase64String(iv);
        }
        public string Encrypt(string text)
        {
            if (text == null || text.Length <= 0)
                throw new ArgumentNullException(nameof(text));
            byte[] encrypted;

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _iv;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encrypted);
        }

        public string Decrypt(string cipherText)
        {
            var encodeCard = Convert.FromBase64String(cipherText);
            if (encodeCard == null || encodeCard.Length <= 0)
                throw new ArgumentNullException(nameof(encodeCard));
            string plaintext;

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _iv;
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (var msDecrypt = new MemoryStream(encodeCard))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
    }
}
