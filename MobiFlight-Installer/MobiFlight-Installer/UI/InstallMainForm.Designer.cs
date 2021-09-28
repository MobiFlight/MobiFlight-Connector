
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallMainForm));
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.SelectedPath = new System.Windows.Forms.TextBox();
            this.buttonChooseFolder = new System.Windows.Forms.Button();
            this.buttonStartInstall = new System.Windows.Forms.Button();
            this.SetupProgressBar = new System.Windows.Forms.ProgressBar();
            this.UpdaterCurrentTask = new System.Windows.Forms.Label();
            this.SetupTitle = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.BoxGithub = new System.Windows.Forms.CheckBox();
            this.BoxTargetDirectory = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // SelectedPath
            // 
            this.SelectedPath.Location = new System.Drawing.Point(47, 270);
            this.SelectedPath.Name = "SelectedPath";
            this.SelectedPath.Size = new System.Drawing.Size(581, 20);
            this.SelectedPath.TabIndex = 0;
            // 
            // buttonChooseFolder
            // 
            this.buttonChooseFolder.Location = new System.Drawing.Point(634, 267);
            this.buttonChooseFolder.Name = "buttonChooseFolder";
            this.buttonChooseFolder.Size = new System.Drawing.Size(118, 23);
            this.buttonChooseFolder.TabIndex = 1;
            this.buttonChooseFolder.Text = "Choose a new folder";
            this.buttonChooseFolder.UseVisualStyleBackColor = true;
            this.buttonChooseFolder.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonStartInstall
            // 
            this.buttonStartInstall.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStartInstall.Location = new System.Drawing.Point(256, 308);
            this.buttonStartInstall.Name = "buttonStartInstall";
            this.buttonStartInstall.Size = new System.Drawing.Size(299, 43);
            this.buttonStartInstall.TabIndex = 2;
            this.buttonStartInstall.Text = "Take OFF";
            this.buttonStartInstall.UseVisualStyleBackColor = true;
            this.buttonStartInstall.Click += new System.EventHandler(this.button2_Click);
            // 
            // SetupProgressBar
            // 
            this.SetupProgressBar.Location = new System.Drawing.Point(47, 371);
            this.SetupProgressBar.Name = "SetupProgressBar";
            this.SetupProgressBar.Size = new System.Drawing.Size(705, 30);
            this.SetupProgressBar.TabIndex = 3;
            // 
            // UpdaterCurrentTask
            // 
            this.UpdaterCurrentTask.AutoSize = true;
            this.UpdaterCurrentTask.Location = new System.Drawing.Point(44, 419);
            this.UpdaterCurrentTask.Name = "UpdaterCurrentTask";
            this.UpdaterCurrentTask.Size = new System.Drawing.Size(0, 13);
            this.UpdaterCurrentTask.TabIndex = 4;
            this.UpdaterCurrentTask.Click += new System.EventHandler(this.label1_Click);
            // 
            // SetupTitle
            // 
            this.SetupTitle.BackColor = System.Drawing.Color.Transparent;
            this.SetupTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SetupTitle.ForeColor = System.Drawing.Color.White;
            this.SetupTitle.Location = new System.Drawing.Point(12, 134);
            this.SetupTitle.Name = "SetupTitle";
            this.SetupTitle.Size = new System.Drawing.Size(776, 42);
            this.SetupTitle.TabIndex = 5;
            this.SetupTitle.Text = "Clear to take off MF9";
            this.SetupTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.SetupTitle.Click += new System.EventHandler(this.SetupTitle_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(43, 247);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(307, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "Choose a folder where to install MobiFlight";
            // 
            // BoxGithub
            // 
            this.BoxGithub.AutoSize = true;
            this.BoxGithub.BackColor = System.Drawing.Color.Transparent;
            this.BoxGithub.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BoxGithub.ForeColor = System.Drawing.Color.White;
            this.BoxGithub.Location = new System.Drawing.Point(134, 18);
            this.BoxGithub.Name = "BoxGithub";
            this.BoxGithub.Size = new System.Drawing.Size(193, 24);
            this.BoxGithub.TabIndex = 8;
            this.BoxGithub.Text = "Github Check Online";
            this.BoxGithub.UseVisualStyleBackColor = false;
            // 
            // BoxTargetDirectory
            // 
            this.BoxTargetDirectory.AutoSize = true;
            this.BoxTargetDirectory.BackColor = System.Drawing.Color.Transparent;
            this.BoxTargetDirectory.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BoxTargetDirectory.ForeColor = System.Drawing.Color.White;
            this.BoxTargetDirectory.Location = new System.Drawing.Point(479, 18);
            this.BoxTargetDirectory.Name = "BoxTargetDirectory";
            this.BoxTargetDirectory.Size = new System.Drawing.Size(214, 24);
            this.BoxTargetDirectory.TabIndex = 9;
            this.BoxTargetDirectory.Text = "Target Directory Rights";
            this.BoxTargetDirectory.UseVisualStyleBackColor = false;
            // 
            // InstallMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::MobiFlightInstaller.Properties.Resources.SetupBackground;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.BoxTargetDirectory);
            this.Controls.Add(this.BoxGithub);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SetupTitle);
            this.Controls.Add(this.UpdaterCurrentTask);
            this.Controls.Add(this.SetupProgressBar);
            this.Controls.Add(this.buttonStartInstall);
            this.Controls.Add(this.buttonChooseFolder);
            this.Controls.Add(this.SelectedPath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InstallMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
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
        private System.Windows.Forms.Label SetupTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox BoxGithub;
        private System.Windows.Forms.CheckBox BoxTargetDirectory;
    }
}