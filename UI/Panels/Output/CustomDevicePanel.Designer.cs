namespace MobiFlight.UI.Panels
{
    partial class CustomDevicePanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomDevicePanel));
            this.displayPinComoBoxLabel = new System.Windows.Forms.Label();
            this.valueTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.MessageTypeComboBox = new System.Windows.Forms.ComboBox();
            this.MessageDescriptionLabel = new System.Windows.Forms.Label();
            this.deviceLabel = new System.Windows.Forms.Label();
            this.customDeviceNamesComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // displayPinComoBoxLabel
            // 
            resources.ApplyResources(this.displayPinComoBoxLabel, "displayPinComoBoxLabel");
            this.displayPinComoBoxLabel.Name = "displayPinComoBoxLabel";
            // 
            // valueTextBox
            // 
            resources.ApplyResources(this.valueTextBox, "valueTextBox");
            this.valueTextBox.Name = "valueTextBox";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // MessageTypeComboBox
            // 
            resources.ApplyResources(this.MessageTypeComboBox, "MessageTypeComboBox");
            this.MessageTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MessageTypeComboBox.FormattingEnabled = true;
            this.MessageTypeComboBox.Items.AddRange(new object[] {
            resources.GetString("MessageTypeComboBox.Items"),
            resources.GetString("MessageTypeComboBox.Items1"),
            resources.GetString("MessageTypeComboBox.Items2")});
            this.MessageTypeComboBox.Name = "MessageTypeComboBox";
            this.MessageTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.MessageTypeComboBox_SelectedIndexChanged);
            // 
            // MessageDescriptionLabel
            // 
            resources.ApplyResources(this.MessageDescriptionLabel, "MessageDescriptionLabel");
            this.MessageDescriptionLabel.Name = "MessageDescriptionLabel";
            // 
            // deviceLabel
            // 
            resources.ApplyResources(this.deviceLabel, "deviceLabel");
            this.deviceLabel.Name = "deviceLabel";
            // 
            // addressesComboBox
            // 
            this.customDeviceNamesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.customDeviceNamesComboBox.FormattingEnabled = true;
            this.customDeviceNamesComboBox.Items.AddRange(new object[] {
            resources.GetString("addressesComboBox.Items"),
            resources.GetString("addressesComboBox.Items1"),
            resources.GetString("addressesComboBox.Items2")});
            resources.ApplyResources(this.customDeviceNamesComboBox, "addressesComboBox");
            this.customDeviceNamesComboBox.Name = "addressesComboBox";
            this.customDeviceNamesComboBox.SelectedValueChanged += new System.EventHandler(this.customDeviceNameComboBox_SelectedValueChanged);
            // 
            // CustomDevicePanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.deviceLabel);
            this.Controls.Add(this.customDeviceNamesComboBox);
            this.Controls.Add(this.MessageDescriptionLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.MessageTypeComboBox);
            this.Controls.Add(this.valueTextBox);
            this.Controls.Add(this.displayPinComoBoxLabel);
            this.Name = "CustomDevicePanel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label displayPinComoBoxLabel;
        public System.Windows.Forms.TextBox valueTextBox;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox MessageTypeComboBox;
        private System.Windows.Forms.Label MessageDescriptionLabel;
        private System.Windows.Forms.Label deviceLabel;
        public System.Windows.Forms.ComboBox customDeviceNamesComboBox;
    }
}
