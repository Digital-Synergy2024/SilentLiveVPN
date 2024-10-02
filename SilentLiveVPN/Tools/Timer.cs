namespace SilentLiveVPN.Tools
{
    using System;
    using System.Windows.Forms;
    using System.Threading.Tasks;
    using System.Threading;
    using Timer = System.Windows.Forms.Timer;

    public class GeoLocationChecker
    {
        private Timer _timer;
        public static Utilities Tools = new Utilities();
        OpenVPNConnector OpenVPN = new OpenVPNConnector();
        public GeoLocationChecker()
        {           
            // Initialize the WinForms Timer
            _timer = new Timer();
            _timer.Interval = 30000; // Set the interval to 3 minutes (180000 ms) 180000
            _timer.Tick += async (sender, e) => await RunIpFetcher(); // Subscribe to the Tick event
        }

        public async Task StartIt()
        {
            
            _timer.Start(); // Start the timer
            //await StartTimer();

        }

        private async Task CheckGeoLocation()
        {
            //await OpenVPN.AppendTextToOutput($"Starting 2", Silent.listBoxOutPut);
            await RunIpFetcher();
             
            //await OpenVPN.AppendTextToOutput($"Starting IPFetcher", Silent.listBoxOutPut);
        }

        public async Task RunIpFetcher()
        {
            try
            {
                await Silent.IsUserConnectedToVpnAsync();
            }
            catch (TimeoutException)
            {
                // Handle timeout scenario
                await OpenVPN.AppendTextToOutput($"The operation timed out while checking VPN connection.", Silent.listBoxOutPut);
            }
        }

        public async Task StopChecking()
        {
            _timer.Stop(); // Stop the timer
            _timer.Dispose(); // Dispose of the timer resources
        }
    }

}
