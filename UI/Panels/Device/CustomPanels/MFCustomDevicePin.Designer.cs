namespace MobiFlight.UI.Panels.Device.CustomPanels
{
    partial class MFCustomDevicePin
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.mfPinLabel = new System.Windows.Forms.Label();
            this.mfPinComboBox = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.mfPinLabel);
            this.panel1.Controls.Add(this.mfPinComboBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(205, 27);
            this.panel1.TabIndex = 7;
            // 
            // mfPinLabel
            // 
            this.mfPinLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.mfPinLabel.Location = new System.Drawing.Point(69, 5);
            this.mfPinLabel.Name = "mfPinLabel";
            this.mfPinLabel.Size = new System.Drawing.Size(82, 18);
            this.mfPinLabel.TabIndex = 14;
            this.mfPinLabel.Text = "Pin";
            // 
            // mfPinComboBox
            // 
            this.mfPinComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mfPinComboBox.FormattingEnabled = true;
            this.mfPinComboBox.Location = new System.Drawing.Point(18, 3);
            this.mfPinComboBox.MaxLength = 2;
            this.mfPinComboBox.Name = "mfPinComboBox";
            this.mfPinComboBox.Size = new System.Drawing.Size(45, 21);
            this.mfPinComboBox.TabIndex = 13;
            // 
            // MFCustomDevicePin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "MFCustomDevicePin";
            this.Size = new System.Drawing.Size(205, 70);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label mfPinLabel;
        private System.Windows.Forms.ComboBox mfPinComboBox;
    }
}
