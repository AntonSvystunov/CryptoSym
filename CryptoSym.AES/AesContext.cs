using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CryptoSym.AES
{
    public class AesContext
    {
        public enum AES
        {
            AES128 = 128,
            AES192 = 192,
            AES256 = 256
        };

        public int KeySize { get; }
        public int BlockSize { get; } = AesTransformation.Nb;

        public delegate void AESLogHandler(string message);
        public event AESLogHandler OnDataLogged;

        public AesContext(AES type = AES.AES128)
        {
            KeySize = GetKeyLength(type);
        }
        
        public byte[] Encrypt(byte[] plainText, byte[] chipherKey)
        {
            int Nr = KeySize + BlockSize + 2;

            byte[] state = (byte[])plainText.Clone();
            uint[] schedule = AesTransformation.KeyExpansion(chipherKey);
            Log(0, "input", state);
            AesTransformation.AddRoundKey(state, schedule.Take(BlockSize).ToArray());
            Log(0, "k_sch", schedule.Take(BlockSize).ToArray());

            for (int round = 1; round < Nr; round++)
            {
                Log(round, "start", state);
                AesTransformation.SubBytes(state);
                Log(round, "s_box", state);
                AesTransformation.ShiftRows(state);
                Log(round, "s_row", state);
                AesTransformation.MixColumns(state);
                Log(round, "m_col", state);
                AesTransformation.AddRoundKey(state, schedule.Skip(round * BlockSize).Take(BlockSize).ToArray());
                Log(round, "k_sch", schedule.Skip(round * BlockSize).Take(BlockSize).ToArray());
            }

            AesTransformation.SubBytes(state);
            Log(10, "s_box", state);
            AesTransformation.ShiftRows(state);
            Log(10, "s_row", state);
            AesTransformation.AddRoundKey(state, schedule.Skip(Nr * BlockSize).Take(BlockSize).ToArray());
            Log(10, "k_sch", schedule.Skip(Nr * BlockSize).Take(BlockSize).ToArray());

            return state;
        }

        public byte[] Decrypt(byte[] chipherText, byte[] chipherKey)
        {
            int Nb = AesTransformation.Nb;
            int Nr = KeySize + Nb + 2;

            byte[] state = (byte[])chipherText.Clone();
            uint[] schedule = AesTransformation.KeyExpansion(chipherKey);
            Log(0, "iinput", state);
            AesTransformation.AddRoundKey(state, schedule.Skip(Nr * Nb).Take(Nb).ToArray());
            Log(0, "ik_sch", schedule.Skip(Nr * Nb).Take(Nb).ToArray());

            for (int round = Nr - 1; round > 0 ; round--)
            {
                Log(round, "istart", state);

                AesTransformation.InvShiftRows(state);
                Log(round, "is_row", state);

                AesTransformation.InvSubBytes(state);
                Log(round, "is_box", state);

                AesTransformation.AddRoundKey(state, schedule.Skip(round * Nb).Take(Nb).ToArray());
                Log(round, "ik_sch", schedule.Skip(round * Nb).Take(Nb).ToArray());

                AesTransformation.InvMixColumns(state);
                Log(round, "im_col", state);
            }

            AesTransformation.InvShiftRows(state);
            Log(10, "is_row", state);
            AesTransformation.InvSubBytes(state);
            Log(10, "is_box", state);
            AesTransformation.AddRoundKey(state, schedule.Take(Nb).ToArray());
            Log(10, "ik_sch", schedule.Take(Nb).ToArray());

            return state;
        }

        public byte[] EncryptStringToBytes(string plainText, byte[] key, bool parallel = false)
        {
            if (key.Length != 4 * KeySize) throw new ArgumentException("Key is not supported");

            byte[] encrypted;
            ICryptoTransform encryptor = CreateEncryptor(key, parallel);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }

            return encrypted;
        }

        public byte[] EncryptBytes(byte[] plainText, byte[] key, bool parallel = false)
        {
            if (key.Length != 4 * KeySize) throw new ArgumentException("Key is not supported");

            byte[] encrypted;
            ICryptoTransform encryptor = CreateEncryptor(key, parallel);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(plainText);
                    csEncrypt.FlushFinalBlock();
                    encrypted = msEncrypt.ToArray();
                }
            }

            return encrypted;
        }


        public string DecryptStringFromBytes(byte[] cipherText, byte[] key, bool parallel = false)
        {
            if (key.Length != 4 * KeySize) throw new ArgumentException("Key is not supported");

            string plaintext = null;

            ICryptoTransform decryptor = CreateDecryptor(key, parallel);

            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }

            return plaintext;
        }

        public byte[] DecryptBytes(byte[] cipherText, byte[] key, bool parallel = false)
        {
            if (key.Length != 4 * KeySize) throw new ArgumentException("Key is not supported");

            byte[] plaintext = null;

            ICryptoTransform decryptor = CreateDecryptor(key, parallel);

            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    int read = 0;
                    byte[] buffer = new byte[1559937];

                    int chunk;
                    while ((chunk = csDecrypt.Read(buffer, read, buffer.Length - read)) > 0)
                    {
                        read += chunk;

                        if (read == buffer.Length)
                        {
                            int nextByte = csDecrypt.ReadByte();
                            if (nextByte == -1)
                            {
                                return buffer;
                            }
                            byte[] newBuffer = new byte[buffer.Length * 2];
                            Array.Copy(buffer, newBuffer, buffer.Length);
                            newBuffer[read] = (byte)nextByte;
                            buffer = newBuffer;
                            read++;
                        }
                    }

                    plaintext = new byte[read];
                    Array.Copy(buffer, plaintext, read);
                }
            }

            return plaintext;
        }

        public static AesContext AES128 => new AesContext(AES.AES128);
        public static AesContext AES192 => new AesContext(AES.AES192);
        public static AesContext AES256 => new AesContext(AES.AES256);
        public ICryptoTransform CreateEncryptor(byte[] key, bool parallel = false) => new AesEncryptor(key, this, parallel);
        public ICryptoTransform CreateDecryptor(byte[] key, bool parallel = false) => new AesDecryptor(key, this, parallel);

        private static int GetKeyLength(AES type)
        {
            return (int)type / 32;
        }

        private void Log(int round, string step, byte[] bytes)
        {
            /*var sb = new StringBuilder();
            sb.Append($"round[{round}].{step}: ");

            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(string.Format("{0:X} ", bytes[i]));
            }

            OnDataLogged?.Invoke(sb.ToString());*/
        }

        private void Log(int round, string step, uint[] nums)
        {
            /*var sb = new StringBuilder();
            sb.Append($"round[{round}].{step}: ");
            var bytes = nums.Select(n => BitConverter.GetBytes(n)).ToArray();

            for (int i = 0; i < bytes.Length; i++)
            {
                for (int j = 0; j < bytes[i].Length; j++)
                    sb.Append(string.Format("{0:X} ", bytes[i][j]));
            }

            OnDataLogged?.Invoke(sb.ToString());*/
        }
    }
}
