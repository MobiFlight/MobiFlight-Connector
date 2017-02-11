using System;
using System.Windows.Forms;

namespace DataLogging
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// Note that the main code is not in this class
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ChartForm());
        }
    }
}
