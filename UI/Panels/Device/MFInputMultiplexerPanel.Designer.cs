namespace MobiFlight.UI.Panels.Settings
{
    partial class MFInputMultiplexerPanel
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MFInputMultiplexerPanel));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mfNumModulesComboBox = new System.Windows.Forms.ComboBox();
            this.numberOfModulesLabel = new System.Windows.Forms.Label();
            this.mfPin1Label = new System.Windows.Forms.Label();
            this.mfPin1ComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.muxDrvPanel = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.mfNumModulesComboBox);
            this.groupBox1.Controls.Add(this.numberOfModulesLabel);
            this.groupBox1.Controls.Add(this.mfPin1Label);
            this.groupBox1.Controls.Add(this.mfPin1ComboBox);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // mfNumModulesComboBox
            // 
            this.mfNumModulesComboBox.BackColor = System.Drawing.SystemColors.Window;
            this.mfNumModulesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mfNumModulesComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.mfNumModulesComboBox, "mfNumModulesComboBox");
            this.mfNumModulesComboBox.Name = "mfNumModulesComboBox";
            this.mfNumModulesComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            this.mfNumModulesComboBox.SelectedValueChanged += new System.EventHandler(this.value_Changed);
            // 
            // numberOfModulesLabel
            // 
            resources.ApplyResources(this.numberOfModulesLabel, "numberOfModulesLabel");
            this.numberOfModulesLabel.Name = "numberOfModulesLabel";
            // 
            // mfPin1Label
            // 
            resources.ApplyResources(this.mfPin1Label, "mfPin1Label");
            this.mfPin1Label.Name = "mfPin1Label";
            // 
            // mfPin1ComboBox
            // 
            this.mfPin1ComboBox.BackColor = System.Drawing.SystemColors.Window;
            this.mfPin1ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mfPin1ComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.mfPin1ComboBox, "mfPin1ComboBox");
            this.mfPin1ComboBox.Name = "mfPin1ComboBox";
            this.mfPin1ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            this.mfPin1ComboBox.SelectedValueChanged += new System.EventHandler(this.value_Changed);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox1);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            this.textBox1.TextChanged += new System.EventHandler(this.value_Changed);
            // 
            // muxDrvPanel
            // 
            resources.ApplyResources(this.muxDrvPanel, "muxDrvPanel");
            this.muxDrvPanel.Name = "muxDrvPanel";
            // 
            // MFInputMultiplexerPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.muxDrvPanel);
            this.Name = "MFInputMultiplexerPanel";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox mfNumModulesComboBox;
        private System.Windows.Forms.Label numberOfModulesLabel;
        private System.Windows.Forms.Label mfPin1Label;
        private System.Windows.Forms.ComboBox mfPin1ComboBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel muxDrvPanel;
    }
}