namespace ArcazeUSB.Panels
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
            this.displayPinComoBoxLabel = new System.Windows.Forms.Label();
            this.minValueTextBox = new System.Windows.Forms.TextBox();
            this.maxValueTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.servoAddressesComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // displayPinComoBoxLabel
            // 
            this.displayPinComoBoxLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.displayPinComoBoxLabel.Location = new System.Drawing.Point(6, 27);
            this.displayPinComoBoxLabel.Name = "displayPinComoBoxLabel";
            this.displayPinComoBoxLabel.Size = new System.Drawing.Size(82, 13);
            this.displayPinComoBoxLabel.TabIndex = 1;
            this.displayPinComoBoxLabel.Text = "Min. value";
            this.displayPinComoBoxLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // minValueTextBox
            // 
            this.minValueTextBox.Location = new System.Drawing.Point(94, 24);
            this.minValueTextBox.Name = "minValueTextBox";
            this.minValueTextBox.Size = new System.Drawing.Size(73, 20);
            this.minValueTextBox.TabIndex = 2;
            // 
            // maxValueTextBox
            // 
            this.maxValueTextBox.Location = new System.Drawing.Point(94, 46);
            this.maxValueTextBox.Name = "maxValueTextBox";
            this.maxValueTextBox.Size = new System.Drawing.Size(73, 20);
            this.maxValueTextBox.TabIndex = 4;
            this.maxValueTextBox.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(6, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Max. value";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(6, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Servo";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // servoAddressesComboBox
            // 
            this.servoAddressesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.servoAddressesComboBox.FormattingEnabled = true;
            this.servoAddressesComboBox.Items.AddRange(new object[] {
            "Pin",
            "7-Segment",
            "3BCD-8Bit-with-Strobe"});
            this.servoAddressesComboBox.Location = new System.Drawing.Point(94, 0);
            this.servoAddressesComboBox.MaximumSize = new System.Drawing.Size(122, 0);
            this.servoAddressesComboBox.MinimumSize = new System.Drawing.Size(47, 0);
            this.servoAddressesComboBox.Name = "servoAddressesComboBox";
            this.servoAddressesComboBox.Size = new System.Drawing.Size(122, 21);
            this.servoAddressesComboBox.TabIndex = 9;
            // 
            // ServoPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.servoAddressesComboBox);
            this.Controls.Add(this.maxValueTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.minValueTextBox);
            this.Controls.Add(this.displayPinComoBoxLabel);
            this.Name = "ServoPanel";
            this.Size = new System.Drawing.Size(255, 72);
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
    }
}
