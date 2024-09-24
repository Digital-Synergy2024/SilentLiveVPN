using System;
using System.Collections.Generic;
//using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
//using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net.NetworkInformation;
//using System.Threading;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Net.Http;
//using System.Net.Http.Headers;
//using static SilentLiveVPN.RasDialWrapper;
//using static SilentLiveVPN.RasDialManager;
//using System.Runtime.InteropServices;
//using System.Management;
//using System.Reflection;

//[assembly: AssemblyKeyFileAttribute("silentVPN.snk")]
//[assembly: AssemblyDelaySignAttribute(true)]

namespace SilentLiveVPN
{


    public partial class Silent : Form
    {
        private System.Windows.Forms.Timer updateTimer;
        private string WifiRouter;
        private float bytesSent;
        private float bytesReceived;
        private int counter = 0;
        private bool OpenVPN;
        private bool Rasdial;
        ContextMenuStrip menu = new ContextMenuStrip { AutoClose = false };

        public Silent()
        {
            OpenVPN = false;
            InitializeComponent();
            Sunisoft.IrisSkin.SkinEngine skin = new Sunisoft.IrisSkin.SkinEngine();
            skin.SkinAllForm = true;
            // var path = "..\\..\\s\\a (1).ssk";
            skin.SkinFile = Environment.CurrentDirectory + @"\s\a (40).ssk";
            InitializeContextMenu();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            //LoadGiphyImage();
            LoadAuthList();
            this.Resize += new EventHandler(Form1_Resize);
            this.Resize += MyForm_Resize; // Subscribe to the Resize event
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            //notifyIcon1_MouseDown
            openToolStripMenuItem.Click += new EventHandler(openToolStripMenuItem_Click);
            exitToolStripMenuItem.Click += new EventHandler(exitToolStripMenuItem_Click);
            notifyIcon1.DoubleClick += new EventHandler(notifyIcon1_MouseDoubleClick);
            notifyIcon1.MouseClick += new MouseEventHandler(notifyIcon1_MouseClick);
            // Subscribe to the Resize event
            //this.Resize += MyForm_Resize;
            // Initialize event handlers
            LoadDataIntoListBox();
            chart1.Enabled = false;
            lblBytesSent.Enabled = false;
            WifiName.Enabled = false;
            lblBytesReceived.Enabled = false;
            InitializeTimer();
            CreateLineChart();
            LoanConfig();
        }

        private void MyForm_Resize(object sender, EventArgs e)
        {
            // Check if the form is minimized
            if (this.WindowState == FormWindowState.Minimized)
            {
                // Close the context menu if it is open
                if (menu.Visible)
                {
                    menu.Close(); // Hide the context menu
                }
            }
        }

        private void LoadGiphyImage()
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

        private async void Option1_ClickAsync(object sender, EventArgs e)
        {
            // Handle Option 1 click
            string selectedVPN = listBox1.SelectedItem.ToString();
            if (OpenVPN)
            {
                ConnecttoOpenVPN();
                await GetExternalIpAsync();
                UpdateContextMenu();
                MessageBox.Show("OpenVPN Connected!", "Message Box", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (Rasdial)
            {
                LoadAuthList();
                ConnectToRasVPN("Silent_VPN", label3.Text, label5.Text);
                await GetExternalIpAsync();
            }
            menu.Close();
            //MessageBox.Show("VPN Connected!", "Message Box", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void Option2_ClickAsync(object sender, EventArgs e)
        {
            // Handle Option 2 click
            if (OpenVPN)
            {

                string processName = "openvpn";
                TerminateProcess(processName);
                label2.Text = "No Connection";
                await GetExternalIpAsync();
                UpdateContextMenu();
            }
            else if (Rasdial)
            {

                DisconnectFromRas();
                await GetExternalIpAsync();
                UpdateContextMenu();
            }
            //MessageBox.Show("VPN Disconnected!", "Message Box", MessageBoxButtons.OK, MessageBoxIcon.Information);
            menu.Close();
        }

        private void Option3_Click(object sender, EventArgs e)
        {

            menu.Close();
        }

        private void Option4_Click(object sender, EventArgs e)
        {
            menu.Close();
            Application.Exit(); // Exit the application
        }

        private void InitializeContextMenu()
        {
            menu.Items.Add("Connect", null, Option1_ClickAsync);
            menu.Items.Add("Disconnect", null, Option2_ClickAsync);
            menu.Items.Add(label2.Text, null, Option3_Click);
            menu.Items.Add("Close SilentVpn", null, Option4_Click);
        }

        private void UpdateContextMenu()
        {
            // Clear existing items and re-add them to reflect the updated label text
            menu.Items.Clear();
            menu.Items.Add("Connect", null, Option1_ClickAsync);
            menu.Items.Add("Disconnect", null, Option2_ClickAsync);
            menu.Items.Add(label2.Text, null, Option3_Click);
            menu.Items.Add("Close SilentVpn", null, Option4_Click);
            notifyIcon1.Text = label2.Text;
        }

        private void removeConectMenuItem() {

            RemoveMenuItem(label2.Text);
        }

        private void RemoveMenuItem(string itemText)
        {
            ToolStripItem itemToRemove = menu.Items.Cast<ToolStripItem>()
                .FirstOrDefault(item => item.Text == itemText);

            if (itemToRemove != null)
            {
                menu.Items.Remove(itemToRemove);
            }
        }

        private void CreateLineChart()
        {
            chart1.Series.Add(new Series("Bytes Sent") { ChartType = SeriesChartType.Line });
            chart1.Series.Add(new Series("Bytes Received") { ChartType = SeriesChartType.Line });
            chart1.Titles.Add("VPN Connection Speed");
        }


        private void InitializeTimer()
        {
            updateTimer = new System.Windows.Forms.Timer();
            updateTimer.Interval = 1000; // Update every second
            updateTimer.Tick += UpdateTimer_TickAsync;
            updateTimer.Start();
        }

        private async void UpdateTimer_TickAsync(object sender, EventArgs e)
        {
            if (chart1.Enabled) {

                PerformanceCounterCategory performanceCounterCategory = new PerformanceCounterCategory("Network Interface");
                string instance = performanceCounterCategory.GetInstanceNames()[1]; // 1st NIC !
                WifiRouter = "";
                foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && adapter.OperationalStatus == OperationalStatus.Up)
                    {
                        //lblBytesSent.Text = "WiFi Adapter Name: " + adapter.Name;
                        WifiRouter = adapter.Name;
                        break; // Exit the loop once we find the first active Wi-Fi adapter
                    }
                }

                PerformanceCounter bytesSentCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", instance);
                PerformanceCounter bytesReceivedCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", instance);

                for (int i = 0; i < 10; ++i)
                {

                    bytesSent = bytesSentCounter.NextValue();
                    bytesReceived = bytesReceivedCounter.NextValue();
                    lblBytesSent.Text = $"Bytes Sent: {bytesSent / 1024}";
                    lblBytesReceived.Text = $"Bytes Received: {bytesReceived / 1024}";
                    WifiName.Text = "WiFi Adapter Name: " + WifiRouter;
                    chart1.Series["Bytes Sent"].Points.AddXY(counter++, bytesSent / 1024);
                    chart1.Series["Bytes Received"].Points.AddXY(counter++, bytesReceived / 1024);
                }
                await GetExternalIpAsync();
                LoadAuthList();
            }
 
        }

        private void LoadDataIntoListBox()
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

        private void LoanConfig()
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

        private void ConnecttoOpenVPN()
        {
            try
            {
                string configPath = "";
                string[] ovpnFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.ovpn");
                ListBox listBoxA = listBox2;
                foreach (string filePath in ovpnFiles)
                {
                    configPath = filePath;
                }
                OpenVPNConnector vpnConnector = new OpenVPNConnector(configPath);
                string vpnAddress = vpnConnector.GetOpenVPNAddress(configPath);
                label2.Text = vpnAddress;                
                string openVpnPath = @"C:\Program Files\OpenVPN\bin\openvpn.exe";
                string authFileName = "auth.txt";
                string authFilePath =Path.Combine(AppDomain.CurrentDomain.BaseDirectory, authFileName);
                vpnConnector.StartConnection(openVpnPath, authFilePath, listBoxA);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message, "Message Box", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void DisconnectFromRas()
        {

            //rasdial "MyVPN" /disconnect

            shell.SendCmd("rasdial", "Silent_VPN /disconnect", "", listBox2);
            /*int lpcb = 0;
            int lpcConnections = 0;

            // First call to RasEnumConnections to get the size needed
            RasEnumConnections(IntPtr.Zero, ref lpcb, ref lpcConnections);

            // Allocate memory for the connections
            IntPtr lpRasConn = Marshal.AllocHGlobal(lpcb);
            try
            {
                // Second call to get the actual connections
                int result = RasEnumConnections(lpRasConn, ref lpcb, ref lpcConnections);
                if (result != 0)
                {
                    OpenVPNConnector.AppendTextToOutput("Error enumerating connections: " + result, listBox2);
                    return;
                }

                // Iterate through the connections
                for (int i = 0; i < lpcConnections; i++)
                {
                    RASCONN rasConn = (RASCONN)Marshal.PtrToStructure(
                        IntPtr.Add(lpRasConn, i * Marshal.SizeOf(typeof(RASCONN))),
                        typeof(RASCONN));

                    // Disconnect the connection
                    int hangUpResult = RasHangUp(rasConn.hRasConn);
                    if (hangUpResult == 0)
                    {
                        OpenVPNConnector.AppendTextToOutput($"Successfully disconnected from {rasConn.szEntryName}", listBox2);
                    }
                    else
                    {
                      
                        OpenVPNConnector.AppendTextToOutput($"Failed to disconnect from {rasConn.szEntryName}: {hangUpResult}", listBox2);
                    }
                }
            }
            finally
            {
                // Free the allocated memory
                Marshal.FreeHGlobal(lpRasConn);
            }*/
        }

        private void ConnectToRasVPN(string vpnName, string username, string password)
        {
            OpenVPNConnector.AppendTextToOutput($"Connecting to {vpnName}...", listBox2);
            shell.SendCmd("rasdial", vpnName + " " + username + " " + password, "", listBox2);
            OpenVPNConnector.AppendTextToOutput("rasdial" + " " + "Silent_VPN" + " " + username + " " + password + "", listBox2);
            //RasDialWrapper.ConnectToVPN("SilentVPN", username, password, listBox2);
        }

        private async Task GetExternalIpAsync()
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

        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            string selectedVPN = listBox1.SelectedItem.ToString();
            if (OpenVPN)
            {
                ConnecttoOpenVPN();
                await GetExternalIpAsync();
                UpdateContextMenu();
                MessageBox.Show("OpenVPN Connected!", "Message Box", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (Rasdial)
            {
                LoadAuthList();
                ConnectToRasVPN("Silent_VPN", label3.Text, label5.Text);
                await GetExternalIpAsync();
            }

        }

        private void LoadAuthList() {

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

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if an item is selected
            if (listBox1.SelectedItem != null)
            {
                // Get the selected item
                string selectedItem = listBox1.SelectedItem.ToString();
                // Display or process the selected item as needed
                MessageBox.Show("You selected: " + selectedItem);
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, EventArgs e)
        {
            this.Show(); // Show the form
            this.WindowState = FormWindowState.Normal; // Restore the window state
            notifyIcon1.Visible = false; // Hide the icon from the system tray
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide(); // Hide the form
                notifyIcon1.Visible = true; // Show the icon in the system tray
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true; // Cancel the closing event
            this.Close(); // Hide the form instead of closing it
            //shell.SendCmd("cmd.exe", "taskkill /IM SilentLiveVPN.exe /F", "", listBox2);
            //Application.Exit(); // Exit the application
        }

        private void ShowContextMenu(MouseEventArgs e)
        {
            if (menu != null)
            {
                Point position = e.Location; // Use the mouse event location
                menu.Show(position); // Show the menu at the cursor position
            }
            else
            {
                MessageBox.Show("ContextMenuStrip is not initialized.");
            }
        }


        private void notifyIcon1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (menu != null && menu.Visible)
                {
                    menu.Hide(); // Simply hide the menu
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                ShowContextMenu(e);
            }
        }


        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (menu != null)
                {
                    NotifyIcon notifyIconSender = (NotifyIcon)sender; // Correctly cast to NotifyIcon
                    Point position = System.Windows.Forms.Cursor.Position; // Access Cursor.Position statically
                    Point ptLowerLeft = position; // Get the current cursor position
                    menu.Show(ptLowerLeft); // Show the menu at the cursor position

                }
                else
                {
                    MessageBox.Show("ContextMenuStrip is not initialized.");
                }

            }

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show(); // Show the form
            this.WindowState = FormWindowState.Normal; // Restore the form to normal state
            notifyIcon1.Visible = false; // Hide the NotifyIcon
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit(); // Exit the application
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
  
            
        }

        private void TerminateProcess(string processName)
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

        private void SaveUserInputToFile(string input1, string input2)
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
        //disconnect button
        private async void button2_Click_1Async(object sender, EventArgs e)
        {
            //MessageBox.Show("Disconnected!");

            if (OpenVPN) {

                string processName = "openvpn";
                TerminateProcess(processName);
                label2.Text = "No Connection";
                await GetExternalIpAsync();
                UpdateContextMenu();
            }
            else if (Rasdial) {

                DisconnectFromRas();
                await GetExternalIpAsync();
                UpdateContextMenu();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SaveUserInputToFile(textBox1.Text, textBox2.Text);
            LoadAuthList();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private async void button6_ClickAsync(object sender, EventArgs e)
        {
            chart1.Visible = !chart1.Visible;
            lblBytesReceived.Visible = !lblBytesReceived.Visible;
            WifiName.Visible = !WifiName.Visible;
            lblBytesSent.Visible = !lblBytesSent.Visible;
            chart1.Enabled = true;
            lblBytesSent.Enabled = true;
            WifiName.Enabled = true;
            lblBytesReceived.Enabled = true;
            await GetExternalIpAsync();
        }

        private void button4_Click(object sender, EventArgs e)
        {


        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (OpenVPN)
            {
                OpenVPN = false;
                //MessageBox.Show("OpenVPN Disabled!");

            }
            else if (!OpenVPN)
            {
                Rasdial = false;
                OpenVPN = true;
                //MessageBox.Show("OpenVPN Enabled! Press connect");
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (Rasdial)
            {
                Rasdial = false;
                //MessageBox.Show("Rasdial Disabled!");
            }
            else if (!Rasdial)
            {
                Rasdial = true;
                OpenVPN = false;
                //MessageBox.Show("Rasdial Enabled!");
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            string vpnNameToCheck = "Silent_VPN"; // Replace with the VPN name you want to check
            if (CreateVPN.IsVpnNameTaken(vpnNameToCheck))
            {
                MessageBox.Show($"The VPN is already setup.");
            }
            else
            {

                // Ensure that the selected item is not null before converting to string
                if (listBox1.SelectedItem != null)
                {
                    string selectedVPN = listBox1.SelectedItem.ToString();

                    // Check if the selectedVPN string is null or empty
                    if (string.IsNullOrEmpty(selectedVPN))
                    {
                        MessageBox.Show("Please select a VPN from the list.");
                        return; // Exit the method if the string is empty
                    }

                    // Proceed to add the VPN if the string is valid
                    CreateVPN.AddVPN(selectedVPN, listBox2);
                }
                else
                {
                    MessageBox.Show("No VPN selected. Please select a VPN.");
                }
            }


        }
        //updateVPN Adapter
        private void button4_Click_1(object sender, EventArgs e)
        {

            string vpnNameToCheck = "Silent_VPN"; // Replace with the VPN name you want to check
            if (CreateVPN.IsVpnNameTaken(vpnNameToCheck))
            {
                // Ensure that the selected item is not null before converting to string
                if (listBox1.SelectedItem != null)
                {
                    string selectedVPN = listBox1.SelectedItem.ToString();

                    // Check if the selectedVPN string is null or empty
                    if (string.IsNullOrEmpty(selectedVPN))
                    {
                        MessageBox.Show("Please select a VPN from the list.");
                        return; // Exit the method if the string is empty
                    }

                    // Proceed to add the VPN if the string is valid
                    CreateVPN.UpdateVPN(selectedVPN, listBox2);
                }
                else
                {
                    MessageBox.Show("No VPN selected. Please select a VPN.");
                }
            }
            else
            {
                MessageBox.Show($"VPN Adapter Isn't Setup!");
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            string vpnNameToCheck = "Silent_VPN"; // Replace with the VPN name you want to check
            if (CreateVPN.IsVpnNameTaken(vpnNameToCheck))
            {

                // Ensure that the selected item is not null before converting to string
                if (listBox1.SelectedItem != null)
                {
                    string selectedVPN = listBox1.SelectedItem.ToString();

                    // Check if the selectedVPN string is null or empty
                    if (string.IsNullOrEmpty(selectedVPN))
                    {
                        MessageBox.Show("Please select a VPN from the list.");
                        return; // Exit the method if the string is empty
                    }

                    // Proceed to add the VPN if the string is valid
                    CreateVPN.GetVPNinfo(selectedVPN, listBox2);
                }
                else
                {
                    MessageBox.Show("No VPN selected. Please select a VPN.");
                }
            }
            else
            {
                    MessageBox.Show($"VPN Adapter Isn't Setup!");
            }
            
        }
    }
}
