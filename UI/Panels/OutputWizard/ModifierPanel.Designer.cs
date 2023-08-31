namespace MobiFlight.UI.Panels.OutputWizard
{
    partial class ModifierPanel
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
            this.modifierListPanel = new System.Windows.Forms.Panel();
            this.modifierControl1 = new MobiFlight.UI.Panels.Modifier.ModifierControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.transformToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.interpolationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blinkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.paddingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBoxModifier = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.comboBoxTestValueType = new System.Windows.Forms.ComboBox();
            this.labelTestResultValue = new System.Windows.Forms.Label();
            this.labelTestResult = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxTestValue = new System.Windows.Forms.TextBox();
            this.buttonTest = new System.Windows.Forms.Button();
            this.modifierListPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBoxModifier.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // modifierListPanel
            // 
            this.modifierListPanel.AutoScroll = true;
            this.modifierListPanel.AutoSize = true;
            this.modifierListPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.modifierListPanel.Controls.Add(this.modifierControl1);
            this.modifierListPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modifierListPanel.Location = new System.Drawing.Point(0, 0);
            this.modifierListPanel.Name = "modifierListPanel";
            this.modifierListPanel.Padding = new System.Windows.Forms.Padding(3);
            this.modifierListPanel.Size = new System.Drawing.Size(494, 368);
            this.modifierListPanel.TabIndex = 7;
            // 
            // modifierControl1
            // 
            this.modifierControl1.AutoSize = true;
            this.modifierControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.modifierControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.modifierControl1.First = false;
            this.modifierControl1.Last = false;
            this.modifierControl1.Location = new System.Drawing.Point(3, 3);
            this.modifierControl1.MinimumSize = new System.Drawing.Size(400, 2);
            this.modifierControl1.Modifier = null;
            this.modifierControl1.Name = "modifierControl1";
            this.modifierControl1.Selected = false;
            this.modifierControl1.Size = new System.Drawing.Size(488, 27);
            this.modifierControl1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.modifierListPanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 56);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(494, 368);
            this.panel1.TabIndex = 7;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(3, 16);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(3);
            this.panel2.Size = new System.Drawing.Size(494, 40);
            this.panel2.TabIndex = 12;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(391, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Add Modifier";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(3, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(382, 32);
            this.label2.TabIndex = 11;
            this.label2.Text = "Modifiers transform your values to new values. Each output is passed on to the ne" +
    "xt modifier as input.";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.transformToolStripMenuItem,
            this.compareToolStripMenuItem,
            this.interpolationToolStripMenuItem,
            this.blinkToolStripMenuItem,
            this.paddingToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(143, 114);
            // 
            // transformToolStripMenuItem
            // 
            this.transformToolStripMenuItem.Name = "transformToolStripMenuItem";
            this.transformToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.transformToolStripMenuItem.Text = "Transform";
            this.transformToolStripMenuItem.Click += new System.EventHandler(this.modifierToolStripMenuItem_Click);
            // 
            // compareToolStripMenuItem
            // 
            this.compareToolStripMenuItem.Name = "compareToolStripMenuItem";
            this.compareToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.compareToolStripMenuItem.Text = "Compare";
            this.compareToolStripMenuItem.Click += new System.EventHandler(this.modifierToolStripMenuItem_Click);
            // 
            // interpolationToolStripMenuItem
            // 
            this.interpolationToolStripMenuItem.Name = "interpolationToolStripMenuItem";
            this.interpolationToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.interpolationToolStripMenuItem.Text = "Interpolation";
            this.interpolationToolStripMenuItem.Click += new System.EventHandler(this.modifierToolStripMenuItem_Click);
            // 
            // blinkToolStripMenuItem
            // 
            this.blinkToolStripMenuItem.Name = "blinkToolStripMenuItem";
            this.blinkToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.blinkToolStripMenuItem.Text = "Blink";
            this.blinkToolStripMenuItem.Click += new System.EventHandler(this.modifierToolStripMenuItem_Click);
            // 
            // paddingToolStripMenuItem
            // 
            this.paddingToolStripMenuItem.Name = "paddingToolStripMenuItem";
            this.paddingToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.paddingToolStripMenuItem.Text = "Padding";
            this.paddingToolStripMenuItem.Click += new System.EventHandler(this.modifierToolStripMenuItem_Click);
            // 
            // groupBoxModifier
            // 
            this.groupBoxModifier.Controls.Add(this.panel3);
            this.groupBoxModifier.Controls.Add(this.panel1);
            this.groupBoxModifier.Controls.Add(this.panel2);
            this.groupBoxModifier.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxModifier.Location = new System.Drawing.Point(0, 0);
            this.groupBoxModifier.Name = "groupBoxModifier";
            this.groupBoxModifier.Size = new System.Drawing.Size(500, 427);
            this.groupBoxModifier.TabIndex = 6;
            this.groupBoxModifier.TabStop = false;
            this.groupBoxModifier.Text = "Modifier";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.comboBoxTestValueType);
            this.panel3.Controls.Add(this.labelTestResultValue);
            this.panel3.Controls.Add(this.labelTestResult);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.textBoxTestValue);
            this.panel3.Controls.Add(this.buttonTest);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(3, 392);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(494, 32);
            this.panel3.TabIndex = 12;
            // 
            // comboBoxTestValueType
            // 
            this.comboBoxTestValueType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTestValueType.FormattingEnabled = true;
            this.comboBoxTestValueType.Items.AddRange(new object[] {
            "Number",
            "String"});
            this.comboBoxTestValueType.Location = new System.Drawing.Point(84, 7);
            this.comboBoxTestValueType.Name = "comboBoxTestValueType";
            this.comboBoxTestValueType.Size = new System.Drawing.Size(71, 21);
            this.comboBoxTestValueType.TabIndex = 12;
            // 
            // labelTestResultValue
            // 
            this.labelTestResultValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTestResultValue.Location = new System.Drawing.Point(411, 5);
            this.labelTestResultValue.Name = "labelTestResultValue";
            this.labelTestResultValue.Size = new System.Drawing.Size(80, 23);
            this.labelTestResultValue.TabIndex = 11;
            this.labelTestResultValue.Text = "#######";
            this.labelTestResultValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelTestResult
            // 
            this.labelTestResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTestResult.Location = new System.Drawing.Point(356, 5);
            this.labelTestResult.Name = "labelTestResult";
            this.labelTestResult.Size = new System.Drawing.Size(49, 23);
            this.labelTestResult.TabIndex = 10;
            this.labelTestResult.Text = "Result";
            this.labelTestResult.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 23);
            this.label1.TabIndex = 9;
            this.label1.Text = "Test value";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxTestValue
            // 
            this.textBoxTestValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxTestValue.Location = new System.Drawing.Point(161, 7);
            this.textBoxTestValue.Name = "textBoxTestValue";
            this.textBoxTestValue.Size = new System.Drawing.Size(105, 20);
            this.textBoxTestValue.TabIndex = 8;
            // 
            // buttonTest
            // 
            this.buttonTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTest.Location = new System.Drawing.Point(272, 5);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(78, 23);
            this.buttonTest.TabIndex = 7;
            this.buttonTest.Text = "Test";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // ModifierPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxModifier);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(500, 0);
            this.Name = "ModifierPanel";
            this.Size = new System.Drawing.Size(500, 427);
            this.modifierListPanel.ResumeLayout(false);
            this.modifierListPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBoxModifier.ResumeLayout(false);
            this.groupBoxModifier.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel modifierListPanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem transformToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem compareToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem interpolationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem blinkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem paddingToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBoxModifier;
        private Modifier.ModifierControl modifierControl1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ComboBox comboBoxTestValueType;
        private System.Windows.Forms.Label labelTestResultValue;
        private System.Windows.Forms.Label labelTestResult;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxTestValue;
        private System.Windows.Forms.Button buttonTest;
    }
}
