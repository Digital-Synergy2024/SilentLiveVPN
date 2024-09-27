namespace SilentLiveVPN
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using static SilentLiveVPN.RasDialWrapper;
    public class RasDialManager
    {
        // Constants for the RasSetCredentials function
        private const int RAS_MAXLEN = 256;
        private const int RASAPIVERSION = 1;
  

        // Structure to hold the credentials
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct RASDIALPARAMS2
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MAXLEN)]
            public string szEntryName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MAXLEN)]
            public string szUserName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MAXLEN)]
            public string szPassword;
        }

        // Importing the RasSetCredentials function from the Rasapi32.dll
        [DllImport("rasapi32.dll", CharSet = CharSet.Auto)]
        public static extern int RasSetCredentials(
            string lpszPhonebook,
            string lpszEntry,
            ref RASDIALPARAMS2 lpCredentials,
            int dwFlags);

        public void OverwritesTheStoredCredentialsWhenCredentialsAreSupplied(string entryName, string userName, string password)
        {
            // Create an instance of RASDIALPARAMS
            RASDIALPARAMS2 rasDialParams = new RASDIALPARAMS2
            {
                szEntryName = entryName,
                szUserName = userName,
                szPassword = password
            };

            // Call RasSetCredentials to overwrite the stored credentials
            int result = RasSetCredentials(null, entryName, ref rasDialParams, 0);

            // Check the result for success or failure
            if (result != 0)
            {
                // Handle the error accordingly
                throw new InvalidOperationException($"Failed to overwrite credentials. Error code: {result}");
            }
            else
            {
                Console.WriteLine("Credentials successfully overwritten.");
            }
        }


        internal static async Task DisconnectFromRas(ListBox listBox2)
        {

            //rasdial "MyVPN" /disconnect

            await shell.SendCmd("rasdial", "Silent_VPN /disconnect", "", listBox2);
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
        public static OpenVPNConnector connector = new OpenVPNConnector();
        public static async Task ConnectToRasVPN(string vpnName, string username, string password, ListBox listBox2)
        {
            await connector.AppendTextToOutput($"Connecting to {vpnName}...", listBox2);
            await shell.SendCmd("rasdial", vpnName + " " + username + " " + password, "", listBox2);
            await connector.AppendTextToOutput("rasdial" + " " + "Silent_VPN" + " " + username + " " + password + "", listBox2);
            //RasDialWrapper.ConnectToVPN("SilentVPN", username, password, listBox2);
        }
    }

}
