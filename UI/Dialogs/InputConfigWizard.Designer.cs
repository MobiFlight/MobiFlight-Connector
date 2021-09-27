namespace MobiFlight.UI.Dialogs
{
    partial class InputConfigWizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputConfigWizard));
            this.MainPanel = new System.Windows.Forms.Panel();
            this.tabControlFsuipc = new System.Windows.Forms.TabControl();
            this.preconditionTabPage = new System.Windows.Forms.TabPage();
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
            this.preconditionSpacerPanel = new System.Windows.Forms.Panel();
            this.preconditionTabTextBox = new System.Windows.Forms.TextBox();
            this.configRefTabPage = new System.Windows.Forms.TabPage();
            this.configRefPanel = new MobiFlight.UI.Panels.Config.ConfigRefPanel();
            this.displayTabPage = new System.Windows.Forms.TabPage();
            this.groupBoxInputSettings = new System.Windows.Forms.GroupBox();
            this.displayTypeGroupBox = new System.Windows.Forms.GroupBox();
            this.arcazeSerialLabel = new System.Windows.Forms.Label();
            this.inputModuleNameComboBox = new System.Windows.Forms.ComboBox();
            this.inputTypeComboBoxLabel = new System.Windows.Forms.Label();
            this.inputTypeComboBox = new System.Windows.Forms.ComboBox();
            this.displayTabTextBox = new System.Windows.Forms.TextBox();
            this.ButtonPanel = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.presetsDataSet = new System.Data.DataSet();
            this.presetDataTable = new System.Data.DataTable();
            this.description = new System.Data.DataColumn();
            this.settingsColumn = new System.Data.DataColumn();
            this.MainPanel.SuspendLayout();
            this.tabControlFsuipc.SuspendLayout();
            this.preconditionTabPage.SuspendLayout();
            this.preconditionSettingsPanel.SuspendLayout();
            this.preconditionSettingsGroupBox.SuspendLayout();
            this.preconditionPinPanel.SuspendLayout();
            this.preconditionRuleConfigPanel.SuspendLayout();
            this.preconditionSelectGroupBox.SuspendLayout();
            this.preconditionListgroupBox.SuspendLayout();
            this.preconditionTreeContextMenuStrip.SuspendLayout();
            this.configRefTabPage.SuspendLayout();
            this.displayTabPage.SuspendLayout();
            this.displayTypeGroupBox.SuspendLayout();
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
            this.tabControlFsuipc.Controls.Add(this.preconditionTabPage);
            this.tabControlFsuipc.Controls.Add(this.configRefTabPage);
            this.tabControlFsuipc.Controls.Add(this.displayTabPage);
            resources.ApplyResources(this.tabControlFsuipc, "tabControlFsuipc");
            this.tabControlFsuipc.Name = "tabControlFsuipc";
            this.tabControlFsuipc.SelectedIndex = 0;
            this.tabControlFsuipc.SelectedIndexChanged += new System.EventHandler(this.tabControlFsuipc_SelectedIndexChanged);
            // 
            // preconditionTabPage
            // 
            this.preconditionTabPage.Controls.Add(this.preconditionSettingsPanel);
            this.preconditionTabPage.Controls.Add(this.preconditionListgroupBox);
            this.preconditionTabPage.Controls.Add(this.preconditionSpacerPanel);
            this.preconditionTabPage.Controls.Add(this.preconditionTabTextBox);
            resources.ApplyResources(this.preconditionTabPage, "preconditionTabPage");
            this.preconditionTabPage.Name = "preconditionTabPage";
            this.preconditionTabPage.UseVisualStyleBackColor = true;
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
            // preconditionSpacerPanel
            // 
            resources.ApplyResources(this.preconditionSpacerPanel, "preconditionSpacerPanel");
            this.preconditionSpacerPanel.Name = "preconditionSpacerPanel";
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
            // configRefTabPage
            // 
            this.configRefTabPage.Controls.Add(this.configRefPanel);
            resources.ApplyResources(this.configRefTabPage, "configRefTabPage");
            this.configRefTabPage.Name = "configRefTabPage";
            this.configRefTabPage.UseVisualStyleBackColor = true;
            // 
            // configRefPanel
            // 
            resources.ApplyResources(this.configRefPanel, "configRefPanel");
            this.configRefPanel.Name = "configRefPanel";
            // 
            // displayTabPage
            // 
            this.displayTabPage.Controls.Add(this.groupBoxInputSettings);
            this.displayTabPage.Controls.Add(this.displayTypeGroupBox);
            this.displayTabPage.Controls.Add(this.displayTabTextBox);
            resources.ApplyResources(this.displayTabPage, "displayTabPage");
            this.displayTabPage.Name = "displayTabPage";
            this.displayTabPage.UseVisualStyleBackColor = true;
            // 
            // groupBoxInputSettings
            // 
            resources.ApplyResources(this.groupBoxInputSettings, "groupBoxInputSettings");
            this.groupBoxInputSettings.Name = "groupBoxInputSettings";
            this.groupBoxInputSettings.TabStop = false;
            // 
            // displayTypeGroupBox
            // 
            this.displayTypeGroupBox.Controls.Add(this.arcazeSerialLabel);
            this.displayTypeGroupBox.Controls.Add(this.inputModuleNameComboBox);
            this.displayTypeGroupBox.Controls.Add(this.inputTypeComboBoxLabel);
            this.displayTypeGroupBox.Controls.Add(this.inputTypeComboBox);
            resources.ApplyResources(this.displayTypeGroupBox, "displayTypeGroupBox");
            this.displayTypeGroupBox.Name = "displayTypeGroupBox";
            this.displayTypeGroupBox.TabStop = false;
            // 
            // arcazeSerialLabel
            // 
            resources.ApplyResources(this.arcazeSerialLabel, "arcazeSerialLabel");
            this.arcazeSerialLabel.Name = "arcazeSerialLabel";
            // 
            // inputModuleNameComboBox
            // 
            this.inputModuleNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inputModuleNameComboBox.DropDownWidth = 300;
            this.inputModuleNameComboBox.FormattingEnabled = true;
            this.inputModuleNameComboBox.Items.AddRange(new object[] {
            resources.GetString("inputModuleNameComboBox.Items"),
            resources.GetString("inputModuleNameComboBox.Items1")});
            resources.ApplyResources(this.inputModuleNameComboBox, "inputModuleNameComboBox");
            this.inputModuleNameComboBox.Name = "inputModuleNameComboBox";
            this.inputModuleNameComboBox.SelectedIndexChanged += new System.EventHandler(this.ModuleSerialComboBox_SelectedIndexChanged);
            this.inputModuleNameComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.displayArcazeSerialComboBox_Validating);
            // 
            // inputTypeComboBoxLabel
            // 
            resources.ApplyResources(this.inputTypeComboBoxLabel, "inputTypeComboBoxLabel");
            this.inputTypeComboBoxLabel.Name = "inputTypeComboBoxLabel";
            // 
            // inputTypeComboBox
            // 
            this.inputTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inputTypeComboBox.FormattingEnabled = true;
            this.inputTypeComboBox.Items.AddRange(new object[] {
            resources.GetString("inputTypeComboBox.Items"),
            resources.GetString("inputTypeComboBox.Items1"),
            resources.GetString("inputTypeComboBox.Items2")});
            resources.ApplyResources(this.inputTypeComboBox, "inputTypeComboBox");
            this.inputTypeComboBox.Name = "inputTypeComboBox";
            this.inputTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.inputTypeComboBox_SelectedIndexChanged);
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
            this.button1.Click += new System.EventHandler(this.okButton_Click);
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
            // InputConfigWizard
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.MainPanel);
            this.Controls.Add(this.ButtonPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "InputConfigWizard";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Load += new System.EventHandler(this.ConfigWizard_Load);
            this.MainPanel.ResumeLayout(false);
            this.tabControlFsuipc.ResumeLayout(false);
            this.preconditionTabPage.ResumeLayout(false);
            this.preconditionTabPage.PerformLayout();
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
            this.configRefTabPage.ResumeLayout(false);
            this.displayTabPage.ResumeLayout(false);
            this.displayTabPage.PerformLayout();
            this.displayTypeGroupBox.ResumeLayout(false);
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
        private System.Data.DataSet presetsDataSet;
        private System.Data.DataTable presetDataTable;
        private System.Data.DataColumn description;
        private System.Data.DataColumn settingsColumn;
        //private System.Windows.Forms.Panel displayBcdPanel;
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
        private System.Windows.Forms.TabControl tabControlFsuipc;
        private System.Windows.Forms.TabPage preconditionTabPage;
        private System.Windows.Forms.Panel preconditionSettingsPanel;
        private System.Windows.Forms.GroupBox preconditionSettingsGroupBox;
        private System.Windows.Forms.Panel preconditionPinPanel;
        private System.Windows.Forms.ComboBox preconditionPinValueComboBox;
        private System.Windows.Forms.Label preconditionPinValueLabel;
        private System.Windows.Forms.ComboBox preconditionPinComboBox;
        private System.Windows.Forms.ComboBox preconditionPortComboBox;
        private System.Windows.Forms.Label preconditonPinLabel;
        private System.Windows.Forms.ComboBox preconditionPinSerialComboBox;
        private System.Windows.Forms.Label preconditionPinSerialLabel;
        private System.Windows.Forms.Panel preconditionRuleConfigPanel;
        private System.Windows.Forms.TextBox preconditionRefValueTextBox;
        private System.Windows.Forms.ComboBox preconditionRefOperandComboBox;
        private System.Windows.Forms.Label preconditionConfigRefOperandLabel;
        private System.Windows.Forms.ComboBox preconditionConfigComboBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button preconditionApplyButton;
        private System.Windows.Forms.GroupBox preconditionSelectGroupBox;
        private System.Windows.Forms.ComboBox preConditionTypeComboBox;
        private System.Windows.Forms.Label preconditionTypeLabel;
        private System.Windows.Forms.GroupBox preconditionListgroupBox;
        private System.Windows.Forms.TreeView preconditionListTreeView;
        private System.Windows.Forms.Panel preconditionSpacerPanel;
        private System.Windows.Forms.TextBox preconditionTabTextBox;
        private System.Windows.Forms.TabPage displayTabPage;
        private System.Windows.Forms.GroupBox groupBoxInputSettings;
        private System.Windows.Forms.GroupBox displayTypeGroupBox;
        private System.Windows.Forms.Label arcazeSerialLabel;
        private System.Windows.Forms.ComboBox inputModuleNameComboBox;
        private System.Windows.Forms.Label inputTypeComboBoxLabel;
        private System.Windows.Forms.ComboBox inputTypeComboBox;
        private System.Windows.Forms.TextBox displayTabTextBox;
        private System.Windows.Forms.TabPage configRefTabPage;
        private Panels.Config.ConfigRefPanel configRefPanel;
    }
}