namespace MobiFlight.UI.Panels.Config
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
            this.panelModifierHint = new System.Windows.Forms.Panel();
            this.buttonModifyTab = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.transformOptionsGroup1 = new MobiFlight.UI.Panels.Config.TransformOptionsGroup();
            this.labelModifierHint = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.maskAndBcdPanel.SuspendLayout();
            this.fsuipcBaseSettingsGroupBox.SuspendLayout();
            this.offsetPanel.SuspendLayout();
            this.fsuipcLoadPresetGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.presetsDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.presetDataTable)).BeginInit();
            this.panelModifierHint.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
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
            // panelModifierHint
            // 
            this.panelModifierHint.Controls.Add(this.panel1);
            this.panelModifierHint.Controls.Add(this.pictureBox1);
            resources.ApplyResources(this.panelModifierHint, "panelModifierHint");
            this.panelModifierHint.Name = "panelModifierHint";
            // 
            // buttonModifyTab
            // 
            resources.ApplyResources(this.buttonModifyTab, "buttonModifyTab");
            this.buttonModifyTab.Name = "buttonModifyTab";
            this.buttonModifyTab.UseVisualStyleBackColor = true;
            this.buttonModifyTab.Click += new System.EventHandler(this.ButtonModifyTab_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::MobiFlight.Properties.Resources.mobiflight_icon_out_led;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // transformOptionsGroup1
            // 
            resources.ApplyResources(this.transformOptionsGroup1, "transformOptionsGroup1");
            this.transformOptionsGroup1.Name = "transformOptionsGroup1";
            // 
            // labelModifierHint
            // 
            resources.ApplyResources(this.labelModifierHint, "labelModifierHint");
            this.labelModifierHint.Name = "labelModifierHint";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.buttonModifyTab);
            this.panel1.Controls.Add(this.labelModifierHint);
            this.panel1.Name = "panel1";
            // 
            // FsuipcConfigPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelModifierHint);
            this.Controls.Add(this.transformOptionsGroup1);
            this.Controls.Add(this.fsuipcBaseSettingsGroupBox);
            this.Controls.Add(this.fsuipcLoadPresetGroupBox);
            this.Name = "FsuipcConfigPanel";
            this.maskAndBcdPanel.ResumeLayout(false);
            this.maskAndBcdPanel.PerformLayout();
            this.fsuipcBaseSettingsGroupBox.ResumeLayout(false);
            this.fsuipcBaseSettingsGroupBox.PerformLayout();
            this.offsetPanel.ResumeLayout(false);
            this.offsetPanel.PerformLayout();
            this.fsuipcLoadPresetGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.presetsDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.presetDataTable)).EndInit();
            this.panelModifierHint.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox fsuipcBaseSettingsGroupBox;
        private System.Windows.Forms.GroupBox fsuipcLoadPresetGroupBox;
        private System.Windows.Forms.Button fsuipcPresetUseButton;
        private System.Windows.Forms.Label labelFsuipcPreset;
        private System.Windows.Forms.ComboBox fsuipcPresetComboBox;
        private System.Data.DataSet presetsDataSet;
        private System.Data.DataTable presetDataTable;
        private System.Data.DataColumn description;
        private System.Data.DataColumn settingsColumn;
        private System.Windows.Forms.Panel offsetPanel;
        private System.Windows.Forms.Label fsuipcTypeLabel;
        private System.Windows.Forms.ComboBox fsuipcOffsetTypeComboBox;
        private System.Windows.Forms.Label fsuipcSizeLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox fsuipcSizeComboBox;
        private System.Windows.Forms.TextBox fsuipcOffsetTextBox;
        private System.Windows.Forms.Panel maskAndBcdPanel;
        private System.Windows.Forms.Button maskEditorButton;
        private System.Windows.Forms.CheckBox fsuipcBcdModeCheckBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox fsuipcMaskTextBox;
        private TransformOptionsGroup transformOptionsGroup1;
        private System.Windows.Forms.Panel panelModifierHint;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonModifyTab;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelModifierHint;
    }
}
