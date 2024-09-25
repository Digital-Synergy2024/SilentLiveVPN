using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
namespace SilentLiveVPN
{
    public class OpenVPNConnector
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


        internal static async Task ConnecttoOpenVPN(string listBoxA, Label label2)
        {
            try
            {
                string configPath = "";
                string[] ovpnFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.ovpn");
                
                foreach (string filePath in ovpnFiles)
                {
                    configPath = filePath;
                }
                OpenVPNConnector vpnConnector = new OpenVPNConnector(configPath);
                string vpnAddress = vpnConnector.GetOpenVPNAddress(configPath);
                label2.Text = vpnAddress;
                string openVpnPath = @"C:\Program Files\OpenVPN\bin\openvpn.exe";
                string authFileName = "auth.txt";
                string authFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, authFileName);
                vpnConnector.StartConnection(openVpnPath, authFilePath, listBoxA);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message, "Message Box", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }


        public static void LoanConfig(Label label4)
        {
            string directoryPath = AppDomain.CurrentDomain.BaseDirectory; // Specify your directory path
            string fileExtension = "*.ovpn"; // Specify the file extension you are looking for

            try
            {
                string[] files = Directory.GetFiles(directoryPath, fileExtension);
                if (files.Length > 0) // Check if any files were found
                {
                    // Extract and display only the file names
                    string[] fileNames = files.Select(file => Path.GetFileName(file)).ToArray();
                    label4.Text = "VPNConfig:" + string.Join(", ", fileNames); // Display the found file names
                }
                else
                {
                    label4.Text = "OVPN file not found.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }


        }

    }
}
