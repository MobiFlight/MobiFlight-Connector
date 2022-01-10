
namespace MobiFlight.UI.Panels.Config
{
    partial class VariablePanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VariablePanel));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.TypeComboBox = new System.Windows.Forms.ComboBox();
            this.NameTextBox = new System.Windows.Forms.ComboBox();
            this.TypeLabel = new System.Windows.Forms.Label();
            this.NameLabel = new System.Windows.Forms.Label();
            this.transformOptionsGroup1 = new MobiFlight.UI.Panels.Config.TransformOptionsGroup();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.TypeComboBox);
            this.groupBox1.Controls.Add(this.NameTextBox);
            this.groupBox1.Controls.Add(this.TypeLabel);
            this.groupBox1.Controls.Add(this.NameLabel);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // TypeComboBox
            // 
            this.TypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TypeComboBox.FormattingEnabled = true;
            this.TypeComboBox.Items.AddRange(new object[] {
            resources.GetString("TypeComboBox.Items"),
            resources.GetString("TypeComboBox.Items1")});
            resources.ApplyResources(this.TypeComboBox, "TypeComboBox");
            this.TypeComboBox.Name = "TypeComboBox";
            this.TypeComboBox.SelectedIndexChanged += new System.EventHandler(this.TypeComboBox_SelectedIndexChanged);
            // 
            // NameTextBox
            // 
            resources.ApplyResources(this.NameTextBox, "NameTextBox");
            this.NameTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.SelectedValueChanged += new System.EventHandler(this.NameTextBox_SelectedValueChanged);
            // 
            // TypeLabel
            // 
            resources.ApplyResources(this.TypeLabel, "TypeLabel");
            this.TypeLabel.Name = "TypeLabel";
            // 
            // NameLabel
            // 
            resources.ApplyResources(this.NameLabel, "NameLabel");
            this.NameLabel.Name = "NameLabel";
            // 
            // transformOptionsGroup1
            // 
            resources.ApplyResources(this.transformOptionsGroup1, "transformOptionsGroup1");
            this.transformOptionsGroup1.Name = "transformOptionsGroup1";
            // 
            // VariablePanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.transformOptionsGroup1);
            this.Controls.Add(this.groupBox1);
            this.Name = "VariablePanel";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private TransformOptionsGroup transformOptionsGroup1;
        private System.Windows.Forms.ComboBox TypeComboBox;
        private System.Windows.Forms.ComboBox NameTextBox;
        private System.Windows.Forms.Label TypeLabel;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.Label label1;
    }
}
