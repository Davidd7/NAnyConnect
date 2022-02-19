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
        // Options-dictionary that stores global settings
        private static Dictionary<string, string> _optionsDict;
        private static Dictionary<string, string> OptionsDict {
            get
            {
                if (_optionsDict != null)
                {
                    return _optionsDict;
                }
                if (File.Exists("n_any_connect_options"))
                {
                    string fileContent = File.ReadAllText("n_any_connect_options");
                    _optionsDict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(fileContent);
                    return _optionsDict;
                }
                _optionsDict = new Dictionary<string, string>();
                return _optionsDict;
            }
        }


        // Properties of the Options-dictionary
        public static string VpnExecutableLocation
        {
            get {
                string res;
                bool success = OptionsDict.TryGetValue("vpn_executable_location", out res);
                if (!success) {
                    res = "\"C:\\Program Files (x86)\\Cisco\\Cisco AnyConnect Secure Mobility Client\\vpncli.exe\"";
                }
                return res;
            }
            set {
                OptionsDict["vpn_executable_location"] = value;
                Save();
            }
        }
        public static bool ReconnectAfterSleep
        {
            get
            {
                string res;
                bool success = OptionsDict.TryGetValue("reconnect_after_sleep", out res);
                if (!success)
                {
                    return true;
                }
                return bool.Parse(res);
            }
            set
            {
                OptionsDict["reconnect_after_sleep"] = value.ToString();
                Save();
            }
        }
        public static bool ShowCommandPromptWindows
        {
            get
            {
                string res;
                bool success = OptionsDict.TryGetValue("show_command_prompt_windows", out res);
                if (!success)
                {
                    return true;
                }
                return bool.Parse(res);
            }
            set
            {
                OptionsDict["show_command_prompt_windows"] = value.ToString();
                Save();
            }
        }




        public static void Save()
        {
            string res = System.Text.Json.JsonSerializer.Serialize(OptionsDict);
            File.WriteAllText("n_any_connect_options", res);
        }





    }



}





