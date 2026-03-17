namespace MobiFlight.UI.Panels.Action
{
    partial class ProSimActionInputPanel
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
            this.PathGroupBox = new System.Windows.Forms.GroupBox();
            this.PathTextBox = new System.Windows.Forms.TextBox();
            this.PathLabel = new System.Windows.Forms.Label();
            this.ExpressionTextBox = new System.Windows.Forms.TextBox();
            this.ExpressionLabel = new System.Windows.Forms.Label();
            this.PathGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // PathGroupBox
            // 
            this.PathGroupBox.Controls.Add(this.ExpressionTextBox);
            this.PathGroupBox.Controls.Add(this.ExpressionLabel);
            this.PathGroupBox.Controls.Add(this.PathTextBox);
            this.PathGroupBox.Controls.Add(this.PathLabel);
            this.PathGroupBox.Location = new System.Drawing.Point(3, 3);
            this.PathGroupBox.Name = "PathGroupBox";
            this.PathGroupBox.Size = new System.Drawing.Size(318, 113);
            this.PathGroupBox.TabIndex = 0;
            this.PathGroupBox.TabStop = false;
            this.PathGroupBox.Text = "ProSim DataRef";
            // 
            // PathTextBox
            // 
            this.PathTextBox.Location = new System.Drawing.Point(79, 28);
            this.PathTextBox.Name = "PathTextBox";
            this.PathTextBox.Size = new System.Drawing.Size(220, 20);
            this.PathTextBox.TabIndex = 1;
            this.PathTextBox.TextChanged += new System.EventHandler(this.PathTextBox_TextChanged);
            // 
            // PathLabel
            // 
            this.PathLabel.AutoSize = true;
            this.PathLabel.Location = new System.Drawing.Point(16, 31);
            this.PathLabel.Name = "PathLabel";
            this.PathLabel.Size = new System.Drawing.Size(57, 13);
            this.PathLabel.TabIndex = 0;
            this.PathLabel.Text = "Path:";
            // 
            // ExpressionTextBox
            // 
            this.ExpressionTextBox.Location = new System.Drawing.Point(79, 64);
            this.ExpressionTextBox.Name = "ExpressionTextBox";
            this.ExpressionTextBox.Size = new System.Drawing.Size(220, 20);
            this.ExpressionTextBox.TabIndex = 3;
            this.ExpressionTextBox.TextChanged += new System.EventHandler(this.ExpressionTextBox_TextChanged);
            // 
            // ExpressionLabel
            // 
            this.ExpressionLabel.AutoSize = true;
            this.ExpressionLabel.Location = new System.Drawing.Point(16, 67);
            this.ExpressionLabel.Name = "ExpressionLabel";
            this.ExpressionLabel.Size = new System.Drawing.Size(61, 13);
            this.ExpressionLabel.TabIndex = 2;
            this.ExpressionLabel.Text = "Value:";
            // 
            // ProSimActionInputPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.PathGroupBox);
            this.Name = "ProSimActionInputPanel";
            this.Size = new System.Drawing.Size(325, 120);
            this.PathGroupBox.ResumeLayout(false);
            this.PathGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox PathGroupBox;
        private System.Windows.Forms.TextBox PathTextBox;
        private System.Windows.Forms.Label PathLabel;
        private System.Windows.Forms.TextBox ExpressionTextBox;
        private System.Windows.Forms.Label ExpressionLabel;
    }
} 