
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.displayPinPanel = new MobiFlight.UI.Panels.DisplayPinPanel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // registerLabel
            // 
            this.registerLabel.Location = new System.Drawing.Point(7, 4);
            this.registerLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.registerLabel.Name = "registerLabel";
            this.registerLabel.Size = new System.Drawing.Size(96, 18);
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
            this.shiftRegistersComboBox.Location = new System.Drawing.Point(108, 4);
            this.shiftRegistersComboBox.MaximumSize = new System.Drawing.Size(134, 0);
            this.shiftRegistersComboBox.MinimumSize = new System.Drawing.Size(35, 0);
            this.shiftRegistersComboBox.Name = "shiftRegistersComboBox";
            this.shiftRegistersComboBox.Size = new System.Drawing.Size(134, 21);
            this.shiftRegistersComboBox.TabIndex = 66;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.shiftRegistersComboBox);
            this.panel1.Controls.Add(this.registerLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(566, 29);
            this.panel1.TabIndex = 68;
            // 
            // displayPinPanel
            // 
            this.displayPinPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displayPinPanel.Location = new System.Drawing.Point(0, 29);
            this.displayPinPanel.Name = "displayPinPanel";
            this.displayPinPanel.Size = new System.Drawing.Size(566, 245);
            this.displayPinPanel.TabIndex = 67;
            // 
            // DisplayShiftRegisterPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.displayPinPanel);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "DisplayShiftRegisterPanel";
            this.Size = new System.Drawing.Size(566, 274);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label registerLabel;
        public System.Windows.Forms.ComboBox shiftRegistersComboBox;
        private System.Windows.Forms.Panel panel1;
        private DisplayPinPanel displayPinPanel;
    }
}
