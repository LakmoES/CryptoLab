using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.CryptoAlgorithms
{
    public class AESAlgorithm : ICryptoAlgorithm
    {
        public AESAlgorithm()
        {
            
            throw new NotImplementedException();
        }
        public CipherMode? CryptoMode
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public ICollection<CipherMode> CryptoModes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ICollection<int> KeySizeCollection
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Decrypt(string message, string password)
        {
            throw new NotImplementedException();
        }

        public string Encrypt(string message, string password)
        {
            throw new NotImplementedException();
        }
    }
}
