using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAnyConnect_test1
{
    internal class Options
    {
        private static Dictionary<string, string> options;

        public static Dictionary<string, string> GetOptions() {
            if (options != null) {
                return options;
            }
            if (File.Exists("n_any_connect_options"))
            {
                string fileContent = File.ReadAllText("n_any_connect_options");  //File.ReadAllLines($"n_any_connect_options");
                options = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(fileContent);
                return options;
            }
            options = new Dictionary<string, string>();
            return options;
        }


        public static void Save()
        {
            string res = System.Text.Json.JsonSerializer.Serialize(GetOptions());
            File.WriteAllText("n_any_connect_options", res);
        }


        public static string VpnExecutableLocation
        {
            get {
                string res;
                bool success = GetOptions().TryGetValue("vpn_executable_location", out res);
                if (!success) {
                    res = "\"C:\\Program Files (x86)\\Cisco\\Cisco AnyConnect Secure Mobility Client\\vpncli.exe\"";
                }
                return res;
            }
            set {
                GetOptions()["vpn_executable_location"] = value;
                Save();
            }
        }


        public static bool ReconnectAfterSleep
        {
            get
            {
                string res;
                bool success = GetOptions().TryGetValue("reconnect_after_sleep", out res);
                if (!success)
                {
                    return true;
                }
                return bool.Parse(res);
            }
            set
            {
                GetOptions()["reconnect_after_sleep"] = value.ToString();
                Save();
            }
        }



        /*public static string[] DictToStringArray(this string str, Dictionary<string, string> dict)
        {
            return dict.Select(x => "[" + x.Key + " " + x.Value + "]").ToArray(); // https://stackoverflow.com/questions/3067282/how-to-write-the-content-of-a-dictionary-to-a-text-file/3067294
        }
        public static string[] StringArrayToDict(this string str, Dictionary<string, string> dict)
        {
            return dict.Select(x => "[" + x.Key + " " + x.Value + "]").ToArray(); // https://stackoverflow.com/questions/3067282/how-to-write-the-content-of-a-dictionary-to-a-text-file/3067294
        }*/

    }



}





