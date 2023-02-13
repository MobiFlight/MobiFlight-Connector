using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Config
{
    public partial class PreconditionPanel : UserControl
    {
        public event EventHandler<EventArgs> PreconditionTreeNodeChanged;
        public event EventHandler<EventArgs> ErrorOnValidating;

        protected bool suspendFormValueChanged = false;

        ErrorProvider errorProvider = new ErrorProvider();

        private PreconditionList Preconditions = new PreconditionList();

        public List<ListItem> Configs { get; private set; } = new List<ListItem>();
        public List<ListItem> Variables { get; private set; } = new List<ListItem>();

        public PreconditionPanel()
        {
            InitializeComponent();
        }

        public void Init()
        {
            _initPreconditionPanel();
            PreconditionTreeNodeChanged += PreconditionPanel_PreconditionTreeNodeChanged;
            preconditionListTreeView.AfterSelect += PreconditionListTreeView_AfterSelect;
            preconditionListTreeView.AfterCheck += PreconditionListTreeView_AfterCheck; ;
            preConditionTypeComboBox.SelectedIndexChanged += FormValueChanged;
            preconditionConfigComboBox.SelectedIndexChanged += FormValueChanged;
            preconditionRefOperandComboBox.SelectedIndexChanged += FormValueChanged;
            preconditionRefValueTextBox.LostFocus += FormValueChanged;
            preconditionPinSerialComboBox.SelectedIndexChanged += FormValueChanged;
            preconditionPortComboBox.SelectedIndexChanged += FormValueChanged;
            preconditionPinComboBox.SelectedIndexChanged += FormValueChanged;
            preconditionPinValueComboBox.SelectedIndexChanged += FormValueChanged;

            var treeViewImageList = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit,
                ImageSize = new System.Drawing.Size(16, 16)
            };
            treeViewImageList.Images.Add("pin", MobiFlight.Properties.Resources.media_stop);
            treeViewImageList.Images.Add("config", MobiFlight.Properties.Resources.media_stop_red);
            treeViewImageList.Images.Add("variable", MobiFlight.Properties.Resources.module_mobiflight);
            treeViewImageList.Images.Add("missing", MobiFlight.Properties.Resources.warning);

            preconditionListTreeView.ImageList = treeViewImageList;
        }

        private void PreconditionListTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            (e.Node.Tag as Precondition).PreconditionActive = e.Node.Checked;
        }

        private void FormValueChanged(object sender, EventArgs e)
        {
            if (suspendFormValueChanged) return;
            UpdatePreconditionAfterChange(sender, e);
        }

        private void PreconditionListTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            suspendFormValueChanged = true;
            Precondition config = (e.Node.Tag as Precondition);
            preConditionTypeComboBox.SelectedValue = config.PreconditionType;
            preconditionSettingsPanel.Enabled = true;

            switch (config.PreconditionType)
            {
                case "variable":
                case "config":
                    try
                    {
                        preconditionConfigComboBox.SelectedValue = config.PreconditionRef;
                    }
                    catch (Exception ex)
                    {
                        // precondition could not be loaded
                        Log.Instance.log($"Precondition could not be loaded: {ex.Message}", LogSeverity.Error);
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
            suspendFormValueChanged = false;
        }

        private void PreconditionPanel_PreconditionTreeNodeChanged(object sender, EventArgs e)
        {
            UpdateNodeLabelsAndImages();
        }

        public void SetAvailableConfigs(List<ListItem> configs)
        {
            Configs = configs;
        }

        public void SetAvailableVariables(Dictionary<string, MobiFlightVariable> dictionary)
        {
            if (Variables == null) return;
            var options = new List<ListItem>();

            foreach (var variable in dictionary.Values)
            {
                options.Add(new ListItem { Label = variable.Name, Value = variable.Name });
            }

            Variables = options;
        }

        private static List<ListItem> GetPreconditionTypeOptions()
        {
            var result = new List<ListItem>() {
                    new ListItem() { Value = "none",    Label = i18n._tr("Label_Precondition_None") },
                    new ListItem() { Value = "config",  Label = i18n._tr("Label_Precondition_ConfigItem") },
                    new ListItem() { Value = "variable",Label = i18n._tr("Label_Precondition_Variable") },
            };

            if (Properties.Settings.Default.ArcazeSupportEnabled)
            {
                result.Add(new ListItem() { Value = "pin", Label = i18n._tr("Label_Precondition_ArcazePin") });
            }
            return result;
        }

        private void _initPreconditionPanel()
        {
            preConditionTypeComboBox.Items.Clear();
            List<ListItem> preconTypes = GetPreconditionTypeOptions();

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
                preconditionListTreeView.Nodes.Add(tmpNode);
                PreconditionTreeNodeChanged?.Invoke(tmpNode, null);
            }

            UpdateNodeLabelsAndImages();

            return true;
        }

        public bool syncToConfig(IBaseConfigItem config)
        {
            config.Preconditions = Preconditions;
            return true;
        }

        private void preConditionTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected = ((sender as ComboBox).SelectedItem as ListItem).Value;

            preconditionSettingsGroupBox.Visible = selected != "none";
            preconditionRuleConfigPanel.Visible = selected == "config" || selected == "variable";
            preconditionPinPanel.Visible = selected == "pin";

            if (selected == "config" || selected == "variable")
            {
                preconditionSettingsGroupBox.Height = preconditionRuleConfigPanel.Height;
                if (selected == "config")
                {
                    preconditionConfigLabel.Text = i18n._tr("Label_Precondition_choose_config");
                    preconditionConfigComboBox.DataSource = Configs;
                }
                else
                {
                    preconditionConfigLabel.Text = i18n._tr("Label_Precondition_choose_variable");
                    preconditionConfigComboBox.DataSource = Variables;
                }

                preconditionConfigComboBox.ValueMember = "Value";
                preconditionConfigComboBox.DisplayMember = "Label";
            }

            else if (preconditionPinPanel.Visible)
            {
                preconditionSettingsGroupBox.Height = preconditionPinPanel.Height;
            }
        }

        private void UpdatePreconditionAfterChange(object sender, EventArgs e)
        {
            // sync the selected node with the current settings from the panels
            TreeNode selectedNode = preconditionListTreeView.SelectedNode;
            if (selectedNode == null) return;

            Precondition c = selectedNode.Tag as Precondition;

            c.PreconditionType = (preConditionTypeComboBox.SelectedItem as ListItem).Value;
            switch (c.PreconditionType)
            {
                case "variable":
                case "config":
                    if (sender == preconditionConfigComboBox)
                        c.PreconditionRef = preconditionConfigComboBox.SelectedValue.ToString();
                    if (sender == preconditionRefOperandComboBox)
                        c.PreconditionOperand = preconditionRefOperandComboBox.Text;
                    if (sender == preconditionRefValueTextBox)
                        c.PreconditionValue = preconditionRefValueTextBox.Text;
                    c.PreconditionActive = selectedNode.Checked;
                    break;

                case "pin":
                    c.PreconditionSerial = preconditionPinSerialComboBox.Text;
                    c.PreconditionValue = preconditionPinValueComboBox.SelectedValue.ToString();
                    c.PreconditionPin = preconditionPortComboBox.Text + preconditionPinComboBox.Text;
                    c.PreconditionActive = selectedNode.Checked;
                    break;
            }

            _updateNodeWithPrecondition(selectedNode, c);
        }

        private void _updateNodeWithPrecondition(TreeNode node, Precondition p)
        {
            node.Checked = p.PreconditionActive;
            node.Tag = p;

            aNDToolStripMenuItem.Checked = p.PreconditionLogic == "and";
            oRToolStripMenuItem.Checked = p.PreconditionLogic == "or";

            PreconditionTreeNodeChanged?.Invoke(node, EventArgs.Empty);
        }

        private void SetNodeImage(TreeNode node, Precondition p, bool referenceIsMissing = false)
        {
            switch (p.PreconditionType)
            {
                case "config":
                    node.ImageKey = "config";
                    break;

                case "variable":
                    node.ImageKey = "variable";
                    break;

                case "pin":
                    node.ImageKey = "pin";
                    break;

                default:
                    node.ImageKey = "";
                    break;
            }

            if (referenceIsMissing)
                node.ImageKey = "missing";

            node.SelectedImageKey = node.ImageKey;
        }

        private void UpdateNodeLabelsAndImages()
        {
            foreach (TreeNode node in preconditionListTreeView.Nodes)
            {
                var p = node.Tag as Precondition;
                String label = p.PreconditionLabel;
                var isMissing = false;
                                   
                if (p.PreconditionType == "config")
                {
                    String replaceString = "[unknown]";
                    if (Configs != null && p.PreconditionRef != null)
                    {
                        var config = Configs.Find(c => c.Value == p.PreconditionRef);
                        if (config == null)
                        {
                            isMissing = true;
                            replaceString = "[missing]";
                            Log.Instance.log($"Precondition: config reference missing > {p.PreconditionRef}", LogSeverity.Warn);
                        }
                        else
                            replaceString = config.Label;
                    }
                    label = label.Replace($"<Ref:{p.PreconditionRef}>", replaceString);
                }
                else if (p.PreconditionType == "variable")
                {
                    label = label.Replace($"<Variable:{p.PreconditionRef}>", p.PreconditionRef != null ? p.PreconditionRef : "");
                }
                else if (p.PreconditionType == "pin")
                {
                    label = label.Replace("<Serial:" + p.PreconditionSerial + ">", SerialNumber.ExtractDeviceName(p.PreconditionSerial));
                }
                else
                {
                    label = label.Replace("none", i18n._tr("Label_Precondition_None"));
                }

                label = label.Replace("<Logic:and>", " (AND)").Replace("<Logic:or>", " (OR)");
                node.Text = label;

                if (NodeIsLastNode(node))
                {
                    node.Text = node.Text.Replace(" (AND)", "").Replace(" (OR)", "");
                }

                SetNodeImage(node, p, isMissing);
            }
        }

        private bool NodeIsLastNode(TreeNode node)
        {
            return
                preconditionListTreeView.Nodes.IndexOf(node) == (preconditionListTreeView.Nodes.Count - 1);
        }

        private void addPreconditionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Precondition p = new Precondition();
            TreeNode n = new TreeNode();
            n.Tag = p;
            Preconditions.Add(p);
            preconditionListTreeView.Nodes.Add(n);
            preconditionListTreeView.SelectedNode = n;
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

            if (Preconditions.Count==0)
            {
                addPreconditionToolStripMenuItem_Click(sender, e);
            }
            
            PreconditionTreeNodeChanged(preconditionListTreeView, null);
        }

        private void preconditionPinSerialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get the deviceinfo for the current arcaze
            ComboBox cb = preconditionPinSerialComboBox;
            string serial = SerialNumber.ExtractSerial(cb.SelectedItem.ToString());
            
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

        #region Validation Events
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
        #endregion
    }
}
