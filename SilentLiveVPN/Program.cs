using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SilentLiveVPN
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Start the long-running task on a separate thread
            Task.Run(() => LongRunningOperation());

            // Run the main form
            Application.Run(new Silent());
        }

        /// <summary>
        /// Simulates a long-running operation.
        /// </summary>
        private static void LongRunningOperation()
        {
            // Simulate a delay (e.g., fetching data)
            for (int i = 0; i < 10; i++)
            {
                System.Threading.Thread.Sleep(1000); // Simulate work
            }
            
        }
    }
}

