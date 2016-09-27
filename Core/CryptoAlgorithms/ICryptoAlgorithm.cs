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
        string Encrypt(string message, string password);
        string Decrypt(string message, string password);
    }
}
