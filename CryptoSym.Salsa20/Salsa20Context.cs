using System;
using System.Security.Cryptography;

namespace CryptoSym.Salsa20
{
    public class Salsa20Context : SymmetricAlgorithm
    {
        protected uint _rounds;

        public Salsa20Context()
        {
            LegalBlockSizesValue = new[] { new KeySizes(512, 512, 0) };
            LegalKeySizesValue = new[] { new KeySizes(128, 256, 128) };

            BlockSizeValue = 512;
            KeySizeValue = 256;
            _rounds = 20;
        }

        public uint Rounds
        {
            get => _rounds;
            set 
            {
                if (value != 8 && value != 12 && value != 20)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"Invalid round count: {value}");
                }

                _rounds = value;
            }
        }

        public override byte[] IV 
        { 
            get => base.IV; 
            set
            {
                ValidateVI(value);
                base.IVValue = (byte[])value.Clone();
            }
        }

        private void ValidateVI(byte[] iv)
        {
            if (iv.Length != 8)
            {
                throw new CryptographicException($"Initialization vector has invalid length: {iv.Length}");
            }
        }

        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            return CreateEncryptor(rgbKey, rgbIV);
        }

        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            ValidKeySize(rgbKey.Length * 8);
            ValidateVI(rgbIV);

            return new Salsa20Encryptor(rgbKey, rgbIV, _rounds);
        }

        public override void GenerateIV()
        {
            IV = GetRandomBytes(8);
        }

        public override void GenerateKey()
        {
            Key = GetRandomBytes(KeySize / 8);
        }

        private static byte[] GetRandomBytes(int byteCount)
        {
            byte[] bytes = new byte[byteCount];
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
                rng.GetBytes(bytes);
            return bytes;
        }
    }
}
