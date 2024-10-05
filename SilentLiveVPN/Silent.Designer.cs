namespace SilentLiveVPN
{
    partial class Silent
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Silent));
            this.button1 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.lblBytesSent = new System.Windows.Forms.Label();
            this.lblBytesReceived = new System.Windows.Forms.Label();
            this.WifiName = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.silentBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.button9 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.button7 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.button6 = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.openToolStripMenuItem = new System.Windows.Forms.MenuStrip();
            this.open = new System.Windows.Forms.ToolStripMenuItem();
            this.close = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.MenuStrip();
            this.OpenVPNlbl = new System.Windows.Forms.Label();
            this.radiallbl = new System.Windows.Forms.Label();
            this.softlbl = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.labelGeo = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.silentBindingSource)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.openToolStripMenuItem.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.button1.Location = new System.Drawing.Point(-4, -1);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(111, 31);
            this.button1.TabIndex = 0;
            this.button1.Text = "Connect";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_ClickAsync);
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.Color.Black;
            this.listBox1.ForeColor = System.Drawing.Color.Red;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 22;
            this.listBox1.Items.AddRange(new object[] {
            "vpn.silentlive.net",
            "example 1",
            "example 2"});
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(331, 158);
            this.listBox1.TabIndex = 3;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // lblBytesSent
            // 
            this.lblBytesSent.AutoSize = true;
            this.lblBytesSent.BackColor = System.Drawing.Color.Transparent;
            this.lblBytesSent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBytesSent.Enabled = false;
            this.lblBytesSent.Font = new System.Drawing.Font("Sylfaen", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBytesSent.ForeColor = System.Drawing.Color.Red;
            this.lblBytesSent.Location = new System.Drawing.Point(3, 3);
            this.lblBytesSent.Name = "lblBytesSent";
            this.lblBytesSent.Size = new System.Drawing.Size(74, 24);
            this.lblBytesSent.TabIndex = 5;
            this.lblBytesSent.Text = "No Data";
            this.lblBytesSent.Visible = false;
            // 
            // lblBytesReceived
            // 
            this.lblBytesReceived.AutoSize = true;
            this.lblBytesReceived.BackColor = System.Drawing.Color.Transparent;
            this.lblBytesReceived.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBytesReceived.Enabled = false;
            this.lblBytesReceived.Font = new System.Drawing.Font("Sylfaen", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBytesReceived.ForeColor = System.Drawing.Color.Red;
            this.lblBytesReceived.Location = new System.Drawing.Point(3, 25);
            this.lblBytesReceived.Name = "lblBytesReceived";
            this.lblBytesReceived.Size = new System.Drawing.Size(74, 24);
            this.lblBytesReceived.TabIndex = 6;
            this.lblBytesReceived.Text = "No Data";
            this.lblBytesReceived.Visible = false;
            // 
            // WifiName
            // 
            this.WifiName.AutoSize = true;
            this.WifiName.BackColor = System.Drawing.Color.Transparent;
            this.WifiName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.WifiName.Enabled = false;
            this.WifiName.Font = new System.Drawing.Font("Sylfaen", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WifiName.ForeColor = System.Drawing.Color.Red;
            this.WifiName.Location = new System.Drawing.Point(3, 47);
            this.WifiName.Name = "WifiName";
            this.WifiName.Size = new System.Drawing.Size(160, 24);
            this.WifiName.TabIndex = 7;
            this.WifiName.Text = "No Router Detected";
            this.WifiName.Visible = false;
            // 
            // linkLabel1
            // 
            this.linkLabel1.Font = new System.Drawing.Font("Sylfaen", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.Location = new System.Drawing.Point(7, 456);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(5);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(140, 38);
            this.linkLabel1.TabIndex = 8;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = " silentlive.net";
            // 
            // chart1
            // 
            this.chart1.BackColor = System.Drawing.Color.Black;
            this.chart1.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.DiagonalRight;
            this.chart1.BorderlineColor = System.Drawing.SystemColors.ControlDark;
            this.chart1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.DashDotDot;
            chartArea3.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea3);
            this.chart1.DataSource = this.silentBindingSource;
            this.chart1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.chart1.Enabled = false;
            this.chart1.ImeMode = System.Windows.Forms.ImeMode.Off;
            legend3.Name = "Legend";
            this.chart1.Legends.Add(legend3);
            this.chart1.Location = new System.Drawing.Point(0, 502);
            this.chart1.Name = "chart1";
            this.chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Fire;
            series3.ChartArea = "ChartArea1";
            series3.Legend = "Legend";
            series3.Name = "Legend";
            this.chart1.Series.Add(series3);
            this.chart1.Size = new System.Drawing.Size(1397, 300);
            this.chart1.TabIndex = 9;
            this.chart1.Text = "chart1";
            this.chart1.Visible = false;
            // 
            // silentBindingSource
            // 
            this.silentBindingSource.DataSource = typeof(SilentLiveVPN.Silent);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "SilentsVpn";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.button2.Location = new System.Drawing.Point(-4, 31);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(111, 31);
            this.button2.TabIndex = 10;
            this.button2.Text = "Disconnect";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click_1Async);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Enabled = false;
            this.label1.Font = new System.Drawing.Font("Sylfaen", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(3, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 24);
            this.label1.TabIndex = 11;
            this.label1.Text = "Your IP:";
            this.label1.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Enabled = false;
            this.label2.Font = new System.Drawing.Font("Sylfaen", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(2, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(149, 27);
            this.label2.TabIndex = 13;
            this.label2.Text = "Connected TO:";
            this.label2.Visible = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Font = new System.Drawing.Font("Sylfaen", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(342, 192);
            this.tabControl1.TabIndex = 14;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.button9);
            this.tabPage1.Controls.Add(this.button8);
            this.tabPage1.Controls.Add(this.radioButton3);
            this.tabPage1.Controls.Add(this.button7);
            this.tabPage1.Controls.Add(this.button4);
            this.tabPage1.Controls.Add(this.button3);
            this.tabPage1.Controls.Add(this.radioButton2);
            this.tabPage1.Controls.Add(this.radioButton1);
            this.tabPage1.Controls.Add(this.button6);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Location = new System.Drawing.Point(4, 31);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(334, 157);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Main";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // button9
            // 
            this.button9.BackColor = System.Drawing.SystemColors.ControlDark;
            this.button9.Location = new System.Drawing.Point(103, 58);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(111, 31);
            this.button9.TabIndex = 21;
            this.button9.Text = "Graph";
            this.button9.UseVisualStyleBackColor = false;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button8
            // 
            this.button8.BackColor = System.Drawing.SystemColors.ControlDark;
            this.button8.Location = new System.Drawing.Point(217, 108);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(111, 46);
            this.button8.TabIndex = 20;
            this.button8.Text = "SoftEtherAdpater";
            this.button8.UseVisualStyleBackColor = false;
            this.button8.Click += new System.EventHandler(this.button8_ClickAsync);
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(220, 73);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(89, 26);
            this.radioButton3.TabIndex = 19;
            this.radioButton3.TabStop = true;
            this.radioButton3.Tag = "SoftEther";
            this.radioButton3.Text = "SoftEther";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            this.button7.BackColor = System.Drawing.SystemColors.ControlDark;
            this.button7.Location = new System.Drawing.Point(103, 31);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(111, 31);
            this.button7.TabIndex = 18;
            this.button7.Text = "AdapterInfo";
            this.button7.UseVisualStyleBackColor = false;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.SystemColors.ControlDark;
            this.button4.Font = new System.Drawing.Font("Sylfaen", 8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.Location = new System.Drawing.Point(104, 3);
            this.button4.Name = "button4";
            this.button4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.button4.Size = new System.Drawing.Size(111, 31);
            this.button4.TabIndex = 17;
            this.button4.Text = "Update Adapter";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click_1);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.ControlDark;
            this.button3.Location = new System.Drawing.Point(-4, 85);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(111, 31);
            this.button3.TabIndex = 16;
            this.button3.Text = "Setup Adapter";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(220, 41);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(108, 26);
            this.radioButton2.TabIndex = 15;
            this.radioButton2.TabStop = true;
            this.radioButton2.Tag = "Rasdial";
            this.radioButton2.Text = "RasdialVPN";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(221, 9);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(95, 26);
            this.radioButton1.TabIndex = 14;
            this.radioButton1.TabStop = true;
            this.radioButton1.Tag = "OpenVPN";
            this.radioButton1.Text = "OpenVPN";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.BackColor = System.Drawing.SystemColors.ControlDark;
            this.button6.Location = new System.Drawing.Point(-4, 58);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(111, 31);
            this.button6.TabIndex = 13;
            this.button6.Text = "Statistics";
            this.button6.UseVisualStyleBackColor = false;
            this.button6.Click += new System.EventHandler(this.button6_ClickAsync);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.listBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 31);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(334, 157);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "VPN Connections";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.Black;
            this.tabPage3.Controls.Add(this.label4);
            this.tabPage3.Controls.Add(this.lblBytesSent);
            this.tabPage3.Controls.Add(this.label2);
            this.tabPage3.Controls.Add(this.lblBytesReceived);
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Controls.Add(this.WifiName);
            this.tabPage3.Location = new System.Drawing.Point(4, 31);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(334, 157);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Statistics";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Enabled = false;
            this.label4.Font = new System.Drawing.Font("Sylfaen", 14.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(2, 118);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 27);
            this.label4.TabIndex = 14;
            this.label4.Text = "ConfigVPN:";
            this.label4.Visible = false;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.label5);
            this.tabPage4.Controls.Add(this.button5);
            this.tabPage4.Controls.Add(this.label3);
            this.tabPage4.Controls.Add(this.textBox1);
            this.tabPage4.Controls.Add(this.textBox2);
            this.tabPage4.Location = new System.Drawing.Point(4, 31);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(334, 157);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Login";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Font = new System.Drawing.Font("Sylfaen", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(6, 119);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 24);
            this.label5.TabIndex = 17;
            this.label5.Text = "Password";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(194, 119);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(121, 32);
            this.button5.TabIndex = 15;
            this.button5.Text = "Save";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Font = new System.Drawing.Font("Sylfaen", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 24);
            this.label3.TabIndex = 15;
            this.label3.Text = "Username";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 41);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(158, 29);
            this.textBox1.TabIndex = 15;
            this.textBox1.Text = "password";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(6, 6);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(158, 29);
            this.textBox2.TabIndex = 16;
            this.textBox2.Text = "Username";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(1282, 13);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(101, 92);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 15;
            this.pictureBox1.TabStop = false;
            // 
            // listBox2
            // 
            this.listBox2.BackColor = System.Drawing.SystemColors.InfoText;
            this.listBox2.Font = new System.Drawing.Font("Sylfaen", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.listBox2.FormattingEnabled = true;
            this.listBox2.HorizontalScrollbar = true;
            this.listBox2.ItemHeight = 25;
            this.listBox2.Location = new System.Drawing.Point(376, 43);
            this.listBox2.Name = "listBox2";
            this.listBox2.ScrollAlwaysVisible = true;
            this.listBox2.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox2.Size = new System.Drawing.Size(877, 404);
            this.listBox2.TabIndex = 16;
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.open,
            this.close});
            this.openToolStripMenuItem.Location = new System.Drawing.Point(0, 24);
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(1397, 24);
            this.openToolStripMenuItem.TabIndex = 17;
            this.openToolStripMenuItem.Text = "menuStrip1";
            // 
            // open
            // 
            this.open.Name = "open";
            this.open.Size = new System.Drawing.Size(125, 20);
            this.open.Text = "toolStripMenuItem1";
            // 
            // close
            // 
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(125, 20);
            this.close.Text = "toolStripMenuItem2";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Location = new System.Drawing.Point(0, 0);
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(1397, 24);
            this.exitToolStripMenuItem.TabIndex = 18;
            this.exitToolStripMenuItem.Text = "menuStrip2";
            // 
            // OpenVPNlbl
            // 
            this.OpenVPNlbl.AutoSize = true;
            this.OpenVPNlbl.BackColor = System.Drawing.Color.Transparent;
            this.OpenVPNlbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.OpenVPNlbl.Cursor = System.Windows.Forms.Cursors.Cross;
            this.OpenVPNlbl.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OpenVPNlbl.ForeColor = System.Drawing.Color.White;
            this.OpenVPNlbl.Location = new System.Drawing.Point(97, 207);
            this.OpenVPNlbl.Name = "OpenVPNlbl";
            this.OpenVPNlbl.Size = new System.Drawing.Size(111, 21);
            this.OpenVPNlbl.TabIndex = 19;
            this.OpenVPNlbl.Text = "Not Connected";
            // 
            // radiallbl
            // 
            this.radiallbl.AutoSize = true;
            this.radiallbl.BackColor = System.Drawing.Color.Transparent;
            this.radiallbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.radiallbl.Cursor = System.Windows.Forms.Cursors.Cross;
            this.radiallbl.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radiallbl.ForeColor = System.Drawing.SystemColors.Control;
            this.radiallbl.Location = new System.Drawing.Point(97, 229);
            this.radiallbl.Name = "radiallbl";
            this.radiallbl.Size = new System.Drawing.Size(111, 21);
            this.radiallbl.TabIndex = 20;
            this.radiallbl.Text = "Not Connected";
            // 
            // softlbl
            // 
            this.softlbl.AutoSize = true;
            this.softlbl.BackColor = System.Drawing.Color.Transparent;
            this.softlbl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.softlbl.Cursor = System.Windows.Forms.Cursors.Cross;
            this.softlbl.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.softlbl.ForeColor = System.Drawing.Color.White;
            this.softlbl.Location = new System.Drawing.Point(93, 250);
            this.softlbl.Name = "softlbl";
            this.softlbl.Size = new System.Drawing.Size(111, 21);
            this.softlbl.TabIndex = 21;
            this.softlbl.Text = "Not Connected";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.Color.Gray;
            this.label11.Enabled = false;
            this.label11.Font = new System.Drawing.Font("Sitka Text", 24F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(119, 336);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(0, 47);
            this.label11.TabIndex = 24;
            this.label11.Visible = false;
            // 
            // labelGeo
            // 
            this.labelGeo.AutoSize = true;
            this.labelGeo.BackColor = System.Drawing.Color.Transparent;
            this.labelGeo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelGeo.Cursor = System.Windows.Forms.Cursors.Cross;
            this.labelGeo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.labelGeo.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGeo.Location = new System.Drawing.Point(12, 271);
            this.labelGeo.Name = "labelGeo";
            this.labelGeo.Size = new System.Drawing.Size(129, 26);
            this.labelGeo.TabIndex = 25;
            this.labelGeo.Text = "GeoLocation";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label6.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.Control;
            this.label6.Location = new System.Drawing.Point(12, 207);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(79, 21);
            this.label6.TabIndex = 26;
            this.label6.Text = "OpenVPN";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label7.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(12, 229);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(62, 21);
            this.label7.TabIndex = 27;
            this.label7.Text = "Rasdial";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label8.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(12, 250);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 21);
            this.label8.TabIndex = 28;
            this.label8.Text = "SoftEther";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label9.Cursor = System.Windows.Forms.Cursors.Cross;
            this.label9.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label9.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(12, 297);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(75, 26);
            this.label9.TabIndex = 29;
            this.label9.Text = "Region";
            // 
            // Silent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.ClientSize = new System.Drawing.Size(1397, 802);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.labelGeo);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.softlbl);
            this.Controls.Add(this.radiallbl);
            this.Controls.Add(this.OpenVPNlbl);
            this.Controls.Add(this.listBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.openToolStripMenuItem);
            this.Controls.Add(this.exitToolStripMenuItem);
            this.Font = new System.Drawing.Font("Sylfaen", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.openToolStripMenuItem;
            this.MaximizeBox = false;
            this.Name = "Silent";
            this.Text = "SilentVPN";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.silentBindingSource)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.openToolStripMenuItem.ResumeLayout(false);
            this.openToolStripMenuItem.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label lblBytesSent;
        private System.Windows.Forms.Label lblBytesReceived;
        private System.Windows.Forms.Label WifiName;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.BindingSource silentBindingSource;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.MenuStrip openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem open;
        private System.Windows.Forms.ToolStripMenuItem close;
        private System.Windows.Forms.MenuStrip exitToolStripMenuItem;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Label label11;
        public System.Windows.Forms.Label OpenVPNlbl;
        public System.Windows.Forms.Label radiallbl;
        public System.Windows.Forms.Label softlbl;
        public System.Windows.Forms.Label labelGeo;
        public System.Windows.Forms.Label label6;
        public System.Windows.Forms.Label label7;
        public System.Windows.Forms.Label label8;
        public System.Windows.Forms.Label label9;
    }
}

