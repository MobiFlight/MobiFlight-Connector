namespace MobiFlight.UI.Panels.Modifier
{
    partial class InterpolationModifierPanel
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
            this.interpolationPanel1 = new MobiFlight.UI.Panels.Config.InterpolationPanel();
            this.SuspendLayout();
            // 
            // interpolationPanel1
            // 
            this.interpolationPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.interpolationPanel1.Location = new System.Drawing.Point(0, 0);
            this.interpolationPanel1.Margin = new System.Windows.Forms.Padding(1);
            this.interpolationPanel1.Name = "interpolationPanel1";
            this.interpolationPanel1.Save = false;
            this.interpolationPanel1.Size = new System.Drawing.Size(509, 173);
            this.interpolationPanel1.TabIndex = 0;
            // 
            // InterpolationModifier
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.interpolationPanel1);
            this.DoubleBuffered = true;
            this.Name = "InterpolationModifier";
            this.Size = new System.Drawing.Size(509, 173);
            this.ResumeLayout(false);

        }

        #endregion
        private Config.InterpolationPanel interpolationPanel1;
    }
}
