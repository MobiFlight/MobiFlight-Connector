namespace ArcazeUSB
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
            this.label7 = new System.Windows.Forms.Label();
            this.arcazeSerialComboBox = new System.Windows.Forms.ComboBox();
            this.generalTabPage = new System.Windows.Forms.TabPage();
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
            this.mfSettingsPanel = new System.Windows.Forms.Panel();
            this.mfModulesTreeView = new System.Windows.Forms.TreeView();
            this.fsuipcTabPage = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.fsuipcPollIntervalTrackBar = new System.Windows.Forms.TrackBar();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.mfApplyPanel = new System.Windows.Forms.Panel();
            this.mfApplyButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.ledDisplaysTabPage.SuspendLayout();
            this.arcazeModuleSettingsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numModulesNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.globalBrightnessTrackBar)).BeginInit();
            this.arcazeModulesGroupBox.SuspendLayout();
            this.generalTabPage.SuspendLayout();
            this.testModeSpeedGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testModeSpeedTrackBar)).BeginInit();
            this.recentFilesGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.recentFilesNumericUpDown)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.mobiFlightTabPage.SuspendLayout();
            this.mfConfiguredModulesGroupBox.SuspendLayout();
            this.mfSettingsPanel.SuspendLayout();
            this.fsuipcTabPage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fsuipcPollIntervalTrackBar)).BeginInit();
            this.mfApplyPanel.SuspendLayout();
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
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // ledDisplaysTabPage
            // 
            this.ledDisplaysTabPage.Controls.Add(this.arcazeModuleSettingsGroupBox);
            this.ledDisplaysTabPage.Controls.Add(this.arcazeModulesGroupBox);
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
            this.numModulesNumericUpDown.Name = "numModulesNumericUpDown";
            this.toolTip1.SetToolTip(this.numModulesNumericUpDown, resources.GetString("numModulesNumericUpDown.ToolTip"));
            this.numModulesNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
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
            // 
            // arcazeModuleTypeLabel
            // 
            resources.ApplyResources(this.arcazeModuleTypeLabel, "arcazeModuleTypeLabel");
            this.arcazeModuleTypeLabel.Name = "arcazeModuleTypeLabel";
            // 
            // arcazeModulesGroupBox
            // 
            this.arcazeModulesGroupBox.Controls.Add(this.label7);
            this.arcazeModulesGroupBox.Controls.Add(this.arcazeSerialComboBox);
            resources.ApplyResources(this.arcazeModulesGroupBox, "arcazeModulesGroupBox");
            this.arcazeModulesGroupBox.Name = "arcazeModulesGroupBox";
            this.arcazeModulesGroupBox.TabStop = false;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // arcazeSerialComboBox
            // 
            this.arcazeSerialComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.arcazeSerialComboBox.FormattingEnabled = true;
            this.arcazeSerialComboBox.Items.AddRange(new object[] {
            resources.GetString("arcazeSerialComboBox.Items")});
            resources.ApplyResources(this.arcazeSerialComboBox, "arcazeSerialComboBox");
            this.arcazeSerialComboBox.Name = "arcazeSerialComboBox";
            this.arcazeSerialComboBox.SelectedIndexChanged += new System.EventHandler(this.arcazeSerialComboBox_SelectedIndexChanged);
            // 
            // generalTabPage
            // 
            this.generalTabPage.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.generalTabPage.Controls.Add(this.testModeSpeedGroupBox);
            this.generalTabPage.Controls.Add(this.recentFilesGroupBox);
            resources.ApplyResources(this.generalTabPage, "generalTabPage");
            this.generalTabPage.Name = "generalTabPage";
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
            resources.ApplyResources(this.mobiFlightTabPage, "mobiFlightTabPage");
            this.mobiFlightTabPage.Name = "mobiFlightTabPage";
            this.mobiFlightTabPage.UseVisualStyleBackColor = true;
            // 
            // mfConfiguredModulesGroupBox
            // 
            this.mfConfiguredModulesGroupBox.Controls.Add(this.mfSettingsPanel);
            this.mfConfiguredModulesGroupBox.Controls.Add(this.mfModulesTreeView);
            resources.ApplyResources(this.mfConfiguredModulesGroupBox, "mfConfiguredModulesGroupBox");
            this.mfConfiguredModulesGroupBox.Name = "mfConfiguredModulesGroupBox";
            this.mfConfiguredModulesGroupBox.TabStop = false;
            // 
            // mfSettingsPanel
            // 
            this.mfSettingsPanel.Controls.Add(this.mfApplyPanel);
            resources.ApplyResources(this.mfSettingsPanel, "mfSettingsPanel");
            this.mfSettingsPanel.Name = "mfSettingsPanel";
            // 
            // mfModulesTreeView
            // 
            resources.ApplyResources(this.mfModulesTreeView, "mfModulesTreeView");
            this.mfModulesTreeView.Name = "mfModulesTreeView";
            this.mfModulesTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("mfModulesTreeView.Nodes"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("mfModulesTreeView.Nodes1")))});
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
            // toolTip1
            // 
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip1.ToolTipTitle = "Hint";
            // 
            // mfApplyPanel
            // 
            this.mfApplyPanel.Controls.Add(this.mfApplyButton);
            resources.ApplyResources(this.mfApplyPanel, "mfApplyPanel");
            this.mfApplyPanel.Name = "mfApplyPanel";
            // 
            // mfApplyButton
            // 
            resources.ApplyResources(this.mfApplyButton, "mfApplyButton");
            this.mfApplyButton.Name = "mfApplyButton";
            this.mfApplyButton.UseVisualStyleBackColor = true;
            // 
            // SettingsDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Name = "SettingsDialog";
            this.panel1.ResumeLayout(false);
            this.ledDisplaysTabPage.ResumeLayout(false);
            this.arcazeModuleSettingsGroupBox.ResumeLayout(false);
            this.arcazeModuleSettingsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numModulesNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.globalBrightnessTrackBar)).EndInit();
            this.arcazeModulesGroupBox.ResumeLayout(false);
            this.arcazeModulesGroupBox.PerformLayout();
            this.generalTabPage.ResumeLayout(false);
            this.testModeSpeedGroupBox.ResumeLayout(false);
            this.testModeSpeedGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testModeSpeedTrackBar)).EndInit();
            this.recentFilesGroupBox.ResumeLayout(false);
            this.recentFilesGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.recentFilesNumericUpDown)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.mobiFlightTabPage.ResumeLayout(false);
            this.mfConfiguredModulesGroupBox.ResumeLayout(false);
            this.mfSettingsPanel.ResumeLayout(false);
            this.fsuipcTabPage.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fsuipcPollIntervalTrackBar)).EndInit();
            this.mfApplyPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TabPage ledDisplaysTabPage;
        private System.Windows.Forms.GroupBox arcazeModulesGroupBox;
        private System.Windows.Forms.ComboBox arcazeSerialComboBox;
        private System.Windows.Forms.TabPage generalTabPage;
        private System.Windows.Forms.GroupBox recentFilesGroupBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown recentFilesNumericUpDown;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage fsuipcTabPage;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar fsuipcPollIntervalTrackBar;
        private System.Windows.Forms.Label label7;
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
        private System.Windows.Forms.TabPage mobiFlightTabPage;
        private System.Windows.Forms.GroupBox mfConfiguredModulesGroupBox;
        private System.Windows.Forms.Panel mfSettingsPanel;
        private System.Windows.Forms.TreeView mfModulesTreeView;
        private System.Windows.Forms.Panel mfApplyPanel;
        private System.Windows.Forms.Button mfApplyButton;
    }
}