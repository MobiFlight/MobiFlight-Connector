namespace MobiFlight.UI.Panels.Settings
{
    partial class GeneralPanel
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
            this.BetaUpdatesGroupBox = new System.Windows.Forms.GroupBox();
            this.BetaUpdateCheckBox = new System.Windows.Forms.CheckBox();
            this.languageGroupBox = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.languageComboBox = new System.Windows.Forms.ComboBox();
            this.languageLabel = new System.Windows.Forms.Label();
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
            this.BetaUpdatesGroupBox.SuspendLayout();
            this.languageGroupBox.SuspendLayout();
            this.offlineModeGroupBox.SuspendLayout();
            this.debugGroupBox.SuspendLayout();
            this.testModeSpeedGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testModeSpeedTrackBar)).BeginInit();
            this.recentFilesGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.recentFilesNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // BetaUpdatesGroupBox
            // 
            this.BetaUpdatesGroupBox.Controls.Add(this.BetaUpdateCheckBox);
            this.BetaUpdatesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BetaUpdatesGroupBox.Location = new System.Drawing.Point(0, 335);
            this.BetaUpdatesGroupBox.Name = "BetaUpdatesGroupBox";
            this.BetaUpdatesGroupBox.Size = new System.Drawing.Size(485, 136);
            this.BetaUpdatesGroupBox.TabIndex = 15;
            this.BetaUpdatesGroupBox.TabStop = false;
            this.BetaUpdatesGroupBox.Text = "Beta Versions";
            // 
            // BetaUpdateCheckBox
            // 
            this.BetaUpdateCheckBox.AutoSize = true;
            this.BetaUpdateCheckBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BetaUpdateCheckBox.Location = new System.Drawing.Point(12, 17);
            this.BetaUpdateCheckBox.Name = "BetaUpdateCheckBox";
            this.BetaUpdateCheckBox.Size = new System.Drawing.Size(234, 17);
            this.BetaUpdateCheckBox.TabIndex = 0;
            this.BetaUpdateCheckBox.Text = "I would like to receive beta version updates.";
            this.BetaUpdateCheckBox.UseVisualStyleBackColor = true;
            // 
            // languageGroupBox
            // 
            this.languageGroupBox.Controls.Add(this.label9);
            this.languageGroupBox.Controls.Add(this.languageComboBox);
            this.languageGroupBox.Controls.Add(this.languageLabel);
            this.languageGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.languageGroupBox.Location = new System.Drawing.Point(0, 261);
            this.languageGroupBox.Name = "languageGroupBox";
            this.languageGroupBox.Size = new System.Drawing.Size(485, 74);
            this.languageGroupBox.TabIndex = 14;
            this.languageGroupBox.TabStop = false;
            this.languageGroupBox.Text = "Language";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label9.Location = new System.Drawing.Point(21, 49);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(292, 13);
            this.label9.TabIndex = 2;
            this.label9.Text = "It requires a restart of MobiFlight if you change the language.";
            // 
            // languageComboBox
            // 
            this.languageComboBox.FormattingEnabled = true;
            this.languageComboBox.Items.AddRange(new object[] {
            "System Default",
            "English",
            "Deutsch"});
            this.languageComboBox.Location = new System.Drawing.Point(189, 19);
            this.languageComboBox.Name = "languageComboBox";
            this.languageComboBox.Size = new System.Drawing.Size(121, 21);
            this.languageComboBox.TabIndex = 1;
            // 
            // languageLabel
            // 
            this.languageLabel.AutoSize = true;
            this.languageLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.languageLabel.Location = new System.Drawing.Point(21, 22);
            this.languageLabel.Name = "languageLabel";
            this.languageLabel.Size = new System.Drawing.Size(156, 13);
            this.languageLabel.TabIndex = 0;
            this.languageLabel.Text = "Change the language of the UI:";
            // 
            // offlineModeGroupBox
            // 
            this.offlineModeGroupBox.Controls.Add(this.label7);
            this.offlineModeGroupBox.Controls.Add(this.offlineModeCheckBox);
            this.offlineModeGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.offlineModeGroupBox.Location = new System.Drawing.Point(0, 170);
            this.offlineModeGroupBox.Name = "offlineModeGroupBox";
            this.offlineModeGroupBox.Size = new System.Drawing.Size(485, 91);
            this.offlineModeGroupBox.TabIndex = 13;
            this.offlineModeGroupBox.TabStop = false;
            this.offlineModeGroupBox.Text = "Offline Mode";
            // 
            // label7
            // 
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(103, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(275, 55);
            this.label7.TabIndex = 2;
            this.label7.Text = "If enabled, configs are executed without connection to the Flight Sim. This is us" +
    "eful for Keyboard Commands and Flight Sims that are not explicitly supported by " +
    "MobiFlight.";
            // 
            // offlineModeCheckBox
            // 
            this.offlineModeCheckBox.AutoSize = true;
            this.offlineModeCheckBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.offlineModeCheckBox.Location = new System.Drawing.Point(12, 19);
            this.offlineModeCheckBox.Name = "offlineModeCheckBox";
            this.offlineModeCheckBox.Size = new System.Drawing.Size(64, 17);
            this.offlineModeCheckBox.TabIndex = 1;
            this.offlineModeCheckBox.Text = "enabled";
            this.offlineModeCheckBox.UseVisualStyleBackColor = true;
            // 
            // debugGroupBox
            // 
            this.debugGroupBox.Controls.Add(this.logLevelComboBox);
            this.debugGroupBox.Controls.Add(this.logLevelLabel);
            this.debugGroupBox.Controls.Add(this.logLevelCheckBox);
            this.debugGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.debugGroupBox.Location = new System.Drawing.Point(0, 120);
            this.debugGroupBox.Name = "debugGroupBox";
            this.debugGroupBox.Size = new System.Drawing.Size(485, 50);
            this.debugGroupBox.TabIndex = 12;
            this.debugGroupBox.TabStop = false;
            this.debugGroupBox.Text = "Logging";
            // 
            // logLevelComboBox
            // 
            this.logLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.logLevelComboBox.FormattingEnabled = true;
            this.logLevelComboBox.Items.AddRange(new object[] {
            "Debug",
            "Info",
            "Warn",
            "Error"});
            this.logLevelComboBox.Location = new System.Drawing.Point(222, 18);
            this.logLevelComboBox.Name = "logLevelComboBox";
            this.logLevelComboBox.Size = new System.Drawing.Size(58, 21);
            this.logLevelComboBox.TabIndex = 2;
            // 
            // logLevelLabel
            // 
            this.logLevelLabel.AutoSize = true;
            this.logLevelLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.logLevelLabel.Location = new System.Drawing.Point(162, 21);
            this.logLevelLabel.Name = "logLevelLabel";
            this.logLevelLabel.Size = new System.Drawing.Size(54, 13);
            this.logLevelLabel.TabIndex = 1;
            this.logLevelLabel.Text = "Log Level";
            // 
            // logLevelCheckBox
            // 
            this.logLevelCheckBox.AutoSize = true;
            this.logLevelCheckBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.logLevelCheckBox.Location = new System.Drawing.Point(12, 20);
            this.logLevelCheckBox.Name = "logLevelCheckBox";
            this.logLevelCheckBox.Size = new System.Drawing.Size(64, 17);
            this.logLevelCheckBox.TabIndex = 0;
            this.logLevelCheckBox.Text = "enabled";
            this.logLevelCheckBox.UseVisualStyleBackColor = true;
            // 
            // testModeSpeedGroupBox
            // 
            this.testModeSpeedGroupBox.Controls.Add(this.label8);
            this.testModeSpeedGroupBox.Controls.Add(this.label6);
            this.testModeSpeedGroupBox.Controls.Add(this.testModeSpeedTrackBar);
            this.testModeSpeedGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.testModeSpeedGroupBox.Location = new System.Drawing.Point(0, 46);
            this.testModeSpeedGroupBox.Name = "testModeSpeedGroupBox";
            this.testModeSpeedGroupBox.Size = new System.Drawing.Size(485, 74);
            this.testModeSpeedGroupBox.TabIndex = 11;
            this.testModeSpeedGroupBox.TabStop = false;
            this.testModeSpeedGroupBox.Text = "Test mode speed";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(428, 48);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Very Fast";
            // 
            // label6
            // 
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(6, 48);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Slow";
            // 
            // testModeSpeedTrackBar
            // 
            this.testModeSpeedTrackBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.testModeSpeedTrackBar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.testModeSpeedTrackBar.Location = new System.Drawing.Point(3, 16);
            this.testModeSpeedTrackBar.Maximum = 4;
            this.testModeSpeedTrackBar.Name = "testModeSpeedTrackBar";
            this.testModeSpeedTrackBar.Size = new System.Drawing.Size(479, 45);
            this.testModeSpeedTrackBar.TabIndex = 0;
            // 
            // recentFilesGroupBox
            // 
            this.recentFilesGroupBox.Controls.Add(this.label1);
            this.recentFilesGroupBox.Controls.Add(this.recentFilesNumericUpDown);
            this.recentFilesGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.recentFilesGroupBox.Location = new System.Drawing.Point(0, 0);
            this.recentFilesGroupBox.Name = "recentFilesGroupBox";
            this.recentFilesGroupBox.Size = new System.Drawing.Size(485, 46);
            this.recentFilesGroupBox.TabIndex = 10;
            this.recentFilesGroupBox.TabStop = false;
            this.recentFilesGroupBox.Text = "Recent files";
            // 
            // label1
            // 
            this.label1.AutoEllipsis = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(9, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(227, 18);
            this.label1.TabIndex = 3;
            this.label1.Text = "Set the number of files shown in the list to";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // recentFilesNumericUpDown
            // 
            this.recentFilesNumericUpDown.Location = new System.Drawing.Point(242, 19);
            this.recentFilesNumericUpDown.Name = "recentFilesNumericUpDown";
            this.recentFilesNumericUpDown.Size = new System.Drawing.Size(38, 20);
            this.recentFilesNumericUpDown.TabIndex = 2;
            // 
            // GeneralPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.BetaUpdatesGroupBox);
            this.Controls.Add(this.languageGroupBox);
            this.Controls.Add(this.offlineModeGroupBox);
            this.Controls.Add(this.debugGroupBox);
            this.Controls.Add(this.testModeSpeedGroupBox);
            this.Controls.Add(this.recentFilesGroupBox);
            this.Name = "GeneralPanel";
            this.Size = new System.Drawing.Size(485, 471);
            this.BetaUpdatesGroupBox.ResumeLayout(false);
            this.BetaUpdatesGroupBox.PerformLayout();
            this.languageGroupBox.ResumeLayout(false);
            this.languageGroupBox.PerformLayout();
            this.offlineModeGroupBox.ResumeLayout(false);
            this.offlineModeGroupBox.PerformLayout();
            this.debugGroupBox.ResumeLayout(false);
            this.debugGroupBox.PerformLayout();
            this.testModeSpeedGroupBox.ResumeLayout(false);
            this.testModeSpeedGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testModeSpeedTrackBar)).EndInit();
            this.recentFilesGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.recentFilesNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox BetaUpdatesGroupBox;
        private System.Windows.Forms.CheckBox BetaUpdateCheckBox;
        private System.Windows.Forms.GroupBox languageGroupBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox languageComboBox;
        private System.Windows.Forms.Label languageLabel;
        private System.Windows.Forms.GroupBox offlineModeGroupBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox offlineModeCheckBox;
        private System.Windows.Forms.GroupBox debugGroupBox;
        private System.Windows.Forms.ComboBox logLevelComboBox;
        private System.Windows.Forms.Label logLevelLabel;
        private System.Windows.Forms.CheckBox logLevelCheckBox;
        private System.Windows.Forms.GroupBox testModeSpeedGroupBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TrackBar testModeSpeedTrackBar;
        private System.Windows.Forms.GroupBox recentFilesGroupBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown recentFilesNumericUpDown;
    }
}
