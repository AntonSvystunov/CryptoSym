using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CryptoSym.Common
{
    public static class CryptoHelpers
    {
        public static byte[] EncryptBytes(ICryptoTransform cryptoTransform, byte[] bytes)
        {
            byte[] encrypted;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using CryptoStream csEncrypt = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
                csEncrypt.Write(bytes);
                csEncrypt.FlushFinalBlock();
                encrypted = memoryStream.ToArray();
            }
            return encrypted;
        }

        public static byte[] EncryptString(ICryptoTransform cryptoTransform, string plainText)
        {
            byte[] encrypted;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using CryptoStream csEncrypt = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);                
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }
                encrypted = memoryStream.ToArray();
            }
            return encrypted;
        }

        public static string DecryptToString(ICryptoTransform cryptoTransform, byte[] cipherText)
        {
            string plaintext = null;

            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using CryptoStream csDecrypt = new CryptoStream(msDecrypt, cryptoTransform, CryptoStreamMode.Read);
                using StreamReader srDecrypt = new StreamReader(csDecrypt);
                plaintext = srDecrypt.ReadToEnd();
            }

            return plaintext;
        }
    }
}
