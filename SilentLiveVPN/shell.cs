using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
namespace SilentLiveVPN
{
    public class shell
    {

        internal static void SendCmd(string vpncmdPath, string command, string command2, ListBox listBox2)
        {

            try {

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = vpncmdPath,
                    Arguments = command,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    Verb = "runas",
                    CreateNoWindow = false
                };


                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            OpenVPNConnector.AppendTextToOutput(e.Data, listBox2);
                        }
                    };

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            OpenVPNConnector.AppendTextToOutput("Error: " + e.Data, listBox2);
                        }
                    };
                    process.StartInfo = startInfo;
                    process.Start();
                    process.WaitForExit();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    //OpenVPNConnector.AppendTextToOutput("VPN connection started. Press any key to stop...", listBox2);
                    //Console.ReadKey();

                    process.Kill(); // Terminate the process when done
                }
            }
            catch (Exception ex)
            {
                OpenVPNConnector.AppendTextToOutput("An error occurred: " + ex.Message, listBox2);
            }



        }
    }
}
// Start the process
/*using (var process = new Process())
{

                    // Create a process start info
                var startInfo = new ProcessStartInfo
                {
                    FileName = vpncmdPath,
                    Arguments = command,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    Verb = "runas",
                    CreateNoWindow = false
                };


    process.StartInfo = startInfo;
    process.Start();
    string output = process.StandardOutput.ReadToEnd();
    string error = process.StandardError.ReadToEnd();
    process.WaitForExit();


    OpenVPNConnector.AppendTextToOutput("Output: " + output, listBox2);

    if (process.ExitCode != 0)
    {
        OpenVPNConnector.AppendTextToOutput($"Error. Exit code: {process.ExitCode}", listBox2);
        OpenVPNConnector.AppendTextToOutput("Error: " + error, listBox2);
    }

}*/
