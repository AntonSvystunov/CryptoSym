using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CryptoSym.AES
{
    public class AesContext: SymmetricAlgorithm
    {
        public enum AES
        {
            AES128 = 128,
            AES192 = 192,
            AES256 = 256
        };

        public AesContext(AES type = AES.AES128)
        {
            LegalBlockSizesValue = new[] { new KeySizes(128, 128, 0) };
            LegalKeySizesValue = new[] { new KeySizes(128, 256, 64) };

            KeySize = GetKeyLength(type);
            BlockSize = 128;
        }

        public bool Parallel { get; set; }
        public AesStreamMode AesStreamMode { get; set; }

        public static AesContext AES128 => new AesContext(AES.AES128);
        public static AesContext AES192 => new AesContext(AES.AES192);
        public static AesContext AES256 => new AesContext(AES.AES256);
        /*public ICryptoTransform CreateEncryptor(byte[] key, bool parallel = false) => new AesEncryptor(key, this, parallel);
        public ICryptoTransform CreateDecryptor(byte[] key, bool parallel = false) => new AesDecryptor(key, this, parallel);*/

        private static int GetKeyLength(AES type)
        {
            return (int)type;
        }

        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            if (!ValidKeySize(rgbKey.Length * 8))
            {
                throw new CryptographicException("Invalid key size; it must be 128 or 256 bits.");
            }

            return new AesDecryptor(rgbKey, rgbIV, Parallel, AesStreamMode);
        }

        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            if (!ValidKeySize(rgbKey.Length * 8))
            {
                throw new CryptographicException("Invalid key size; it must be 128 or 256 bits.");
            }

            return new AesEncryptor(rgbKey, rgbIV, Parallel, AesStreamMode);
        }

        public override byte[] IV 
        { 
            get => base.IV; 
            set
            {
                if (IV.Length != BlockSize / 8)
                {
                    throw new CryptographicException("IV should be equal to block size");
                }
                IVValue = (byte[])value.Clone();
            }
        }

        public override void GenerateIV()
        {
            IV = GetRandomBytes(BlockSize / 8);
        }

        public override void GenerateKey()
        {
            Key = GetRandomBytes(KeySize / 8);
        }

        private static byte[] GetRandomBytes(int byteCount)
        {
            byte[] bytes = new byte[byteCount];
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            return bytes;
        }
    }
}
