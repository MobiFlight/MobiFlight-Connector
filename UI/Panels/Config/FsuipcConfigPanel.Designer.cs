﻿namespace MobiFlight.UI.Panels.Config
{
    partial class FsuipcConfigPanel
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

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FsuipcConfigPanel));
            this.fsuipcMoreOptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.valuePanel = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.fsuipcValueTextBox = new System.Windows.Forms.TextBox();
            this.SubstringPanel = new System.Windows.Forms.Panel();
            this.SubstringTransformationCheckBox = new System.Windows.Forms.CheckBox();
            this.SubStringToTextBox = new System.Windows.Forms.TextBox();
            this.SubStringToLabel = new System.Windows.Forms.Label();
            this.SubStringFromTextBox = new System.Windows.Forms.TextBox();
            this.multiplyPanel = new System.Windows.Forms.Panel();
            this.TransformationCheckBox = new System.Windows.Forms.CheckBox();
            this.fsuipcMultiplyTextBox = new System.Windows.Forms.TextBox();
            this.maskAndBcdPanel = new System.Windows.Forms.Panel();
            this.maskEditorButton = new System.Windows.Forms.Button();
            this.fsuipcBcdModeCheckBox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.fsuipcMaskTextBox = new System.Windows.Forms.TextBox();
            this.fsuipcBaseSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.offsetPanel = new System.Windows.Forms.Panel();
            this.fsuipcTypeLabel = new System.Windows.Forms.Label();
            this.fsuipcOffsetTypeComboBox = new System.Windows.Forms.ComboBox();
            this.fsuipcSizeLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.fsuipcSizeComboBox = new System.Windows.Forms.ComboBox();
            this.fsuipcOffsetTextBox = new System.Windows.Forms.TextBox();
            this.fsuipcLoadPresetGroupBox = new System.Windows.Forms.GroupBox();
            this.fsuipcPresetUseButton = new System.Windows.Forms.Button();
            this.labelFsuipcPreset = new System.Windows.Forms.Label();
            this.fsuipcPresetComboBox = new System.Windows.Forms.ComboBox();
            this.presetsDataSet = new System.Data.DataSet();
            this.presetDataTable = new System.Data.DataTable();
            this.description = new System.Data.DataColumn();
            this.settingsColumn = new System.Data.DataColumn();
            this.fsuipcMoreOptionsGroupBox.SuspendLayout();
            this.valuePanel.SuspendLayout();
            this.SubstringPanel.SuspendLayout();
            this.multiplyPanel.SuspendLayout();
            this.maskAndBcdPanel.SuspendLayout();
            this.fsuipcBaseSettingsGroupBox.SuspendLayout();
            this.offsetPanel.SuspendLayout();
            this.fsuipcLoadPresetGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.presetsDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.presetDataTable)).BeginInit();
            this.SuspendLayout();
            // 
            // fsuipcMoreOptionsGroupBox
            // 
            resources.ApplyResources(this.fsuipcMoreOptionsGroupBox, "fsuipcMoreOptionsGroupBox");
            this.fsuipcMoreOptionsGroupBox.Controls.Add(this.valuePanel);
            this.fsuipcMoreOptionsGroupBox.Controls.Add(this.SubstringPanel);
            this.fsuipcMoreOptionsGroupBox.Controls.Add(this.multiplyPanel);
            this.fsuipcMoreOptionsGroupBox.Name = "fsuipcMoreOptionsGroupBox";
            this.fsuipcMoreOptionsGroupBox.TabStop = false;
            // 
            // valuePanel
            // 
            resources.ApplyResources(this.valuePanel, "valuePanel");
            this.valuePanel.Controls.Add(this.label5);
            this.valuePanel.Controls.Add(this.fsuipcValueTextBox);
            this.valuePanel.Name = "valuePanel";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // fsuipcValueTextBox
            // 
            resources.ApplyResources(this.fsuipcValueTextBox, "fsuipcValueTextBox");
            this.fsuipcValueTextBox.Name = "fsuipcValueTextBox";
            this.fsuipcValueTextBox.TextChanged += new System.EventHandler(this.fsuipcValueTextBox_TextChanged);
            this.fsuipcValueTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.fsuipcValueTextBox_Validating);
            // 
            // SubstringPanel
            // 
            resources.ApplyResources(this.SubstringPanel, "SubstringPanel");
            this.SubstringPanel.Controls.Add(this.SubstringTransformationCheckBox);
            this.SubstringPanel.Controls.Add(this.SubStringToTextBox);
            this.SubstringPanel.Controls.Add(this.SubStringToLabel);
            this.SubstringPanel.Controls.Add(this.SubStringFromTextBox);
            this.SubstringPanel.Name = "SubstringPanel";
            // 
            // SubstringTransformationCheckBox
            // 
            this.SubstringTransformationCheckBox.Checked = true;
            this.SubstringTransformationCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.SubstringTransformationCheckBox, "SubstringTransformationCheckBox");
            this.SubstringTransformationCheckBox.Name = "SubstringTransformationCheckBox";
            this.SubstringTransformationCheckBox.UseVisualStyleBackColor = true;
            // 
            // SubStringToTextBox
            // 
            resources.ApplyResources(this.SubStringToTextBox, "SubStringToTextBox");
            this.SubStringToTextBox.Name = "SubStringToTextBox";
            // 
            // SubStringToLabel
            // 
            resources.ApplyResources(this.SubStringToLabel, "SubStringToLabel");
            this.SubStringToLabel.Name = "SubStringToLabel";
            // 
            // SubStringFromTextBox
            // 
            resources.ApplyResources(this.SubStringFromTextBox, "SubStringFromTextBox");
            this.SubStringFromTextBox.Name = "SubStringFromTextBox";
            // 
            // multiplyPanel
            // 
            resources.ApplyResources(this.multiplyPanel, "multiplyPanel");
            this.multiplyPanel.Controls.Add(this.TransformationCheckBox);
            this.multiplyPanel.Controls.Add(this.fsuipcMultiplyTextBox);
            this.multiplyPanel.Name = "multiplyPanel";
            // 
            // TransformationCheckBox
            // 
            this.TransformationCheckBox.Checked = true;
            this.TransformationCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.TransformationCheckBox, "TransformationCheckBox");
            this.TransformationCheckBox.Name = "TransformationCheckBox";
            this.TransformationCheckBox.UseVisualStyleBackColor = true;
            this.TransformationCheckBox.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // fsuipcMultiplyTextBox
            // 
            resources.ApplyResources(this.fsuipcMultiplyTextBox, "fsuipcMultiplyTextBox");
            this.fsuipcMultiplyTextBox.Name = "fsuipcMultiplyTextBox";
            this.fsuipcMultiplyTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.fsuipcMultiplyTextBox_Validating);
            // 
            // maskAndBcdPanel
            // 
            resources.ApplyResources(this.maskAndBcdPanel, "maskAndBcdPanel");
            this.maskAndBcdPanel.Controls.Add(this.maskEditorButton);
            this.maskAndBcdPanel.Controls.Add(this.fsuipcBcdModeCheckBox);
            this.maskAndBcdPanel.Controls.Add(this.label3);
            this.maskAndBcdPanel.Controls.Add(this.fsuipcMaskTextBox);
            this.maskAndBcdPanel.Name = "maskAndBcdPanel";
            // 
            // maskEditorButton
            // 
            resources.ApplyResources(this.maskEditorButton, "maskEditorButton");
            this.maskEditorButton.Name = "maskEditorButton";
            this.maskEditorButton.UseVisualStyleBackColor = true;
            this.maskEditorButton.Click += new System.EventHandler(this.maskEditorButton_Click);
            // 
            // fsuipcBcdModeCheckBox
            // 
            resources.ApplyResources(this.fsuipcBcdModeCheckBox, "fsuipcBcdModeCheckBox");
            this.fsuipcBcdModeCheckBox.Name = "fsuipcBcdModeCheckBox";
            this.fsuipcBcdModeCheckBox.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // fsuipcMaskTextBox
            // 
            resources.ApplyResources(this.fsuipcMaskTextBox, "fsuipcMaskTextBox");
            this.fsuipcMaskTextBox.Name = "fsuipcMaskTextBox";
            this.fsuipcMaskTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.fsuipcMaskTextBox_Validating);
            // 
            // fsuipcBaseSettingsGroupBox
            // 
            resources.ApplyResources(this.fsuipcBaseSettingsGroupBox, "fsuipcBaseSettingsGroupBox");
            this.fsuipcBaseSettingsGroupBox.Controls.Add(this.maskAndBcdPanel);
            this.fsuipcBaseSettingsGroupBox.Controls.Add(this.offsetPanel);
            this.fsuipcBaseSettingsGroupBox.Name = "fsuipcBaseSettingsGroupBox";
            this.fsuipcBaseSettingsGroupBox.TabStop = false;
            // 
            // offsetPanel
            // 
            resources.ApplyResources(this.offsetPanel, "offsetPanel");
            this.offsetPanel.Controls.Add(this.fsuipcTypeLabel);
            this.offsetPanel.Controls.Add(this.fsuipcOffsetTypeComboBox);
            this.offsetPanel.Controls.Add(this.fsuipcSizeLabel);
            this.offsetPanel.Controls.Add(this.label1);
            this.offsetPanel.Controls.Add(this.fsuipcSizeComboBox);
            this.offsetPanel.Controls.Add(this.fsuipcOffsetTextBox);
            this.offsetPanel.Name = "offsetPanel";
            // 
            // fsuipcTypeLabel
            // 
            resources.ApplyResources(this.fsuipcTypeLabel, "fsuipcTypeLabel");
            this.fsuipcTypeLabel.Name = "fsuipcTypeLabel";
            // 
            // fsuipcOffsetTypeComboBox
            // 
            this.fsuipcOffsetTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fsuipcOffsetTypeComboBox.FormattingEnabled = true;
            this.fsuipcOffsetTypeComboBox.Items.AddRange(new object[] {
            resources.GetString("fsuipcOffsetTypeComboBox.Items"),
            resources.GetString("fsuipcOffsetTypeComboBox.Items1"),
            resources.GetString("fsuipcOffsetTypeComboBox.Items2")});
            resources.ApplyResources(this.fsuipcOffsetTypeComboBox, "fsuipcOffsetTypeComboBox");
            this.fsuipcOffsetTypeComboBox.Name = "fsuipcOffsetTypeComboBox";
            this.fsuipcOffsetTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.fsuipcOffsetTypeComboBox_SelectedIndexChanged);
            // 
            // fsuipcSizeLabel
            // 
            resources.ApplyResources(this.fsuipcSizeLabel, "fsuipcSizeLabel");
            this.fsuipcSizeLabel.Name = "fsuipcSizeLabel";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // fsuipcSizeComboBox
            // 
            this.fsuipcSizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fsuipcSizeComboBox.FormattingEnabled = true;
            this.fsuipcSizeComboBox.Items.AddRange(new object[] {
            resources.GetString("fsuipcSizeComboBox.Items"),
            resources.GetString("fsuipcSizeComboBox.Items1"),
            resources.GetString("fsuipcSizeComboBox.Items2"),
            resources.GetString("fsuipcSizeComboBox.Items3")});
            resources.ApplyResources(this.fsuipcSizeComboBox, "fsuipcSizeComboBox");
            this.fsuipcSizeComboBox.Name = "fsuipcSizeComboBox";
            this.fsuipcSizeComboBox.SelectedIndexChanged += new System.EventHandler(this.fsuipcSizeComboBox_SelectedIndexChanged);
            // 
            // fsuipcOffsetTextBox
            // 
            resources.ApplyResources(this.fsuipcOffsetTextBox, "fsuipcOffsetTextBox");
            this.fsuipcOffsetTextBox.Name = "fsuipcOffsetTextBox";
            this.fsuipcOffsetTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.fsuipcOffsetTextBox_Validating);
            // 
            // fsuipcLoadPresetGroupBox
            // 
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.fsuipcPresetUseButton);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.labelFsuipcPreset);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.fsuipcPresetComboBox);
            resources.ApplyResources(this.fsuipcLoadPresetGroupBox, "fsuipcLoadPresetGroupBox");
            this.fsuipcLoadPresetGroupBox.Name = "fsuipcLoadPresetGroupBox";
            this.fsuipcLoadPresetGroupBox.TabStop = false;
            // 
            // fsuipcPresetUseButton
            // 
            resources.ApplyResources(this.fsuipcPresetUseButton, "fsuipcPresetUseButton");
            this.fsuipcPresetUseButton.Name = "fsuipcPresetUseButton";
            this.fsuipcPresetUseButton.UseVisualStyleBackColor = true;
            this.fsuipcPresetUseButton.Click += new System.EventHandler(this.fsuipcPresetUseButton_Click);
            // 
            // labelFsuipcPreset
            // 
            resources.ApplyResources(this.labelFsuipcPreset, "labelFsuipcPreset");
            this.labelFsuipcPreset.Name = "labelFsuipcPreset";
            // 
            // fsuipcPresetComboBox
            // 
            resources.ApplyResources(this.fsuipcPresetComboBox, "fsuipcPresetComboBox");
            this.fsuipcPresetComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.fsuipcPresetComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.fsuipcPresetComboBox.DropDownWidth = 250;
            this.fsuipcPresetComboBox.FormattingEnabled = true;
            this.fsuipcPresetComboBox.Name = "fsuipcPresetComboBox";
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
            // FsuipcConfigPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fsuipcMoreOptionsGroupBox);
            this.Controls.Add(this.fsuipcBaseSettingsGroupBox);
            this.Controls.Add(this.fsuipcLoadPresetGroupBox);
            this.Name = "FsuipcConfigPanel";
            this.fsuipcMoreOptionsGroupBox.ResumeLayout(false);
            this.fsuipcMoreOptionsGroupBox.PerformLayout();
            this.valuePanel.ResumeLayout(false);
            this.valuePanel.PerformLayout();
            this.SubstringPanel.ResumeLayout(false);
            this.SubstringPanel.PerformLayout();
            this.multiplyPanel.ResumeLayout(false);
            this.multiplyPanel.PerformLayout();
            this.maskAndBcdPanel.ResumeLayout(false);
            this.maskAndBcdPanel.PerformLayout();
            this.fsuipcBaseSettingsGroupBox.ResumeLayout(false);
            this.fsuipcBaseSettingsGroupBox.PerformLayout();
            this.offsetPanel.ResumeLayout(false);
            this.offsetPanel.PerformLayout();
            this.fsuipcLoadPresetGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.presetsDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.presetDataTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox fsuipcMoreOptionsGroupBox;
        private System.Windows.Forms.GroupBox fsuipcBaseSettingsGroupBox;
        private System.Windows.Forms.GroupBox fsuipcLoadPresetGroupBox;
        private System.Windows.Forms.Button fsuipcPresetUseButton;
        private System.Windows.Forms.Label labelFsuipcPreset;
        private System.Windows.Forms.ComboBox fsuipcPresetComboBox;
        private System.Data.DataSet presetsDataSet;
        private System.Data.DataTable presetDataTable;
        private System.Data.DataColumn description;
        private System.Data.DataColumn settingsColumn;
        private System.Windows.Forms.Panel valuePanel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox fsuipcValueTextBox;
        private System.Windows.Forms.Panel offsetPanel;
        private System.Windows.Forms.Label fsuipcTypeLabel;
        private System.Windows.Forms.ComboBox fsuipcOffsetTypeComboBox;
        private System.Windows.Forms.Label fsuipcSizeLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox fsuipcSizeComboBox;
        private System.Windows.Forms.TextBox fsuipcOffsetTextBox;
        private System.Windows.Forms.Panel multiplyPanel;
        private System.Windows.Forms.TextBox fsuipcMultiplyTextBox;
        private System.Windows.Forms.Panel maskAndBcdPanel;
        private System.Windows.Forms.Button maskEditorButton;
        private System.Windows.Forms.CheckBox fsuipcBcdModeCheckBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox fsuipcMaskTextBox;
        private System.Windows.Forms.Panel SubstringPanel;
        private System.Windows.Forms.TextBox SubStringToTextBox;
        private System.Windows.Forms.Label SubStringToLabel;
        private System.Windows.Forms.TextBox SubStringFromTextBox;
        private System.Windows.Forms.CheckBox TransformationCheckBox;
        private System.Windows.Forms.CheckBox SubstringTransformationCheckBox;
    }
}
