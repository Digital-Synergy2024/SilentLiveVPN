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
using static SilentLiveVPN.RasDialWrapper;
using static SilentLiveVPN.RasDialManager;
using static SilentLiveVPN.Utilities;

namespace SilentLiveVPN
{ 

    public partial class Silent : Form
    {
        public static Utilities Tools = new Utilities();
        Variables variables = new Variables();
        public static ContextMenuStrip menu = new ContextMenuStrip { AutoClose = false };

        public static Variables vars = new Variables();
        OpenVPNManager manager = new OpenVPNManager(vars);

        public Silent()
        {
            variables.OpenVPN = false;
            InitializeComponent();
            Sunisoft.IrisSkin.SkinEngine skin = new Sunisoft.IrisSkin.SkinEngine();
            skin.SkinAllForm = true;
            // var path = "..\\..\\s\\a (1).ssk";
            skin.SkinFile = Environment.CurrentDirectory + @"\s\a (40).ssk";
            InitializeContextMenu();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            //Tools.LoadGiphyImage(pictureBox1);
            Tools.LoadAuthList(label3, label5);
            this.Resize += new EventHandler(Form1_Resize);
            this.Resize += MyForm_Resize; // Subscribe to the Resize event
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            openToolStripMenuItem.Click += new EventHandler(openToolStripMenuItem_Click);
            exitToolStripMenuItem.Click += new EventHandler(exitToolStripMenuItem_Click);
            notifyIcon1.DoubleClick += new EventHandler(notifyIcon1_MouseDoubleClick);
            notifyIcon1.MouseClick += new MouseEventHandler(notifyIcon1_MouseClick);
            // Subscribe to the Resize event
            //this.Resize += MyForm_Resize;
            // Initialize event handlers
            Tools.LoadDataIntoListBox(listBox1);
            chart1.Enabled = false;
            lblBytesSent.Enabled = false;
            WifiName.Enabled = false;
            lblBytesReceived.Enabled = false;
            Tools.InitializeTimer(chart1, lblBytesSent, lblBytesReceived, WifiName, label1, label3, label5);
            CreateLineChart();
            OpenVPNConnector.LoanConfig(label4);
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
            await Tools.Connect(listBox1, label1,  label2, label3, label5, listBox2);
            menu.Close();
            //MessageBox.Show("VPN Connected!", "Message Box", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public async void Option2_ClickAsync(object sender, EventArgs e)
        {
            // Handle Option 2 click
            await Tools.DisConnect(listBox2, label1, label2);
            //MessageBox.Show("VPN Disconnected!", "Message Box", MessageBoxButtons.OK, MessageBoxIcon.Information);
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


        public async void button1_ClickAsync(object sender, EventArgs e)
        {
            await Tools.Connect(listBox1, label1, label2, label3, label5, listBox2);
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

        //disconnect button
        private async void button2_Click_1Async(object sender, EventArgs e)
        {
            await Tools.DisConnect(listBox2, label1, label2);
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            await Tools.SaveUserInputToFile(textBox1.Text, textBox2.Text);
            Tools.LoadAuthList(label3, label5);
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
            await Tools.GetExternalIpAsync(label1);
        }

        private void button4_Click(object sender, EventArgs e)
        {


        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            manager.ToggleOpenVPN();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            manager.ToggleOpenVPN();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            
            if (CreateVPN.IsVpnNameTaken(variables.VpnNameToCheck))
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
                    CreateVPN.AddVPN(variables.SelectedVPN , listBox2);
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

            if (CreateVPN.IsVpnNameTaken(variables.VpnNameToCheck))
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
                    CreateVPN.UpdateVPN(variables.SelectedVPN , listBox2);
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
           
            if (CreateVPN.IsVpnNameTaken(variables.VpnNameToCheck))
            {

                // Ensure that the selected item is not null before converting to string
                if (listBox1.SelectedItem != null)
                {
                    variables.SelectedVPN  = listBox1.SelectedItem.ToString();

                    // Check if the variables.SelectedVPN  string is null or empty
                    if (string.IsNullOrEmpty(variables.SelectedVPN ))
                    {
                        MessageBox.Show("Please select a VPN from the list.");
                        return; // Exit the method if the string is empty
                    }

                    // Proceed to add the VPN if the string is valid
                    CreateVPN.GetVPNinfo(variables.SelectedVPN , listBox2);
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
