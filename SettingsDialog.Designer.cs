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
            this.fsuipcTabPage = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.fsuipcPollIntervalTrackBar = new System.Windows.Forms.TrackBar();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
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
            this.fsuipcTabPage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fsuipcPollIntervalTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.okButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Name = "panel1";
            this.toolTip1.SetToolTip(this.panel1, global::ArcazeUSB.ProjectMessages.conf);
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.ImageKey = global::ArcazeUSB.ProjectMessages.conf;
            this.okButton.Name = "okButton";
            this.toolTip1.SetToolTip(this.okButton, global::ArcazeUSB.ProjectMessages.conf);
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.ImageKey = global::ArcazeUSB.ProjectMessages.conf;
            this.cancelButton.Name = "cancelButton";
            this.toolTip1.SetToolTip(this.cancelButton, global::ArcazeUSB.ProjectMessages.conf);
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // ledDisplaysTabPage
            // 
            resources.ApplyResources(this.ledDisplaysTabPage, "ledDisplaysTabPage");
            this.ledDisplaysTabPage.Controls.Add(this.arcazeModuleSettingsGroupBox);
            this.ledDisplaysTabPage.Controls.Add(this.arcazeModulesGroupBox);
            this.ledDisplaysTabPage.ImageKey = global::ArcazeUSB.ProjectMessages.conf;
            this.ledDisplaysTabPage.Name = "ledDisplaysTabPage";
            this.toolTip1.SetToolTip(this.ledDisplaysTabPage, global::ArcazeUSB.ProjectMessages.conf);
            this.ledDisplaysTabPage.UseVisualStyleBackColor = true;
            this.ledDisplaysTabPage.Validating += new System.ComponentModel.CancelEventHandler(this.ledDisplaysTabPage_Validating);
            // 
            // arcazeModuleSettingsGroupBox
            // 
            resources.ApplyResources(this.arcazeModuleSettingsGroupBox, "arcazeModuleSettingsGroupBox");
            this.arcazeModuleSettingsGroupBox.Controls.Add(this.numModulesLabel);
            this.arcazeModuleSettingsGroupBox.Controls.Add(this.numModulesNumericUpDown);
            this.arcazeModuleSettingsGroupBox.Controls.Add(this.arcazeModuleTypeComboBox);
            this.arcazeModuleSettingsGroupBox.Controls.Add(this.globalBrightnessLabel);
            this.arcazeModuleSettingsGroupBox.Controls.Add(this.globalBrightnessTrackBar);
            this.arcazeModuleSettingsGroupBox.Controls.Add(this.arcazeModuleTypeLabel);
            this.arcazeModuleSettingsGroupBox.Name = "arcazeModuleSettingsGroupBox";
            this.arcazeModuleSettingsGroupBox.TabStop = false;
            this.toolTip1.SetToolTip(this.arcazeModuleSettingsGroupBox, global::ArcazeUSB.ProjectMessages.conf);
            // 
            // numModulesLabel
            // 
            resources.ApplyResources(this.numModulesLabel, "numModulesLabel");
            this.numModulesLabel.ImageKey = global::ArcazeUSB.ProjectMessages.conf;
            this.numModulesLabel.Name = "numModulesLabel";
            this.toolTip1.SetToolTip(this.numModulesLabel, global::ArcazeUSB.ProjectMessages.conf);
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
            resources.ApplyResources(this.arcazeModuleTypeComboBox, "arcazeModuleTypeComboBox");
            this.arcazeModuleTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.arcazeModuleTypeComboBox.FormattingEnabled = true;
            this.arcazeModuleTypeComboBox.Items.AddRange(new object[] {
            resources.GetString("arcazeModuleTypeComboBox.Items")});
            this.arcazeModuleTypeComboBox.Name = "arcazeModuleTypeComboBox";
            this.toolTip1.SetToolTip(this.arcazeModuleTypeComboBox, global::ArcazeUSB.ProjectMessages.conf);
            this.arcazeModuleTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.arcazeModuleTypeComboBox_SelectedIndexChanged);
            // 
            // globalBrightnessLabel
            // 
            resources.ApplyResources(this.globalBrightnessLabel, "globalBrightnessLabel");
            this.globalBrightnessLabel.ImageKey = global::ArcazeUSB.ProjectMessages.conf;
            this.globalBrightnessLabel.Name = "globalBrightnessLabel";
            this.toolTip1.SetToolTip(this.globalBrightnessLabel, global::ArcazeUSB.ProjectMessages.conf);
            // 
            // globalBrightnessTrackBar
            // 
            resources.ApplyResources(this.globalBrightnessTrackBar, "globalBrightnessTrackBar");
            this.globalBrightnessTrackBar.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.globalBrightnessTrackBar.Maximum = 9;
            this.globalBrightnessTrackBar.Minimum = 1;
            this.globalBrightnessTrackBar.Name = "globalBrightnessTrackBar";
            this.toolTip1.SetToolTip(this.globalBrightnessTrackBar, resources.GetString("globalBrightnessTrackBar.ToolTip"));
            this.globalBrightnessTrackBar.Value = 9;
            // 
            // arcazeModuleTypeLabel
            // 
            resources.ApplyResources(this.arcazeModuleTypeLabel, "arcazeModuleTypeLabel");
            this.arcazeModuleTypeLabel.ImageKey = global::ArcazeUSB.ProjectMessages.conf;
            this.arcazeModuleTypeLabel.Name = "arcazeModuleTypeLabel";
            this.toolTip1.SetToolTip(this.arcazeModuleTypeLabel, global::ArcazeUSB.ProjectMessages.conf);
            // 
            // arcazeModulesGroupBox
            // 
            resources.ApplyResources(this.arcazeModulesGroupBox, "arcazeModulesGroupBox");
            this.arcazeModulesGroupBox.Controls.Add(this.label7);
            this.arcazeModulesGroupBox.Controls.Add(this.arcazeSerialComboBox);
            this.arcazeModulesGroupBox.Name = "arcazeModulesGroupBox";
            this.arcazeModulesGroupBox.TabStop = false;
            this.toolTip1.SetToolTip(this.arcazeModulesGroupBox, global::ArcazeUSB.ProjectMessages.conf);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.ImageKey = global::ArcazeUSB.ProjectMessages.conf;
            this.label7.Name = "label7";
            this.toolTip1.SetToolTip(this.label7, global::ArcazeUSB.ProjectMessages.conf);
            // 
            // arcazeSerialComboBox
            // 
            resources.ApplyResources(this.arcazeSerialComboBox, "arcazeSerialComboBox");
            this.arcazeSerialComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.arcazeSerialComboBox.FormattingEnabled = true;
            this.arcazeSerialComboBox.Items.AddRange(new object[] {
            resources.GetString("arcazeSerialComboBox.Items")});
            this.arcazeSerialComboBox.Name = "arcazeSerialComboBox";
            this.toolTip1.SetToolTip(this.arcazeSerialComboBox, global::ArcazeUSB.ProjectMessages.conf);
            this.arcazeSerialComboBox.SelectedIndexChanged += new System.EventHandler(this.arcazeSerialComboBox_SelectedIndexChanged);
            // 
            // generalTabPage
            // 
            resources.ApplyResources(this.generalTabPage, "generalTabPage");
            this.generalTabPage.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.generalTabPage.Controls.Add(this.testModeSpeedGroupBox);
            this.generalTabPage.Controls.Add(this.recentFilesGroupBox);
            this.generalTabPage.ImageKey = global::ArcazeUSB.ProjectMessages.conf;
            this.generalTabPage.Name = "generalTabPage";
            this.toolTip1.SetToolTip(this.generalTabPage, global::ArcazeUSB.ProjectMessages.conf);
            this.generalTabPage.ToolTipText = global::ArcazeUSB.ProjectMessages.conf;
            // 
            // testModeSpeedGroupBox
            // 
            resources.ApplyResources(this.testModeSpeedGroupBox, "testModeSpeedGroupBox");
            this.testModeSpeedGroupBox.Controls.Add(this.label8);
            this.testModeSpeedGroupBox.Controls.Add(this.label6);
            this.testModeSpeedGroupBox.Controls.Add(this.testModeSpeedTrackBar);
            this.testModeSpeedGroupBox.Name = "testModeSpeedGroupBox";
            this.testModeSpeedGroupBox.TabStop = false;
            this.toolTip1.SetToolTip(this.testModeSpeedGroupBox, global::ArcazeUSB.ProjectMessages.conf);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.ImageKey = global::ArcazeUSB.ProjectMessages.conf;
            this.label8.Name = "label8";
            this.toolTip1.SetToolTip(this.label8, global::ArcazeUSB.ProjectMessages.conf);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.ImageKey = global::ArcazeUSB.ProjectMessages.conf;
            this.label6.Name = "label6";
            this.toolTip1.SetToolTip(this.label6, global::ArcazeUSB.ProjectMessages.conf);
            // 
            // testModeSpeedTrackBar
            // 
            resources.ApplyResources(this.testModeSpeedTrackBar, "testModeSpeedTrackBar");
            this.testModeSpeedTrackBar.Maximum = 4;
            this.testModeSpeedTrackBar.Name = "testModeSpeedTrackBar";
            this.toolTip1.SetToolTip(this.testModeSpeedTrackBar, global::ArcazeUSB.ProjectMessages.conf);
            // 
            // recentFilesGroupBox
            // 
            resources.ApplyResources(this.recentFilesGroupBox, "recentFilesGroupBox");
            this.recentFilesGroupBox.Controls.Add(this.label1);
            this.recentFilesGroupBox.Controls.Add(this.recentFilesNumericUpDown);
            this.recentFilesGroupBox.Name = "recentFilesGroupBox";
            this.recentFilesGroupBox.TabStop = false;
            this.toolTip1.SetToolTip(this.recentFilesGroupBox, global::ArcazeUSB.ProjectMessages.conf);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.AutoEllipsis = true;
            this.label1.ImageKey = global::ArcazeUSB.ProjectMessages.conf;
            this.label1.Name = "label1";
            this.toolTip1.SetToolTip(this.label1, global::ArcazeUSB.ProjectMessages.conf);
            // 
            // recentFilesNumericUpDown
            // 
            resources.ApplyResources(this.recentFilesNumericUpDown, "recentFilesNumericUpDown");
            this.recentFilesNumericUpDown.Name = "recentFilesNumericUpDown";
            this.toolTip1.SetToolTip(this.recentFilesNumericUpDown, global::ArcazeUSB.ProjectMessages.conf);
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.generalTabPage);
            this.tabControl1.Controls.Add(this.ledDisplaysTabPage);
            this.tabControl1.Controls.Add(this.fsuipcTabPage);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.toolTip1.SetToolTip(this.tabControl1, global::ArcazeUSB.ProjectMessages.conf);
            // 
            // fsuipcTabPage
            // 
            resources.ApplyResources(this.fsuipcTabPage, "fsuipcTabPage");
            this.fsuipcTabPage.Controls.Add(this.groupBox1);
            this.fsuipcTabPage.ImageKey = global::ArcazeUSB.ProjectMessages.conf;
            this.fsuipcTabPage.Name = "fsuipcTabPage";
            this.toolTip1.SetToolTip(this.fsuipcTabPage, global::ArcazeUSB.ProjectMessages.conf);
            this.fsuipcTabPage.ToolTipText = global::ArcazeUSB.ProjectMessages.conf;
            this.fsuipcTabPage.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.fsuipcPollIntervalTrackBar);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox1, global::ArcazeUSB.ProjectMessages.conf);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.ImageKey = global::ArcazeUSB.ProjectMessages.conf;
            this.label4.Name = "label4";
            this.toolTip1.SetToolTip(this.label4, global::ArcazeUSB.ProjectMessages.conf);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.ImageKey = global::ArcazeUSB.ProjectMessages.conf;
            this.label3.Name = "label3";
            this.toolTip1.SetToolTip(this.label3, global::ArcazeUSB.ProjectMessages.conf);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ImageKey = global::ArcazeUSB.ProjectMessages.conf;
            this.label2.Name = "label2";
            this.toolTip1.SetToolTip(this.label2, global::ArcazeUSB.ProjectMessages.conf);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.ImageKey = global::ArcazeUSB.ProjectMessages.conf;
            this.label5.Name = "label5";
            this.toolTip1.SetToolTip(this.label5, global::ArcazeUSB.ProjectMessages.conf);
            // 
            // fsuipcPollIntervalTrackBar
            // 
            resources.ApplyResources(this.fsuipcPollIntervalTrackBar, "fsuipcPollIntervalTrackBar");
            this.fsuipcPollIntervalTrackBar.BackColor = System.Drawing.SystemColors.Window;
            this.fsuipcPollIntervalTrackBar.LargeChange = 2;
            this.fsuipcPollIntervalTrackBar.Minimum = 2;
            this.fsuipcPollIntervalTrackBar.Name = "fsuipcPollIntervalTrackBar";
            this.toolTip1.SetToolTip(this.fsuipcPollIntervalTrackBar, global::ArcazeUSB.ProjectMessages.conf);
            this.fsuipcPollIntervalTrackBar.Value = 10;
            // 
            // toolTip1
            // 
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip1.ToolTipTitle = "Hint";
            // 
            // SettingsDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Name = "SettingsDialog";
            this.toolTip1.SetToolTip(this, global::ArcazeUSB.ProjectMessages.conf);
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
            ((System.ComponentModel.ISupportInitialize)(this.recentFilesNumericUpDown)).EndInit();
            this.tabControl1.ResumeLayout(false);
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
    }
}