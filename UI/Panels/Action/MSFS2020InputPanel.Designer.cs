namespace MobiFlight.UI.Panels.Action
{
    partial class MSFS2020InputPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MSFS2020InputPanel));
            this.DeviceLabel = new System.Windows.Forms.Label();
            this.GroupComboBox = new System.Windows.Forms.ComboBox();
            this.EventIdComboBox = new System.Windows.Forms.ComboBox();
            this.EventLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // DeviceLabel
            // 
            resources.ApplyResources(this.DeviceLabel, "DeviceLabel");
            this.DeviceLabel.Name = "DeviceLabel";
            // 
            // GroupComboBox
            // 
            resources.ApplyResources(this.GroupComboBox, "GroupComboBox");
            this.GroupComboBox.FormattingEnabled = true;
            this.GroupComboBox.Name = "GroupComboBox";
            this.GroupComboBox.SelectedIndexChanged += new System.EventHandler(this.DeviceComboBox_SelectedIndexChanged);
            // 
            // EventIdComboBox
            // 
            resources.ApplyResources(this.EventIdComboBox, "EventIdComboBox");
            this.EventIdComboBox.DropDownWidth = 300;
            this.EventIdComboBox.FormattingEnabled = true;
            this.EventIdComboBox.Name = "EventIdComboBox";
            // 
            // EventLabel
            // 
            resources.ApplyResources(this.EventLabel, "EventLabel");
            this.EventLabel.Name = "EventLabel";
            // 
            // MSFS2020InputPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.EventIdComboBox);
            this.Controls.Add(this.EventLabel);
            this.Controls.Add(this.GroupComboBox);
            this.Controls.Add(this.DeviceLabel);
            this.Name = "MSFS2020InputPanel";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label DeviceLabel;
        private System.Windows.Forms.ComboBox GroupComboBox;
        private System.Windows.Forms.ComboBox EventIdComboBox;
        private System.Windows.Forms.Label EventLabel;
    }
}
