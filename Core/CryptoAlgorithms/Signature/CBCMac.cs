using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core.CryptoAlgorithms.Signature
{
    public class CBCMac
    {
        public static string Generate(byte[] data, byte[] key)
        {
            var iv = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            var result = new byte[16];

            // Create DES and encrypt.
            var des = DES.Create();
            des.Key = key;
            des.IV = iv;
            des.Padding = PaddingMode.None;
            des.Mode = CipherMode.CBC;
            ICryptoTransform cryptoTransform = des.CreateEncryptor(key, iv);
            cryptoTransform.TransformBlock(data, 0, 16, result, 0);

            // Get the last eight bytes of the encrypted data.
            var lastEightBytes = new byte[8];
            Array.Copy(result, 8, lastEightBytes, 0, 8);

            // Convert to hex.
            var hexResult = string.Empty;
            foreach (byte ascii in lastEightBytes)
            {
                int n = (int)ascii;
                hexResult += n.ToString("X").PadLeft(2, '0');
            }

            return hexResult;
        }
    }
}
