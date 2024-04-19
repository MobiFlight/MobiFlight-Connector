﻿namespace MobiFlight.UI.Panels.Settings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GeneralPanel));
            this.BetaUpdatesGroupBox = new System.Windows.Forms.GroupBox();
            this.BetaUpdateCheckBox = new System.Windows.Forms.CheckBox();
            this.languageGroupBox = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.languageComboBox = new System.Windows.Forms.ComboBox();
            this.languageLabel = new System.Windows.Forms.Label();
            this.debugGroupBox = new System.Windows.Forms.GroupBox();
            this.LogJoystickAxisCheckBox = new System.Windows.Forms.CheckBox();
            this.logLevelComboBox = new System.Windows.Forms.ComboBox();
            this.logLevelLabel = new System.Windows.Forms.Label();
            this.logLevelCheckBox = new System.Windows.Forms.CheckBox();
            this.testModeSpeedGroupBox = new System.Windows.Forms.GroupBox();
            this.labelTestModeFast = new System.Windows.Forms.Label();
            this.labelTestModeSlow = new System.Windows.Forms.Label();
            this.testModeSpeedTrackBar = new System.Windows.Forms.TrackBar();
            this.recentFilesGroupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.recentFilesNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.CommunityFeedbackGroupBox = new System.Windows.Forms.GroupBox();
            this.CommunityFeedbackCheckBox = new System.Windows.Forms.CheckBox();
            this.ConfigExecutionGroupBox = new System.Windows.Forms.GroupBox();
            this.labelExplainExecution = new System.Windows.Forms.Label();
            this.labelExecutionFast = new System.Windows.Forms.Label();
            this.labelExecutionSlow = new System.Windows.Forms.Label();
            this.fsuipcPollIntervalTrackBar = new System.Windows.Forms.TrackBar();
            this.SpeedTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.autoRetriggerGoupBox = new System.Windows.Forms.GroupBox();
            this.minimizeOnAutoRunCheckbox = new System.Windows.Forms.CheckBox();
            this.autoRetriggerCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBoxHubHopAutoCheck = new System.Windows.Forms.GroupBox();
            this.checkBoxHubHopAutoCheck = new System.Windows.Forms.CheckBox();
            this.BetaUpdatesGroupBox.SuspendLayout();
            this.languageGroupBox.SuspendLayout();
            this.debugGroupBox.SuspendLayout();
            this.testModeSpeedGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testModeSpeedTrackBar)).BeginInit();
            this.recentFilesGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.recentFilesNumericUpDown)).BeginInit();
            this.CommunityFeedbackGroupBox.SuspendLayout();
            this.ConfigExecutionGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fsuipcPollIntervalTrackBar)).BeginInit();
            this.SpeedTableLayoutPanel.SuspendLayout();
            this.autoRetriggerGoupBox.SuspendLayout();
            this.groupBoxHubHopAutoCheck.SuspendLayout();
            this.SuspendLayout();
            // 
            // BetaUpdatesGroupBox
            // 
            this.BetaUpdatesGroupBox.Controls.Add(this.BetaUpdateCheckBox);
            resources.ApplyResources(this.BetaUpdatesGroupBox, "BetaUpdatesGroupBox");
            this.BetaUpdatesGroupBox.Name = "BetaUpdatesGroupBox";
            this.BetaUpdatesGroupBox.TabStop = false;
            // 
            // BetaUpdateCheckBox
            // 
            resources.ApplyResources(this.BetaUpdateCheckBox, "BetaUpdateCheckBox");
            this.BetaUpdateCheckBox.Name = "BetaUpdateCheckBox";
            this.BetaUpdateCheckBox.UseVisualStyleBackColor = true;
            // 
            // languageGroupBox
            // 
            this.languageGroupBox.Controls.Add(this.label9);
            this.languageGroupBox.Controls.Add(this.languageComboBox);
            this.languageGroupBox.Controls.Add(this.languageLabel);
            resources.ApplyResources(this.languageGroupBox, "languageGroupBox");
            this.languageGroupBox.Name = "languageGroupBox";
            this.languageGroupBox.TabStop = false;
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // languageComboBox
            // 
            this.languageComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.languageComboBox.FormattingEnabled = true;
            this.languageComboBox.Items.AddRange(new object[] {
            resources.GetString("languageComboBox.Items"),
            resources.GetString("languageComboBox.Items1"),
            resources.GetString("languageComboBox.Items2")});
            resources.ApplyResources(this.languageComboBox, "languageComboBox");
            this.languageComboBox.Name = "languageComboBox";
            // 
            // languageLabel
            // 
            resources.ApplyResources(this.languageLabel, "languageLabel");
            this.languageLabel.Name = "languageLabel";
            // 
            // debugGroupBox
            // 
            this.debugGroupBox.Controls.Add(this.LogJoystickAxisCheckBox);
            this.debugGroupBox.Controls.Add(this.logLevelComboBox);
            this.debugGroupBox.Controls.Add(this.logLevelLabel);
            this.debugGroupBox.Controls.Add(this.logLevelCheckBox);
            resources.ApplyResources(this.debugGroupBox, "debugGroupBox");
            this.debugGroupBox.Name = "debugGroupBox";
            this.debugGroupBox.TabStop = false;
            // 
            // LogJoystickAxisCheckBox
            // 
            resources.ApplyResources(this.LogJoystickAxisCheckBox, "LogJoystickAxisCheckBox");
            this.LogJoystickAxisCheckBox.Name = "LogJoystickAxisCheckBox";
            this.LogJoystickAxisCheckBox.UseVisualStyleBackColor = true;
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
            this.testModeSpeedGroupBox.Controls.Add(this.labelTestModeFast);
            this.testModeSpeedGroupBox.Controls.Add(this.labelTestModeSlow);
            this.testModeSpeedGroupBox.Controls.Add(this.testModeSpeedTrackBar);
            resources.ApplyResources(this.testModeSpeedGroupBox, "testModeSpeedGroupBox");
            this.testModeSpeedGroupBox.Name = "testModeSpeedGroupBox";
            this.testModeSpeedGroupBox.TabStop = false;
            // 
            // labelTestModeFast
            // 
            resources.ApplyResources(this.labelTestModeFast, "labelTestModeFast");
            this.labelTestModeFast.Name = "labelTestModeFast";
            // 
            // labelTestModeSlow
            // 
            resources.ApplyResources(this.labelTestModeSlow, "labelTestModeSlow");
            this.labelTestModeSlow.Name = "labelTestModeSlow";
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
            // CommunityFeedbackGroupBox
            // 
            this.CommunityFeedbackGroupBox.Controls.Add(this.CommunityFeedbackCheckBox);
            resources.ApplyResources(this.CommunityFeedbackGroupBox, "CommunityFeedbackGroupBox");
            this.CommunityFeedbackGroupBox.Name = "CommunityFeedbackGroupBox";
            this.CommunityFeedbackGroupBox.TabStop = false;
            // 
            // CommunityFeedbackCheckBox
            // 
            resources.ApplyResources(this.CommunityFeedbackCheckBox, "CommunityFeedbackCheckBox");
            this.CommunityFeedbackCheckBox.Name = "CommunityFeedbackCheckBox";
            this.CommunityFeedbackCheckBox.UseVisualStyleBackColor = true;
            // 
            // ConfigExecutionGroupBox
            // 
            this.ConfigExecutionGroupBox.Controls.Add(this.labelExplainExecution);
            this.ConfigExecutionGroupBox.Controls.Add(this.labelExecutionFast);
            this.ConfigExecutionGroupBox.Controls.Add(this.labelExecutionSlow);
            this.ConfigExecutionGroupBox.Controls.Add(this.fsuipcPollIntervalTrackBar);
            resources.ApplyResources(this.ConfigExecutionGroupBox, "ConfigExecutionGroupBox");
            this.ConfigExecutionGroupBox.Name = "ConfigExecutionGroupBox";
            this.ConfigExecutionGroupBox.TabStop = false;
            // 
            // labelExplainExecution
            // 
            resources.ApplyResources(this.labelExplainExecution, "labelExplainExecution");
            this.labelExplainExecution.Name = "labelExplainExecution";
            // 
            // labelExecutionFast
            // 
            resources.ApplyResources(this.labelExecutionFast, "labelExecutionFast");
            this.labelExecutionFast.Name = "labelExecutionFast";
            // 
            // labelExecutionSlow
            // 
            resources.ApplyResources(this.labelExecutionSlow, "labelExecutionSlow");
            this.labelExecutionSlow.Name = "labelExecutionSlow";
            // 
            // fsuipcPollIntervalTrackBar
            // 
            resources.ApplyResources(this.fsuipcPollIntervalTrackBar, "fsuipcPollIntervalTrackBar");
            this.fsuipcPollIntervalTrackBar.LargeChange = 2;
            this.fsuipcPollIntervalTrackBar.Minimum = 2;
            this.fsuipcPollIntervalTrackBar.Name = "fsuipcPollIntervalTrackBar";
            this.fsuipcPollIntervalTrackBar.Value = 10;
            // 
            // SpeedTableLayoutPanel
            // 
            resources.ApplyResources(this.SpeedTableLayoutPanel, "SpeedTableLayoutPanel");
            this.SpeedTableLayoutPanel.Controls.Add(this.testModeSpeedGroupBox, 1, 0);
            this.SpeedTableLayoutPanel.Controls.Add(this.ConfigExecutionGroupBox, 0, 0);
            this.SpeedTableLayoutPanel.Name = "SpeedTableLayoutPanel";
            // 
            // autoRetriggerGoupBox
            // 
            this.autoRetriggerGoupBox.Controls.Add(this.minimizeOnAutoRunCheckbox);
            this.autoRetriggerGoupBox.Controls.Add(this.autoRetriggerCheckBox);
            resources.ApplyResources(this.autoRetriggerGoupBox, "autoRetriggerGoupBox");
            this.autoRetriggerGoupBox.Name = "autoRetriggerGoupBox";
            this.autoRetriggerGoupBox.TabStop = false;
            // 
            // minimizeOnAutoRunCheckbox
            // 
            resources.ApplyResources(this.minimizeOnAutoRunCheckbox, "minimizeOnAutoRunCheckbox");
            this.minimizeOnAutoRunCheckbox.Name = "minimizeOnAutoRunCheckbox";
            this.minimizeOnAutoRunCheckbox.UseVisualStyleBackColor = true;
            // 
            // autoRetriggerCheckBox
            // 
            resources.ApplyResources(this.autoRetriggerCheckBox, "autoRetriggerCheckBox");
            this.autoRetriggerCheckBox.Name = "autoRetriggerCheckBox";
            this.autoRetriggerCheckBox.UseVisualStyleBackColor = true;
            // 
            // groupBoxHubHopAutoCheck
            // 
            this.groupBoxHubHopAutoCheck.Controls.Add(this.checkBoxHubHopAutoCheck);
            resources.ApplyResources(this.groupBoxHubHopAutoCheck, "groupBoxHubHopAutoCheck");
            this.groupBoxHubHopAutoCheck.Name = "groupBoxHubHopAutoCheck";
            this.groupBoxHubHopAutoCheck.TabStop = false;
            // 
            // checkBoxHubHopAutoCheck
            // 
            resources.ApplyResources(this.checkBoxHubHopAutoCheck, "checkBoxHubHopAutoCheck");
            this.checkBoxHubHopAutoCheck.Name = "checkBoxHubHopAutoCheck";
            this.checkBoxHubHopAutoCheck.UseVisualStyleBackColor = true;
            // 
            // GeneralPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SpeedTableLayoutPanel);
            this.Controls.Add(this.languageGroupBox);
            this.Controls.Add(this.groupBoxHubHopAutoCheck);
            this.Controls.Add(this.autoRetriggerGoupBox);
            this.Controls.Add(this.CommunityFeedbackGroupBox);
            this.Controls.Add(this.BetaUpdatesGroupBox);
            this.Controls.Add(this.debugGroupBox);
            this.Controls.Add(this.recentFilesGroupBox);
            this.Name = "GeneralPanel";
            this.BetaUpdatesGroupBox.ResumeLayout(false);
            this.BetaUpdatesGroupBox.PerformLayout();
            this.languageGroupBox.ResumeLayout(false);
            this.debugGroupBox.ResumeLayout(false);
            this.debugGroupBox.PerformLayout();
            this.testModeSpeedGroupBox.ResumeLayout(false);
            this.testModeSpeedGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testModeSpeedTrackBar)).EndInit();
            this.recentFilesGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.recentFilesNumericUpDown)).EndInit();
            this.CommunityFeedbackGroupBox.ResumeLayout(false);
            this.ConfigExecutionGroupBox.ResumeLayout(false);
            this.ConfigExecutionGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fsuipcPollIntervalTrackBar)).EndInit();
            this.SpeedTableLayoutPanel.ResumeLayout(false);
            this.autoRetriggerGoupBox.ResumeLayout(false);
            this.autoRetriggerGoupBox.PerformLayout();
            this.groupBoxHubHopAutoCheck.ResumeLayout(false);
            this.groupBoxHubHopAutoCheck.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox BetaUpdatesGroupBox;
        private System.Windows.Forms.CheckBox BetaUpdateCheckBox;
        private System.Windows.Forms.GroupBox languageGroupBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox languageComboBox;
        private System.Windows.Forms.Label languageLabel;
        private System.Windows.Forms.GroupBox debugGroupBox;
        private System.Windows.Forms.ComboBox logLevelComboBox;
        private System.Windows.Forms.Label logLevelLabel;
        private System.Windows.Forms.CheckBox logLevelCheckBox;
        private System.Windows.Forms.GroupBox testModeSpeedGroupBox;
        private System.Windows.Forms.TrackBar testModeSpeedTrackBar;
        private System.Windows.Forms.GroupBox recentFilesGroupBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown recentFilesNumericUpDown;
        private System.Windows.Forms.GroupBox CommunityFeedbackGroupBox;
        private System.Windows.Forms.CheckBox CommunityFeedbackCheckBox;
        private System.Windows.Forms.CheckBox LogJoystickAxisCheckBox;
        private System.Windows.Forms.GroupBox ConfigExecutionGroupBox;
        private System.Windows.Forms.TrackBar fsuipcPollIntervalTrackBar;
        private System.Windows.Forms.TableLayoutPanel SpeedTableLayoutPanel;
        private System.Windows.Forms.GroupBox autoRetriggerGoupBox;
        private System.Windows.Forms.CheckBox autoRetriggerCheckBox;
        private System.Windows.Forms.CheckBox minimizeOnAutoRunCheckbox;
        private System.Windows.Forms.GroupBox groupBoxHubHopAutoCheck;
        private System.Windows.Forms.CheckBox checkBoxHubHopAutoCheck;
        private System.Windows.Forms.Label labelTestModeFast;
        private System.Windows.Forms.Label labelTestModeSlow;
        private System.Windows.Forms.Label labelExecutionSlow;
        private System.Windows.Forms.Label labelExecutionFast;
        private System.Windows.Forms.Label labelExplainExecution;
    }
}
