namespace MobiFlight.Panels.Group
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
            this.fsuipcMoreOptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.valuePanel = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.fsuipcValueTextBox = new System.Windows.Forms.TextBox();
            this.SubstringPanel = new System.Windows.Forms.Panel();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SubStringLabelFrom = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.multiplyPanel = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
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
            this.label2 = new System.Windows.Forms.Label();
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
            this.fsuipcMoreOptionsGroupBox.AutoSize = true;
            this.fsuipcMoreOptionsGroupBox.Controls.Add(this.valuePanel);
            this.fsuipcMoreOptionsGroupBox.Controls.Add(this.SubstringPanel);
            this.fsuipcMoreOptionsGroupBox.Controls.Add(this.multiplyPanel);
            this.fsuipcMoreOptionsGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.fsuipcMoreOptionsGroupBox.Location = new System.Drawing.Point(0, 144);
            this.fsuipcMoreOptionsGroupBox.Name = "fsuipcMoreOptionsGroupBox";
            this.fsuipcMoreOptionsGroupBox.Size = new System.Drawing.Size(315, 93);
            this.fsuipcMoreOptionsGroupBox.TabIndex = 21;
            this.fsuipcMoreOptionsGroupBox.TabStop = false;
            this.fsuipcMoreOptionsGroupBox.Text = "More Options";
            // 
            // valuePanel
            // 
            this.valuePanel.AutoSize = true;
            this.valuePanel.Controls.Add(this.label5);
            this.valuePanel.Controls.Add(this.fsuipcValueTextBox);
            this.valuePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.valuePanel.Location = new System.Drawing.Point(3, 65);
            this.valuePanel.Name = "valuePanel";
            this.valuePanel.Size = new System.Drawing.Size(309, 25);
            this.valuePanel.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(53, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Set Value";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // fsuipcValueTextBox
            // 
            this.fsuipcValueTextBox.Location = new System.Drawing.Point(110, 2);
            this.fsuipcValueTextBox.Name = "fsuipcValueTextBox";
            this.fsuipcValueTextBox.Size = new System.Drawing.Size(102, 20);
            this.fsuipcValueTextBox.TabIndex = 17;
            this.fsuipcValueTextBox.TextChanged += new System.EventHandler(this.fsuipcValueTextBox_TextChanged);
            this.fsuipcValueTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.fsuipcValueTextBox_Validating);
            // 
            // SubstringPanel
            // 
            this.SubstringPanel.AutoSize = true;
            this.SubstringPanel.Controls.Add(this.textBox3);
            this.SubstringPanel.Controls.Add(this.label6);
            this.SubstringPanel.Controls.Add(this.SubStringLabelFrom);
            this.SubstringPanel.Controls.Add(this.textBox2);
            this.SubstringPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.SubstringPanel.Location = new System.Drawing.Point(3, 40);
            this.SubstringPanel.Name = "SubstringPanel";
            this.SubstringPanel.Size = new System.Drawing.Size(309, 25);
            this.SubstringPanel.TabIndex = 11;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(192, 2);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(52, 20);
            this.textBox3.TabIndex = 20;
            // 
            // label6
            // 
            this.label6.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(165, 5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(26, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = "to";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SubStringLabelFrom
            // 
            this.SubStringLabelFrom.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.SubStringLabelFrom.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.SubStringLabelFrom.Location = new System.Drawing.Point(3, 5);
            this.SubStringLabelFrom.Name = "SubStringLabelFrom";
            this.SubStringLabelFrom.Size = new System.Drawing.Size(106, 13);
            this.SubStringLabelFrom.TabIndex = 18;
            this.SubStringLabelFrom.Text = "Substring from";
            this.SubStringLabelFrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(110, 2);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(52, 20);
            this.textBox2.TabIndex = 17;
            // 
            // multiplyPanel
            // 
            this.multiplyPanel.AutoSize = true;
            this.multiplyPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.multiplyPanel.Controls.Add(this.label4);
            this.multiplyPanel.Controls.Add(this.fsuipcMultiplyTextBox);
            this.multiplyPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.multiplyPanel.Location = new System.Drawing.Point(3, 16);
            this.multiplyPanel.Name = "multiplyPanel";
            this.multiplyPanel.Size = new System.Drawing.Size(309, 24);
            this.multiplyPanel.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(48, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Transform";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // fsuipcMultiplyTextBox
            // 
            this.fsuipcMultiplyTextBox.Location = new System.Drawing.Point(110, 1);
            this.fsuipcMultiplyTextBox.Name = "fsuipcMultiplyTextBox";
            this.fsuipcMultiplyTextBox.Size = new System.Drawing.Size(195, 20);
            this.fsuipcMultiplyTextBox.TabIndex = 13;
            this.fsuipcMultiplyTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.fsuipcMultiplyTextBox_Validating);
            // 
            // maskAndBcdPanel
            // 
            this.maskAndBcdPanel.AutoSize = true;
            this.maskAndBcdPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.maskAndBcdPanel.Controls.Add(this.maskEditorButton);
            this.maskAndBcdPanel.Controls.Add(this.fsuipcBcdModeCheckBox);
            this.maskAndBcdPanel.Controls.Add(this.label3);
            this.maskAndBcdPanel.Controls.Add(this.fsuipcMaskTextBox);
            this.maskAndBcdPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.maskAndBcdPanel.Location = new System.Drawing.Point(3, 67);
            this.maskAndBcdPanel.Margin = new System.Windows.Forms.Padding(0);
            this.maskAndBcdPanel.Name = "maskAndBcdPanel";
            this.maskAndBcdPanel.Size = new System.Drawing.Size(309, 25);
            this.maskAndBcdPanel.TabIndex = 14;
            // 
            // maskEditorButton
            // 
            this.maskEditorButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.maskEditorButton.Location = new System.Drawing.Point(188, -1);
            this.maskEditorButton.Name = "maskEditorButton";
            this.maskEditorButton.Size = new System.Drawing.Size(24, 23);
            this.maskEditorButton.TabIndex = 17;
            this.maskEditorButton.Text = "...";
            this.maskEditorButton.UseVisualStyleBackColor = true;
            this.maskEditorButton.Click += new System.EventHandler(this.maskEditorButton_Click);
            // 
            // fsuipcBcdModeCheckBox
            // 
            this.fsuipcBcdModeCheckBox.AutoSize = true;
            this.fsuipcBcdModeCheckBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.fsuipcBcdModeCheckBox.Location = new System.Drawing.Point(231, 3);
            this.fsuipcBcdModeCheckBox.Name = "fsuipcBcdModeCheckBox";
            this.fsuipcBcdModeCheckBox.Size = new System.Drawing.Size(78, 17);
            this.fsuipcBcdModeCheckBox.TabIndex = 15;
            this.fsuipcBcdModeCheckBox.Text = "BCD Mode";
            this.fsuipcBcdModeCheckBox.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(21, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Mask value with";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // fsuipcMaskTextBox
            // 
            this.fsuipcMaskTextBox.Location = new System.Drawing.Point(110, 1);
            this.fsuipcMaskTextBox.Name = "fsuipcMaskTextBox";
            this.fsuipcMaskTextBox.Size = new System.Drawing.Size(75, 20);
            this.fsuipcMaskTextBox.TabIndex = 14;
            this.fsuipcMaskTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.fsuipcMaskTextBox_Validating);
            // 
            // fsuipcBaseSettingsGroupBox
            // 
            this.fsuipcBaseSettingsGroupBox.AutoSize = true;
            this.fsuipcBaseSettingsGroupBox.Controls.Add(this.maskAndBcdPanel);
            this.fsuipcBaseSettingsGroupBox.Controls.Add(this.offsetPanel);
            this.fsuipcBaseSettingsGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.fsuipcBaseSettingsGroupBox.Location = new System.Drawing.Point(0, 49);
            this.fsuipcBaseSettingsGroupBox.Name = "fsuipcBaseSettingsGroupBox";
            this.fsuipcBaseSettingsGroupBox.Size = new System.Drawing.Size(315, 95);
            this.fsuipcBaseSettingsGroupBox.TabIndex = 20;
            this.fsuipcBaseSettingsGroupBox.TabStop = false;
            this.fsuipcBaseSettingsGroupBox.Text = "Base settings";
            // 
            // offsetPanel
            // 
            this.offsetPanel.AutoSize = true;
            this.offsetPanel.Controls.Add(this.fsuipcTypeLabel);
            this.offsetPanel.Controls.Add(this.fsuipcOffsetTypeComboBox);
            this.offsetPanel.Controls.Add(this.label2);
            this.offsetPanel.Controls.Add(this.label1);
            this.offsetPanel.Controls.Add(this.fsuipcSizeComboBox);
            this.offsetPanel.Controls.Add(this.fsuipcOffsetTextBox);
            this.offsetPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.offsetPanel.Location = new System.Drawing.Point(3, 16);
            this.offsetPanel.Name = "offsetPanel";
            this.offsetPanel.Size = new System.Drawing.Size(309, 51);
            this.offsetPanel.TabIndex = 10;
            // 
            // fsuipcTypeLabel
            // 
            this.fsuipcTypeLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.fsuipcTypeLabel.Location = new System.Drawing.Point(1, 28);
            this.fsuipcTypeLabel.Name = "fsuipcTypeLabel";
            this.fsuipcTypeLabel.Size = new System.Drawing.Size(108, 18);
            this.fsuipcTypeLabel.TabIndex = 13;
            this.fsuipcTypeLabel.Text = "Value Type";
            this.fsuipcTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // fsuipcOffsetTypeComboBox
            // 
            this.fsuipcOffsetTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fsuipcOffsetTypeComboBox.FormattingEnabled = true;
            this.fsuipcOffsetTypeComboBox.Items.AddRange(new object[] {
            "Int",
            "Float",
            "String"});
            this.fsuipcOffsetTypeComboBox.Location = new System.Drawing.Point(110, 27);
            this.fsuipcOffsetTypeComboBox.Name = "fsuipcOffsetTypeComboBox";
            this.fsuipcOffsetTypeComboBox.Size = new System.Drawing.Size(65, 21);
            this.fsuipcOffsetTypeComboBox.TabIndex = 14;
            this.fsuipcOffsetTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.fsuipcOffsetTypeComboBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(173, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 17);
            this.label2.TabIndex = 10;
            this.label2.Text = "Size in Bytes";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(74, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Offset";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // fsuipcSizeComboBox
            // 
            this.fsuipcSizeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fsuipcSizeComboBox.FormattingEnabled = true;
            this.fsuipcSizeComboBox.Items.AddRange(new object[] {
            "1",
            "2",
            "4",
            "8"});
            this.fsuipcSizeComboBox.Location = new System.Drawing.Point(255, 27);
            this.fsuipcSizeComboBox.Name = "fsuipcSizeComboBox";
            this.fsuipcSizeComboBox.Size = new System.Drawing.Size(50, 21);
            this.fsuipcSizeComboBox.TabIndex = 12;
            this.fsuipcSizeComboBox.SelectedIndexChanged += new System.EventHandler(this.fsuipcSizeComboBox_SelectedIndexChanged);
            // 
            // fsuipcOffsetTextBox
            // 
            this.fsuipcOffsetTextBox.Location = new System.Drawing.Point(110, 4);
            this.fsuipcOffsetTextBox.Name = "fsuipcOffsetTextBox";
            this.fsuipcOffsetTextBox.Size = new System.Drawing.Size(52, 20);
            this.fsuipcOffsetTextBox.TabIndex = 11;
            this.fsuipcOffsetTextBox.Text = "0xAAAA";
            this.fsuipcOffsetTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.fsuipcOffsetTextBox_Validating);
            // 
            // fsuipcLoadPresetGroupBox
            // 
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.fsuipcPresetUseButton);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.labelFsuipcPreset);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.fsuipcPresetComboBox);
            this.fsuipcLoadPresetGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.fsuipcLoadPresetGroupBox.Location = new System.Drawing.Point(0, 0);
            this.fsuipcLoadPresetGroupBox.Name = "fsuipcLoadPresetGroupBox";
            this.fsuipcLoadPresetGroupBox.Size = new System.Drawing.Size(315, 49);
            this.fsuipcLoadPresetGroupBox.TabIndex = 22;
            this.fsuipcLoadPresetGroupBox.TabStop = false;
            this.fsuipcLoadPresetGroupBox.Text = "Load preset";
            // 
            // fsuipcPresetUseButton
            // 
            this.fsuipcPresetUseButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.fsuipcPresetUseButton.Location = new System.Drawing.Point(274, 20);
            this.fsuipcPresetUseButton.Name = "fsuipcPresetUseButton";
            this.fsuipcPresetUseButton.Size = new System.Drawing.Size(36, 23);
            this.fsuipcPresetUseButton.TabIndex = 6;
            this.fsuipcPresetUseButton.Text = "use";
            this.fsuipcPresetUseButton.UseVisualStyleBackColor = true;
            this.fsuipcPresetUseButton.Click += new System.EventHandler(this.fsuipcPresetUseButton_Click);
            // 
            // labelFsuipcPreset
            // 
            this.labelFsuipcPreset.AutoSize = true;
            this.labelFsuipcPreset.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelFsuipcPreset.Location = new System.Drawing.Point(55, 24);
            this.labelFsuipcPreset.Name = "labelFsuipcPreset";
            this.labelFsuipcPreset.Size = new System.Drawing.Size(58, 13);
            this.labelFsuipcPreset.TabIndex = 5;
            this.labelFsuipcPreset.Text = "Use preset";
            this.labelFsuipcPreset.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // fsuipcPresetComboBox
            // 
            this.fsuipcPresetComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.fsuipcPresetComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.fsuipcPresetComboBox.FormattingEnabled = true;
            this.fsuipcPresetComboBox.Location = new System.Drawing.Point(113, 21);
            this.fsuipcPresetComboBox.Name = "fsuipcPresetComboBox";
            this.fsuipcPresetComboBox.Size = new System.Drawing.Size(159, 21);
            this.fsuipcPresetComboBox.TabIndex = 4;
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
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.fsuipcMoreOptionsGroupBox);
            this.Controls.Add(this.fsuipcBaseSettingsGroupBox);
            this.Controls.Add(this.fsuipcLoadPresetGroupBox);
            this.Name = "FsuipcConfigPanel";
            this.Size = new System.Drawing.Size(315, 239);
            this.Load += new System.EventHandler(this.FsuipcConfigPanel_Load);
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
            this.fsuipcLoadPresetGroupBox.PerformLayout();
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
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox fsuipcSizeComboBox;
        private System.Windows.Forms.TextBox fsuipcOffsetTextBox;
        private System.Windows.Forms.Panel multiplyPanel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox fsuipcMultiplyTextBox;
        private System.Windows.Forms.Panel maskAndBcdPanel;
        private System.Windows.Forms.Button maskEditorButton;
        private System.Windows.Forms.CheckBox fsuipcBcdModeCheckBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox fsuipcMaskTextBox;
        private System.Windows.Forms.Panel SubstringPanel;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label SubStringLabelFrom;
        private System.Windows.Forms.TextBox textBox2;
    }
}
