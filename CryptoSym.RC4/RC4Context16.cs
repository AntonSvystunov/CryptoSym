using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoSym.RC4
{
    public class RC4Context16 : RC4ContextBase
    {
        public override uint BlockSize => sizeof(ushort);

        private readonly ushort[] SBlock;

        public RC4Context16()
        {
            SBlock = new ushort[(int)Math.Pow(2, sizeof(ushort) * 8)];
            for (int i = 0; i < SBlock.Length; i++)
            {
                SBlock[i] = (ushort)i;
            }
        }

        private ushort[] GetSchedule(ushort[] key)
        {
            ushort[] schedule = (ushort[])SBlock.Clone();

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

        private ushort Generate(ushort[] schedule, ref ulong x, ref ulong y)
        {
            x = (x + 1) % Boolean;
            y = (y + schedule[x]) % Boolean;

            var t = schedule[x];
            schedule[x] = schedule[y];
            schedule[y] = t;

            return schedule[((uint)schedule[x] + schedule[y]) % Boolean];
        }

        private ushort[] GetUShort(byte[] data)
        {
            ushort[] sdata = new ushort[(int)Math.Ceiling(data.Length / 2.0)];
            Buffer.BlockCopy(data, 0, sdata, 0, data.Length);
            return sdata;
        }
        private byte[] GetBytes(ushort[] data)
        {
            byte[] sdata = new byte[(int)Math.Ceiling(data.Length * 2.0)];
            Buffer.BlockCopy(data, 0, sdata, 0, data.Length * 2);

            int j = sdata.Length;
            for (; j >= 1 && sdata[j - 1] == 0x00; j--) ;
            return sdata.Take(j).ToArray();
        }


        public override byte[] Decrypt(byte[] data, byte[] key)
        {
            return Encrypt(data, key);
        }

        public override byte[] Encrypt(byte[] data, byte[] key)
        {
            var sdata = GetUShort(data);
            var skey = GetUShort(key);

            ushort[] schedule = GetSchedule(skey);
            ulong x = 0, y = 0;
            ushort[] encoded = new ushort[sdata.Length];

            for (int m = 0; m < sdata.Length; m++)
            {
                encoded[m] = (ushort)(sdata[m] ^ Generate(schedule, ref x, ref y));
            }

            return GetBytes(encoded);
        }
    }
}
