namespace MobiFlight.UI.Panels.Modifier
{
    partial class TransformModifierPanel
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
            this.multiplyPanel = new System.Windows.Forms.Panel();
            this.expressionTextBox = new System.Windows.Forms.TextBox();
            this.labelExpression = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.multiplyPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // multiplyPanel
            // 
            this.multiplyPanel.AutoSize = true;
            this.multiplyPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.multiplyPanel.Controls.Add(this.labelExpression);
            this.multiplyPanel.Controls.Add(this.expressionTextBox);
            this.multiplyPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.multiplyPanel.Location = new System.Drawing.Point(0, 0);
            this.multiplyPanel.Name = "multiplyPanel";
            this.multiplyPanel.Size = new System.Drawing.Size(400, 27);
            this.multiplyPanel.TabIndex = 16;
            // 
            // expressionTextBox
            // 
            this.expressionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.expressionTextBox.Location = new System.Drawing.Point(81, 4);
            this.expressionTextBox.Name = "expressionTextBox";
            this.expressionTextBox.Size = new System.Drawing.Size(303, 20);
            this.expressionTextBox.TabIndex = 13;
            // 
            // labelExpression
            // 
            this.labelExpression.Location = new System.Drawing.Point(4, 3);
            this.labelExpression.Name = "labelExpression";
            this.labelExpression.Size = new System.Drawing.Size(71, 21);
            this.labelExpression.TabIndex = 14;
            this.labelExpression.Text = "Expression:";
            this.labelExpression.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel1
            // 
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(81, 33);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(303, 34);
            this.panel1.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(303, 34);
            this.label1.TabIndex = 0;
            this.label1.Text = "You can use ncalc expession syntax for more complex transformations.\r\n";
            // 
            // TransformModifier
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.multiplyPanel);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(400, 0);
            this.Name = "TransformModifier";
            this.Size = new System.Drawing.Size(400, 70);
            this.multiplyPanel.ResumeLayout(false);
            this.multiplyPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel multiplyPanel;
        private System.Windows.Forms.TextBox expressionTextBox;
        private System.Windows.Forms.Label labelExpression;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
    }
}
