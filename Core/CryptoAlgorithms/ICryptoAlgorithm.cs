using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Core.CryptoAlgorithms
{
    public interface ICryptoAlgorithm
    {
        string Name { get; }
        string Encrypt(X509Certificate2 x509, string stringToEncrypt);
        string Decrypt(X509Certificate2 x509, string stringTodecrypt);
    }
}
