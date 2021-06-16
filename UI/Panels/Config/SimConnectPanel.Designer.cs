
namespace MobiFlight.UI.Panels.Config
{
    partial class SimConnectPanel
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
            this.SimVarNameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.PresetComboBox = new System.Windows.Forms.ComboBox();
            this.EventLabel = new System.Windows.Forms.Label();
            this.GroupComboBox = new System.Windows.Forms.ComboBox();
            this.DeviceLabel = new System.Windows.Forms.Label();
            this.transformOptionsGroup1 = new MobiFlight.UI.Panels.Config.TransformOptionsGroup();
            this.label5 = new System.Windows.Forms.Label();
            this.LVarExamplePanel = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.AVarExamplePanel = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.LVarExamplePanel.SuspendLayout();
            this.AVarExamplePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // SimVarNameTextBox
            // 
            this.SimVarNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SimVarNameTextBox.Location = new System.Drawing.Point(68, 60);
            this.SimVarNameTextBox.Name = "SimVarNameTextBox";
            this.SimVarNameTextBox.Size = new System.Drawing.Size(307, 20);
            this.SimVarNameTextBox.TabIndex = 0;
            this.SimVarNameTextBox.TextChanged += new System.EventHandler(this.SimVarNameTextBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Variable";
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(3, 16);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(3);
            this.label2.Size = new System.Drawing.Size(385, 26);
            this.label2.TabIndex = 2;
            this.label2.Text = "Define the sim variable name that you would like to read from MSFS2020.";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.transformOptionsGroup1);
            this.groupBox1.Controls.Add(this.AVarExamplePanel);
            this.groupBox1.Controls.Add(this.LVarExamplePanel);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(391, 358);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MSFS2020 (WASM)";
            // 
            // linkLabel1
            // 
            this.linkLabel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.linkLabel1.Location = new System.Drawing.Point(272, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.linkLabel1.Size = new System.Drawing.Size(113, 28);
            this.linkLabel1.TabIndex = 6;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "see Docs";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(3);
            this.label4.Size = new System.Drawing.Size(375, 28);
            this.label4.TabIndex = 5;
            this.label4.Text = "Local Variables (L-Vars) - (L:YourLvarName)";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 126);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(3);
            this.label3.Size = new System.Drawing.Size(385, 24);
            this.label3.TabIndex = 4;
            this.label3.Text = "Examples:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.PresetComboBox);
            this.panel1.Controls.Add(this.EventLabel);
            this.panel1.Controls.Add(this.GroupComboBox);
            this.panel1.Controls.Add(this.DeviceLabel);
            this.panel1.Controls.Add(this.SimVarNameTextBox);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 42);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(385, 84);
            this.panel1.TabIndex = 3;
            // 
            // PresetComboBox
            // 
            this.PresetComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PresetComboBox.DropDownWidth = 300;
            this.PresetComboBox.FormattingEnabled = true;
            this.PresetComboBox.Location = new System.Drawing.Point(68, 33);
            this.PresetComboBox.Name = "PresetComboBox";
            this.PresetComboBox.Size = new System.Drawing.Size(307, 21);
            this.PresetComboBox.TabIndex = 7;
            this.PresetComboBox.SelectedIndexChanged += new System.EventHandler(this.PresetComboBox_SelectedIndexChanged);
            // 
            // EventLabel
            // 
            this.EventLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.EventLabel.Location = new System.Drawing.Point(11, 33);
            this.EventLabel.Name = "EventLabel";
            this.EventLabel.Size = new System.Drawing.Size(51, 18);
            this.EventLabel.TabIndex = 6;
            this.EventLabel.Text = "Preset";
            this.EventLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GroupComboBox
            // 
            this.GroupComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupComboBox.FormattingEnabled = true;
            this.GroupComboBox.Location = new System.Drawing.Point(68, 7);
            this.GroupComboBox.Name = "GroupComboBox";
            this.GroupComboBox.Size = new System.Drawing.Size(307, 21);
            this.GroupComboBox.TabIndex = 5;
            this.GroupComboBox.SelectedIndexChanged += new System.EventHandler(this.GroupComboBox_SelectedIndexChanged);
            // 
            // DeviceLabel
            // 
            this.DeviceLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DeviceLabel.Location = new System.Drawing.Point(11, 7);
            this.DeviceLabel.Name = "DeviceLabel";
            this.DeviceLabel.Size = new System.Drawing.Size(51, 18);
            this.DeviceLabel.TabIndex = 4;
            this.DeviceLabel.Text = "Group";
            this.DeviceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // transformOptionsGroup1
            // 
            this.transformOptionsGroup1.AutoSize = true;
            this.transformOptionsGroup1.Dock = System.Windows.Forms.DockStyle.Top;
            this.transformOptionsGroup1.Location = new System.Drawing.Point(3, 206);
            this.transformOptionsGroup1.Name = "transformOptionsGroup1";
            this.transformOptionsGroup1.Size = new System.Drawing.Size(385, 93);
            this.transformOptionsGroup1.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(0, 0);
            this.label5.Name = "label5";
            this.label5.Padding = new System.Windows.Forms.Padding(3);
            this.label5.Size = new System.Drawing.Size(385, 28);
            this.label5.TabIndex = 8;
            this.label5.Text = "Aircraft Variables (A-Vars) - (A:COM ACTIVE:1,Number)";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LVarExamplePanel
            // 
            this.LVarExamplePanel.Controls.Add(this.button1);
            this.LVarExamplePanel.Controls.Add(this.label4);
            this.LVarExamplePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.LVarExamplePanel.Location = new System.Drawing.Point(3, 150);
            this.LVarExamplePanel.Name = "LVarExamplePanel";
            this.LVarExamplePanel.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.LVarExamplePanel.Size = new System.Drawing.Size(385, 28);
            this.LVarExamplePanel.TabIndex = 9;
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Right;
            this.button1.Location = new System.Drawing.Point(272, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(103, 28);
            this.button1.TabIndex = 6;
            this.button1.Text = "Get List from Sim";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // AVarExamplePanel
            // 
            this.AVarExamplePanel.Controls.Add(this.linkLabel1);
            this.AVarExamplePanel.Controls.Add(this.label5);
            this.AVarExamplePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.AVarExamplePanel.Location = new System.Drawing.Point(3, 178);
            this.AVarExamplePanel.Name = "AVarExamplePanel";
            this.AVarExamplePanel.Size = new System.Drawing.Size(385, 28);
            this.AVarExamplePanel.TabIndex = 10;
            // 
            // SimConnectPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "SimConnectPanel";
            this.Size = new System.Drawing.Size(391, 358);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.LVarExamplePanel.ResumeLayout(false);
            this.AVarExamplePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox SimVarNameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private TransformOptionsGroup transformOptionsGroup1;
        private System.Windows.Forms.ComboBox PresetComboBox;
        private System.Windows.Forms.Label EventLabel;
        private System.Windows.Forms.ComboBox GroupComboBox;
        private System.Windows.Forms.Label DeviceLabel;
        private System.Windows.Forms.Panel AVarExamplePanel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel LVarExamplePanel;
        private System.Windows.Forms.Button button1;
    }
}
