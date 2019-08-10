namespace MobiFlight
{
    partial class SettingsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsDialog));
            this.panel1 = new System.Windows.Forms.Panel();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.ledDisplaysTabPage = new System.Windows.Forms.TabPage();
            this.arcazeModuleSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.numModulesLabel = new System.Windows.Forms.Label();
            this.numModulesNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.arcazeModuleTypeComboBox = new System.Windows.Forms.ComboBox();
            this.globalBrightnessLabel = new System.Windows.Forms.Label();
            this.globalBrightnessTrackBar = new System.Windows.Forms.TrackBar();
            this.arcazeModuleTypeLabel = new System.Windows.Forms.Label();
            this.arcazeModulesGroupBox = new System.Windows.Forms.GroupBox();
            this.ArcazeModuleTreeView = new System.Windows.Forms.TreeView();
            this.mfTreeViewImageList = new System.Windows.Forms.ImageList(this.components);
            this.arcazeSettingsLabel = new System.Windows.Forms.Label();
            this.mfModuleSettingsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ledOutputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ledSegmentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.servoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stepperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LcdDisplayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.encoderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.uploadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.updateFirmwareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.regenerateSerialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generalTabPage = new System.Windows.Forms.TabPage();
            this.offlineModeGroupBox = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.offlineModeCheckBox = new System.Windows.Forms.CheckBox();
            this.debugGroupBox = new System.Windows.Forms.GroupBox();
            this.logLevelComboBox = new System.Windows.Forms.ComboBox();
            this.logLevelLabel = new System.Windows.Forms.Label();
            this.logLevelCheckBox = new System.Windows.Forms.CheckBox();
            this.testModeSpeedGroupBox = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.testModeSpeedTrackBar = new System.Windows.Forms.TrackBar();
            this.recentFilesGroupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.recentFilesNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.mobiFlightTabPage = new System.Windows.Forms.TabPage();
            this.mfConfiguredModulesGroupBox = new System.Windows.Forms.GroupBox();
            this.mfModulesTreeView = new System.Windows.Forms.TreeView();
            this.mfSettingsPanel = new System.Windows.Forms.Panel();
            this.mobiflightSettingsToolStrip = new System.Windows.Forms.ToolStrip();
            this.uploadToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.addDeviceToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.addEncoderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addButtonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.addStepperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addServoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addLedModuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addOutputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addLcdDisplayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeDeviceToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.firmwareSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.FwAutoUpdateCheckBox = new System.Windows.Forms.CheckBox();
            this.firmwareArduinoIdeButton = new System.Windows.Forms.Button();
            this.firmwareArduinoIdePathTextBox = new System.Windows.Forms.TextBox();
            this.firmwareArduinoIdeLabel = new System.Windows.Forms.Label();
            this.mobiflightSettingsLabel = new System.Windows.Forms.Label();
            this.fsuipcTabPage = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.fsuipcPollIntervalTrackBar = new System.Windows.Forms.TrackBar();
            this.firmwareSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.firmwareUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.firmwareUpdateBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.panel1.SuspendLayout();
            this.ledDisplaysTabPage.SuspendLayout();
            this.arcazeModuleSettingsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numModulesNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.globalBrightnessTrackBar)).BeginInit();
            this.arcazeModulesGroupBox.SuspendLayout();
            this.mfModuleSettingsContextMenuStrip.SuspendLayout();
            this.generalTabPage.SuspendLayout();
            this.offlineModeGroupBox.SuspendLayout();
            this.debugGroupBox.SuspendLayout();
            this.testModeSpeedGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testModeSpeedTrackBar)).BeginInit();
            this.recentFilesGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.recentFilesNumericUpDown)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.mobiFlightTabPage.SuspendLayout();
            this.mfConfiguredModulesGroupBox.SuspendLayout();
            this.mobiflightSettingsToolStrip.SuspendLayout();
            this.firmwareSettingsGroupBox.SuspendLayout();
            this.fsuipcTabPage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fsuipcPollIntervalTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.okButton);
            this.panel1.Controls.Add(this.cancelButton);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // ledDisplaysTabPage
            // 
            this.ledDisplaysTabPage.Controls.Add(this.arcazeModuleSettingsGroupBox);
            this.ledDisplaysTabPage.Controls.Add(this.arcazeModulesGroupBox);
            this.ledDisplaysTabPage.Controls.Add(this.arcazeSettingsLabel);
            resources.ApplyResources(this.ledDisplaysTabPage, "ledDisplaysTabPage");
            this.ledDisplaysTabPage.Name = "ledDisplaysTabPage";
            this.ledDisplaysTabPage.UseVisualStyleBackColor = true;
            this.ledDisplaysTabPage.Validating += new System.ComponentModel.CancelEventHandler(this.ledDisplaysTabPage_Validating);
            // 
            // arcazeModuleSettingsGroupBox
            // 
            this.arcazeModuleSettingsGroupBox.Controls.Add(this.numModulesLabel);
            this.arcazeModuleSettingsGroupBox.Controls.Add(this.numModulesNumericUpDown);
            this.arcazeModuleSettingsGroupBox.Controls.Add(this.arcazeModuleTypeComboBox);
            this.arcazeModuleSettingsGroupBox.Controls.Add(this.globalBrightnessLabel);
            this.arcazeModuleSettingsGroupBox.Controls.Add(this.globalBrightnessTrackBar);
            this.arcazeModuleSettingsGroupBox.Controls.Add(this.arcazeModuleTypeLabel);
            resources.ApplyResources(this.arcazeModuleSettingsGroupBox, "arcazeModuleSettingsGroupBox");
            this.arcazeModuleSettingsGroupBox.Name = "arcazeModuleSettingsGroupBox";
            this.arcazeModuleSettingsGroupBox.TabStop = false;
            // 
            // numModulesLabel
            // 
            resources.ApplyResources(this.numModulesLabel, "numModulesLabel");
            this.numModulesLabel.Name = "numModulesLabel";
            // 
            // numModulesNumericUpDown
            // 
            resources.ApplyResources(this.numModulesNumericUpDown, "numModulesNumericUpDown");
            this.numModulesNumericUpDown.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.numModulesNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numModulesNumericUpDown.Name = "numModulesNumericUpDown";
            this.toolTip1.SetToolTip(this.numModulesNumericUpDown, resources.GetString("numModulesNumericUpDown.ToolTip"));
            this.numModulesNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numModulesNumericUpDown.ValueChanged += new System.EventHandler(this.numModulesNumericUpDown_ValueChanged);
            // 
            // arcazeModuleTypeComboBox
            // 
            this.arcazeModuleTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.arcazeModuleTypeComboBox.FormattingEnabled = true;
            this.arcazeModuleTypeComboBox.Items.AddRange(new object[] {
            resources.GetString("arcazeModuleTypeComboBox.Items")});
            resources.ApplyResources(this.arcazeModuleTypeComboBox, "arcazeModuleTypeComboBox");
            this.arcazeModuleTypeComboBox.Name = "arcazeModuleTypeComboBox";
            this.arcazeModuleTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.arcazeModuleTypeComboBox_SelectedIndexChanged);
            // 
            // globalBrightnessLabel
            // 
            resources.ApplyResources(this.globalBrightnessLabel, "globalBrightnessLabel");
            this.globalBrightnessLabel.Name = "globalBrightnessLabel";
            // 
            // globalBrightnessTrackBar
            // 
            this.globalBrightnessTrackBar.BackColor = System.Drawing.SystemColors.ControlLightLight;
            resources.ApplyResources(this.globalBrightnessTrackBar, "globalBrightnessTrackBar");
            this.globalBrightnessTrackBar.Maximum = 9;
            this.globalBrightnessTrackBar.Minimum = 1;
            this.globalBrightnessTrackBar.Name = "globalBrightnessTrackBar";
            this.toolTip1.SetToolTip(this.globalBrightnessTrackBar, resources.GetString("globalBrightnessTrackBar.ToolTip"));
            this.globalBrightnessTrackBar.Value = 9;
            this.globalBrightnessTrackBar.ValueChanged += new System.EventHandler(this.numModulesNumericUpDown_ValueChanged);
            // 
            // arcazeModuleTypeLabel
            // 
            resources.ApplyResources(this.arcazeModuleTypeLabel, "arcazeModuleTypeLabel");
            this.arcazeModuleTypeLabel.Name = "arcazeModuleTypeLabel";
            // 
            // arcazeModulesGroupBox
            // 
            this.arcazeModulesGroupBox.Controls.Add(this.ArcazeModuleTreeView);
            resources.ApplyResources(this.arcazeModulesGroupBox, "arcazeModulesGroupBox");
            this.arcazeModulesGroupBox.Name = "arcazeModulesGroupBox";
            this.arcazeModulesGroupBox.TabStop = false;
            // 
            // ArcazeModuleTreeView
            // 
            resources.ApplyResources(this.ArcazeModuleTreeView, "ArcazeModuleTreeView");
            this.ArcazeModuleTreeView.ImageList = this.mfTreeViewImageList;
            this.ArcazeModuleTreeView.Name = "ArcazeModuleTreeView";
            this.ArcazeModuleTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("ArcazeModuleTreeView.Nodes"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("ArcazeModuleTreeView.Nodes1")))});
            this.ArcazeModuleTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ArcazeModuleTreeView_AfterSelect);
            // 
            // mfTreeViewImageList
            // 
            this.mfTreeViewImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            resources.ApplyResources(this.mfTreeViewImageList, "mfTreeViewImageList");
            this.mfTreeViewImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // arcazeSettingsLabel
            // 
            resources.ApplyResources(this.arcazeSettingsLabel, "arcazeSettingsLabel");
            this.arcazeSettingsLabel.Name = "arcazeSettingsLabel";
            // 
            // mfModuleSettingsContextMenuStrip
            // 
            this.mfModuleSettingsContextMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mfModuleSettingsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.toolStripMenuItem1,
            this.uploadToolStripMenuItem,
            this.toolStripMenuItem2,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem3,
            this.updateFirmwareToolStripMenuItem,
            this.regenerateSerialToolStripMenuItem,
            this.reloadConfigToolStripMenuItem});
            this.mfModuleSettingsContextMenuStrip.Name = "mfModuleSettingsContextMenuStrip";
            resources.ApplyResources(this.mfModuleSettingsContextMenuStrip, "mfModuleSettingsContextMenuStrip");
            this.mfModuleSettingsContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.mfModuleSettingsContextMenuStrip_Opening);
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ledOutputToolStripMenuItem,
            this.ledSegmentToolStripMenuItem,
            this.servoToolStripMenuItem,
            this.stepperToolStripMenuItem,
            this.LcdDisplayToolStripMenuItem,
            this.toolStripMenuItem4,
            this.buttonToolStripMenuItem,
            this.encoderToolStripMenuItem});
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            resources.ApplyResources(this.addToolStripMenuItem, "addToolStripMenuItem");
            // 
            // ledOutputToolStripMenuItem
            // 
            this.ledOutputToolStripMenuItem.Name = "ledOutputToolStripMenuItem";
            resources.ApplyResources(this.ledOutputToolStripMenuItem, "ledOutputToolStripMenuItem");
            this.ledOutputToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // ledSegmentToolStripMenuItem
            // 
            this.ledSegmentToolStripMenuItem.Name = "ledSegmentToolStripMenuItem";
            resources.ApplyResources(this.ledSegmentToolStripMenuItem, "ledSegmentToolStripMenuItem");
            this.ledSegmentToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // servoToolStripMenuItem
            // 
            this.servoToolStripMenuItem.Name = "servoToolStripMenuItem";
            resources.ApplyResources(this.servoToolStripMenuItem, "servoToolStripMenuItem");
            this.servoToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // stepperToolStripMenuItem
            // 
            this.stepperToolStripMenuItem.Name = "stepperToolStripMenuItem";
            resources.ApplyResources(this.stepperToolStripMenuItem, "stepperToolStripMenuItem");
            this.stepperToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // LcdDisplayToolStripMenuItem
            // 
            this.LcdDisplayToolStripMenuItem.Name = "LcdDisplayToolStripMenuItem";
            resources.ApplyResources(this.LcdDisplayToolStripMenuItem, "LcdDisplayToolStripMenuItem");
            this.LcdDisplayToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
            // 
            // buttonToolStripMenuItem
            // 
            this.buttonToolStripMenuItem.Name = "buttonToolStripMenuItem";
            resources.ApplyResources(this.buttonToolStripMenuItem, "buttonToolStripMenuItem");
            this.buttonToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // encoderToolStripMenuItem
            // 
            this.encoderToolStripMenuItem.Name = "encoderToolStripMenuItem";
            resources.ApplyResources(this.encoderToolStripMenuItem, "encoderToolStripMenuItem");
            this.encoderToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            resources.ApplyResources(this.removeToolStripMenuItem, "removeToolStripMenuItem");
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeDeviceToolStripButton_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // uploadToolStripMenuItem
            // 
            this.uploadToolStripMenuItem.Name = "uploadToolStripMenuItem";
            resources.ApplyResources(this.uploadToolStripMenuItem, "uploadToolStripMenuItem");
            this.uploadToolStripMenuItem.Click += new System.EventHandler(this.uploadToolStripButton_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripButton_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            resources.ApplyResources(this.saveToolStripMenuItem, "saveToolStripMenuItem");
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripButton_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            resources.ApplyResources(this.saveAsToolStripMenuItem, "saveAsToolStripMenuItem");
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            // 
            // updateFirmwareToolStripMenuItem
            // 
            this.updateFirmwareToolStripMenuItem.Name = "updateFirmwareToolStripMenuItem";
            resources.ApplyResources(this.updateFirmwareToolStripMenuItem, "updateFirmwareToolStripMenuItem");
            this.updateFirmwareToolStripMenuItem.Click += new System.EventHandler(this.updateFirmwareToolStripMenuItem_Click);
            // 
            // regenerateSerialToolStripMenuItem
            // 
            this.regenerateSerialToolStripMenuItem.Name = "regenerateSerialToolStripMenuItem";
            resources.ApplyResources(this.regenerateSerialToolStripMenuItem, "regenerateSerialToolStripMenuItem");
            this.regenerateSerialToolStripMenuItem.Click += new System.EventHandler(this.regenerateSerialToolStripMenuItem_Click);
            // 
            // reloadConfigToolStripMenuItem
            // 
            this.reloadConfigToolStripMenuItem.Name = "reloadConfigToolStripMenuItem";
            resources.ApplyResources(this.reloadConfigToolStripMenuItem, "reloadConfigToolStripMenuItem");
            this.reloadConfigToolStripMenuItem.Click += new System.EventHandler(this.reloadConfigToolStripMenuItem_Click);
            // 
            // generalTabPage
            // 
            this.generalTabPage.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.generalTabPage.Controls.Add(this.offlineModeGroupBox);
            this.generalTabPage.Controls.Add(this.debugGroupBox);
            this.generalTabPage.Controls.Add(this.testModeSpeedGroupBox);
            this.generalTabPage.Controls.Add(this.recentFilesGroupBox);
            resources.ApplyResources(this.generalTabPage, "generalTabPage");
            this.generalTabPage.Name = "generalTabPage";
            // 
            // offlineModeGroupBox
            // 
            this.offlineModeGroupBox.Controls.Add(this.label7);
            this.offlineModeGroupBox.Controls.Add(this.offlineModeCheckBox);
            resources.ApplyResources(this.offlineModeGroupBox, "offlineModeGroupBox");
            this.offlineModeGroupBox.Name = "offlineModeGroupBox";
            this.offlineModeGroupBox.TabStop = false;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // offlineModeCheckBox
            // 
            resources.ApplyResources(this.offlineModeCheckBox, "offlineModeCheckBox");
            this.offlineModeCheckBox.Name = "offlineModeCheckBox";
            this.offlineModeCheckBox.UseVisualStyleBackColor = true;
            // 
            // debugGroupBox
            // 
            this.debugGroupBox.Controls.Add(this.logLevelComboBox);
            this.debugGroupBox.Controls.Add(this.logLevelLabel);
            this.debugGroupBox.Controls.Add(this.logLevelCheckBox);
            resources.ApplyResources(this.debugGroupBox, "debugGroupBox");
            this.debugGroupBox.Name = "debugGroupBox";
            this.debugGroupBox.TabStop = false;
            // 
            // logLevelComboBox
            // 
            this.logLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.logLevelComboBox.FormattingEnabled = true;
            this.logLevelComboBox.Items.AddRange(new object[] {
            resources.GetString("logLevelComboBox.Items"),
            resources.GetString("logLevelComboBox.Items1"),
            resources.GetString("logLevelComboBox.Items2"),
            resources.GetString("logLevelComboBox.Items3")});
            resources.ApplyResources(this.logLevelComboBox, "logLevelComboBox");
            this.logLevelComboBox.Name = "logLevelComboBox";
            // 
            // logLevelLabel
            // 
            resources.ApplyResources(this.logLevelLabel, "logLevelLabel");
            this.logLevelLabel.Name = "logLevelLabel";
            // 
            // logLevelCheckBox
            // 
            resources.ApplyResources(this.logLevelCheckBox, "logLevelCheckBox");
            this.logLevelCheckBox.Name = "logLevelCheckBox";
            this.logLevelCheckBox.UseVisualStyleBackColor = true;
            // 
            // testModeSpeedGroupBox
            // 
            this.testModeSpeedGroupBox.Controls.Add(this.label8);
            this.testModeSpeedGroupBox.Controls.Add(this.label6);
            this.testModeSpeedGroupBox.Controls.Add(this.testModeSpeedTrackBar);
            resources.ApplyResources(this.testModeSpeedGroupBox, "testModeSpeedGroupBox");
            this.testModeSpeedGroupBox.Name = "testModeSpeedGroupBox";
            this.testModeSpeedGroupBox.TabStop = false;
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // testModeSpeedTrackBar
            // 
            resources.ApplyResources(this.testModeSpeedTrackBar, "testModeSpeedTrackBar");
            this.testModeSpeedTrackBar.Maximum = 4;
            this.testModeSpeedTrackBar.Name = "testModeSpeedTrackBar";
            // 
            // recentFilesGroupBox
            // 
            this.recentFilesGroupBox.Controls.Add(this.label1);
            this.recentFilesGroupBox.Controls.Add(this.recentFilesNumericUpDown);
            resources.ApplyResources(this.recentFilesGroupBox, "recentFilesGroupBox");
            this.recentFilesGroupBox.Name = "recentFilesGroupBox";
            this.recentFilesGroupBox.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoEllipsis = true;
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // recentFilesNumericUpDown
            // 
            resources.ApplyResources(this.recentFilesNumericUpDown, "recentFilesNumericUpDown");
            this.recentFilesNumericUpDown.Name = "recentFilesNumericUpDown";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.generalTabPage);
            this.tabControl1.Controls.Add(this.ledDisplaysTabPage);
            this.tabControl1.Controls.Add(this.mobiFlightTabPage);
            this.tabControl1.Controls.Add(this.fsuipcTabPage);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // mobiFlightTabPage
            // 
            this.mobiFlightTabPage.Controls.Add(this.mfConfiguredModulesGroupBox);
            this.mobiFlightTabPage.Controls.Add(this.firmwareSettingsGroupBox);
            this.mobiFlightTabPage.Controls.Add(this.mobiflightSettingsLabel);
            resources.ApplyResources(this.mobiFlightTabPage, "mobiFlightTabPage");
            this.mobiFlightTabPage.Name = "mobiFlightTabPage";
            this.mobiFlightTabPage.UseVisualStyleBackColor = true;
            // 
            // mfConfiguredModulesGroupBox
            // 
            this.mfConfiguredModulesGroupBox.Controls.Add(this.mfModulesTreeView);
            this.mfConfiguredModulesGroupBox.Controls.Add(this.mfSettingsPanel);
            this.mfConfiguredModulesGroupBox.Controls.Add(this.mobiflightSettingsToolStrip);
            resources.ApplyResources(this.mfConfiguredModulesGroupBox, "mfConfiguredModulesGroupBox");
            this.mfConfiguredModulesGroupBox.Name = "mfConfiguredModulesGroupBox";
            this.mfConfiguredModulesGroupBox.TabStop = false;
            // 
            // mfModulesTreeView
            // 
            this.mfModulesTreeView.ContextMenuStrip = this.mfModuleSettingsContextMenuStrip;
            resources.ApplyResources(this.mfModulesTreeView, "mfModulesTreeView");
            this.mfModulesTreeView.ImageList = this.mfTreeViewImageList;
            this.mfModulesTreeView.Name = "mfModulesTreeView";
            this.mfModulesTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("mfModulesTreeView.Nodes"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("mfModulesTreeView.Nodes1")))});
            this.mfModulesTreeView.ShowNodeToolTips = true;
            this.mfModulesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.mfModulesTreeView_AfterSelect);
            // 
            // mfSettingsPanel
            // 
            resources.ApplyResources(this.mfSettingsPanel, "mfSettingsPanel");
            this.mfSettingsPanel.Name = "mfSettingsPanel";
            // 
            // mobiflightSettingsToolStrip
            // 
            resources.ApplyResources(this.mobiflightSettingsToolStrip, "mobiflightSettingsToolStrip");
            this.mobiflightSettingsToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uploadToolStripButton,
            this.toolStripSeparator1,
            this.openToolStripButton,
            this.saveToolStripButton,
            this.toolStripSeparator2,
            this.addDeviceToolStripDropDownButton,
            this.removeDeviceToolStripButton,
            this.toolStripSeparator4});
            this.mobiflightSettingsToolStrip.Name = "mobiflightSettingsToolStrip";
            this.mobiflightSettingsToolStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.mobiflightSettingsToolStrip_ItemClicked);
            // 
            // uploadToolStripButton
            // 
            this.uploadToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.uploadToolStripButton.Image = global::MobiFlight.Properties.Resources.export1;
            resources.ApplyResources(this.uploadToolStripButton, "uploadToolStripButton");
            this.uploadToolStripButton.Name = "uploadToolStripButton";
            this.uploadToolStripButton.Click += new System.EventHandler(this.uploadToolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // openToolStripButton
            // 
            this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openToolStripButton.Image = global::MobiFlight.Properties.Resources.folder;
            resources.ApplyResources(this.openToolStripButton, "openToolStripButton");
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Click += new System.EventHandler(this.openToolStripButton_Click);
            // 
            // saveToolStripButton
            // 
            this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveToolStripButton.Image = global::MobiFlight.Properties.Resources.disk_blue;
            resources.ApplyResources(this.saveToolStripButton, "saveToolStripButton");
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Click += new System.EventHandler(this.saveToolStripButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // addDeviceToolStripDropDownButton
            // 
            this.addDeviceToolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addEncoderToolStripMenuItem,
            this.addButtonToolStripMenuItem,
            this.toolStripSeparator3,
            this.addStepperToolStripMenuItem,
            this.addServoToolStripMenuItem,
            this.addLedModuleToolStripMenuItem,
            this.addOutputToolStripMenuItem,
            this.addLcdDisplayToolStripMenuItem});
            this.addDeviceToolStripDropDownButton.Image = global::MobiFlight.Properties.Resources.star_yellow_add;
            resources.ApplyResources(this.addDeviceToolStripDropDownButton, "addDeviceToolStripDropDownButton");
            this.addDeviceToolStripDropDownButton.Name = "addDeviceToolStripDropDownButton";
            // 
            // addEncoderToolStripMenuItem
            // 
            this.addEncoderToolStripMenuItem.Name = "addEncoderToolStripMenuItem";
            resources.ApplyResources(this.addEncoderToolStripMenuItem, "addEncoderToolStripMenuItem");
            this.addEncoderToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // addButtonToolStripMenuItem
            // 
            this.addButtonToolStripMenuItem.Name = "addButtonToolStripMenuItem";
            resources.ApplyResources(this.addButtonToolStripMenuItem, "addButtonToolStripMenuItem");
            this.addButtonToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // addStepperToolStripMenuItem
            // 
            this.addStepperToolStripMenuItem.Name = "addStepperToolStripMenuItem";
            resources.ApplyResources(this.addStepperToolStripMenuItem, "addStepperToolStripMenuItem");
            this.addStepperToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // addServoToolStripMenuItem
            // 
            this.addServoToolStripMenuItem.Name = "addServoToolStripMenuItem";
            resources.ApplyResources(this.addServoToolStripMenuItem, "addServoToolStripMenuItem");
            this.addServoToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // addLedModuleToolStripMenuItem
            // 
            this.addLedModuleToolStripMenuItem.Name = "addLedModuleToolStripMenuItem";
            resources.ApplyResources(this.addLedModuleToolStripMenuItem, "addLedModuleToolStripMenuItem");
            this.addLedModuleToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // addOutputToolStripMenuItem
            // 
            this.addOutputToolStripMenuItem.Name = "addOutputToolStripMenuItem";
            resources.ApplyResources(this.addOutputToolStripMenuItem, "addOutputToolStripMenuItem");
            this.addOutputToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // addLcdDisplayToolStripMenuItem
            // 
            this.addLcdDisplayToolStripMenuItem.Name = "addLcdDisplayToolStripMenuItem";
            resources.ApplyResources(this.addLcdDisplayToolStripMenuItem, "addLcdDisplayToolStripMenuItem");
            this.addLcdDisplayToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // removeDeviceToolStripButton
            // 
            this.removeDeviceToolStripButton.Image = global::MobiFlight.Properties.Resources.star_yellow_delete;
            resources.ApplyResources(this.removeDeviceToolStripButton, "removeDeviceToolStripButton");
            this.removeDeviceToolStripButton.Name = "removeDeviceToolStripButton";
            this.removeDeviceToolStripButton.Click += new System.EventHandler(this.removeDeviceToolStripButton_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // firmwareSettingsGroupBox
            // 
            this.firmwareSettingsGroupBox.Controls.Add(this.FwAutoUpdateCheckBox);
            this.firmwareSettingsGroupBox.Controls.Add(this.firmwareArduinoIdeButton);
            this.firmwareSettingsGroupBox.Controls.Add(this.firmwareArduinoIdePathTextBox);
            this.firmwareSettingsGroupBox.Controls.Add(this.firmwareArduinoIdeLabel);
            resources.ApplyResources(this.firmwareSettingsGroupBox, "firmwareSettingsGroupBox");
            this.firmwareSettingsGroupBox.Name = "firmwareSettingsGroupBox";
            this.firmwareSettingsGroupBox.TabStop = false;
            // 
            // FwAutoUpdateCheckBox
            // 
            resources.ApplyResources(this.FwAutoUpdateCheckBox, "FwAutoUpdateCheckBox");
            this.FwAutoUpdateCheckBox.Name = "FwAutoUpdateCheckBox";
            this.toolTip1.SetToolTip(this.FwAutoUpdateCheckBox, resources.GetString("FwAutoUpdateCheckBox.ToolTip"));
            this.FwAutoUpdateCheckBox.UseVisualStyleBackColor = true;
            // 
            // firmwareArduinoIdeButton
            // 
            this.firmwareArduinoIdeButton.Image = global::MobiFlight.Properties.Resources.folder1;
            resources.ApplyResources(this.firmwareArduinoIdeButton, "firmwareArduinoIdeButton");
            this.firmwareArduinoIdeButton.Name = "firmwareArduinoIdeButton";
            this.firmwareArduinoIdeButton.UseVisualStyleBackColor = true;
            this.firmwareArduinoIdeButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // firmwareArduinoIdePathTextBox
            // 
            resources.ApplyResources(this.firmwareArduinoIdePathTextBox, "firmwareArduinoIdePathTextBox");
            this.firmwareArduinoIdePathTextBox.Name = "firmwareArduinoIdePathTextBox";
            this.firmwareArduinoIdePathTextBox.TextChanged += new System.EventHandler(this.firmwareArduinoIdePathTextBox_TextChanged);
            this.firmwareArduinoIdePathTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.firmwareArduinoIdePathTextBox_Validating);
            // 
            // firmwareArduinoIdeLabel
            // 
            resources.ApplyResources(this.firmwareArduinoIdeLabel, "firmwareArduinoIdeLabel");
            this.firmwareArduinoIdeLabel.Name = "firmwareArduinoIdeLabel";
            // 
            // mobiflightSettingsLabel
            // 
            resources.ApplyResources(this.mobiflightSettingsLabel, "mobiflightSettingsLabel");
            this.mobiflightSettingsLabel.Name = "mobiflightSettingsLabel";
            this.mobiflightSettingsLabel.Click += new System.EventHandler(this.mobiflightSettingsLabel_Click);
            // 
            // fsuipcTabPage
            // 
            this.fsuipcTabPage.Controls.Add(this.groupBox1);
            resources.ApplyResources(this.fsuipcTabPage, "fsuipcTabPage");
            this.fsuipcTabPage.Name = "fsuipcTabPage";
            this.fsuipcTabPage.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.fsuipcPollIntervalTrackBar);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // fsuipcPollIntervalTrackBar
            // 
            this.fsuipcPollIntervalTrackBar.BackColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.fsuipcPollIntervalTrackBar, "fsuipcPollIntervalTrackBar");
            this.fsuipcPollIntervalTrackBar.LargeChange = 2;
            this.fsuipcPollIntervalTrackBar.Minimum = 2;
            this.fsuipcPollIntervalTrackBar.Name = "fsuipcPollIntervalTrackBar";
            this.fsuipcPollIntervalTrackBar.Value = 10;
            // 
            // firmwareSettingsToolStripMenuItem
            // 
            this.firmwareSettingsToolStripMenuItem.Name = "firmwareSettingsToolStripMenuItem";
            resources.ApplyResources(this.firmwareSettingsToolStripMenuItem, "firmwareSettingsToolStripMenuItem");
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // firmwareUpdateToolStripMenuItem
            // 
            this.firmwareUpdateToolStripMenuItem.Name = "firmwareUpdateToolStripMenuItem";
            resources.ApplyResources(this.firmwareUpdateToolStripMenuItem, "firmwareUpdateToolStripMenuItem");
            // 
            // toolTip1
            // 
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip1.ToolTipTitle = "Hint";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // SettingsDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "SettingsDialog";
            this.Shown += new System.EventHandler(this.SettingsDialog_Shown);
            this.panel1.ResumeLayout(false);
            this.ledDisplaysTabPage.ResumeLayout(false);
            this.arcazeModuleSettingsGroupBox.ResumeLayout(false);
            this.arcazeModuleSettingsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numModulesNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.globalBrightnessTrackBar)).EndInit();
            this.arcazeModulesGroupBox.ResumeLayout(false);
            this.mfModuleSettingsContextMenuStrip.ResumeLayout(false);
            this.generalTabPage.ResumeLayout(false);
            this.offlineModeGroupBox.ResumeLayout(false);
            this.offlineModeGroupBox.PerformLayout();
            this.debugGroupBox.ResumeLayout(false);
            this.debugGroupBox.PerformLayout();
            this.testModeSpeedGroupBox.ResumeLayout(false);
            this.testModeSpeedGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testModeSpeedTrackBar)).EndInit();
            this.recentFilesGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.recentFilesNumericUpDown)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.mobiFlightTabPage.ResumeLayout(false);
            this.mfConfiguredModulesGroupBox.ResumeLayout(false);
            this.mfConfiguredModulesGroupBox.PerformLayout();
            this.mobiflightSettingsToolStrip.ResumeLayout(false);
            this.mobiflightSettingsToolStrip.PerformLayout();
            this.firmwareSettingsGroupBox.ResumeLayout(false);
            this.firmwareSettingsGroupBox.PerformLayout();
            this.fsuipcTabPage.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fsuipcPollIntervalTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TabPage ledDisplaysTabPage;
        private System.Windows.Forms.GroupBox arcazeModulesGroupBox;
        private System.Windows.Forms.TabPage generalTabPage;
        private System.Windows.Forms.GroupBox recentFilesGroupBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown recentFilesNumericUpDown;
        private System.Windows.Forms.TabPage fsuipcTabPage;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar fsuipcPollIntervalTrackBar;
        private System.Windows.Forms.GroupBox arcazeModuleSettingsGroupBox;
        private System.Windows.Forms.Label numModulesLabel;
        private System.Windows.Forms.NumericUpDown numModulesNumericUpDown;
        private System.Windows.Forms.ComboBox arcazeModuleTypeComboBox;
        private System.Windows.Forms.Label globalBrightnessLabel;
        private System.Windows.Forms.TrackBar globalBrightnessTrackBar;
        private System.Windows.Forms.Label arcazeModuleTypeLabel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox testModeSpeedGroupBox;
        private System.Windows.Forms.TrackBar testModeSpeedTrackBar;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox mfConfiguredModulesGroupBox;
        private System.Windows.Forms.Panel mfSettingsPanel;
        private System.Windows.Forms.TreeView mfModulesTreeView;
        private System.Windows.Forms.Label mobiflightSettingsLabel;
        private System.Windows.Forms.ContextMenuStrip mfModuleSettingsContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ledOutputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ledSegmentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem servoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stepperToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem uploadToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStrip mobiflightSettingsToolStrip;
        private System.Windows.Forms.ToolStripButton openToolStripButton;
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.ToolStripButton uploadToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton removeDeviceToolStripButton;
        private System.Windows.Forms.ToolStripDropDownButton addDeviceToolStripDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem addStepperToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addServoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addLedModuleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addOutputToolStripMenuItem;
        private System.Windows.Forms.ImageList mfTreeViewImageList;
        private System.Windows.Forms.ToolStripMenuItem addEncoderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addButtonToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.GroupBox debugGroupBox;
        private System.Windows.Forms.ComboBox logLevelComboBox;
        private System.Windows.Forms.Label logLevelLabel;
        private System.Windows.Forms.CheckBox logLevelCheckBox;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem updateFirmwareToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem firmwareSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem firmwareUpdateToolStripMenuItem;
        private System.Windows.Forms.GroupBox firmwareSettingsGroupBox;
        private System.Windows.Forms.Button firmwareArduinoIdeButton;
        private System.Windows.Forms.TextBox firmwareArduinoIdePathTextBox;
        private System.Windows.Forms.Label firmwareArduinoIdeLabel;
        private System.ComponentModel.BackgroundWorker firmwareUpdateBackgroundWorker;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem buttonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem encoderToolStripMenuItem;
        public System.Windows.Forms.TabControl tabControl1;
        public System.Windows.Forms.TabPage mobiFlightTabPage;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.ToolStripMenuItem regenerateSerialToolStripMenuItem;
        private System.Windows.Forms.TreeView ArcazeModuleTreeView;
        private System.Windows.Forms.Label arcazeSettingsLabel;
        private System.Windows.Forms.CheckBox FwAutoUpdateCheckBox;
        private System.Windows.Forms.ToolStripMenuItem reloadConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LcdDisplayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addLcdDisplayToolStripMenuItem;
        private System.Windows.Forms.GroupBox offlineModeGroupBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox offlineModeCheckBox;
    }
}