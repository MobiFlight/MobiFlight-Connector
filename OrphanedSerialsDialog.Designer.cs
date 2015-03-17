namespace MobiFlight
{
    partial class OrphanedSerialsDialog
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

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrphanedSerialsDialog));
            this.InfoLabel = new System.Windows.Forms.Label();
            this.OkCancelPanel = new System.Windows.Forms.Panel();
            this.DlgCancelButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.orphanedSerialsGroupBox = new System.Windows.Forms.GroupBox();
            this.orphanedSerialsListBox = new System.Windows.Forms.ListBox();
            this.connectedModulesGroupBox = new System.Windows.Forms.GroupBox();
            this.connectedModulesAssignButton = new System.Windows.Forms.Button();
            this.connectedModulesComboBox = new System.Windows.Forms.ComboBox();
            this.OkCancelPanel.SuspendLayout();
            this.orphanedSerialsGroupBox.SuspendLayout();
            this.connectedModulesGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // InfoLabel
            // 
            resources.ApplyResources(this.InfoLabel, "InfoLabel");
            this.InfoLabel.ImageKey = global::MobiFlight.ProjectMessages.conf;
            this.InfoLabel.Name = "InfoLabel";
            // 
            // OkCancelPanel
            // 
            resources.ApplyResources(this.OkCancelPanel, "OkCancelPanel");
            this.OkCancelPanel.Controls.Add(this.DlgCancelButton);
            this.OkCancelPanel.Controls.Add(this.OkButton);
            this.OkCancelPanel.Name = "OkCancelPanel";
            // 
            // DlgCancelButton
            // 
            resources.ApplyResources(this.DlgCancelButton, "DlgCancelButton");
            this.DlgCancelButton.ImageKey = global::MobiFlight.ProjectMessages.conf;
            this.DlgCancelButton.Name = "DlgCancelButton";
            this.DlgCancelButton.UseVisualStyleBackColor = true;
            this.DlgCancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // OkButton
            // 
            resources.ApplyResources(this.OkButton, "OkButton");
            this.OkButton.ImageKey = global::MobiFlight.ProjectMessages.conf;
            this.OkButton.Name = "OkButton";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // orphanedSerialsGroupBox
            // 
            resources.ApplyResources(this.orphanedSerialsGroupBox, "orphanedSerialsGroupBox");
            this.orphanedSerialsGroupBox.Controls.Add(this.orphanedSerialsListBox);
            this.orphanedSerialsGroupBox.Name = "orphanedSerialsGroupBox";
            this.orphanedSerialsGroupBox.TabStop = false;
            // 
            // orphanedSerialsListBox
            // 
            resources.ApplyResources(this.orphanedSerialsListBox, "orphanedSerialsListBox");
            this.orphanedSerialsListBox.FormattingEnabled = true;
            this.orphanedSerialsListBox.Items.AddRange(new object[] {
            resources.GetString("orphanedSerialsListBox.Items"),
            resources.GetString("orphanedSerialsListBox.Items1"),
            resources.GetString("orphanedSerialsListBox.Items2"),
            resources.GetString("orphanedSerialsListBox.Items3")});
            this.orphanedSerialsListBox.Name = "orphanedSerialsListBox";
            this.orphanedSerialsListBox.SelectedIndexChanged += new System.EventHandler(this.orphanedSerialsListBox_SelectedIndexChanged);
            // 
            // connectedModulesGroupBox
            // 
            resources.ApplyResources(this.connectedModulesGroupBox, "connectedModulesGroupBox");
            this.connectedModulesGroupBox.Controls.Add(this.connectedModulesAssignButton);
            this.connectedModulesGroupBox.Controls.Add(this.connectedModulesComboBox);
            this.connectedModulesGroupBox.Name = "connectedModulesGroupBox";
            this.connectedModulesGroupBox.TabStop = false;
            // 
            // connectedModulesAssignButton
            // 
            resources.ApplyResources(this.connectedModulesAssignButton, "connectedModulesAssignButton");
            this.connectedModulesAssignButton.ImageKey = global::MobiFlight.ProjectMessages.conf;
            this.connectedModulesAssignButton.Name = "connectedModulesAssignButton";
            this.connectedModulesAssignButton.UseVisualStyleBackColor = true;
            this.connectedModulesAssignButton.Click += new System.EventHandler(this.connectedModulesAssignButton_Click);
            // 
            // connectedModulesComboBox
            // 
            resources.ApplyResources(this.connectedModulesComboBox, "connectedModulesComboBox");
            this.connectedModulesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.connectedModulesComboBox.FormattingEnabled = true;
            this.connectedModulesComboBox.Items.AddRange(new object[] {
            resources.GetString("connectedModulesComboBox.Items"),
            resources.GetString("connectedModulesComboBox.Items1"),
            resources.GetString("connectedModulesComboBox.Items2")});
            this.connectedModulesComboBox.Name = "connectedModulesComboBox";
            // 
            // OrphanedSerialsDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.orphanedSerialsGroupBox);
            this.Controls.Add(this.connectedModulesGroupBox);
            this.Controls.Add(this.OkCancelPanel);
            this.Controls.Add(this.InfoLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OrphanedSerialsDialog";
            this.OkCancelPanel.ResumeLayout(false);
            this.orphanedSerialsGroupBox.ResumeLayout(false);
            this.connectedModulesGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label InfoLabel;
        private System.Windows.Forms.Panel OkCancelPanel;
        private System.Windows.Forms.Button DlgCancelButton;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.GroupBox orphanedSerialsGroupBox;
        private System.Windows.Forms.ListBox orphanedSerialsListBox;
        private System.Windows.Forms.GroupBox connectedModulesGroupBox;
        private System.Windows.Forms.Button connectedModulesAssignButton;
        private System.Windows.Forms.ComboBox connectedModulesComboBox;
    }
}