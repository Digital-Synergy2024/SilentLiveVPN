using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace SilentLiveVPN
{
    public class CreateVPN
    {
        public static string serverAddress; // Replace with actual server address


        static void CombinePBKFiles(string file1, string file2, string outputFile)
        {
            HashSet<string> uniqueEntries = new HashSet<string>();

            // Read the first PBK file
            ReadPBKFile(file1, uniqueEntries);
            // Read the second PBK file
            ReadPBKFile(file2, uniqueEntries);

            // Write the combined unique entries to the output file
            WritePBKFile(outputFile, uniqueEntries);
        }

        static void ReadPBKFile(string filePath, HashSet<string> uniqueEntries)
        {
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    // Add each line to the HashSet (duplicates will be ignored)
                    uniqueEntries.Add(line.Trim());
                }
            }
            else
            {
                //Console.WriteLine($"File not found: {filePath}");
            }
        }

        static void WritePBKFile(string filePath, HashSet<string> uniqueEntries)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (string entry in uniqueEntries)
                {
                    writer.WriteLine(entry);
                }
            }
            //Console.WriteLine($"Combined PBK file created at: {filePath}");
        }

        static void checkPhoneBook()
        {

            // Define the path for the output PBK file
            string outputPbkFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                                            @"AppData\Roaming\Microsoft\Network\Connections\PBK\rasphone.pbk");

            // Check if the file exists
            if (File.Exists(outputPbkFile))
            {
                //Console.WriteLine("The file exists at the specified path.");
   
            }
            else
            {
                //Console.WriteLine("The file does not exist. Creating the file...");

                try
                {
                    // Create the directory if it does not exist
                    string directoryPath = Path.GetDirectoryName(outputPbkFile);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    // Create the file
                    using (FileStream fs = File.Create(outputPbkFile))
                    {
                        // Optionally, write some initial content to the file
                        using (StreamWriter writer = new StreamWriter(fs))
                        {
                            //writer.WriteLine("This is a newly created PBK file.");
                        }
                    }

                    //Console.WriteLine("The file has been created successfully.");
                }
                catch (Exception)
                {
                    //Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }




        static void AddToPhonebook(ListBox listBox2)
        {
            // Define the paths to the PBK files
            string pbkFile1 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                                            @"AppData\Roaming\Microsoft\Network\Connections\PBK\rasphone.pbk");
            string pbkFile2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                            @"Microsoft\Network\Connections\Pbk\rasphone.pbk");

            // Define the path for the output PBK file
            string outputPbkFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                                            @"AppData\Roaming\Microsoft\Network\Connections\PBK\rasphone.pbk");


            checkPhoneBook();

            try
            {
                // Read the contents of both PBK files
                string content1 = File.ReadAllText(pbkFile1);
                string content2 = File.ReadAllText(pbkFile2);

                // Combine the contents with a newline in between
                string combinedContent = content1 + Environment.NewLine + content2;

                // Write the combined content to the output file
                File.WriteAllText(outputPbkFile, combinedContent);
                OpenVPNConnector.AppendTextToOutput("PBK files combined successfully and saved to: " + outputPbkFile, listBox2);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred Adding Phonebook: {ex.Message}");
            }
        }

        public static void AddVPN(string ServerAddress, ListBox listBox2)
        {
            OpenVPNConnector.AppendTextToOutput("Wait a moment while the VPN is being configured...", listBox2);

            // Configure VPN using PowerShell
            string vpnCommand = $"Add-VpnConnection -Name Silent_VPN -ServerAddress {ServerAddress} -AllUserConnection -AuthenticationMethod MSChapv2 -EncryptionLevel Optional -Force -L2tpPsk pre-shared-key -PassThru -RememberCredential -TunnelType L2tp";
            ExecutePowerShellCommand(vpnCommand, listBox2);
            serverAddress = ServerAddress;

            //Thread.Sleep(10000);
            AddToPhonebook(listBox2);
            // Add route
            //string routeCommand = "route add -p 172.16.5.0 mask 255.255.255.0 192.168.198.254";
            //ExecuteCommand(routeCommand);
        }

        public static void UpdateVPN(string ServerAddress, ListBox listBox2)
        {
            OpenVPNConnector.AppendTextToOutput("Updating VPN...", listBox2);
            //Set-VpnConnection -Name "MyVPN" -ServerAddress "vpn.newserver.com"

            // Configure VPN using PowerShell
            string vpnCommand = $"Set-VpnConnection -Name Silent_VPN -ServerAddress {ServerAddress}";
            ExecutePowerShellCommand(vpnCommand, listBox2);

        }

        public static void GetVPNinfo(string ServerAddress, ListBox listBox2)
        {
            OpenVPNConnector.AppendTextToOutput("GetVPNinfo VPN info...", listBox2);
            //Set-VpnConnection -Name "MyVPN" -ServerAddress "vpn.newserver.com"

            // Configure VPN using PowerShell
            string vpnCommand = $"Get-VpnConnection";
            ExecutePowerShellCommand(vpnCommand, listBox2);

        }

        public static bool IsVpnNameTaken(string vpnName)
        {
            string command = $"Get-VpnConnection | Where-Object {{ $_.Name -eq '{vpnName}' }}";
            return ExecutePowerShellCommandA(command);
        }

        public static bool ExecutePowerShellCommandA(string command)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "PowerShell.exe",
                    Arguments = $"-Command \"{command}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    // Start the process
                    process.Start();

                    // Read the output and error streams
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    // Wait for the process to exit
                    process.WaitForExit();

                    // Check for errors
                    if (!string.IsNullOrEmpty(error))
                    {
                        MessageBox.Show($"Error occurred: {error}");
                        return false; // Indicate failure due to error
                    }

                    // If output is not empty, the command executed successfully
                    return !string.IsNullOrEmpty(output);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                return false;
            }

        }

        static void ReadConfigLog(ListBox listBox2)
        {
            string filePath = "config.log"; // Specify the path to your config file

            try
            {
                // Check if the file exists
                if (File.Exists(filePath))
                {
                    // Read all text from the file
                    string fileContent = File.ReadAllText(filePath);

                    // Output the content to the console or log it
                    //Console.WriteLine("Contents of the config file:");
                    //Console.WriteLine(fileContent);

                    // Optionally, write the output to a log file
                    //File.WriteAllText("config.log", fileContent);

                    // Assuming OpenVPNConnector is a class with a method to append text to a list box
                    OpenVPNConnector.AppendTextToOutput(fileContent, listBox2);
                }
                else
                {
                    //Console.WriteLine($"The file '{filePath}' does not exist.");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        public static void ExecutePowerShellCommand(string command, ListBox listBox2)
        {

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "PowerShell.exe",
                    Arguments = $"-Command \"{command}\"",
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

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    Console.ReadKey();

                    process.Kill(); // Terminate the process when done
                }
            }
            catch (Exception ex)
            {
                OpenVPNConnector.AppendTextToOutput("An error occurred: " + ex.Message, listBox2);
            }
        }


        public static void ExecuteCommand(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", $"/C {command}")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using (Process process = Process.Start(psi))
            {
                process.WaitForExit();
            }
        }
    }
}
