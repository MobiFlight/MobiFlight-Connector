
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
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.LVarExamplePanel = new System.Windows.Forms.Panel();
            this.LVarListButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.AVarExamplePanel = new System.Windows.Forms.Panel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label5 = new System.Windows.Forms.Label();
            this.ExampleLabel = new System.Windows.Forms.Label();
            this.CustomCodePanel = new System.Windows.Forms.Panel();
            this.ExpandButton = new System.Windows.Forms.Button();
            this.PresetPanel = new System.Windows.Forms.Panel();
            this.PresetGroupBox = new System.Windows.Forms.GroupBox();
            this.ShowExpertSerttingsCheckBox = new System.Windows.Forms.CheckBox();
            this.DescriptionLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.PresetComboBox = new System.Windows.Forms.ComboBox();
            this.MatchLabel = new System.Windows.Forms.Label();
            this.FilterGroupBox = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.FilterLabel = new System.Windows.Forms.Label();
            this.FilterTextBox = new System.Windows.Forms.TextBox();
            this.VendorLabel = new System.Windows.Forms.Label();
            this.VendorComboBox = new System.Windows.Forms.ComboBox();
            this.AircraftComboBox = new System.Windows.Forms.ComboBox();
            this.AircraftLabel = new System.Windows.Forms.Label();
            this.SystemLabel = new System.Windows.Forms.Label();
            this.SystemComboBox = new System.Windows.Forms.ComboBox();
            this.transformOptionsGroup1 = new MobiFlight.UI.Panels.Config.TransformOptionsGroup();
            this.ExpertSettingsPanel = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            this.LVarExamplePanel.SuspendLayout();
            this.AVarExamplePanel.SuspendLayout();
            this.CustomCodePanel.SuspendLayout();
            this.PresetPanel.SuspendLayout();
            this.PresetGroupBox.SuspendLayout();
            this.FilterGroupBox.SuspendLayout();
            this.ExpertSettingsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // SimVarNameTextBox
            // 
            this.SimVarNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SimVarNameTextBox.Location = new System.Drawing.Point(75, 3);
            this.SimVarNameTextBox.MaxLength = 1024;
            this.SimVarNameTextBox.Multiline = true;
            this.SimVarNameTextBox.Name = "SimVarNameTextBox";
            this.SimVarNameTextBox.Size = new System.Drawing.Size(459, 21);
            this.SimVarNameTextBox.TabIndex = 0;
            this.SimVarNameTextBox.TextChanged += new System.EventHandler(this.SimVarNameTextBox_TextChanged);
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(3, 16);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(3);
            this.label2.Size = new System.Drawing.Size(598, 26);
            this.label2.TabIndex = 2;
            this.label2.Text = "Define the sim variable name that you would like to read from MSFS2020.";
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.transformOptionsGroup1);
            this.groupBox1.Controls.Add(this.PresetGroupBox);
            this.groupBox1.Controls.Add(this.FilterGroupBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(604, 530);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MSFS2020 (WASM)";
            // 
            // LVarExamplePanel
            // 
            this.LVarExamplePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.LVarExamplePanel.Controls.Add(this.LVarListButton);
            this.LVarExamplePanel.Controls.Add(this.label4);
            this.LVarExamplePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.LVarExamplePanel.Location = new System.Drawing.Point(0, 76);
            this.LVarExamplePanel.Name = "LVarExamplePanel";
            this.LVarExamplePanel.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.LVarExamplePanel.Size = new System.Drawing.Size(595, 26);
            this.LVarExamplePanel.TabIndex = 9;
            // 
            // LVarListButton
            // 
            this.LVarListButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.LVarListButton.Location = new System.Drawing.Point(482, 0);
            this.LVarListButton.Name = "LVarListButton";
            this.LVarListButton.Size = new System.Drawing.Size(103, 26);
            this.LVarListButton.TabIndex = 6;
            this.LVarListButton.Text = "Get List from Sim";
            this.LVarListButton.UseVisualStyleBackColor = true;
            this.LVarListButton.Click += new System.EventHandler(this.GetLVarsListButton_Click);
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Name = "label4";
            this.label4.Padding = new System.Windows.Forms.Padding(74, 3, 3, 3);
            this.label4.Size = new System.Drawing.Size(585, 26);
            this.label4.TabIndex = 5;
            this.label4.Text = "Local Variables (L-Vars) - (L:YourLvarName)";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AVarExamplePanel
            // 
            this.AVarExamplePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AVarExamplePanel.Controls.Add(this.linkLabel1);
            this.AVarExamplePanel.Controls.Add(this.label5);
            this.AVarExamplePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.AVarExamplePanel.Location = new System.Drawing.Point(0, 50);
            this.AVarExamplePanel.Name = "AVarExamplePanel";
            this.AVarExamplePanel.Size = new System.Drawing.Size(595, 26);
            this.AVarExamplePanel.TabIndex = 10;
            // 
            // linkLabel1
            // 
            this.linkLabel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.linkLabel1.Location = new System.Drawing.Point(482, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.linkLabel1.Size = new System.Drawing.Size(113, 26);
            this.linkLabel1.TabIndex = 6;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "see Docs";
            this.linkLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(0, 0);
            this.label5.Name = "label5";
            this.label5.Padding = new System.Windows.Forms.Padding(74, 3, 3, 3);
            this.label5.Size = new System.Drawing.Size(595, 26);
            this.label5.TabIndex = 8;
            this.label5.Text = "Aircraft Variables (A-Vars) - (A:COM ACTIVE FREQUENCY:1, Number)";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ExampleLabel
            // 
            this.ExampleLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ExampleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExampleLabel.Location = new System.Drawing.Point(0, 32);
            this.ExampleLabel.Name = "ExampleLabel";
            this.ExampleLabel.Padding = new System.Windows.Forms.Padding(74, 3, 3, 3);
            this.ExampleLabel.Size = new System.Drawing.Size(595, 18);
            this.ExampleLabel.TabIndex = 4;
            this.ExampleLabel.Text = "Examples:";
            // 
            // CustomCodePanel
            // 
            this.CustomCodePanel.AutoSize = true;
            this.CustomCodePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CustomCodePanel.Controls.Add(this.SimVarNameTextBox);
            this.CustomCodePanel.Controls.Add(this.ExpandButton);
            this.CustomCodePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.CustomCodePanel.Location = new System.Drawing.Point(0, 0);
            this.CustomCodePanel.Name = "CustomCodePanel";
            this.CustomCodePanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.CustomCodePanel.Size = new System.Drawing.Size(595, 32);
            this.CustomCodePanel.TabIndex = 11;
            // 
            // ExpandButton
            // 
            this.ExpandButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ExpandButton.Location = new System.Drawing.Point(540, 2);
            this.ExpandButton.Name = "ExpandButton";
            this.ExpandButton.Size = new System.Drawing.Size(21, 21);
            this.ExpandButton.TabIndex = 8;
            this.ExpandButton.Text = "+";
            this.ExpandButton.UseVisualStyleBackColor = true;
            this.ExpandButton.Click += new System.EventHandler(this.ExpandButton_Click);
            // 
            // PresetPanel
            // 
            this.PresetPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.PresetPanel.Controls.Add(this.MatchLabel);
            this.PresetPanel.Controls.Add(this.PresetComboBox);
            this.PresetPanel.Controls.Add(this.label3);
            this.PresetPanel.Controls.Add(this.DescriptionLabel);
            this.PresetPanel.Controls.Add(this.ShowExpertSerttingsCheckBox);
            this.PresetPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.PresetPanel.Location = new System.Drawing.Point(0, 16);
            this.PresetPanel.Name = "PresetPanel";
            this.PresetPanel.Size = new System.Drawing.Size(595, 108);
            this.PresetPanel.TabIndex = 3;
            // 
            // PresetGroupBox
            // 
            this.PresetGroupBox.AutoSize = true;
            this.PresetGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.PresetGroupBox.Controls.Add(this.ExpertSettingsPanel);
            this.PresetGroupBox.Controls.Add(this.PresetPanel);
            this.PresetGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.PresetGroupBox.Location = new System.Drawing.Point(3, 111);
            this.PresetGroupBox.Name = "PresetGroupBox";
            this.PresetGroupBox.Padding = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.PresetGroupBox.Size = new System.Drawing.Size(598, 226);
            this.PresetGroupBox.TabIndex = 11;
            this.PresetGroupBox.TabStop = false;
            this.PresetGroupBox.Text = "Select Preset";
            // 
            // ShowExpertSerttingsCheckBox
            // 
            this.ShowExpertSerttingsCheckBox.AutoSize = true;
            this.ShowExpertSerttingsCheckBox.Location = new System.Drawing.Point(78, 85);
            this.ShowExpertSerttingsCheckBox.Name = "ShowExpertSerttingsCheckBox";
            this.ShowExpertSerttingsCheckBox.Size = new System.Drawing.Size(114, 17);
            this.ShowExpertSerttingsCheckBox.TabIndex = 0;
            this.ShowExpertSerttingsCheckBox.Text = "Show Preset Code";
            this.ShowExpertSerttingsCheckBox.UseVisualStyleBackColor = true;
            this.ShowExpertSerttingsCheckBox.CheckedChanged += new System.EventHandler(this.ShowExpertSerttingsCheckBox_CheckedChanged);
            // 
            // DescriptionLabel
            // 
            this.DescriptionLabel.AutoEllipsis = true;
            this.DescriptionLabel.BackColor = System.Drawing.SystemColors.Window;
            this.DescriptionLabel.Location = new System.Drawing.Point(75, 43);
            this.DescriptionLabel.Name = "DescriptionLabel";
            this.DescriptionLabel.Padding = new System.Windows.Forms.Padding(5);
            this.DescriptionLabel.Size = new System.Drawing.Size(345, 39);
            this.DescriptionLabel.TabIndex = 18;
            this.DescriptionLabel.Text = "label7";
            // 
            // label3
            // 
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(75, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 17);
            this.label3.TabIndex = 17;
            this.label3.Text = "Description";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PresetComboBox
            // 
            this.PresetComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PresetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PresetComboBox.DropDownWidth = 246;
            this.PresetComboBox.FormattingEnabled = true;
            this.PresetComboBox.Location = new System.Drawing.Point(75, 3);
            this.PresetComboBox.Name = "PresetComboBox";
            this.PresetComboBox.Size = new System.Drawing.Size(345, 21);
            this.PresetComboBox.TabIndex = 7;
            this.PresetComboBox.SelectedIndexChanged += new System.EventHandler(this.PresetComboBox_SelectedIndexChanged);
            // 
            // MatchLabel
            // 
            this.MatchLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.MatchLabel.Location = new System.Drawing.Point(423, 3);
            this.MatchLabel.Name = "MatchLabel";
            this.MatchLabel.Size = new System.Drawing.Size(169, 21);
            this.MatchLabel.TabIndex = 16;
            this.MatchLabel.Text = "{0} Presets match";
            this.MatchLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FilterGroupBox
            // 
            this.FilterGroupBox.Controls.Add(this.button1);
            this.FilterGroupBox.Controls.Add(this.FilterLabel);
            this.FilterGroupBox.Controls.Add(this.FilterTextBox);
            this.FilterGroupBox.Controls.Add(this.VendorLabel);
            this.FilterGroupBox.Controls.Add(this.VendorComboBox);
            this.FilterGroupBox.Controls.Add(this.AircraftComboBox);
            this.FilterGroupBox.Controls.Add(this.AircraftLabel);
            this.FilterGroupBox.Controls.Add(this.SystemLabel);
            this.FilterGroupBox.Controls.Add(this.SystemComboBox);
            this.FilterGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.FilterGroupBox.Location = new System.Drawing.Point(3, 42);
            this.FilterGroupBox.Name = "FilterGroupBox";
            this.FilterGroupBox.Size = new System.Drawing.Size(598, 69);
            this.FilterGroupBox.TabIndex = 11;
            this.FilterGroupBox.TabStop = false;
            this.FilterGroupBox.Text = "Filter Preset List";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(543, 36);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(45, 24);
            this.button1.TabIndex = 15;
            this.button1.Text = "Reset";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // FilterLabel
            // 
            this.FilterLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.FilterLabel.Location = new System.Drawing.Point(420, 17);
            this.FilterLabel.Name = "FilterLabel";
            this.FilterLabel.Size = new System.Drawing.Size(51, 18);
            this.FilterLabel.TabIndex = 10;
            this.FilterLabel.Text = "Search";
            this.FilterLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FilterTextBox
            // 
            this.FilterTextBox.Location = new System.Drawing.Point(423, 38);
            this.FilterTextBox.Name = "FilterTextBox";
            this.FilterTextBox.Size = new System.Drawing.Size(114, 20);
            this.FilterTextBox.TabIndex = 9;
            this.FilterTextBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // VendorLabel
            // 
            this.VendorLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.VendorLabel.Location = new System.Drawing.Point(74, 20);
            this.VendorLabel.Name = "VendorLabel";
            this.VendorLabel.Size = new System.Drawing.Size(51, 18);
            this.VendorLabel.TabIndex = 13;
            this.VendorLabel.Text = "Vendor";
            this.VendorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // VendorComboBox
            // 
            this.VendorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.VendorComboBox.DropDownWidth = 200;
            this.VendorComboBox.FormattingEnabled = true;
            this.VendorComboBox.Location = new System.Drawing.Point(75, 38);
            this.VendorComboBox.Name = "VendorComboBox";
            this.VendorComboBox.Size = new System.Drawing.Size(110, 21);
            this.VendorComboBox.TabIndex = 11;
            this.VendorComboBox.SelectedIndexChanged += new System.EventHandler(this.OnFilter_SelectedIndexChanged);
            // 
            // AircraftComboBox
            // 
            this.AircraftComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AircraftComboBox.DropDownWidth = 200;
            this.AircraftComboBox.FormattingEnabled = true;
            this.AircraftComboBox.Location = new System.Drawing.Point(191, 38);
            this.AircraftComboBox.Name = "AircraftComboBox";
            this.AircraftComboBox.Size = new System.Drawing.Size(110, 21);
            this.AircraftComboBox.TabIndex = 12;
            // 
            // AircraftLabel
            // 
            this.AircraftLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.AircraftLabel.Location = new System.Drawing.Point(188, 17);
            this.AircraftLabel.Name = "AircraftLabel";
            this.AircraftLabel.Size = new System.Drawing.Size(44, 21);
            this.AircraftLabel.TabIndex = 14;
            this.AircraftLabel.Text = "Aircraft";
            this.AircraftLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SystemLabel
            // 
            this.SystemLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.SystemLabel.Location = new System.Drawing.Point(304, 15);
            this.SystemLabel.Name = "SystemLabel";
            this.SystemLabel.Size = new System.Drawing.Size(44, 22);
            this.SystemLabel.TabIndex = 4;
            this.SystemLabel.Text = "System";
            this.SystemLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SystemComboBox
            // 
            this.SystemComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SystemComboBox.DropDownWidth = 200;
            this.SystemComboBox.FormattingEnabled = true;
            this.SystemComboBox.Location = new System.Drawing.Point(307, 38);
            this.SystemComboBox.Name = "SystemComboBox";
            this.SystemComboBox.Size = new System.Drawing.Size(110, 21);
            this.SystemComboBox.TabIndex = 5;
            this.SystemComboBox.SelectedIndexChanged += new System.EventHandler(this.GroupComboBox_SelectedIndexChanged);
            // 
            // transformOptionsGroup1
            // 
            this.transformOptionsGroup1.AutoSize = true;
            this.transformOptionsGroup1.Dock = System.Windows.Forms.DockStyle.Top;
            this.transformOptionsGroup1.Location = new System.Drawing.Point(3, 337);
            this.transformOptionsGroup1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.transformOptionsGroup1.Name = "transformOptionsGroup1";
            this.transformOptionsGroup1.Size = new System.Drawing.Size(598, 93);
            this.transformOptionsGroup1.TabIndex = 7;
            // 
            // ExpertSettingsPanel
            // 
            this.ExpertSettingsPanel.AutoSize = true;
            this.ExpertSettingsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ExpertSettingsPanel.Controls.Add(this.LVarExamplePanel);
            this.ExpertSettingsPanel.Controls.Add(this.AVarExamplePanel);
            this.ExpertSettingsPanel.Controls.Add(this.ExampleLabel);
            this.ExpertSettingsPanel.Controls.Add(this.CustomCodePanel);
            this.ExpertSettingsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.ExpertSettingsPanel.Location = new System.Drawing.Point(0, 124);
            this.ExpertSettingsPanel.Name = "ExpertSettingsPanel";
            this.ExpertSettingsPanel.Size = new System.Drawing.Size(595, 102);
            this.ExpertSettingsPanel.TabIndex = 9;
            // 
            // SimConnectPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.groupBox1);
            this.Name = "SimConnectPanel";
            this.Size = new System.Drawing.Size(604, 530);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.LVarExamplePanel.ResumeLayout(false);
            this.AVarExamplePanel.ResumeLayout(false);
            this.CustomCodePanel.ResumeLayout(false);
            this.CustomCodePanel.PerformLayout();
            this.PresetPanel.ResumeLayout(false);
            this.PresetPanel.PerformLayout();
            this.PresetGroupBox.ResumeLayout(false);
            this.PresetGroupBox.PerformLayout();
            this.FilterGroupBox.ResumeLayout(false);
            this.FilterGroupBox.PerformLayout();
            this.ExpertSettingsPanel.ResumeLayout(false);
            this.ExpertSettingsPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox SimVarNameTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label ExampleLabel;
        private System.Windows.Forms.Panel PresetPanel;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private TransformOptionsGroup transformOptionsGroup1;
        private System.Windows.Forms.ComboBox PresetComboBox;
        private System.Windows.Forms.ComboBox SystemComboBox;
        private System.Windows.Forms.Label SystemLabel;
        private System.Windows.Forms.Panel AVarExamplePanel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel LVarExamplePanel;
        private System.Windows.Forms.Button LVarListButton;
        private System.Windows.Forms.Button ExpandButton;
        private System.Windows.Forms.Label AircraftLabel;
        private System.Windows.Forms.Label VendorLabel;
        private System.Windows.Forms.ComboBox AircraftComboBox;
        private System.Windows.Forms.ComboBox VendorComboBox;
        private System.Windows.Forms.Label FilterLabel;
        private System.Windows.Forms.TextBox FilterTextBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label MatchLabel;
        private System.Windows.Forms.GroupBox PresetGroupBox;
        private System.Windows.Forms.GroupBox FilterGroupBox;
        private System.Windows.Forms.Panel CustomCodePanel;
        private System.Windows.Forms.Label DescriptionLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox ShowExpertSerttingsCheckBox;
        private System.Windows.Forms.Panel ExpertSettingsPanel;
    }
}
