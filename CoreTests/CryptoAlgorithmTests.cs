using System;
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
            var cersArray = GetX509Certificates();

            X509Certificate2 certificate = cersArray.FirstOrDefault(x => x.Subject.Contains("lakmoes@onu.edu.ua"));
            Assert.IsNotNull(certificate, "certificate is null");
            var encrypted = RSAAlgorithm.Encrypt(certificate, message);
            var decrypted = RSAAlgorithm.Decrypt(certificate, encrypted);

            Assert.AreNotEqual(encrypted, decrypted);
            Assert.AreEqual(message, decrypted);
        }
    }
}
