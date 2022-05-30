namespace MobiFlight.UI.Panels.Config
{
    partial class XplaneDataRefPanel
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
            this.label1 = new System.Windows.Forms.Label();
            this.DataRefTextBox = new System.Windows.Forms.TextBox();
            this.transformOptionsGroup1 = new MobiFlight.UI.Panels.Config.TransformOptionsGroup();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.DataRefTextBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(521, 63);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "DataRefSettings";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "DataRef";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DataRefTextBox
            // 
            this.DataRefTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DataRefTextBox.Location = new System.Drawing.Point(113, 26);
            this.DataRefTextBox.Name = "DataRefTextBox";
            this.DataRefTextBox.Size = new System.Drawing.Size(398, 20);
            this.DataRefTextBox.TabIndex = 1;
            // 
            // transformOptionsGroup1
            // 
            this.transformOptionsGroup1.Dock = System.Windows.Forms.DockStyle.Top;
            this.transformOptionsGroup1.Location = new System.Drawing.Point(0, 63);
            this.transformOptionsGroup1.Name = "transformOptionsGroup1";
            this.transformOptionsGroup1.Size = new System.Drawing.Size(521, 94);
            this.transformOptionsGroup1.TabIndex = 1;
            // 
            // XplaneDataRefPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.transformOptionsGroup1);
            this.Controls.Add(this.groupBox1);
            this.Name = "XplaneDataRefPanel";
            this.Size = new System.Drawing.Size(521, 167);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox DataRefTextBox;
        private System.Windows.Forms.Label label1;
        private TransformOptionsGroup transformOptionsGroup1;
    }
}
