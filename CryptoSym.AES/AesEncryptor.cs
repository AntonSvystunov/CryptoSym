using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSym.AES
{
    internal class AesEncryptor : ICryptoTransform
    {
        private readonly AesContext _aesContext;
        private readonly byte[] _key;
        private readonly bool _parallel;

        public AesEncryptor(byte[] key, AesContext.AES type, bool parallel = false)
        {
            _aesContext = new AesContext(type);
            if (key.Length != _aesContext.KeySize * 4) throw new ArgumentException("Key size not supported");
            _key = (byte[])key.Clone();
            _parallel = parallel;
        }

        public AesEncryptor(byte[] key, AesContext aesContext, bool parallel = false)
        {
            _aesContext = aesContext;
            if (key.Length != _aesContext.KeySize * 4) throw new ArgumentException("Key size not supported");
            _key = (byte[])key.Clone();
            _parallel = parallel;
        }

        public bool CanReuseTransform => false;

        public bool CanTransformMultipleBlocks => true;

        public int InputBlockSize => _aesContext.BlockSize * 4;

        public int OutputBlockSize => _aesContext.BlockSize * 4;

        public void Dispose()
        {
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            var bytesEncrypted = 0;

            if (_parallel)
            {
                Parallel.For(0, inputCount / InputBlockSize, (i) =>
                {
                    byte[] input = inputBuffer.Skip(inputOffset + i * InputBlockSize).Take(InputBlockSize).ToArray();
                    byte[] key = (byte[])_key.Clone();

                    byte[] output = _aesContext.Encrypt(input, key);
                    output.CopyTo(outputBuffer, outputOffset + i * InputBlockSize);
                    bytesEncrypted += output.Length;
                });
            }
            else
            {
                for (int i = 0; i < inputCount; i += InputBlockSize)
                {
                    byte[] input = inputBuffer.Skip(inputOffset + i).Take(InputBlockSize).ToArray();
                    byte[] key = (byte[])_key.Clone();

                    byte[] output = _aesContext.Encrypt(input, key);
                    output.CopyTo(outputBuffer, outputOffset + i);
                    bytesEncrypted += output.Length;
                }
            }

            return bytesEncrypted;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (inputCount == 0) return new byte[0];

            var tempBuffer = new byte[InputBlockSize];
            Array.Copy(inputBuffer, inputOffset, tempBuffer, 0, inputCount);

            if (inputCount < InputBlockSize)
            {
                tempBuffer[inputCount] = 0x80;
            }

            var transformed = new byte[InputBlockSize];
            TransformBlock(tempBuffer, 0, InputBlockSize, transformed, 0);
            return transformed;
        }
    }
}
