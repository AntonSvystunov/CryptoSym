using System;

namespace CryptoSym.Kalyna
{
    public class KalynaContext
    {
        public enum BlockSize
        {
            B128,
            B256,
            B512
        }

        public enum KeySize
        {
            K128,
            K256,
            K512
        }

        public uint Nb { get; private set; }
        public uint Nk { get; private set; }
        public uint Nr { get; private set; }

        private void Init(BlockSize blockSize, KeySize keySize)
        {
            if (blockSize == BlockSize.B128)
            {
                Nb = 2;
                if (keySize == KeySize.K128)
                {
                    Nk = 2;
                    Nr = 10;
                }
                else if (keySize == KeySize.K256)
                {
                    Nk = 4;
                    Nr = 14;
                }
                else
                {
                    throw new ArgumentNullException("Key size not supported");
                }
            }
            else if (blockSize == BlockSize.B256)
            {
                Nb = 4;
                if (keySize == KeySize.K256)
                {
                    Nk = 4;
                    Nr = 14;
                }
                else if (keySize == KeySize.K512)
                {
                    Nk = 8;
                    Nr = 18;
                }
                else
                {
                    throw new ArgumentNullException("Key size not supported");
                }
            }
            else
            {
                Nb = 8;
                if (keySize == KeySize.K512)
                {
                    Nk = 8;
                    Nr = 18;
                }
                else
                {
                    throw new ArgumentNullException("Key size not supported");
                }
            }
        }
        public KalynaContext(BlockSize blockSize, KeySize keySize)
        {
            Init(blockSize, keySize);
        }

        public ulong[] Encrypt(ulong[] plainText, ulong[] chipherKey)
        {
            int round = 0;

            ulong[] state = new ulong[Nb];

            ulong[][] roundKeys = KalynaTransformation.KeyExpansion(chipherKey, ref state, Nb, Nk, Nr);
            
            Array.Copy(plainText, state, Nb);

            KalynaTransformation.AddRoundKey(roundKeys[round], state);
            for (round = 1; round < Nr; ++round)
            {
                KalynaTransformation.EncipherRound(ref state);
                KalynaTransformation.XorRoundKey(roundKeys[round], state);
            }
            KalynaTransformation.EncipherRound(ref state);
            KalynaTransformation.AddRoundKey(roundKeys[Nr], state);

            return state;
        }

        public ulong[] Decrypt(ulong[] chipherText, ulong[] chipherKey)
        {
            uint round = Nr;
            ulong[] state = new ulong[Nb];

            ulong[][] roundKeys = KalynaTransformation.KeyExpansion(chipherKey, ref state, Nb, Nk, Nr);

            Array.Copy(chipherText, state, Nb);

            KalynaTransformation.SubRoundKey(roundKeys[round], state);
            for (round = Nr - 1; round > 0; --round)
            {
                KalynaTransformation.DecipherRound(ref state);
                KalynaTransformation.XorRoundKey(roundKeys[round], state);
            }
            KalynaTransformation.DecipherRound(ref state);
            KalynaTransformation.SubRoundKey(roundKeys[0], state);

            return state;
        }
    }
}
