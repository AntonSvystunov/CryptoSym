using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CryptoSym.RC4
{
    public abstract class RC4ContextBase
    {
        public abstract uint BlockSize { get; }
        public ulong Boolean => (ulong)Math.Pow(2, 8 * BlockSize);

        public abstract byte[] Encrypt(byte[] data, byte[] key);
        public abstract byte[] Decrypt(byte[] data, byte[] key);
        public ICryptoTransform GetTransform(byte[] key) => new RC4Transform(this, key);
    }
}
