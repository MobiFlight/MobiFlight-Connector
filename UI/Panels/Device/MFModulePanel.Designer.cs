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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MFModulePanel));
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
            this.labelProjectValue = new System.Windows.Forms.Label();
            this.groupBoxDetails = new System.Windows.Forms.GroupBox();
            this.labelProject = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonUploadDefaultConfig = new System.Windows.Forms.Button();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.buttonSupport = new System.Windows.Forms.Button();
            this.buttonDocs = new System.Windows.Forms.Button();
            this.buttonWebsite = new System.Windows.Forms.Button();
            this.groupBoxName.SuspendLayout();
            this.groupBoxInformation.SuspendLayout();
            this.groupBoxDetails.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBoxName
            // 
            this.groupBoxName.Controls.Add(this.moduleNameTextBox);
            resources.ApplyResources(this.groupBoxName, "groupBoxName");
            this.groupBoxName.Name = "groupBoxName";
            this.groupBoxName.TabStop = false;
            // 
            // moduleNameTextBox
            // 
            resources.ApplyResources(this.moduleNameTextBox, "moduleNameTextBox");
            this.moduleNameTextBox.Name = "moduleNameTextBox";
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
            resources.ApplyResources(this.groupBoxInformation, "groupBoxInformation");
            this.groupBoxInformation.Name = "groupBoxInformation";
            this.groupBoxInformation.TabStop = false;
            // 
            // TypeValueLabel
            // 
            resources.ApplyResources(this.TypeValueLabel, "TypeValueLabel");
            this.TypeValueLabel.Name = "TypeValueLabel";
            this.toolTip1.SetToolTip(this.TypeValueLabel, resources.GetString("TypeValueLabel.ToolTip"));
            // 
            // TypeLabel
            // 
            resources.ApplyResources(this.TypeLabel, "TypeLabel");
            this.TypeLabel.Name = "TypeLabel";
            // 
            // SerialValueLabel
            // 
            resources.ApplyResources(this.SerialValueLabel, "SerialValueLabel");
            this.SerialValueLabel.Name = "SerialValueLabel";
            // 
            // SerialLabel
            // 
            resources.ApplyResources(this.SerialLabel, "SerialLabel");
            this.SerialLabel.Name = "SerialLabel";
            // 
            // PortValueLabel
            // 
            resources.ApplyResources(this.PortValueLabel, "PortValueLabel");
            this.PortValueLabel.Name = "PortValueLabel";
            // 
            // PortLabel
            // 
            resources.ApplyResources(this.PortLabel, "PortLabel");
            this.PortLabel.Name = "PortLabel";
            // 
            // FirmwareValueLabel
            // 
            resources.ApplyResources(this.FirmwareValueLabel, "FirmwareValueLabel");
            this.FirmwareValueLabel.Name = "FirmwareValueLabel";
            // 
            // FirmwareLabel
            // 
            resources.ApplyResources(this.FirmwareLabel, "FirmwareLabel");
            this.FirmwareLabel.Name = "FirmwareLabel";
            // 
            // labelProjectValue
            // 
            resources.ApplyResources(this.labelProjectValue, "labelProjectValue");
            this.labelProjectValue.Name = "labelProjectValue";
            this.toolTip1.SetToolTip(this.labelProjectValue, resources.GetString("labelProjectValue.ToolTip"));
            // 
            // groupBoxDetails
            // 
            resources.ApplyResources(this.groupBoxDetails, "groupBoxDetails");
            this.groupBoxDetails.Controls.Add(this.labelProjectValue);
            this.groupBoxDetails.Controls.Add(this.labelProject);
            this.groupBoxDetails.Controls.Add(this.panel1);
            this.groupBoxDetails.Controls.Add(this.pictureBoxLogo);
            this.groupBoxDetails.Controls.Add(this.buttonSupport);
            this.groupBoxDetails.Controls.Add(this.buttonDocs);
            this.groupBoxDetails.Controls.Add(this.buttonWebsite);
            this.groupBoxDetails.Name = "groupBoxDetails";
            this.groupBoxDetails.TabStop = false;
            // 
            // labelProject
            // 
            resources.ApplyResources(this.labelProject, "labelProject");
            this.labelProject.Name = "labelProject";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.buttonUploadDefaultConfig);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // buttonUploadDefaultConfig
            // 
            resources.ApplyResources(this.buttonUploadDefaultConfig, "buttonUploadDefaultConfig");
            this.buttonUploadDefaultConfig.Name = "buttonUploadDefaultConfig";
            this.buttonUploadDefaultConfig.UseVisualStyleBackColor = true;
            // 
            // pictureBoxLogo
            // 
            resources.ApplyResources(this.pictureBoxLogo, "pictureBoxLogo");
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.TabStop = false;
            // 
            // buttonSupport
            // 
            resources.ApplyResources(this.buttonSupport, "buttonSupport");
            this.buttonSupport.Name = "buttonSupport";
            this.buttonSupport.UseVisualStyleBackColor = true;
            // 
            // buttonDocs
            // 
            resources.ApplyResources(this.buttonDocs, "buttonDocs");
            this.buttonDocs.Name = "buttonDocs";
            this.buttonDocs.UseVisualStyleBackColor = true;
            // 
            // buttonWebsite
            // 
            resources.ApplyResources(this.buttonWebsite, "buttonWebsite");
            this.buttonWebsite.Name = "buttonWebsite";
            this.buttonWebsite.UseVisualStyleBackColor = true;
            // 
            // MFModulePanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxDetails);
            this.Controls.Add(this.groupBoxInformation);
            this.Controls.Add(this.groupBoxName);
            this.Name = "MFModulePanel";
            this.groupBoxName.ResumeLayout(false);
            this.groupBoxName.PerformLayout();
            this.groupBoxInformation.ResumeLayout(false);
            this.groupBoxInformation.PerformLayout();
            this.groupBoxDetails.ResumeLayout(false);
            this.groupBoxDetails.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
