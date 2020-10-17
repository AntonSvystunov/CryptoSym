using CryptoSym.Common;
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
        [Fact]
        public void RoundTrip32()
        {
            byte[] k = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 };
            byte[] n = { 3, 1, 4, 1, 5, 9, 2, 6 };

            var salsa = new Salsa20Context();

            var plainText = "Hello World 123 aasjakjskakjahjdkha jkdhjkaSHJKD HASJKDHASJDJKASH DJKASHDJ AJDH217EH12W1HE89H I912H EH12 H";

            var encrypted = CryptoHelpers.EncryptString(salsa.CreateEncryptor(k, n), plainText);
            var roundTrip = CryptoHelpers.DecryptToString(salsa.CreateDecryptor(k, n), encrypted);

            Assert.Equal(plainText, roundTrip);
        }

        [Fact]
        public void RoundTrip16()
        {
            byte[] k = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            byte[] n = { 3, 1, 4, 1, 5, 9, 2, 6 };

            var salsa = new Salsa20Context();

            var plainText = "Hello World 123 aasjakjskakjahjdkha jkdhjkaSHJKD HASJKDHASJDJKASH DJKASHDJ AJDH217EH12W1HE89H I912H EH12 H";

            var encrypted = CryptoHelpers.EncryptString(salsa.CreateEncryptor(k, n), plainText);
            var roundTrip = CryptoHelpers.DecryptToString(salsa.CreateDecryptor(k, n), encrypted);

            Assert.Equal(plainText, roundTrip);
        }
    }
}
