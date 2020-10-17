using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace CryptoSym.Salsa20.Tests
{
    public class Salsa20ContextTests
    {
        private byte[] EncryptToBytes(ICryptoTransform cryptoTransform, string plainText)
        {
            byte[] encrypted;
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, cryptoTransform, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
            return encrypted;
        }

        private string DecryptToString(ICryptoTransform cryptoTransform, byte[] cipherText)
        {
            string plaintext = null;

            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, cryptoTransform, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }

            return plaintext;
        }

        [Fact]
        public void RoundTrip32()
        {
            byte[] k = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 };
            byte[] n = { 3, 1, 4, 1, 5, 9, 2, 6 };

            var salsa = new Salsa20Context();

            var plainText = "Hello World 123 aasjakjskakjahjdkha jkdhjkaSHJKD HASJKDHASJDJKASH DJKASHDJ AJDH217EH12W1HE89H I912H EH12 H";

            var encrypted = EncryptToBytes(salsa.CreateEncryptor(k, n), plainText);
            var roundTrip = DecryptToString(salsa.CreateDecryptor(k, n), encrypted);

            Assert.Equal(plainText, roundTrip);
        }

        [Fact]
        public void RoundTrip16()
        {
            byte[] k = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            byte[] n = { 3, 1, 4, 1, 5, 9, 2, 6 };

            var salsa = new Salsa20Context();

            var plainText = "Hello World 123 aasjakjskakjahjdkha jkdhjkaSHJKD HASJKDHASJDJKASH DJKASHDJ AJDH217EH12W1HE89H I912H EH12 H";

            var encrypted = EncryptToBytes(salsa.CreateEncryptor(k, n), plainText);
            var roundTrip = DecryptToString(salsa.CreateDecryptor(k, n), encrypted);

            Assert.Equal(plainText, roundTrip);
        }
    }
}
