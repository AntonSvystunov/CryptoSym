# CryptoSym
.NET Core implementation of AES and Kalyna symetric cryptography algrorithms.

## CryptoSym.AES

Implementation of AES based on [standard](https://nvlpubs.nist.gov/nistpubs/FIPS/NIST.FIPS.197.pdf).

The core class of assembly is **AesContext**
```c#
public class AesContext
```
**AesContext** contains implementation of all 3 types of AES which are described in [standard](https://nvlpubs.nist.gov/nistpubs/FIPS/NIST.FIPS.197.pdf):
- AES-128
- AES-192
- AES-256

For convinience, you can use static properties of **AesContext** class to get a context of desired type:
```c#
public static AesContext AES128 => new AesContext(AES.AES128);
public static AesContext AES192 => new AesContext(AES.AES192);
public static AesContext AES256 => new AesContext(AES.AES256);
```

### API
#### Simple encryption
For working with block-sized data you can use clean Encrypt and Decrypt methods of **AesContext** class:
```c#
public byte[] Encrypt(byte[] plainText, byte[] chipherKey)
```
```c#
public byte[] Decrypt(byte[] chipherText, byte[] chipherKey)
```
This methods can be used to construct some wrappers arround standart AES algorithm.
Example:
```c#
var aes = AesContext.AES128;
var encryptedBytes = aes.Encrypt(plainBytes, key);
var roundTripBytes = aes.Decrypt(encryptedBytes, key);
Console.WriteLine("{0} {1}", bytes.Length, roundTripBytes.Length);
```
#### Working with complex binary data
For encryption of complex binary data you can use EncryptBytes and DecryptBytes methods. They can be run in parallel modes to optimize calculations. Size of input plain text is unlimited. This methods can be used to work with large files.
```c#
public byte[] EncryptBytes(byte[] plainText, byte[] key, bool parallel = false)
```
```c#
public byte[] DecryptBytes(byte[] cipherText, byte[] key, bool parallel = false)
```
Example:
```c#
var aes = AesContext.AES192;
var encryptedBytes = aes.EncryptBytes(plainBytes, key);
var roundTripBytes = aes.EncryptBytes(encryptedBytes, key);
Console.WriteLine("{0} {1}", bytes.Length, roundTripBytes.Length);
```
#### Working with strings
**AesContext** class provides special methods for working with text data. 
```c#
public byte[] EncryptStringToBytes(string plainText, byte[] key, bool parallel = false)
```
```c#
public string DecryptStringFromBytes(byte[] cipherText, byte[] key, bool parallel = false)
```
Example:
```c#
var aes = AesContext.AES192;
var encryptedBytes = aes.EncryptStringToBytes(plaintext, key);
var roundTripText = aes.DecryptStringFromBytes(encryptedBytes, key);
Console.WriteLine("{0} {1}", plaintext, roundTripText);
```
#### ICryptoTransform implementations
To make **AesContext** compatible with **CryptoStream**, **AesEncryptor** and **AesDecryptor** classes-implementations of **ICryptoTransform** interface were implemented.
```c#
internal class AesDecryptor : ICryptoTransform
```
```c#
internal class AesEncryptor : ICryptoTransform
```
This classes provide stream cipher functionality for AES. Padding is implemented based on [ISO/IEC 7816-4](https://en.wikipedia.org/wiki/Padding_(cryptography)).
To create an instance on ICryptoTransform for specific **AesContext** following methods can be used:
```c#
public ICryptoTransform CreateEncryptor(byte[] key, bool parallel = false) => new AesEncryptor(key, this, parallel);
public ICryptoTransform CreateDecryptor(byte[] key, bool parallel = false) => new AesDecryptor(key, this, parallel);
```
### Implementation details
Steps of AES algorithm was moved to a static class **AesTransformation**.
```c#
public static class AesTransformation
```
Following AES transformation were implemented:
```c#
public static void SubBytes(byte[] state);
public static void InvSubBytes(byte[] state);

public static void ShiftRows(byte[] state);
public static void InvShiftRows(byte[] state);

public static void MixColumns(byte[] state);
public static void InvMixColumns(byte[] state);

public static uint[] KeyExpansion(byte[] key);
public static void AddRoundKey(byte[] state, uint[] w);
```
## Benchmark
File size: 12 kb
| Algorithm	| Encryption time (ms) | Decryption time (ms) |
|---|---|---|
| AES128 | 148 | 127 |
| AES128 - Parallel | 122	| 24 |
| AES192 | 73	| 148 |
| AES192 - Parallel | 18 | 32 |
| AES256 | 95 | 191 |
| AES256- Parallel | 19 | 42 |


File size: 1 gb
| Algorithm	| Encryption time (ms) | Decryption time (ms) |
|---|---|---|
| AES128 | 4309253 | 5189360 |
| AES128 - Parallel | 1827301	| 1727192 |
| AES192 | -	| - |
| AES192 - Parallel | - | - |
| AES256 | - | - |
| AES256- Parallel | - | - |

File size 1mb

| Algorithm	| Encryption time (ms) | Decryption time (ms) |
|---|---|---|
| Salsa20 - 128 bit key | 124 | 87 |
| Salsa20 - 256 bit key | 86 | 82 |
| --- |
| RC4 - 8 bit block - 128 bit key | 177 | 175 |
| RC4 - 8 bit block - 192 bit key | 175 | 178 |
| RC4 - 8 bit block - 256 bit key | 174 | 168 |
| RC4 - 16 bit block - 128 bit key | 177 | 175 |
| RC4 - 16 bit block - 192 bit key | 175 | 178 |
| RC4 - 16 bit block - 256 bit key | 174 | 168 |
| --- |
| ECB - AES128 | 2051 | 5832 |
| ECB - AES128 - Parallel | 614 | 953 |
| ECB - AES192 | 4094 | 7876 |
| ECB - AES192 - Parallel | 811 | 1261 |
| ECB - AES256 | 4870 | 9376 |
| ECB - AES256 - Parallel | 796 | 1446 |
| --- |
| CBC - AES128 | 3487 | 6316 |
| CBC - AES128 - Parallel | 551 | 988 |
| CBC - AES192 | 4181 | 7689 |
| CBC - AES192 - Parallel | 691 | 1601 |
| CBC - AES256 | 4670 | 9293 |
| CBC - AES256 - Parallel | 916 | 1619 |
| --- |
| CFB - AES128 | 3178 | 6668 |
| CFB - AES128 - Parallel | 553 | 1013 |
| CFB - AES192 | 4172 | 8073 |
| CFB - AES192 - Parallel | 840 | 1281 |
| CFB - AES256 | 4829 | 9469 |
| CFB - AES256 - Parallel | 826 | 1514 |
| --- |
| OFB - AES128 | 3571 | 6382 |
| OFB - AES128 - Parallel | 657 | 1265 |
| OFB - AES192 | 3964 | 8161 |
| OFB - AES192 - Parallel | 679 | 1239 |
| OFB - AES256 | 4860 | 9461 |
| OFB - AES256 - Parallel | 816 | 1841 |
| --- |
| CTR - AES128 | 3242 | 6992 |
| CTR - AES128 - Parallel | 581 | 1044 |
| CTR - AES192 | 4155 | 7783 |
| CTR - AES192 - Parallel | 846 | 1187 |
| CTR - AES256 | 4840 | 9117 |
| CTR - AES256 - Parallel | 1129 | 1459 |
