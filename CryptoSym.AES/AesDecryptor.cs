using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSym.AES
{
    internal sealed class AesDecryptor : ICryptoTransform
    {
        private byte[] _key;
        private byte[] _iv;
        private readonly bool _parallel;
        private readonly AesStreamMode _streamMode;

        public AesDecryptor(byte[] key, byte[] iv, bool parallel = false, AesStreamMode streamMode = AesStreamMode.ECB)
        {
            _key = (byte[])key.Clone();
            if (streamMode != AesStreamMode.ECB)
            {
                _iv = (byte[])iv.Clone();
            }
            _parallel = parallel;
            _streamMode = streamMode;
        }

        public bool CanReuseTransform => false;

        public bool CanTransformMultipleBlocks => true;

        public int InputBlockSize => 16;

        public int OutputBlockSize => 16;

        public void Dispose()
        {
            if (_key != null)
            {
                Array.Clear(_key, 0, _key.Length);
            }

            if (_iv != null)
            {
                Array.Clear(_iv, 0, _iv.Length);
            }

            _key = null;
            _iv = null;
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            return _streamMode switch
            {
                AesStreamMode.ECB => EBC(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset),
                _ => throw new NotImplementedException($"Stream mode {_streamMode} not implemented for AES")
            };            
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (inputCount == 0) return new byte[0];

            var tempBuffer = new byte[InputBlockSize];
            Array.Copy(inputBuffer, inputOffset, tempBuffer, 0, inputCount);
            var transformed = new byte[InputBlockSize];
            TransformBlock(tempBuffer, 0, InputBlockSize, transformed, 0);
            return transformed;
        }

        private int EBC(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            int bytesDecrypted = 0;

            if (_parallel)
            {
                Parallel.For(0, inputCount / InputBlockSize, (i) =>
                {
                    byte[] input = inputBuffer.Skip(inputOffset + i).Take(InputBlockSize).ToArray();

                    byte[] key = (byte[])_key.Clone();
                    byte[] output = AesTransformation.DecryptBlock(input, key);

                    if ((i + 1) * InputBlockSize >= inputCount)
                    {
                        int k = output.Length - 1;
                        while (k >= 0 && output[k] == 0) { k--; }
                        if (k >= 0 && output[k] == 0x80)
                        {
                            output = output.Take(k).ToArray();
                        }
                    }

                    output.CopyTo(outputBuffer, outputOffset + i);
                    bytesDecrypted += output.Length;
                });
            }
            else
            {
                for (int i = 0; i < inputCount; i += InputBlockSize)
                {
                    byte[] input = inputBuffer.Skip(inputOffset + i).Take(InputBlockSize).ToArray();

                    byte[] key = (byte[])_key.Clone();

                    byte[] output = AesTransformation.DecryptBlock(input, key);

                    if (i + InputBlockSize >= inputCount)
                    {
                        int k = output.Length - 1;
                        while (k >= 0 && output[k] == 0) { k--; }
                        if (k >= 0 && output[k] == 0x80)
                        {
                            output = output.Take(k).ToArray();
                        }
                    }

                    output.CopyTo(outputBuffer, outputOffset + i);
                    bytesDecrypted += output.Length;
                }
            }

            return bytesDecrypted;
        }
    }
}
