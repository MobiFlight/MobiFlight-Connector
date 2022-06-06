namespace MobiFlight.UI.Panels.Settings
{
    partial class MobiFlightPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MobiFlightPanel));
            this.mfConfiguredModulesGroupBox = new System.Windows.Forms.GroupBox();
            this.mfModulesTreeView = new System.Windows.Forms.TreeView();
            this.mfModuleSettingsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ledOutputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ledSegmentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.servoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stepperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LcdDisplayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ShiftRegisterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.encoderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analogDeviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inputShiftRegisterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inputMultiplexerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.uploadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.updateFirmwareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.regenerateSerialToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.ignoreCOMPortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dontIgnoreCOMPortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mfTreeViewImageList = new System.Windows.Forms.ImageList(this.components);
            this.mfSettingsPanel = new System.Windows.Forms.Panel();
            this.mobiflightSettingsToolStrip = new System.Windows.Forms.ToolStrip();
            this.uploadToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.openToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.addDeviceToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.addOutputToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addLedModuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addServoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addStepperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addLcdDisplayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addShiftRegisterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.addEncoderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addButtonToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.analogDeviceToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addInputShiftRegisterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addInputMultiplexerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeDeviceToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.mobiflightSettingsLabel = new System.Windows.Forms.Label();
            this.firmwareSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.IgnoredComPortsLabel = new System.Windows.Forms.Label();
            this.IgnoredComPortsTextBox = new System.Windows.Forms.TextBox();
            this.IgnoreComPortsCheckBox = new System.Windows.Forms.CheckBox();
            this.FwAutoInstallCheckBox = new System.Windows.Forms.CheckBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.CompatibleBoardScanToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.mfConfiguredModulesGroupBox.SuspendLayout();
            this.mfModuleSettingsContextMenuStrip.SuspendLayout();
            this.mobiflightSettingsToolStrip.SuspendLayout();
            this.firmwareSettingsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // mfConfiguredModulesGroupBox
            // 
            resources.ApplyResources(this.mfConfiguredModulesGroupBox, "mfConfiguredModulesGroupBox");
            this.mfConfiguredModulesGroupBox.Controls.Add(this.mfModulesTreeView);
            this.mfConfiguredModulesGroupBox.Controls.Add(this.mfSettingsPanel);
            this.mfConfiguredModulesGroupBox.Controls.Add(this.mobiflightSettingsToolStrip);
            this.errorProvider1.SetError(this.mfConfiguredModulesGroupBox, resources.GetString("mfConfiguredModulesGroupBox.Error"));
            this.errorProvider1.SetIconAlignment(this.mfConfiguredModulesGroupBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("mfConfiguredModulesGroupBox.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.mfConfiguredModulesGroupBox, ((int)(resources.GetObject("mfConfiguredModulesGroupBox.IconPadding"))));
            this.mfConfiguredModulesGroupBox.Name = "mfConfiguredModulesGroupBox";
            this.mfConfiguredModulesGroupBox.TabStop = false;
            this.CompatibleBoardScanToolTip.SetToolTip(this.mfConfiguredModulesGroupBox, resources.GetString("mfConfiguredModulesGroupBox.ToolTip"));
            // 
            // mfModulesTreeView
            // 
            resources.ApplyResources(this.mfModulesTreeView, "mfModulesTreeView");
            this.mfModulesTreeView.ContextMenuStrip = this.mfModuleSettingsContextMenuStrip;
            this.errorProvider1.SetError(this.mfModulesTreeView, resources.GetString("mfModulesTreeView.Error"));
            this.errorProvider1.SetIconAlignment(this.mfModulesTreeView, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("mfModulesTreeView.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.mfModulesTreeView, ((int)(resources.GetObject("mfModulesTreeView.IconPadding"))));
            this.mfModulesTreeView.ImageList = this.mfTreeViewImageList;
            this.mfModulesTreeView.Name = "mfModulesTreeView";
            this.mfModulesTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("mfModulesTreeView.Nodes"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("mfModulesTreeView.Nodes1")))});
            this.mfModulesTreeView.ShowNodeToolTips = true;
            this.CompatibleBoardScanToolTip.SetToolTip(this.mfModulesTreeView, resources.GetString("mfModulesTreeView.ToolTip"));
            this.mfModulesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.mfModulesTreeView_AfterSelect);
            this.mfModulesTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.mfModulesTreeView_NodeMouseClick);
            // 
            // mfModuleSettingsContextMenuStrip
            // 
            resources.ApplyResources(this.mfModuleSettingsContextMenuStrip, "mfModuleSettingsContextMenuStrip");
            this.errorProvider1.SetError(this.mfModuleSettingsContextMenuStrip, resources.GetString("mfModuleSettingsContextMenuStrip.Error"));
            this.errorProvider1.SetIconAlignment(this.mfModuleSettingsContextMenuStrip, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("mfModuleSettingsContextMenuStrip.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.mfModuleSettingsContextMenuStrip, ((int)(resources.GetObject("mfModuleSettingsContextMenuStrip.IconPadding"))));
            this.mfModuleSettingsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.toolStripMenuItem1,
            this.uploadToolStripMenuItem,
            this.toolStripMenuItem2,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripMenuItem3,
            this.updateFirmwareToolStripMenuItem,
            this.regenerateSerialToolStripMenuItem,
            this.reloadConfigToolStripMenuItem,
            this.toolStripMenuItem5,
            this.ignoreCOMPortToolStripMenuItem,
            this.dontIgnoreCOMPortToolStripMenuItem});
            this.mfModuleSettingsContextMenuStrip.Name = "mfModuleSettingsContextMenuStrip";
            this.CompatibleBoardScanToolTip.SetToolTip(this.mfModuleSettingsContextMenuStrip, resources.GetString("mfModuleSettingsContextMenuStrip.ToolTip"));
            // 
            // addToolStripMenuItem
            // 
            resources.ApplyResources(this.addToolStripMenuItem, "addToolStripMenuItem");
            this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ledOutputToolStripMenuItem,
            this.ledSegmentToolStripMenuItem,
            this.servoToolStripMenuItem,
            this.stepperToolStripMenuItem,
            this.LcdDisplayToolStripMenuItem,
            this.ShiftRegisterToolStripMenuItem,
            this.toolStripMenuItem4,
            this.buttonToolStripMenuItem,
            this.encoderToolStripMenuItem,
            this.analogDeviceToolStripMenuItem,
            this.inputShiftRegisterToolStripMenuItem,
            this.inputMultiplexerToolStripMenuItem});
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            // 
            // ledOutputToolStripMenuItem
            // 
            resources.ApplyResources(this.ledOutputToolStripMenuItem, "ledOutputToolStripMenuItem");
            this.ledOutputToolStripMenuItem.Name = "ledOutputToolStripMenuItem";
            this.ledOutputToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // ledSegmentToolStripMenuItem
            // 
            resources.ApplyResources(this.ledSegmentToolStripMenuItem, "ledSegmentToolStripMenuItem");
            this.ledSegmentToolStripMenuItem.Name = "ledSegmentToolStripMenuItem";
            this.ledSegmentToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // servoToolStripMenuItem
            // 
            resources.ApplyResources(this.servoToolStripMenuItem, "servoToolStripMenuItem");
            this.servoToolStripMenuItem.Name = "servoToolStripMenuItem";
            this.servoToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // stepperToolStripMenuItem
            // 
            resources.ApplyResources(this.stepperToolStripMenuItem, "stepperToolStripMenuItem");
            this.stepperToolStripMenuItem.Name = "stepperToolStripMenuItem";
            this.stepperToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // LcdDisplayToolStripMenuItem
            // 
            resources.ApplyResources(this.LcdDisplayToolStripMenuItem, "LcdDisplayToolStripMenuItem");
            this.LcdDisplayToolStripMenuItem.Name = "LcdDisplayToolStripMenuItem";
            this.LcdDisplayToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // ShiftRegisterToolStripMenuItem
            // 
            resources.ApplyResources(this.ShiftRegisterToolStripMenuItem, "ShiftRegisterToolStripMenuItem");
            this.ShiftRegisterToolStripMenuItem.Name = "ShiftRegisterToolStripMenuItem";
            this.ShiftRegisterToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            // 
            // buttonToolStripMenuItem
            // 
            resources.ApplyResources(this.buttonToolStripMenuItem, "buttonToolStripMenuItem");
            this.buttonToolStripMenuItem.Name = "buttonToolStripMenuItem";
            this.buttonToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // encoderToolStripMenuItem
            // 
            resources.ApplyResources(this.encoderToolStripMenuItem, "encoderToolStripMenuItem");
            this.encoderToolStripMenuItem.Name = "encoderToolStripMenuItem";
            this.encoderToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // analogDeviceToolStripMenuItem
            // 
            resources.ApplyResources(this.analogDeviceToolStripMenuItem, "analogDeviceToolStripMenuItem");
            this.analogDeviceToolStripMenuItem.Name = "analogDeviceToolStripMenuItem";
            this.analogDeviceToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // inputShiftRegisterToolStripMenuItem
            // 
            resources.ApplyResources(this.inputShiftRegisterToolStripMenuItem, "inputShiftRegisterToolStripMenuItem");
            this.inputShiftRegisterToolStripMenuItem.Name = "inputShiftRegisterToolStripMenuItem";
            this.inputShiftRegisterToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // inputMultiplexerToolStripMenuItem
            // 
            resources.ApplyResources(this.inputMultiplexerToolStripMenuItem, "inputMultiplexerToolStripMenuItem");
            this.inputMultiplexerToolStripMenuItem.Name = "inputMultiplexerToolStripMenuItem";
            this.inputMultiplexerToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            resources.ApplyResources(this.removeToolStripMenuItem, "removeToolStripMenuItem");
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeDeviceToolStripButton_Click);
            // 
            // toolStripMenuItem1
            // 
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            // 
            // uploadToolStripMenuItem
            // 
            resources.ApplyResources(this.uploadToolStripMenuItem, "uploadToolStripMenuItem");
            this.uploadToolStripMenuItem.Name = "uploadToolStripMenuItem";
            this.uploadToolStripMenuItem.Click += new System.EventHandler(this.uploadToolStripButton_Click);
            // 
            // toolStripMenuItem2
            // 
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            // 
            // openToolStripMenuItem
            // 
            resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripButton_Click);
            // 
            // saveToolStripMenuItem
            // 
            resources.ApplyResources(this.saveToolStripMenuItem, "saveToolStripMenuItem");
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripButton_Click);
            // 
            // toolStripMenuItem3
            // 
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            // 
            // updateFirmwareToolStripMenuItem
            // 
            resources.ApplyResources(this.updateFirmwareToolStripMenuItem, "updateFirmwareToolStripMenuItem");
            this.updateFirmwareToolStripMenuItem.Name = "updateFirmwareToolStripMenuItem";
            this.updateFirmwareToolStripMenuItem.Click += new System.EventHandler(this.updateFirmwareToolStripMenuItem_Click);
            // 
            // regenerateSerialToolStripMenuItem
            // 
            resources.ApplyResources(this.regenerateSerialToolStripMenuItem, "regenerateSerialToolStripMenuItem");
            this.regenerateSerialToolStripMenuItem.Name = "regenerateSerialToolStripMenuItem";
            this.regenerateSerialToolStripMenuItem.Click += new System.EventHandler(this.regenerateSerialToolStripMenuItem_Click);
            // 
            // reloadConfigToolStripMenuItem
            // 
            resources.ApplyResources(this.reloadConfigToolStripMenuItem, "reloadConfigToolStripMenuItem");
            this.reloadConfigToolStripMenuItem.Name = "reloadConfigToolStripMenuItem";
            this.reloadConfigToolStripMenuItem.Click += new System.EventHandler(this.reloadConfigToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            resources.ApplyResources(this.toolStripMenuItem5, "toolStripMenuItem5");
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            // 
            // ignoreCOMPortToolStripMenuItem
            // 
            resources.ApplyResources(this.ignoreCOMPortToolStripMenuItem, "ignoreCOMPortToolStripMenuItem");
            this.ignoreCOMPortToolStripMenuItem.Name = "ignoreCOMPortToolStripMenuItem";
            this.ignoreCOMPortToolStripMenuItem.Click += new System.EventHandler(this.ignoreCOMPortToolStripMenuItem_Click);
            // 
            // dontIgnoreCOMPortToolStripMenuItem
            // 
            resources.ApplyResources(this.dontIgnoreCOMPortToolStripMenuItem, "dontIgnoreCOMPortToolStripMenuItem");
            this.dontIgnoreCOMPortToolStripMenuItem.Name = "dontIgnoreCOMPortToolStripMenuItem";
            this.dontIgnoreCOMPortToolStripMenuItem.Click += new System.EventHandler(this.dontIgnoreCOMPortToolStripMenuItem_Click);
            // 
            // mfTreeViewImageList
            // 
            this.mfTreeViewImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            resources.ApplyResources(this.mfTreeViewImageList, "mfTreeViewImageList");
            this.mfTreeViewImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // mfSettingsPanel
            // 
            resources.ApplyResources(this.mfSettingsPanel, "mfSettingsPanel");
            this.errorProvider1.SetError(this.mfSettingsPanel, resources.GetString("mfSettingsPanel.Error"));
            this.errorProvider1.SetIconAlignment(this.mfSettingsPanel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("mfSettingsPanel.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.mfSettingsPanel, ((int)(resources.GetObject("mfSettingsPanel.IconPadding"))));
            this.mfSettingsPanel.Name = "mfSettingsPanel";
            this.CompatibleBoardScanToolTip.SetToolTip(this.mfSettingsPanel, resources.GetString("mfSettingsPanel.ToolTip"));
            // 
            // mobiflightSettingsToolStrip
            // 
            resources.ApplyResources(this.mobiflightSettingsToolStrip, "mobiflightSettingsToolStrip");
            this.errorProvider1.SetError(this.mobiflightSettingsToolStrip, resources.GetString("mobiflightSettingsToolStrip.Error"));
            this.errorProvider1.SetIconAlignment(this.mobiflightSettingsToolStrip, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("mobiflightSettingsToolStrip.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.mobiflightSettingsToolStrip, ((int)(resources.GetObject("mobiflightSettingsToolStrip.IconPadding"))));
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
            this.CompatibleBoardScanToolTip.SetToolTip(this.mobiflightSettingsToolStrip, resources.GetString("mobiflightSettingsToolStrip.ToolTip"));
            // 
            // uploadToolStripButton
            // 
            resources.ApplyResources(this.uploadToolStripButton, "uploadToolStripButton");
            this.uploadToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.uploadToolStripButton.Image = global::MobiFlight.Properties.Resources.export1;
            this.uploadToolStripButton.Name = "uploadToolStripButton";
            this.uploadToolStripButton.Click += new System.EventHandler(this.uploadToolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // openToolStripButton
            // 
            resources.ApplyResources(this.openToolStripButton, "openToolStripButton");
            this.openToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openToolStripButton.Image = global::MobiFlight.Properties.Resources.folder;
            this.openToolStripButton.Name = "openToolStripButton";
            this.openToolStripButton.Click += new System.EventHandler(this.openToolStripButton_Click);
            // 
            // saveToolStripButton
            // 
            resources.ApplyResources(this.saveToolStripButton, "saveToolStripButton");
            this.saveToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveToolStripButton.Image = global::MobiFlight.Properties.Resources.disk_blue;
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Click += new System.EventHandler(this.saveToolStripButton_Click);
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // addDeviceToolStripDropDownButton
            // 
            resources.ApplyResources(this.addDeviceToolStripDropDownButton, "addDeviceToolStripDropDownButton");
            this.addDeviceToolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addOutputToolStripMenuItem,
            this.addLedModuleToolStripMenuItem,
            this.addServoToolStripMenuItem,
            this.addStepperToolStripMenuItem,
            this.addLcdDisplayToolStripMenuItem,
            this.addShiftRegisterToolStripMenuItem,
            this.toolStripSeparator3,
            this.addEncoderToolStripMenuItem,
            this.addButtonToolStripMenuItem,
            this.analogDeviceToolStripMenuItem1,
            this.addInputShiftRegisterToolStripMenuItem,
            this.addInputMultiplexerToolStripMenuItem});
            this.addDeviceToolStripDropDownButton.Image = global::MobiFlight.Properties.Resources.star_yellow_add;
            this.addDeviceToolStripDropDownButton.Name = "addDeviceToolStripDropDownButton";
            // 
            // addOutputToolStripMenuItem
            // 
            resources.ApplyResources(this.addOutputToolStripMenuItem, "addOutputToolStripMenuItem");
            this.addOutputToolStripMenuItem.Name = "addOutputToolStripMenuItem";
            this.addOutputToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // addLedModuleToolStripMenuItem
            // 
            resources.ApplyResources(this.addLedModuleToolStripMenuItem, "addLedModuleToolStripMenuItem");
            this.addLedModuleToolStripMenuItem.Name = "addLedModuleToolStripMenuItem";
            this.addLedModuleToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // addServoToolStripMenuItem
            // 
            resources.ApplyResources(this.addServoToolStripMenuItem, "addServoToolStripMenuItem");
            this.addServoToolStripMenuItem.Name = "addServoToolStripMenuItem";
            this.addServoToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // addStepperToolStripMenuItem
            // 
            resources.ApplyResources(this.addStepperToolStripMenuItem, "addStepperToolStripMenuItem");
            this.addStepperToolStripMenuItem.Name = "addStepperToolStripMenuItem";
            this.addStepperToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // addLcdDisplayToolStripMenuItem
            // 
            resources.ApplyResources(this.addLcdDisplayToolStripMenuItem, "addLcdDisplayToolStripMenuItem");
            this.addLcdDisplayToolStripMenuItem.Name = "addLcdDisplayToolStripMenuItem";
            this.addLcdDisplayToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // addShiftRegisterToolStripMenuItem
            // 
            resources.ApplyResources(this.addShiftRegisterToolStripMenuItem, "addShiftRegisterToolStripMenuItem");
            this.addShiftRegisterToolStripMenuItem.Name = "addShiftRegisterToolStripMenuItem";
            this.addShiftRegisterToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // addEncoderToolStripMenuItem
            // 
            resources.ApplyResources(this.addEncoderToolStripMenuItem, "addEncoderToolStripMenuItem");
            this.addEncoderToolStripMenuItem.Name = "addEncoderToolStripMenuItem";
            this.addEncoderToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // addButtonToolStripMenuItem
            // 
            resources.ApplyResources(this.addButtonToolStripMenuItem, "addButtonToolStripMenuItem");
            this.addButtonToolStripMenuItem.Name = "addButtonToolStripMenuItem";
            this.addButtonToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // analogDeviceToolStripMenuItem1
            // 
            resources.ApplyResources(this.analogDeviceToolStripMenuItem1, "analogDeviceToolStripMenuItem1");
            this.analogDeviceToolStripMenuItem1.Name = "analogDeviceToolStripMenuItem1";
            this.analogDeviceToolStripMenuItem1.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // addInputShiftRegisterToolStripMenuItem
            // 
            resources.ApplyResources(this.addInputShiftRegisterToolStripMenuItem, "addInputShiftRegisterToolStripMenuItem");
            this.addInputShiftRegisterToolStripMenuItem.Name = "addInputShiftRegisterToolStripMenuItem";
            this.addInputShiftRegisterToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // addInputMultiplexerToolStripMenuItem
            // 
            resources.ApplyResources(this.addInputMultiplexerToolStripMenuItem, "addInputMultiplexerToolStripMenuItem");
            this.addInputMultiplexerToolStripMenuItem.Name = "addInputMultiplexerToolStripMenuItem";
            this.addInputMultiplexerToolStripMenuItem.Click += new System.EventHandler(this.addDeviceTypeToolStripMenuItem_Click);
            // 
            // removeDeviceToolStripButton
            // 
            resources.ApplyResources(this.removeDeviceToolStripButton, "removeDeviceToolStripButton");
            this.removeDeviceToolStripButton.Image = global::MobiFlight.Properties.Resources.star_yellow_delete;
            this.removeDeviceToolStripButton.Name = "removeDeviceToolStripButton";
            this.removeDeviceToolStripButton.Click += new System.EventHandler(this.removeDeviceToolStripButton_Click);
            // 
            // toolStripSeparator4
            // 
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            // 
            // mobiflightSettingsLabel
            // 
            resources.ApplyResources(this.mobiflightSettingsLabel, "mobiflightSettingsLabel");
            this.errorProvider1.SetError(this.mobiflightSettingsLabel, resources.GetString("mobiflightSettingsLabel.Error"));
            this.errorProvider1.SetIconAlignment(this.mobiflightSettingsLabel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("mobiflightSettingsLabel.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.mobiflightSettingsLabel, ((int)(resources.GetObject("mobiflightSettingsLabel.IconPadding"))));
            this.mobiflightSettingsLabel.Name = "mobiflightSettingsLabel";
            this.CompatibleBoardScanToolTip.SetToolTip(this.mobiflightSettingsLabel, resources.GetString("mobiflightSettingsLabel.ToolTip"));
            // 
            // firmwareSettingsGroupBox
            // 
            resources.ApplyResources(this.firmwareSettingsGroupBox, "firmwareSettingsGroupBox");
            this.firmwareSettingsGroupBox.Controls.Add(this.IgnoredComPortsLabel);
            this.firmwareSettingsGroupBox.Controls.Add(this.IgnoredComPortsTextBox);
            this.firmwareSettingsGroupBox.Controls.Add(this.IgnoreComPortsCheckBox);
            this.firmwareSettingsGroupBox.Controls.Add(this.FwAutoInstallCheckBox);
            this.errorProvider1.SetError(this.firmwareSettingsGroupBox, resources.GetString("firmwareSettingsGroupBox.Error"));
            this.errorProvider1.SetIconAlignment(this.firmwareSettingsGroupBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("firmwareSettingsGroupBox.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.firmwareSettingsGroupBox, ((int)(resources.GetObject("firmwareSettingsGroupBox.IconPadding"))));
            this.firmwareSettingsGroupBox.Name = "firmwareSettingsGroupBox";
            this.firmwareSettingsGroupBox.TabStop = false;
            this.CompatibleBoardScanToolTip.SetToolTip(this.firmwareSettingsGroupBox, resources.GetString("firmwareSettingsGroupBox.ToolTip"));
            // 
            // IgnoredComPortsLabel
            // 
            resources.ApplyResources(this.IgnoredComPortsLabel, "IgnoredComPortsLabel");
            this.errorProvider1.SetError(this.IgnoredComPortsLabel, resources.GetString("IgnoredComPortsLabel.Error"));
            this.errorProvider1.SetIconAlignment(this.IgnoredComPortsLabel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("IgnoredComPortsLabel.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.IgnoredComPortsLabel, ((int)(resources.GetObject("IgnoredComPortsLabel.IconPadding"))));
            this.IgnoredComPortsLabel.Name = "IgnoredComPortsLabel";
            this.CompatibleBoardScanToolTip.SetToolTip(this.IgnoredComPortsLabel, resources.GetString("IgnoredComPortsLabel.ToolTip"));
            // 
            // IgnoredComPortsTextBox
            // 
            resources.ApplyResources(this.IgnoredComPortsTextBox, "IgnoredComPortsTextBox");
            this.errorProvider1.SetError(this.IgnoredComPortsTextBox, resources.GetString("IgnoredComPortsTextBox.Error"));
            this.errorProvider1.SetIconAlignment(this.IgnoredComPortsTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("IgnoredComPortsTextBox.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.IgnoredComPortsTextBox, ((int)(resources.GetObject("IgnoredComPortsTextBox.IconPadding"))));
            this.IgnoredComPortsTextBox.Name = "IgnoredComPortsTextBox";
            this.CompatibleBoardScanToolTip.SetToolTip(this.IgnoredComPortsTextBox, resources.GetString("IgnoredComPortsTextBox.ToolTip"));
            // 
            // IgnoreComPortsCheckBox
            // 
            resources.ApplyResources(this.IgnoreComPortsCheckBox, "IgnoreComPortsCheckBox");
            this.errorProvider1.SetError(this.IgnoreComPortsCheckBox, resources.GetString("IgnoreComPortsCheckBox.Error"));
            this.errorProvider1.SetIconAlignment(this.IgnoreComPortsCheckBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("IgnoreComPortsCheckBox.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.IgnoreComPortsCheckBox, ((int)(resources.GetObject("IgnoreComPortsCheckBox.IconPadding"))));
            this.IgnoreComPortsCheckBox.Name = "IgnoreComPortsCheckBox";
            this.CompatibleBoardScanToolTip.SetToolTip(this.IgnoreComPortsCheckBox, resources.GetString("IgnoreComPortsCheckBox.ToolTip"));
            this.IgnoreComPortsCheckBox.UseVisualStyleBackColor = true;
            // 
            // FwAutoInstallCheckBox
            // 
            resources.ApplyResources(this.FwAutoInstallCheckBox, "FwAutoInstallCheckBox");
            this.errorProvider1.SetError(this.FwAutoInstallCheckBox, resources.GetString("FwAutoInstallCheckBox.Error"));
            this.errorProvider1.SetIconAlignment(this.FwAutoInstallCheckBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("FwAutoInstallCheckBox.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.FwAutoInstallCheckBox, ((int)(resources.GetObject("FwAutoInstallCheckBox.IconPadding"))));
            this.FwAutoInstallCheckBox.Name = "FwAutoInstallCheckBox";
            this.CompatibleBoardScanToolTip.SetToolTip(this.FwAutoInstallCheckBox, resources.GetString("FwAutoInstallCheckBox.ToolTip"));
            this.FwAutoInstallCheckBox.UseVisualStyleBackColor = true;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            resources.ApplyResources(this.errorProvider1, "errorProvider1");
            // 
            // MobiFlightPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mfConfiguredModulesGroupBox);
            this.Controls.Add(this.firmwareSettingsGroupBox);
            this.Controls.Add(this.mobiflightSettingsLabel);
            this.errorProvider1.SetError(this, resources.GetString("$this.Error"));
            this.errorProvider1.SetIconAlignment(this, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("$this.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this, ((int)(resources.GetObject("$this.IconPadding"))));
            this.Name = "MobiFlightPanel";
            this.CompatibleBoardScanToolTip.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Load += new System.EventHandler(this.MobiFlightPanel_Load);
            this.mfConfiguredModulesGroupBox.ResumeLayout(false);
            this.mfConfiguredModulesGroupBox.PerformLayout();
            this.mfModuleSettingsContextMenuStrip.ResumeLayout(false);
            this.mobiflightSettingsToolStrip.ResumeLayout(false);
            this.mobiflightSettingsToolStrip.PerformLayout();
            this.firmwareSettingsGroupBox.ResumeLayout(false);
            this.firmwareSettingsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox mfConfiguredModulesGroupBox;
        private System.Windows.Forms.TreeView mfModulesTreeView;
        private System.Windows.Forms.Panel mfSettingsPanel;
        private System.Windows.Forms.ToolStrip mobiflightSettingsToolStrip;
        private System.Windows.Forms.ToolStripButton uploadToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton openToolStripButton;
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripDropDownButton addDeviceToolStripDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem addEncoderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addButtonToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem addStepperToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addServoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addLedModuleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addOutputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addLcdDisplayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addShiftRegisterToolStripMenuItem;        
        private System.Windows.Forms.ToolStripButton removeDeviceToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ImageList mfTreeViewImageList;
        private System.Windows.Forms.ContextMenuStrip mfModuleSettingsContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ledOutputToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ledSegmentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem servoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stepperToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LcdDisplayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ShiftRegisterToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem buttonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem encoderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem uploadToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem updateFirmwareToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem regenerateSerialToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadConfigToolStripMenuItem;
        private System.Windows.Forms.Label mobiflightSettingsLabel;
        private System.Windows.Forms.GroupBox firmwareSettingsGroupBox;
        private System.Windows.Forms.CheckBox FwAutoInstallCheckBox;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.ToolStripMenuItem analogDeviceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem analogDeviceToolStripMenuItem1;
        private System.Windows.Forms.ToolTip CompatibleBoardScanToolTip;
        private System.Windows.Forms.ToolStripMenuItem addInputShiftRegisterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inputShiftRegisterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addInputMultiplexerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inputMultiplexerToolStripMenuItem;
        private System.Windows.Forms.Label IgnoredComPortsLabel;
        private System.Windows.Forms.TextBox IgnoredComPortsTextBox;
        private System.Windows.Forms.CheckBox IgnoreComPortsCheckBox;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem ignoreCOMPortToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dontIgnoreCOMPortToolStripMenuItem;
    }
}
