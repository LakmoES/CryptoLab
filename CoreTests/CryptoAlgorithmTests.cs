using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core;
using Core.CryptoAlgorithms;

namespace CoreTests
{
    [TestClass]
    public class CryptoAlgorithmTests
    {
        private static X509Certificate2[] GetX509Certificates()
        {
            var cers = CertRepository.GetCertificates();
            X509Certificate2[] cersArray = new X509Certificate2[cers.Count];
            cers.CopyTo(cersArray, 0);
            return cersArray;
        }

        [TestMethod]
        public void TestRsaEncryptDecrypt()
        {
            string message = "Hello world!";
            message = KeyGenerator.GetRandomKey(117); //118 is not working. WHY??!??!?
            var cersArray = GetX509Certificates();

            X509Certificate2 certificate = cersArray.FirstOrDefault(x => x.Subject.Contains("lakmoes@onu.edu.ua"));
            Assert.IsNotNull(certificate, "certificate is null");
            var rsa = new RSAAlgorithm();
            var encrypted = rsa.Encrypt(certificate, message);
            var decrypted = rsa.Decrypt(certificate, encrypted);

            Assert.AreNotEqual(encrypted, decrypted);
            Assert.AreEqual(message, decrypted);
        }

        [TestMethod]
        public void TestDesEncryptDecrypt()
        {
            string message = "Hello world!";
            string password = "password";
            var des = new DESAlgorithm();

            if (des.CryptoModes.Count <= 0)
                Assert.Fail("des.CryptoModes.Count <= 0");

            foreach (var cryptoMode in des.CryptoModes)
            {
                des.CryptoMode = cryptoMode;

                var encrypted = des.Encrypt(message, password);
                var decrypted = des.Decrypt(encrypted, password);

                Assert.AreNotEqual(message, encrypted, "encrypted and message are equal!");
                Assert.AreEqual(message, decrypted, "message and decrypted are not equal!");
            }
        }

        [TestMethod]
        public void TestAesEncryptDecrypt()
        {
            string message = "Hello world!";
            string password = "passwordpasswordpasswordpassword";
            var aes = new AESAlgorithm();

            if (aes.CryptoModes.Count <= 0)
                Assert.Fail("aes.CryptoModes.Count <= 0");

            foreach (var cryptoMode in aes.CryptoModes)
            {
                aes.CryptoMode = cryptoMode;

                var encrypted = aes.Encrypt(message, password);
                var decrypted = aes.Decrypt(encrypted, password);

                Assert.AreNotEqual(message, encrypted, "encrypted and message are equal!");
                Assert.AreEqual(message, decrypted, "message and decrypted are not equal!");
            }
        }
    }
}
