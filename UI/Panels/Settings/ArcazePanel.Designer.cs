namespace MobiFlight.UI.Panels.Settings
{
    partial class ArcazePanel
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Arcaze_1");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Arcaze_2");
            this.arcazeModuleSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.numModulesLabel = new System.Windows.Forms.Label();
            this.numModulesNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.arcazeModuleTypeComboBox = new System.Windows.Forms.ComboBox();
            this.globalBrightnessLabel = new System.Windows.Forms.Label();
            this.globalBrightnessTrackBar = new System.Windows.Forms.TrackBar();
            this.arcazeModuleTypeLabel = new System.Windows.Forms.Label();
            this.arcazeModulesGroupBox = new System.Windows.Forms.GroupBox();
            this.ArcazeModuleTreeView = new System.Windows.Forms.TreeView();
            this.arcazeModuleSettingsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numModulesNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.globalBrightnessTrackBar)).BeginInit();
            this.arcazeModulesGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // arcazeModuleSettingsGroupBox
            // 
            this.arcazeModuleSettingsGroupBox.Controls.Add(this.numModulesLabel);
            this.arcazeModuleSettingsGroupBox.Controls.Add(this.numModulesNumericUpDown);
            this.arcazeModuleSettingsGroupBox.Controls.Add(this.arcazeModuleTypeComboBox);
            this.arcazeModuleSettingsGroupBox.Controls.Add(this.globalBrightnessLabel);
            this.arcazeModuleSettingsGroupBox.Controls.Add(this.globalBrightnessTrackBar);
            this.arcazeModuleSettingsGroupBox.Controls.Add(this.arcazeModuleTypeLabel);
            this.arcazeModuleSettingsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.arcazeModuleSettingsGroupBox.Location = new System.Drawing.Point(179, 0);
            this.arcazeModuleSettingsGroupBox.Name = "arcazeModuleSettingsGroupBox";
            this.arcazeModuleSettingsGroupBox.Size = new System.Drawing.Size(379, 524);
            this.arcazeModuleSettingsGroupBox.TabIndex = 6;
            this.arcazeModuleSettingsGroupBox.TabStop = false;
            this.arcazeModuleSettingsGroupBox.Text = "Module Settings";
            // 
            // numModulesLabel
            // 
            this.numModulesLabel.AutoSize = true;
            this.numModulesLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.numModulesLabel.Location = new System.Drawing.Point(47, 55);
            this.numModulesLabel.Name = "numModulesLabel";
            this.numModulesLabel.Size = new System.Drawing.Size(35, 13);
            this.numModulesLabel.TabIndex = 13;
            this.numModulesLabel.Text = "Count";
            // 
            // numModulesNumericUpDown
            // 
            this.numModulesNumericUpDown.Location = new System.Drawing.Point(88, 53);
            this.numModulesNumericUpDown.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.numModulesNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numModulesNumericUpDown.Name = "numModulesNumericUpDown";
            this.numModulesNumericUpDown.Size = new System.Drawing.Size(38, 20);
            this.numModulesNumericUpDown.TabIndex = 12;
            this.numModulesNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numModulesNumericUpDown.ValueChanged += new System.EventHandler(this.numModulesNumericUpDown_ValueChanged);
            // 
            // arcazeModuleTypeComboBox
            // 
            this.arcazeModuleTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.arcazeModuleTypeComboBox.FormattingEnabled = true;
            this.arcazeModuleTypeComboBox.Items.AddRange(new object[] {
            "Modul A"});
            this.arcazeModuleTypeComboBox.Location = new System.Drawing.Point(88, 19);
            this.arcazeModuleTypeComboBox.Name = "arcazeModuleTypeComboBox";
            this.arcazeModuleTypeComboBox.Size = new System.Drawing.Size(104, 21);
            this.arcazeModuleTypeComboBox.TabIndex = 11;
            this.arcazeModuleTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.arcazeModuleTypeComboBox_SelectedIndexChanged);
            // 
            // globalBrightnessLabel
            // 
            this.globalBrightnessLabel.AutoSize = true;
            this.globalBrightnessLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.globalBrightnessLabel.Location = new System.Drawing.Point(37, 86);
            this.globalBrightnessLabel.Name = "globalBrightnessLabel";
            this.globalBrightnessLabel.Size = new System.Drawing.Size(45, 26);
            this.globalBrightnessLabel.TabIndex = 10;
            this.globalBrightnessLabel.Text = "Global \r\nintensity";
            this.globalBrightnessLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // globalBrightnessTrackBar
            // 
            this.globalBrightnessTrackBar.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.globalBrightnessTrackBar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.globalBrightnessTrackBar.Location = new System.Drawing.Point(80, 84);
            this.globalBrightnessTrackBar.Maximum = 9;
            this.globalBrightnessTrackBar.Minimum = 1;
            this.globalBrightnessTrackBar.Name = "globalBrightnessTrackBar";
            this.globalBrightnessTrackBar.Size = new System.Drawing.Size(122, 45);
            this.globalBrightnessTrackBar.TabIndex = 9;
            this.globalBrightnessTrackBar.Value = 9;
            // 
            // arcazeModuleTypeLabel
            // 
            this.arcazeModuleTypeLabel.AutoSize = true;
            this.arcazeModuleTypeLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.arcazeModuleTypeLabel.Location = new System.Drawing.Point(5, 22);
            this.arcazeModuleTypeLabel.Name = "arcazeModuleTypeLabel";
            this.arcazeModuleTypeLabel.Size = new System.Drawing.Size(80, 13);
            this.arcazeModuleTypeLabel.TabIndex = 8;
            this.arcazeModuleTypeLabel.Text = "Extension Type";
            // 
            // arcazeModulesGroupBox
            // 
            this.arcazeModulesGroupBox.Controls.Add(this.ArcazeModuleTreeView);
            this.arcazeModulesGroupBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.arcazeModulesGroupBox.Location = new System.Drawing.Point(0, 0);
            this.arcazeModulesGroupBox.Name = "arcazeModulesGroupBox";
            this.arcazeModulesGroupBox.Size = new System.Drawing.Size(179, 524);
            this.arcazeModulesGroupBox.TabIndex = 14;
            this.arcazeModulesGroupBox.TabStop = false;
            this.arcazeModulesGroupBox.Text = "Available Arcaze Modules";
            // 
            // ArcazeModuleTreeView
            // 
            this.ArcazeModuleTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ArcazeModuleTreeView.Location = new System.Drawing.Point(3, 16);
            this.ArcazeModuleTreeView.Name = "ArcazeModuleTreeView";
            treeNode1.Name = "Module_0";
            treeNode1.Text = "Arcaze_1";
            treeNode2.Name = "MF_MICRO_COM5";
            treeNode2.Text = "Arcaze_2";
            this.ArcazeModuleTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            this.ArcazeModuleTreeView.Size = new System.Drawing.Size(173, 505);
            this.ArcazeModuleTreeView.TabIndex = 3;
            this.ArcazeModuleTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ArcazeModuleTreeView_AfterSelect);
            // 
            // ArcazePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.arcazeModuleSettingsGroupBox);
            this.Controls.Add(this.arcazeModulesGroupBox);
            this.Name = "ArcazePanel";
            this.Size = new System.Drawing.Size(558, 524);
            this.arcazeModuleSettingsGroupBox.ResumeLayout(false);
            this.arcazeModuleSettingsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numModulesNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.globalBrightnessTrackBar)).EndInit();
            this.arcazeModulesGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox arcazeModuleSettingsGroupBox;
        private System.Windows.Forms.Label numModulesLabel;
        private System.Windows.Forms.NumericUpDown numModulesNumericUpDown;
        private System.Windows.Forms.ComboBox arcazeModuleTypeComboBox;
        private System.Windows.Forms.Label globalBrightnessLabel;
        private System.Windows.Forms.TrackBar globalBrightnessTrackBar;
        private System.Windows.Forms.Label arcazeModuleTypeLabel;
        private System.Windows.Forms.GroupBox arcazeModulesGroupBox;
        private System.Windows.Forms.TreeView ArcazeModuleTreeView;
    }
}
