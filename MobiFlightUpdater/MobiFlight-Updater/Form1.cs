using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight_Updater
{
    public partial class UpdaterForm : Form
    {
        public UpdaterForm()
        {
            InitializeComponent();
        }

        private async void UpdaterForm_Shown(object sender, EventArgs e)
        {
            var result = false;
            while (!result)
            {
                result = await PerformUpdate();

                if (!result)
                {
                    DialogResult dResult = MessageBox.Show("MobiFlight appears to be running and cannot be stopped. Please stop manually and try the update again.", "Error", MessageBoxButtons.RetryCancel);
                    if (dResult != DialogResult.Retry) break;
                }
            }

            if (result) {
                var proc1 = new ProcessStartInfo();
                var args = Environment.GetCommandLineArgs();
                string applicationName = $"MobiFlightSetup-{args[1]}.exe";
                proc1.UseShellExecute = true;
                proc1.FileName = applicationName;
                proc1.WindowStyle = ProcessWindowStyle.Normal;
                Process p = Process.Start(proc1);
            }

            this.Close();
        }

        private async Task<bool> PerformUpdate()
        {
            List<Task<bool>> tasks = new List<Task<bool>>();
            tasks.Add(Task.Run(() =>
            {
                System.Threading.Thread.Sleep(5000);
                return false;
            }));
            tasks.Add(Task.Run(() =>
            {
                string proc = "MFConnector";
                while (Process.GetProcessesByName(proc).Length > 0)
                {
                    System.Threading.Thread.Sleep(500);
                }

                return true;
            }));

            var result = await Task.WhenAny(tasks);
            return result.Result;
        }
    }
}
