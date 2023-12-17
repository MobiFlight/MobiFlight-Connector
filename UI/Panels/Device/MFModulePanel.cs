using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Web.UI.WebControls;

namespace MobiFlight.UI.Panels.Settings.Device
{
    public partial class MFModulePanel : UserControl
    {
        /// <summary>
        /// Gets raised whenever config object has changed
        /// </summary>
        public event EventHandler Changed;
        public event EventHandler<string> UploadDefaultConfigRequested;

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
                var boards = BoardDefinitions.GetBoardsByHardwareId(module.HardwareId).FindAll(b => b.PartnerLevel==BoardPartnerLevel.Core);
                var tooltipLabel = string.Join(" / ",boards.Select(b=>b.Info.FriendlyName).ToList());
                toolTip1.SetToolTip(TypeValueLabel, tooltipLabel);
                TypeValueLabel.Text = i18n._tr("uiLabelModuleTYPE_COMPATIBLE") + $" ({tooltipLabel})";
            } 

            PortValueLabel.Text = module.Port;

            DisplayDetails(module.Board);

            initialized = true;
        }

        private void DisplayDetails(Board board)
        {
            if (board.Info.Community == null)
            {
                groupBoxDetails.Visible = false;
                return;
            }

            if (board.Info.Community?.Project != null)
                labelProjectValue.Text = board.Info.Community.Project;
            else
                labelProjectValue.Text= "Unknown";

            if (board.Info.Community != null)
            {
                pictureBoxLogo.Image = board.Info.Community.Logo;
                pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            } else
            {
                pictureBoxLogo.Visible = false;
            }
            

            if (board.Info.Community?.Website != null)
                buttonWebsite.Click += (s, e) => { Process.Start(board.Info.Community.Website); };
            else
                buttonWebsite.Enabled = false;

            if (board.Info.Community?.Docs != null)
                buttonDocs.Click += (s, e) => { Process.Start(board.Info.Community.Docs); };
            else
                buttonDocs.Enabled = false;
            
            if (board.Info.Community?.Support != null)
                buttonSupport.Click += (s, e) => { Process.Start(board.Info.Community.Support); };
            else
                buttonSupport.Enabled = false;


            panel1.Visible = false;
            if (board.Info.HasDefaultDeviceConfig)
            {
                var configFile = board.GetDefaultConfigPath();
                if (configFile != null && File.Exists(configFile)){
                    buttonUploadDefaultConfig.Click += (s, e) =>
                    {
                        UploadDefaultConfigRequested?.Invoke(this, configFile);
                    };
                    panel1.Visible = true;
                }
            }
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
