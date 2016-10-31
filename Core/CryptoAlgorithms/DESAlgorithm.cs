using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Core.CryptoAlgorithms.Interfaces;

namespace Core.CryptoAlgorithms
{
    public class DESAlgorithm : ICryptoAlgorithm
    {
        private readonly DESCryptoServiceProvider _provider;
        public DESAlgorithm()
        {
            Name = "DES";
            _provider = new DESCryptoServiceProvider();
            KeySizeCollection = new List<int>
            {
                64
            };
            CryptoModes = new List<CipherMode>
            {
                CipherMode.CBC,
                CipherMode.ECB,
                CipherMode.CFB
            };
        }
        public string Name { get; }

        public ICollection<int> KeySizeCollection { get; }

        public CipherMode? CryptoMode
        {
            set { _provider.Mode = (CipherMode) value; }
            get { return _provider.Mode; }
        }

        public ICollection<CipherMode> CryptoModes { get; }

        public string Encrypt(string message, string password)
        {
            // Encode message and password
            byte[] messageBytes = ASCIIEncoding.Default.GetBytes(message);
            byte[] passwordBytes = ASCIIEncoding.ASCII.GetBytes(password);

            // Set encryption settings -- Use password for both key and init. vector
            ICryptoTransform transform = _provider.CreateEncryptor(passwordBytes, passwordBytes);
            CryptoStreamMode mode = CryptoStreamMode.Write;

            // Set up streams and encrypt
            MemoryStream memStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memStream, transform, mode);
            cryptoStream.Write(messageBytes, 0, messageBytes.Length);
            cryptoStream.FlushFinalBlock();

            // Read the encrypted message from the memory stream
            byte[] encryptedMessageBytes = new byte[memStream.Length];
            memStream.Position = 0;
            memStream.Read(encryptedMessageBytes, 0, encryptedMessageBytes.Length);

            // Encode the encrypted message as base64 string
            string encryptedMessage = Convert.ToBase64String(encryptedMessageBytes);

            return encryptedMessage;
        }

        public string Decrypt(string encryptedMessage, string password)
        {
            // Convert encrypted message and password to bytes
            byte[] encryptedMessageBytes = Convert.FromBase64String(encryptedMessage);
            byte[] passwordBytes = ASCIIEncoding.ASCII.GetBytes(password);

            // Set encryption settings -- Use password for both key and init. vector
            ICryptoTransform transform = _provider.CreateDecryptor(passwordBytes, passwordBytes);
            CryptoStreamMode mode = CryptoStreamMode.Write;

            // Set up streams and decrypt
            MemoryStream memStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memStream, transform, mode);
            cryptoStream.Write(encryptedMessageBytes, 0, encryptedMessageBytes.Length);
            cryptoStream.FlushFinalBlock();

            // Read decrypted message from memory stream
            byte[] decryptedMessageBytes = new byte[memStream.Length];
            memStream.Position = 0;
            memStream.Read(decryptedMessageBytes, 0, decryptedMessageBytes.Length);

            // Encode deencrypted binary data to base64 string
            string message = ASCIIEncoding.Default.GetString(decryptedMessageBytes);

            return message;
        }
    }
}
