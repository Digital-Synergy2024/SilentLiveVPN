using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace SilentLiveVPN
{
    public class shell
    {

        internal static async Task SendCmd(string vpncmdPath, string command, string command2, ListBox listBox2)
        {
            OpenVPNConnector connector = new OpenVPNConnector();
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = vpncmdPath,
                    Arguments = command,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    Verb = "runas",
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            listBox2.Invoke((MethodInvoker)async delegate
                            {
                                await connector.AppendTextToOutput(e.Data, listBox2);
                            });
                        }
                    };

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            listBox2.Invoke((MethodInvoker)async delegate
                            {
                                await connector.AppendTextToOutput("Error: " + e.Data, listBox2);
                            });
                        }
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    // Await the process completion in a non-blocking manner
                    await Task.Run(() => process.WaitForExit());
                }
            }
            catch (Exception ex)
            {
                listBox2.Invoke((MethodInvoker)async delegate
                {
                    await connector.AppendTextToOutput($"An error occurred: {ex.Message} - {ex.StackTrace}", listBox2);
                });
            }
        }

    }
}



/*internal static async Task SendCmd(string vpncmdPath, string command, string command2, ListBox listBox2)
{
    OpenVPNConnector connector = new OpenVPNConnector();
    try {

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = vpncmdPath,
            Arguments = command,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            Verb = "runas",
            CreateNoWindow = true
        };


        using (Process process = new Process { StartInfo = startInfo })
        {

            process.OutputDataReceived += async (sender, e) =>
            {
                try {

                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        await connector.AppendTextToOutput(e.Data, listBox2);
                    }
                }
                catch (Exception ex)
                {
                    // Handle the exception appropriately
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            };

            process.ErrorDataReceived += async (sender, e) =>
            {
                try {

                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        await connector.AppendTextToOutput("Error: " + e.Data, listBox2);

                    }
                }
                catch (Exception ex)
                {
                    // Handle the exception appropriately
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await connector.AppendTextToOutput($"An error occurred: {ex.Message} - {ex.StackTrace}", listBox2);
                }
            };
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            //OpenVPNConnector.AppendTextToOutput("VPN connection started. Press any key to stop...", listBox2);
            //Console.ReadKey();

            //process.Kill(); // Terminate the process when done
        }
    }
    catch (Exception ex)
    {
        await connector.AppendTextToOutput($"An error occurred: {ex.Message} - {ex.StackTrace}", listBox2);
    }
}
}*/

