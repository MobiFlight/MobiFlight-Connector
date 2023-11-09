using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings.Device
{
    public partial class MFModulePanel : UserControl
    {
        /// <summary>
        /// Gets raised whenever config object has changed
        /// </summary>
        public event EventHandler Changed;

        private MobiFlightModule module;
        bool initialized = false;

        public MFModulePanel()
        {
            InitializeComponent();
        }

        public MFModulePanel(MobiFlightModule module)
            : this()
        {
            // TODO: Complete member initialization
            this.module = module;
            moduleNameTextBox.Text = module.Name;
            if (module.Name == "Unknown")
            {
                moduleNameTextBox.Text = i18n._tr("uiLabelModuleNAME_UNKNOWN");
                moduleNameTextBox.ReadOnly = true;
            } 
            FirmwareValueLabel.Text = module.Version;
            SerialValueLabel.Text = module.Serial != string.Empty ? module.Serial : " - ";

            TypeValueLabel.Text = module.Type;
            if(module.Type==MobiFlightModule.TYPE_COMPATIBLE)
            {
                var boards = BoardDefinitions.GetBoardsByHardwareId(module.HardwareId);
                var tooltipLabel = string.Join(" / ",boards.Select(b=>b.Info.FriendlyName).ToList());
                toolTip1.SetToolTip(TypeValueLabel, tooltipLabel);
                TypeValueLabel.Text = i18n._tr("uiLabelModuleTYPE_COMPATIBLE") + $" ({tooltipLabel})";
            } 

            PortValueLabel.Text = module.Port;

            initialized = true;
        }

        private void value_Changed(object sender, EventArgs e)
        {
            if (!initialized) return;

            if (!module.HasFirmwareFeature(FirmwareFeature.SetName))
            {
                MessageBox.Show(i18n._tr("uiMessageSettingsDialogFirmwareVersionTooLowException"), i18n._tr("Hint"));
                return;
            }
            module.Name = moduleNameTextBox.Text;

            if (Changed != null)
                Changed(module, new EventArgs());
        }

        private void moduleNameTextBox_Validating(object sender, CancelEventArgs e)
        {

        }
    }
}
