namespace MobiFlight.Panels
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.simpleTabPage = new System.Windows.Forms.TabPage();
            this.label4 = new System.Windows.Forms.Label();
            this.maxRotationPercentNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.maxValueTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.minValueTextBox = new System.Windows.Forms.TextBox();
            this.displayPinComoBoxLabel = new System.Windows.Forms.Label();
            this.advancedTabPage = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.servoAddressesComboBox = new System.Windows.Forms.ComboBox();
            this.interpolationPanel1 = new MobiFlight.Panels.Group.InterpolationPanel();
            this.tabControl1.SuspendLayout();
            this.simpleTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxRotationPercentNumericUpDown)).BeginInit();
            this.advancedTabPage.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.simpleTabPage);
            this.tabControl1.Controls.Add(this.advancedTabPage);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // simpleTabPage
            // 
            this.simpleTabPage.Controls.Add(this.label4);
            this.simpleTabPage.Controls.Add(this.maxRotationPercentNumericUpDown);
            this.simpleTabPage.Controls.Add(this.label3);
            this.simpleTabPage.Controls.Add(this.maxValueTextBox);
            this.simpleTabPage.Controls.Add(this.label1);
            this.simpleTabPage.Controls.Add(this.minValueTextBox);
            this.simpleTabPage.Controls.Add(this.displayPinComoBoxLabel);
            resources.ApplyResources(this.simpleTabPage, "simpleTabPage");
            this.simpleTabPage.Name = "simpleTabPage";
            this.simpleTabPage.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
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
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // maxValueTextBox
            // 
            resources.ApplyResources(this.maxValueTextBox, "maxValueTextBox");
            this.maxValueTextBox.Name = "maxValueTextBox";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // minValueTextBox
            // 
            resources.ApplyResources(this.minValueTextBox, "minValueTextBox");
            this.minValueTextBox.Name = "minValueTextBox";
            // 
            // displayPinComoBoxLabel
            // 
            resources.ApplyResources(this.displayPinComoBoxLabel, "displayPinComoBoxLabel");
            this.displayPinComoBoxLabel.Name = "displayPinComoBoxLabel";
            // 
            // advancedTabPage
            // 
            this.advancedTabPage.Controls.Add(this.interpolationPanel1);
            resources.ApplyResources(this.advancedTabPage, "advancedTabPage");
            this.advancedTabPage.Name = "advancedTabPage";
            this.advancedTabPage.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.servoAddressesComboBox);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
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
            // interpolationPanel1
            // 
            resources.ApplyResources(this.interpolationPanel1, "interpolationPanel1");
            this.interpolationPanel1.Name = "interpolationPanel1";
            // 
            // ServoPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Name = "ServoPanel";
            this.tabControl1.ResumeLayout(false);
            this.simpleTabPage.ResumeLayout(false);
            this.simpleTabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxRotationPercentNumericUpDown)).EndInit();
            this.advancedTabPage.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage simpleTabPage;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.NumericUpDown maxRotationPercentNumericUpDown;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox maxValueTextBox;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox minValueTextBox;
        private System.Windows.Forms.Label displayPinComoBoxLabel;
        private System.Windows.Forms.TabPage advancedTabPage;
        private Group.InterpolationPanel interpolationPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox servoAddressesComboBox;
    }
}
