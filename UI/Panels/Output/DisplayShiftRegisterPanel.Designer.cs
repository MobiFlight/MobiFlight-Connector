
namespace MobiFlight.UI.Panels
{
    partial class DisplayShiftRegisterPanel
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
            this.registerLabel = new System.Windows.Forms.Label();
            this.shiftRegistersComboBox = new System.Windows.Forms.ComboBox();
            this.pinSelectPanel = new MobiFlight.UI.Panels.PinSelectPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // registerLabel
            // 
            this.registerLabel.AutoSize = true;
            this.registerLabel.Location = new System.Drawing.Point(20, 11);
            this.registerLabel.Name = "registerLabel";
            this.registerLabel.Size = new System.Drawing.Size(93, 17);
            this.registerLabel.TabIndex = 0;
            this.registerLabel.Text = "Shift Register";
            // 
            // shiftRegistersComboBox
            // 
            this.shiftRegistersComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.shiftRegistersComboBox.FormattingEnabled = true;
            this.shiftRegistersComboBox.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "A",
            "B",
            "C",
            "D",
            "E",
            "F"});
            this.shiftRegistersComboBox.Location = new System.Drawing.Point(120, 8);
            this.shiftRegistersComboBox.Margin = new System.Windows.Forms.Padding(4);
            this.shiftRegistersComboBox.MaximumSize = new System.Drawing.Size(177, 0);
            this.shiftRegistersComboBox.MinimumSize = new System.Drawing.Size(45, 0);
            this.shiftRegistersComboBox.Name = "shiftRegistersComboBox";
            this.shiftRegistersComboBox.Size = new System.Drawing.Size(177, 24);
            this.shiftRegistersComboBox.TabIndex = 66;
            // 
            // pinSelectPanel
            // 
            this.pinSelectPanel.AutoScroll = true;
            this.pinSelectPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pinSelectPanel.BackColor = System.Drawing.SystemColors.Control;
            this.pinSelectPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pinSelectPanel.Location = new System.Drawing.Point(0, 51);
            this.pinSelectPanel.Margin = new System.Windows.Forms.Padding(4);
            this.pinSelectPanel.MinimumSize = new System.Drawing.Size(300, 200);
            this.pinSelectPanel.Name = "pinSelectPanel";
            this.pinSelectPanel.Size = new System.Drawing.Size(453, 237);
            this.pinSelectPanel.TabIndex = 67;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.shiftRegistersComboBox);
            this.panel1.Controls.Add(this.registerLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(453, 44);
            this.panel1.TabIndex = 68;
            // 
            // DisplayShiftRegisterPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pinSelectPanel);
            this.Name = "DisplayShiftRegisterPanel";
            this.Size = new System.Drawing.Size(453, 288);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label registerLabel;
        public System.Windows.Forms.ComboBox shiftRegistersComboBox;
        private PinSelectPanel pinSelectPanel;
        private System.Windows.Forms.Panel panel1;
    }
}
