namespace SilentLiveVPN.Tools
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Newtonsoft.Json.Linq;
    using System.IO;
    using Newtonsoft.Json;
    using System.Net;

    public class ipFecthing{

        public class IpFetcher
        {
            private static readonly HttpClient client = new HttpClient();
            public static OpenVPNConnector connector = new OpenVPNConnector();
            public static async Task<string> GetPublicIpAsync()
            {

                try
                {
                    string ip = await client.GetStringAsync("https://api.ipify.org");
                    return ip;
                }
                catch (Exception ex)
                {
                    await connector.AppendTextToOutput($"Error: {ex.Message}", Silent.listBoxOutPut);
                    return null;
                }
            }
    
        }


        public class VpnChecker
        {
            private static HashSet<string> knownVpnIps = new HashSet<string>
            {
                "192.0.2.1", // Example VPN IP                  "69.5.138.33"
                "203.0.113.5", // Another example
                "51.79.52.118",
                "74.91.115.15",
                "69.5.138.33"
            };

            public static bool IsVpnIp(string ip)
            {
                return knownVpnIps.Contains(ip);
            }
        }

        public class GeoLocationService
        {
            private static readonly HttpClient client = new HttpClient();
            public static Utilities Tools = new Utilities();
            public static OpenVPNConnector connector = new OpenVPNConnector();
            public static async Task<string> GetGeoLocationAsync(string ip)
            {
                try
                {
                    string response = await client.GetStringAsync($"http://www.geoplugin.net/json.gp?ip={ip}");

                    return response; // This will return JSON data with location info
                }
                catch (Exception ex)
                {
                    await connector.AppendTextToOutput($"Error fetching geo-location: {ex.Message}", Silent.listBoxOutPut);
                    return null;
                }
            }
        }

        public class VpnDetection
        {
            public static Utilities Tools = new Utilities();
            public static OpenVPNConnector connector = new OpenVPNConnector();

            public static string GetIp()
            {
                string IPAddress = "";
                IPHostEntry Host = default(IPHostEntry);
                string Hostname = null;
                Hostname = System.Environment.MachineName;
                Host = Dns.GetHostEntry(Hostname);
                foreach (IPAddress IP in Host.AddressList)
                {
                    if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        IPAddress = Convert.ToString(IP);
                    }
                }

                return IPAddress;
            }


        }

    }

}




