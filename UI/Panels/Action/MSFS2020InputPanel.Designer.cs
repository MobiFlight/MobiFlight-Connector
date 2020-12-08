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
            this.DeviceLabel = new System.Windows.Forms.Label();
            this.DeviceComboBox = new System.Windows.Forms.ComboBox();
            this.EventIdComboBox = new System.Windows.Forms.ComboBox();
            this.EventLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // DeviceLabel
            // 
            this.DeviceLabel.Location = new System.Drawing.Point(14, 19);
            this.DeviceLabel.Name = "DeviceLabel";
            this.DeviceLabel.Size = new System.Drawing.Size(51, 18);
            this.DeviceLabel.TabIndex = 0;
            this.DeviceLabel.Text = "Device";
            this.DeviceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DeviceComboBox
            // 
            this.DeviceComboBox.FormattingEnabled = true;
            this.DeviceComboBox.Location = new System.Drawing.Point(71, 19);
            this.DeviceComboBox.Name = "DeviceComboBox";
            this.DeviceComboBox.Size = new System.Drawing.Size(171, 21);
            this.DeviceComboBox.TabIndex = 1;
            this.DeviceComboBox.SelectedIndexChanged += new System.EventHandler(this.DeviceComboBox_SelectedIndexChanged);
            // 
            // eventIdComboBox
            // 
            this.EventIdComboBox.FormattingEnabled = true;
            this.EventIdComboBox.Location = new System.Drawing.Point(71, 48);
            this.EventIdComboBox.Name = "eventIdComboBox";
            this.EventIdComboBox.Size = new System.Drawing.Size(171, 21);
            this.EventIdComboBox.TabIndex = 3;
            // 
            // EventLabel
            // 
            this.EventLabel.Location = new System.Drawing.Point(14, 48);
            this.EventLabel.Name = "EventLabel";
            this.EventLabel.Size = new System.Drawing.Size(51, 18);
            this.EventLabel.TabIndex = 2;
            this.EventLabel.Text = "Event";
            this.EventLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MSFS2020InputPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.EventIdComboBox);
            this.Controls.Add(this.EventLabel);
            this.Controls.Add(this.DeviceComboBox);
            this.Controls.Add(this.DeviceLabel);
            this.Name = "MSFS2020InputPanel";
            this.Size = new System.Drawing.Size(265, 96);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label DeviceLabel;
        private System.Windows.Forms.ComboBox DeviceComboBox;
        private System.Windows.Forms.ComboBox EventIdComboBox;
        private System.Windows.Forms.Label EventLabel;
    }
}
