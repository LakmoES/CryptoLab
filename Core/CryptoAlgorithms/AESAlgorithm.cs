using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.CryptoAlgorithms
{
    public class AESAlgorithm : ICryptoAlgorithm
    {
        private readonly AesCryptoServiceProvider _provider;
        public AESAlgorithm()
        {
            Name = "AES";
            _provider = new AesCryptoServiceProvider();
            KeySizeCollection = new List<int>
            {
                256
            };
            CryptoModes = new List<CipherMode>
            {
                CipherMode.CBC,
                CipherMode.ECB
            };
        }
        public string Name { get; }

        public ICollection<int> KeySizeCollection { get; }

        public CipherMode? CryptoMode
        {
            set { _provider.Mode = (CipherMode)value; }
            get { return _provider.Mode; }
        }

        public ICollection<CipherMode> CryptoModes { get; }

        public string Decrypt(string message, string password)
        {
            if (message == null || message.Length <= 0)
                throw new ArgumentNullException("empty text");
            if (password == null || password.Length <= 0)
                throw new ArgumentNullException("empty key");

            string plaintext = null;

            _provider.Key = ASCIIEncoding.ASCII.GetBytes(password);
            _provider.IV = ASCIIEncoding.ASCII.GetBytes(password.Substring(0, password.Length/2));


            ICryptoTransform decryptor = _provider.CreateDecryptor(_provider.Key, _provider.IV);

            MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(message));
            CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            StreamReader srDecrypt = new StreamReader(csDecrypt);

            plaintext = srDecrypt.ReadToEnd();

            return plaintext;
        }

        public string Encrypt(string message, string password)
        {
            if (message == null || message.Length <= 0)
                throw new ArgumentNullException("empty text");
            if (password == null || password.Length <= 0)
                throw new ArgumentNullException("empty key");


            _provider.Key = ASCIIEncoding.ASCII.GetBytes(password);
            _provider.IV = ASCIIEncoding.ASCII.GetBytes(password.Substring(0, password.Length/2));


            ICryptoTransform encryptor = _provider.CreateEncryptor(_provider.Key, _provider.IV);

            MemoryStream msEncrypt = new MemoryStream();
            CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            StreamWriter swEncrypt = new StreamWriter(csEncrypt);

            swEncrypt.Write(message);
            swEncrypt.Flush();
            csEncrypt.FlushFinalBlock();
            swEncrypt.Flush();

            var encrypted = msEncrypt.ToArray();
            return Convert.ToBase64String(encrypted);
        }
    }
}
