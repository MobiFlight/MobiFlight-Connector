﻿namespace MobiFlight.UI.Panels.Action
{
    partial class ProSimInputPanel
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
            this.proSimDatarefPanel1 = new MobiFlight.UI.Panels.Config.ProSimDataRefPanel();
            this.SuspendLayout();
            // 
            // proSimDatarefPanel1
            // 
            this.proSimDatarefPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.proSimDatarefPanel1.Location = new System.Drawing.Point(0, 0);
            this.proSimDatarefPanel1.MaximumSize = new System.Drawing.Size(1000, 1000);
            this.proSimDatarefPanel1.MinimumSize = new System.Drawing.Size(620, 400);
            this.proSimDatarefPanel1.Name = "proSimDatarefPanel1";
            this.proSimDatarefPanel1.Padding = new System.Windows.Forms.Padding(3);
            this.proSimDatarefPanel1.Path = "";
            this.proSimDatarefPanel1.Size = new System.Drawing.Size(636, 400);
            this.proSimDatarefPanel1.TabIndex = 0;
            this.proSimDatarefPanel1.Load += new System.EventHandler(this.proSimDatarefPanel1_Load);
            // 
            // ProSimInputPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.proSimDatarefPanel1);
            this.Name = "ProSimInputPanel";
            this.Size = new System.Drawing.Size(636, 400);
            this.ResumeLayout(false);

        }

        #endregion

        private Config.ProSimDataRefPanel proSimDatarefPanel1;
    }
}
