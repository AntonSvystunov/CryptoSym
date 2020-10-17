using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CryptoSym.RC4
{
    public class RC4Transform: ICryptoTransform
    {
        protected readonly RC4ContextBase _context;
        protected readonly byte[] _key;

        public RC4Transform(RC4ContextBase context, byte[] key)
        {
            _context = context;
            _key = key;
        }

        public bool CanReuseTransform => false;

        public bool CanTransformMultipleBlocks => true;

        public int InputBlockSize => ushort.MaxValue;

        public int OutputBlockSize => ushort.MaxValue;

        public void Dispose()
        {
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            byte[] plainBytes = new byte[inputCount];
            Array.Copy(inputBuffer, inputOffset, plainBytes, 0, inputCount);
            byte[] encodedBytes = _context.Encrypt(plainBytes, _key);
            Array.Copy(encodedBytes, 0, outputBuffer, outputOffset, encodedBytes.Length);
            return inputCount;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            byte[] encodedBytes = new byte[inputCount];
            TransformBlock(inputBuffer, inputOffset, inputCount, encodedBytes, 0);
            return encodedBytes;
        }
    }
}
