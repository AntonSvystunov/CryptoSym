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
        private int counter = 0;

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
                AesStreamMode.CBC => CBC(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset),
                AesStreamMode.CFB => CFB(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset),
                AesStreamMode.OFB => OFB(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset),
                AesStreamMode.CTR => CTR(inputBuffer, inputOffset, inputCount, outputBuffer, outputOffset),
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
                    byte[] input = new byte[InputBlockSize];
                    Array.Copy(inputBuffer, inputOffset + i * InputBlockSize, input, 0, InputBlockSize);

                    byte[] key = (byte[])_key.Clone();
                    byte[] output = AesTransformation.DecryptBlock(input, key);

                    if ((i + 1) * InputBlockSize >= inputCount)
                    {
                        output = RemovePadding(output);
                    }

                    output.CopyTo(outputBuffer, outputOffset + i * InputBlockSize);
                    bytesDecrypted += output.Length;
                });
            }
            else
            {
                for (int i = 0; i < inputCount; i += InputBlockSize)
                {
                    byte[] input = new byte[InputBlockSize];
                    Array.Copy(inputBuffer, inputOffset + i, input, 0, InputBlockSize);

                    byte[] key = (byte[])_key.Clone();

                    byte[] output = AesTransformation.DecryptBlock(input, key);

                    if (i + InputBlockSize >= inputCount)
                    {
                        output = RemovePadding(output);
                    }

                    output.CopyTo(outputBuffer, outputOffset + i);
                    bytesDecrypted += output.Length;
                }
            }

            return bytesDecrypted;
        }

        private int CBC(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            var bytesDecrypted = 0;
            var key = (byte[])_key.Clone();

            if (_parallel)
            {
                Parallel.For(0, inputCount / InputBlockSize, (i) =>
                {
                    byte[] input = new byte[InputBlockSize];
                    Array.Copy(inputBuffer, inputOffset + i * InputBlockSize, input, 0, InputBlockSize);
                    byte[] iv = new byte[InputBlockSize];
                    if (i == 0)
                    {
                        iv = (byte[])_iv.Clone();
                    }
                    else
                    {
                        Array.Copy(inputBuffer, inputOffset + (i - 1) * InputBlockSize, iv, 0, InputBlockSize);
                    }

                    byte[] output = AesTransformation.DecryptBlock(input, key);

                    for (int j = 0; j < output.Length; j++)
                    {
                        output[j] ^= iv[j];
                    }

                    if ((i + 1) * InputBlockSize >= inputCount)
                    {
                        output = RemovePadding(output);
                    }

                    output.CopyTo(outputBuffer, outputOffset + i * InputBlockSize);
                    bytesDecrypted += output.Length;
                });
            }
            else
            {
                for (int i = 0; i < inputCount; i += InputBlockSize)
                {
                    byte[] input = new byte[InputBlockSize];
                    Array.Copy(inputBuffer, inputOffset + i, input, 0, InputBlockSize);

                    byte[] output = AesTransformation.DecryptBlock(input, key);

                    for (int j = 0; j < output.Length; j++)
                    {
                        output[j] ^= _iv[j];
                    }

                    if (i + InputBlockSize >= inputCount)
                    {
                        output = RemovePadding(output);
                    }

                    output.CopyTo(outputBuffer, outputOffset + i);
                    input.CopyTo(_iv, 0);

                    bytesDecrypted += output.Length;
                }
            }            

            return bytesDecrypted;
        }

        private int CFB(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            var bytesDecrypted = 0;
            var key = (byte[])_key.Clone();

            if (_parallel)
            {
                Parallel.For(0, inputCount / InputBlockSize, (i) =>
                {
                    byte[] input = new byte[InputBlockSize];
                    Array.Copy(inputBuffer, inputOffset + i * InputBlockSize, input, 0, InputBlockSize);
                    byte[] iv = new byte[InputBlockSize];
                    if (i == 0)
                    {
                        iv = (byte[])_iv.Clone();
                    }
                    else
                    {
                        Array.Copy(inputBuffer, inputOffset + (i - 1) * InputBlockSize, iv, 0, InputBlockSize);
                    }

                    byte[] output = AesTransformation.EncryptBlock(iv, key);

                    for (int j = 0; j < output.Length; j++)
                    {
                        output[j] ^= input[j];
                    }

                    if ((i + 1) * InputBlockSize >= inputCount)
                    {
                        output = RemovePadding(output);
                    }

                    output.CopyTo(outputBuffer, outputOffset + i * InputBlockSize);
                    bytesDecrypted += output.Length;
                });
            }
            else
            {
                for (int i = 0; i < inputCount; i += InputBlockSize)
                {
                    byte[] output = AesTransformation.EncryptBlock(_iv, key);

                    byte[] input = new byte[InputBlockSize];
                    Array.Copy(inputBuffer, inputOffset + i, input, 0, InputBlockSize);                    

                    for (int j = 0; j < output.Length; j++)
                    {
                        output[j] ^= input[j];
                    }

                    if (i + InputBlockSize >= inputCount)
                    {
                        output = RemovePadding(output);
                    }

                    output.CopyTo(outputBuffer, outputOffset + i);
                    input.CopyTo(_iv, 0);

                    bytesDecrypted += output.Length;
                }
            }

            return bytesDecrypted;
        }

        private int OFB(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            var bytesDecrypted = 0;
            var key = (byte[])_key.Clone();

            for (int i = 0; i < inputCount; i += InputBlockSize)
            {
                byte[] output = AesTransformation.EncryptBlock(_iv, key);
                output.CopyTo(_iv, 0);

                byte[] input = new byte[InputBlockSize];
                Array.Copy(inputBuffer, inputOffset + i, input, 0, InputBlockSize);

                for (int j = 0; j < input.Length; j++)
                {
                    output[j] ^= input[j];
                }

                if (i + InputBlockSize >= inputCount)
                {
                    output = RemovePadding(output);
                }

                output.CopyTo(outputBuffer, outputOffset + i);

                bytesDecrypted += output.Length;
            }

            return bytesDecrypted;
        }
        private int CTR(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            var bytesEncrypted = 0;

            if (_parallel)
            {
                Parallel.For(0, inputCount / InputBlockSize, (i) =>
                {
                    byte[] input = new byte[InputBlockSize];
                    Array.Copy(inputBuffer, inputOffset + i * InputBlockSize, input, 0, InputBlockSize);

                    byte[] iv = new byte[InputBlockSize];
                    byte[] counterBytes = BitConverter.GetBytes(counter + i);
                    Array.Copy(_iv, 0, iv, 0, 12);
                    Array.Copy(counterBytes, 0, iv, 12, 4);

                    byte[] key = (byte[])_key.Clone();

                    byte[] output = AesTransformation.EncryptBlock(iv, key);

                    for (int j = 0; j < input.Length; j++)
                    {
                        output[j] ^= input[j];
                    }

                    if ((i + 1) * InputBlockSize >= inputCount)
                    {
                        output = RemovePadding(output);
                    }

                    output.CopyTo(outputBuffer, outputOffset + i * InputBlockSize);
                    bytesEncrypted += output.Length;
                });
                counter = inputCount / InputBlockSize;
            }
            else
            {
                byte[] key = (byte[])_key.Clone();
                for (int i = 0; i < inputCount; i += InputBlockSize)
                {
                    byte[] input = new byte[InputBlockSize];
                    Array.Copy(inputBuffer, inputOffset + i, input, 0, InputBlockSize);

                    byte[] iv = new byte[InputBlockSize];
                    byte[] counterBytes = BitConverter.GetBytes(counter);
                    Array.Copy(_iv, 0, iv, 0, 12);
                    Array.Copy(counterBytes, 0, iv, 12, 4);

                    byte[] output = AesTransformation.EncryptBlock(iv, key);

                    for (int j = 0; j < input.Length; j++)
                    {
                        output[j] ^= input[j];
                    }

                    if (i + InputBlockSize >= inputCount)
                    {
                        output = RemovePadding(output);
                    }

                    output.CopyTo(outputBuffer, outputOffset + i);
                    bytesEncrypted += output.Length;
                    counter += 1;
                }
            }

            return bytesEncrypted;
        }

        private static byte[] RemovePadding(byte[] output)
        {
            int k = output.Length - 1;
            while (k >= 0 && output[k] == 0) { k--; }
            if (k >= 0 && output[k] == 0x80)
            {
                output = output.Take(k).ToArray();
            }
            return output;
        }
    }
}
