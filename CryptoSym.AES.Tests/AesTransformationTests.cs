using System;
using System.Linq;
using Xunit;

namespace CryptoSym.AES.Tests
{
    public class AesTransformationTests
    {
        [Fact]
        public void SubBytesDirect()
        {
            byte[] input = { 0x00, 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80, 0x90, 0xa0, 0xb0, 0xc0, 0xd0, 0xe0, 0xf0 };
            byte[] output = { 0x63, 0xca, 0xb7, 0x04, 0x09, 0x53, 0xd0, 0x51, 0xcd, 0x60, 0xe0, 0xe7, 0xba, 0x70, 0xe1, 0x8c };

            AesTransformation.SubBytes(input);

            Assert.Equal(output, input);
        }

        [Fact]
        public void SubBytesIndirect()
        {
            byte[] input = { 0x63, 0xca, 0xb7, 0x04, 0x09, 0x53, 0xd0, 0x51, 0xcd, 0x60, 0xe0, 0xe7, 0xba, 0x70, 0xe1, 0x8c };
            byte[] output = { 0x00, 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80, 0x90, 0xa0, 0xb0, 0xc0, 0xd0, 0xe0, 0xf0 };
            
            AesTransformation.InvSubBytes(input);

            Assert.Equal(output, input);
        }

        [Fact]
        public void SubBytesAndInvSubSytesThrowsOnInvalidSizedByteArray()
        {
            byte[] input = { 0x63, 0xca, 0xb7 };
            Assert.Throws<ArgumentException>(() =>
            {
                AesTransformation.SubBytes(input);
            });

            Assert.Throws<ArgumentException>(() =>
            {
                AesTransformation.InvSubBytes(input);
            });
        }

        [Fact]
        public void ShiftRowsDirect()
        {
            byte[] input = { 0x63, 0xca, 0xb7, 0x04, 0x09, 0x53, 0xd0, 0x51, 0xcd, 0x60, 0xe0, 0xe7, 0xba, 0x70, 0xe1, 0x8c };
            byte[] output = { 0x63, 0x53, 0xe0, 0x8c, 0x09, 0x60, 0xe1, 0x04, 0xcd, 0x70, 0xb7, 0x51, 0xba, 0xca, 0xd0, 0xe7 };

            AesTransformation.ShiftRows(input);

            Assert.Equal(output, input);
        }

        [Fact]
        public void ShiftRowsInDirect()
        {
            byte[] input =  { 0x63, 0x53, 0xe0, 0x8c, 0x09, 0x60, 0xe1, 0x04, 0xcd, 0x70, 0xb7, 0x51, 0xba, 0xca, 0xd0, 0xe7 };
            byte[] output = { 0x63, 0xca, 0xb7, 0x04, 0x09, 0x53, 0xd0, 0x51, 0xcd, 0x60, 0xe0, 0xe7, 0xba, 0x70, 0xe1, 0x8c };
            
            AesTransformation.InvShiftRows(input);
            
            Assert.Equal(output, input);
        }

        [Fact]
        public void MixColumnsDirect()
        {
            byte[] input = { 0x63, 0x53, 0xe0, 0x8c, 0x09, 0x60, 0xe1, 0x04, 0xcd, 0x70, 0xb7, 0x51, 0xba, 0xca, 0xd0, 0xe7 };
            byte[] output = { 0x5f, 0x72, 0x64, 0x15, 0x57, 0xf5, 0xbc, 0x92, 0xf7, 0xbe, 0x3b, 0x29, 0x1d, 0xb9, 0xf9, 0x1a };
            AesTransformation.MixColumns(input);

            Assert.Equal(output, input);
        }

        [Fact]
        public void MixColumnsIndirect()
        {
            byte[] input = { 0x5f, 0x72, 0x64, 0x15, 0x57, 0xf5, 0xbc, 0x92, 0xf7, 0xbe, 0x3b, 0x29, 0x1d, 0xb9, 0xf9, 0x1a };
            byte[] output = { 0x63, 0x53, 0xe0, 0x8c, 0x09, 0x60, 0xe1, 0x04, 0xcd, 0x70, 0xb7, 0x51, 0xba, 0xca, 0xd0, 0xe7 };

            AesTransformation.InvMixColumns(input);
            Assert.Equal(output, input);
        }

        [Fact]
        public void KeyExpansionDirect()
        {
            byte[] input = { 0x2b, 0x7e, 0x15, 0x16, 0x28, 0xae, 0xd2, 0xa6, 0xab, 0xf7, 0x15, 0x88, 0x09, 0xcf, 0x4f, 0x3c };
            uint[] expected = {
                BitConverter.ToUInt32(new byte[]{ 0xa0, 0xfa, 0xfe, 0x17 }, 0),
                BitConverter.ToUInt32(new byte[]{ 0x88, 0x54, 0x2c, 0xb1 }, 0),
                BitConverter.ToUInt32(new byte[]{ 0x23, 0xa3, 0x39, 0x39 }, 0),
                BitConverter.ToUInt32(new byte[]{ 0x2a, 0x6c, 0x76, 0x05 }, 0)
            };

            uint[] actual = AesTransformation.KeyExpansion(input).Skip(4).Take(4).ToArray();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AddRoundKey()
        {
            byte[] input = { 0x32, 0x43, 0xf6, 0xa8, 0x88, 0x5a, 0x30, 0x8d, 0x31, 0x31, 0x98, 0xa2, 0xe0, 0x37, 0x07, 0x34 };
            uint[] w = {
                BitConverter.ToUInt32(new byte[]{ 0x2b, 0x7e, 0x15, 0x16 }, 0),
                BitConverter.ToUInt32(new byte[]{ 0x28, 0xae, 0xd2, 0xa6 }, 0),
                BitConverter.ToUInt32(new byte[]{ 0xab, 0xf7, 0x15, 0x88 }, 0),
                BitConverter.ToUInt32(new byte[]{ 0x09, 0xcf, 0x4f, 0x3c }, 0)
            };

            byte[] expected = { 0x19, 0x3d, 0xe3, 0xbe, 0xa0, 0xf4, 0xe2, 0x2b, 0x9a, 0xc6, 0x8d, 0x2a, 0xe9, 0xf8, 0x48, 0x08 };
            AES.AesTransformation.AddRoundKey(input, w);

            Assert.Equal(expected, input);
        }

        [Fact]
        public void Correctness128()
        {
            var plainText = "Hello world123456788993812371237123789123";
            var cipherKey128 = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f };

            var aes = AesContext.AES128;
            var encrypted = aes.EncryptStringToBytes(plainText, cipherKey128);

            var actual = aes.DecryptStringFromBytes(encrypted, cipherKey128);

            Assert.Equal(plainText, actual);
        }

        [Fact]
        public void Correctness192()
        {
            var plainText = "Hello world123456788993812371237123789123";
            var cipherKey192 = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, };

            var aes = AesContext.AES192;
            var encrypted = aes.EncryptStringToBytes(plainText, cipherKey192);

            var actual = aes.DecryptStringFromBytes(encrypted, cipherKey192);

            Assert.Equal(plainText, actual);
        }

        [Fact]
        public void Correctness256()
        {
            var plainText = "Hello world123456788993812371237123789123";
            var cipherKey256 = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f };

            var aes = AesContext.AES256;
            var encrypted = aes.EncryptStringToBytes(plainText, cipherKey256);

            var actual = aes.DecryptStringFromBytes(encrypted, cipherKey256);

            Assert.Equal(plainText, actual);
        }

        [Fact]
        public void Correctness128Binary()
        {
            var plainText = "Hello world123456788993812371237123789123";
            var inputBytes = System.Text.Encoding.Default.GetBytes(plainText);

            var cipherKey128 = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f };

            var aes = AesContext.AES128;
            var encrypted = aes.EncryptBytes(inputBytes, cipherKey128);

            var actual = aes.DecryptBytes(encrypted, cipherKey128);

            Assert.Equal(inputBytes, actual);
        }

        [Fact]
        public void Correctness192Binary()
        {
            var plainText = "Hello world123456788993812371237123789123";
            var inputBytes = System.Text.Encoding.Default.GetBytes(plainText);

            var cipherKey192 = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, };

            var aes = AesContext.AES192;
            var encrypted = aes.EncryptBytes(inputBytes, cipherKey192);

            var actual = aes.DecryptStringFromBytes(encrypted, cipherKey192);

            Assert.Equal(plainText, actual);
        }

        [Fact]
        public void Correctness256Binary()
        {
            var plainText = "Hello world1234567889938123712371237891231111111111111111000010101010011sds";
            var inputBytes = System.Text.Encoding.Default.GetBytes(plainText);

            var cipherKey256 = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f };

            var aes = AesContext.AES256;
            var encrypted = aes.EncryptBytes(inputBytes, cipherKey256);

            var actual = aes.DecryptStringFromBytes(encrypted, cipherKey256);

            Assert.Equal(plainText, actual);
        }

    }
}
