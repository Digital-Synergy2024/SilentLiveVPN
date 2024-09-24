using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
//using System.Threading;
using System.IO;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Windows.Forms.DataVisualization.Charting;

namespace SilentLiveVPN.Utilities
{
    public partial class Utilities 
    {
        public static string WifiRouter = "";
        public static float bytesSent;
        public static float bytesReceived;
        public static int counter = 0;
        public static bool OpenVPN;
        public static bool Rasdial;
        public static string vpnNameToCheck = "Silent_VPN";
        public static string selectedVPN;
        internal static Timer updateTimer;


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


        public void LoadGiphyImage(PictureBox pictureBox)
        {
            // Path to your Giphy.webp file
            string imagePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\giphy.webp";

            // Load the image using System.Drawing
            using (var image = Image.FromFile(imagePath))
            {
                // Set the PictureBox image
                pictureBox.Image = new Bitmap(image);
            }
        }
    }



}
