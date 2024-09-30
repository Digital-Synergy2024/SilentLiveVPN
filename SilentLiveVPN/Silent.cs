using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using static SilentLiveVPN.RasDialWrapper;
using static SilentLiveVPN.RasDialManager;
using static SilentLiveVPN.Utilities;
using static SilentLiveVPN.CreateVPN;
using Timer = System.Threading.Timer;

namespace SilentLiveVPN
{ 

    public partial class Silent : Form
    {
        public static Utilities Tools = new Utilities();
        public static CreateVPN createVPN = new CreateVPN();
        Variables variables = new Variables();
        OpenVPNConnector OpenVPN = new OpenVPNConnector();
        public static ContextMenuStrip menu = new ContextMenuStrip { AutoClose = false };

        public static ListBox listBoxOutPut;
        public static RadioButton RadioButtonVPN1;
        public static RadioButton RadioButtonVPN2;
        public static RadioButton RadioButtonVPN3;
        public static Label OpenVPNlblA;
        public static Label SoftlblA;
        public static Label RadiallblA;
        public static Label labelGeoA;
        public Silent()
        {
            
            InitializeComponent();
            Sunisoft.IrisSkin.SkinEngine skin = new Sunisoft.IrisSkin.SkinEngine();
            skin.SkinAllForm = true;
            // var path = "..\\..\\s\\a (1).ssk";
            skin.SkinFile = Environment.CurrentDirectory + @"\s\a (20).ssk";
            InitializeContextMenu();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            //Tools.LoadGiphyImage(pictureBox1);
            Tools.LoadAuthList(label3, label5);
            this.Resize += new EventHandler(Form1_Resize);
            // Initialize event handlers
            this.Resize += MyForm_Resize; // Subscribe to the Resize event
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosingAsync);
            openToolStripMenuItem.Click += new EventHandler(openToolStripMenuItem_Click);
            exitToolStripMenuItem.Click += new EventHandler(exitToolStripMenuItem_Click);
            notifyIcon1.DoubleClick += new EventHandler(notifyIcon1_MouseDoubleClick);
            notifyIcon1.MouseClick += new MouseEventHandler(notifyIcon1_MouseClick);
            notifyIcon1.Visible = false; // Hide the icon from the system tray
            Tools.LoadDataIntoListBox(listBox1);
            chart1.Enabled = false;
            lblBytesSent.Enabled = false;
            WifiName.Enabled = false;
            lblBytesReceived.Enabled = false;
            Tools.InitializeTimer(chart1, lblBytesSent, lblBytesReceived, WifiName, label1, label3, label5);
            CreateLineChart();
            OpenVPNConnector.LoanConfig(label4);
            listBoxOutPut = listBox2;
            RadioButtonVPN1 = radioButton1;
            RadioButtonVPN2 = radioButton2;
            RadioButtonVPN3 = radioButton3;
            OpenVPNlblA = OpenVPNlbl;
            SoftlblA = softlbl;
            RadiallblA = radiallbl;
            labelGeoA = labelGeo;
            StartChecking();
        }

        private Timer _timer;

        public void StartChecking()
        {
            // Set the timer to call the CheckGeoLocation method every 3 minutes (180000 ms)
            _timer = new Timer(CheckGeoLocation, null, TimeSpan.Zero, TimeSpan.FromMinutes(3));
        }

        private async void CheckGeoLocation(object state)
        {
            await RunIpFetcher();
        }

        public async Task RunIpFetcher()
        {
            await Tools.CheckIfVPNConnectedAsync();
        }

        public void StopChecking()
        {
            _timer?.Change(Timeout.Infinite, 0);
            _timer?.Dispose();
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

        public async void Option1_ClickAsync(object sender, EventArgs e)
        {
            // Handle Option 1 click
            await Tools.Connect(listBox1, label1,  label2, label3, label5, listBox2, radioButton1, radioButton2, radioButton3);
            menu.Close();
        }

        public async void Option2_ClickAsync(object sender, EventArgs e)
        {
            // Handle Option 2 click
            await Tools.DisConnect(listBox2, label1, label2, label3, label5, radioButton1, radioButton2, radioButton3);
            menu.Close();
        }

        private void Option3_Click(object sender, EventArgs e)
        {

            menu.Close();
        }

        public static void Option4_Click(object sender, EventArgs e)
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

        public void UpdateContextMenu()
        {
            // Clear existing items and re-add them to reflect the updated label text
            //menu.Items.Clear();
            menu.Items.Add("Connect", null, Option1_ClickAsync);
            menu.Items.Add("Disconnect", null, Option2_ClickAsync);
            menu.Items.Add(label2.Text, null, Option3_Click);
            menu.Items.Add("Close SilentVpn", null, Option4_Click);
            notifyIcon1.Text = label2.Text;
        }

        public void removeConectMenuItem() {

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


        public async void button1_ClickAsync(object sender, EventArgs e)
        {
            await Tools.Connect(listBox1, label1, label2, label3, label5, listBox2, radioButton1, radioButton2, radioButton3);
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

        private async void Form1_FormClosingAsync(object sender, FormClosingEventArgs e)
        {
            // Show a confirmation dialog before closing
            var result = MessageBox.Show("Are you sure you want to close?", "Confirm", MessageBoxButtons.YesNo);

            if (result == DialogResult.No)
            {
                e.Cancel = true; // Cancel the closing event
                return;
            }

            // Perform any asynchronous operations here
            await PerformLongRunningOperationAsync();
        }

        private async Task PerformLongRunningOperationAsync()
        {
            // Simulate a long-running operation
            await Task.Run(async () =>
            {
                // Simulate work (e.g., saving data, cleaning up resources)
                await Tools.TerminateProcess("SIlentLiveVPN.exe");
                System.Threading.Thread.Sleep(3000); // Simulate a delay
            });
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

            if (this.WindowState == FormWindowState.Minimized) {
                this.Show(); // Show the form
                this.WindowState = FormWindowState.Normal; // Restore the form to normal state
                notifyIcon1.Visible = false; // Hide the NotifyIcon
            }

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit(); // Exit the application
        }

        //disconnect button
        private async void button2_Click_1Async(object sender, EventArgs e)
        {
            await Tools.DisConnect(listBox2, label1, label2, label3, label5, radioButton1, radioButton2, radioButton3);
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            await Tools.SaveUserInputToFileAsync(textBox1.Text, textBox2.Text);
            Tools.LoadAuthList(label3, label5);
        }
        //statistics
        private async void button6_ClickAsync(object sender, EventArgs e)
        {
            lblBytesReceived.Visible = !lblBytesReceived.Visible;
            WifiName.Visible = !WifiName.Visible;
            lblBytesSent.Visible = !lblBytesSent.Visible;
            lblBytesSent.Enabled = true;
            WifiName.Enabled = true;
            lblBytesReceived.Enabled = true;
            await Tools.GetExternalIpAsync(label1);
        }

        //graph
        private void button9_Click(object sender, EventArgs e)
        {
            chart1.Visible = !chart1.Visible;
            chart1.Enabled = true;
        }

        private async void button3_Click_1(object sender, EventArgs e)
        {
            bool isTaken = createVPN.IsVpnNameTaken(variables.VpnNameToCheck);

            if (isTaken)
            {
                MessageBox.Show($"The VPN is already setup.");
            }
            else
            {

                // Ensure that the selected item is not null before converting to string
                if (listBox1.SelectedItem != null)
                {
                    variables.SelectedVPN = listBox1.SelectedItem.ToString();

                    // Check if the variables.SelectedVPN  string is null or empty
                    if (string.IsNullOrEmpty(variables.SelectedVPN ))
                    {
                        MessageBox.Show("Please select a VPN from the list.");
                        return; // Exit the method if the string is empty
                    }

                    // Proceed to add the VPN if the string is valid
                    await createVPN.AddVPN(variables.SelectedVPN , listBox2);
                }
                else
                {
                    MessageBox.Show("No VPN selected. Please select a VPN.");
                }
            }


        }
        //updateVPN Adapter
        private async void button4_Click_1(object sender, EventArgs e)
        {
            bool isTaken = createVPN.IsVpnNameTaken(variables.VpnNameToCheck);
            if (isTaken)
            {
                // Ensure that the selected item is not null before converting to string
                if (listBox1.SelectedItem != null)
                {
                    variables.SelectedVPN   = listBox1.SelectedItem.ToString();

                    // Check if the variables.SelectedVPN  string is null or empty
                    if (string.IsNullOrEmpty(variables.SelectedVPN ))
                    {
                        MessageBox.Show("Please select a VPN from the list.");
                        return; // Exit the method if the string is empty
                    }

                    // Proceed to add the VPN if the string is valid
                    await createVPN.UpdateVPN(variables.SelectedVPN , listBox2);
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

        public async void button7_Click(object sender, EventArgs e)
        {

            try {

                if (radioButton1.Checked) {

                    MessageBox.Show("Disabled!");
                }
                else if (radioButton2.Checked)
                {

                    bool isTaken = createVPN.IsVpnNameTaken(variables.VpnNameToCheck);

                    if (isTaken)
                    {

                        // Ensure that the selected item is not null before converting to string
                        if (listBox1.SelectedItem != null)
                        {
                            variables.SelectedVPN = listBox1.SelectedItem.ToString();

                            // Check if the variables.SelectedVPN  string is null or empty
                            if (string.IsNullOrEmpty(variables.SelectedVPN))
                            {
                                MessageBox.Show("Please select a VPN from the list.");
                                return; // Exit the method if the string is empty
                            }

                            // Proceed to add the VPN if the string is valid
                            await createVPN.GetVPNinfo(variables.SelectedVPN, listBox2);
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
                else if (radioButton3.Checked)
                {
                    string[] commands = new string[3] {

                        $"/CLIENT 127.0.0.1 /CMD Accountlist",
                        $"",
                        $""
                    };
                    await shell.SendCmd("vpncmd", commands[0], " ", listBox2);
                }

            }
            catch (Exception ex){

                await OpenVPN.AppendTextToOutput($"vpncmd{ex}",  listBox2);

            }

        }

        private async void button8_ClickAsync(object sender, EventArgs e)
        {

            string[] commands = new string[3] {

                $"/CLIENT 127.0.0.1 /CMD NicCreate VPN2{/*Has to be VPN2 or get Error Code 32 */""}",
                $"/CLIENT 127.0.0.1 /CMD AccountCreate {label3.Text} /SERVER:vpn.silentlive.net:443 /HUB:silent /USERNAME:{label3.Text} /NICNAME:VPN2",
                $"/CLIENT 127.0.0.1 /CMD AccountPasswordSet {label3.Text} /PASSWORD:{label5.Text} /TYPE:Standard"
            };
            await OpenVPN.AppendTextToOutput($"Cmd: {commands.ToString()}", listBox2);
            await shell.SendCmd("vpncmd", commands[0], " ", listBox2);
            await shell.SendCmd("vpncmd", commands[1], " ", listBox2);
            await shell.SendCmd("vpncmd", commands[2], " ", listBox2);
        }

    }
}
