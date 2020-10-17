using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CryptoSym.AES
{
    public static class AesTransformation
    {
        private static void CheckState(byte[] state)
        {
            if (state.Length % Nb != 0)
            {
                throw new ArgumentException("State array size should be divisible by Nb provided by standard.");
            }
        }

        public static readonly int Nb = 4;

        public static void SubBytes(byte[] state)
        {
            CheckState(state);

            SBox sBox = SBox.GetInstance();
            for (int i = 0; i < state.Length; i++)
            {
                state[i] = sBox.Substitute(state[i]);
            }
        }

        public static void InvSubBytes(byte[] state)
        {
            CheckState(state);

            SBox sBox = SBox.GetInstance();
            for (int i = 0; i < state.Length; i++)
            {
                state[i] = sBox.InvSubstitute(state[i]);
            }
        }

        private static int Shift(int r, int c)
        {
            return (c + r) % Nb;
        }

        public static void ShiftRows(byte[] state)
        {
            CheckState(state);
            var copy = (byte[])state.Clone();

            for (int r = 1; r < 4; r++)
            {
                for (int c = 0; c < Nb; c++)
                {
                    state[r + c * Nb] = copy[r + Shift(r, c) * Nb];
                }
            }
        }

        public static void InvShiftRows(byte[] state)
        {
            CheckState(state);
            var copy = (byte[])state.Clone();

            for (int r = 1; r < 4; r++)
            {
                for (int c = 0; c < Nb; c++)
                {
                    state[r + Shift(r, c) * Nb] = copy[r + c * Nb];
                }
            }
        }

        public static void MixColumns(byte[] state)
        {
            CheckState(state);
            var state2 = (byte[])state.Clone();

            for (int c = 0; c < 4; c++)
            {
                state[0 + c * Nb] = (byte)(GMul(0x02, state2[0 + c * Nb]) ^ GMul(0x03, state2[1 + c * Nb]) ^ state2[2 + c * Nb] ^ state2[3 + c * Nb]);
                state[1 + c * Nb] = (byte)(state2[0 + c * Nb] ^ GMul(0x02, state2[1 + c * Nb]) ^ GMul(0x03, state2[2 + c * Nb]) ^ state2[3 + c * Nb]);
                state[2 + c * Nb] = (byte)(state2[0 + c * Nb] ^ state2[1 + c * Nb] ^ GMul(0x02, state2[2 + c * Nb]) ^ GMul(0x03, state2[3 + c * Nb]));
                state[3 + c * Nb] = (byte)(GMul(0x03, state2[0 + c * Nb]) ^ state2[1 + c * Nb] ^ state2[2 + c * Nb] ^ GMul(0x02, state2[3 + c * Nb]));
            }
        }

        public static void InvMixColumns(byte[] state)
        {
            CheckState(state);
            var copy = (byte[])state.Clone();

            for (int c = 0; c < 4; c++)
            {
                copy[0 + c * Nb] = (byte)(GMul(0x0e, state[0 + c * Nb]) ^ GMul(0x0b, state[1 + c * Nb]) ^ GMul(0x0d, state[2 + c * Nb]) ^ GMul(0x09, state[3 + c * Nb]));
                copy[1 + c * Nb] = (byte)(GMul(0x09, state[0 + c * Nb]) ^ GMul(0x0e, state[1 + c * Nb]) ^ GMul(0x0b, state[2 + c * Nb]) ^ GMul(0x0d, state[3 + c * Nb]));
                copy[2 + c * Nb] = (byte)(GMul(0x0d, state[0 + c * Nb]) ^ GMul(0x09, state[1 + c * Nb]) ^ GMul(0x0e, state[2 + c * Nb]) ^ GMul(0x0b, state[3 + c * Nb]));
                copy[3 + c * Nb] = (byte)(GMul(0x0b, state[0 + c * Nb]) ^ GMul(0x0d, state[1 + c * Nb]) ^ GMul(0x09, state[2 + c * Nb]) ^ GMul(0x0e, state[3 + c * Nb]));
            }

            copy.CopyTo(state, 0);
        }

        private static byte GMul(byte a, byte b)
        {
            byte p = 0;

            foreach (var _ in Enumerable.Range(0, 8))
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

        public static uint[] KeyExpansion(byte[] key)
        {
            if (key.Length != 16 && key.Length != 24 && key.Length != 32)
            {
                throw new ArgumentException($"Key length {key.Length} is unsupported");
            }

            int nk = key.Length / 4;
            uint[] w = new uint[Nb * (nk + 6 + 1)];

            int i = 0;
            while (i < nk)
            {
                w[i] = BitConverter.ToUInt32(key, 4 * i);
                i++;
            }

            byte rcon = 0x01;
            while (i < w.Length)
            {
                uint temp = w[i - 1];

                if (i % nk == 0)
                {
                    temp = SubWord(RotWord(temp)) ^ BitConverter.ToUInt32(new byte[] { rcon, 0, 0, 0 }, 0);
                    rcon = GMul(rcon, 2);
                } else if (nk > 6 && (i % nk == 4))
                {
                    temp = SubWord(temp);
                }

                w[i] = w[i - nk] ^ temp;
                i++;
            }

            return w;
        }

        public static uint SubWord(uint word)
        {
            byte[] bytes = BitConverter.GetBytes(word);
            SBox sBox = SBox.GetInstance();
            for (int i = 0; i < sizeof(int); i++)
            {
                bytes[i] = sBox.Substitute(bytes[i]);
            }
            return BitConverter.ToUInt32(bytes);
        }

        public static uint RotWord(uint word)
        {
            byte[] bytes = BitConverter.GetBytes(word);
            byte t = bytes[0];
            for (int i = 0; i < sizeof(int) - 1; i++)
            {
                bytes[i] = bytes[i + 1];
            }
            bytes[3] = t;
            return BitConverter.ToUInt32(bytes);
        }

        public static void AddRoundKey(byte[] state, uint[] w)
        {
            CheckState(state);
            byte[][] wBytes = w.Select(w => BitConverter.GetBytes(w)).ToArray();

            for (int r = 0; r < 4; r++)
            {
                for (int c = 0; c < Nb; c++)
                {
                    state[r + c * Nb] ^= wBytes[c][r];
                }
            }
        }

        public static byte[] EncryptBlock(byte[] plainText, byte[] cipherKey)
        {
            int Nr = cipherKey.Length / 4 + Nb + 2;

            byte[] state = (byte[])plainText.Clone();
            byte[] key = (byte[])cipherKey.Clone();

            uint[] schedule = KeyExpansion(key);
            AddRoundKey(state, schedule.Take(Nb).ToArray());

            for (int round = 1; round < Nr; round++)
            {
                SubBytes(state);
                ShiftRows(state);
                MixColumns(state);
                AddRoundKey(state, schedule.Skip(round * Nb).Take(Nb).ToArray());
            }

            SubBytes(state);
            ShiftRows(state);
            AddRoundKey(state, schedule.Skip(Nr * Nb).Take(Nb).ToArray());
            return state;
        }

        public static byte[] DecryptBlock(byte[] chipherText, byte[] cipherKey)
        {
            int Nr = cipherKey.Length / 4 + Nb + 2;
            
            byte[] state = (byte[])chipherText.Clone();
            byte[] key = (byte[])cipherKey.Clone();

            uint[] schedule = KeyExpansion(key);

            AddRoundKey(state, schedule.Skip(Nr * Nb).Take(Nb).ToArray());

            for (int round = Nr - 1; round > 0; round--)
            {
                InvShiftRows(state);
                InvSubBytes(state);
                AddRoundKey(state, schedule.Skip(round * Nb).Take(Nb).ToArray());
                InvMixColumns(state);
            }

            InvShiftRows(state);
            InvSubBytes(state);
            AddRoundKey(state, schedule.Take(Nb).ToArray());
            return state;
        }
    }
}
