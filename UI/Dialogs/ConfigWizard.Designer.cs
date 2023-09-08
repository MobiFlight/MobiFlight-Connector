namespace MobiFlight.UI.Dialogs
{
    partial class ConfigWizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigWizard));
            this.MainPanel = new System.Windows.Forms.Panel();
            this.tabControlFsuipc = new System.Windows.Forms.TabControl();
            this.fsuipcTabPage = new System.Windows.Forms.TabPage();
            this.xplaneDataRefPanel1 = new MobiFlight.UI.Panels.Config.XplaneDataRefPanel();
            this.variablePanel1 = new MobiFlight.UI.Panels.Config.VariablePanel();
            this.simConnectPanel1 = new MobiFlight.UI.Panels.Config.SimConnectPanel();
            this.FsuipcSettingsPanel = new System.Windows.Forms.Panel();
            this.fsuipcConfigPanel = new MobiFlight.UI.Panels.Config.FsuipcConfigPanel();
            this.fsuipcHintLabel = new System.Windows.Forms.Label();
            this.OffsetTypePanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.OffsetTypeVariableRadioButton = new System.Windows.Forms.RadioButton();
            this.OffsetTypeSimConnectRadioButton = new System.Windows.Forms.RadioButton();
            this.OffsetTypeFsuipRadioButton = new System.Windows.Forms.RadioButton();
            this.OffsetTypeXplaneRadioButton = new System.Windows.Forms.RadioButton();
            this.compareTabPage = new System.Windows.Forms.TabPage();
            this.modifierPanel1 = new MobiFlight.UI.Panels.OutputWizard.ModifierPanel();
            this.referencesGroupBox = new System.Windows.Forms.GroupBox();
            this.configRefPanel = new MobiFlight.UI.Panels.Config.ConfigRefPanel();
            this.displayTabPage = new System.Windows.Forms.TabPage();
            this.displayPanel1 = new MobiFlight.UI.Panels.OutputWizard.DisplayPanel();
            this.preconditionTabPage = new System.Windows.Forms.TabPage();
            this.preconditionPanel = new MobiFlight.UI.Panels.Config.PreconditionPanel();
            this.ButtonPanel = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.presetsDataSet = new System.Data.DataSet();
            this.presetDataTable = new System.Data.DataTable();
            this.description = new System.Data.DataColumn();
            this.settingsColumn = new System.Data.DataColumn();
            this.testValuePanel1 = new MobiFlight.UI.Panels.Config.TestValuePanel();
            this.MainPanel.SuspendLayout();
            this.tabControlFsuipc.SuspendLayout();
            this.fsuipcTabPage.SuspendLayout();
            this.FsuipcSettingsPanel.SuspendLayout();
            this.OffsetTypePanel.SuspendLayout();
            this.compareTabPage.SuspendLayout();
            this.referencesGroupBox.SuspendLayout();
            this.displayTabPage.SuspendLayout();
            this.preconditionTabPage.SuspendLayout();
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
            this.tabControlFsuipc.Controls.Add(this.fsuipcTabPage);
            this.tabControlFsuipc.Controls.Add(this.compareTabPage);
            this.tabControlFsuipc.Controls.Add(this.displayTabPage);
            this.tabControlFsuipc.Controls.Add(this.preconditionTabPage);
            resources.ApplyResources(this.tabControlFsuipc, "tabControlFsuipc");
            this.tabControlFsuipc.Name = "tabControlFsuipc";
            this.tabControlFsuipc.SelectedIndex = 0;
            this.tabControlFsuipc.SelectedIndexChanged += new System.EventHandler(this.tabControlFsuipc_SelectedIndexChanged);
            // 
            // fsuipcTabPage
            // 
            this.fsuipcTabPage.Controls.Add(this.xplaneDataRefPanel1);
            this.fsuipcTabPage.Controls.Add(this.variablePanel1);
            this.fsuipcTabPage.Controls.Add(this.simConnectPanel1);
            this.fsuipcTabPage.Controls.Add(this.FsuipcSettingsPanel);
            this.fsuipcTabPage.Controls.Add(this.OffsetTypePanel);
            resources.ApplyResources(this.fsuipcTabPage, "fsuipcTabPage");
            this.fsuipcTabPage.Name = "fsuipcTabPage";
            this.fsuipcTabPage.UseVisualStyleBackColor = true;
            // 
            // xplaneDataRefPanel1
            // 
            resources.ApplyResources(this.xplaneDataRefPanel1, "xplaneDataRefPanel1");
            this.xplaneDataRefPanel1.Name = "xplaneDataRefPanel1";
            // 
            // variablePanel1
            // 
            resources.ApplyResources(this.variablePanel1, "variablePanel1");
            this.variablePanel1.Name = "variablePanel1";
            // 
            // simConnectPanel1
            // 
            resources.ApplyResources(this.simConnectPanel1, "simConnectPanel1");
            this.simConnectPanel1.Name = "simConnectPanel1";
            // 
            // FsuipcSettingsPanel
            // 
            this.FsuipcSettingsPanel.Controls.Add(this.fsuipcConfigPanel);
            this.FsuipcSettingsPanel.Controls.Add(this.fsuipcHintLabel);
            resources.ApplyResources(this.FsuipcSettingsPanel, "FsuipcSettingsPanel");
            this.FsuipcSettingsPanel.Name = "FsuipcSettingsPanel";
            // 
            // fsuipcConfigPanel
            // 
            resources.ApplyResources(this.fsuipcConfigPanel, "fsuipcConfigPanel");
            this.fsuipcConfigPanel.Modifier = null;
            this.fsuipcConfigPanel.Name = "fsuipcConfigPanel";
            this.fsuipcConfigPanel.PresetFile = "";
            // 
            // fsuipcHintLabel
            // 
            resources.ApplyResources(this.fsuipcHintLabel, "fsuipcHintLabel");
            this.fsuipcHintLabel.Name = "fsuipcHintLabel";
            // 
            // OffsetTypePanel
            // 
            this.OffsetTypePanel.Controls.Add(this.label1);
            this.OffsetTypePanel.Controls.Add(this.OffsetTypeVariableRadioButton);
            this.OffsetTypePanel.Controls.Add(this.OffsetTypeSimConnectRadioButton);
            this.OffsetTypePanel.Controls.Add(this.OffsetTypeFsuipRadioButton);
            this.OffsetTypePanel.Controls.Add(this.OffsetTypeXplaneRadioButton);
            resources.ApplyResources(this.OffsetTypePanel, "OffsetTypePanel");
            this.OffsetTypePanel.Name = "OffsetTypePanel";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // OffsetTypeVariableRadioButton
            // 
            resources.ApplyResources(this.OffsetTypeVariableRadioButton, "OffsetTypeVariableRadioButton");
            this.OffsetTypeVariableRadioButton.Name = "OffsetTypeVariableRadioButton";
            this.OffsetTypeVariableRadioButton.TabStop = true;
            this.OffsetTypeVariableRadioButton.UseVisualStyleBackColor = true;
            this.OffsetTypeVariableRadioButton.CheckedChanged += new System.EventHandler(this.OffsetTypeFsuipRadioButton_CheckedChanged);
            // 
            // OffsetTypeSimConnectRadioButton
            // 
            resources.ApplyResources(this.OffsetTypeSimConnectRadioButton, "OffsetTypeSimConnectRadioButton");
            this.OffsetTypeSimConnectRadioButton.Name = "OffsetTypeSimConnectRadioButton";
            this.OffsetTypeSimConnectRadioButton.TabStop = true;
            this.OffsetTypeSimConnectRadioButton.UseVisualStyleBackColor = true;
            this.OffsetTypeSimConnectRadioButton.CheckedChanged += new System.EventHandler(this.OffsetTypeFsuipRadioButton_CheckedChanged);
            // 
            // OffsetTypeFsuipRadioButton
            // 
            resources.ApplyResources(this.OffsetTypeFsuipRadioButton, "OffsetTypeFsuipRadioButton");
            this.OffsetTypeFsuipRadioButton.Name = "OffsetTypeFsuipRadioButton";
            this.OffsetTypeFsuipRadioButton.TabStop = true;
            this.OffsetTypeFsuipRadioButton.UseVisualStyleBackColor = true;
            this.OffsetTypeFsuipRadioButton.CheckedChanged += new System.EventHandler(this.OffsetTypeFsuipRadioButton_CheckedChanged);
            // 
            // OffsetTypeXplaneRadioButton
            // 
            resources.ApplyResources(this.OffsetTypeXplaneRadioButton, "OffsetTypeXplaneRadioButton");
            this.OffsetTypeXplaneRadioButton.Name = "OffsetTypeXplaneRadioButton";
            this.OffsetTypeXplaneRadioButton.TabStop = true;
            this.OffsetTypeXplaneRadioButton.UseVisualStyleBackColor = true;
            this.OffsetTypeXplaneRadioButton.CheckedChanged += new System.EventHandler(this.OffsetTypeFsuipRadioButton_CheckedChanged);
            // 
            // compareTabPage
            // 
            this.compareTabPage.Controls.Add(this.modifierPanel1);
            this.compareTabPage.Controls.Add(this.referencesGroupBox);
            resources.ApplyResources(this.compareTabPage, "compareTabPage");
            this.compareTabPage.Name = "compareTabPage";
            this.compareTabPage.UseVisualStyleBackColor = true;
            // 
            // modifierPanel1
            // 
            resources.ApplyResources(this.modifierPanel1, "modifierPanel1");
            this.modifierPanel1.Name = "modifierPanel1";
            // 
            // referencesGroupBox
            // 
            this.referencesGroupBox.Controls.Add(this.configRefPanel);
            resources.ApplyResources(this.referencesGroupBox, "referencesGroupBox");
            this.referencesGroupBox.Name = "referencesGroupBox";
            this.referencesGroupBox.TabStop = false;
            // 
            // configRefPanel
            // 
            resources.ApplyResources(this.configRefPanel, "configRefPanel");
            this.configRefPanel.Name = "configRefPanel";
            // 
            // displayTabPage
            // 
            this.displayTabPage.Controls.Add(this.displayPanel1);
            resources.ApplyResources(this.displayTabPage, "displayTabPage");
            this.displayTabPage.Name = "displayTabPage";
            this.displayTabPage.UseVisualStyleBackColor = true;
            // 
            // displayPanel1
            // 
            resources.ApplyResources(this.displayPanel1, "displayPanel1");
            this.displayPanel1.Name = "displayPanel1";
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
            this.button1.Click += new System.EventHandler(this.button1_Click);
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
            // testValuePanel1
            // 
            resources.ApplyResources(this.testValuePanel1, "testValuePanel1");
            this.testValuePanel1.Name = "testValuePanel1";
            // 
            // ConfigWizard
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.MainPanel);
            this.Controls.Add(this.testValuePanel1);
            this.Controls.Add(this.ButtonPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "ConfigWizard";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConfigWizard_FormClosing);
            this.Load += new System.EventHandler(this.ConfigWizard_Load);
            this.MainPanel.ResumeLayout(false);
            this.tabControlFsuipc.ResumeLayout(false);
            this.fsuipcTabPage.ResumeLayout(false);
            this.fsuipcTabPage.PerformLayout();
            this.FsuipcSettingsPanel.ResumeLayout(false);
            this.OffsetTypePanel.ResumeLayout(false);
            this.OffsetTypePanel.PerformLayout();
            this.compareTabPage.ResumeLayout(false);
            this.referencesGroupBox.ResumeLayout(false);
            this.displayTabPage.ResumeLayout(false);
            this.preconditionTabPage.ResumeLayout(false);
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
        private System.Windows.Forms.TabControl tabControlFsuipc;
        private System.Windows.Forms.TabPage fsuipcTabPage;
        private System.Windows.Forms.TabPage compareTabPage;
        private System.Windows.Forms.TabPage displayTabPage;
        private System.Data.DataSet presetsDataSet;
        private System.Data.DataTable presetDataTable;
        private System.Data.DataColumn description;
        private System.Data.DataColumn settingsColumn;
        //private System.Windows.Forms.Panel displayBcdPanel;
        private System.Windows.Forms.TabPage preconditionTabPage;
        private UI.Panels.Config.FsuipcConfigPanel fsuipcConfigPanel;
        private System.Windows.Forms.GroupBox referencesGroupBox;
        private UI.Panels.Config.ConfigRefPanel configRefPanel;
        private System.Windows.Forms.Panel FsuipcSettingsPanel;
        private System.Windows.Forms.Panel OffsetTypePanel;
        private System.Windows.Forms.RadioButton OffsetTypeSimConnectRadioButton;
        private System.Windows.Forms.RadioButton OffsetTypeFsuipRadioButton;
        private Panels.Config.SimConnectPanel simConnectPanel1;
        private System.Windows.Forms.RadioButton OffsetTypeVariableRadioButton;
        private Panels.Config.VariablePanel variablePanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label fsuipcHintLabel;
        private Panels.Config.PreconditionPanel preconditionPanel;
        private Panels.OutputWizard.DisplayPanel displayPanel1;
        private System.Windows.Forms.RadioButton OffsetTypeXplaneRadioButton;
        private Panels.Config.XplaneDataRefPanel xplaneDataRefPanel1;
        private Panels.OutputWizard.ModifierPanel modifierPanel1;
        private Panels.Config.TestValuePanel testValuePanel1;
    }
}