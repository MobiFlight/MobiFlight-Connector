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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ArcazePanel));
            this.arcazeModuleSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.numModulesLabel = new System.Windows.Forms.Label();
            this.numModulesNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.arcazeModuleTypeComboBox = new System.Windows.Forms.ComboBox();
            this.globalBrightnessLabel = new System.Windows.Forms.Label();
            this.globalBrightnessTrackBar = new System.Windows.Forms.TrackBar();
            this.arcazeModuleTypeLabel = new System.Windows.Forms.Label();
            this.arcazeModulesGroupBox = new System.Windows.Forms.GroupBox();
            this.ArcazeModuleTreeView = new System.Windows.Forms.TreeView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ArcazeSupportEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.ArcazePanelSettingsPanel = new System.Windows.Forms.Panel();
            this.ArcazeNoModulesFoundPanel = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.arcazeModuleSettingsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numModulesNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.globalBrightnessTrackBar)).BeginInit();
            this.arcazeModulesGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.ArcazePanelSettingsPanel.SuspendLayout();
            this.ArcazeNoModulesFoundPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // arcazeModuleSettingsGroupBox
            // 
            resources.ApplyResources(this.arcazeModuleSettingsGroupBox, "arcazeModuleSettingsGroupBox");
            this.arcazeModuleSettingsGroupBox.Controls.Add(this.numModulesLabel);
            this.arcazeModuleSettingsGroupBox.Controls.Add(this.numModulesNumericUpDown);
            this.arcazeModuleSettingsGroupBox.Controls.Add(this.arcazeModuleTypeComboBox);
            this.arcazeModuleSettingsGroupBox.Controls.Add(this.globalBrightnessLabel);
            this.arcazeModuleSettingsGroupBox.Controls.Add(this.globalBrightnessTrackBar);
            this.arcazeModuleSettingsGroupBox.Controls.Add(this.arcazeModuleTypeLabel);
            this.arcazeModuleSettingsGroupBox.Name = "arcazeModuleSettingsGroupBox";
            this.arcazeModuleSettingsGroupBox.TabStop = false;
            // 
            // numModulesLabel
            // 
            resources.ApplyResources(this.numModulesLabel, "numModulesLabel");
            this.numModulesLabel.Name = "numModulesLabel";
            // 
            // numModulesNumericUpDown
            // 
            resources.ApplyResources(this.numModulesNumericUpDown, "numModulesNumericUpDown");
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
            this.numModulesNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numModulesNumericUpDown.ValueChanged += new System.EventHandler(this.numModulesNumericUpDown_ValueChanged);
            // 
            // arcazeModuleTypeComboBox
            // 
            resources.ApplyResources(this.arcazeModuleTypeComboBox, "arcazeModuleTypeComboBox");
            this.arcazeModuleTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.arcazeModuleTypeComboBox.FormattingEnabled = true;
            this.arcazeModuleTypeComboBox.Items.AddRange(new object[] {
            resources.GetString("arcazeModuleTypeComboBox.Items")});
            this.arcazeModuleTypeComboBox.Name = "arcazeModuleTypeComboBox";
            this.arcazeModuleTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.arcazeModuleTypeComboBox_SelectedIndexChanged);
            // 
            // globalBrightnessLabel
            // 
            resources.ApplyResources(this.globalBrightnessLabel, "globalBrightnessLabel");
            this.globalBrightnessLabel.Name = "globalBrightnessLabel";
            // 
            // globalBrightnessTrackBar
            // 
            resources.ApplyResources(this.globalBrightnessTrackBar, "globalBrightnessTrackBar");
            this.globalBrightnessTrackBar.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.globalBrightnessTrackBar.Maximum = 9;
            this.globalBrightnessTrackBar.Minimum = 1;
            this.globalBrightnessTrackBar.Name = "globalBrightnessTrackBar";
            this.globalBrightnessTrackBar.Value = 9;
            // 
            // arcazeModuleTypeLabel
            // 
            resources.ApplyResources(this.arcazeModuleTypeLabel, "arcazeModuleTypeLabel");
            this.arcazeModuleTypeLabel.Name = "arcazeModuleTypeLabel";
            // 
            // arcazeModulesGroupBox
            // 
            resources.ApplyResources(this.arcazeModulesGroupBox, "arcazeModulesGroupBox");
            this.arcazeModulesGroupBox.Controls.Add(this.ArcazeModuleTreeView);
            this.arcazeModulesGroupBox.Name = "arcazeModulesGroupBox";
            this.arcazeModulesGroupBox.TabStop = false;
            // 
            // ArcazeModuleTreeView
            // 
            resources.ApplyResources(this.ArcazeModuleTreeView, "ArcazeModuleTreeView");
            this.ArcazeModuleTreeView.Name = "ArcazeModuleTreeView";
            this.ArcazeModuleTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("ArcazeModuleTreeView.Nodes"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("ArcazeModuleTreeView.Nodes1")))});
            this.ArcazeModuleTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ArcazeModuleTreeView_AfterSelect);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.ArcazeSupportEnabledCheckBox);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // ArcazeSupportEnabledCheckBox
            // 
            resources.ApplyResources(this.ArcazeSupportEnabledCheckBox, "ArcazeSupportEnabledCheckBox");
            this.ArcazeSupportEnabledCheckBox.Name = "ArcazeSupportEnabledCheckBox";
            this.ArcazeSupportEnabledCheckBox.UseVisualStyleBackColor = true;
            this.ArcazeSupportEnabledCheckBox.CheckedChanged += new System.EventHandler(this.ArcazeSupportEnabledCheckBox_CheckedChanged);
            // 
            // ArcazePanelSettingsPanel
            // 
            resources.ApplyResources(this.ArcazePanelSettingsPanel, "ArcazePanelSettingsPanel");
            this.ArcazePanelSettingsPanel.Controls.Add(this.arcazeModuleSettingsGroupBox);
            this.ArcazePanelSettingsPanel.Controls.Add(this.arcazeModulesGroupBox);
            this.ArcazePanelSettingsPanel.Name = "ArcazePanelSettingsPanel";
            // 
            // ArcazeNoModulesFoundPanel
            // 
            resources.ApplyResources(this.ArcazeNoModulesFoundPanel, "ArcazeNoModulesFoundPanel");
            this.ArcazeNoModulesFoundPanel.Controls.Add(this.button1);
            this.ArcazeNoModulesFoundPanel.Controls.Add(this.label1);
            this.ArcazeNoModulesFoundPanel.Name = "ArcazeNoModulesFoundPanel";
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // ArcazePanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ArcazeNoModulesFoundPanel);
            this.Controls.Add(this.ArcazePanelSettingsPanel);
            this.Controls.Add(this.groupBox1);
            this.Name = "ArcazePanel";
            this.arcazeModuleSettingsGroupBox.ResumeLayout(false);
            this.arcazeModuleSettingsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numModulesNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.globalBrightnessTrackBar)).EndInit();
            this.arcazeModulesGroupBox.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ArcazePanelSettingsPanel.ResumeLayout(false);
            this.ArcazeNoModulesFoundPanel.ResumeLayout(false);
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
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox ArcazeSupportEnabledCheckBox;
        private System.Windows.Forms.Panel ArcazePanelSettingsPanel;
        private System.Windows.Forms.Panel ArcazeNoModulesFoundPanel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
    }
}
