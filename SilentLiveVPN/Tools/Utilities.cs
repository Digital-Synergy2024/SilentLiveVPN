using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.IO;
using System.Drawing;

namespace SilentLiveVPN
{
    public class Utilities
    {
        public class Variables
        {
            public Timer updateTimer;
            private string wifiRouter;
            public float bytesSent;
            public float bytesReceived;
            public int counter = 0;
            //public bool openVPN;
            //public bool rasdial;
            public string processName = "openvpn";
            string vpnNameToCheck = "Silent_VPN";
            string selectedVPN;

            public bool OpenVPN { get; set; }
            public bool Rasdial { get; set; }


            public Variables()
            {
                updateTimer = new Timer();
            }

            public string SelectedVPN
            {
                get { return selectedVPN; }
                set { selectedVPN = value; }
            }

            public string VpnNameToCheck
            {
                get { return vpnNameToCheck; }
                set { vpnNameToCheck = value; }
            }

            public string ProcessName
            {
                get { return processName; }
                set { processName = value; }
            }

            public string WifiRouter
            {
                get { return wifiRouter; }
                set { wifiRouter = value; }
            }

            public float BytesSent
            {
                get { return bytesSent; }
                set { bytesSent = value; }
            }

            public float BytesReceived
            {
                get { return bytesReceived; }
                set { bytesReceived = value; }
            }

            public int Counter
            {
                get { return counter; }
                set { counter = value; }
            }

            /*public bool OpenVPN
            {
                get { return openVPN; }
                set { openVPN = value; }
            }

            public bool Rasdial
            {
                get { return rasdial; }
                set { rasdial = value; }
            }*/
        }

        private Variables variables;

        public Utilities()
        {
            variables = new Variables();
        }


        // Define an enumeration for OpenVPN states
        public enum OpenVPNState
        {
            Disabled,
            Enabled
        }


        // Class containing the method to toggle OpenVPN
        public class OpenVPNManager
        {
            private Variables variables;

            public OpenVPNManager(Variables vars)
            {
                variables = vars;
            }

            public void ToggleOpenVPN()
            {
                // Convert the boolean state to an enum
                OpenVPNState state = variables.OpenVPN ? OpenVPNState.Enabled : OpenVPNState.Disabled;

                switch (state)
                {
                    case OpenVPNState.Disabled:
                        variables.OpenVPN = true; // Enable OpenVPN
                        variables.Rasdial = false; // Disable Rasdial
                                                   // Uncomment the line below to show a message
                        MessageBox.Show("OpenVPN Enabled! Press connect");
                        break;

                    case OpenVPNState.Enabled:
                        variables.OpenVPN = false; // Disable OpenVPN
                        variables.Rasdial = true;  // Uncomment the line below to show a message
                        MessageBox.Show("OpenVPN Disabled!");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("Unexpected OpenVPN state");
                }
            }
        }

        public void InitializeTimer(Chart chart1, Label lblBytesSent, Label lblBytesReceived, Label WifiName, Label label1, Label label3, Label label5)
        {
            variables.updateTimer.Interval = 1000; // Update every second
            variables.updateTimer.Tick += async (sender, e) => await UpdateTimer_TickAsync(sender, e, chart1, lblBytesSent, lblBytesReceived, WifiName, label1, label3, label5);
            variables.updateTimer.Start();
        }

        public async Task UpdateTimer_TickAsync(object sender, EventArgs e, Chart chart1, Label lblBytesSent, Label lblBytesReceived, Label WifiName, Label label1, Label label3, Label label5)
        {
            if (chart1.Enabled)
            {
                PerformanceCounterCategory performanceCounterCategory = new PerformanceCounterCategory("Network Interface");
                string instance = performanceCounterCategory.GetInstanceNames()[1]; // 1st NIC
                variables.WifiRouter = "";

                foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && adapter.OperationalStatus == OperationalStatus.Up)
                    {
                        variables.WifiRouter = adapter.Name;
                        break; // Exit the loop once we find the first active Wi-Fi adapter
                    }
                }

                PerformanceCounter bytesSentCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", instance);
                PerformanceCounter bytesReceivedCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", instance);

                for (int i = 0; i < 10; ++i)
                {
                    variables.bytesSent = bytesSentCounter.NextValue();
                    variables.bytesReceived = bytesReceivedCounter.NextValue();
                    lblBytesSent.Text = $"Bytes Sent: {variables.bytesSent / 1024}";
                    lblBytesReceived.Text = $"Bytes Received: {variables.bytesReceived / 1024}";
                    WifiName.Text = "WiFi Adapter Name: " + variables.WifiRouter;
                    chart1.Series["Bytes Sent"].Points.AddXY(variables.counter++, variables.bytesSent / 1024);
                    chart1.Series["Bytes Received"].Points.AddXY(variables.counter++, variables.bytesReceived / 1024);
                }
                await GetExternalIpAsync(label1);
                LoadAuthList(label3, label5);
            }
        }

        public async Task GetExternalIpAsync(Label label1)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string ip = await client.GetStringAsync("http://api.ipify.org");
                    label1.Text = $"Your External IP: {ip}";

                }
                catch (Exception ex)
                {
                    label1.Text = $"Error: {ex.Message}";
                }
            }
        }


        public void LoadDataIntoListBox(ListBox listBox1)
        {
            try
            {
                // Specify the path to your text file
                string filePath = "vpns.txt";

                // Read all lines from the text file
                string[] lines = File.ReadAllLines(filePath);

                // Clear existing items in the ListBox
                listBox1.Items.Clear();

                // Add each line to the ListBox
                foreach (string line in lines)
                {
                    listBox1.Items.Add(line);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while reading the file: " + ex.Message);
            }
        }

        public void LoadAuthList(Label label3, Label label5)
        {

            // Define the path to the text file
            string filePath = "auth.txt";

            // Initialize a list to hold the data
            List<string> items = new List<string>();

            try
            {
                // Read all lines from the file and store them in the list
                items.AddRange(File.ReadAllLines(filePath));
                foreach (var item in items)
                {

                }
                label3.Text = $"{items[0]}";
                label5.Text = $"{items[1]}";
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Error: The file '{filePath}' was not found. {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }

        public async Task TerminateProcess(string processName)
        {
            try
            {
                Process[] processes = Process.GetProcessesByName(processName);
                if (processes.Length == 0)
                {
                    MessageBox.Show($"No process found with the name: {processName}");
                    return;
                }
                foreach (var process in processes)
                {
                    process.Kill();
                }
                MessageBox.Show($"{processName} has been terminated.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        public async Task SaveUserInputToFile(string input1, string input2)
        {
            // Define the file path where the data will be saved
            string filePath = "auth.txt";
            File.WriteAllText(filePath, string.Empty);
            try
            {
                // Use StreamWriter to write to the file
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    // Write the inputs to the file
                    writer.WriteLine($"{input2}");
                    writer.WriteLine($"{input1}");
                    //writer.WriteLine("----------"); // Separator for clarity
                }

                // Notify the user that the data has been saved
                MessageBox.Show("Data saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                // Handle any errors that may occur during file writing
                MessageBox.Show($"An error occurred while saving data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadGiphyImage(PictureBox pictureBox1)
        {
            // Path to your Giphy.webp file
            string imagePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\giphy.webp";

            // Load the image using System.Drawing
            using (var image = Image.FromFile(imagePath))
            {
                // Set the PictureBox image
                pictureBox1.Image = new Bitmap(image);
            }
        }

        public async Task CallUpdateContextMenuAsync()
        {
            // Assuming Silent is a class that has been instantiated
            Silent silentInstance = new Silent();

            // Call the UpdateContextMenu method
            silentInstance.UpdateContextMenu();

            // If you need to perform additional asynchronous operations, you can await them here
            await Task.Delay(100); // Simulating an asynchronous operation
        }

        public async Task Connect(ListBox listBox1, Label label1, Label label2, Label label3, Label label5, ListBox listBox2) {

            variables.SelectedVPN = listBox1.SelectedItem.ToString();
            if (variables.OpenVPN)
            {
                await OpenVPNConnector.ConnecttoOpenVPN(listBox2, label2);
                await GetExternalIpAsync(label1);
                await CallUpdateContextMenuAsync();
                MessageBox.Show("OpenVPN Connected!", "Message Box", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (variables.Rasdial)
            {
                LoadAuthList(label3, label5);
                await RasDialManager.ConnectToRasVPN("Silent_VPN", label3.Text, label5.Text, listBox2);
                await GetExternalIpAsync(label1);
            }
        }

        public async Task DisConnect(ListBox listBox2, Label label1, Label label2)
        {
            if (variables.OpenVPN)
            {
                await TerminateProcess(variables.ProcessName);
                label2.Text = "No Connection";
                await GetExternalIpAsync(label1);
                await CallUpdateContextMenuAsync();
            }
            else if (variables.Rasdial)
            {
                await RasDialManager.DisconnectFromRas(listBox2);
                await GetExternalIpAsync(label1);
                await CallUpdateContextMenuAsync();
            }

        }
    }
}
