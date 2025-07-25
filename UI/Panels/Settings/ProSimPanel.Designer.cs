namespace MobiFlight.UI.Panels.Settings
{
    partial class ProSimPanel
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
            this.proSimMaxRetryNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.proSimAutoConnectCheckBox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.proSimHostTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.proSimPortTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.proSimMaxRetryNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.proSimMaxRetryNumericUpDown);
            this.groupBox1.Controls.Add(this.proSimAutoConnectCheckBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.proSimPortTextBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.proSimHostTextBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(551, 120);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ProSim Settings";
            // 
            // proSimMaxRetryNumericUpDown
            // 
            this.proSimMaxRetryNumericUpDown.Location = new System.Drawing.Point(120, 95);
            this.proSimMaxRetryNumericUpDown.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.proSimMaxRetryNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.proSimMaxRetryNumericUpDown.Name = "proSimMaxRetryNumericUpDown";
            this.proSimMaxRetryNumericUpDown.Size = new System.Drawing.Size(83, 20);
            this.proSimMaxRetryNumericUpDown.TabIndex = 6;
            this.proSimMaxRetryNumericUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // proSimAutoConnectCheckBox
            // 
            this.proSimAutoConnectCheckBox.AutoSize = true;
            this.proSimAutoConnectCheckBox.Location = new System.Drawing.Point(9, 69);
            this.proSimAutoConnectCheckBox.Name = "proSimAutoConnectCheckBox";
            this.proSimAutoConnectCheckBox.Size = new System.Drawing.Size(194, 17);
            this.proSimAutoConnectCheckBox.TabIndex = 5;
            this.proSimAutoConnectCheckBox.Text = "Enable automatic ProSim connection";
            this.proSimAutoConnectCheckBox.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Max retry attempts:";
            // 
            // proSimHostTextBox
            // 
            this.proSimHostTextBox.Location = new System.Drawing.Point(49, 17);
            this.proSimHostTextBox.Name = "proSimHostTextBox";
            this.proSimHostTextBox.Size = new System.Drawing.Size(154, 20);
            this.proSimHostTextBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Host:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port:";
            // 
            // textBox1
            // 
            this.proSimPortTextBox.Location = new System.Drawing.Point(49, 43);
            this.proSimPortTextBox.Name = "textBox1";
            this.proSimPortTextBox.Size = new System.Drawing.Size(100, 20);
            this.proSimPortTextBox.TabIndex = 3;
            // 
            // ProSimPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "ProSimPanel";
            this.Size = new System.Drawing.Size(558, 524);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.proSimMaxRetryNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox proSimHostTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox proSimPortTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox proSimAutoConnectCheckBox;
        private System.Windows.Forms.NumericUpDown proSimMaxRetryNumericUpDown;
    }
}
