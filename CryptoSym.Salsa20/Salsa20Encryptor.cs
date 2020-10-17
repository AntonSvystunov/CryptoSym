using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CryptoSym.Salsa20
{
    public class Salsa20Encryptor : ICryptoTransform
    {
        protected uint[] _state;
        protected uint _rounds;
        
        public Salsa20Encryptor(byte[] key, byte[] iv, uint rounds)
        {
            if (key.Length != 16 && key.Length != 32)
            {
                throw new ArgumentException($"Key length not supported: {key.Length}");
            }

            if (iv.Length < 8)
            {
                throw new ArgumentException($"Invalid initialization vector size: {iv.Length}");
            }

            _rounds = rounds;
            _state = Salsa20Transformation.CreateInitialState(key, iv);
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            byte[] tempBuffer = new byte[64];
            int encryptedBytes = 0;

            while (inputCount > 0)
            {
                Salsa20Transformation.SalsaBlock(tempBuffer, _state, _rounds);
                Salsa20Transformation.IncrementSalsaState(_state);

                int blockSize = Math.Min(InputBlockSize, inputCount);
                for (int i = 0; i < blockSize; i++)
                {
                    outputBuffer[outputOffset + i] = (byte)(inputBuffer[inputOffset + i] ^ tempBuffer[i]);
                }

                encryptedBytes += blockSize;

                inputCount -= InputBlockSize;
                outputOffset += InputBlockSize;
                inputOffset += InputBlockSize;
            }

            return encryptedBytes;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            byte[] output = new byte[inputCount];
            TransformBlock(inputBuffer, inputOffset, inputCount, output, 0);
            return output;
        }

        public bool CanReuseTransform => false;

        public bool CanTransformMultipleBlocks => true;

        public int InputBlockSize => 16 * sizeof(uint);

        public int OutputBlockSize => 16 * sizeof(uint);

        public void Dispose()
        {
            if (_state != null)
            {
                Array.Clear(_state, 0, _state.Length);
            }
            _state = null;
        }
    }
}
