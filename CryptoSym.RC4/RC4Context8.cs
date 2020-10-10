using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoSym.RC4
{
    public class RC4Context8 : RC4ContextBase
    {
        public override uint BlockSize => sizeof(byte);

        private readonly byte[] SBlock; 

        public RC4Context8()
        {
            SBlock = new byte[(int)Math.Pow(2, sizeof(byte) * 8)];
            for (int i = 0; i < SBlock.Length; i++)
            {
                SBlock[i] = (byte)i;
            }
        }

        private byte[] GetSchedule(byte[] key)
        {
            byte[] schedule = (byte[])SBlock.Clone();

            ulong j = 0;
            for (uint i = 0; i < Boolean; i++)
            {
                j = (j + schedule[i] + key[i % key.Length]) % Boolean;
                var t = schedule[i];
                schedule[i] = schedule[j];
                schedule[j] = t;
            }

            return schedule;
        }

        private byte Generate(byte[] schedule, ref ulong x, ref ulong y)
        {
            x = (x + 1) % Boolean;
            y = (y + schedule[x]) % Boolean;

            var t = schedule[x];
            schedule[x] = schedule[y];
            schedule[y] = t;

            return schedule[((uint)schedule[x] + (uint)schedule[y]) % Boolean];
        }

        public override byte[] Decrypt(byte[] data, byte[] key)
        {
            return Encrypt(data, key);
        }

        public override byte[] Encrypt(byte[] data, byte[] key)
        {
            byte[] schedule = GetSchedule(key);
            ulong x = 0, y = 0;
            byte[] encoded = new byte[data.Length];

            for (int m = 0; m < data.Length; m++)
            {
                encoded[m] = (byte)(data[m] ^ Generate(schedule, ref x, ref y));
            }

            return encoded;
        }
    }
}
