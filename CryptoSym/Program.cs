using CryptoSym.AES;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CryptoSym
{
    class Program
    {
        static void RunCheck(AesContext context, byte[] bytes, byte[] cipher, bool parallel = false)
        {
            Stopwatch sw = new Stopwatch();
            Console.WriteLine("Encryption: ");
            sw.Start();
            var encryptedBytes = context.EncryptBytes(bytes, cipher, parallel);
            sw.Stop();
            Console.WriteLine($"Time: {sw.ElapsedMilliseconds} ms.");
            Console.WriteLine("Decryption: ");
            sw.Restart();
            var roundTrip = context.DecryptBytes(bytes, cipher, parallel);
            sw.Stop();
            Console.WriteLine($"Time: {sw.ElapsedMilliseconds} ms.");
        }

        static void PrintReport(string fileName)
        {
            if (!File.Exists(fileName)) return;

            var fileInfo = new FileInfo(fileName);
            Console.WriteLine($"File: {fileInfo.Name}\nLength: {fileInfo.Length} bytes\n");

            var cipherKey128 = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f };
            var cipherKey192 = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, };
            var cipherKey256 = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f };


            var fileBytes = File.ReadAllBytes(fileName);
            // AES128
            Console.WriteLine("\n\tAES128\n");
            RunCheck(AES.AesContext.AES128, fileBytes, cipherKey128);
            Console.WriteLine("\n\tAES128 - Parallel\n");
            RunCheck(AES.AesContext.AES128, fileBytes, cipherKey128, true);

            // AES192
            Console.WriteLine("\n\tAES192\n");
            RunCheck(AES.AesContext.AES192, fileBytes, cipherKey192);
            Console.WriteLine("\n\tAES192 - Parallel\n");
            RunCheck(AES.AesContext.AES192, fileBytes, cipherKey192, true);
            // AES128
            Console.WriteLine("\n\tAES256\n");
            RunCheck(AES.AesContext.AES256, fileBytes, cipherKey256);
            Console.WriteLine("\n\tAES256- Parallel\n");
            RunCheck(AES.AesContext.AES256, fileBytes, cipherKey256, true);
        }


        static void Print(byte[] bytes)
        {
            for (int j = 0; j < 4; j++)                
            {
                for (int i = 0; i < 4; i++)
                {
                    Console.Write("{0:X} ", bytes[i * 4 + j]);
                }
                Console.WriteLine();
            }

            Console.WriteLine();
        }

        static void PrintPlain(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                Console.Write("{0:X} ", bytes[i]);
            }

            Console.WriteLine();
        }
        
        static void Main(string[] args)
        {
            var inputFile = "1gb.test";
            PrintReport(inputFile);
        }
    }
}
