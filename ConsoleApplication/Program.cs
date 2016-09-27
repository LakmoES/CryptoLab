using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Core;
using Core.CryptoAlgorithms;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var cers = CertRepository.GetCertificates();
            Console.WriteLine($"MY repository: {cers.Count}");

            foreach (var cer in cers)
            {
                int pubKeySize = cer.GetPublicKeyString().Length * sizeof(char);
                Console.WriteLine($"\nSubject: {cer.Subject}\nPublic key: ({pubKeySize}) {cer.GetPublicKeyString()}");
                Console.WriteLine($"Pub key size: {cer.PublicKey.Key.KeySize}");
                //Console.WriteLine(cer.PublicKey.EncodedKeyValue.Format(false));
            }
            TestDES();
            Console.ReadLine();
        }

        static void TestDES()
        {
            string message = "My very good message!";
            for (int i = 0; i < 10; ++i)
            {
                string password = KeyGenerator.GetRandomKey(8);

                var des = new DESAlgotithm();

                string encrypted = des.Encrypt(message, password);
                string decrypted = des.Decrypt(encrypted, password);

                Console.WriteLine($"\n\nmessage: {message}\nencrypted: {encrypted}\ndecrypted: {decrypted}");
            }
        }
    }
}
