namespace MobiFlight_Updater
{
    partial class UpdaterForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdaterForm));
            this.ButtonExit = new System.Windows.Forms.Button();
            this.ButtonInstall = new System.Windows.Forms.Button();
            this.ListVersions = new System.Windows.Forms.ListBox();
            this.ButtonDownload = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.StatusCurrent = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonStartMF = new System.Windows.Forms.Button();
            this.ShowNotes = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // ButtonExit
            // 
            this.ButtonExit.Location = new System.Drawing.Point(211, 411);
            this.ButtonExit.Name = "ButtonExit";
            this.ButtonExit.Size = new System.Drawing.Size(75, 49);
            this.ButtonExit.TabIndex = 0;
            this.ButtonExit.Text = "Exit";
            this.ButtonExit.UseVisualStyleBackColor = true;
            this.ButtonExit.Click += new System.EventHandler(this.ButtonExit_Click);
            // 
            // ButtonInstall
            // 
            this.ButtonInstall.Location = new System.Drawing.Point(12, 411);
            this.ButtonInstall.Name = "ButtonInstall";
            this.ButtonInstall.Size = new System.Drawing.Size(76, 49);
            this.ButtonInstall.TabIndex = 2;
            this.ButtonInstall.Text = "INSTALL";
            this.ButtonInstall.UseVisualStyleBackColor = true;
            this.ButtonInstall.Click += new System.EventHandler(this.ButtonInstall_Click);
            // 
            // ListVersions
            // 
            this.ListVersions.FormattingEnabled = true;
            this.ListVersions.Location = new System.Drawing.Point(16, 139);
            this.ListVersions.Name = "ListVersions";
            this.ListVersions.Size = new System.Drawing.Size(270, 251);
            this.ListVersions.TabIndex = 3;
            this.ListVersions.SelectedIndexChanged += new System.EventHandler(this.ListVersions_SelectedIndexChanged);
            // 
            // ButtonDownload
            // 
            this.ButtonDownload.Location = new System.Drawing.Point(88, 81);
            this.ButtonDownload.Name = "ButtonDownload";
            this.ButtonDownload.Size = new System.Drawing.Size(108, 39);
            this.ButtonDownload.TabIndex = 4;
            this.ButtonDownload.Text = "Download";
            this.ButtonDownload.UseVisualStyleBackColor = true;
            this.ButtonDownload.Click += new System.EventHandler(this.ButtonDownload_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 505);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(274, 23);
            this.progressBar1.TabIndex = 6;
            // 
            // StatusCurrent
            // 
            this.StatusCurrent.AutoSize = true;
            this.StatusCurrent.Location = new System.Drawing.Point(33, 489);
            this.StatusCurrent.Name = "StatusCurrent";
            this.StatusCurrent.Size = new System.Drawing.Size(0, 13);
            this.StatusCurrent.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "label1";
            // 
            // ButtonStartMF
            // 
            this.ButtonStartMF.Enabled = false;
            this.ButtonStartMF.Location = new System.Drawing.Point(115, 411);
            this.ButtonStartMF.Name = "ButtonStartMF";
            this.ButtonStartMF.Size = new System.Drawing.Size(69, 49);
            this.ButtonStartMF.TabIndex = 9;
            this.ButtonStartMF.Text = "Start MobiFlight";
            this.ButtonStartMF.UseVisualStyleBackColor = true;
            this.ButtonStartMF.Click += new System.EventHandler(this.Button1_Click);
            // 
            // ShowNotes
            // 
            this.ShowNotes.Location = new System.Drawing.Point(309, 2);
            this.ShowNotes.MinimumSize = new System.Drawing.Size(20, 20);
            this.ShowNotes.Name = "ShowNotes";
            this.ShowNotes.Size = new System.Drawing.Size(664, 539);
            this.ShowNotes.TabIndex = 10;
            // 
            // UpdaterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(975, 541);
            this.Controls.Add(this.ShowNotes);
            this.Controls.Add(this.ButtonStartMF);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.StatusCurrent);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.ButtonDownload);
            this.Controls.Add(this.ListVersions);
            this.Controls.Add(this.ButtonInstall);
            this.Controls.Add(this.ButtonExit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UpdaterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MobiFlight Updater";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonExit;
        private System.Windows.Forms.Button ButtonInstall;
        private System.Windows.Forms.ListBox ListVersions;
        private System.Windows.Forms.Button ButtonDownload;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label StatusCurrent;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ButtonStartMF;
        private System.Windows.Forms.WebBrowser ShowNotes;
    }
}

