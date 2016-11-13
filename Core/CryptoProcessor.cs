using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Core.CryptoAlgorithms;
using Core.CryptoAlgorithms.Interfaces;
using Core.CryptoAlgorithms.Signature;
using SHA1 = Core.CryptoAlgorithms.SHA1;

namespace Core
{
    public class CryptoProcessor
    {
        private static string ImportKeyFromFile(X509Certificate2 cert, string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found: '{path}'");

            var encryptedKey = File.ReadAllText(path);
            var rsa = new CryptoAlgorithms.RSA();

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

        public static bool Encrypt(out List<string> errorList, ICryptoAlgorithm cryptoAlgorithm, int keySize, 
            string targetMessageFile, string targetSessionFile, string messagePath, 
            string sessionFileEncryptingPath, X509Certificate2 ownCertificate, string targetHmacPath, string targetCBCMacPath, X509Certificate2 partnerCertificate = null, 
            string targetSignatureFile = null)
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

            var rsa = new CryptoAlgorithms.RSA();
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

            string encryptedMessage;
            try
            {
                encryptedMessage = cryptoAlgorithm.Encrypt(message, sessionKey);
            }
            catch (Exception ex)
            {
                errorList.Add(ex.Message);
                return false;
            }

            try
            {
                SaveFile(encryptedMessage, targetMessageFile);
                if (!string.IsNullOrEmpty(targetSessionFile))
                    SaveFile(encryptedSessionKey, targetSessionFile);
            }
            catch (Exception ex)
            {
                errorList.Add(ex.Message);
                return false;
            }
            try
            {
                var keyByte = Encoding.ASCII.GetBytes(sessionKey);
                var messageByte = Encoding.Default.GetBytes(message);
                HMACSHA1 hmacsha1 = new HMACSHA1(keyByte);
                var hashmessage = hmacsha1.ComputeHash(messageByte);
                string hmac1 = ByteToString(hashmessage);
                SaveFile(hmac1, targetHmacPath);
            }
            catch (Exception ex)
            {
                errorList.Add(ex.Message);
                return false;
            }

            if (targetSignatureFile != null)
            {
                try
                {
                    var hashedText = SHA1.GetHash(message);
                    string encryptedText = new CryptoAlgorithms.RSA().SignHash(ownCertificate, hashedText);
                    SaveFile(encryptedText, targetSignatureFile);
                }
                catch (Exception ex)
                {
                    errorList.Add(ex.Message);
                    return false;
                }
            }
            if (targetCBCMacPath != null)
            {
                try
                {
                    var messageBytes = Encoding.Default.GetBytes(message);
                    var sessionKeyBytes = Encoding.ASCII.GetBytes(sessionKey.Substring(0, 8));
                    var cbcMacText = CBCMac.Generate(messageBytes, sessionKeyBytes);
                    SaveFile(cbcMacText, targetCBCMacPath);
                }
                catch (Exception ex)
                {
                    errorList.Add(ex.Message);
                    return false;
                }
            }
            return true;
        }
        private static string ByteToString(byte[] buff)
        {
            string sbinary = buff.Aggregate("", (current, t) => current + t.ToString("X2"));

            return sbinary;
        }

        public static bool Decrypt(out List<string> errorList, ICryptoAlgorithm cryptoAlgorithm, string targetDecryptedFile,
            string cryptedPath, string sessionFilePath, X509Certificate2 ownCertificate, string hmacPath = null, 
            string cbcPath = null, X509Certificate2 parnterCertificate = null, string signatureFilePath = null)
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

            var rsa = new CryptoAlgorithms.RSA();
            string sessionKey;
            try
            {
                sessionKey = rsa.Decrypt(ownCertificate, cryptedSessionKey);
            }
            catch (Exception ex)
            {
                errorList.Add(ex.Message);
                return false;
            }

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
            
            string message;
            try
            {
                message = cryptoAlgorithm.Decrypt(cryptedMessage, sessionKey);
            }
            catch (Exception ex)
            {
                errorList.Add(ex.Message);
                return false;
            }
            if (!string.IsNullOrEmpty(hmacPath))
            {
                try
                {
                    var keyByte = Encoding.ASCII.GetBytes(sessionKey);
                    var messageByte = Encoding.Default.GetBytes(message);
                    HMACSHA1 hmacsha1 = new HMACSHA1(keyByte);
                    var hashmessage = hmacsha1.ComputeHash(messageByte);
                    string hmacCurrent = ByteToString(hashmessage);
                    string hmacFromFile = ReadFile(hmacPath);
                    if (hmacCurrent != hmacFromFile)
                    {
                        errorList.Add("Проверка целостности HMAC не пройдена!");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    errorList.Add(ex.Message);
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(signatureFilePath)) //ЭЦП
            {
                string signature = File.ReadAllText(signatureFilePath);
                string hashedText = SHA1.GetHash(message);
                try
                {
                    if (!rsa.VerifySign(parnterCertificate, hashedText, signature))
                    {
                        errorList.Add("Проверка подписи не пройдена!");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    errorList.Add(ex.Message);
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(cbcPath))
            {
                try
                {
                    var messageBytes = Encoding.Default.GetBytes(message);
                    var sessionKeyBytes = Encoding.ASCII.GetBytes(sessionKey.Substring(0, 8));
                    var cbcMacGenerated = CBCMac.Generate(messageBytes, sessionKeyBytes);
                    var cbcMacFromFile = ReadFile(cbcPath);
                    if (cbcMacGenerated != cbcMacFromFile)
                    {
                        errorList.Add("Проверка CBC-MAC не пройдена.");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    errorList.Add(ex.Message);
                    return false;
                }
            }

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
