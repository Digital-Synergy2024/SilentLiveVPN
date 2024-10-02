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
using static SilentLiveVPN.Tools.ipFecthing;
using Newtonsoft.Json.Linq;
using System.Threading;
using Timer = System.Windows.Forms.Timer;

namespace SilentLiveVPN
{
    public class Utilities
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        OpenVPNConnector connector = new OpenVPNConnector();
        public static Silent silent = new Silent();
        public static Tools.GeoLocationChecker GeoA = new Tools.GeoLocationChecker();
        public static bool isConnected = false;
        public bool OpenVPN = true;
        public bool Rasdial = true;
        public bool SoftEther = true;
        public class Variables
        {
            public Timer updateTimer;
            public Timer updateGeoData;
            public float bytesSent;
            public float bytesReceived;
            public int counter = 0;
            public string processName = "openvpn";
            public string username;
            public string nic;

            public Variables()
            {
                updateTimer = new Timer();
                updateGeoData = new Timer();
            }

            public string SelectedVPN { get; set; }

            public string VpnNameToCheck { get; set; } = "Silent_VPN";

            public string ProcessName
            {
                get { return processName; }
                set { processName = value; }
            }

            public string WifiRouter { get; set; }

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

        }

        private Variables variables;

        public Utilities()
        {
            variables = new Variables();
        }

        public void StopTimers()
        {
            variables.updateTimer.Stop();
            variables.updateGeoData.Stop();
            variables.updateTimer.Dispose();
            variables.updateGeoData.Dispose();
        }


        public void InitializeTimer(Chart chart1, Label lblBytesSent, Label lblBytesReceived, Label WifiName, Label label1, Label label3, Label label5)
        {

            /*GeoData*/
            variables.updateGeoData.Interval = 10000; // Update 
            variables.updateGeoData.Tick += async (sender, e) => await UpdateGeoDataTimer_TickAsync(sender, e);
            variables.updateGeoData.Start();

            variables.updateTimer.Interval = 1000; // Update every second
            variables.updateTimer.Tick += async (sender, e) => await UpdateTimer_TickAsync(sender, e, chart1, lblBytesSent, lblBytesReceived, WifiName, label1, label3, label5);
            variables.updateTimer.Start();
        }

        public async Task CheckIfVPNConnectedAsync() {

            try {
                //await connector.AppendTextToOutput($"Starting IPFetcher 3", Silent.listBoxOutPut);
                //await VpnDetection.IsUserConnectedToVpnAsync();
                //await Silent.IsUserConnectedToVpnAsync();

            } catch(Exception ex) {

                MessageBox.Show($"An error occurred GeoLocation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public async Task UpdateGeoDataTimer_TickAsync(object sender, EventArgs e) {

            var token = cts.Token;
            //_= Task.Run(async () => await ReadGeoDataAsync(token));
            await ReadGeoDataAsync(token);
        }

        public async Task UpdateTimer_TickAsync(object sender, EventArgs e, Chart chart1, Label lblBytesSent, Label lblBytesReceived, Label WifiName, Label label1, Label label3, Label label5)
        {
            PerformanceCounterCategory performanceCounterCategory = new PerformanceCounterCategory("Network Interface");
            bool instancethere = true;
            string instance2 = performanceCounterCategory.GetInstanceNames().Length > 0 ? performanceCounterCategory.GetInstanceNames()[0] : string.Empty;
            string instance = performanceCounterCategory.GetInstanceNames().Length > 0 ? performanceCounterCategory.GetInstanceNames()[1] : string.Empty;
            if (string.IsNullOrEmpty(instance))
            {
                WifiName.Text = "No Network Interface Found";
                return;
            }

            string ethernetAdapterName = string.Empty;
            string wifiAdapterName = string.Empty;

            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet && adapter.OperationalStatus == OperationalStatus.Up)
                {
                    ethernetAdapterName = adapter.Name;
                    instancethere = false;
                }
                else if (adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && adapter.OperationalStatus == OperationalStatus.Up)
                {
                    wifiAdapterName = adapter.Name;
                }
            }

            WifiName.Text = !string.IsNullOrEmpty(ethernetAdapterName) ? $"Ethernet Adapter Name: {ethernetAdapterName}" :
                            !string.IsNullOrEmpty(wifiAdapterName) ? $"WiFi Adapter Name: {wifiAdapterName}" :
                            "No Active Network Adapters";

            PerformanceCounter bytesSentCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", instancethere ? instance : instance2);
            PerformanceCounter bytesReceivedCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", instancethere ? instance : instance2);

            for (int i = 0; i < 10; ++i)
            {
                variables.bytesSent = bytesSentCounter.NextValue();
                variables.bytesReceived = bytesReceivedCounter.NextValue();
                lblBytesSent.Text = $"Bytes Sent: {variables.bytesSent / 1024} KB";
                lblBytesReceived.Text = $"Bytes Received: {variables.bytesReceived / 1024} KB";
                chart1.Series["Bytes Sent"].Points.AddXY(variables.counter++, variables.bytesSent / 1024);
                chart1.Series["Bytes Received"].Points.AddXY(variables.counter++, variables.bytesReceived / 1024);

            }

            await GetExternalIpAsync(label1);
            LoadAuthList(label3, label5);
        }

        public async Task ReadGeoDataAsync(CancellationToken token)
        {
            string filePath = "geodata.json";

            using (StreamReader reader = new StreamReader(filePath))
            {
                string existingJson = await reader.ReadToEndAsync();

                JArray readGeoDataArray = JArray.Parse(existingJson);
                JObject readGeoData = (JObject)readGeoDataArray[0];
                // Parse the JSON data
                //JObject readGeoData = JObject.Parse(existingJson);
                string latitude = readGeoData["geoplugin_latitude"].ToString();
                string longitude = readGeoData["geoplugin_longitude"].ToString();
                string country = readGeoData["geoplugin_countryName"].ToString();
                string city = readGeoData["geoplugin_city"].ToString();
                string state = readGeoData["geoplugin_regionCode"].ToString();

                // Update UI elements safely
                UpdateUI(latitude, longitude, country, city, state);
            }

        }

        private void UpdateUI(string latitude, string longitude, string country, string city, string state)
        {
            if (Silent.labelGeoA.IsHandleCreated)
            {
                Silent.labelGeoA.BeginInvoke((MethodInvoker)delegate {
                    Silent.labelGeoA.Text = $"GeoLocation: {latitude} / {longitude}";
                    Silent.labelGeoA.ForeColor = Color.FromArgb(255, 0, 0);
                });
            }

            if (Silent.labelGeoB.IsHandleCreated)
            {
                Silent.labelGeoB.BeginInvoke((MethodInvoker)delegate {
                    Silent.labelGeoB.Text = $"Country: {country} / City: {city} / ST: {state}";
                    Silent.labelGeoB.ForeColor = Color.FromArgb(255, 0, 0);
                });
            }

            UpdateConnectionStatus();
        }

        private void UpdateConnectionStatus()
        {
            if (Silent.OpenVPNlblA.IsHandleCreated)
            {
                Silent.OpenVPNlblA.BeginInvoke((MethodInvoker)delegate {
                    Silent.OpenVPNlblA.Text = OpenVPN ? "Connected" : "Disconnected";
                    Silent.OpenVPNlblA.ForeColor = OpenVPN ? Color.FromArgb(28, 168, 25) : Color.FromArgb(255, 0, 0);
                });
            }

            if (Silent.RadiallblA.IsHandleCreated)
            {
                Silent.RadiallblA.BeginInvoke((MethodInvoker)delegate {
                    Silent.RadiallblA.Text = Rasdial ? "Connected" : "Disconnected";
                    Silent.RadiallblA.ForeColor = Rasdial ? Color.FromArgb(28, 168, 25) : Color.FromArgb(255, 0, 0);
                });
            }

            if (Silent.SoftlblA.IsHandleCreated)
            {
                Silent.SoftlblA.BeginInvoke((MethodInvoker)delegate {
                    Silent.SoftlblA.Text = SoftEther ? "Connected" : "Disconnected";
                    Silent.SoftlblA.ForeColor = SoftEther ? System.Drawing.Color.FromArgb(28, 168, 25) : System.Drawing.Color.FromArgb(255, 0, 0);
                });
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
                MessageBox.Show("An error occurred while reading the file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
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
            catch (FileNotFoundException )
            {
                //Console.WriteLine($"Error: The file '{filePath}' was not found. {ex.Message}");
            }
            catch (Exception )
            {
                //Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }

        public async Task TerminateProcess(string processName)
        {
            try
            {
                // Use Task.Run to execute the blocking operation on a separate thread
                await Task.Run(() =>
                {
                    Process[] processes = Process.GetProcessesByName(processName);
                    if (processes.Length == 0)
                    {
                        // Use a more appropriate way to notify the user in an async context
                        throw new InvalidOperationException($"No process found with the name: {processName}");
                    }
                    foreach (var process in processes)
                    {
                        process.Kill();
                    }
                });

                // Notify the user after the processes have been terminated
                MessageBox.Show($"{processName} has been terminated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (InvalidOperationException ex)
            {
                // Handle specific exceptions related to process termination
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception ex)
            {
                // Handle any other exceptions that may occur
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task SaveUserInputToFileAsync(string input1, string input2)
        {
            // Define the file path where the data will be saved
            string filePath = "auth.txt";
            File.WriteAllText(filePath, string.Empty);
            try
            {
                // Use StreamWriter to write to the file asynchronously
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {

                    // Write the inputs to the file asynchronously
                    await writer.WriteLineAsync(input2);
                    await writer.WriteLineAsync(input1);
                    // await writer.WriteLineAsync("----------"); // Uncomment for a separator
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

            silentInstance.removeConectMenuItem();

            // Call the UpdateContextMenu method
            silentInstance.UpdateContextMenu();

            // If you need to perform additional asynchronous operations, you can await them here
            await Task.Delay(100); // Simulating an asynchronous operation
        }
        //public static Silent silent = new Silent();


        public async Task Connect(ListBox listBox1, Label label1, Label label2, Label label3, Label label5, ListBox listBox2, RadioButton radioButton1, RadioButton radioButton2, RadioButton radioButton3) {

            try {
                //Utilities.isConnected = true;
                if (radioButton1.Checked)
                {
                    await connector.AppendTextToOutput("Connecting...", listBox2);
                    await GetExternalIpAsync(label1);
                    //await GeoA.StartTimer();
                    await CallUpdateContextMenuAsync();
                    await OpenVPNConnector.ConnecttoOpenVPN(listBox2, label2);

                    //OpenVPN = false;
                }
                else if (radioButton2.Checked)
                {
                    await connector.AppendTextToOutput("Connecting...", listBox2);
                    LoadAuthList(label3, label5);
                    await RasDialManager.ConnectToRasVPN("Silent_VPN", label3.Text, label5.Text, listBox2);
                    await GetExternalIpAsync(label1);
                    await CallUpdateContextMenuAsync();
                    //Rasdial = false;
                }
                else if (radioButton3.Checked) {

                    LoadAuthList(label3, label5);
                    await shell.SendCmd($"vpncmd", $"/CLIENT 127.0.0.1 /CMD AccountRetrySet {label3.Text} /NUM:0 /INTERVAL:5", "", listBox2);
                    await shell.SendCmd($"vpncmd", $"/CLIENT 127.0.0.1 /CMD AccountConnect  {label3.Text}", "", listBox2);
                    await CallUpdateContextMenuAsync();
                    //SoftEther = false; 
                }
                
            }
            catch (Exception ex)
            {
                // Handle any errors that may occur during file writing
                MessageBox.Show($"An error occurred while Connecting: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }           
        }

        public async Task DisConnect(ListBox listBox2, Label label1, Label label2, Label label3, Label label5, RadioButton radioButton1, RadioButton radioButton2, RadioButton radioButton3)
        {
            try {

                if (radioButton1.Checked)
                {
                    await TerminateProcess(variables.ProcessName);
                    //label1.Text = "No Connection";
                    await GetExternalIpAsync(label1);
                    await CallUpdateContextMenuAsync();
                    //OpenVPN = false;
                }
                else if (radioButton2.Checked)
                {
                    await RasDialManager.DisconnectFromRas(listBox2);
                    await GetExternalIpAsync(label1);
                    await CallUpdateContextMenuAsync();
                    //Rasdial = false;
                }
                else if (radioButton3.Checked)
                {
                    LoadAuthList(label3, label5);
                    await shell.SendCmd($"vpncmd", $"/CLIENT 127.0.0.1 /CMD AccountDisconnect {label3.Text}", "", listBox2);
                    //SoftEther = true;
                }
            }
            catch (Exception ex)
            {
                // Handle any errors that may occur during file writing
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }          
        }
    }
}
