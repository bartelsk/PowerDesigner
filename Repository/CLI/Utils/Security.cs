// Code by Karlo Bartels - https://github.com/bartelsk/PowerDesigner
// Licensed under the Apache License, Version 2.0. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace PDRepository.CLI.Utils
{
    /// <summary>
    /// This class provides various security utility methods.
    /// </summary>
    internal class Security
    {
        /// <summary>
        /// Returns the value from a <see cref="SecureString"/>.
        /// </summary>
        /// <param name="value">The secure string value.</param>
        /// <returns>A string type.</returns>
        public static string SecureStringToString(SecureString value)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        /// <summary>
        /// Encrypts string data.
        /// </summary>
        /// <param name="text">The data to encrypt.</param>
        /// <returns>The encrypted string in Base64 format.</returns>
        public static string Encrypt(string text)
        {
            string keyString = EncryptionKey;
            byte[] key = Encoding.UTF8.GetBytes(keyString);
            using (Aes aesAlg = Aes.Create())
            {
                using (ICryptoTransform encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                {
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(text);
                            }
                        }
                        byte[] iv = aesAlg.IV;
                        byte[] decryptedContent = msEncrypt.ToArray();
                        byte[] result = new byte[iv.Length + decryptedContent.Length];
                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);
                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        /// <summary>
        /// Decrypts cipher data.
        /// </summary>
        /// <param name="cipherText">The data to decrypt in Base64 format.</param>
        /// <returns>The decrypted cipher text.</returns>
        public static string Decrypt(string cipherText)
        {
            string keyString = EncryptionKey;
            byte[] fullCipher = Convert.FromBase64String(cipherText);

            byte[] iv = new byte[16];
            byte[] cipher = new byte[fullCipher.Length - 16];
            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);

            byte[] key = Encoding.UTF8.GetBytes(keyString);
            using (Aes aesAlg = Aes.Create())
            {
                using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(key, iv))
                {
                    string result;
                    using (MemoryStream msDecrypt = new MemoryStream(cipher))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                    return result;
                }
            }
        }

        /// <summary>
        /// Returns the key used to encrypt and decrypt data.
        /// Ensures encrypted data can only be decrypted with the same machine/user name combination.
        /// </summary>
        private static string EncryptionKey
        {
            get
            {
                int keyLen = 32;
                string key = Environment.MachineName + Environment.UserName;
                if (key.Length > keyLen)
                {
                    key = key.Substring(0, keyLen);
                }
                else if (key.Length < keyLen)
                {
                    int len = key.Length;
                    for (int i = 0; i < keyLen - len; i++)
                    {
                        key += ((char)(65 + i)).ToString();
                    }
                }
                return key;
            }
        }
    }
}
