using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoSym.AES
{
    public static class ByteExtensions
    {
        public static byte LeftShift(this byte b, byte shift)
        {
            var t = (b << shift);
            return unchecked((byte)(t & 0xFF));
        }

        public static byte RightShift(this byte b, byte shift)
        {
            var t = (b >> shift);
            return unchecked((byte)(t & 0xFF));
        }

        public static byte Or(this byte a, byte b)
        {
            var t = (a | b);
            return unchecked((byte)(t & 0xFF));
        }

        public static byte And(this byte a, byte b)
        {
            var t = (a & b);
            return unchecked((byte)(t & 0xFF));
        }

        public static byte Xor(this byte a, byte b)
        {
            var t = (a ^ b);
            return unchecked((byte)(t & 0xFF));
        }

        public static byte GMul(this byte a, byte b)
        { 
            byte p = 0;

            foreach (var _ in Enumerable.Range(0,8))
            {
                if ((b & 1) != 0)
                {
                    p ^= a;
                }

                bool isReduced = (a & 0x80) != 0;

                a <<= 1;

                if (isReduced)
                {
                    a ^= 0x1B;
                }
                b >>= 1;
            }

            return p;
        }
    }
}
