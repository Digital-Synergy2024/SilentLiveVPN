using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

class Program
{

    class IpFetcher
    {
        private static readonly HttpClient client = new HttpClient();
        public static async Task<string> GetPublicIpAsync()
        {

            try
            {
                string ip = await client.GetStringAsync("https://api.ipify.org");
                Console.WriteLine($"IP: {ip} ");
                return ip;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message} ");
                return null;
            }
        }

        public static async Task<string> GetGeoLocationAsync(string ip)
        {

            try
            {
                // https://ipapi.co/69.5.138.33/json/
                string WebRequest = $"http://www.geoplugin.net/json.gp?ip={ip}";
                Console.WriteLine($"Requesting IP: {WebRequest}  ");
                string response = await client.GetStringAsync(WebRequest);
                Console.WriteLine($"Reponse: {response}  ");
                return response; // This will return JSON data with location info
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error fetching geo-location: {ex.Message} ");
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
                "74.91.115.15",
                "69.5.138.33"
        };

        public static bool IsVpnIp(string ip)
        {
            return knownVpnIps.Contains(ip);
        }
    }

    [DllImport("MyCustomActions.CA.dll", CharSet = CharSet.Unicode)]
    public static extern int ExtractZipFile();
    //private static readonly HttpClient client = new HttpClient();
    static async Task Main(string[] args)
    {
        //DriveInfo drive = new DriveInfo("C");
        //Console.WriteLine($"Available space: {drive.AvailableFreeSpace / (1024 * 1024)} MB");

        //int result = ExtractZipFile();
        //Console.WriteLine($"Custom Action Result: {result}");


        try {

            string publicIp = await IpFetcher.GetPublicIpAsync();
            //if (publicIp == null) return false;
            //JObject obj = JObject.Parse(Resp);
            //JObject finalobj = JObject.Parse(("{latLng:[" + obj["geoplugin_latitude"].ToString() + ", " + obj["geoplugin_longitude"] + "], city: \'" + obj["geoplugin_city"].ToString().Replace("'", "\'") + "\'}"));
            bool isVpn = VpnChecker.IsVpnIp(publicIp);
            if (isVpn)
            {
                string geoInfo = await IpFetcher.GetGeoLocationAsync(publicIp);
                // Assuming geoInfo is in JSON format 
                var geoData = JObject.Parse(geoInfo);
                string latitude= geoData["geoplugin_latitude"].ToString();
                string longatiude = geoData["geoplugin_longitude"].ToString();
                string country = geoData["geoplugin_countryName"].ToString();
                string city = geoData["geoplugin_city"].ToString();
                string state = geoData["geoplugin_regionCode"].ToString();
                Console.WriteLine($"Geo-Location Info: LA/LONG:{latitude}/{longatiude} Country: {country} City: {city} ST:  {state}");

            }

        } catch (Exception ex) {

            Console.WriteLine($"Error fetching geo-location: {ex.Message} ");
           
        }

    }
}

