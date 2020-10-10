using System;
using Xunit;
using System.Text;

namespace CryptoSym.RC4.Tests
{
    public class RC4Tests
    {
        [Fact]
        public void RC48Bit()
        {
            var expected = "Test text needed to be encoded. Hello world. 123455";
            var key = "MyStr0ngPa$$word12^9_2l";

            var plainBytes = Encoding.Default.GetBytes(expected);
            var keyBytes = Encoding.Default.GetBytes(key);

            var rc4 = RC4ContextFactory.GetContext(RC4BlockSize.Byte);

            var encodedBytes = rc4.Encrypt(plainBytes, keyBytes);
            var decodedBytes = rc4.Decrypt(encodedBytes, keyBytes);

            var actual = Encoding.Default.GetString(decodedBytes);

            Assert.Equal(plainBytes, decodedBytes);
            Assert.Equal(actual, expected);
        }

        [Fact]
        public void RC416Bit()
        {
            var expected = "Test text needed to be encoded. Hello world. 123455";
            var key = "MyStr0ngPa$$word12^9_2l";

            var plainBytes = Encoding.Default.GetBytes(expected);
            var keyBytes = Encoding.Default.GetBytes(key);

            var rc4 = RC4ContextFactory.GetContext(RC4BlockSize.Short);

            var encodedBytes = rc4.Encrypt(plainBytes, keyBytes);
            var decodedBytes = rc4.Decrypt(encodedBytes, keyBytes);

            var actual = Encoding.Default.GetString(decodedBytes);

            Assert.Equal(plainBytes, decodedBytes);
            Assert.Equal(actual, expected);
        }
    }
}
