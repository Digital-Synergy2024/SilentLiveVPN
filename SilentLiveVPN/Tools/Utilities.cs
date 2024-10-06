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
using Newtonsoft.Json.Linq;
using System.Threading;
using Timer = System.Windows.Forms.Timer;
using Newtonsoft.Json;
using System.Net;

namespace SilentLiveVPN
{
    public class Utilities
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        OpenVPNConnector connector = new OpenVPNConnector();
        public static Silent silent = new Silent();
        public static bool isConnected = false;
 
        public string ExternalIP = "";
        public static bool instancethere = true;
        public static PerformanceCounterCategory performanceCounterCategory = new PerformanceCounterCategory("Network Interface");

        private PerformanceCounter bytesSentCounter;
        private PerformanceCounter bytesReceivedCounter;
        private string instance;
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
            public static bool OpenVPN { get; set; }
            public static bool Rasdial { get; set; }
            public static bool SoftEther { get; set; }
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


        public async Task InitializeTimerAsync(Chart chart1, Label lblBytesSent, Label lblBytesReceived, Label WifiName, Label label1, Label label3, Label label5, RichTextBox richTextBoxGeo)
        {

            InitializePerformanceCounters();

            // GeoData Timer
            variables.updateGeoData.Interval = 90000; // Update every 90 seconds
            variables.updateGeoData.Tick += async (sender, e) =>
            {
                try
                {
                    await UpdateGeoDataTimer_TickAsync(sender, e, richTextBoxGeo, label1);
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., log them)
                    //Console.WriteLine($"Error in GeoData Timer: {ex.Message}");
                }
            };
            variables.updateGeoData.Start();

            // General Timer
            variables.updateTimer.Interval = 5000; // Update every 5 seconds
            variables.updateTimer.Tick += async (sender, e) =>
            {
                try
                {
                    await UpdateTimer_TickAsync(sender, e, chart1, lblBytesSent, lblBytesReceived, WifiName, label1, label3, label5);
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., log them)
                    //Console.WriteLine($"Error in General Timer: {ex.Message}");
                }
            };
            variables.updateTimer.Start();
        }


        public void UpdateUI(string latitude, string longitude, string country, string city, string state, RichTextBox richTextBoxGeo)
        {

            if (richTextBoxGeo.IsHandleCreated)
            {
                richTextBoxGeo.Invoke((MethodInvoker)async delegate {
                    richTextBoxGeo.Clear(); // Clear previous text

                    // Set the first part of the text to red
                    richTextBoxGeo.SelectionColor = Color.White;
                    richTextBoxGeo.AppendText($"GeoLocation: ");

                    // Set the second part of the text to blue
                    richTextBoxGeo.SelectionColor = Color.Red;
                    richTextBoxGeo.AppendText($"{latitude} / {longitude}");

                    richTextBoxGeo.SelectionColor = Color.White;
                    richTextBoxGeo.AppendText($"\nLocation: ");

                    // Set the second part of the text to blue
                    richTextBoxGeo.SelectionColor = Color.Red;
                    richTextBoxGeo.AppendText($"Country: {country} / City: {city} / ST: {state}");

                    // Reset selection color to default
                    richTextBoxGeo.SelectionColor = richTextBoxGeo.ForeColor;
                });
            }

        }

        private const string GeoPluginUrl = "http://www.geoplugin.net/json.gp?ip=";
        public async Task ReadGeoDataAsync(RichTextBox richTextBoxGeo)
        {

            try
            {
                string requestUrl = $"{GeoPluginUrl}{ExternalIP}";

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(requestUrl);
                    response.EnsureSuccessStatusCode();

                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JObject readGeoData = JObject.Parse(jsonResponse);

                    // Extract geo-location details
                    string latitude = readGeoData["geoplugin_latitude"]?.ToString() ?? "N/A";
                    string longitude = readGeoData["geoplugin_longitude"]?.ToString() ?? "N/A";
                    string country = readGeoData["geoplugin_countryName"]?.ToString() ?? "N/A";
                    string city = readGeoData["geoplugin_city"]?.ToString() ?? "N/A";
                    string state = readGeoData["geoplugin_regionCode"]?.ToString() ?? "N/A";

                    // Update UI elements safely
                    UpdateUI(latitude, longitude, country, city, state, richTextBoxGeo);
                }
            }
            catch (HttpRequestException httpEx)
            {                
                //await connector.AppendTextToOutput($"Error retrieving geo-location data: {httpEx.Message}", Silent.listBoxOutPut);
                if (richTextBoxGeo.IsHandleCreated)
                {
                    richTextBoxGeo.Invoke((MethodInvoker)async delegate {
                        richTextBoxGeo.Clear(); // Clear previous text

                        // Set the first part of the text to red
                        richTextBoxGeo.SelectionColor = Color.White;
                        richTextBoxGeo.AppendText($"Error Trying to Get GeoLocation: {httpEx.Message}" );

                        // Reset selection color to default
                        richTextBoxGeo.SelectionColor = richTextBoxGeo.ForeColor;
                    });
                }
            }
        }

        public async Task UpdateGeoDataTimer_TickAsync(object sender, EventArgs e, RichTextBox richTextBoxGeo, Label label1) {

            await ReadGeoDataAsync(richTextBoxGeo);
            await CallUpdateContextMenuAsync();
            await GetExternalIpAsync(label1);
        }

        public void InitializePerformanceCounters()
        {
            instance = GetNetworkInstance();

            if (!string.IsNullOrEmpty(instance))
            {
                bytesSentCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", instance);
                bytesReceivedCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", instance);
            }
            else
            {
                // Handle the case where no instance is found
                throw new InvalidOperationException("No valid network instance found.");
            }
        }

        private string GetNetworkInstance()
        {
            string[] instances = performanceCounterCategory.GetInstanceNames();
            return instances.Length > 0 ? instances[0] : string.Empty; // Return the first instance or empty
        }

        public async Task UpdateTimer_TickAsync(object sender, EventArgs e, Chart chart1, Label lblBytesSent, Label lblBytesReceived, Label WifiName, Label label1, Label label3, Label label5)
        {
            if (bytesSentCounter == null || bytesReceivedCounter == null)
            {
                
                WifiName.BeginInvoke((MethodInvoker)delegate { WifiName.Text = "Performance counters not initialized."; });
                return;
            }

            // Update UI with network adapter information
            UpdateNetworkAdapterInfo(WifiName);

            for (int i = 0; i < 10; ++i)
            {
                float bytesSent = bytesSentCounter.NextValue();
                float bytesReceived = bytesReceivedCounter.NextValue();

                lblBytesSent.BeginInvoke((MethodInvoker)async delegate { lblBytesSent.Text = $"Bytes Sent: {bytesSent / 1024} KB"; });
                lblBytesReceived.BeginInvoke((MethodInvoker)async delegate { lblBytesReceived.Text = $"Bytes Received: {bytesReceived / 1024} KB"; });
                chart1.BeginInvoke((MethodInvoker)async delegate {
                    chart1.Series["Bytes Sent"].Points.AddXY(i, bytesSent / 1024);
                    chart1.Series["Bytes Received"].Points.AddXY(i, bytesReceived / 1024);
                });

                await Task.Delay(100); // Optional delay to prevent UI freezing
            }

            LoadAuthList(label3, label5);
        }

        private void UpdateNetworkAdapterInfo(Label WifiName)
        {
            string ethernetAdapterName = string.Empty;
            string wifiAdapterName = string.Empty;

            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet && adapter.OperationalStatus == OperationalStatus.Up)
                {
                    ethernetAdapterName = adapter.Name;
                }
                else if (adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && adapter.OperationalStatus == OperationalStatus.Up)
                {
                    wifiAdapterName = adapter.Name;
                }
            }

            WifiName.BeginInvoke((MethodInvoker)async delegate {
                WifiName.Text = !string.IsNullOrEmpty(ethernetAdapterName) ? $"Ethernet Adapter Name: {ethernetAdapterName}" :
                !string.IsNullOrEmpty(wifiAdapterName) ? $"WiFi Adapter Name: {wifiAdapterName}" :
                "No Active Network Adapters";
            });

        }
        
        
        private static readonly HttpClient client = new HttpClient();

        public static async Task<string> GetExternalIpAddressAsync()
        {
            string primaryApiUrl = "https://api.ipify.org";
            string secondaryApiUrl = "https://api.myip.com";

            try
            {
                // Attempt to get the IP address from the primary API
                string ipAddress = await GetIpAddressFromApi(primaryApiUrl);
                return ipAddress;
            }
            catch (HttpRequestException ex) 
            {
                // If we receive a 429 status code, switch to the secondary API
                try
                {
                    string ipAddress = await GetIpAddressFromApi(secondaryApiUrl);
                    return ipAddress;
                }
                catch (Exception secondaryEx)
                {
                    // Log the error from the secondary API
                    //Console.WriteLine($"Secondary API failed: {secondaryEx.Message}");
                    string ipAddress = "Error";
                    return ipAddress;
                }
            }
            catch (Exception ex)
            {
                // Log any other unexpected errors
                //Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                string ipAddress = "Error";
                return ipAddress;
            }
        }

        private static async Task<string> GetIpAddressFromApi(string apiUrl)
        {
            // Make the HTTP request to the specified API
            HttpResponseMessage response = await client.GetAsync(apiUrl);

            // Check if the response is successful
            if (response.IsSuccessStatusCode)
            {
                // Read and return the response content
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                // Throw an exception if the response indicates an error
                //throw new HttpRequestException($"Error fetching IP address from {apiUrl}. Status code: {response.StatusCode}", null);
                return null;
            }
        }

        public static List<IPAddress> ReadIpAddressesFromFile(string filePath)
        {
            List<IPAddress> ipAddresses = new List<IPAddress>();

            try
            {
                // Read all lines from the specified file
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    // Trim whitespace and check if the line is not empty
                    string trimmedLine = line.Trim();
                    if (!string.IsNullOrEmpty(trimmedLine))
                    {
                        // Validate the IP address
                        if (IPAddress.TryParse(trimmedLine, out IPAddress ipAddress))
                        {
                            ipAddresses.Add(ipAddress);
                        }
                        else
                        {
                            MessageBox.Show($"Invalid IP address found: {trimmedLine}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"Error: The file '{filePath}' was not found. {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Error: Access to the file '{filePath}' is denied. {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return ipAddresses;
        }

        public async Task GetExternalIpAsync(Label label1) {
            using (HttpClient client = new HttpClient()) {
                try {                   
                    ExternalIP = await client.GetStringAsync("http://api.ipify.org");
                    label1.Text = $"Your External IP: {ExternalIP}";
                    
                    // Overwrite the file with an empty string
                    File.WriteAllText("ip.txt", string.Empty);

                    // Save the IP address to the specified text file
                    using (StreamWriter writer = new StreamWriter("ip.txt", true)) {
                        
                        await writer.WriteLineAsync($"{ExternalIP}");
                    }

                }
                catch (Exception ex) {
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
                
            }
            catch (Exception)
            {
                
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

        public async Task Connect(ListBox listBox1, Label label1, Label label2, Label label3, Label label5, ListBox listBox2, RadioButton radioButton1, RadioButton radioButton2, RadioButton radioButton3) {

            try {
                //Utilities.isConnected = true;
                if (Variables.OpenVPN)
                {
                    await connector.AppendTextToOutput("Connecting...", listBox2);
                    await GetExternalIpAsync(label1);
                    //await CallUpdateContextMenuAsync();
                    await OpenVPNConnector.ConnecttoOpenVPN(listBox2, label2);
                }
                else if (Variables.Rasdial)
                {
                    await connector.AppendTextToOutput("Connecting...", listBox2);
                    LoadAuthList(label3, label5);
                    await RasDialManager.ConnectToRasVPN("Silent_VPN", label3.Text, label5.Text, listBox2);
                    await GetExternalIpAsync(label1);
                    //await CallUpdateContextMenuAsync();
                }
                else if (Variables.SoftEther) {

                    LoadAuthList(label3, label5);
                    await shell.SendCmd($"vpncmd", $"/CLIENT 127.0.0.1 /CMD AccountRetrySet {label3.Text} /NUM:0 /INTERVAL:5", "", listBox2);
                    await shell.SendCmd($"vpncmd", $"/CLIENT 127.0.0.1 /CMD AccountConnect  {label3.Text}", "", listBox2);
                    await GetExternalIpAsync(label1);
                    //await CallUpdateContextMenuAsync();
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

                if(Variables.OpenVPN)
                {
                    await TerminateProcess(variables.ProcessName);
                    label1.Text = "No Connection";
                    await GetExternalIpAsync(label1);
                    //await CallUpdateContextMenuAsync();
                }
                else if (Variables.Rasdial)
                {
                    await RasDialManager.DisconnectFromRas(listBox2);
                    await GetExternalIpAsync(label1);
                    //await CallUpdateContextMenuAsync();
                }
                else if (Variables.SoftEther)
                {
                    LoadAuthList(label3, label5);
                    await shell.SendCmd($"vpncmd", $"/CLIENT 127.0.0.1 /CMD AccountDisconnect {label3.Text}", "", listBox2);
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
