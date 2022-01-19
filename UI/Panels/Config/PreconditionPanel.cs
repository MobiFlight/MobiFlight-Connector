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

namespace MobiFlight.UI.Panels.Config
{
    public partial class PreconditionPanel : UserControl
    {
        public event EventHandler<EventArgs> PreconditionTreeNodeChanged;
        public event EventHandler<EventArgs> ErrorOnValidating;

        ErrorProvider errorProvider = new ErrorProvider();

        DataSet _dataSetConfig = null;
        private PreconditionList Preconditions = new PreconditionList();

        public PreconditionPanel()
        {
            InitializeComponent();
        }

        public void Init()
        {
            _initPreconditionPanel();
        }

        public void preparePreconditionPanel(DataSet dataSetConfig, String filterGuid)
        {
            _dataSetConfig = dataSetConfig;
            DataRow[] rows = dataSetConfig.Tables["config"].Select("guid <> '" + filterGuid + "'");

            // this filters the current config
            DataView dv = new DataView(dataSetConfig.Tables["config"]);
            dv.RowFilter = "guid <> '" + filterGuid + "'";
            preconditionConfigComboBox.DataSource = dv;
            preconditionConfigComboBox.ValueMember = "guid";
            preconditionConfigComboBox.DisplayMember = "description";

            if (preconditionConfigComboBox.Items.Count == 0)
            {
                List<ListItem> preconTypes = new List<ListItem>() {
                new ListItem() { Value = "none",    Label = i18n._tr("LabelPrecondition_None") },
                new ListItem() { Value = "pin",     Label = i18n._tr("LabelPrecondition_ArcazePin") }
                };
                preConditionTypeComboBox.DataSource = preconTypes;
                preConditionTypeComboBox.DisplayMember = "Label";
                preConditionTypeComboBox.ValueMember = "Value";
                preConditionTypeComboBox.SelectedIndex = 0;
            }
        }

        private void _initPreconditionPanel()
        {
            preConditionTypeComboBox.Items.Clear();
            List<ListItem> preconTypes = new List<ListItem>() {
                new ListItem() { Value = "none",    Label = i18n._tr("LabelPrecondition_None") },
                new ListItem() { Value = "config",  Label = i18n._tr("LabelPrecondition_ConfigItem") },
                new ListItem() { Value = "pin",     Label = i18n._tr("LabelPrecondition_ArcazePin") }
            };
            preConditionTypeComboBox.DataSource = preconTypes;
            preConditionTypeComboBox.DisplayMember = "Label";
            preConditionTypeComboBox.ValueMember = "Value";
            preConditionTypeComboBox.SelectedIndex = 0;

            preconditionConfigComboBox.SelectedIndex = 0;
            preconditionRefOperandComboBox.SelectedIndex = 0;

            // init the pin-type config panel
            List<ListItem> preconPinValues = new List<ListItem>() {
                new ListItem() { Value = "0", Label = "Off" },
                new ListItem() { Value = "1", Label = "On" },
            };

            preconditionPinValueComboBox.DataSource = preconPinValues;
            preconditionPinValueComboBox.DisplayMember = "Label";
            preconditionPinValueComboBox.ValueMember = "Value";
            preconditionPinValueComboBox.SelectedIndex = 0;

            preconditionSettingsPanel.Enabled = false;
            ApplyButtonPanel.Visible = false;
        }

        public void SetModules(List<ListItem> ModuleList)
        {
            preconditionPinSerialComboBox.Items.Clear();
            preconditionPinSerialComboBox.Items.Add(new ListItem() { Value = "-", Label = "" });
            preconditionPinSerialComboBox.Items.AddRange(ModuleList.ToArray());
            preconditionPinSerialComboBox.SelectedIndex = 0;
        }

        public bool syncFromConfig(IBaseConfigItem config)
        {
            preconditionListTreeView.Nodes.Clear();
            Preconditions = config.Preconditions.Clone() as PreconditionList;

            foreach (Precondition p in Preconditions)
            {
                TreeNode tmpNode = new TreeNode();
                tmpNode.Text = p.ToString();
                tmpNode.Tag = p;
                tmpNode.Checked = p.PreconditionActive;
                try
                {
                    _updateNodeWithPrecondition(tmpNode, p);
                    preconditionListTreeView.Nodes.Add(tmpNode);
                }
                catch (IndexOutOfRangeException e)
                {
                    Log.Instance.log("An orphaned precondition has been found", LogSeverity.Error);
                    continue;
                }
            }

            overridePreconditionCheckBox.Checked = config.Preconditions.ExecuteOnFalse;
            overridePreconditionTextBox.Text = config.Preconditions.FalseCaseValue;

            return true;
        }

        public bool syncToConfig(IBaseConfigItem config)
        {
            config.Preconditions = Preconditions;
            config.Preconditions.ExecuteOnFalse = overridePreconditionCheckBox.Checked;
            config.Preconditions.FalseCaseValue = overridePreconditionTextBox.Text;
            return true;
        }

        private void preConditionTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = ((sender as ComboBox).SelectedItem as ListItem).Value;

            preconditionSettingsGroupBox.Visible = selected != "none";
            preconditionRuleConfigPanel.Visible = selected == "config";
            preconditionPinPanel.Visible = selected == "pin";

            if (preconditionRuleConfigPanel.Visible)
                preconditionSettingsGroupBox.Height = preconditionRuleConfigPanel.Height;
            else if (preconditionPinPanel.Visible)
            {
                preconditionSettingsGroupBox.Height = preconditionPinPanel.Height;
            }
        }

        private void preconditionRefValueTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (!(preconditionRuleConfigPanel).Visible)
            {
                removeError(preconditionRefValueTextBox);
                return;
            }

            if (preconditionRefValueTextBox.Text.Trim() == "")
            {
                e.Cancel = true;
                displayError(preconditionRefValueTextBox, i18n._tr("uiMessageConfigWizard_SelectComparison"));
            }
            else
            {
                removeError(preconditionRefValueTextBox);
            }
        }

        private void preconditionPinSerialComboBox_Validating(object sender, CancelEventArgs e)
        {
            if (!(preconditionPinPanel).Visible)
            {
                removeError(preconditionRefValueTextBox);
                return;
            }

            if (preconditionPinSerialComboBox.Items.Count > 1 && preconditionPinSerialComboBox.Text.Trim() == "-")
            {
                e.Cancel = true;
                ErrorOnValidating?.Invoke(this, new EventArgs());

                preconditionPinSerialComboBox.Focus();
                displayError(preconditionPinSerialComboBox, i18n._tr("uiMessageConfigWizard_SelectArcaze"));
            }
            else
            {
                removeError(preconditionPinSerialComboBox);
            }

        }

        private void preconditionPinComboBox_Validating(object sender, CancelEventArgs e)
        {
            if (!(preconditionPinPanel).Visible)
            {
                removeError(preconditionPinComboBox);
                return;
            }

            if (preconditionPinSerialComboBox.SelectedIndex > 0 && preconditionPinComboBox.SelectedIndex == -1)
            {
                e.Cancel = true;
                ErrorOnValidating?.Invoke(this, new EventArgs());

                displayError(preconditionPinComboBox, i18n._tr("Please_select_a_pin"));
            }
            else
            {
                removeError(preconditionPinComboBox);
            }
        }

        private void preconditionPortComboBox_Validating(object sender, CancelEventArgs e)
        {
            if (!(preconditionPinPanel).Visible)
            {
                removeError(preconditionPortComboBox);
                return;
            }

            if (preconditionPinSerialComboBox.SelectedIndex > 0 && preconditionPortComboBox.SelectedIndex == -1)
            {
                e.Cancel = true;
                ErrorOnValidating?.Invoke(this, new EventArgs());
                displayError(preconditionPortComboBox, i18n._tr("Please_select_a_port"));
            }
            else
            {
                removeError(preconditionPortComboBox);
            }
        }


        private void preconditionListTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            preconditionListTreeView.SelectedNode = e.Node;
            //if (e.Button != System.Windows.Forms.MouseButtons.Left) return;

            Precondition config = (e.Node.Tag as Precondition);
            preConditionTypeComboBox.SelectedValue = config.PreconditionType;
            preconditionSettingsPanel.Enabled = true;
            ApplyButtonPanel.Visible = true;
            config.PreconditionActive = e.Node.Checked;

            switch (config.PreconditionType)
            {
                case "config":
                    try
                    {
                        preconditionConfigComboBox.SelectedValue = config.PreconditionRef;
                    }
                    catch (Exception exc)
                    {
                        // precondition could not be loaded
                        Log.Instance.log("preconditionListTreeView_NodeMouseClick : Precondition could not be loaded, " + exc.Message, LogSeverity.Debug);
                    }

                    ComboBoxHelper.SetSelectedItem(preconditionRefOperandComboBox, config.PreconditionOperand);
                    preconditionRefValueTextBox.Text = config.PreconditionValue;
                    break;

                case "pin":
                    ArcazeIoBasic io = new ArcazeIoBasic(config.PreconditionPin);
                    ComboBoxHelper.SetSelectedItemByPart(preconditionPinSerialComboBox, config.PreconditionSerial);
                    preconditionPinValueComboBox.SelectedValue = config.PreconditionValue;
                    preconditionPortComboBox.SelectedIndex = io.Port;
                    preconditionPinComboBox.SelectedIndex = io.Pin;
                    break;
            }

            aNDToolStripMenuItem.Checked = config.PreconditionLogic == "and";
            oRToolStripMenuItem.Checked = config.PreconditionLogic == "or";
        }

        private void preconditionApplyButton_Click(object sender, EventArgs e)
        {
            // sync the selected node with the current settings from the panels
            TreeNode selectedNode = preconditionListTreeView.SelectedNode;
            if (selectedNode == null) return;

            Precondition c = selectedNode.Tag as Precondition;

            c.PreconditionType = (preConditionTypeComboBox.SelectedItem as ListItem).Value;
            switch (c.PreconditionType)
            {
                case "config":
                    c.PreconditionRef = preconditionConfigComboBox.SelectedValue.ToString();
                    c.PreconditionOperand = preconditionRefOperandComboBox.Text;
                    c.PreconditionValue = preconditionRefValueTextBox.Text;
                    c.PreconditionActive = true;
                    break;

                case "pin":
                    c.PreconditionSerial = preconditionPinSerialComboBox.Text;
                    c.PreconditionValue = preconditionPinValueComboBox.SelectedValue.ToString();
                    c.PreconditionPin = preconditionPortComboBox.Text + preconditionPinComboBox.Text;
                    c.PreconditionActive = true;
                    break;
            }

            _updateNodeWithPrecondition(selectedNode, c);
        }

        private void _updateNodeWithPrecondition(TreeNode node, Precondition p)
        {
            String label = p.PreconditionLabel;
            if (p.PreconditionType == "config")
            {
                String replaceString = "[unknown]";
                if (_dataSetConfig != null)
                {
                    DataRow[] rows = _dataSetConfig.Tables["config"].Select("guid = '" + p.PreconditionRef + "'");
                    if (rows.Count() == 0) throw new IndexOutOfRangeException(); // an orphaned entry has been found
                    replaceString = rows[0]["description"] as String;
                }
                label = label.Replace("<Ref:" + p.PreconditionRef + ">", replaceString);
            }
            else if (p.PreconditionType == "pin")
            {
                label = label.Replace("<Serial:" + p.PreconditionSerial + ">", p.PreconditionSerial.Split('/')[0]);
            }

            label = label.Replace("<Logic:and>", " (AND)").Replace("<Logic:or>", " (OR)");
            node.Checked = p.PreconditionActive;
            node.Tag = p;
            node.Text = label;
            aNDToolStripMenuItem.Checked = p.PreconditionLogic == "and";
            oRToolStripMenuItem.Checked = p.PreconditionLogic == "or";
        }


        private void addPreconditionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Precondition p = new Precondition();
            TreeNode n = new TreeNode();
            n.Tag = p;
            Preconditions.Add(p);
            preconditionListTreeView.Nodes.Add(n);
            _updateNodeWithPrecondition(n, p);
        }

        private void andOrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = preconditionListTreeView.SelectedNode;
            Precondition p = selectedNode.Tag as Precondition;
            if ((sender as ToolStripMenuItem).Text == "AND")
                p.PreconditionLogic = "and";
            else
                p.PreconditionLogic = "or";

            _updateNodeWithPrecondition(selectedNode, p);
        }

        private void removePreconditionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = preconditionListTreeView.SelectedNode;
            Precondition p = selectedNode.Tag as Precondition;
            Preconditions.Remove(p);
            preconditionListTreeView.Nodes.Remove(selectedNode);
        }

        private void preconditionPinSerialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get the deviceinfo for the current arcaze
            ComboBox cb = preconditionPinSerialComboBox;
            String serial = SerialNumber.ExtractSerial(cb.SelectedItem.ToString());
            // if (serial == "" && config.DisplaySerial != null) serial = ArcazeModuleSettings.ExtractSerial(config.DisplaySerial);

            if (serial.IndexOf("SN") != 0)
            {
                preconditionPortComboBox.Items.Clear();
                preconditionPinComboBox.Items.Clear();

                List<ListItem> ports = new List<ListItem>();

                foreach (String v in ArcazeModule.getPorts())
                {
                    ports.Add(new ListItem() { Label = v, Value = v });
                    if (v == "B" || v == "E" || v == "H" || v == "K")
                    {
                        ports.Add(new ListItem() { Label = "-----", Value = "-----" });
                    }

                    if (v == "A" || v == "B")
                    {
                        preconditionPortComboBox.Items.Add(v);
                    }
                }

                List<ListItem> pins = new List<ListItem>();
                foreach (String v in ArcazeModule.getPins())
                {
                    pins.Add(new ListItem() { Label = v, Value = v });
                    preconditionPinComboBox.Items.Add(v);
                }
            }
        }

        private void displayError(Control control, String message)
        {
            errorProvider.SetError(
                    control,
                    message);
            MessageBox.Show(message, i18n._tr("Hint"));
        }

        private void removeError(Control control)
        {
            errorProvider.SetError(
                    control,
                    "");
        }
    }
}
