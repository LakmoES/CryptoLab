using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Core.CryptoAlgorithms;

namespace Core
{
    public class CryptoProcessor
    {
        private static string ImportKeyFromFile(X509Certificate2 cert, string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found: '{path}'");

            var encryptedKey = File.ReadAllText(path);
            var rsa = new RSAAlgorithm();

            return rsa.Decrypt(cert, encryptedKey);
        }

        private static string ReadFile(string path)
        {
            if(!File.Exists(path))
                throw new FileNotFoundException($"File not found: '{path}'");

            return File.ReadAllText(path);
        }

        private static void SaveFile(string message, string path)
        {
            //File.Create(path);
            File.WriteAllText(path, message);
        }

        public static bool Encrypt(out List<string> errorList, ICryptoAlgorithm cryptoAlgorithm, int keySize, string targetMessageFile, string targetSessionFile, string messagePath, string sessionFileEncryptingPath, X509Certificate2 ownCertificate, X509Certificate2 partnerCertificate)
        {
            errorList = new List<string>();
            string sessionKey;

            if (string.IsNullOrEmpty(sessionFileEncryptingPath))
                sessionKey = KeyGenerator.GetRandomKey(keySize/8);
            else
            {
                try
                {
                    sessionKey = ImportKeyFromFile(ownCertificate, sessionFileEncryptingPath);
                }
                catch (FileNotFoundException ex)
                {
                    errorList.Add(ex.Message);
                    return false;
                }
            }

            var rsa = new RSAAlgorithm();
            string encryptedSessionKey;
            try
            {
                encryptedSessionKey = rsa.Encrypt(partnerCertificate, sessionKey);
            }
            catch(Exception ex) {
                errorList.Add(ex.Message);
                return false;
            }

            string message;
            try
            {
                message = ReadFile(messagePath);
            }
            catch (FileNotFoundException ex)
            {
                errorList.Add(ex.Message);
                return false;
            }

            var encryptedMessage = cryptoAlgorithm.Encrypt(message, sessionKey);

            SaveFile(encryptedMessage, targetMessageFile);
            if (!string.IsNullOrEmpty(targetSessionFile))
                SaveFile(encryptedSessionKey, targetSessionFile);

            return true;
        }

        public static bool Decrypt(out List<string> errorList, ICryptoAlgorithm cryptoAlgorithm, string targetDecryptedFile,
            string cryptedPath, string sessionFilePath, X509Certificate2 ownCertificate)
        {
            errorList = new List<string>();

            string cryptedSessionKey;
            try
            {
                cryptedSessionKey = ReadFile(sessionFilePath);
            }
            catch (FileNotFoundException ex)
            {
                errorList.Add(ex.Message);
                return false;
            }

            var rsa = new RSAAlgorithm();
            var sessionKey = rsa.Decrypt(ownCertificate, cryptedSessionKey);

            string cryptedMessage;
            try
            {
                cryptedMessage = ReadFile(cryptedPath);
            }
            catch (FileNotFoundException ex)
            {
                errorList.Add(ex.Message);
                return false;
            }
            
            var message = cryptoAlgorithm.Decrypt(cryptedMessage, sessionKey);

            try
            {
                SaveFile(message, targetDecryptedFile);
            }
            catch (Exception ex)
            {
                errorList.Add(ex.Message);
                return false;
            }

            return true;
        }
    }
}
