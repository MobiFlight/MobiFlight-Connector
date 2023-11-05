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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.moduleNameTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TypeValueLabel = new System.Windows.Forms.Label();
            this.TypeLabel = new System.Windows.Forms.Label();
            this.SerialValueLabel = new System.Windows.Forms.Label();
            this.SerialLabel = new System.Windows.Forms.Label();
            this.PortValueLabel = new System.Windows.Forms.Label();
            this.PortLabel = new System.Windows.Forms.Label();
            this.FirmwareValueLabel = new System.Windows.Forms.Label();
            this.FirmwareLabel = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.moduleNameTextBox);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(183, 48);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Name";
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.TypeValueLabel);
            this.groupBox1.Controls.Add(this.TypeLabel);
            this.groupBox1.Controls.Add(this.SerialValueLabel);
            this.groupBox1.Controls.Add(this.SerialLabel);
            this.groupBox1.Controls.Add(this.PortValueLabel);
            this.groupBox1.Controls.Add(this.PortLabel);
            this.groupBox1.Controls.Add(this.FirmwareValueLabel);
            this.groupBox1.Controls.Add(this.FirmwareLabel);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 48);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(183, 105);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Information";
            // 
            // TypeValueLabel
            // 
            this.TypeValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TypeValueLabel.Location = new System.Drawing.Point(73, 74);
            this.TypeValueLabel.Name = "TypeValueLabel";
            this.TypeValueLabel.Size = new System.Drawing.Size(96, 24);
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
            this.SerialValueLabel.Size = new System.Drawing.Size(96, 13);
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
            this.PortValueLabel.Size = new System.Drawing.Size(96, 13);
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
            this.FirmwareValueLabel.Size = new System.Drawing.Size(96, 13);
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
            // MFModulePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "MFModulePanel";
            this.Size = new System.Drawing.Size(183, 241);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox moduleNameTextBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label FirmwareValueLabel;
        private System.Windows.Forms.Label FirmwareLabel;
        private System.Windows.Forms.Label SerialValueLabel;
        private System.Windows.Forms.Label SerialLabel;
        private System.Windows.Forms.Label PortValueLabel;
        private System.Windows.Forms.Label PortLabel;
        private System.Windows.Forms.Label TypeValueLabel;
        private System.Windows.Forms.Label TypeLabel;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
