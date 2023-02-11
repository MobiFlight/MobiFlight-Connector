namespace MobiFlight.UI.Dialogs
{
    partial class InputConfigWizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputConfigWizard));
            this.MainPanel = new System.Windows.Forms.Panel();
            this.tabControlFsuipc = new System.Windows.Forms.TabControl();
            this.preconditionTabPage = new System.Windows.Forms.TabPage();
            this.preconditionPanel = new MobiFlight.UI.Panels.Config.PreconditionPanel();
            this.configRefTabPage = new System.Windows.Forms.TabPage();
            this.configRefPanel = new MobiFlight.UI.Panels.Config.ConfigRefPanel();
            this.displayTabPage = new System.Windows.Forms.TabPage();
            this.groupBoxInputSettings = new System.Windows.Forms.GroupBox();
            this.displayTypeGroupBox = new System.Windows.Forms.GroupBox();
            this.ScanForInputButton = new System.Windows.Forms.Button();
            this.DeviceNotAvailableWarningLabel = new System.Windows.Forms.Label();
            this.inputPinDropDown = new System.Windows.Forms.ComboBox();
            this.arcazeSerialLabel = new System.Windows.Forms.Label();
            this.inputModuleNameComboBox = new System.Windows.Forms.ComboBox();
            this.inputTypeComboBoxLabel = new System.Windows.Forms.Label();
            this.inputTypeComboBox = new System.Windows.Forms.ComboBox();
            this.displayTabTextBox = new System.Windows.Forms.TextBox();
            this.ButtonPanel = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.presetsDataSet = new System.Data.DataSet();
            this.presetDataTable = new System.Data.DataTable();
            this.description = new System.Data.DataColumn();
            this.settingsColumn = new System.Data.DataColumn();
            this.MainPanel.SuspendLayout();
            this.tabControlFsuipc.SuspendLayout();
            this.preconditionTabPage.SuspendLayout();
            this.configRefTabPage.SuspendLayout();
            this.displayTabPage.SuspendLayout();
            this.displayTypeGroupBox.SuspendLayout();
            this.ButtonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.presetsDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.presetDataTable)).BeginInit();
            this.SuspendLayout();
            // 
            // MainPanel
            // 
            this.MainPanel.Controls.Add(this.tabControlFsuipc);
            resources.ApplyResources(this.MainPanel, "MainPanel");
            this.MainPanel.Name = "MainPanel";
            // 
            // tabControlFsuipc
            // 
            this.tabControlFsuipc.Controls.Add(this.displayTabPage);
            this.tabControlFsuipc.Controls.Add(this.preconditionTabPage);
            this.tabControlFsuipc.Controls.Add(this.configRefTabPage);
            resources.ApplyResources(this.tabControlFsuipc, "tabControlFsuipc");
            this.tabControlFsuipc.Name = "tabControlFsuipc";
            this.tabControlFsuipc.SelectedIndex = 0;
            this.tabControlFsuipc.SelectedIndexChanged += new System.EventHandler(this.tabControlFsuipc_SelectedIndexChanged);
            // 
            // preconditionTabPage
            // 
            this.preconditionTabPage.Controls.Add(this.preconditionPanel);
            resources.ApplyResources(this.preconditionTabPage, "preconditionTabPage");
            this.preconditionTabPage.Name = "preconditionTabPage";
            this.preconditionTabPage.UseVisualStyleBackColor = true;
            // 
            // preconditionPanel
            // 
            resources.ApplyResources(this.preconditionPanel, "preconditionPanel");
            this.preconditionPanel.Name = "preconditionPanel";
            // 
            // configRefTabPage
            // 
            this.configRefTabPage.Controls.Add(this.configRefPanel);
            resources.ApplyResources(this.configRefTabPage, "configRefTabPage");
            this.configRefTabPage.Name = "configRefTabPage";
            this.configRefTabPage.UseVisualStyleBackColor = true;
            // 
            // configRefPanel
            // 
            resources.ApplyResources(this.configRefPanel, "configRefPanel");
            this.configRefPanel.Name = "configRefPanel";
            // 
            // displayTabPage
            // 
            this.displayTabPage.Controls.Add(this.groupBoxInputSettings);
            this.displayTabPage.Controls.Add(this.displayTypeGroupBox);
            this.displayTabPage.Controls.Add(this.displayTabTextBox);
            resources.ApplyResources(this.displayTabPage, "displayTabPage");
            this.displayTabPage.Name = "displayTabPage";
            this.displayTabPage.UseVisualStyleBackColor = true;
            // 
            // groupBoxInputSettings
            // 
            resources.ApplyResources(this.groupBoxInputSettings, "groupBoxInputSettings");
            this.groupBoxInputSettings.Name = "groupBoxInputSettings";
            this.groupBoxInputSettings.TabStop = false;
            // 
            // displayTypeGroupBox
            // 
            this.displayTypeGroupBox.Controls.Add(this.ScanForInputButton);
            this.displayTypeGroupBox.Controls.Add(this.DeviceNotAvailableWarningLabel);
            this.displayTypeGroupBox.Controls.Add(this.inputPinDropDown);
            this.displayTypeGroupBox.Controls.Add(this.arcazeSerialLabel);
            this.displayTypeGroupBox.Controls.Add(this.inputModuleNameComboBox);
            this.displayTypeGroupBox.Controls.Add(this.inputTypeComboBoxLabel);
            this.displayTypeGroupBox.Controls.Add(this.inputTypeComboBox);
            resources.ApplyResources(this.displayTypeGroupBox, "displayTypeGroupBox");
            this.displayTypeGroupBox.Name = "displayTypeGroupBox";
            this.displayTypeGroupBox.TabStop = false;
            // 
            // ScanForInputButton
            // 
            resources.ApplyResources(this.ScanForInputButton, "ScanForInputButton");
            this.ScanForInputButton.Name = "ScanForInputButton";
            this.ScanForInputButton.UseVisualStyleBackColor = true;
            this.ScanForInputButton.Click += new System.EventHandler(this.ScanForInputButton_Click);
            // 
            // DeviceNotAvailableWarningLabel
            // 
            resources.ApplyResources(this.DeviceNotAvailableWarningLabel, "DeviceNotAvailableWarningLabel");
            this.DeviceNotAvailableWarningLabel.Name = "DeviceNotAvailableWarningLabel";
            // 
            // inputPinDropDown
            // 
            this.inputPinDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inputPinDropDown.FormattingEnabled = true;
            resources.ApplyResources(this.inputPinDropDown, "inputPinDropDown");
            this.inputPinDropDown.Name = "inputPinDropDown";
            // 
            // arcazeSerialLabel
            // 
            resources.ApplyResources(this.arcazeSerialLabel, "arcazeSerialLabel");
            this.arcazeSerialLabel.Name = "arcazeSerialLabel";
            // 
            // inputModuleNameComboBox
            // 
            this.inputModuleNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inputModuleNameComboBox.DropDownWidth = 300;
            this.inputModuleNameComboBox.FormattingEnabled = true;
            this.inputModuleNameComboBox.Items.AddRange(new object[] {
            resources.GetString("inputModuleNameComboBox.Items"),
            resources.GetString("inputModuleNameComboBox.Items1")});
            resources.ApplyResources(this.inputModuleNameComboBox, "inputModuleNameComboBox");
            this.inputModuleNameComboBox.Name = "inputModuleNameComboBox";
            this.inputModuleNameComboBox.SelectedIndexChanged += new System.EventHandler(this.ModuleSerialComboBox_SelectedIndexChanged);
            this.inputModuleNameComboBox.Validating += new System.ComponentModel.CancelEventHandler(this.displayArcazeSerialComboBox_Validating);
            // 
            // inputTypeComboBoxLabel
            // 
            resources.ApplyResources(this.inputTypeComboBoxLabel, "inputTypeComboBoxLabel");
            this.inputTypeComboBoxLabel.Name = "inputTypeComboBoxLabel";
            // 
            // inputTypeComboBox
            // 
            this.inputTypeComboBox.DisplayMember = "Name";
            this.inputTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inputTypeComboBox.FormattingEnabled = true;
            this.inputTypeComboBox.Items.AddRange(new object[] {
            resources.GetString("inputTypeComboBox.Items"),
            resources.GetString("inputTypeComboBox.Items1"),
            resources.GetString("inputTypeComboBox.Items2")});
            resources.ApplyResources(this.inputTypeComboBox, "inputTypeComboBox");
            this.inputTypeComboBox.Name = "inputTypeComboBox";
            this.inputTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.inputTypeComboBox_SelectedIndexChanged);
            // 
            // displayTabTextBox
            // 
            this.displayTabTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.displayTabTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.displayTabTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.displayTabTextBox, "displayTabTextBox");
            this.displayTabTextBox.Name = "displayTabTextBox";
            this.displayTabTextBox.ReadOnly = true;
            this.displayTabTextBox.TabStop = false;
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.Controls.Add(this.button1);
            this.ButtonPanel.Controls.Add(this.cancelButton);
            resources.ApplyResources(this.ButtonPanel, "ButtonPanel");
            this.ButtonPanel.Name = "ButtonPanel";
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
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
            // InputConfigWizard
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.MainPanel);
            this.Controls.Add(this.ButtonPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "InputConfigWizard";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InputConfigWizard_FormClosing);
            this.Load += new System.EventHandler(this.ConfigWizard_Load);
            this.Shown += new System.EventHandler(this.InputConfigWizard_Shown);
            this.MainPanel.ResumeLayout(false);
            this.tabControlFsuipc.ResumeLayout(false);
            this.preconditionTabPage.ResumeLayout(false);
            this.configRefTabPage.ResumeLayout(false);
            this.displayTabPage.ResumeLayout(false);
            this.displayTabPage.PerformLayout();
            this.displayTypeGroupBox.ResumeLayout(false);
            this.ButtonPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.presetsDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.presetDataTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel MainPanel;
        private System.Windows.Forms.Panel ButtonPanel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button cancelButton;
        private System.Data.DataSet presetsDataSet;
        private System.Data.DataTable presetDataTable;
        private System.Data.DataColumn description;
        private System.Data.DataColumn settingsColumn;
        private System.Windows.Forms.TabControl tabControlFsuipc;
        private System.Windows.Forms.TabPage preconditionTabPage;
        private System.Windows.Forms.TabPage displayTabPage;
        private System.Windows.Forms.GroupBox groupBoxInputSettings;
        private System.Windows.Forms.GroupBox displayTypeGroupBox;
        private System.Windows.Forms.Label arcazeSerialLabel;
        private System.Windows.Forms.ComboBox inputModuleNameComboBox;
        private System.Windows.Forms.Label inputTypeComboBoxLabel;
        private System.Windows.Forms.ComboBox inputTypeComboBox;
        private System.Windows.Forms.TextBox displayTabTextBox;
        private System.Windows.Forms.TabPage configRefTabPage;
        private Panels.Config.ConfigRefPanel configRefPanel;
        private System.Windows.Forms.ComboBox inputPinDropDown;
        private Panels.Config.PreconditionPanel preconditionPanel;
        private System.Windows.Forms.Label DeviceNotAvailableWarningLabel;
        private System.Windows.Forms.Button ScanForInputButton;
    }
}