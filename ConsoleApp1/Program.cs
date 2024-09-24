using System;
using System.Runtime.InteropServices;
using System.IO;
class Program
{
    [DllImport("MyCustomActions.CA.dll", CharSet = CharSet.Unicode)]
    public static extern int ExtractZipFile();

    static void Main(string[] args)
    {
        DriveInfo drive = new DriveInfo("C");
        Console.WriteLine($"Available space: {drive.AvailableFreeSpace / (1024 * 1024)} MB");

        //int result = ExtractZipFile();
        //Console.WriteLine($"Custom Action Result: {result}");
    }
}

