using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
namespace SilentLiveVPN
{
    public class RasDialWrapper
    {

        // Importing the RasHangUp function from the RAS API
        [DllImport("rasapi32.dll", CharSet = CharSet.Auto)]
        public static extern int RasHangUp(IntPtr hRasConn);

        // Importing the RasEnumConnections function to get the active connections
        [DllImport("rasapi32.dll", CharSet = CharSet.Auto)]
        public static extern int RasEnumConnections(IntPtr lpRasConn, ref int lpcb, ref int lpcConnections);

        // Struct to hold connection information
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct RASCONN
        {
            public int dwSize;
            public IntPtr hRasConn;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szEntryName;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct RasEntry
        {
            public int dwSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szEntryName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szPhoneNumber;
            public int dwFlags;
            public int dwCountryID;
            public int dwCountryCode;
            public int dwReserved;
            public int dwfOptions;
            public int dwfNetProtocols;
            public int dwfDialingOptions;
            public int dwfFramingProtocol;
            public int dwfEncryptionType;
            public int dwMaxConnections;
            public int dwMaxIdleTime;
            public int dwIdleDisconnectSeconds;
            public int dwfOptions2;
            public int dwfReserved;
        }


        [DllImport("Rasapi32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto)]
        public static extern int RasGetErrorString(int errorCode, StringBuilder errorString, ref int errorStringSize);

        [DllImport("rasapi32.dll", CharSet = CharSet.Auto)]
        public static extern int RasSetEntryProperties(
        string lpszPhonebook,
        string lpszEntry,
        ref RasEntry lpEntry,
        int dwEntrySize,
        IntPtr lpPassword,
        int dwPasswordSize);

        internal static void CreateL2TPVPN(string entryName, string phoneNumber, ListBox listBox2)
        {
            RasEntry entry = new RasEntry
            {
                dwSize = Marshal.SizeOf(typeof(RasEntry)),
                szEntryName = entryName,
                szPhoneNumber = phoneNumber,
                dwFlags = 0, // Set appropriate flags
                dwfNetProtocols = 0x00000001, // Use appropriate protocol
                dwfFramingProtocol = 0x00000002, // L2TP
                 // Set other properties as needed
            };

            int result = RasSetEntryProperties(null, entryName, ref entry, Marshal.SizeOf(entry), IntPtr.Zero, 0);

            if (result != 0)
            {
                LogError(result, listBox2);

            }
        }

        [DllImport("rasapi32.dll", CharSet = CharSet.Auto)]
        public static extern int RasDial(
        IntPtr lpRasDialExtensions,
        string lpszPhonebook,
        ref RasDialParams lpRasDialParams,
        int dwNotifierType,
        IntPtr dwNotifier,
        out IntPtr lphRasConn);

        const int MAX_PATH = 260;
        const int RAS_MaxDeviceType = 16;
        const int RAS_MaxPhoneNumber = 128;
        const int RAS_MaxEntryName = 256;
        const int RAS_MaxDeviceName = 128;

        const int RAS_Connected = 0x2000;

        /*[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct RasDialParams
        {
            public int dwSize;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szEntryName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szPhoneNumber;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szCallbackNumber;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szUserName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szPassword;
        }*/

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct RasDialParams
        {
            public int dwSize;
            public IntPtr hrasconn;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxEntryName + 1)]
            public string szEntryName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxEntryName + 1)]
            public string szUserName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxEntryName + 1)]
            public string szPassword;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxDeviceType + 1)]
            public string szDeviceType;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = RAS_MaxDeviceName + 1)]
            public string szDeviceName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
            public string szPhonebook;
            public int dwSubEntry;
        }

        internal static void LogError(int errorCode, ListBox listBox2)
        {
            StringBuilder errorMessage = new StringBuilder(256);
            int size = errorMessage.Capacity;
            RasGetErrorString(errorCode, errorMessage, ref size);
            OpenVPNConnector.AppendTextToOutput($"Error connecting to VPN: {errorCode} - {errorMessage}", listBox2);
        }

        internal static void ConnectToVPN(string entryName, string username, string password, ListBox listBox2)
        {
            RasDialParams dialParams = new RasDialParams
            {
                dwSize = Marshal.SizeOf(typeof(RasDialParams)),
                szEntryName = entryName,
                szUserName = username,
                szPassword = password
            };

            IntPtr hRasConn;
            int result = RasDial(IntPtr.Zero, null, ref dialParams, 0, IntPtr.Zero, out hRasConn);

            // Convert the integer result to a string
            string resultString = result.ToString();

            if (result == 0)
            {
                OpenVPNConnector.AppendTextToOutput($"Connected to {entryName} VPN successfully!", listBox2);
            }
            else
            {
                OpenVPNConnector.AppendTextToOutput($"Failed to connect to VPN. Error code: {resultString}", listBox2);
                OpenVPNConnector.AppendTextToOutput($"VPN:{entryName} UserName:{username} Password:{password}", listBox2);
                LogError(result, listBox2);
            }
        }

    }
}
