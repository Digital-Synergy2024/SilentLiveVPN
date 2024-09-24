using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SilentLiveVPN
{
    class OpenVPNConnector
    {
        private string configFilePath; // Path to the .ovpn configuration file

        public string GetOpenVPNAddress(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return "File not found.";
            }

            string[] lines = File.ReadAllLines(filePath);
            string addressLine = lines.FirstOrDefault(line => line.StartsWith("remote"));

            if (addressLine != null)
            {
                // Split the line to get the address and port
                string[] parts = addressLine.Split(' ');
                if (parts.Length >= 2)
                {
                    return $"Connected to: {parts[1]} on port {parts[2]}";
                }
            }

            return "No remote address found.";
        }

        public OpenVPNConnector(string configFile)
        {
            if (File.Exists(configFile))
            {
                configFilePath = configFile;
            }
            else
            {
                throw new FileNotFoundException("Configuration file not found.", configFile);
            }
        }

        // Helper method to append text to the ListBox safely
        internal static void AppendTextToListBox(ListBox listBox2, string text)
        {
            if (listBox2.InvokeRequired)
            {
                listBox2.Invoke(new Action(() => listBox2.Items.Add(text)));
            }
            else
            {
                listBox2.Items.Add(text);
                listBox2.TopIndex = listBox2.Items.Count - 1; // Scroll to the bottom
            }
        }

        internal static void AppendTextToOutput(string text, ListBox listBox2)
        {

            try {
                if (listBox2.InvokeRequired)
                {
                    // Correctly pass the parameters to the method
                    listBox2.Invoke(new Action(() => AppendTextToOutput(text, listBox2)));
                }
                else
                {
                    AppendTextToListBox(listBox2, text);
                }
            }
            catch (Exception ex)
            {
                AppendTextToOutput("An error occurred: " + ex.Message, listBox2);
            }

        }


        public void StartConnection(string openVpnPath, string authFilePath, ListBox listBox2)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = openVpnPath,
                    Arguments = $"--config \"{configFilePath}\" --auth-user-pass \"{authFilePath}\" ",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            AppendTextToOutput(e.Data, listBox2);
                        }
                    };

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            AppendTextToOutput("Error: " + e.Data, listBox2);
                        }
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    AppendTextToOutput("OpenVPN connection started. Press any key to stop...", listBox2);
                    Console.ReadKey();

                    process.Kill(); // Terminate the process when done
                }
            }
            catch (Exception ex)
            {
                AppendTextToOutput("An error occurred: " + ex.Message, listBox2);
            }
        }

    }
}
