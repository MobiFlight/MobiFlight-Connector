namespace MobiFlight.UI.Panels.Modifier
{
    partial class InterpolationModifier
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
            this.interpolationGroupBox = new System.Windows.Forms.GroupBox();
            this.interpolationPanel1 = new MobiFlight.UI.Panels.Config.InterpolationPanel();
            this.interpolationCheckBox = new System.Windows.Forms.CheckBox();
            this.interpolationGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // interpolationGroupBox
            // 
            this.interpolationGroupBox.Controls.Add(this.interpolationPanel1);
            this.interpolationGroupBox.Controls.Add(this.interpolationCheckBox);
            this.interpolationGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.interpolationGroupBox.Location = new System.Drawing.Point(0, 0);
            this.interpolationGroupBox.Name = "interpolationGroupBox";
            this.interpolationGroupBox.Size = new System.Drawing.Size(509, 172);
            this.interpolationGroupBox.TabIndex = 2;
            this.interpolationGroupBox.TabStop = false;
            this.interpolationGroupBox.Text = "Interpolation Settings";
            // 
            // interpolationPanel1
            // 
            this.interpolationPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.interpolationPanel1.Location = new System.Drawing.Point(3, 39);
            this.interpolationPanel1.Margin = new System.Windows.Forms.Padding(1);
            this.interpolationPanel1.Name = "interpolationPanel1";
            this.interpolationPanel1.Save = false;
            this.interpolationPanel1.Size = new System.Drawing.Size(503, 130);
            this.interpolationPanel1.TabIndex = 0;
            // 
            // interpolationCheckBox
            // 
            this.interpolationCheckBox.AutoSize = true;
            this.interpolationCheckBox.Checked = true;
            this.interpolationCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.interpolationCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.interpolationCheckBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.interpolationCheckBox.Location = new System.Drawing.Point(3, 16);
            this.interpolationCheckBox.Name = "interpolationCheckBox";
            this.interpolationCheckBox.Padding = new System.Windows.Forms.Padding(3);
            this.interpolationCheckBox.Size = new System.Drawing.Size(503, 23);
            this.interpolationCheckBox.TabIndex = 9;
            this.interpolationCheckBox.Text = "Apply interpolation to modify the current value";
            this.interpolationCheckBox.UseVisualStyleBackColor = true;
            // 
            // InterpolationModifier
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.interpolationGroupBox);
            this.Name = "InterpolationModifier";
            this.Size = new System.Drawing.Size(509, 173);
            this.interpolationGroupBox.ResumeLayout(false);
            this.interpolationGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox interpolationGroupBox;
        private Config.InterpolationPanel interpolationPanel1;
        private System.Windows.Forms.CheckBox interpolationCheckBox;
    }
}
