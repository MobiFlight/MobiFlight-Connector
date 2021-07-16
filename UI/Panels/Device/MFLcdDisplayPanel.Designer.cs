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
            resources.ApplyResources(this.DiksplaySettingsGroupBox, "DiksplaySettingsGroupBox");
            this.DiksplaySettingsGroupBox.Controls.Add(this.AddressComboBox);
            this.DiksplaySettingsGroupBox.Controls.Add(this.label3);
            this.DiksplaySettingsGroupBox.Controls.Add(this.LinesTextBox);
            this.DiksplaySettingsGroupBox.Controls.Add(this.ColTextBox);
            this.DiksplaySettingsGroupBox.Controls.Add(this.label2);
            this.DiksplaySettingsGroupBox.Controls.Add(this.label1);
            this.errorProvider.SetError(this.DiksplaySettingsGroupBox, resources.GetString("DiksplaySettingsGroupBox.Error"));
            this.errorProvider.SetIconAlignment(this.DiksplaySettingsGroupBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("DiksplaySettingsGroupBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.DiksplaySettingsGroupBox, ((int)(resources.GetObject("DiksplaySettingsGroupBox.IconPadding"))));
            this.DiksplaySettingsGroupBox.Name = "DiksplaySettingsGroupBox";
            this.DiksplaySettingsGroupBox.TabStop = false;
            // 
            // AddressComboBox
            // 
            resources.ApplyResources(this.AddressComboBox, "AddressComboBox");
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
            this.errorProvider.SetError(this.AddressComboBox, resources.GetString("AddressComboBox.Error"));
            this.AddressComboBox.FormattingEnabled = true;
            this.errorProvider.SetIconAlignment(this.AddressComboBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("AddressComboBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.AddressComboBox, ((int)(resources.GetObject("AddressComboBox.IconPadding"))));
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
            this.AddressComboBox.Name = "AddressComboBox";
            this.AddressComboBox.SelectedValueChanged += new System.EventHandler(this.AddressComboBox_SelectedValueChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.errorProvider.SetError(this.label3, resources.GetString("label3.Error"));
            this.errorProvider.SetIconAlignment(this.label3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label3.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label3, ((int)(resources.GetObject("label3.IconPadding"))));
            this.label3.Name = "label3";
            // 
            // LinesTextBox
            // 
            resources.ApplyResources(this.LinesTextBox, "LinesTextBox");
            this.errorProvider.SetError(this.LinesTextBox, resources.GetString("LinesTextBox.Error"));
            this.errorProvider.SetIconAlignment(this.LinesTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("LinesTextBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.LinesTextBox, ((int)(resources.GetObject("LinesTextBox.IconPadding"))));
            this.LinesTextBox.Name = "LinesTextBox";
            this.LinesTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.ColTextBox_Validating);
            // 
            // ColTextBox
            // 
            resources.ApplyResources(this.ColTextBox, "ColTextBox");
            this.errorProvider.SetError(this.ColTextBox, resources.GetString("ColTextBox.Error"));
            this.errorProvider.SetIconAlignment(this.ColTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("ColTextBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.ColTextBox, ((int)(resources.GetObject("ColTextBox.IconPadding"))));
            this.ColTextBox.Name = "ColTextBox";
            this.ColTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.ColTextBox_Validating);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.errorProvider.SetError(this.label2, resources.GetString("label2.Error"));
            this.errorProvider.SetIconAlignment(this.label2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label2.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label2, ((int)(resources.GetObject("label2.IconPadding"))));
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.errorProvider.SetError(this.label1, resources.GetString("label1.Error"));
            this.errorProvider.SetIconAlignment(this.label1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label1.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label1, ((int)(resources.GetObject("label1.IconPadding"))));
            this.label1.Name = "label1";
            // 
            // NameGroupBox
            // 
            resources.ApplyResources(this.NameGroupBox, "NameGroupBox");
            this.NameGroupBox.Controls.Add(this.NameTextBox);
            this.errorProvider.SetError(this.NameGroupBox, resources.GetString("NameGroupBox.Error"));
            this.errorProvider.SetIconAlignment(this.NameGroupBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("NameGroupBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.NameGroupBox, ((int)(resources.GetObject("NameGroupBox.IconPadding"))));
            this.NameGroupBox.Name = "NameGroupBox";
            this.NameGroupBox.TabStop = false;
            // 
            // NameTextBox
            // 
            resources.ApplyResources(this.NameTextBox, "NameTextBox");
            this.errorProvider.SetError(this.NameTextBox, resources.GetString("NameTextBox.Error"));
            this.errorProvider.SetIconAlignment(this.NameTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("NameTextBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.NameTextBox, ((int)(resources.GetObject("NameTextBox.IconPadding"))));
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.TextChanged += new System.EventHandler(this.value_Changed);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            resources.ApplyResources(this.errorProvider, "errorProvider");
            // 
            // MFLcddDisplayPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.Controls.Add(this.NameGroupBox);
            this.Controls.Add(this.DiksplaySettingsGroupBox);
            this.errorProvider.SetError(this, resources.GetString("$this.Error"));
            this.errorProvider.SetIconAlignment(this, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("$this.IconAlignment"))));
            this.errorProvider.SetIconPadding(this, ((int)(resources.GetObject("$this.IconPadding"))));
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
