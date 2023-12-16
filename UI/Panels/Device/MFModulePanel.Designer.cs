namespace MobiFlight.UI.Panels.Settings.Device
{
    partial class MFModulePanel
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBoxName = new System.Windows.Forms.GroupBox();
            this.moduleNameTextBox = new System.Windows.Forms.TextBox();
            this.groupBoxInformation = new System.Windows.Forms.GroupBox();
            this.TypeValueLabel = new System.Windows.Forms.Label();
            this.TypeLabel = new System.Windows.Forms.Label();
            this.SerialValueLabel = new System.Windows.Forms.Label();
            this.SerialLabel = new System.Windows.Forms.Label();
            this.PortValueLabel = new System.Windows.Forms.Label();
            this.PortLabel = new System.Windows.Forms.Label();
            this.FirmwareValueLabel = new System.Windows.Forms.Label();
            this.FirmwareLabel = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBoxDetails = new System.Windows.Forms.GroupBox();
            this.buttonWebsite = new System.Windows.Forms.Button();
            this.buttonDocs = new System.Windows.Forms.Button();
            this.buttonSupport = new System.Windows.Forms.Button();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonUploadDefaultConfig = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.labelProjectValue = new System.Windows.Forms.Label();
            this.labelProject = new System.Windows.Forms.Label();
            this.groupBoxName.SuspendLayout();
            this.groupBoxInformation.SuspendLayout();
            this.groupBoxDetails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxName
            // 
            this.groupBoxName.Controls.Add(this.moduleNameTextBox);
            this.groupBoxName.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxName.Location = new System.Drawing.Point(0, 0);
            this.groupBoxName.Name = "groupBoxName";
            this.groupBoxName.Size = new System.Drawing.Size(282, 48);
            this.groupBoxName.TabIndex = 5;
            this.groupBoxName.TabStop = false;
            this.groupBoxName.Text = "Name";
            // 
            // moduleNameTextBox
            // 
            this.moduleNameTextBox.Location = new System.Drawing.Point(18, 19);
            this.moduleNameTextBox.Name = "moduleNameTextBox";
            this.moduleNameTextBox.Size = new System.Drawing.Size(151, 20);
            this.moduleNameTextBox.TabIndex = 0;
            this.moduleNameTextBox.TextChanged += new System.EventHandler(this.value_Changed);
            this.moduleNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.moduleNameTextBox_Validating);
            // 
            // groupBoxInformation
            // 
            this.groupBoxInformation.Controls.Add(this.TypeValueLabel);
            this.groupBoxInformation.Controls.Add(this.TypeLabel);
            this.groupBoxInformation.Controls.Add(this.SerialValueLabel);
            this.groupBoxInformation.Controls.Add(this.SerialLabel);
            this.groupBoxInformation.Controls.Add(this.PortValueLabel);
            this.groupBoxInformation.Controls.Add(this.PortLabel);
            this.groupBoxInformation.Controls.Add(this.FirmwareValueLabel);
            this.groupBoxInformation.Controls.Add(this.FirmwareLabel);
            this.groupBoxInformation.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxInformation.Location = new System.Drawing.Point(0, 48);
            this.groupBoxInformation.Name = "groupBoxInformation";
            this.groupBoxInformation.Size = new System.Drawing.Size(282, 105);
            this.groupBoxInformation.TabIndex = 6;
            this.groupBoxInformation.TabStop = false;
            this.groupBoxInformation.Text = "Information";
            // 
            // TypeValueLabel
            // 
            this.TypeValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TypeValueLabel.Location = new System.Drawing.Point(73, 74);
            this.TypeValueLabel.Name = "TypeValueLabel";
            this.TypeValueLabel.Size = new System.Drawing.Size(195, 24);
            this.TypeValueLabel.TabIndex = 7;
            this.TypeValueLabel.Text = "MobiFlight Mega";
            this.toolTip1.SetToolTip(this.TypeValueLabel, "Test");
            // 
            // TypeLabel
            // 
            this.TypeLabel.AutoSize = true;
            this.TypeLabel.Location = new System.Drawing.Point(18, 74);
            this.TypeLabel.Name = "TypeLabel";
            this.TypeLabel.Size = new System.Drawing.Size(31, 13);
            this.TypeLabel.TabIndex = 6;
            this.TypeLabel.Text = "Type";
            // 
            // SerialValueLabel
            // 
            this.SerialValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SerialValueLabel.Location = new System.Drawing.Point(73, 56);
            this.SerialValueLabel.Name = "SerialValueLabel";
            this.SerialValueLabel.Size = new System.Drawing.Size(195, 13);
            this.SerialValueLabel.TabIndex = 5;
            this.SerialValueLabel.Text = "S-N / XXX";
            // 
            // SerialLabel
            // 
            this.SerialLabel.AutoSize = true;
            this.SerialLabel.Location = new System.Drawing.Point(18, 56);
            this.SerialLabel.Name = "SerialLabel";
            this.SerialLabel.Size = new System.Drawing.Size(33, 13);
            this.SerialLabel.TabIndex = 4;
            this.SerialLabel.Text = "Serial";
            // 
            // PortValueLabel
            // 
            this.PortValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PortValueLabel.Location = new System.Drawing.Point(73, 38);
            this.PortValueLabel.Name = "PortValueLabel";
            this.PortValueLabel.Size = new System.Drawing.Size(195, 13);
            this.PortValueLabel.TabIndex = 3;
            this.PortValueLabel.Text = "ComX";
            // 
            // PortLabel
            // 
            this.PortLabel.AutoSize = true;
            this.PortLabel.Location = new System.Drawing.Point(18, 38);
            this.PortLabel.Name = "PortLabel";
            this.PortLabel.Size = new System.Drawing.Size(26, 13);
            this.PortLabel.TabIndex = 2;
            this.PortLabel.Text = "Port";
            // 
            // FirmwareValueLabel
            // 
            this.FirmwareValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FirmwareValueLabel.Location = new System.Drawing.Point(73, 20);
            this.FirmwareValueLabel.Name = "FirmwareValueLabel";
            this.FirmwareValueLabel.Size = new System.Drawing.Size(195, 13);
            this.FirmwareValueLabel.TabIndex = 1;
            this.FirmwareValueLabel.Text = "x.x.x";
            // 
            // FirmwareLabel
            // 
            this.FirmwareLabel.AutoSize = true;
            this.FirmwareLabel.Location = new System.Drawing.Point(18, 20);
            this.FirmwareLabel.Name = "FirmwareLabel";
            this.FirmwareLabel.Size = new System.Drawing.Size(49, 13);
            this.FirmwareLabel.TabIndex = 0;
            this.FirmwareLabel.Text = "Firmware";
            // 
            // groupBoxDetails
            // 
            this.groupBoxDetails.Controls.Add(this.labelProjectValue);
            this.groupBoxDetails.Controls.Add(this.labelProject);
            this.groupBoxDetails.Controls.Add(this.panel1);
            this.groupBoxDetails.Controls.Add(this.pictureBoxLogo);
            this.groupBoxDetails.Controls.Add(this.buttonSupport);
            this.groupBoxDetails.Controls.Add(this.buttonDocs);
            this.groupBoxDetails.Controls.Add(this.buttonWebsite);
            this.groupBoxDetails.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxDetails.Location = new System.Drawing.Point(0, 153);
            this.groupBoxDetails.Name = "groupBoxDetails";
            this.groupBoxDetails.Size = new System.Drawing.Size(282, 152);
            this.groupBoxDetails.TabIndex = 7;
            this.groupBoxDetails.TabStop = false;
            this.groupBoxDetails.Text = "More Details";
            // 
            // buttonWebsite
            // 
            this.buttonWebsite.Location = new System.Drawing.Point(23, 89);
            this.buttonWebsite.Name = "buttonWebsite";
            this.buttonWebsite.Size = new System.Drawing.Size(75, 23);
            this.buttonWebsite.TabIndex = 12;
            this.buttonWebsite.Text = "Website";
            this.buttonWebsite.UseVisualStyleBackColor = true;
            // 
            // buttonDocs
            // 
            this.buttonDocs.Location = new System.Drawing.Point(104, 89);
            this.buttonDocs.Name = "buttonDocs";
            this.buttonDocs.Size = new System.Drawing.Size(75, 23);
            this.buttonDocs.TabIndex = 13;
            this.buttonDocs.Text = "Docs";
            this.buttonDocs.UseVisualStyleBackColor = true;
            // 
            // buttonSupport
            // 
            this.buttonSupport.Location = new System.Drawing.Point(185, 89);
            this.buttonSupport.Name = "buttonSupport";
            this.buttonSupport.Size = new System.Drawing.Size(72, 23);
            this.buttonSupport.TabIndex = 14;
            this.buttonSupport.Text = "Support";
            this.buttonSupport.UseVisualStyleBackColor = true;
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Location = new System.Drawing.Point(21, 40);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(236, 43);
            this.pictureBoxLogo.TabIndex = 15;
            this.pictureBoxLogo.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.buttonUploadDefaultConfig);
            this.panel1.Location = new System.Drawing.Point(18, 118);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(250, 28);
            this.panel1.TabIndex = 16;
            // 
            // buttonUploadDefaultConfig
            // 
            this.buttonUploadDefaultConfig.Location = new System.Drawing.Point(167, 3);
            this.buttonUploadDefaultConfig.Name = "buttonUploadDefaultConfig";
            this.buttonUploadDefaultConfig.Size = new System.Drawing.Size(72, 23);
            this.buttonUploadDefaultConfig.TabIndex = 17;
            this.buttonUploadDefaultConfig.Text = "Upload";
            this.buttonUploadDefaultConfig.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(158, 23);
            this.label1.TabIndex = 18;
            this.label1.Text = "Upload board\'s default config:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelProjectValue
            // 
            this.labelProjectValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelProjectValue.Location = new System.Drawing.Point(73, 19);
            this.labelProjectValue.Name = "labelProjectValue";
            this.labelProjectValue.Size = new System.Drawing.Size(184, 18);
            this.labelProjectValue.TabIndex = 18;
            this.labelProjectValue.Text = "MobiFlight Mega";
            this.toolTip1.SetToolTip(this.labelProjectValue, "Test");
            // 
            // labelProject
            // 
            this.labelProject.AutoSize = true;
            this.labelProject.Location = new System.Drawing.Point(18, 19);
            this.labelProject.Name = "labelProject";
            this.labelProject.Size = new System.Drawing.Size(40, 13);
            this.labelProject.TabIndex = 17;
            this.labelProject.Text = "Project";
            // 
            // MFModulePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.groupBoxDetails);
            this.Controls.Add(this.groupBoxInformation);
            this.Controls.Add(this.groupBoxName);
            this.Name = "MFModulePanel";
            this.Size = new System.Drawing.Size(282, 325);
            this.groupBoxName.ResumeLayout(false);
            this.groupBoxName.PerformLayout();
            this.groupBoxInformation.ResumeLayout(false);
            this.groupBoxInformation.PerformLayout();
            this.groupBoxDetails.ResumeLayout(false);
            this.groupBoxDetails.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxName;
        private System.Windows.Forms.TextBox moduleNameTextBox;
        private System.Windows.Forms.GroupBox groupBoxInformation;
        private System.Windows.Forms.Label FirmwareValueLabel;
        private System.Windows.Forms.Label FirmwareLabel;
        private System.Windows.Forms.Label SerialValueLabel;
        private System.Windows.Forms.Label SerialLabel;
        private System.Windows.Forms.Label PortValueLabel;
        private System.Windows.Forms.Label PortLabel;
        private System.Windows.Forms.Label TypeValueLabel;
        private System.Windows.Forms.Label TypeLabel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBoxDetails;
        private System.Windows.Forms.Button buttonWebsite;
        private System.Windows.Forms.Button buttonSupport;
        private System.Windows.Forms.Button buttonDocs;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonUploadDefaultConfig;
        private System.Windows.Forms.Label labelProjectValue;
        private System.Windows.Forms.Label labelProject;
    }
}
