using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.CryptoAlgorithms
{
    public static class SHA1
    {
        public static string GetHash(string source)
        {
            SHA1Managed hash = new SHA1Managed();
            var hashBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(source));
            return Encoding.ASCII.GetString(hashBytes);
        }
    }
}
