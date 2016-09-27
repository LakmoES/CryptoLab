using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class KeyGenerator
    {
        public static string GetRandomKey(int keySize)
        {
            byte[] bytes = new byte[keySize];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);

            return Encoding.ASCII.GetString(bytes);
        }
    }
}
