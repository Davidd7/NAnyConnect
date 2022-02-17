using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NAnyConnect_test1
{
    /*
     
     */
    internal class PasswordManager
    {

        public static string GetPassword(int id)
        {
            string[] res = File.ReadAllLines($"obscure{id}.txt");
            //return ProtectedData.Unprotect(Encoding.UTF8.GetBytes(res[0]), Encoding.UTF8.GetBytes(res[1]), DataProtectionScope.CurrentUser).ToString() ?? "";
            byte[] pwd_byte = ProtectedData.Unprotect( res[0].Split('-').Select(b => Convert.ToByte(b, 16)).ToArray(), res[1].Split('-').Select(b => Convert.ToByte(b, 16)).ToArray(), DataProtectionScope.CurrentUser)/*.ToString() ?? ""*/; // https://stackoverflow.com/questions/1230303/bitconverter-tostring-in-reverse
            string pwd = Encoding.UTF8.GetString(pwd_byte);//.ToString();   //Split('-').Select(b => Convert.ToByte(b, 16)).ToArray();
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


    }
}
