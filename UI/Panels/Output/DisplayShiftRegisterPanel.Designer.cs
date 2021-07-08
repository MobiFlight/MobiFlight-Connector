
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
            this.registerLabel.Location = new System.Drawing.Point(0, 10);
            this.registerLabel.Name = "registerLabel";
            this.registerLabel.Size = new System.Drawing.Size(138, 28);
            this.registerLabel.TabIndex = 0;
            this.registerLabel.Text = "Shift Register";
            this.registerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            this.shiftRegistersComboBox.Location = new System.Drawing.Point(143, 11);
            this.shiftRegistersComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.shiftRegistersComboBox.MaximumSize = new System.Drawing.Size(199, 0);
            this.shiftRegistersComboBox.MinimumSize = new System.Drawing.Size(50, 0);
            this.shiftRegistersComboBox.Name = "shiftRegistersComboBox";
            this.shiftRegistersComboBox.Size = new System.Drawing.Size(199, 28);
            this.shiftRegistersComboBox.TabIndex = 66;
            // 
            // pinSelectPanel
            // 
            this.pinSelectPanel.AutoScroll = true;
            this.pinSelectPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pinSelectPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pinSelectPanel.Location = new System.Drawing.Point(0, 55);
            this.pinSelectPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pinSelectPanel.MinimumSize = new System.Drawing.Size(338, 250);
            this.pinSelectPanel.Name = "pinSelectPanel";
            this.pinSelectPanel.Size = new System.Drawing.Size(510, 305);
            this.pinSelectPanel.TabIndex = 67;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.shiftRegistersComboBox);
            this.panel1.Controls.Add(this.registerLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(510, 55);
            this.panel1.TabIndex = 68;
            // 
            // DisplayShiftRegisterPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.pinSelectPanel);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "DisplayShiftRegisterPanel";
            this.Size = new System.Drawing.Size(510, 360);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label registerLabel;
        public System.Windows.Forms.ComboBox shiftRegistersComboBox;
        private PinSelectPanel pinSelectPanel;
        private System.Windows.Forms.Panel panel1;
    }
}
