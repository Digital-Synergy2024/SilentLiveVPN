namespace SilentLiveVPN
{
    using System;
    using System.Runtime.InteropServices;

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
    }

}
