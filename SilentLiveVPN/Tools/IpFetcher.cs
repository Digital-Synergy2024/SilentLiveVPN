namespace SilentLiveVPN.Tools
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Newtonsoft.Json.Linq;
    public class ipFecthing{

        class IpFetcher
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


        class VpnChecker
        {
            private static HashSet<string> knownVpnIps = new HashSet<string>
            {
                "192.0.2.1", // Example VPN IP
                "203.0.113.5", // Another example
                "51.79.52.118",
                "74.91.115.15"
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
                    string response = await client.GetStringAsync($"https://ipapi.co/{ip}/json/");

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
            public static async Task<bool> IsUserConnectedToVpnAsync()
            {
                string publicIp = await IpFetcher.GetPublicIpAsync();
                if (publicIp == null) return false;

                bool isVpn = VpnChecker.IsVpnIp(publicIp);
                if (isVpn)
                {
                    string geoInfo = await GeoLocationService.GetGeoLocationAsync(publicIp);
                    // Assuming geoInfo is in JSON format
                    var geoData = JObject.Parse(geoInfo);
                    string country = geoData["country"].ToString();
                    string city = geoData["city"].ToString();
                    await connector.AppendTextToOutput($"Geo-Location Info: Country: {country} City: {city}", Silent.listBoxOutPut);
                    Silent.labelGeoA.Text = $"GeoLaction: {geoInfo}";

                    if (Silent.RadioButtonVPN1.Checked) {

                        Silent.OpenVPNlblA.Text = "Connected";

                    } else if (Silent.RadioButtonVPN2.Checked) {

                        Silent.RadiallblA.Text = "Connected";

                    }
                    else if (Silent.RadioButtonVPN3.Checked)
                    {

                        Silent.SoftlblA.Text = "Connected";

                    }
                    return true; // User is connected to a VPN
                }
                Silent.OpenVPNlblA.Text = "Disconnected";
                Silent.RadiallblA.Text = "Disconnected";
                Silent.SoftlblA.Text = "Disconnected";
                return false; // User is not connected to a VPN
            }
        }

    }



}
