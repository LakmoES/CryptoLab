using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
            string message = "My very good message!";
            string password = KeyGenerator.GetRandomKey(8);
            Console.WriteLine(CBC_MAC(Encoding.Default.GetBytes(message), Encoding.ASCII.GetBytes(password)));

            HMAC();

            Console.ReadLine();
        }

        public static string CBC_MAC(byte[] data, byte[] key)
        {
            var IV = new byte[] {0, 0, 0, 0, 0, 0, 0, 0};
            var result = new byte[16];

            // Create DES and encrypt.
            var des = DES.Create();
            des.Key = key;
            des.IV = IV;
            des.Padding = PaddingMode.None;
            des.Mode = CipherMode.CBC;
            ICryptoTransform cryptoTransform = des.CreateEncryptor(key, IV);
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

        private static void HMAC()
        {
            string message;
            string key;
            key = "0000";
            message = "1";
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(key);
            
            HMACMD5 hmacmd5 = new HMACMD5(keyByte);
            HMACSHA1 hmacsha1 = new HMACSHA1(keyByte);

            byte[] messageBytes = encoding.GetBytes(message);
            byte[] hashmessage = hmacmd5.ComputeHash(messageBytes);
            string hmac1 = ByteToString(hashmessage);

            hashmessage = hmacsha1.ComputeHash(messageBytes);
            string hmac2 = ByteToString(hashmessage);
            
            Console.WriteLine($"hmac1: {hmac1} hmac2:{hmac2}");
        }
        public static string ByteToString(byte[] buff)
        {
            string sbinary = "";

            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }
            return (sbinary);
        }
    }
}
