using MobiFlight.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Forms
{
    public partial class FirmwareUpdateProcess : Form
    {
        public event EventHandler OnBeforeFirmwareUpdate;
        public event EventHandler OnAfterFirmwareUpdate;
        public event FirmwareUpdateFinishedEventHandler OnFinished;

        public delegate void FirmwareUpdateFinishedEventHandler (List<MobiFlightModule> modules);

        List<MobiFlightModule> modules = new List<MobiFlightModule>();
        List<MobiFlightModule> FailedModules = new List<MobiFlightModule>();

        public int TotalModuleCount { get; set; }

        private int NumberOfModulesForFirmwareUpdate = 0;
        private bool UpdateResult = true;

        public FirmwareUpdateProcess()
        {
            InitializeComponent();
            progressBar1.Step = 1;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 1000;
            progressBar1.Value = 0;
            timer1.Interval = 1000;
        }

        public void AddModule(MobiFlightModule module)
        {
            modules.Add(module);
            TotalModuleCount = modules.Count;
        }

        public void ClearModules()
        {
            modules.Clear();
        }

        public void SetCurrentModule(MobiFlightModule module, int Count)
        {
            StatusLabel.Text = string.Format(
                                    i18n._tr("uiMessageFirmwareUpdateStatus"),
                                    module.Name,
                                    module.Port,
                                    Count,
                                    TotalModuleCount
                                    );
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value + (timer1.Interval) > progressBar1.Maximum) 
                progressBar1.Value = 0;
            progressBar1.Value += (timer1.Interval);
        }

        private void FirmwareUpdateProcess_Shown(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            FailedModules.Clear();

            NumberOfModulesForFirmwareUpdate = modules.Count;

            foreach (MobiFlightModule module in modules)
            {
                SetCurrentModule(module, modules.Count);
                UpdateModule(module);
            }
            timer1.Start();
        }

        private void FirmwareUpdateProcess_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
        }

        void UpdateModule(MobiFlightModule module)
        {
            String arduinoIdePath = Properties.Settings.Default.ArduinoIdePathDefault;
            String firmwarePath = Directory.GetCurrentDirectory() + "\\firmware";

            if (!MobiFlightFirmwareUpdater.IsValidArduinoIdePath(arduinoIdePath))
            {
                MessageBox.Show(
                    i18n._tr("uiMessageFirmwareCheckPath"),
                    i18n._tr("Hint"), MessageBoxButtons.OK);
                return;
            }

            OnBeforeFirmwareUpdate?.Invoke(module, null);
            module.Disconnect();

            MobiFlightFirmwareUpdater.ArduinoIdePath = arduinoIdePath;
            MobiFlightFirmwareUpdater.FirmwarePath = firmwarePath;

            Task<bool>.Run(
                () => {
                    bool UpdateResult = MobiFlightFirmwareUpdater.Update(module);
                    return UpdateResult;
                })
                .ContinueWith( 
                result =>
                {
                    NumberOfModulesForFirmwareUpdate--;

                    StatusLabel.Text = string.Format(
                                        "{0} ({1}) firmware update completed ({2}/{3})",
                                        module.Name,
                                        module.Port,
                                        TotalModuleCount - NumberOfModulesForFirmwareUpdate,
                                        TotalModuleCount
                                        );

                    if (!result.Result)
                        FailedModules.Add(module);

                    progressBar1.Value = (int)Math.Round(progressBar1.Maximum * ((TotalModuleCount - NumberOfModulesForFirmwareUpdate) / (float)TotalModuleCount));

                    OnAfterFirmwareUpdate?.Invoke(module, null);

                    if (NumberOfModulesForFirmwareUpdate == 0)
                    {
                        timer1.Stop();
                        this.Hide();
                        OnFinished?.Invoke(FailedModules);
                        this.Close();
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext()
                );
        }
    }
}
