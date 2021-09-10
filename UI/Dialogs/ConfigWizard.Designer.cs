namespace MobiFlight.UI.Dialogs
{
    partial class ConfigWizard
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigWizard));
            this.MainPanel = new System.Windows.Forms.Panel();
            this.tabControlFsuipc = new System.Windows.Forms.TabControl();
            this.fsuipcTabPage = new System.Windows.Forms.TabPage();
            this.referencesGroupBox = new System.Windows.Forms.GroupBox();
            this.configRefPanel = new MobiFlight.UI.Panels.Config.ConfigRefPanel();
            this.variablePanel1 = new MobiFlight.UI.Panels.Config.VariablePanel();
            this.simConnectPanel1 = new MobiFlight.UI.Panels.Config.SimConnectPanel();
            this.FsuipcSettingsPanel = new System.Windows.Forms.Panel();
            this.fsuipcConfigPanel = new MobiFlight.UI.Panels.Config.FsuipcConfigPanel();
            this.fsuipcHintLabel = new System.Windows.Forms.Label();
            this.OffsetTypePanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.OffsetTypeVariableRadioButton = new System.Windows.Forms.RadioButton();
            this.OffsetTypeSimConnectRadioButton = new System.Windows.Forms.RadioButton();
            this.OffsetTypeFsuipRadioButton = new System.Windows.Forms.RadioButton();
            this.compareTabPage = new System.Windows.Forms.TabPage();
            this.compareSpacerPanel = new System.Windows.Forms.Panel();
            this.interpolationGroupBox = new System.Windows.Forms.GroupBox();
            this.interpolationPanel1 = new MobiFlight.UI.Panels.Config.InterpolationPanel();
            this.interpolationCheckBox = new System.Windows.Forms.CheckBox();
            this.comparisonSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.comparisonSettingsPanel = new System.Windows.Forms.Panel();
            this.comparisonValueTextBox = new System.Windows.Forms.TextBox();
            this.comparisonElseValueTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.comparisonIfValueTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comparisonOperandComboBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comparisonActiveCheckBox = new System.Windows.Forms.CheckBox();
            this.comparisonHintTtextBox = new System.Windows.Forms.TextBox();
            this.displayTabPage = new System.Windows.Forms.TabPage();
            this.testSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.displayPinTestStopButton = new System.Windows.Forms.Button();
            this.displayPinTestButton = new System.Windows.Forms.Button();
            this.groupBoxDisplaySettings = new System.Windows.Forms.GroupBox();
            this.displayTypeGroupBox = new System.Windows.Forms.GroupBox();
            this.arcazeSerialLabel = new System.Windows.Forms.Label();
            this.displayModuleNameComboBox = new System.Windows.Forms.ComboBox();
            this.displayTypeComboBoxLabel = new System.Windows.Forms.Label();
            this.displayTypeComboBox = new System.Windows.Forms.ComboBox();
            this.displayTabTextBox = new System.Windows.Forms.TextBox();
            this.preconditionTabPage = new System.Windows.Forms.TabPage();
            this.preconditionSpacerPanel = new System.Windows.Forms.Panel();
            this.overrideGroupBox = new System.Windows.Forms.GroupBox();
            this.overridePreconditionTextBox = new System.Windows.Forms.TextBox();
            this.overridePreconditionCheckBox = new System.Windows.Forms.CheckBox();
            this.preconditionSettingsPanel = new System.Windows.Forms.Panel();
            this.preconditionSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.preconditionPinPanel = new System.Windows.Forms.Panel();
            this.preconditionPinValueComboBox = new System.Windows.Forms.ComboBox();
            this.preconditionPinValueLabel = new System.Windows.Forms.Label();
            this.preconditionPinComboBox = new System.Windows.Forms.ComboBox();
            this.preconditionPortComboBox = new System.Windows.Forms.ComboBox();
            this.preconditonPinLabel = new System.Windows.Forms.Label();
            this.preconditionPinSerialComboBox = new System.Windows.Forms.ComboBox();
            this.preconditionPinSerialLabel = new System.Windows.Forms.Label();
            this.preconditionRuleConfigPanel = new System.Windows.Forms.Panel();
            this.preconditionRefValueTextBox = new System.Windows.Forms.TextBox();
            this.preconditionRefOperandComboBox = new System.Windows.Forms.ComboBox();
            this.preconditionConfigRefOperandLabel = new System.Windows.Forms.Label();
            this.preconditionConfigComboBox = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.preconditionApplyButton = new System.Windows.Forms.Button();
            this.preconditionSelectGroupBox = new System.Windows.Forms.GroupBox();
            this.preConditionTypeComboBox = new System.Windows.Forms.ComboBox();
            this.preconditionTypeLabel = new System.Windows.Forms.Label();
            this.preconditionListgroupBox = new System.Windows.Forms.GroupBox();
            this.preconditionListTreeView = new System.Windows.Forms.TreeView();
            this.preconditionTreeContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addPreconditionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removePreconditionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.addGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.logicSelectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aNDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.preconditionTreeImageList = new System.Windows.Forms.ImageList(this.components);
            this.preconditionTabTextBox = new System.Windows.Forms.TextBox();
            this.ButtonPanel = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.presetsDataSet = new System.Data.DataSet();
            this.presetDataTable = new System.Data.DataTable();
            this.description = new System.Data.DataColumn();
            this.settingsColumn = new System.Data.DataColumn();
            this.MainPanel.SuspendLayout();
            this.tabControlFsuipc.SuspendLayout();
            this.fsuipcTabPage.SuspendLayout();
            this.referencesGroupBox.SuspendLayout();
            this.FsuipcSettingsPanel.SuspendLayout();
            this.OffsetTypePanel.SuspendLayout();
            this.compareTabPage.SuspendLayout();
            this.interpolationGroupBox.SuspendLayout();
            this.comparisonSettingsGroupBox.SuspendLayout();
            this.comparisonSettingsPanel.SuspendLayout();
            this.displayTabPage.SuspendLayout();
            this.testSettingsGroupBox.SuspendLayout();
            this.displayTypeGroupBox.SuspendLayout();
            this.preconditionTabPage.SuspendLayout();
            this.overrideGroupBox.SuspendLayout();
            this.preconditionSettingsPanel.SuspendLayout();
            this.preconditionSettingsGroupBox.SuspendLayout();
            this.preconditionPinPanel.SuspendLayout();
            this.preconditionRuleConfigPanel.SuspendLayout();
            this.preconditionSelectGroupBox.SuspendLayout();
            this.preconditionListgroupBox.SuspendLayout();
            this.preconditionTreeContextMenuStrip.SuspendLayout();
            this.ButtonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.presetsDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.presetDataTable)).BeginInit();
            this.SuspendLayout();
            // 
            // MainPanel
            // 
            this.MainPanel.Controls.Add(this.tabControlFsuipc);
            resources.ApplyResources(this.MainPanel, "MainPanel");
            this.MainPanel.Name = "MainPanel";
            // 
            // tabControlFsuipc
            // 
            this.tabControlFsuipc.Controls.Add(this.fsuipcTabPage);
            this.tabControlFsuipc.Controls.Add(this.compareTabPage);
            this.tabControlFsuipc.Controls.Add(this.displayTabPage);
            this.tabControlFsuipc.Controls.Add(this.preconditionTabPage);
            resources.ApplyResources(this.tabControlFsuipc, "tabControlFsuipc");
            this.tabControlFsuipc.Name = "tabControlFsuipc";
            this.tabControlFsuipc.SelectedIndex = 0;
            this.tabControlFsuipc.SelectedIndexChanged += new System.EventHandler(this.tabControlFsuipc_SelectedIndexChanged);
            // 
            // fsuipcTabPage
            // 
            this.fsuipcTabPage.Controls.Add(this.referencesGroupBox);
            this.fsuipcTabPage.Controls.Add(this.variablePanel1);
            this.fsuipcTabPage.Controls.Add(this.simConnectPanel1);
            this.fsuipcTabPage.Controls.Add(this.FsuipcSettingsPanel);
            this.fsuipcTabPage.Controls.Add(this.OffsetTypePanel);
            resources.ApplyResources(this.fsuipcTabPage, "fsuipcTabPage");
            this.fsuipcTabPage.Name = "fsuipcTabPage";
            this.fsuipcTabPage.UseVisualStyleBackColor = true;
            // 
            // referencesGroupBox
            // 
            this.referencesGroupBox.Controls.Add(this.configRefPanel);
            resources.ApplyResources(this.referencesGroupBox, "referencesGroupBox");
            this.referencesGroupBox.Name = "referencesGroupBox";
            this.referencesGroupBox.TabStop = false;
            // 
            // configRefPanel
            // 
            resources.ApplyResources(this.configRefPanel, "configRefPanel");
            this.configRefPanel.Name = "configRefPanel";
            // 
            // variablePanel1
            // 
            resources.ApplyResources(this.variablePanel1, "variablePanel1");
            this.variablePanel1.Name = "variablePanel1";
            // 
            // simConnectPanel1
            // 
            resources.ApplyResources(this.simConnectPanel1, "simConnectPanel1");
            this.simConnectPanel1.LVars = ((System.Collections.Generic.List<string>)(resources.GetObject("simConnectPanel1.LVars")));
            this.simConnectPanel1.Name = "simConnectPanel1";
            this.simConnectPanel1.PresetFile = "Presets\\msfs2020_simvars.cip";
            this.simConnectPanel1.PresetFileUser = "Presets\\msfs2020_simvars_user.cip";
            // 
            // FsuipcSettingsPanel
            // 
            this.FsuipcSettingsPanel.Controls.Add(this.fsuipcConfigPanel);
            this.FsuipcSettingsPanel.Controls.Add(this.fsuipcHintLabel);
            resources.ApplyResources(this.FsuipcSettingsPanel, "FsuipcSettingsPanel");
            this.FsuipcSettingsPanel.Name = "FsuipcSettingsPanel";
            // 
            // fsuipcConfigPanel
            // 
            resources.ApplyResources(this.fsuipcConfigPanel, "fsuipcConfigPanel");
            this.fsuipcConfigPanel.Name = "fsuipcConfigPanel";
            this.fsuipcConfigPanel.PresetFile = "";
            // 
            // fsuipcHintLabel
            // 
            resources.ApplyResources(this.fsuipcHintLabel, "fsuipcHintLabel");
            this.fsuipcHintLabel.Name = "fsuipcHintLabel";
            // 
            // OffsetTypePanel
            // 
            this.OffsetTypePanel.Controls.Add(this.label1);
            this.OffsetTypePanel.Controls.Add(this.OffsetTypeVariableRadioButton);
            this.OffsetTypePanel.Controls.Add(this.OffsetTypeSimConnectRadioButton);
            this.OffsetTypePanel.Controls.Add(this.OffsetTypeFsuipRadioButton);
            resources.ApplyResources(this.OffsetTypePanel, "OffsetTypePanel");
            this.OffsetTypePanel.Name = "OffsetTypePanel";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // OffsetTypeVariableRadioButton
            // 
            resources.ApplyResources(this.OffsetTypeVariableRadioButton, "OffsetTypeVariableRadioButton");
            this.OffsetTypeVariableRadioButton.Name = "OffsetTypeVariableRadioButton";
            this.OffsetTypeVariableRadioButton.TabStop = true;
            this.OffsetTypeVariableRadioButton.UseVisualStyleBackColor = true;
            this.OffsetTypeVariableRadioButton.CheckedChanged += new System.EventHandler(this.OffsetTypeFsuipRadioButton_CheckedChanged);
            // 
            // OffsetTypeSimConnectRadioButton
            // 
            resources.ApplyResources(this.OffsetTypeSimConnectRadioButton, "OffsetTypeSimConnectRadioButton");
            this.OffsetTypeSimConnectRadioButton.Name = "OffsetTypeSimConnectRadioButton";
            this.OffsetTypeSimConnectRadioButton.TabStop = true;
            this.OffsetTypeSimConnectRadioButton.UseVisualStyleBackColor = true;
            this.OffsetTypeSimConnectRadioButton.CheckedChanged += new System.EventHandler(this.OffsetTypeFsuipRadioButton_CheckedChanged);
            // 
            // OffsetTypeFsuipRadioButton
            // 
            resources.ApplyResources(this.OffsetTypeFsuipRadioButton, "OffsetTypeFsuipRadioButton");
            this.OffsetTypeFsuipRadioButton.Name = "OffsetTypeFsuipRadioButton";
            this.OffsetTypeFsuipRadioButton.TabStop = true;
            this.OffsetTypeFsuipRadioButton.UseVisualStyleBackColor = true;
            this.OffsetTypeFsuipRadioButton.CheckedChanged += new System.EventHandler(this.OffsetTypeFsuipRadioButton_CheckedChanged);
            // 
            // compareTabPage
            // 
            this.compareTabPage.Controls.Add(this.compareSpacerPanel);
            this.compareTabPage.Controls.Add(this.interpolationGroupBox);
            this.compareTabPage.Controls.Add(this.comparisonSettingsGroupBox);
            this.compareTabPage.Controls.Add(this.comparisonHintTtextBox);
            resources.ApplyResources(this.compareTabPage, "compareTabPage");
            this.compareTabPage.Name = "compareTabPage";
            this.compareTabPage.UseVisualStyleBackColor = true;
            // 
            // compareSpacerPanel
            // 
            resources.ApplyResources(this.compareSpacerPanel, "compareSpacerPanel");
            this.compareSpacerPanel.Name = "compareSpacerPanel";
            // 
            // interpolationGroupBox
            // 
            this.interpolationGroupBox.Controls.Add(this.interpolationPanel1);
            this.interpolationGroupBox.Controls.Add(this.interpolationCheckBox);
            resources.ApplyResources(this.interpolationGroupBox, "interpolationGroupBox");
            this.interpolationGroupBox.Name = "interpolationGroupBox";
            this.interpolationGroupBox.TabStop = false;
            // 
            // interpolationPanel1
            // 
            resources.ApplyResources(this.interpolationPanel1, "interpolationPanel1");
            this.interpolationPanel1.Name = "interpolationPanel1";
            this.interpolationPanel1.Save = false;
            // 
            // interpolationCheckBox
            // 
            resources.ApplyResources(this.interpolationCheckBox, "interpolationCheckBox");
            this.interpolationCheckBox.Checked = true;
            this.interpolationCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.interpolationCheckBox.Name = "interpolationCheckBox";
            this.interpolationCheckBox.UseVisualStyleBackColor = true;
            this.interpolationCheckBox.CheckedChanged += new System.EventHandler(this.interpolationCheckBox_CheckedChanged);
            // 
            // comparisonSettingsGroupBox
            // 
            this.comparisonSettingsGroupBox.Controls.Add(this.comparisonSettingsPanel);
            this.comparisonSettingsGroupBox.Controls.Add(this.comparisonActiveCheckBox);
            resources.ApplyResources(this.comparisonSettingsGroupBox, "comparisonSettingsGroupBox");
            this.comparisonSettingsGroupBox.Name = "comparisonSettingsGroupBox";
            this.comparisonSettingsGroupBox.TabStop = false;
            // 
            // comparisonSettingsPanel
            // 
            this.comparisonSettingsPanel.Controls.Add(this.comparisonValueTextBox);
            this.comparisonSettingsPanel.Controls.Add(this.comparisonElseValueTextBox);
            this.comparisonSettingsPanel.Controls.Add(this.label8);
            this.comparisonSettingsPanel.Controls.Add(this.comparisonIfValueTextBox);
            this.comparisonSettingsPanel.Controls.Add(this.label7);
            this.comparisonSettingsPanel.Controls.Add(this.comparisonOperandComboBox);
            this.comparisonSettingsPanel.Controls.Add(this.label6);
            resources.ApplyResources(this.comparisonSettingsPanel, "comparisonSettingsPanel");
            this.comparisonSettingsPanel.Name = "comparisonSettingsPanel";
            // 
            // comparisonValueTextBox
            // 
            resources.ApplyResources(this.comparisonValueTextBox, "comparisonValueTextBox");
            this.comparisonValueTextBox.Name = "comparisonValueTextBox";
            // 
            // comparisonElseValueTextBox
            // 
            resources.ApplyResources(this.comparisonElseValueTextBox, "comparisonElseValueTextBox");
            this.comparisonElseValueTextBox.Name = "comparisonElseValueTextBox";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // comparisonIfValueTextBox
            // 
            resources.ApplyResources(this.comparisonIfValueTextBox, "comparisonIfValueTextBox");
            this.comparisonIfValueTextBox.Name = "comparisonIfValueTextBox";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // comparisonOperandComboBox
            // 
            this.comparisonOperandComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comparisonOperandComboBox.FormattingEnabled = true;
            this.comparisonOperandComboBox.Items.AddRange(new object[] {
            resources.GetString("comparisonOperandComboBox.Items"),
            resources.GetString("comparisonOperandComboBox.Items1"),
            resources.GetString("comparisonOperandComboBox.Items2"),
            resources.GetString("comparisonOperandComboBox.Items3"),
            resources.GetString("comparisonOperandComboBox.Items4"),
            resources.GetString("comparisonOperandComboBox.Items5"),
            resources.GetString("comparisonOperandComboBox.Items6")});
            resources.ApplyResources(this.comparisonOperandComboBox, "comparisonOperandComboBox");
            this.comparisonOperandComboBox.Name = "comparisonOperandComboBox";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // comparisonActiveCheckBox
            // 
            resources.ApplyResources(this.comparisonActiveCheckBox, "comparisonActiveCheckBox");
            this.comparisonActiveCheckBox.Name = "comparisonActiveCheckBox";
            this.comparisonActiveCheckBox.UseVisualStyleBackColor = true;
            this.comparisonActiveCheckBox.CheckedChanged += new System.EventHandler(this.comparisonActiveCheckBox_CheckedChanged);
            // 
            // comparisonHintTtextBox
            // 
            this.comparisonHintTtextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.comparisonHintTtextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.comparisonHintTtextBox.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.comparisonHintTtextBox, "comparisonHintTtextBox");
            this.comparisonHintTtextBox.Name = "comparisonHintTtextBox";
            this.comparisonHintTtextBox.ReadOnly = true;
            this.comparisonHintTtextBox.TabStop = false;
            // 
            // displayTabPage
            // 
            this.displayTabPage.Controls.Add(this.testSettingsGroupBox);
            this.displayTabPage.Controls.Add(this.groupBoxDisplaySettings);
            this.displayTabPage.Controls.Add(this.displayTypeGroupBox);
            this.displayTabPage.Controls.Add(this.displayTabTextBox);
            resources.ApplyResources(this.displayTabPage, "displayTabPage");
            this.displayTabPage.Name = "displayTabPage";
            this.displayTabPage.UseVisualStyleBackColor = true;
            // 
            // testSettingsGroupBox
            // 
            this.testSettingsGroupBox.Controls.Add(this.displayPinTestStopButton);
            this.testSettingsGroupBox.Controls.Add(this.displayPinTestButton);
            resources.ApplyResources(this.testSettingsGroupBox, "testSettingsGroupBox");
            this.testSettingsGroupBox.Name = "testSettingsGroupBox";
            this.testSettingsGroupBox.TabStop = false;
            // 
            // displayPinTestStopButton
            // 
            resources.ApplyResources(this.displayPinTestStopButton, "displayPinTestStopButton");
            this.displayPinTestStopButton.Image = global::MobiFlight.Properties.Resources.media_stop;
            this.displayPinTestStopButton.Name = "displayPinTestStopButton";
            this.displayPinTestStopButton.UseVisualStyleBackColor = true;
            this.displayPinTestStopButton.Click += new System.EventHandler(this.displayPinTestStopButton_Click);
            // 
            // displayPinTestButton
            // 
            resources.ApplyResources(this.displayPinTestButton, "displayPinTestButton");
            this.displayPinTestButton.Image = global::MobiFlight.Properties.Resources.media_play;
            this.displayPinTestButton.Name = "displayPinTestButton";
            this.displayPinTestButton.UseVisualStyleBackColor = true;
            this.displayPinTestButton.Click += new System.EventHandler(this.displayPinTestButton_Click);
            // 
            // groupBoxDisplaySettings
            // 
            resources.ApplyResources(this.groupBoxDisplaySettings, "groupBoxDisplaySettings");
            this.groupBoxDisplaySettings.Name = "groupBoxDisplaySettings";
            this.groupBoxDisplaySettings.TabStop = false;
            // 
            // displayTypeGroupBox
            // 
            this.displayTypeGroupBox.Controls.Add(this.arcazeSerialLabel);
            this.displayTypeGroupBox.Controls.Add(this.displayModuleNameComboBox);
            this.displayTypeGroupBox.Controls.Add(this.displayTypeComboBoxLabel);
            this.displayTypeGroupBox.Controls.Add(this.displayTypeComboBox);
            resources.ApplyResources(this.displayTypeGroupBox, "displayTypeGroupBox");
            this.displayTypeGroupBox.Name = "displayTypeGroupBox";
            this.displayTypeGroupBox.TabStop = false;
            // 
            // arcazeSerialLabel
            // 
            resources.ApplyResources(this.arcazeSerialLabel, "arcazeSerialLabel");
            this.arcazeSerialLabel.Name = "arcazeSerialLabel";
            // 
            // displayModuleNameComboBox
            // 
            this.displayModuleNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.displayModuleNameComboBox.FormattingEnabled = true;
            this.displayModuleNameComboBox.Items.AddRange(new object[] {
            resources.GetString("displayModuleNameComboBox.Items"),
            resources.GetString("displayModuleNameComboBox.Items1")});
            resources.ApplyResources(this.displayModuleNameComboBox, "displayModuleNameComboBox");
            this.displayModuleNameComboBox.Name = "displayModuleNameComboBox";
            this.displayModuleNameComboBox.SelectedIndexChanged += new System.EventHandler(this.displaySerialComboBox_SelectedIndexChanged);
            this.displayModuleNameComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.displayArcazeSerialComboBox_Validating);
            // 
            // displayTypeComboBoxLabel
            // 
            resources.ApplyResources(this.displayTypeComboBoxLabel, "displayTypeComboBoxLabel");
            this.displayTypeComboBoxLabel.Name = "displayTypeComboBoxLabel";
            // 
            // displayTypeComboBox
            // 
            this.displayTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.displayTypeComboBox.FormattingEnabled = true;
            this.displayTypeComboBox.Items.AddRange(new object[] {
            resources.GetString("displayTypeComboBox.Items"),
            resources.GetString("displayTypeComboBox.Items1"),
            resources.GetString("displayTypeComboBox.Items2")});
            resources.ApplyResources(this.displayTypeComboBox, "displayTypeComboBox");
            this.displayTypeComboBox.Name = "displayTypeComboBox";
            this.displayTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.displayTypeComboBox_SelectedIndexChanged);
            // 
            // displayTabTextBox
            // 
            this.displayTabTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.displayTabTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.displayTabTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.displayTabTextBox, "displayTabTextBox");
            this.displayTabTextBox.Name = "displayTabTextBox";
            this.displayTabTextBox.ReadOnly = true;
            this.displayTabTextBox.TabStop = false;
            // 
            // preconditionTabPage
            // 
            this.preconditionTabPage.Controls.Add(this.preconditionSpacerPanel);
            this.preconditionTabPage.Controls.Add(this.overrideGroupBox);
            this.preconditionTabPage.Controls.Add(this.preconditionSettingsPanel);
            this.preconditionTabPage.Controls.Add(this.preconditionListgroupBox);
            this.preconditionTabPage.Controls.Add(this.preconditionTabTextBox);
            resources.ApplyResources(this.preconditionTabPage, "preconditionTabPage");
            this.preconditionTabPage.Name = "preconditionTabPage";
            this.preconditionTabPage.UseVisualStyleBackColor = true;
            // 
            // preconditionSpacerPanel
            // 
            resources.ApplyResources(this.preconditionSpacerPanel, "preconditionSpacerPanel");
            this.preconditionSpacerPanel.Name = "preconditionSpacerPanel";
            // 
            // overrideGroupBox
            // 
            this.overrideGroupBox.Controls.Add(this.overridePreconditionTextBox);
            this.overrideGroupBox.Controls.Add(this.overridePreconditionCheckBox);
            resources.ApplyResources(this.overrideGroupBox, "overrideGroupBox");
            this.overrideGroupBox.Name = "overrideGroupBox";
            this.overrideGroupBox.TabStop = false;
            // 
            // overridePreconditionTextBox
            // 
            resources.ApplyResources(this.overridePreconditionTextBox, "overridePreconditionTextBox");
            this.overridePreconditionTextBox.Name = "overridePreconditionTextBox";
            // 
            // overridePreconditionCheckBox
            // 
            resources.ApplyResources(this.overridePreconditionCheckBox, "overridePreconditionCheckBox");
            this.overridePreconditionCheckBox.Name = "overridePreconditionCheckBox";
            this.overridePreconditionCheckBox.UseVisualStyleBackColor = true;
            // 
            // preconditionSettingsPanel
            // 
            this.preconditionSettingsPanel.Controls.Add(this.preconditionSettingsGroupBox);
            this.preconditionSettingsPanel.Controls.Add(this.preconditionApplyButton);
            this.preconditionSettingsPanel.Controls.Add(this.preconditionSelectGroupBox);
            resources.ApplyResources(this.preconditionSettingsPanel, "preconditionSettingsPanel");
            this.preconditionSettingsPanel.Name = "preconditionSettingsPanel";
            // 
            // preconditionSettingsGroupBox
            // 
            this.preconditionSettingsGroupBox.Controls.Add(this.preconditionPinPanel);
            this.preconditionSettingsGroupBox.Controls.Add(this.preconditionRuleConfigPanel);
            resources.ApplyResources(this.preconditionSettingsGroupBox, "preconditionSettingsGroupBox");
            this.preconditionSettingsGroupBox.Name = "preconditionSettingsGroupBox";
            this.preconditionSettingsGroupBox.TabStop = false;
            // 
            // preconditionPinPanel
            // 
            this.preconditionPinPanel.Controls.Add(this.preconditionPinValueComboBox);
            this.preconditionPinPanel.Controls.Add(this.preconditionPinValueLabel);
            this.preconditionPinPanel.Controls.Add(this.preconditionPinComboBox);
            this.preconditionPinPanel.Controls.Add(this.preconditionPortComboBox);
            this.preconditionPinPanel.Controls.Add(this.preconditonPinLabel);
            this.preconditionPinPanel.Controls.Add(this.preconditionPinSerialComboBox);
            this.preconditionPinPanel.Controls.Add(this.preconditionPinSerialLabel);
            resources.ApplyResources(this.preconditionPinPanel, "preconditionPinPanel");
            this.preconditionPinPanel.Name = "preconditionPinPanel";
            // 
            // preconditionPinValueComboBox
            // 
            this.preconditionPinValueComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.preconditionPinValueComboBox.FormattingEnabled = true;
            this.preconditionPinValueComboBox.Items.AddRange(new object[] {
            resources.GetString("preconditionPinValueComboBox.Items"),
            resources.GetString("preconditionPinValueComboBox.Items1")});
            resources.ApplyResources(this.preconditionPinValueComboBox, "preconditionPinValueComboBox");
            this.preconditionPinValueComboBox.Name = "preconditionPinValueComboBox";
            // 
            // preconditionPinValueLabel
            // 
            resources.ApplyResources(this.preconditionPinValueLabel, "preconditionPinValueLabel");
            this.preconditionPinValueLabel.Name = "preconditionPinValueLabel";
            // 
            // preconditionPinComboBox
            // 
            this.preconditionPinComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.preconditionPinComboBox.FormattingEnabled = true;
            this.preconditionPinComboBox.Items.AddRange(new object[] {
            resources.GetString("preconditionPinComboBox.Items"),
            resources.GetString("preconditionPinComboBox.Items1"),
            resources.GetString("preconditionPinComboBox.Items2"),
            resources.GetString("preconditionPinComboBox.Items3"),
            resources.GetString("preconditionPinComboBox.Items4"),
            resources.GetString("preconditionPinComboBox.Items5")});
            resources.ApplyResources(this.preconditionPinComboBox, "preconditionPinComboBox");
            this.preconditionPinComboBox.Name = "preconditionPinComboBox";
            this.preconditionPinComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.preconditionPinComboBox_Validating);
            // 
            // preconditionPortComboBox
            // 
            this.preconditionPortComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.preconditionPortComboBox.FormattingEnabled = true;
            this.preconditionPortComboBox.Items.AddRange(new object[] {
            resources.GetString("preconditionPortComboBox.Items"),
            resources.GetString("preconditionPortComboBox.Items1"),
            resources.GetString("preconditionPortComboBox.Items2"),
            resources.GetString("preconditionPortComboBox.Items3"),
            resources.GetString("preconditionPortComboBox.Items4"),
            resources.GetString("preconditionPortComboBox.Items5")});
            resources.ApplyResources(this.preconditionPortComboBox, "preconditionPortComboBox");
            this.preconditionPortComboBox.Name = "preconditionPortComboBox";
            this.preconditionPortComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.preconditionPortComboBox_Validating);
            // 
            // preconditonPinLabel
            // 
            resources.ApplyResources(this.preconditonPinLabel, "preconditonPinLabel");
            this.preconditonPinLabel.Name = "preconditonPinLabel";
            // 
            // preconditionPinSerialComboBox
            // 
            this.preconditionPinSerialComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.preconditionPinSerialComboBox.FormattingEnabled = true;
            this.preconditionPinSerialComboBox.Items.AddRange(new object[] {
            resources.GetString("preconditionPinSerialComboBox.Items"),
            resources.GetString("preconditionPinSerialComboBox.Items1")});
            resources.ApplyResources(this.preconditionPinSerialComboBox, "preconditionPinSerialComboBox");
            this.preconditionPinSerialComboBox.Name = "preconditionPinSerialComboBox";
            this.preconditionPinSerialComboBox.SelectedIndexChanged += new System.EventHandler(this.preconditionPinSerialComboBox_SelectedIndexChanged);
            this.preconditionPinSerialComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.preconditionPinSerialComboBox_Validating);
            // 
            // preconditionPinSerialLabel
            // 
            resources.ApplyResources(this.preconditionPinSerialLabel, "preconditionPinSerialLabel");
            this.preconditionPinSerialLabel.Name = "preconditionPinSerialLabel";
            // 
            // preconditionRuleConfigPanel
            // 
            this.preconditionRuleConfigPanel.Controls.Add(this.preconditionRefValueTextBox);
            this.preconditionRuleConfigPanel.Controls.Add(this.preconditionRefOperandComboBox);
            this.preconditionRuleConfigPanel.Controls.Add(this.preconditionConfigRefOperandLabel);
            this.preconditionRuleConfigPanel.Controls.Add(this.preconditionConfigComboBox);
            this.preconditionRuleConfigPanel.Controls.Add(this.label11);
            resources.ApplyResources(this.preconditionRuleConfigPanel, "preconditionRuleConfigPanel");
            this.preconditionRuleConfigPanel.Name = "preconditionRuleConfigPanel";
            this.preconditionRuleConfigPanel.Validating += new System.ComponentModel.CancelEventHandler(this.preconditionRuleConfigPanel_Validating);
            // 
            // preconditionRefValueTextBox
            // 
            resources.ApplyResources(this.preconditionRefValueTextBox, "preconditionRefValueTextBox");
            this.preconditionRefValueTextBox.Name = "preconditionRefValueTextBox";
            this.preconditionRefValueTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.preconditionRefValueTextBox_Validating);
            // 
            // preconditionRefOperandComboBox
            // 
            this.preconditionRefOperandComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.preconditionRefOperandComboBox.FormattingEnabled = true;
            this.preconditionRefOperandComboBox.Items.AddRange(new object[] {
            resources.GetString("preconditionRefOperandComboBox.Items"),
            resources.GetString("preconditionRefOperandComboBox.Items1"),
            resources.GetString("preconditionRefOperandComboBox.Items2"),
            resources.GetString("preconditionRefOperandComboBox.Items3"),
            resources.GetString("preconditionRefOperandComboBox.Items4"),
            resources.GetString("preconditionRefOperandComboBox.Items5")});
            resources.ApplyResources(this.preconditionRefOperandComboBox, "preconditionRefOperandComboBox");
            this.preconditionRefOperandComboBox.Name = "preconditionRefOperandComboBox";
            // 
            // preconditionConfigRefOperandLabel
            // 
            resources.ApplyResources(this.preconditionConfigRefOperandLabel, "preconditionConfigRefOperandLabel");
            this.preconditionConfigRefOperandLabel.Name = "preconditionConfigRefOperandLabel";
            // 
            // preconditionConfigComboBox
            // 
            this.preconditionConfigComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.preconditionConfigComboBox.FormattingEnabled = true;
            this.preconditionConfigComboBox.Items.AddRange(new object[] {
            resources.GetString("preconditionConfigComboBox.Items"),
            resources.GetString("preconditionConfigComboBox.Items1")});
            resources.ApplyResources(this.preconditionConfigComboBox, "preconditionConfigComboBox");
            this.preconditionConfigComboBox.Name = "preconditionConfigComboBox";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // preconditionApplyButton
            // 
            resources.ApplyResources(this.preconditionApplyButton, "preconditionApplyButton");
            this.preconditionApplyButton.Name = "preconditionApplyButton";
            this.preconditionApplyButton.UseVisualStyleBackColor = true;
            this.preconditionApplyButton.Click += new System.EventHandler(this.preconditionApplyButton_Click);
            // 
            // preconditionSelectGroupBox
            // 
            this.preconditionSelectGroupBox.Controls.Add(this.preConditionTypeComboBox);
            this.preconditionSelectGroupBox.Controls.Add(this.preconditionTypeLabel);
            resources.ApplyResources(this.preconditionSelectGroupBox, "preconditionSelectGroupBox");
            this.preconditionSelectGroupBox.Name = "preconditionSelectGroupBox";
            this.preconditionSelectGroupBox.TabStop = false;
            // 
            // preConditionTypeComboBox
            // 
            this.preConditionTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.preConditionTypeComboBox.FormattingEnabled = true;
            this.preConditionTypeComboBox.Items.AddRange(new object[] {
            resources.GetString("preConditionTypeComboBox.Items"),
            resources.GetString("preConditionTypeComboBox.Items1"),
            resources.GetString("preConditionTypeComboBox.Items2")});
            resources.ApplyResources(this.preConditionTypeComboBox, "preConditionTypeComboBox");
            this.preConditionTypeComboBox.Name = "preConditionTypeComboBox";
            this.preConditionTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.preConditionTypeComboBox_SelectedIndexChanged);
            // 
            // preconditionTypeLabel
            // 
            resources.ApplyResources(this.preconditionTypeLabel, "preconditionTypeLabel");
            this.preconditionTypeLabel.Name = "preconditionTypeLabel";
            // 
            // preconditionListgroupBox
            // 
            this.preconditionListgroupBox.Controls.Add(this.preconditionListTreeView);
            resources.ApplyResources(this.preconditionListgroupBox, "preconditionListgroupBox");
            this.preconditionListgroupBox.Name = "preconditionListgroupBox";
            this.preconditionListgroupBox.TabStop = false;
            // 
            // preconditionListTreeView
            // 
            this.preconditionListTreeView.CheckBoxes = true;
            this.preconditionListTreeView.ContextMenuStrip = this.preconditionTreeContextMenuStrip;
            resources.ApplyResources(this.preconditionListTreeView, "preconditionListTreeView");
            this.preconditionListTreeView.ImageList = this.preconditionTreeImageList;
            this.preconditionListTreeView.Name = "preconditionListTreeView";
            this.preconditionListTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("preconditionListTreeView.Nodes"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("preconditionListTreeView.Nodes1")))});
            this.preconditionListTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.preconditionListTreeView_NodeMouseClick);
            // 
            // preconditionTreeContextMenuStrip
            // 
            this.preconditionTreeContextMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.preconditionTreeContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addPreconditionToolStripMenuItem,
            this.removePreconditionToolStripMenuItem,
            this.toolStripSeparator1,
            this.addGroupToolStripMenuItem,
            this.removeGroupToolStripMenuItem,
            this.toolStripSeparator2,
            this.logicSelectToolStripMenuItem});
            this.preconditionTreeContextMenuStrip.Name = "preconditionTreeContextMenuStrip";
            resources.ApplyResources(this.preconditionTreeContextMenuStrip, "preconditionTreeContextMenuStrip");
            // 
            // addPreconditionToolStripMenuItem
            // 
            this.addPreconditionToolStripMenuItem.Name = "addPreconditionToolStripMenuItem";
            resources.ApplyResources(this.addPreconditionToolStripMenuItem, "addPreconditionToolStripMenuItem");
            this.addPreconditionToolStripMenuItem.Click += new System.EventHandler(this.addPreconditionToolStripMenuItem_Click);
            // 
            // removePreconditionToolStripMenuItem
            // 
            this.removePreconditionToolStripMenuItem.Name = "removePreconditionToolStripMenuItem";
            resources.ApplyResources(this.removePreconditionToolStripMenuItem, "removePreconditionToolStripMenuItem");
            this.removePreconditionToolStripMenuItem.Click += new System.EventHandler(this.removePreconditionToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // addGroupToolStripMenuItem
            // 
            this.addGroupToolStripMenuItem.Name = "addGroupToolStripMenuItem";
            resources.ApplyResources(this.addGroupToolStripMenuItem, "addGroupToolStripMenuItem");
            // 
            // removeGroupToolStripMenuItem
            // 
            this.removeGroupToolStripMenuItem.Name = "removeGroupToolStripMenuItem";
            resources.ApplyResources(this.removeGroupToolStripMenuItem, "removeGroupToolStripMenuItem");
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // logicSelectToolStripMenuItem
            // 
            this.logicSelectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aNDToolStripMenuItem,
            this.oRToolStripMenuItem});
            this.logicSelectToolStripMenuItem.Name = "logicSelectToolStripMenuItem";
            resources.ApplyResources(this.logicSelectToolStripMenuItem, "logicSelectToolStripMenuItem");
            // 
            // aNDToolStripMenuItem
            // 
            this.aNDToolStripMenuItem.Name = "aNDToolStripMenuItem";
            resources.ApplyResources(this.aNDToolStripMenuItem, "aNDToolStripMenuItem");
            this.aNDToolStripMenuItem.Click += new System.EventHandler(this.andOrToolStripMenuItem_Click);
            // 
            // oRToolStripMenuItem
            // 
            this.oRToolStripMenuItem.Name = "oRToolStripMenuItem";
            resources.ApplyResources(this.oRToolStripMenuItem, "oRToolStripMenuItem");
            this.oRToolStripMenuItem.Click += new System.EventHandler(this.andOrToolStripMenuItem_Click);
            // 
            // preconditionTreeImageList
            // 
            this.preconditionTreeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("preconditionTreeImageList.ImageStream")));
            this.preconditionTreeImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.preconditionTreeImageList.Images.SetKeyName(0, "media_stop.png");
            this.preconditionTreeImageList.Images.SetKeyName(1, "media_stop_red.png");
            // 
            // preconditionTabTextBox
            // 
            this.preconditionTabTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.preconditionTabTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.preconditionTabTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.preconditionTabTextBox, "preconditionTabTextBox");
            this.preconditionTabTextBox.Name = "preconditionTabTextBox";
            this.preconditionTabTextBox.ReadOnly = true;
            this.preconditionTabTextBox.TabStop = false;
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.Controls.Add(this.button1);
            this.ButtonPanel.Controls.Add(this.cancelButton);
            resources.ApplyResources(this.ButtonPanel, "ButtonPanel");
            this.ButtonPanel.Name = "ButtonPanel";
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // presetsDataSet
            // 
            this.presetsDataSet.DataSetName = "ArcazeUsbConnectorPreset";
            this.presetsDataSet.Tables.AddRange(new System.Data.DataTable[] {
            this.presetDataTable});
            // 
            // presetDataTable
            // 
            this.presetDataTable.Columns.AddRange(new System.Data.DataColumn[] {
            this.description,
            this.settingsColumn});
            this.presetDataTable.TableName = "config";
            // 
            // description
            // 
            this.description.ColumnName = "description";
            // 
            // settingsColumn
            // 
            this.settingsColumn.Caption = "settings";
            this.settingsColumn.ColumnName = "settings";
            this.settingsColumn.DataType = typeof(object);
            // 
            // ConfigWizard
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.MainPanel);
            this.Controls.Add(this.ButtonPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "ConfigWizard";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigWizard_FormClosing);
            this.Load += new System.EventHandler(this.ConfigWizard_Load);
            this.MainPanel.ResumeLayout(false);
            this.tabControlFsuipc.ResumeLayout(false);
            this.fsuipcTabPage.ResumeLayout(false);
            this.fsuipcTabPage.PerformLayout();
            this.referencesGroupBox.ResumeLayout(false);
            this.FsuipcSettingsPanel.ResumeLayout(false);
            this.OffsetTypePanel.ResumeLayout(false);
            this.OffsetTypePanel.PerformLayout();
            this.compareTabPage.ResumeLayout(false);
            this.compareTabPage.PerformLayout();
            this.interpolationGroupBox.ResumeLayout(false);
            this.interpolationGroupBox.PerformLayout();
            this.comparisonSettingsGroupBox.ResumeLayout(false);
            this.comparisonSettingsGroupBox.PerformLayout();
            this.comparisonSettingsPanel.ResumeLayout(false);
            this.comparisonSettingsPanel.PerformLayout();
            this.displayTabPage.ResumeLayout(false);
            this.displayTabPage.PerformLayout();
            this.testSettingsGroupBox.ResumeLayout(false);
            this.displayTypeGroupBox.ResumeLayout(false);
            this.displayTypeGroupBox.PerformLayout();
            this.preconditionTabPage.ResumeLayout(false);
            this.preconditionTabPage.PerformLayout();
            this.overrideGroupBox.ResumeLayout(false);
            this.overrideGroupBox.PerformLayout();
            this.preconditionSettingsPanel.ResumeLayout(false);
            this.preconditionSettingsGroupBox.ResumeLayout(false);
            this.preconditionPinPanel.ResumeLayout(false);
            this.preconditionPinPanel.PerformLayout();
            this.preconditionRuleConfigPanel.ResumeLayout(false);
            this.preconditionRuleConfigPanel.PerformLayout();
            this.preconditionSelectGroupBox.ResumeLayout(false);
            this.preconditionSelectGroupBox.PerformLayout();
            this.preconditionListgroupBox.ResumeLayout(false);
            this.preconditionTreeContextMenuStrip.ResumeLayout(false);
            this.ButtonPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.presetsDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.presetDataTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel MainPanel;
        private System.Windows.Forms.Panel ButtonPanel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TabControl tabControlFsuipc;
        private System.Windows.Forms.TabPage fsuipcTabPage;
        private System.Windows.Forms.TabPage compareTabPage;
        private System.Windows.Forms.GroupBox comparisonSettingsGroupBox;
        private System.Windows.Forms.CheckBox comparisonActiveCheckBox;
        private System.Windows.Forms.TextBox comparisonHintTtextBox;
        private System.Windows.Forms.TabPage displayTabPage;
        private System.Windows.Forms.TextBox displayTabTextBox;
        private System.Windows.Forms.GroupBox groupBoxDisplaySettings;
        private System.Windows.Forms.GroupBox displayTypeGroupBox;
        private System.Windows.Forms.Label displayTypeComboBoxLabel;
        private System.Windows.Forms.ComboBox displayTypeComboBox;
        private System.Windows.Forms.Panel comparisonSettingsPanel;
        private System.Windows.Forms.TextBox comparisonValueTextBox;
        private System.Windows.Forms.TextBox comparisonElseValueTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox comparisonIfValueTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comparisonOperandComboBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label arcazeSerialLabel;
        private System.Windows.Forms.ComboBox displayModuleNameComboBox;
        private System.Data.DataSet presetsDataSet;
        private System.Data.DataTable presetDataTable;
        private System.Data.DataColumn description;
        private System.Data.DataColumn settingsColumn;
        //private System.Windows.Forms.Panel displayBcdPanel;
        private System.Windows.Forms.TabPage preconditionTabPage;
        private System.Windows.Forms.GroupBox preconditionSettingsGroupBox;
        private System.Windows.Forms.GroupBox preconditionSelectGroupBox;
        private System.Windows.Forms.ComboBox preConditionTypeComboBox;
        private System.Windows.Forms.Label preconditionTypeLabel;
        private System.Windows.Forms.TextBox preconditionTabTextBox;
        private System.Windows.Forms.Panel preconditionRuleConfigPanel;
        private System.Windows.Forms.ComboBox preconditionConfigComboBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox preconditionRefValueTextBox;
        private System.Windows.Forms.ComboBox preconditionRefOperandComboBox;
        private System.Windows.Forms.Label preconditionConfigRefOperandLabel;
        private System.Windows.Forms.Panel preconditionPinPanel;
        private System.Windows.Forms.ComboBox preconditionPortComboBox;
        private System.Windows.Forms.Label preconditonPinLabel;
        private System.Windows.Forms.ComboBox preconditionPinSerialComboBox;
        private System.Windows.Forms.Label preconditionPinSerialLabel;
        private System.Windows.Forms.ComboBox preconditionPinComboBox;
        private System.Windows.Forms.ComboBox preconditionPinValueComboBox;
        private System.Windows.Forms.Label preconditionPinValueLabel;
        private System.Windows.Forms.GroupBox testSettingsGroupBox;
        private System.Windows.Forms.Button displayPinTestButton;
        private System.Windows.Forms.Button displayPinTestStopButton;
        private System.Windows.Forms.Panel compareSpacerPanel;
        private System.Windows.Forms.Panel preconditionSpacerPanel;
        private System.Windows.Forms.Button preconditionApplyButton;
        private System.Windows.Forms.GroupBox preconditionListgroupBox;
        private System.Windows.Forms.TreeView preconditionListTreeView;
        private System.Windows.Forms.ContextMenuStrip preconditionTreeContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addPreconditionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removePreconditionToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem addGroupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeGroupToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem logicSelectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aNDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oRToolStripMenuItem;
        private System.Windows.Forms.ImageList preconditionTreeImageList;
        private System.Windows.Forms.Panel preconditionSettingsPanel;
        private System.Windows.Forms.GroupBox interpolationGroupBox;
        private UI.Panels.Config.InterpolationPanel interpolationPanel1;
        private System.Windows.Forms.CheckBox interpolationCheckBox;
        private UI.Panels.Config.FsuipcConfigPanel fsuipcConfigPanel;
        private System.Windows.Forms.GroupBox overrideGroupBox;
        private System.Windows.Forms.TextBox overridePreconditionTextBox;
        private System.Windows.Forms.CheckBox overridePreconditionCheckBox;
        private System.Windows.Forms.GroupBox referencesGroupBox;
        private UI.Panels.Config.ConfigRefPanel configRefPanel;
        private System.Windows.Forms.Panel FsuipcSettingsPanel;
        private System.Windows.Forms.Panel OffsetTypePanel;
        private System.Windows.Forms.RadioButton OffsetTypeSimConnectRadioButton;
        private System.Windows.Forms.RadioButton OffsetTypeFsuipRadioButton;
        private Panels.Config.SimConnectPanel simConnectPanel1;
        private System.Windows.Forms.RadioButton OffsetTypeVariableRadioButton;
        private Panels.Config.VariablePanel variablePanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label fsuipcHintLabel;
    }
}