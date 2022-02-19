using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NAnyConnect_test1
{

    /// <summary>
    /// Class for storing and retrieving passwords.
    /// Please note: This is not more secure than saving the password in plain text in a batch file.
    /// The password might be encrypted via the ProtectedData-class, but it's decrypted during the execution of the program (which is started without authentication), temporarily stored in strings (which is a security risk) and passed to the cmd in plain text.
    /// </summary>
    internal class PasswordManager
    {

        public static string GetPassword(int id)
        {
            string[] res = File.ReadAllLines($"obscure{id}.txt");
            byte[] pwd_byte = ProtectedData.Unprotect(ByteArrayStringToByteArray(res[0]), ByteArrayStringToByteArray(res[1]), DataProtectionScope.CurrentUser);
            string pwd = Encoding.UTF8.GetString(pwd_byte);
            return pwd;
        }

        public static void SetPassword(int id, ref string pwd) {

            // Data to protect. Convert a string to a byte[] using Encoding.UTF8.GetBytes().
            byte[] plaintext = Encoding.UTF8.GetBytes(pwd);

            // Generate additional entropy (will be used as the Initialization vector)
            byte[] entropy = new byte[20];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(entropy);
            }

            byte[] ciphertext = ProtectedData.Protect(plaintext, entropy, DataProtectionScope.CurrentUser);

            string fileName = $"obscure{id}.txt";
            StreamWriter writer = new StreamWriter(fileName);
            using (writer)
            {
                writer.WriteLine(BitConverter.ToString( ciphertext ));
                writer.WriteLine(BitConverter.ToString( entropy ));
            }
        }

        private static Byte[] ByteArrayStringToByteArray(string str) {
            return str.Split('-').Select(b => Convert.ToByte(b, 16)).ToArray(); // https://stackoverflow.com/questions/1230303/bitconverter-tostring-in-reverse
        }


    }
}
