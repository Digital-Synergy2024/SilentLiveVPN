using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Net.Sockets;
using System.Text;
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

        public string GetEncMethodHttp()
        {
            // Define the OpenVPN executable path
            string openVpnPath = @"C:\Program Files\OpenVPN\bin\openvpn.exe";
            string encryptionMethod = "";

            // Create a new process to run OpenVPN
            Process openVpnProcess = new Process();
            openVpnProcess.StartInfo.FileName = openVpnPath;
            openVpnProcess.StartInfo.Arguments = $"--config \"{configFilePath}\" --show-ciphers"; // Adjust arguments as needed
            openVpnProcess.StartInfo.UseShellExecute = false;
            openVpnProcess.StartInfo.RedirectStandardOutput = true;
            openVpnProcess.StartInfo.RedirectStandardError = true;
            openVpnProcess.StartInfo.CreateNoWindow = true;

            try
            {
                // Start the OpenVPN process
                openVpnProcess.Start();

                // Read the output
                string output = openVpnProcess.StandardOutput.ReadToEnd();
                string error = openVpnProcess.StandardError.ReadToEnd();

                // Wait for the process to exit
                openVpnProcess.WaitForExit();

                // Check for errors
                if (!string.IsNullOrEmpty(error))
                {
                    return "Error: " + error;
                }

                // Process the output to find the cipher and auth method
                string cipher = ExtractCipher(output);
                string auth = ExtractAuth(output);

                // Write the results to a text file
                WriteToFile("encryption_info.txt", cipher, auth);
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }

            return encryptionMethod;
        }

        string ExtractCipher(string output)
        {
            // Logic to extract the cipher from the output
            var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (line.StartsWith("Available ciphers:"))
                {
                    // Start extracting ciphers
                    int index = Array.IndexOf(lines, line) + 1;
                    while (index < lines.Length && !lines[index].StartsWith("Available authentication methods:"))
                    {
                        if (!string.IsNullOrWhiteSpace(lines[index]))
                        {
                            return lines[index].Trim(); // Return the first cipher found
                        }
                        index++;
                    }
                }
            }
            return "No cipher found";
        }

        string ExtractAuth(string output)
        {
            // Logic to extract the auth method from the output
            var lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (line.StartsWith("Available authentication methods:"))
                {
                    // Start extracting authentication methods
                    int index = Array.IndexOf(lines, line) + 1;
                    while (index < lines.Length)
                    {
                        if (!string.IsNullOrWhiteSpace(lines[index]))
                        {
                            return lines[index].Trim(); // Return the first auth method found
                        }
                        index++;
                    }
                }
            }
            return "No auth method found";
        }

        void WriteToFile(string filePath, string cipher, string auth)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"Cipher: {cipher}");
                writer.WriteLine($"Auth: {auth}");
            }
        }


        public string GetEncryptionMethod()
        {
            //to use enable management 127.0.0.1 7505 on OpenVPn
            const string Host = "51.79.52.118";
            const int Port = 7505;
            using (TcpClient client = new TcpClient(Host, Port))
            using (NetworkStream stream = client.GetStream())
            using (StreamReader reader = new StreamReader(stream))
            using (StreamWriter writer = new StreamWriter(stream) { AutoFlush = true })
            {
                // Send the status command
                writer.WriteLine("status");
                string response;
                StringBuilder sb = new StringBuilder();

                // Read the response until we find the encryption method
                while ((response = reader.ReadLine()) != null)
                {
                    sb.AppendLine(response);
                    if (response.Contains("Cipher")) // Look for the line containing the encryption method
                    {
                        return response; // Return the line with the encryption method
                    }
                }
            }
            return "Encryption method not found.";
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

        public async Task StartConnection(string openVpnPath, string authFilePath, ListBox listBox2)
        {
            try
            {
                string encryptionMethod = GetEncryptionMethod(configFilePath);

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = openVpnPath,
                    Arguments = $"--config \"{configFilePath}\" --auth-user-pass \"{authFilePath}\" --data-ciphers {encryptionMethod}",
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
                            listBox2.Invoke((MethodInvoker)async delegate
                            {
                                await AppendTextToOutput(e.Data, listBox2);
                            });
                        }
                    };

                    process.ErrorDataReceived += async (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            listBox2.Invoke((MethodInvoker)async delegate
                            {
                                await AppendTextToOutput("Error: " + e.Data, listBox2);
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
                    await AppendTextToOutput($"An error occurred: {ex.Message} - {ex.StackTrace}", listBox2);
                });
            }
        }


        /*public async Task StartConnection(string openVpnPath, string authFilePath, ListBox listBox2)
        {
            try
            {
                //AddAllEncryptionMethods(configFilePath);
                // Get the encryption method from the .ovpn file  GetEncMethodHttp
                string encryptionMethod = GetEncryptionMethod(configFilePath);
                //string encryptionMethod = GetEncMethodHttp();

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = openVpnPath,
                    Arguments = $"--config \"{configFilePath}\" --auth-user-pass \"{authFilePath}\" --data-ciphers {encryptionMethod}",
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
        }*/

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

        private void AddAllEncryptionMethods(string configFilePath)
        {
            // Define the ciphers and authentication methods
            string dataCiphers = "NULL-CIPHER:AES-128-CBC:AES-192-CBC:AES-256-CBC:BF-CBC:CAST-CBC:CAST5-CBC:DES-CBC:DES-EDE-CBC:DES-EDE3-CBC:DESX-CBC:RC2-40-CBC:RC2-64-CBC:RC2-CBC:CAMELLIA-128-CBC:CAMELLIA-192-CBC:CAMELLIA-256-CBC";
            string authMethods = "SHA:SHA1:SHA256:SHA384:SHA512:MD5:MD4:RMD160";

            try
            {
                // Read the existing content of the OVPN file
                string[] lines = File.ReadAllLines(configFilePath);
                using (StreamWriter writer = new StreamWriter(configFilePath))
                {
                    foreach (string line in lines)
                    {
                        writer.WriteLine(line);
                    }
                    // Add the --data-ciphers and --auth lines at the end
                    writer.WriteLine($"data-ciphers {dataCiphers}");
                    writer.WriteLine($"auth {authMethods}");
                }
                Console.WriteLine("Successfully added --data-ciphers and --auth to the OVPN file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }


        private string GetEncryptionMethod(string configFilePath)
        {
            string encryptionMethod = string.Empty;

            // Read all lines from the .ovpn file
            var lines = File.ReadAllLines(configFilePath);
            foreach (var line in lines)
            {
                // Check if the line contains the cipher directive
                if (line.StartsWith("cipher", StringComparison.OrdinalIgnoreCase))
                {
                    // Split the line to get the encryption method
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 1)
                    {
                        encryptionMethod = parts[1]; // The second part is the encryption method
                    }
                    break; // Exit the loop once we find the cipher
                }
            }

            return encryptionMethod;
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
                MessageBox.Show("An error occurred while reading the file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }


    }


}

