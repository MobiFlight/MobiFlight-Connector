﻿namespace MobiFlight.UI.Panels.Config
{
    partial class PreconditionPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreconditionPanel));
            this.preconditionListgroupBox = new System.Windows.Forms.GroupBox();
            this.preconditionListTreeView = new System.Windows.Forms.TreeView();
            this.preconditionTabTextBox = new System.Windows.Forms.TextBox();
            this.preconditionSelectGroupBox = new System.Windows.Forms.GroupBox();
            this.preConditionTypeComboBox = new System.Windows.Forms.ComboBox();
            this.preconditionTypeLabel = new System.Windows.Forms.Label();
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
            this.preconditionTreeImageList = new System.Windows.Forms.ImageList(this.components);
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
            this.preconditionListgroupBox.SuspendLayout();
            this.preconditionSelectGroupBox.SuspendLayout();
            this.overrideGroupBox.SuspendLayout();
            this.preconditionSettingsPanel.SuspendLayout();
            this.preconditionSettingsGroupBox.SuspendLayout();
            this.preconditionPinPanel.SuspendLayout();
            this.preconditionRuleConfigPanel.SuspendLayout();
            this.preconditionTreeContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // preconditionListgroupBox
            // 
            resources.ApplyResources(this.preconditionListgroupBox, "preconditionListgroupBox");
            this.preconditionListgroupBox.Controls.Add(this.preconditionListTreeView);
            this.preconditionListgroupBox.Name = "preconditionListgroupBox";
            this.preconditionListgroupBox.TabStop = false;
            // 
            // preconditionListTreeView
            // 
            resources.ApplyResources(this.preconditionListTreeView, "preconditionListTreeView");
            this.preconditionListTreeView.CheckBoxes = true;
            this.preconditionListTreeView.ContextMenuStrip = this.preconditionTreeContextMenuStrip;
            this.preconditionListTreeView.ImageList = this.preconditionTreeImageList;
            this.preconditionListTreeView.Name = "preconditionListTreeView";
            this.preconditionListTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("preconditionListTreeView.Nodes"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("preconditionListTreeView.Nodes1")))});
            this.preconditionListTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.preconditionListTreeView_NodeMouseClick);
            // 
            // preconditionTabTextBox
            // 
            resources.ApplyResources(this.preconditionTabTextBox, "preconditionTabTextBox");
            this.preconditionTabTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.preconditionTabTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.preconditionTabTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.preconditionTabTextBox.Name = "preconditionTabTextBox";
            this.preconditionTabTextBox.ReadOnly = true;
            this.preconditionTabTextBox.TabStop = false;
            // 
            // preconditionSelectGroupBox
            // 
            resources.ApplyResources(this.preconditionSelectGroupBox, "preconditionSelectGroupBox");
            this.preconditionSelectGroupBox.Controls.Add(this.preConditionTypeComboBox);
            this.preconditionSelectGroupBox.Controls.Add(this.preconditionTypeLabel);
            this.preconditionSelectGroupBox.Name = "preconditionSelectGroupBox";
            this.preconditionSelectGroupBox.TabStop = false;
            // 
            // preConditionTypeComboBox
            // 
            resources.ApplyResources(this.preConditionTypeComboBox, "preConditionTypeComboBox");
            this.preConditionTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.preConditionTypeComboBox.FormattingEnabled = true;
            this.preConditionTypeComboBox.Items.AddRange(new object[] {
            resources.GetString("preConditionTypeComboBox.Items"),
            resources.GetString("preConditionTypeComboBox.Items1"),
            resources.GetString("preConditionTypeComboBox.Items2")});
            this.preConditionTypeComboBox.Name = "preConditionTypeComboBox";
            this.preConditionTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.preConditionTypeComboBox_SelectedIndexChanged);
            // 
            // preconditionTypeLabel
            // 
            resources.ApplyResources(this.preconditionTypeLabel, "preconditionTypeLabel");
            this.preconditionTypeLabel.Name = "preconditionTypeLabel";
            // 
            // preconditionSpacerPanel
            // 
            resources.ApplyResources(this.preconditionSpacerPanel, "preconditionSpacerPanel");
            this.preconditionSpacerPanel.Name = "preconditionSpacerPanel";
            // 
            // overrideGroupBox
            // 
            resources.ApplyResources(this.overrideGroupBox, "overrideGroupBox");
            this.overrideGroupBox.Controls.Add(this.overridePreconditionTextBox);
            this.overrideGroupBox.Controls.Add(this.overridePreconditionCheckBox);
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
            resources.ApplyResources(this.preconditionSettingsPanel, "preconditionSettingsPanel");
            this.preconditionSettingsPanel.Controls.Add(this.preconditionSettingsGroupBox);
            this.preconditionSettingsPanel.Controls.Add(this.preconditionApplyButton);
            this.preconditionSettingsPanel.Controls.Add(this.preconditionSelectGroupBox);
            this.preconditionSettingsPanel.Name = "preconditionSettingsPanel";
            // 
            // preconditionSettingsGroupBox
            // 
            resources.ApplyResources(this.preconditionSettingsGroupBox, "preconditionSettingsGroupBox");
            this.preconditionSettingsGroupBox.Controls.Add(this.preconditionPinPanel);
            this.preconditionSettingsGroupBox.Controls.Add(this.preconditionRuleConfigPanel);
            this.preconditionSettingsGroupBox.Name = "preconditionSettingsGroupBox";
            this.preconditionSettingsGroupBox.TabStop = false;
            // 
            // preconditionPinPanel
            // 
            resources.ApplyResources(this.preconditionPinPanel, "preconditionPinPanel");
            this.preconditionPinPanel.Controls.Add(this.preconditionPinValueComboBox);
            this.preconditionPinPanel.Controls.Add(this.preconditionPinValueLabel);
            this.preconditionPinPanel.Controls.Add(this.preconditionPinComboBox);
            this.preconditionPinPanel.Controls.Add(this.preconditionPortComboBox);
            this.preconditionPinPanel.Controls.Add(this.preconditonPinLabel);
            this.preconditionPinPanel.Controls.Add(this.preconditionPinSerialComboBox);
            this.preconditionPinPanel.Controls.Add(this.preconditionPinSerialLabel);
            this.preconditionPinPanel.Name = "preconditionPinPanel";
            // 
            // preconditionPinValueComboBox
            // 
            resources.ApplyResources(this.preconditionPinValueComboBox, "preconditionPinValueComboBox");
            this.preconditionPinValueComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.preconditionPinValueComboBox.FormattingEnabled = true;
            this.preconditionPinValueComboBox.Items.AddRange(new object[] {
            resources.GetString("preconditionPinValueComboBox.Items"),
            resources.GetString("preconditionPinValueComboBox.Items1")});
            this.preconditionPinValueComboBox.Name = "preconditionPinValueComboBox";
            // 
            // preconditionPinValueLabel
            // 
            resources.ApplyResources(this.preconditionPinValueLabel, "preconditionPinValueLabel");
            this.preconditionPinValueLabel.Name = "preconditionPinValueLabel";
            // 
            // preconditionPinComboBox
            // 
            resources.ApplyResources(this.preconditionPinComboBox, "preconditionPinComboBox");
            this.preconditionPinComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.preconditionPinComboBox.FormattingEnabled = true;
            this.preconditionPinComboBox.Items.AddRange(new object[] {
            resources.GetString("preconditionPinComboBox.Items"),
            resources.GetString("preconditionPinComboBox.Items1"),
            resources.GetString("preconditionPinComboBox.Items2"),
            resources.GetString("preconditionPinComboBox.Items3"),
            resources.GetString("preconditionPinComboBox.Items4"),
            resources.GetString("preconditionPinComboBox.Items5")});
            this.preconditionPinComboBox.Name = "preconditionPinComboBox";
            this.preconditionPinComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.preconditionPinComboBox_Validating);

            // 
            // preconditionPortComboBox
            // 
            resources.ApplyResources(this.preconditionPortComboBox, "preconditionPortComboBox");
            this.preconditionPortComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.preconditionPortComboBox.FormattingEnabled = true;
            this.preconditionPortComboBox.Items.AddRange(new object[] {
            resources.GetString("preconditionPortComboBox.Items"),
            resources.GetString("preconditionPortComboBox.Items1"),
            resources.GetString("preconditionPortComboBox.Items2"),
            resources.GetString("preconditionPortComboBox.Items3"),
            resources.GetString("preconditionPortComboBox.Items4"),
            resources.GetString("preconditionPortComboBox.Items5")});
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
            resources.ApplyResources(this.preconditionPinSerialComboBox, "preconditionPinSerialComboBox");
            this.preconditionPinSerialComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.preconditionPinSerialComboBox.FormattingEnabled = true;
            this.preconditionPinSerialComboBox.Items.AddRange(new object[] {
            resources.GetString("preconditionPinSerialComboBox.Items"),
            resources.GetString("preconditionPinSerialComboBox.Items1")});
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
            resources.ApplyResources(this.preconditionRuleConfigPanel, "preconditionRuleConfigPanel");
            this.preconditionRuleConfigPanel.Controls.Add(this.preconditionRefValueTextBox);
            this.preconditionRuleConfigPanel.Controls.Add(this.preconditionRefOperandComboBox);
            this.preconditionRuleConfigPanel.Controls.Add(this.preconditionConfigRefOperandLabel);
            this.preconditionRuleConfigPanel.Controls.Add(this.preconditionConfigComboBox);
            this.preconditionRuleConfigPanel.Controls.Add(this.label11);
            this.preconditionRuleConfigPanel.Name = "preconditionRuleConfigPanel";  

            // 
            // preconditionRefValueTextBox
            // 
            resources.ApplyResources(this.preconditionRefValueTextBox, "preconditionRefValueTextBox");
            this.preconditionRefValueTextBox.Name = "preconditionRefValueTextBox";
            this.preconditionRefValueTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.preconditionRefValueTextBox_Validating);
            // 
            // preconditionRefOperandComboBox
            // 
            resources.ApplyResources(this.preconditionRefOperandComboBox, "preconditionRefOperandComboBox");
            this.preconditionRefOperandComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.preconditionRefOperandComboBox.FormattingEnabled = true;
            this.preconditionRefOperandComboBox.Items.AddRange(new object[] {
            resources.GetString("preconditionRefOperandComboBox.Items"),
            resources.GetString("preconditionRefOperandComboBox.Items1"),
            resources.GetString("preconditionRefOperandComboBox.Items2"),
            resources.GetString("preconditionRefOperandComboBox.Items3"),
            resources.GetString("preconditionRefOperandComboBox.Items4"),
            resources.GetString("preconditionRefOperandComboBox.Items5")});
            this.preconditionRefOperandComboBox.Name = "preconditionRefOperandComboBox";
            // 
            // preconditionConfigRefOperandLabel
            // 
            resources.ApplyResources(this.preconditionConfigRefOperandLabel, "preconditionConfigRefOperandLabel");
            this.preconditionConfigRefOperandLabel.Name = "preconditionConfigRefOperandLabel";
            // 
            // preconditionConfigComboBox
            // 
            resources.ApplyResources(this.preconditionConfigComboBox, "preconditionConfigComboBox");
            this.preconditionConfigComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.preconditionConfigComboBox.FormattingEnabled = true;
            this.preconditionConfigComboBox.Items.AddRange(new object[] {
            resources.GetString("preconditionConfigComboBox.Items"),
            resources.GetString("preconditionConfigComboBox.Items1")});
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
            // preconditionTreeImageList
            // 
            this.preconditionTreeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("preconditionTreeImageList.ImageStream")));
            this.preconditionTreeImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.preconditionTreeImageList.Images.SetKeyName(0, "media_stop.png");
            this.preconditionTreeImageList.Images.SetKeyName(1, "media_stop_red.png");
            // 
            // preconditionTreeContextMenuStrip
            // 
            resources.ApplyResources(this.preconditionTreeContextMenuStrip, "preconditionTreeContextMenuStrip");
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
            // 
            // addPreconditionToolStripMenuItem
            // 
            resources.ApplyResources(this.addPreconditionToolStripMenuItem, "addPreconditionToolStripMenuItem");
            this.addPreconditionToolStripMenuItem.Name = "addPreconditionToolStripMenuItem";
            this.addPreconditionToolStripMenuItem.Click += new System.EventHandler(this.addPreconditionToolStripMenuItem_Click);

            // 
            // removePreconditionToolStripMenuItem
            // 
            resources.ApplyResources(this.removePreconditionToolStripMenuItem, "removePreconditionToolStripMenuItem");
            this.removePreconditionToolStripMenuItem.Name = "removePreconditionToolStripMenuItem";
            this.removePreconditionToolStripMenuItem.Click += new System.EventHandler(this.removePreconditionToolStripMenuItem_Click);

            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // addGroupToolStripMenuItem
            // 
            resources.ApplyResources(this.addGroupToolStripMenuItem, "addGroupToolStripMenuItem");
            this.addGroupToolStripMenuItem.Name = "addGroupToolStripMenuItem";
            // 
            // removeGroupToolStripMenuItem
            // 
            resources.ApplyResources(this.removeGroupToolStripMenuItem, "removeGroupToolStripMenuItem");
            this.removeGroupToolStripMenuItem.Name = "removeGroupToolStripMenuItem";
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // logicSelectToolStripMenuItem
            // 
            resources.ApplyResources(this.logicSelectToolStripMenuItem, "logicSelectToolStripMenuItem");
            this.logicSelectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aNDToolStripMenuItem,
            this.oRToolStripMenuItem});
            this.logicSelectToolStripMenuItem.Name = "logicSelectToolStripMenuItem";
            // 
            // aNDToolStripMenuItem
            // 
            resources.ApplyResources(this.aNDToolStripMenuItem, "aNDToolStripMenuItem");
            this.aNDToolStripMenuItem.Name = "aNDToolStripMenuItem";
            this.aNDToolStripMenuItem.Click += new System.EventHandler(this.andOrToolStripMenuItem_Click);
            // 
            // oRToolStripMenuItem
            // 
            resources.ApplyResources(this.oRToolStripMenuItem, "oRToolStripMenuItem");
            this.oRToolStripMenuItem.Name = "oRToolStripMenuItem";
            this.oRToolStripMenuItem.Click += new System.EventHandler(this.andOrToolStripMenuItem_Click);

            // 
            // PreconditionPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.preconditionSpacerPanel);
            this.Controls.Add(this.overrideGroupBox);
            this.Controls.Add(this.preconditionSettingsPanel);
            this.Controls.Add(this.preconditionListgroupBox);
            this.Controls.Add(this.preconditionTabTextBox);
            this.Name = "PreconditionPanel";
            this.preconditionListgroupBox.ResumeLayout(false);
            this.preconditionSelectGroupBox.ResumeLayout(false);
            this.overrideGroupBox.ResumeLayout(false);
            this.overrideGroupBox.PerformLayout();
            this.preconditionSettingsPanel.ResumeLayout(false);
            this.preconditionSettingsGroupBox.ResumeLayout(false);
            this.preconditionPinPanel.ResumeLayout(false);
            this.preconditionRuleConfigPanel.ResumeLayout(false);
            this.preconditionRuleConfigPanel.PerformLayout();
            this.preconditionTreeContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox preconditionListgroupBox;
        private System.Windows.Forms.TreeView preconditionListTreeView;
        private System.Windows.Forms.TextBox preconditionTabTextBox;
        private System.Windows.Forms.GroupBox preconditionSelectGroupBox;
        private System.Windows.Forms.ComboBox preConditionTypeComboBox;
        private System.Windows.Forms.Label preconditionTypeLabel;
        private System.Windows.Forms.Panel preconditionSpacerPanel;
        private System.Windows.Forms.GroupBox overrideGroupBox;
        private System.Windows.Forms.TextBox overridePreconditionTextBox;
        private System.Windows.Forms.CheckBox overridePreconditionCheckBox;
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
        private System.Windows.Forms.ImageList preconditionTreeImageList;
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
    }
}
