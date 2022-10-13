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

        public bool ResetMode = false;
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
            var Message = i18n._tr("uiMessageFirmwareUpdateStatus");
            if (ResetMode) Message = i18n._tr("uiMessageFirmwareResetStatus");

            StatusLabel.Text = string.Format(
                                    Message,
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
                if (!ResetMode)
                    UpdateModule(module);
                else
                    ResetModule(module);
            }
            timer1.Start();
        }

        private void FirmwareUpdateProcess_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
        }

        async void UpdateOrResetModule(MobiFlightModule module, bool IsUpdate)
        {
            var MessageComplete = i18n._tr("uiMessageFirmwareUpdateComplete");
            var MessageTimeout = i18n._tr("uiMessageFirmwareUpdateTimeout");

            if (!IsUpdate)
            {
                MessageComplete = i18n._tr("uiMessageFirmwareResetComplete");
                MessageTimeout = i18n._tr("uiMessageFirmwareResetTimeout");
            }

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

            int timeout = 15000;
            var task = Task<bool>.Run(() => {
                bool UpdateResult;
                if (IsUpdate)
                    UpdateResult = MobiFlightFirmwareUpdater.Update(module);
                else
                    UpdateResult = MobiFlightFirmwareUpdater.Reset(module);
                return UpdateResult;
            });
            if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
            {
                NumberOfModulesForFirmwareUpdate--;

                StatusLabel.Text = string.Format(
                                    MessageComplete,
                                    module.Name,
                                    module.Port,
                                    TotalModuleCount - NumberOfModulesForFirmwareUpdate,
                                    TotalModuleCount
                                    );

                if (!task.Result)
                    FailedModules.Add(module);
            }
            else
            {
                NumberOfModulesForFirmwareUpdate--;
                FailedModules.Add(module);
                StatusLabel.Text = string.Format(
                                    MessageTimeout,
                                    module.Name,
                                    module.Port,
                                    TotalModuleCount - NumberOfModulesForFirmwareUpdate,
                                    TotalModuleCount
                                    );
            };

            progressBar1.Value = (int)Math.Round(progressBar1.Maximum * ((TotalModuleCount - NumberOfModulesForFirmwareUpdate) / (float)TotalModuleCount));

            OnAfterFirmwareUpdate?.Invoke(module, null);

            if (NumberOfModulesForFirmwareUpdate == 0)
            {
                timer1.Stop();
                this.Hide();
                OnFinished?.Invoke(FailedModules);
                this.Close();
            }
        }

        void UpdateModule(MobiFlightModule module)
        {
            UpdateOrResetModule(module, true);
        }

        void ResetModule(MobiFlightModule module)
        {
            UpdateOrResetModule(module, false);
        }
    }
}
