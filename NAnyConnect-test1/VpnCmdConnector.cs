using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NAnyConnect_test1
{
    internal class VpnCmdConnector
    {
        private static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public static async void Connect(string vpnExecutableLocation, string fileContent)
        {
            // Using semaphoreSlim so that only one thread interacts with the vpn-cmd-interface at any moment
            await semaphoreSlim.WaitAsync();
            try
            {
                // Creating file that will be redirected to the vpn-cmd-interface
                StreamWriter writer = new StreamWriter("test.txt");
                using (writer)
                {
                    writer.Write(fileContent);
                }
                // Interacting with the vpn-cmd-interface to start a connection
                string strCmdText = $"/C {vpnExecutableLocation} -s < test.txt";
                if (Options.ShowCommandPromptWindows)
                {
                    await Process.Start("CMD.exe", strCmdText).WaitForExitAsync();
                }
                else 
                {
                    Process process = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.CreateNoWindow = true;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = strCmdText;
                    process.StartInfo = startInfo;
                    process.Start();
                    await process.WaitForExitAsync();
                }
                // Deleting the created file again
                File.Delete("test.txt");
            }
            finally {
                semaphoreSlim.Release();
            }
        }

        public static async void Disconnect(string vpnExecutableLocation)
        {
            // Using semaphoreSlim so that only one thread interacts with the vpn-cmd-interface at any moment
            await semaphoreSlim.WaitAsync();
            try
            {
                // Interacting with the vpn-cmd-interface to disconnect a possibly running connection
                string strCmdText = $"/C {vpnExecutableLocation} disconnect";
                if (Options.ShowCommandPromptWindows)
                {
                    await Process.Start("CMD.exe", strCmdText).WaitForExitAsync();
                }
                else
                {
                    Process process = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.CreateNoWindow = true;
                    startInfo.FileName = "cmd.exe";
                    startInfo.Arguments = strCmdText;
                    process.StartInfo = startInfo;
                    process.Start();
                    await process.WaitForExitAsync();
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

    }
}
