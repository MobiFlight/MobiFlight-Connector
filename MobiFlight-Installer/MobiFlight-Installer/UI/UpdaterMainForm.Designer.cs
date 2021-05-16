
namespace MobiFlightInstaller.UI
{
    public partial class UpdaterMainForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdaterMainForm));
            this.UpdaterProgressBar = new System.Windows.Forms.ProgressBar();
            this.UpdaterCurrentTask = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // UpdaterProgressBar
            // 
            this.UpdaterProgressBar.Location = new System.Drawing.Point(173, 83);
            this.UpdaterProgressBar.Name = "UpdaterProgressBar";
            this.UpdaterProgressBar.Size = new System.Drawing.Size(414, 23);
            this.UpdaterProgressBar.TabIndex = 0;
            // 
            // UpdaterCurrentTask
            // 
            this.UpdaterCurrentTask.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.UpdaterCurrentTask.AutoSize = true;
            this.UpdaterCurrentTask.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpdaterCurrentTask.Location = new System.Drawing.Point(168, 35);
            this.UpdaterCurrentTask.Name = "UpdaterCurrentTask";
            this.UpdaterCurrentTask.Size = new System.Drawing.Size(76, 26);
            this.UpdaterCurrentTask.TabIndex = 1;
            this.UpdaterCurrentTask.Text = "label1";
            this.UpdaterCurrentTask.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.UpdaterCurrentTask.Click += new System.EventHandler(this.label1_Click);
            // 
            // UpdaterMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 149);
            this.Controls.Add(this.UpdaterCurrentTask);
            this.Controls.Add(this.UpdaterProgressBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UpdaterMainForm";
            this.Text = "MobiFlight Installer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ProgressBar UpdaterProgressBar;
        private System.Windows.Forms.Label UpdaterCurrentTask;
    }
}