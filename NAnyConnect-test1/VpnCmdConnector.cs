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
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public static async void Connect(string vpnExecutableLocation, string fileContent)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                StreamWriter writer = new StreamWriter("test.txt");
                using (writer)
                {
                    writer.Write(fileContent);
                }

                string strCmdText = $"/C {vpnExecutableLocation} -s < test.txt";
                await Process.Start("CMD.exe", strCmdText).WaitForExitAsync();
                //System.Threading.Thread.Sleep(1000);

                File.Delete("test.txt");
            }
            finally {
                semaphoreSlim.Release();
            }
        }

        public static async void Disconnect(string vpnExecutableLocation)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                string strCmdText = $"/C {vpnExecutableLocation} disconnect";
                await Process.Start("CMD.exe", strCmdText).WaitForExitAsync();
                //System.Threading.Thread.Sleep(1000);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

    }
}
