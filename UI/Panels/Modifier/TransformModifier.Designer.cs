namespace MobiFlight.UI.Panels.Modifier
{
    partial class TransformModifier
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
            this.multiplyPanel = new System.Windows.Forms.Panel();
            this.TransformationCheckBox = new System.Windows.Forms.CheckBox();
            this.fsuipcMultiplyTextBox = new System.Windows.Forms.TextBox();
            this.multiplyPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // multiplyPanel
            // 
            this.multiplyPanel.AutoSize = true;
            this.multiplyPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.multiplyPanel.Controls.Add(this.TransformationCheckBox);
            this.multiplyPanel.Controls.Add(this.fsuipcMultiplyTextBox);
            this.multiplyPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.multiplyPanel.Location = new System.Drawing.Point(0, 0);
            this.multiplyPanel.Name = "multiplyPanel";
            this.multiplyPanel.Size = new System.Drawing.Size(285, 24);
            this.multiplyPanel.TabIndex = 16;
            // 
            // TransformationCheckBox
            // 
            this.TransformationCheckBox.Checked = true;
            this.TransformationCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.TransformationCheckBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.TransformationCheckBox.Location = new System.Drawing.Point(10, 3);
            this.TransformationCheckBox.Name = "TransformationCheckBox";
            this.TransformationCheckBox.Size = new System.Drawing.Size(94, 18);
            this.TransformationCheckBox.TabIndex = 15;
            this.TransformationCheckBox.Text = "Transform";
            this.TransformationCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.TransformationCheckBox.UseVisualStyleBackColor = true;
            // 
            // fsuipcMultiplyTextBox
            // 
            this.fsuipcMultiplyTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fsuipcMultiplyTextBox.Location = new System.Drawing.Point(110, 1);
            this.fsuipcMultiplyTextBox.Name = "fsuipcMultiplyTextBox";
            this.fsuipcMultiplyTextBox.Size = new System.Drawing.Size(153, 20);
            this.fsuipcMultiplyTextBox.TabIndex = 13;
            // 
            // TransformModifier
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.multiplyPanel);
            this.Name = "TransformModifier";
            this.Size = new System.Drawing.Size(285, 151);
            this.multiplyPanel.ResumeLayout(false);
            this.multiplyPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel multiplyPanel;
        private System.Windows.Forms.CheckBox TransformationCheckBox;
        private System.Windows.Forms.TextBox fsuipcMultiplyTextBox;
    }
}
