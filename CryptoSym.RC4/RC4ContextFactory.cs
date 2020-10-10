using System;

namespace CryptoSym.RC4
{
    public enum RC4BlockSize
    {
        Byte,
        Short
    }

    public static class RC4ContextFactory
    {
        public static RC4ContextBase GetContext(RC4BlockSize type)
        {
            return type switch
            {
                RC4BlockSize.Byte => new RC4Context8(),
                RC4BlockSize.Short => new RC4Context16(),
                _ => throw new NotImplementedException($"{type.ToString()} not implemented"),
            };
        }
    }
}
