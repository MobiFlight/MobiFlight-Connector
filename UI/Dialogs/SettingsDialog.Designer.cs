namespace MobiFlight.UI.Dialogs
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
            this.ArcazeTabPage = new System.Windows.Forms.TabPage();
            this.mfTreeViewImageList = new System.Windows.Forms.ImageList(this.components);
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
            this.analogDeviceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.mobiFlightTabPage = new System.Windows.Forms.TabPage();
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
            this.generalPanel = new MobiFlight.UI.Panels.Settings.GeneralPanel();
            this.arcazePanel = new MobiFlight.UI.Panels.Settings.ArcazePanel();
            this.mobiFlightPanel = new MobiFlight.UI.Panels.Settings.MobiFlightPanel();
            this.panel1.SuspendLayout();
            this.ArcazeTabPage.SuspendLayout();
            this.mfModuleSettingsContextMenuStrip.SuspendLayout();
            this.generalTabPage.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.mobiFlightTabPage.SuspendLayout();
            this.fsuipcTabPage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fsuipcPollIntervalTrackBar)).BeginInit();
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
            this.cancelButton.CausesValidation = false;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // ArcazeTabPage
            // 
            this.ArcazeTabPage.Controls.Add(this.arcazePanel);
            resources.ApplyResources(this.ArcazeTabPage, "ArcazeTabPage");
            this.ArcazeTabPage.Name = "ArcazeTabPage";
            this.ArcazeTabPage.UseVisualStyleBackColor = true;
            this.ArcazeTabPage.Validating += new System.ComponentModel.CancelEventHandler(this.ledDisplaysTabPage_Validating);
            // 
            // mfTreeViewImageList
            // 
            this.mfTreeViewImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            resources.ApplyResources(this.mfTreeViewImageList, "mfTreeViewImageList");
            this.mfTreeViewImageList.TransparentColor = System.Drawing.Color.Transparent;
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
            this.encoderToolStripMenuItem,
            this.analogDeviceToolStripMenuItem});
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            resources.ApplyResources(this.addToolStripMenuItem, "addToolStripMenuItem");
            // 
            // ledOutputToolStripMenuItem
            // 
            this.ledOutputToolStripMenuItem.Name = "ledOutputToolStripMenuItem";
            resources.ApplyResources(this.ledOutputToolStripMenuItem, "ledOutputToolStripMenuItem");
            // 
            // ledSegmentToolStripMenuItem
            // 
            this.ledSegmentToolStripMenuItem.Name = "ledSegmentToolStripMenuItem";
            resources.ApplyResources(this.ledSegmentToolStripMenuItem, "ledSegmentToolStripMenuItem");
            // 
            // servoToolStripMenuItem
            // 
            this.servoToolStripMenuItem.Name = "servoToolStripMenuItem";
            resources.ApplyResources(this.servoToolStripMenuItem, "servoToolStripMenuItem");
            // 
            // stepperToolStripMenuItem
            // 
            this.stepperToolStripMenuItem.Name = "stepperToolStripMenuItem";
            resources.ApplyResources(this.stepperToolStripMenuItem, "stepperToolStripMenuItem");
            // 
            // LcdDisplayToolStripMenuItem
            // 
            this.LcdDisplayToolStripMenuItem.Name = "LcdDisplayToolStripMenuItem";
            resources.ApplyResources(this.LcdDisplayToolStripMenuItem, "LcdDisplayToolStripMenuItem");
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
            // 
            // encoderToolStripMenuItem
            // 
            this.encoderToolStripMenuItem.Name = "encoderToolStripMenuItem";
            resources.ApplyResources(this.encoderToolStripMenuItem, "encoderToolStripMenuItem");
            // 
            // analogDeviceToolStripMenuItem
            // 
            this.analogDeviceToolStripMenuItem.Name = "analogDeviceToolStripMenuItem";
            resources.ApplyResources(this.analogDeviceToolStripMenuItem, "analogDeviceToolStripMenuItem");
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            resources.ApplyResources(this.removeToolStripMenuItem, "removeToolStripMenuItem");
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
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            resources.ApplyResources(this.saveToolStripMenuItem, "saveToolStripMenuItem");
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
            // 
            // regenerateSerialToolStripMenuItem
            // 
            this.regenerateSerialToolStripMenuItem.Name = "regenerateSerialToolStripMenuItem";
            resources.ApplyResources(this.regenerateSerialToolStripMenuItem, "regenerateSerialToolStripMenuItem");
            // 
            // reloadConfigToolStripMenuItem
            // 
            this.reloadConfigToolStripMenuItem.Name = "reloadConfigToolStripMenuItem";
            resources.ApplyResources(this.reloadConfigToolStripMenuItem, "reloadConfigToolStripMenuItem");
            // 
            // generalTabPage
            // 
            this.generalTabPage.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.generalTabPage.Controls.Add(this.generalPanel);
            resources.ApplyResources(this.generalTabPage, "generalTabPage");
            this.generalTabPage.Name = "generalTabPage";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.generalTabPage);
            this.tabControl1.Controls.Add(this.ArcazeTabPage);
            this.tabControl1.Controls.Add(this.mobiFlightTabPage);
            this.tabControl1.Controls.Add(this.fsuipcTabPage);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // mobiFlightTabPage
            // 
            this.mobiFlightTabPage.Controls.Add(this.mobiFlightPanel);
            resources.ApplyResources(this.mobiFlightTabPage, "mobiFlightTabPage");
            this.mobiFlightTabPage.Name = "mobiFlightTabPage";
            this.mobiFlightTabPage.UseVisualStyleBackColor = true;
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
            // generalPanel
            // 
            resources.ApplyResources(this.generalPanel, "generalPanel");
            this.generalPanel.Name = "generalPanel";
            // 
            // arcazePanel
            // 
            resources.ApplyResources(this.arcazePanel, "arcazePanel");
            this.arcazePanel.ModuleConfigChanged = false;
            this.arcazePanel.Name = "arcazePanel";
            // 
            // mobiFlightPanel
            // 
            resources.ApplyResources(this.mobiFlightPanel, "mobiFlightPanel");
            this.mobiFlightPanel.Name = "mobiFlightPanel";
            // 
            // SettingsDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Name = "SettingsDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Shown += new System.EventHandler(this.SettingsDialog_Shown);
            this.panel1.ResumeLayout(false);
            this.ArcazeTabPage.ResumeLayout(false);
            this.mfModuleSettingsContextMenuStrip.ResumeLayout(false);
            this.generalTabPage.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.mobiFlightTabPage.ResumeLayout(false);
            this.fsuipcTabPage.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fsuipcPollIntervalTrackBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TabPage ArcazeTabPage;
        private System.Windows.Forms.TabPage generalTabPage;
        private System.Windows.Forms.TabPage fsuipcTabPage;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar fsuipcPollIntervalTrackBar;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label5;
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
        private System.Windows.Forms.ImageList mfTreeViewImageList;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem updateFirmwareToolStripMenuItem;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem firmwareSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem firmwareUpdateToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker firmwareUpdateBackgroundWorker;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem buttonToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem encoderToolStripMenuItem;
        public System.Windows.Forms.TabControl tabControl1;
        public System.Windows.Forms.TabPage mobiFlightTabPage;
        private System.Windows.Forms.ToolStripMenuItem regenerateSerialToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LcdDisplayToolStripMenuItem;
        private Panels.Settings.GeneralPanel generalPanel;
        private Panels.Settings.ArcazePanel arcazePanel;
        private Panels.Settings.MobiFlightPanel mobiFlightPanel;
        private System.Windows.Forms.ToolStripMenuItem analogDeviceToolStripMenuItem;
    }
}