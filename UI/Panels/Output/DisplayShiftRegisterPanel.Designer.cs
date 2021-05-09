
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabSimple = new System.Windows.Forms.TabPage();
            this.label14 = new System.Windows.Forms.Label();
            this.displayPinComboBox = new System.Windows.Forms.ComboBox();
            this.tabAdvanced = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.outputPanelLabel = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabSimple.SuspendLayout();
            this.tabAdvanced.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // registerLabel
            // 
            this.registerLabel.AutoSize = true;
            this.registerLabel.Location = new System.Drawing.Point(19, 10);
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
            this.shiftRegistersComboBox.Location = new System.Drawing.Point(119, 7);
            this.shiftRegistersComboBox.Margin = new System.Windows.Forms.Padding(4);
            this.shiftRegistersComboBox.MaximumSize = new System.Drawing.Size(177, 0);
            this.shiftRegistersComboBox.MinimumSize = new System.Drawing.Size(45, 0);
            this.shiftRegistersComboBox.Name = "shiftRegistersComboBox";
            this.shiftRegistersComboBox.Size = new System.Drawing.Size(177, 24);
            this.shiftRegistersComboBox.TabIndex = 66;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabSimple);
            this.tabControl1.Controls.Add(this.tabAdvanced);
            this.tabControl1.Location = new System.Drawing.Point(11, 38);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(340, 217);
            this.tabControl1.TabIndex = 75;
            // 
            // tabSimple
            // 
            this.tabSimple.Controls.Add(this.label14);
            this.tabSimple.Controls.Add(this.displayPinComboBox);
            this.tabSimple.Location = new System.Drawing.Point(4, 25);
            this.tabSimple.Name = "tabSimple";
            this.tabSimple.Padding = new System.Windows.Forms.Padding(3);
            this.tabSimple.Size = new System.Drawing.Size(332, 188);
            this.tabSimple.TabIndex = 0;
            this.tabSimple.Text = "Simple";
            this.tabSimple.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(22, 18);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(75, 17);
            this.label14.TabIndex = 8;
            this.label14.Text = "Output Pin";
            // 
            // displayPinComboBox
            // 
            this.displayPinComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.displayPinComboBox.FormattingEnabled = true;
            this.displayPinComboBox.Items.AddRange(new object[] {
            "Pin",
            "7-Segment",
            "3BCD-8Bit-with-Strobe"});
            this.displayPinComboBox.Location = new System.Drawing.Point(105, 15);
            this.displayPinComboBox.Margin = new System.Windows.Forms.Padding(4);
            this.displayPinComboBox.MaximumSize = new System.Drawing.Size(161, 0);
            this.displayPinComboBox.MinimumSize = new System.Drawing.Size(61, 0);
            this.displayPinComboBox.Name = "displayPinComboBox";
            this.displayPinComboBox.Size = new System.Drawing.Size(141, 24);
            this.displayPinComboBox.TabIndex = 7;
            // 
            // tabAdvanced
            // 
            this.tabAdvanced.Controls.Add(this.tableLayoutPanel);
            this.tabAdvanced.Controls.Add(this.outputPanelLabel);
            this.tabAdvanced.Location = new System.Drawing.Point(4, 25);
            this.tabAdvanced.Name = "tabAdvanced";
            this.tabAdvanced.Padding = new System.Windows.Forms.Padding(3);
            this.tabAdvanced.Size = new System.Drawing.Size(332, 188);
            this.tabAdvanced.TabIndex = 1;
            this.tabAdvanced.Text = "Advanced";
            this.tabAdvanced.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 9;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel.Controls.Add(this.label9, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.label10, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel.Controls.Add(this.label3, 3, 0);
            this.tableLayoutPanel.Controls.Add(this.label4, 4, 0);
            this.tableLayoutPanel.Controls.Add(this.label5, 5, 0);
            this.tableLayoutPanel.Controls.Add(this.label6, 6, 0);
            this.tableLayoutPanel.Controls.Add(this.label7, 7, 0);
            this.tableLayoutPanel.Controls.Add(this.label8, 8, 0);
            this.tableLayoutPanel.Controls.Add(this.label13, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.label11, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.label12, 0, 4);
            this.tableLayoutPanel.Location = new System.Drawing.Point(3, 33);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 6;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(289, 140);
            this.tableLayoutPanel.TabIndex = 72;
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(24, 33);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(16, 17);
            this.label9.TabIndex = 3;
            this.label9.Text = "1";
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(24, 61);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(16, 17);
            this.label10.TabIndex = 4;
            this.label10.Text = "2";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(71, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "1";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(99, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "2";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(127, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "3";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(155, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(16, 17);
            this.label4.TabIndex = 2;
            this.label4.Text = "4";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(183, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(16, 17);
            this.label5.TabIndex = 2;
            this.label5.Text = "5";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(211, 5);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(16, 17);
            this.label6.TabIndex = 2;
            this.label6.Text = "6";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(239, 5);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(16, 17);
            this.label7.TabIndex = 2;
            this.label7.Text = "7";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(267, 5);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(16, 17);
            this.label8.TabIndex = 7;
            this.label8.Text = "8";
            // 
            // label13
            // 
            this.label13.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(3, 5);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(59, 17);
            this.label13.TabIndex = 8;
            this.label13.Text = "Row\\Col";
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(24, 89);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(16, 17);
            this.label11.TabIndex = 5;
            this.label11.Text = "3";
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(24, 117);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(16, 17);
            this.label12.TabIndex = 6;
            this.label12.Text = "4";
            // 
            // outputPanelLabel
            // 
            this.outputPanelLabel.AutoSize = true;
            this.outputPanelLabel.Location = new System.Drawing.Point(6, 13);
            this.outputPanelLabel.Name = "outputPanelLabel";
            this.outputPanelLabel.Size = new System.Drawing.Size(139, 17);
            this.outputPanelLabel.TabIndex = 71;
            this.outputPanelLabel.Text = "Register Output Pins";
            // 
            // DisplayShiftRegisterPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.shiftRegistersComboBox);
            this.Controls.Add(this.registerLabel);
            this.Name = "DisplayShiftRegisterPanel";
            this.Size = new System.Drawing.Size(388, 265);
            this.tabControl1.ResumeLayout(false);
            this.tabSimple.ResumeLayout(false);
            this.tabSimple.PerformLayout();
            this.tabAdvanced.ResumeLayout(false);
            this.tabAdvanced.PerformLayout();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label registerLabel;
        public System.Windows.Forms.ComboBox shiftRegistersComboBox;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabSimple;
        private System.Windows.Forms.TabPage tabAdvanced;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label outputPanelLabel;
        private System.Windows.Forms.Label label14;
        public System.Windows.Forms.ComboBox displayPinComboBox;
    }
}
