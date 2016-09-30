using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Core.CryptoAlgorithms
{
    public interface ICryptoAlgorithm
    {
        string Name { get; }
        ICollection<int> KeySizeCollection { get; }
        ICollection<CipherMode> CryptoModes { get; }
        CipherMode? CryptoMode { set; get; }
        string Encrypt(string message, string password);
        string Decrypt(string message, string password);
    }
}
