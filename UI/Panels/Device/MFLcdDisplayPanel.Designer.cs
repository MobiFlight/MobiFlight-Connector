namespace MobiFlight.UI.Panels.Settings.Device
{
    partial class MFLcddDisplayPanel
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MFLcddDisplayPanel));
            this.DiksplaySettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.AddressComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.LinesTextBox = new System.Windows.Forms.TextBox();
            this.ColTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.NameGroupBox = new System.Windows.Forms.GroupBox();
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.DiksplaySettingsGroupBox.SuspendLayout();
            this.NameGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // DiksplaySettingsGroupBox
            // 
            this.DiksplaySettingsGroupBox.Controls.Add(this.AddressComboBox);
            this.DiksplaySettingsGroupBox.Controls.Add(this.label3);
            this.DiksplaySettingsGroupBox.Controls.Add(this.LinesTextBox);
            this.DiksplaySettingsGroupBox.Controls.Add(this.ColTextBox);
            this.DiksplaySettingsGroupBox.Controls.Add(this.label2);
            this.DiksplaySettingsGroupBox.Controls.Add(this.label1);
            resources.ApplyResources(this.DiksplaySettingsGroupBox, "DiksplaySettingsGroupBox");
            this.DiksplaySettingsGroupBox.Name = "DiksplaySettingsGroupBox";
            this.DiksplaySettingsGroupBox.TabStop = false;
            // 
            // AddressComboBox
            // 
            this.AddressComboBox.AutoCompleteCustomSource.AddRange(new string[] {
            resources.GetString("AddressComboBox.AutoCompleteCustomSource"),
            resources.GetString("AddressComboBox.AutoCompleteCustomSource1"),
            resources.GetString("AddressComboBox.AutoCompleteCustomSource2"),
            resources.GetString("AddressComboBox.AutoCompleteCustomSource3"),
            resources.GetString("AddressComboBox.AutoCompleteCustomSource4"),
            resources.GetString("AddressComboBox.AutoCompleteCustomSource5"),
            resources.GetString("AddressComboBox.AutoCompleteCustomSource6"),
            resources.GetString("AddressComboBox.AutoCompleteCustomSource7"),
            resources.GetString("AddressComboBox.AutoCompleteCustomSource8"),
            resources.GetString("AddressComboBox.AutoCompleteCustomSource9"),
            resources.GetString("AddressComboBox.AutoCompleteCustomSource10"),
            resources.GetString("AddressComboBox.AutoCompleteCustomSource11"),
            resources.GetString("AddressComboBox.AutoCompleteCustomSource12"),
            resources.GetString("AddressComboBox.AutoCompleteCustomSource13"),
            resources.GetString("AddressComboBox.AutoCompleteCustomSource14"),
            resources.GetString("AddressComboBox.AutoCompleteCustomSource15")});
            this.AddressComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.AddressComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AddressComboBox.FormattingEnabled = true;
            this.AddressComboBox.Items.AddRange(new object[] {
            resources.GetString("AddressComboBox.Items"),
            resources.GetString("AddressComboBox.Items1"),
            resources.GetString("AddressComboBox.Items2"),
            resources.GetString("AddressComboBox.Items3"),
            resources.GetString("AddressComboBox.Items4"),
            resources.GetString("AddressComboBox.Items5"),
            resources.GetString("AddressComboBox.Items6"),
            resources.GetString("AddressComboBox.Items7"),
            resources.GetString("AddressComboBox.Items8"),
            resources.GetString("AddressComboBox.Items9"),
            resources.GetString("AddressComboBox.Items10"),
            resources.GetString("AddressComboBox.Items11"),
            resources.GetString("AddressComboBox.Items12"),
            resources.GetString("AddressComboBox.Items13"),
            resources.GetString("AddressComboBox.Items14"),
            resources.GetString("AddressComboBox.Items15")});
            resources.ApplyResources(this.AddressComboBox, "AddressComboBox");
            this.AddressComboBox.Name = "AddressComboBox";
            this.AddressComboBox.SelectedValueChanged += new System.EventHandler(this.AddressComboBox_SelectedValueChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // LinesTextBox
            // 
            resources.ApplyResources(this.LinesTextBox, "LinesTextBox");
            this.LinesTextBox.Name = "LinesTextBox";
            this.LinesTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.ColTextBox_Validating);
            // 
            // ColTextBox
            // 
            resources.ApplyResources(this.ColTextBox, "ColTextBox");
            this.ColTextBox.Name = "ColTextBox";
            this.ColTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.ColTextBox_Validating);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // NameGroupBox
            // 
            this.NameGroupBox.Controls.Add(this.NameTextBox);
            resources.ApplyResources(this.NameGroupBox, "NameGroupBox");
            this.NameGroupBox.Name = "NameGroupBox";
            this.NameGroupBox.TabStop = false;
            // 
            // NameTextBox
            // 
            resources.ApplyResources(this.NameTextBox, "NameTextBox");
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.TextChanged += new System.EventHandler(this.value_Changed);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // MFLcddDisplayPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.Controls.Add(this.NameGroupBox);
            this.Controls.Add(this.DiksplaySettingsGroupBox);
            this.Name = "MFLcddDisplayPanel";
            this.DiksplaySettingsGroupBox.ResumeLayout(false);
            this.DiksplaySettingsGroupBox.PerformLayout();
            this.NameGroupBox.ResumeLayout(false);
            this.NameGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox DiksplaySettingsGroupBox;
        private System.Windows.Forms.GroupBox NameGroupBox;
        private System.Windows.Forms.TextBox NameTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox LinesTextBox;
        private System.Windows.Forms.TextBox ColTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.ComboBox AddressComboBox;
    }
}
