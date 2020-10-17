using System;
using System.Linq;
using Xunit;

namespace CryptoSym.Salsa20.Tests
{
    public class Salsa20TransformationTests
    {
        [Fact]
        public void LeftRotate()
        {
            uint expected = 0x150f0fd8;
            uint actual = Salsa20Transformation.RotateLeft(0xc0a8787e, 5);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void QuaterRoundCorrectZeros()
        {
            uint[] s = { 0x00000000, 0x00000000, 0x00000000, 0x00000000 };
            (s[0], s[1], s[2], s[3]) = Salsa20Transformation.QuarterRound(s[0], s[1], s[2], s[3]);

            Assert.Equal(s[0], (uint)0x00000000);
            Assert.Equal(s[1], (uint)0x00000000);
            Assert.Equal(s[2], (uint)0x00000000);
            Assert.Equal(s[3], (uint)0x00000000);
        }

        [Fact]
        public void QuaterRoundCorrect1()
        {
            uint[] s = { 0x00000001, 0x00000000, 0x00000000, 0x00000000 };
            (s[0], s[1], s[2], s[3]) = Salsa20Transformation.QuarterRound(s[0], s[1], s[2], s[3]);

            Assert.Equal(s[0], (uint)0x08008145);
            Assert.Equal(s[1], (uint)0x00000080);
            Assert.Equal(s[2], (uint)0x00010200);
            Assert.Equal(s[3], (uint)0x20500000);
        }

        [Fact]
        public void QuaterRoundCorrect2()
        {
            uint[] s = { 0x00000000, 0x00000001, 0x00000000, 0x00000000 };
            (s[0], s[1], s[2], s[3]) = Salsa20Transformation.QuarterRound(s[0], s[1], s[2], s[3]);

            Assert.Equal(s[0], (uint)0x88000100);
            Assert.Equal(s[1], (uint)0x00000001);
            Assert.Equal(s[2], (uint)0x00000200);
            Assert.Equal(s[3], (uint)0x00402000);
        }

        [Fact]
        public void RowRoundCorrect()
        { 
            uint[] s = { 0x00000001, 0x00000000, 0x00000000, 0x00000000,
                         0x00000001, 0x00000000, 0x00000000, 0x00000000,
                         0x00000001, 0x00000000, 0x00000000, 0x00000000,
                         0x00000001, 0x00000000, 0x00000000, 0x00000000 };

            Salsa20Transformation.RowRound(s);
            Assert.Equal(s[0], (uint)0x08008145);
            Assert.Equal(s[1], (uint)0x00000080);
            Assert.Equal(s[2], (uint)0x00010200);
            Assert.Equal(s[3], (uint)0x20500000);
            Assert.Equal(s[4], (uint)0x20100001);
            Assert.Equal(s[5], (uint)0x00048044);
            Assert.Equal(s[6], (uint)0x00000080);
            Assert.Equal(s[7], (uint)0x00010000);
            Assert.Equal(s[8], (uint)0x00000001);
            Assert.Equal(s[9], (uint)0x00002000);
            Assert.Equal(s[10], (uint)0x80040000);
            Assert.Equal(s[11], (uint)0x00000000);
            Assert.Equal(s[12], (uint)0x00000001);
            Assert.Equal(s[13], (uint)0x00000200);
            Assert.Equal(s[14], (uint)0x00402000);
            Assert.Equal(s[15], (uint)0x88000100);
        }

        [Fact]
        public void RowRoundCorrect2()
        {
            uint[] s = { 0x08521bd6, 0x1fe88837, 0xbb2aa576, 0x3aa26365,
                         0xc54c6a5b, 0x2fc74c2f, 0x6dd39cc3, 0xda0a64f6,
                         0x90a2f23d, 0x067f95a6, 0x06b35f61, 0x41e4732e,
                         0xe859c100, 0xea4d84b7, 0x0f619bff, 0xbc6e965a };

            Salsa20Transformation.RowRound(s);
            Assert.Equal(s[0], (uint)0xa890d39d);
            Assert.Equal(s[1], (uint)0x65d71596);
            Assert.Equal(s[2], (uint)0xe9487daa);
            Assert.Equal(s[3], (uint)0xc8ca6a86);
            Assert.Equal(s[4], (uint)0x949d2192);
            Assert.Equal(s[5], (uint)0x764b7754);
            Assert.Equal(s[6], (uint)0xe408d9b9);
            Assert.Equal(s[7], (uint)0x7a41b4d1);
            Assert.Equal(s[8], (uint)0x3402e183);
            Assert.Equal(s[9], (uint)0x3c3af432);
            Assert.Equal(s[10], (uint)0x50669f96);
            Assert.Equal(s[11], (uint)0xd89ef0a8);
            Assert.Equal(s[12], (uint)0x0040ede5);
            Assert.Equal(s[13], (uint)0xb545fbce);
            Assert.Equal(s[14], (uint)0xd257ed4f);
            Assert.Equal(s[15], (uint)0x1818882d);
        }

        [Fact]
        public void ColumnRoundCorrect()
        {
            uint[] s = { 0x00000001, 0x00000000, 0x00000000, 0x00000000,
                         0x00000001, 0x00000000, 0x00000000, 0x00000000,
                         0x00000001, 0x00000000, 0x00000000, 0x00000000,
                         0x00000001, 0x00000000, 0x00000000, 0x00000000 };

            Salsa20Transformation.ColumnRound(s);
            Assert.Equal(s[0], (uint)0x10090288);
            Assert.Equal(s[1], (uint)0x00000000);
            Assert.Equal(s[2], (uint)0x00000000);
            Assert.Equal(s[3], (uint)0x00000000);
            Assert.Equal(s[4], (uint)0x00000101);
            Assert.Equal(s[5], (uint)0x00000000);
            Assert.Equal(s[6], (uint)0x00000000);
            Assert.Equal(s[7], (uint)0x00000000);
            Assert.Equal(s[8], (uint)0x00020401);
            Assert.Equal(s[9], (uint)0x00000000);
            Assert.Equal(s[10], (uint)0x00000000);
            Assert.Equal(s[11], (uint)0x00000000);
            Assert.Equal(s[12], (uint)0x40a04001);
            Assert.Equal(s[13], (uint)0x00000000);
            Assert.Equal(s[14], (uint)0x00000000);
            Assert.Equal(s[15], (uint)0x00000000);
        }

        [Fact]
        public void ColumnRoundCorrect2()
        {
            uint[] s = { 0x08521bd6, 0x1fe88837, 0xbb2aa576, 0x3aa26365,
                         0xc54c6a5b, 0x2fc74c2f, 0x6dd39cc3, 0xda0a64f6,
                         0x90a2f23d, 0x067f95a6, 0x06b35f61, 0x41e4732e,
                         0xe859c100, 0xea4d84b7, 0x0f619bff, 0xbc6e965a };

            Salsa20Transformation.ColumnRound(s);
            Assert.Equal(s[0], (uint)0x8c9d190a);
            Assert.Equal(s[1], (uint)0xce8e4c90);
            Assert.Equal(s[2], (uint)0x1ef8e9d3);
            Assert.Equal(s[3], (uint)0x1326a71a);
            Assert.Equal(s[4], (uint)0x90a20123);
            Assert.Equal(s[5], (uint)0xead3c4f3);
            Assert.Equal(s[6], (uint)0x63a091a0);
            Assert.Equal(s[7], (uint)0xf0708d69);
            Assert.Equal(s[8], (uint)0x789b010c);
            Assert.Equal(s[9], (uint)0xd195a681);
            Assert.Equal(s[10], (uint)0xeb7d5504);
            Assert.Equal(s[11], (uint)0xa774135c);
            Assert.Equal(s[12], (uint)0x481c2027);
            Assert.Equal(s[13], (uint)0x53a8e4b5);
            Assert.Equal(s[14], (uint)0x4c1f89c5);
            Assert.Equal(s[15], (uint)0x3f78c9c8);
        }

        [Fact]
        public void DoubleRoundCorrect()
        {
            uint[] s = { 0x00000001, 0x00000000, 0x00000000, 0x00000000,
                         0x00000000, 0x00000000, 0x00000000, 0x00000000,
                         0x00000000, 0x00000000, 0x00000000, 0x00000000,
                         0x00000000, 0x00000000, 0x00000000, 0x00000000 };

            Salsa20Transformation.DoubleRound(s);
            Assert.Equal(s[0], (uint)0x8186a22d);
            Assert.Equal(s[1], (uint)0x0040a284);
            Assert.Equal(s[2], (uint)0x82479210);
            Assert.Equal(s[3], (uint)0x06929051);
            Assert.Equal(s[4], (uint)0x08000090);
            Assert.Equal(s[5], (uint)0x02402200);
            Assert.Equal(s[6], (uint)0x00004000);
            Assert.Equal(s[7], (uint)0x00800000);
            Assert.Equal(s[8], (uint)0x00010200);
            Assert.Equal(s[9], (uint)0x20400000);
            Assert.Equal(s[10], (uint)0x08008104);
            Assert.Equal(s[11], (uint)0x00000000);
            Assert.Equal(s[12], (uint)0x20500000);
            Assert.Equal(s[13], (uint)0xa0000040);
            Assert.Equal(s[14], (uint)0x0008180a);
            Assert.Equal(s[15], (uint)0x612a8020);
        }

        [Fact]
        public void DoubleRoundCorrect2()
        {
            uint[] s = { 0xde501066, 0x6f9eb8f7, 0xe4fbbd9b, 0x454e3f57,
                         0xb75540d3, 0x43e93a4c, 0x3a6f2aa0, 0x726d6b36,
                         0x9243f484, 0x9145d1e8, 0x4fa9d247, 0xdc8dee11,
                         0x054bf545, 0x254dd653, 0xd9421b6d, 0x67b276c1 };

            Salsa20Transformation.DoubleRound(s);
            Assert.Equal(s[0], (uint)0xccaaf672);
            Assert.Equal(s[1], (uint)0x23d960f7);
            Assert.Equal(s[2], (uint)0x9153e63a);
            Assert.Equal(s[3], (uint)0xcd9a60d0);
            Assert.Equal(s[4], (uint)0x50440492);
            Assert.Equal(s[5], (uint)0xf07cad19);
            Assert.Equal(s[6], (uint)0xae344aa0);
            Assert.Equal(s[7], (uint)0xdf4cfdfc);
            Assert.Equal(s[8], (uint)0xca531c29);
            Assert.Equal(s[9], (uint)0x8e7943db);
            Assert.Equal(s[10], (uint)0xac1680cd);
            Assert.Equal(s[11], (uint)0xd503ca00);
            Assert.Equal(s[12], (uint)0xa74b2ad6);
            Assert.Equal(s[13], (uint)0xbc331c5c);
            Assert.Equal(s[14], (uint)0x1dda24c7);
            Assert.Equal(s[15], (uint)0xee928277);
        }

        [Fact]
        public void LittleEndianCorrect()
        {
            byte[] arr = { 86, 75, 30, 9 };
            uint expected = 0x091e4b56;
            uint actual = Salsa20Transformation.LittleEndian(arr, 0);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void RevLittleEndianCorrect()
        {
            byte[] arr = new byte[4];
            uint input = 0x091e4b56;

            Salsa20Transformation.RevLittleEndian(input, arr, 0);

            Assert.Equal(arr[0], (byte)86);
            Assert.Equal(arr[1], (byte)75);
            Assert.Equal(arr[2], (byte)30);
            Assert.Equal(arr[3], (byte)9);
        }

        [Fact]
        public void SalsaBlockCorrectZeros()
        {
            uint[] input = {
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
            byte[] expected = new byte[input.Length * 4];
            expected.Initialize();

            byte[] actual = new byte[expected.Length];
            Salsa20Transformation.SalsaBlock(actual, input, 20);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void SalsaBlockCorrect()
        {
            uint[] input = {
                Salsa20Transformation.LittleEndian(new byte[]{ 211, 159, 13, 115 }, 0),
                Salsa20Transformation.LittleEndian(new byte[]{ 76, 55, 82, 183 }, 0),
                Salsa20Transformation.LittleEndian(new byte[]{ 3, 117, 222, 37, }, 0),
                Salsa20Transformation.LittleEndian(new byte[]{ 191, 187, 234, 136,}, 0),
                Salsa20Transformation.LittleEndian(new byte[]{ 49, 237, 179, 48,}, 0),
                Salsa20Transformation.LittleEndian(new byte[]{ 1, 106, 178, 219,}, 0),
                Salsa20Transformation.LittleEndian(new byte[]{ 175, 199, 166, 48,}, 0),
                Salsa20Transformation.LittleEndian(new byte[]{ 86, 16, 179, 207,}, 0),
                Salsa20Transformation.LittleEndian(new byte[]{ 31, 240, 32, 63,}, 0),
                Salsa20Transformation.LittleEndian(new byte[]{ 15, 83, 93, 161,}, 0),
                Salsa20Transformation.LittleEndian(new byte[]{ 116, 147, 48, 113,}, 0),
                Salsa20Transformation.LittleEndian(new byte[]{ 238, 55, 204, 36,}, 0),
                Salsa20Transformation.LittleEndian(new byte[]{ 79, 201, 235, 79,}, 0),
                Salsa20Transformation.LittleEndian(new byte[]{ 3, 81, 156, 47, }, 0),
                Salsa20Transformation.LittleEndian(new byte[]{ 203, 26, 244, 243,}, 0),
                Salsa20Transformation.LittleEndian(new byte[]{ 88, 118, 104, 54 }, 0)
            };

            byte[] h = {
    211, 159, 13, 115, 76, 55, 82, 183, 3, 117, 222, 37, 191, 187, 234, 136,
    49, 237, 179, 48, 1, 106, 178, 219, 175, 199, 166, 48, 86, 16, 179, 207,
    31, 240, 32, 63, 15, 83, 93, 161, 116, 147, 48, 113, 238, 55, 204, 36,
    79, 201, 235, 79, 3, 81, 156, 47, 203, 26, 244, 243, 88, 118, 104, 54 };

            Assert.Equal(h.Take(4).ToArray(), BitConverter.GetBytes(input[0]));

            byte[] expected = {
                109, 42, 178, 168, 156, 240, 248, 238, 168, 196, 190, 203, 26, 110, 170, 154,
                29, 29, 150, 26, 150, 30, 235, 249, 190, 163, 251, 48, 69, 144, 51, 57,
                118, 40, 152, 157, 180, 57, 27, 94, 107, 42, 236, 35, 27, 111, 114, 114,
                219, 236, 232, 135, 111, 155, 110, 18, 24, 232, 95, 158, 179, 19, 48, 202 };

            byte[] actual = new byte[expected.Length];
            Salsa20Transformation.SalsaBlock(actual, input, 20);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void InitialState32()
        {
            byte[] k = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 };
            byte[] n = { 3, 1, 4, 1, 5, 9, 2, 6 };
            uint[] expected = {
                0x61707865, 0x04030201, 0x08070605, 0x0c0b0a09,
                0x100f0e0d, 0x3320646e, 0x01040103, 0x06020905,
                0x00000000, 0x00000000, 0x79622d32, 0x14131211,
                0x18171615, 0x1c1b1a19, 0x201f1e1d, 0x6b206574
            };

            uint[] actual = Salsa20Transformation.CreateInitialState(k, n);
            Assert.Equal(expected, actual);
        }
    }
}
