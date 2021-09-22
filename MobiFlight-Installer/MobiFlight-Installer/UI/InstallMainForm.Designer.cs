
namespace MobiFlightInstaller.UI
{
    partial class InstallMainForm
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
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.SelectedPath = new System.Windows.Forms.TextBox();
            this.buttonChooseFolder = new System.Windows.Forms.Button();
            this.buttonStartInstall = new System.Windows.Forms.Button();
            this.SetupProgressBar = new System.Windows.Forms.ProgressBar();
            this.UpdaterCurrentTask = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // SelectedPath
            // 
            this.SelectedPath.Location = new System.Drawing.Point(44, 84);
            this.SelectedPath.Name = "SelectedPath";
            this.SelectedPath.Size = new System.Drawing.Size(509, 20);
            this.SelectedPath.TabIndex = 0;
            // 
            // buttonChooseFolder
            // 
            this.buttonChooseFolder.Location = new System.Drawing.Point(575, 84);
            this.buttonChooseFolder.Name = "buttonChooseFolder";
            this.buttonChooseFolder.Size = new System.Drawing.Size(174, 23);
            this.buttonChooseFolder.TabIndex = 1;
            this.buttonChooseFolder.Text = "Choose a new folder";
            this.buttonChooseFolder.UseVisualStyleBackColor = true;
            this.buttonChooseFolder.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonStartInstall
            // 
            this.buttonStartInstall.Location = new System.Drawing.Point(281, 142);
            this.buttonStartInstall.Name = "buttonStartInstall";
            this.buttonStartInstall.Size = new System.Drawing.Size(299, 62);
            this.buttonStartInstall.TabIndex = 2;
            this.buttonStartInstall.Text = "INSTALL";
            this.buttonStartInstall.UseVisualStyleBackColor = true;
            this.buttonStartInstall.Click += new System.EventHandler(this.button2_Click);
            // 
            // SetupProgressBar
            // 
            this.SetupProgressBar.Location = new System.Drawing.Point(44, 240);
            this.SetupProgressBar.Name = "SetupProgressBar";
            this.SetupProgressBar.Size = new System.Drawing.Size(705, 30);
            this.SetupProgressBar.TabIndex = 3;
            // 
            // UpdaterCurrentTask
            // 
            this.UpdaterCurrentTask.AutoSize = true;
            this.UpdaterCurrentTask.Location = new System.Drawing.Point(41, 317);
            this.UpdaterCurrentTask.Name = "UpdaterCurrentTask";
            this.UpdaterCurrentTask.Size = new System.Drawing.Size(0, 13);
            this.UpdaterCurrentTask.TabIndex = 4;
            this.UpdaterCurrentTask.Click += new System.EventHandler(this.label1_Click);
            // 
            // InstallMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.UpdaterCurrentTask);
            this.Controls.Add(this.SetupProgressBar);
            this.Controls.Add(this.buttonStartInstall);
            this.Controls.Add(this.buttonChooseFolder);
            this.Controls.Add(this.SelectedPath);
            this.Name = "InstallMainForm";
            this.Text = "MobiFlight Setup";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox SelectedPath;
        private System.Windows.Forms.Button buttonChooseFolder;
        private System.Windows.Forms.Button buttonStartInstall;
        private System.Windows.Forms.ProgressBar SetupProgressBar;
        private System.Windows.Forms.Label UpdaterCurrentTask;
    }
}