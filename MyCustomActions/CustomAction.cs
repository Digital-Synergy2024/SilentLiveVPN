using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace MyCustomActions
{
    public static class CustomActions
    {
        [CustomAction]
        public static ActionResult ExtractZipFile(Session session)
        {

            try
            {
                session.Log($"ExtractZipFile Started! ");


                session.Log($"Extracting 2... ");
                session.Log("ExtractZipFile Started!");
                string zipFilePath = session.CustomActionData["ZIPFILEPATH"];
                string extractPath = zipFilePath + "skin.zip";

                session.Log($"zipFilePath: {zipFilePath}");
                session.Log($"extractPath: {extractPath}");

                //if (!Directory.Exists(extractPath))
                //{
                //Directory.CreateDirectory(extractPath);
                //}

                session.Log("Extracting zip file...");
                //ZipFile.ExtractToDirectory(zipFilePath, extractPath);
                session.Log("Extraction completed successfully.");
                return ActionResult.Success;
            }
            catch (UnauthorizedAccessException ex)
            {
                session.Log("Access denied: " + ex.Message);
                return ActionResult.Failure;
            }
            catch (Exception ex)
            {
                session.Log("Error extracting zip file: " + ex.Message);
                return ActionResult.Failure;
            }
        }

        [CustomAction]
        public static ActionResult ShowLog(Session session)
        {
            // Construct the path to the MSBuild log file in the Temp folder
            string installDir = session.CustomActionData["LOGFILEPATH"];
            string currentDirectory = Directory.GetCurrentDirectory();
            string logFileName = "install.log"; // The name of the log file
            string logFilePath = Path.Combine(currentDirectory, logFileName);
 
            // Check if the log file exists
            if (File.Exists(@"C:\Users\mike\Desktop\MyVpn\source\SilentLiveVPN\SetupSilentVPN\bin\x86\Release\install2.log"))
            {
                // Read the content of the log file
                string logContent = File.ReadAllText(@"C:\Users\mike\Desktop\MyVpn\source\SilentLiveVPN\SetupSilentVPN\bin\x86\Release\install2.log");

                // Display the log content in a message box
                MessageBox.Show(logContent, "Installation Log", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Show an error message if the log file is not found
                MessageBox.Show($"Log file not found: {logFilePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return ActionResult.Success;
        }


    }
}
