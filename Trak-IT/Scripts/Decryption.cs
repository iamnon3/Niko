using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Trak_IT.Scripts
{
    internal class Decryption
    {
        public string DecryptMain(byte[] code)
        {
            

            string password = "password";

            byte[] key = new byte[32];
            byte[] iv = new byte[16];

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                Array.Copy(hash, 0, key, 0, 32);
                Array.Copy(hash, 0, iv, 0, 16);
            }

            string decryptedPassword = Decrypt(code, key, iv);
            return decryptedPassword;
        }

        public string Decrypt(byte[] cipheredText, byte[] key, byte[] iv)
        {
            string plainText = String.Empty;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(cipheredText))
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                using (StreamReader streamReader = new StreamReader(cryptoStream))
                {
                    plainText = streamReader.ReadToEnd();
                }
            }

            return plainText;
        }


        //for encrypting
        public static string EncryptMain(string studentID)
        {
            string password = "password";

            byte[] key = new byte[32];
            byte[] iv = new byte[16];

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                Array.Copy(hash, 0, key, 0, 32);
                Array.Copy(hash, 0, iv, 0, 16);
            }

            byte[] encryptedPassword = Encrypt(studentID, key, iv);
            string encryptedPasswordString = Convert.ToBase64String(encryptedPassword);
            return encryptedPasswordString;
        }

        static byte[] Encrypt(string plainText, byte[] key, byte[] iv)
        {
            byte[] cipheredText;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(plainText);
                    }

                    cipheredText = memoryStream.ToArray();
                }
                
            }

            return cipheredText;
        }

    }
}
