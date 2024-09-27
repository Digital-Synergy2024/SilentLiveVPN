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

        internal async Task AppendTextToOutput(string text, ListBox listBox2)
        {
            try
            {
                // Check if we need to invoke on the UI thread
                if (listBox2.InvokeRequired)
                {

                    try {

                        // Use Task.Run to offload the work to the thread pool
                        await Task.Factory.StartNew(() =>
                        {
                            // Invoke the method on the UI thread
                            listBox2.Invoke(new Action(() => AppendTextToOutput(text, listBox2).GetAwaiter()));

                        });
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception appropriately
                        MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                else
                {
                    // Directly append text to the ListBox
                    await AppendTextToListBox(listBox2, text);
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions asynchronously
                await AppendTextToOutput("An error occurred: " + ex.Message, listBox2);
            }
        }

        private async Task AppendTextToListBox(ListBox listBox, string text)
        {

            try {
                // Simulate a delay for demonstration purposes
                await Task.Delay(100); // Simulate some asynchronous work

                // Append text to the ListBox
                listBox.Items.Add(text);
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private string configFilePath; // Path to the .ovpn configuration file
        public async Task StartConnection(string openVpnPath, string authFilePath, ListBox listBox2)
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
                    process.OutputDataReceived += async (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            await AppendTextToOutput(e.Data, listBox2);
                        }
                    };

                    process.ErrorDataReceived += async (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            await AppendTextToOutput("Error: " + e.Data, listBox2);
                        }
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    //await AppendTextToOutput("OpenVPN connection started. Press any key to stop...", listBox2);
                    //Console.ReadKey();

                    //process.Kill(); // Terminate the process when done
                }
            }
            catch (Exception ex)
            {
                await AppendTextToOutput("An error occurred: " + ex.Message, listBox2);
            }
        }

        public void OpenVPNConnect(string configFile)
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

        public static async Task ConnecttoOpenVPN(ListBox listBoxA, Label label2)
        {
            try
            {
                
                string configPath = "";
                string[] ovpnFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.ovpn");

                foreach (string filePath in ovpnFiles)
                {
                    configPath = filePath;
                }
                OpenVPNConnector vpnConnector = new OpenVPNConnector();
                vpnConnector.OpenVPNConnect(configPath);
                string vpnAddress = vpnConnector.GetOpenVPNAddress(configPath);
                label2.Text = vpnAddress;
                string openVpnPath = @"C:\Program Files\OpenVPN\bin\openvpn.exe";
                string authFileName = "auth.txt";
                string authFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, authFileName);
                await vpnConnector.StartConnection(openVpnPath, authFilePath, listBoxA);
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

