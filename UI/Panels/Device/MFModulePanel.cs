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
using System.Security.Policy;
using System.Web;

namespace MobiFlight.UI.Panels.Settings.Device
{
    public partial class MFModulePanel : UserControl
    {
        /// <summary>
        /// Gets raised whenever config object has changed
        /// </summary>
        public event EventHandler Changed;
        public event EventHandler<string> UploadDefaultConfigRequested;
        const string redirectUrl = "https://mobiflight.com?redirect=";

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
            if (!module.HasMfFirmware())
            {
                moduleNameTextBox.Text = i18n._tr("uiLabelModuleNAME_UNKNOWN");
                moduleNameTextBox.ReadOnly = true;
            } 
            FirmwareValueLabel.Text = module.Version;
            SerialValueLabel.Text = module.Serial != string.Empty ? module.Serial : " - ";

            TypeValueLabel.Text = module.Board.Info.MobiFlightTypeLabel ?? module.Type;
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

            if (board.Info.BoardPicture != null)
            {
                pictureBoxLogo.Image = board.Info.BoardPicture;
                pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            } else
            {
                pictureBoxLogo.Visible = false;
            }
            

            if (board.Info.Community?.Website != null)
                buttonWebsite.Click += (s, e) => { Process.Start(CreateRedirectTarget(board.Info.Community.Website)); };
            else
                buttonWebsite.Enabled = false;

            if (board.Info.Community?.Docs != null)
                buttonDocs.Click += (s, e) => { Process.Start(CreateRedirectTarget(board.Info.Community.Docs)); };
            else
                buttonDocs.Enabled = false;
            
            if (board.Info.Community?.Support != null)
                buttonSupport.Click += (s, e) => { Process.Start(CreateRedirectTarget(board.Info.Community.Support)); };
            else
                buttonSupport.Enabled = false;


            
            var defaultDeviceConfigFile = board.GetDefaultDeviceConfigFilePath();
            if (defaultDeviceConfigFile != null && File.Exists(defaultDeviceConfigFile))
            {
                buttonUploadDefaultConfig.Click += (s, e) =>
                {
                    UploadDefaultConfigRequested?.Invoke(this, defaultDeviceConfigFile);
                };
                panel1.Visible = true;
            } else
            {
                panel1.Visible = false;
            }
        }

        private string CreateRedirectTarget(string target)
        {
            var redirectTarget = target;
            redirectTarget = $"{redirectUrl}{redirectTarget}";

            return redirectTarget;
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
