using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.OutputWizard
{
    public partial class DisplayPanel : UserControl
    {
        public event EventHandler SettingsDialogRequested;

        OutputConfigItem config = null;
        OutputConfigItem originalConfig = null;

        Panels.DisplayPinPanel displayPinPanel = new Panels.DisplayPinPanel();
        Panels.DisplayBcdPanel displayBcdPanel = new Panels.DisplayBcdPanel();
        Panels.DisplayLedDisplayPanel displayLedDisplayPanel = new Panels.DisplayLedDisplayPanel();
        Panels.DisplayNothingSelectedPanel displayNothingSelectedPanel = new Panels.DisplayNothingSelectedPanel();
        Panels.LCDDisplayPanel displayLcdDisplayPanel = new Panels.LCDDisplayPanel();
        Panels.ServoPanel servoPanel = new Panels.ServoPanel();
        Panels.StepperPanel stepperPanel = new Panels.StepperPanel();
        Panels.DisplayShiftRegisterPanel displayShiftRegisterPanel = new Panels.DisplayShiftRegisterPanel();


        public DisplayPanel()
        {
            InitializeComponent();
        }

        public void SetConfigRefsDataView(DataView dv, string filterGuid)
        {
            displayLedDisplayPanel.SetConfigRefsDataView(dv, filterGuid);
        }

        public void Init()
        {
            _initDisplayPanels();
        }

        protected void _initDisplayPanels()
        {
            // make all panels small and store the common height
            groupBoxDisplaySettings.Controls.Add(displayPinPanel);
            displayPinPanel.Dock = DockStyle.Top;
            groupBoxDisplaySettings.Controls.Add(displayBcdPanel);
            displayBcdPanel.Dock = DockStyle.Top;
            groupBoxDisplaySettings.Controls.Add(displayLedDisplayPanel);
            displayLedDisplayPanel.Dock = DockStyle.Top;
            groupBoxDisplaySettings.Controls.Add(displayNothingSelectedPanel);
            displayNothingSelectedPanel.Dock = DockStyle.Top;
            groupBoxDisplaySettings.Controls.Add(servoPanel);
            servoPanel.Dock = DockStyle.Top;
            groupBoxDisplaySettings.Controls.Add(stepperPanel);
            stepperPanel.Dock = DockStyle.Top;
            groupBoxDisplaySettings.Controls.Add(displayShiftRegisterPanel);
            displayShiftRegisterPanel.Dock = DockStyle.Top;
            stepperPanel.OnManualCalibrationTriggered += new EventHandler<Panels.ManualCalibrationTriggeredEventArgs>(stepperPanel_OnManualCalibrationTriggered);
            stepperPanel.OnSetZeroTriggered += new EventHandler(stepperPanel_OnSetZeroTriggered);
            stepperPanel.OnStepperSelected += StepperPanel_OnStepperSelected;


            groupBoxDisplaySettings.Controls.Add(displayLcdDisplayPanel);
            displayLcdDisplayPanel.AutoSize = false;
            displayLcdDisplayPanel.Height = 0;
            displayLcdDisplayPanel.Dock = DockStyle.Top;

            displayPanels.Clear();
            displayPanelHeight = 0;
            displayPanels.Add(displayPinPanel);
            displayPanels.Add(displayBcdPanel);
            displayPanels.Add(displayLedDisplayPanel);
            displayPanels.Add(displayNothingSelectedPanel);
            displayPanels.Add(servoPanel);
            displayPanels.Add(stepperPanel);
            displayPanels.Add(displayLcdDisplayPanel);
            displayPanels.Add(displayShiftRegisterPanel);

            foreach (UserControl p in displayPanels)
            {
                if (p.Height > 0 && (p.Height > displayPanelHeight)) displayPanelHeight = p.Height;
                p.Height = 0;
            } //foreach

            displayNothingSelectedPanel.Height = displayPanelHeight;
            displayNothingSelectedPanel.Enabled = true;
        }

        private void displaySerialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
#if DEBUG
            Log.Instance.log("displaySerialComboBox_SelectedIndexChanged: called.", LogSeverity.Debug);
#endif
            // check which extension type is available to current serial
            ComboBox cb = (sender as ComboBox);

            try
            {
                // disable test button
                // in case that no display is selected                
                String serial = SerialNumber.ExtractSerial(cb.SelectedItem.ToString());

                displayTypeComboBox.Enabled = groupBoxDisplaySettings.Enabled = testSettingsGroupBox.Enabled = (serial != "");
                // serial is empty if no module is selected (on init of form)
                //if (serial == "") return;                

                // update the available types depending on the 
                // type of module
                if (serial.IndexOf("SN") != 0)
                {
                    displayTypeComboBox.Items.Clear();
                    displayTypeComboBox.Items.Add("Pin");
                    displayTypeComboBox.Items.Add(ArcazeLedDigit.TYPE);
                    displayTypeComboBox.Items.Add(MobiFlightShiftRegister.TYPE);
                    //displayTypeComboBox.Items.Add(ArcazeBcd4056.TYPE);
                }
                else
                {
                    displayTypeComboBox.Items.Clear();
                    MobiFlightModule module = _execManager.getMobiFlightModuleCache().GetModuleBySerial(serial);
                    foreach (DeviceType devType in module.GetConnectedOutputDeviceTypes())
                    {
#if DEBUG
                        Log.Instance.log("displaySerialComboBox_SelectedIndexChanged: Adding Device Type: " + devType.ToString(), LogSeverity.Debug);
#endif
                        switch (devType)
                        {
                            case DeviceType.LedModule:
                                displayTypeComboBox.Items.Add(ArcazeLedDigit.TYPE);
                                break;

                            case DeviceType.Output:
                                displayTypeComboBox.Items.Add("Pin");
                                //displayTypeComboBox.Items.Add(ArcazeBcd4056.TYPE);
                                break;

                            case DeviceType.Servo:
                                displayTypeComboBox.Items.Add(DeviceType.Servo.ToString("F"));
                                break;

                            case DeviceType.Stepper:
                                displayTypeComboBox.Items.Add(DeviceType.Stepper.ToString("F"));
                                break;

                            case DeviceType.LcdDisplay:
                                displayTypeComboBox.Items.Add(DeviceType.LcdDisplay.ToString("F"));
                                break;

                            case DeviceType.ShiftRegister:
                                displayTypeComboBox.Items.Add(MobiFlightShiftRegister.TYPE);
                                break;
                        }
                    }

                    if (displayTypeComboBox.Items.Count == 0)
                    {
                        if (MessageBox.Show(
                                i18n._tr("uiMessageSelectedModuleDoesNotContainAnyOutputDevices"),
                                i18n._tr("Hint"),
                                MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes
                            )
                        {
                            if (SettingsDialogRequested != null)
                            {
                                SettingsDialogRequested(module, new EventArgs());

                                // trigger reload of Type ComboBox
                                int CurrentIdx = displayModuleNameComboBox.SelectedIndex;
                                displayModuleNameComboBox.SelectedIndex = 0;
                                displayModuleNameComboBox.SelectedIndex = CurrentIdx;
                            }
                        }
                    }
                }

                // third tab
                if (!ComboBoxHelper.SetSelectedItem(displayTypeComboBox, config.DisplayType))
                {
                    // TODO: provide error message
                    Log.Instance.log("displayArcazeSerialComboBox_SelectedIndexChanged : Problem setting Display Type ComboBox", LogSeverity.Debug);
                }

            }
            catch (Exception ex)
            {
                displayPinPanel.displayPinBrightnessPanel.Visible = false;
                displayPinPanel.displayPinBrightnessPanel.Enabled = false;
                Log.Instance.log("displayArcazeSerialComboBox_SelectedIndexChanged : Some Exception occurred" + ex.Message, LogSeverity.Debug);
            }
        }
    }
}
