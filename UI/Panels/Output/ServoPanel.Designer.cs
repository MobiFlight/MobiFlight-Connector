namespace MobiFlight.UI.Panels
{
    partial class ServoPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServoPanel));
            this.displayPinComoBoxLabel = new System.Windows.Forms.Label();
            this.minValueTextBox = new System.Windows.Forms.TextBox();
            this.maxValueTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.servoAddressesComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.maxRotationPercentNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.maxRotationPercentNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // displayPinComoBoxLabel
            // 
            resources.ApplyResources(this.displayPinComoBoxLabel, "displayPinComoBoxLabel");
            this.displayPinComoBoxLabel.Name = "displayPinComoBoxLabel";
            // 
            // minValueTextBox
            // 
            resources.ApplyResources(this.minValueTextBox, "minValueTextBox");
            this.minValueTextBox.Name = "minValueTextBox";
            // 
            // maxValueTextBox
            // 
            resources.ApplyResources(this.maxValueTextBox, "maxValueTextBox");
            this.maxValueTextBox.Name = "maxValueTextBox";
            this.maxValueTextBox.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // servoAddressesComboBox
            // 
            this.servoAddressesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.servoAddressesComboBox.FormattingEnabled = true;
            this.servoAddressesComboBox.Items.AddRange(new object[] {
            resources.GetString("servoAddressesComboBox.Items"),
            resources.GetString("servoAddressesComboBox.Items1"),
            resources.GetString("servoAddressesComboBox.Items2")});
            resources.ApplyResources(this.servoAddressesComboBox, "servoAddressesComboBox");
            this.servoAddressesComboBox.Name = "servoAddressesComboBox";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // maxRotationPercentNumericUpDown
            // 
            resources.ApplyResources(this.maxRotationPercentNumericUpDown, "maxRotationPercentNumericUpDown");
            this.maxRotationPercentNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.maxRotationPercentNumericUpDown.Name = "maxRotationPercentNumericUpDown";
            this.maxRotationPercentNumericUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // ServoPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.maxRotationPercentNumericUpDown);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.servoAddressesComboBox);
            this.Controls.Add(this.maxValueTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.minValueTextBox);
            this.Controls.Add(this.displayPinComoBoxLabel);
            this.Name = "ServoPanel";
            ((System.ComponentModel.ISupportInitialize)(this.maxRotationPercentNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label displayPinComoBoxLabel;
        public System.Windows.Forms.TextBox minValueTextBox;
        public System.Windows.Forms.TextBox maxValueTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox servoAddressesComboBox;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.NumericUpDown maxRotationPercentNumericUpDown;
        private System.Windows.Forms.Label label4;
    }
}
