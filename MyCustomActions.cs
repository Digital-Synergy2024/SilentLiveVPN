using System;
using System.IO;
using System.IO.Compression;
using Microsoft.Deployment.WindowsInstaller;
using System.IO.Compression.FileSystem;
namespace MyCustomActions
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult ExtractZipFile(Session session)
        {
            try
            {
                string zipFilePath = session["INSTALLDIR"] + "file.zip"; // Path to the zip file
                string extractPath = session["INSTALLDIR"]; // Destination path

                // Extract the zip file
                ZipFile.ExtractToDirectory(zipFilePath, extractPath);
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.Log("Error extracting zip file: " + ex.Message);
                return ActionResult.Failure;
            }
        }
    }
}

