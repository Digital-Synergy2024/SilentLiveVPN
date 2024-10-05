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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Threading;
using static SilentLiveVPN.RasDialWrapper;
using static SilentLiveVPN.RasDialManager;
using static SilentLiveVPN.Utilities;
using static SilentLiveVPN.CreateVPN;
namespace SilentLiveVPN
{ 

    public partial class Silent : Form
    {
        public static Utilities Tools = new Utilities();
        public static CreateVPN createVPN = new CreateVPN();
        Variables variables = new Variables();
        OpenVPNConnector OpenVPN = new OpenVPNConnector();
        
        public static ContextMenuStrip menu = new ContextMenuStrip { AutoClose = false };

        /*Note to self: WinForms doesn't like static labels will cause UI issues updating after pressing VPN connect button*/
        public static ListBox listBoxOutPut;
        public static RadioButton RadioButtonVPN1;
        public static RadioButton RadioButtonVPN2;
        public static RadioButton RadioButtonVPN3;
        public static Label OpenVPNlblA;
        public static Label SoftlblA;
        public static Label RadiallblA;
        public static Label labelGeoA;
        public static Label labelGeoB;
        public  Silent()
        {         
            InitializeComponent();
            Sunisoft.IrisSkin.SkinEngine skin = new Sunisoft.IrisSkin.SkinEngine();
            skin.SkinAllForm = true;
            // var path = "..\\..\\s\\a (1).ssk";
            skin.SkinFile = Environment.CurrentDirectory + @"\s\a (43).ssk";
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
            Tools.InitializeTimer(chart1, lblBytesSent, lblBytesReceived, WifiName, label1, label3, label5, labelGeo, label9);
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
            labelGeoB = label9;
            radioButton1.CheckedChanged += RadioButton_CheckedChanged;
            radioButton2.CheckedChanged += RadioButton_CheckedChanged;
            radioButton3.CheckedChanged += RadioButton_CheckedChanged;
        }

        private void UpdateLabel(Label label, bool status) {
            if (label.IsHandleCreated)
            {
                label.Invoke((MethodInvoker) delegate {
                    label.Text = status ? "Enabled" : "Disabled";
                    label.ForeColor = status ? Color.FromArgb(28, 168, 25) : Color.FromArgb(255, 0, 0);
                });
            }
        }

        private async void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null && radioButton.Checked)
            {
                
                // Check if RadioButtonVPN1 (OpenVPN) is selected
                if (radioButton1.Checked)
                {

                    ToggleOpenVPN();
                    // Disable other VPNs
                    Utilities.Variables.Rasdial = false;
                    Utilities.Variables.SoftEther = false;
                    UpdateLabel(radiallbl, Utilities.Variables.Rasdial);
                    UpdateLabel(softlbl, Utilities.Variables.SoftEther);
                }
                // Similar checks for other radio buttons...
                else if (radioButton2.Checked)
                {
                    ToggleOpenVPN();
                    // Disable other VPNs
                    Utilities.Variables.OpenVPN = false;
                    Utilities.Variables.SoftEther = false;
                    UpdateLabel(OpenVPNlbl, Utilities.Variables.OpenVPN);
                    UpdateLabel(softlbl, Utilities.Variables.SoftEther);
                }
                else if (radioButton3.Checked)
                {
                    ToggleOpenVPN();
                    // Disable other VPNs
                    Utilities.Variables.Rasdial = false;
                    Utilities.Variables.OpenVPN = false;
                    UpdateLabel(radiallbl, Utilities.Variables.Rasdial);
                    UpdateLabel(OpenVPNlbl, Utilities.Variables.OpenVPN);
                }
            }
        }

        private void ToggleOpenVPN()
        {
            Utilities.Variables.OpenVPN = !Utilities.Variables.OpenVPN; // Toggle the boolean value
            UpdateLabel(OpenVPNlbl, Utilities.Variables.OpenVPN);

            Utilities.Variables.Rasdial = !Utilities.Variables.Rasdial; // Toggle the boolean value
            UpdateLabel(radiallbl, Utilities.Variables.Rasdial);

            Utilities.Variables.SoftEther = !Utilities.Variables.SoftEther; // Toggle the boolean value
            UpdateLabel(softlbl, Utilities.Variables.SoftEther);
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
            if (Utilities.Variables.OpenVPN)
            {
                await OpenVPN.AppendTextToOutput("Connecting...", listBox2);
                await Tools.GetExternalIpAsync(label1);
                await Tools.CallUpdateContextMenuAsync();
                await OpenVPNConnector.ConnecttoOpenVPN(listBox2, label2);
                //Tools.OpenVPN = false;
            }
            else if (Utilities.Variables.Rasdial)
            {
                await OpenVPN.AppendTextToOutput("Connecting...", listBox2);
                Tools.LoadAuthList(label3, label5);
                await RasDialManager.ConnectToRasVPN("Silent_VPN", label3.Text, label5.Text, listBox2);
                await Tools.GetExternalIpAsync(label1);
                await Tools.CallUpdateContextMenuAsync();
                //Tools.Rasdial = false;
            }
            else if (Utilities.Variables.SoftEther)
            {

                Tools.LoadAuthList(label3, label5);
                await shell.SendCmd($"vpncmd", $"/CLIENT 127.0.0.1 /CMD AccountRetrySet {label3.Text} /NUM:0 /INTERVAL:5", "", listBox2);
                await shell.SendCmd($"vpncmd", $"/CLIENT 127.0.0.1 /CMD AccountConnect  {label3.Text}", "", listBox2);
                //Tools.SoftEther = false;
            }
            menu.Close();
        }

        public async void Option2_ClickAsync(object sender, EventArgs e)
        {
            // Handle Option 2 click
            if (Utilities.Variables.OpenVPN)
            {
                await Tools.TerminateProcess(variables.ProcessName);
                //label1.Text = "No Connection";
                await Tools.GetExternalIpAsync(label1);
                await Tools.CallUpdateContextMenuAsync();
                //OpenVPN = false;
            }
            else if (Utilities.Variables.Rasdial)
            {
                await RasDialManager.DisconnectFromRas(listBox2);
                await Tools.GetExternalIpAsync(label1);
                await Tools.CallUpdateContextMenuAsync();
                //Rasdial = false;
            }
            else if (Utilities.Variables.SoftEther)
            {
                Tools.LoadAuthList(label3, label5);
                await shell.SendCmd($"vpncmd", $"/CLIENT 127.0.0.1 /CMD AccountDisconnect {label3.Text}", "", listBox2);
                //SoftEther = true;
            }
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
            menu.Items.Add(Tools.ExternalIP, null, Option3_Click);
            menu.Items.Add("Close SilentVpn", null, Option4_Click);
        }

        public void UpdateContextMenu()
        {
            // Clear existing items and re-add them to reflect the updated label text
            menu.Items.Clear();
            menu.Items.Add("Connect", null, Option1_ClickAsync);
            menu.Items.Add("Disconnect", null, Option2_ClickAsync);
            menu.Items.Add(Tools.ExternalIP, null, Option3_Click);
            menu.Items.Add("Close SilentVpn", null, Option4_Click);
            notifyIcon1.Text = label1.Text;
        }

        public void removeConectMenuItem() {

            RemoveMenuItem(label1.Text);
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
            chart1.ChartAreas[0].BackColor = Color.Black;
        }

        public async void button1_ClickAsync(object sender, EventArgs e) {            
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
                Thread.Sleep(3000); // Simulate a delay
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
            label1.Visible = !label1.Visible;
            label2.Visible = !label2.Visible;
            label4.Visible = !label4.Visible;
            lblBytesSent.Enabled = true;
            WifiName.Enabled = true;
            lblBytesReceived.Enabled = true;
            label4.Enabled = true;
            label1.Enabled = true;
            label2.Enabled = true;
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
            //ReadGeoData();
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
