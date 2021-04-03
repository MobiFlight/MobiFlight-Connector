using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlightInstaller
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            LogAppenderFile logAppenderFile = new LogAppenderFile(false);
            Log.Instance.AddAppender(logAppenderFile);
            Log.Instance.Enabled = true;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UI.UpdaterMainForm());
        }
    }
}
