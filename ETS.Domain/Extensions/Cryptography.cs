using ETS.Domain.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ETS.Domain.Extensions
{
    public class Cryptography
    {
        public class CharGenerator
        {
            private static readonly List<string> templates = new List<string> { "12345678890", "123456678890abcdefghijklmnopqrstuvwxyz", "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ", "ABCDEFGHIJKLMNOPQRSTUVWXYZ", "abcdefghijklmnopqrstuvwxyz" };

            public static string genID(int size, CharacterSet type)
            {
                char[] generated = new char[size];
                char[] characters = templates[(int)type].ToCharArray();
                int sampleLength = characters.Length - 1;
                var random = new Random();

                for (int i = 0; i < size; i++)
                {
                    int index = random.Next(0, sampleLength);
                    generated[i] = characters[index];
                }

                return new string(generated);
            }
        }

        public class AES : IDisposable
        {
            private readonly CipherMode mode;
            private readonly PaddingMode padding;
            private readonly int keySize;
            private readonly int blockSize;
            private Aes? AesObj;
            private readonly string SystemKey = "5ec89d79032c6430351b64911ceab673651888d88edc8efae41877e726b1b6c0";

            public AES(int blockSize = 128, int keySize = 128, CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7)
            {
                this.blockSize = blockSize;
                this.keySize = keySize;
                this.padding = padding;
                this.mode = mode;
            }

            private void initAes(byte[] keyBytes, byte[] ivbytes)
            {
                Dispose();
                AesObj = Aes.Create();
                AesObj.KeySize = keySize;
                AesObj.BlockSize = blockSize;
                AesObj.IV = ivbytes;
                AesObj.Key = keyBytes;
                AesObj.Mode = mode;
                AesObj.Padding = padding;
            }
            public void setKeys(string key, string? IV = null)
            {
                IV = string.IsNullOrEmpty(IV) ? key : IV;
                int keyByteSize = this.keySize / 8;
                var keyBytes = new byte[keyByteSize];
                var IVBytes = new byte[keyByteSize];
                var secretKeyBytes = Encoding.UTF8.GetBytes(key);
                var IVBkeyytes = Encoding.UTF8.GetBytes(IV);
                Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
                Array.Copy(IVBkeyytes, keyBytes, Math.Min(IVBkeyytes.Length, IVBytes.Length));
                initAes(secretKeyBytes, IVBkeyytes);
            }
            public void setKeys(byte[] keyBytes, byte[] IvBytes)
            {

                int keyByteSize = this.keySize / 8;
                if (IvBytes == null)
                {
                    IvBytes = new byte[keyByteSize];
                }
                initAes(keyBytes, IvBytes);
            }
            public byte[] encrypt(byte[] data)
            {
                if (AesObj is null)
                    throw new Exception("Key is missing");
                byte[]? encrypted = null;
                ICryptoTransform encryptor = AesObj.CreateEncryptor(AesObj.Key, AesObj.IV);
                using (MemoryStream mstream = new MemoryStream())
                {
                    using (CryptoStream csstream = new CryptoStream(mstream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swriter = new StreamWriter(csstream))
                        {
                            var str = Encoding.Default.GetString(data);
                            swriter.Write(str);
                        }
                        encrypted = mstream.ToArray();
                    }
                }
                return encrypted;
            }
            public string decrypt(byte[] data)
            {
                if (AesObj is null)
                    throw new Exception("Key is missing");
                string? decrypted = null;
                ICryptoTransform decryptor = AesObj.CreateDecryptor(AesObj.Key, AesObj.IV);
                using (MemoryStream mstream = new MemoryStream(data))
                {
                    using (CryptoStream csstream = new CryptoStream(mstream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sreader = new StreamReader(csstream))
                        {
                            decrypted = sreader.ReadToEnd();
                        }
                    }
                }
                return decrypted;
            }
            public string? encrypt(string plainText)
            {
                try
                {
                    var plainBytes = Encoding.UTF8.GetBytes(plainText);
                    var t = encrypt(plainBytes);
                    return Convert.ToBase64String(t);
                }
                catch
                {
                    return null;
                }
            }

            public string? encrypt<T>(T obj) where T : class
            {
                try
                {
                    string plainText = JObject.FromObject(obj).ToString();
                    return encrypt(plainText);
                }
                catch
                {
                    return null;
                }
            }
            public string? decrypt(string encryptedText)
            {
                try
                {
                    var encryptedBytes = Convert.FromBase64String(encryptedText);
                    var result = decrypt(encryptedBytes);
                    return result;
                }
                catch
                {
                    return null;
                }
            }
            public void Dispose()
            {
                try
                {
                    AesObj?.Dispose();
                }
                catch { }
            }
        }

        public static string ComputeHmacSha512(string plainText, string key, bool toLowerHex = true)
        {
            // Convert strings into byte arrays using UTF-8 encoding
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] messageBytes = Encoding.UTF8.GetBytes(plainText);

            // Initialize the HMACSHA512 instance with the secret key
            using (var hmac = new HMACSHA512(keyBytes))
            {
                // Compute the hash value
                byte[] hashBytes = hmac.ComputeHash(messageBytes);

                // Option A: Return as Hexadecimal format (e.g., a03f...)
                if (toLowerHex)
                {
                    return Convert.ToHexString(hashBytes).ToLowerInvariant();
                }

                // Option B: Return as Base64 format (e.g., qR3x...)
                return Convert.ToBase64String(hashBytes);
            }
        }


    }
}
