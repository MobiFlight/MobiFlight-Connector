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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.displayPinTestStopButton = new System.Windows.Forms.Button();
            this.displayPinTestButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxTestValueType = new System.Windows.Forms.ComboBox();
            this.labelTestResultValue = new System.Windows.Forms.Label();
            this.labelTestResult = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxTestValue = new System.Windows.Forms.TextBox();
            this.toolTipResultLabel = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.displayPinTestStopButton);
            this.groupBox1.Controls.Add(this.displayPinTestButton);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.comboBoxTestValueType);
            this.groupBox1.Controls.Add(this.labelTestResultValue);
            this.groupBox1.Controls.Add(this.labelTestResult);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxTestValue);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(6, 3);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(634, 46);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Test current settings";
            // 
            // displayPinTestStopButton
            // 
            this.displayPinTestStopButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.displayPinTestStopButton.Enabled = false;
            this.displayPinTestStopButton.Image = global::MobiFlight.Properties.Resources.media_stop;
            this.displayPinTestStopButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.displayPinTestStopButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.displayPinTestStopButton.Location = new System.Drawing.Point(369, 17);
            this.displayPinTestStopButton.Name = "displayPinTestStopButton";
            this.displayPinTestStopButton.Size = new System.Drawing.Size(58, 23);
            this.displayPinTestStopButton.TabIndex = 21;
            this.displayPinTestStopButton.Text = "Stop";
            this.displayPinTestStopButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.displayPinTestStopButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.displayPinTestStopButton.UseVisualStyleBackColor = true;
            this.displayPinTestStopButton.Click += new System.EventHandler(this.displayPinTestStopButton_Click);
            // 
            // displayPinTestButton
            // 
            this.displayPinTestButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.displayPinTestButton.Image = global::MobiFlight.Properties.Resources.media_play;
            this.displayPinTestButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.displayPinTestButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.displayPinTestButton.Location = new System.Drawing.Point(305, 17);
            this.displayPinTestButton.Name = "displayPinTestButton";
            this.displayPinTestButton.Size = new System.Drawing.Size(58, 23);
            this.displayPinTestButton.TabIndex = 20;
            this.displayPinTestButton.Text = "Test";
            this.displayPinTestButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.displayPinTestButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.displayPinTestButton.UseVisualStyleBackColor = true;
            this.displayPinTestButton.Click += new System.EventHandler(this.displayPinTestButton_Click);
            // 
            // label3
            // 
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(125, 16);
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
            this.comboBoxTestValueType.Location = new System.Drawing.Point(45, 18);
            this.comboBoxTestValueType.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.comboBoxTestValueType.Name = "comboBoxTestValueType";
            this.comboBoxTestValueType.Size = new System.Drawing.Size(71, 21);
            this.comboBoxTestValueType.TabIndex = 18;
            // 
            // labelTestResultValue
            // 
            this.labelTestResultValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTestResultValue.AutoEllipsis = true;
            this.labelTestResultValue.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTestResultValue.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelTestResultValue.Location = new System.Drawing.Point(485, 17);
            this.labelTestResultValue.Margin = new System.Windows.Forms.Padding(0);
            this.labelTestResultValue.Name = "labelTestResultValue";
            this.labelTestResultValue.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.labelTestResultValue.Size = new System.Drawing.Size(143, 23);
            this.labelTestResultValue.TabIndex = 17;
            this.labelTestResultValue.Text = "asd42342342342342342";
            this.labelTestResultValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelTestResult
            // 
            this.labelTestResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTestResult.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelTestResult.Location = new System.Drawing.Point(433, 17);
            this.labelTestResult.Name = "labelTestResult";
            this.labelTestResult.Size = new System.Drawing.Size(49, 23);
            this.labelTestResult.TabIndex = 16;
            this.labelTestResult.Text = "Result:";
            this.labelTestResult.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(10, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 23);
            this.label2.TabIndex = 15;
            this.label2.Text = "Type";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxTestValue
            // 
            this.textBoxTestValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTestValue.Location = new System.Drawing.Point(171, 18);
            this.textBoxTestValue.Name = "textBoxTestValue";
            this.textBoxTestValue.Size = new System.Drawing.Size(128, 20);
            this.textBoxTestValue.TabIndex = 14;
            // 
            // TestValuePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "TestValuePanel";
            this.Padding = new System.Windows.Forms.Padding(6, 3, 6, 3);
            this.Size = new System.Drawing.Size(646, 52);
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
        private System.Windows.Forms.Button displayPinTestStopButton;
        private System.Windows.Forms.Button displayPinTestButton;
        private System.Windows.Forms.ToolTip toolTipResultLabel;
    }
}
