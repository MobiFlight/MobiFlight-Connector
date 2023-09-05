namespace MobiFlight.UI.Panels.Config
{
    partial class TestValuePanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxTestValueType = new System.Windows.Forms.ComboBox();
            this.labelTestResultValue = new System.Windows.Forms.Label();
            this.labelTestResult = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTestValue = new System.Windows.Forms.TextBox();
            this.buttonTest = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboBoxTestValueType);
            this.groupBox1.Controls.Add(this.labelTestResultValue);
            this.groupBox1.Controls.Add(this.labelTestResult);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxTestValue);
            this.groupBox1.Controls.Add(this.buttonTest);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(508, 46);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Test current settings";
            // 
            // label3
            // 
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(135, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 23);
            this.label3.TabIndex = 19;
            this.label3.Text = "Value";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxTestValueType
            // 
            this.comboBoxTestValueType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTestValueType.FormattingEnabled = true;
            this.comboBoxTestValueType.Items.AddRange(new object[] {
            "Number",
            "String"});
            this.comboBoxTestValueType.Location = new System.Drawing.Point(58, 18);
            this.comboBoxTestValueType.Name = "comboBoxTestValueType";
            this.comboBoxTestValueType.Size = new System.Drawing.Size(71, 21);
            this.comboBoxTestValueType.TabIndex = 18;
            // 
            // labelTestResultValue
            // 
            this.labelTestResultValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTestResultValue.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelTestResultValue.Location = new System.Drawing.Point(430, 16);
            this.labelTestResultValue.Name = "labelTestResultValue";
            this.labelTestResultValue.Size = new System.Drawing.Size(75, 23);
            this.labelTestResultValue.TabIndex = 17;
            this.labelTestResultValue.Text = "#######";
            this.labelTestResultValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelTestResult
            // 
            this.labelTestResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTestResult.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelTestResult.Location = new System.Drawing.Point(378, 16);
            this.labelTestResult.Name = "labelTestResult";
            this.labelTestResult.Size = new System.Drawing.Size(49, 23);
            this.labelTestResult.TabIndex = 16;
            this.labelTestResult.Text = "Result";
            this.labelTestResult.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(10, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 23);
            this.label2.TabIndex = 15;
            this.label2.Text = "Type";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxTestValue
            // 
            this.textBoxTestValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTestValue.Location = new System.Drawing.Point(181, 18);
            this.textBoxTestValue.Name = "textBoxTestValue";
            this.textBoxTestValue.Size = new System.Drawing.Size(125, 20);
            this.textBoxTestValue.TabIndex = 14;
            // 
            // buttonTest
            // 
            this.buttonTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTest.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonTest.Location = new System.Drawing.Point(312, 16);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(60, 23);
            this.buttonTest.TabIndex = 13;
            this.buttonTest.Text = "Test";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // TestValuePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "TestValuePanel";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(514, 52);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxTestValueType;
        private System.Windows.Forms.Label labelTestResultValue;
        private System.Windows.Forms.Label labelTestResult;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxTestValue;
        private System.Windows.Forms.Button buttonTest;
    }
}
