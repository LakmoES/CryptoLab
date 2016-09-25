using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class CertRepository
    {
        public static X509Certificate2Collection GetCertificates()
        {
            X509Store store = new X509Store(StoreName.My);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection cers = store.Certificates;
            return cers;
        }
    }
}
