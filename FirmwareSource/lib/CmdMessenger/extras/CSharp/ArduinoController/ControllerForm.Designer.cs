namespace ArduinoController
{
    partial class ControllerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;



        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.EnableLedCheckBox = new System.Windows.Forms.CheckBox();
            this.LedFrequencyLabelTrackBar = new System.Windows.Forms.TrackBar();
            this.LedFrequencyLabel = new System.Windows.Forms.Label();
            this.LedFrequencyValue = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.LedFrequencyLabelTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // EnableLedCheckBox
            // 
            this.EnableLedCheckBox.AutoSize = true;
            this.EnableLedCheckBox.Checked = true;
            this.EnableLedCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.EnableLedCheckBox.Location = new System.Drawing.Point(30, 12);
            this.EnableLedCheckBox.Name = "EnableLedCheckBox";
            this.EnableLedCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.EnableLedCheckBox.Size = new System.Drawing.Size(80, 17);
            this.EnableLedCheckBox.TabIndex = 0;
            this.EnableLedCheckBox.Text = "Enable Led";
            this.EnableLedCheckBox.UseVisualStyleBackColor = true;
            this.EnableLedCheckBox.CheckedChanged += new System.EventHandler(this.EnableLedCheckBoxCheckedChanged);
            // 
            // LedFrequencyLabelTrackBar
            // 
            this.LedFrequencyLabelTrackBar.Location = new System.Drawing.Point(90, 35);
            this.LedFrequencyLabelTrackBar.Maximum = 240;
            this.LedFrequencyLabelTrackBar.Name = "LedFrequencyLabelTrackBar";
            this.LedFrequencyLabelTrackBar.Size = new System.Drawing.Size(208, 45);
            this.LedFrequencyLabelTrackBar.TabIndex = 1;
            this.LedFrequencyLabelTrackBar.Tag = "";
            this.LedFrequencyLabelTrackBar.TickFrequency = 10;
            this.LedFrequencyLabelTrackBar.Scroll += new System.EventHandler(this.LedFrequencyTrackBarScroll);
            this.LedFrequencyLabelTrackBar.ValueChanged += new System.EventHandler(this.LedFrequencyLabelTrackBarValueChanged);
            // 
            // LedFrequencyLabel
            // 
            this.LedFrequencyLabel.AutoSize = true;
            this.LedFrequencyLabel.Location = new System.Drawing.Point(14, 36);
            this.LedFrequencyLabel.Name = "LedFrequencyLabel";
            this.LedFrequencyLabel.Size = new System.Drawing.Size(78, 13);
            this.LedFrequencyLabel.TabIndex = 2;
            this.LedFrequencyLabel.Text = "Led Frequency";
            // 
            // LedFrequencyValue
            // 
            this.LedFrequencyValue.AutoSize = true;
            this.LedFrequencyValue.Location = new System.Drawing.Point(304, 38);
            this.LedFrequencyValue.Name = "LedFrequencyValue";
            this.LedFrequencyValue.Size = new System.Drawing.Size(13, 13);
            this.LedFrequencyValue.TabIndex = 3;
            this.LedFrequencyValue.Text = "0";
            // 
            // ControllerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 85);
            this.Controls.Add(this.LedFrequencyValue);
            this.Controls.Add(this.LedFrequencyLabel);
            this.Controls.Add(this.LedFrequencyLabelTrackBar);
            this.Controls.Add(this.EnableLedCheckBox);
            this.Name = "ControllerForm";
            this.Text = "Arduino Controller";
            ((System.ComponentModel.ISupportInitialize)(this.LedFrequencyLabelTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox EnableLedCheckBox;
        private System.Windows.Forms.TrackBar LedFrequencyLabelTrackBar;
        private System.Windows.Forms.Label LedFrequencyLabel;
        private System.Windows.Forms.Label LedFrequencyValue;
    }
}

