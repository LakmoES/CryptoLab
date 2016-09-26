using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Core;

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
                Console.WriteLine($"Subject: {cer.Subject}\nPublic key: ({pubKeySize}) {cer.GetPublicKeyString()}\n");
                //Console.WriteLine(cer.PublicKey.EncodedKeyValue.Format(false));
            }

            Console.ReadLine();
        }
    }
}
