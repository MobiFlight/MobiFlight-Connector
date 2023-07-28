namespace MobiFlight.UI.Panels.Modifier
{
    partial class BlinkModifierPanel
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
            this.textBoxBlinkValue = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxOnOffSequence = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBoxBlinkValue
            // 
            this.textBoxBlinkValue.Location = new System.Drawing.Point(104, 12);
            this.textBoxBlinkValue.Name = "textBoxBlinkValue";
            this.textBoxBlinkValue.Size = new System.Drawing.Size(143, 20);
            this.textBoxBlinkValue.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Blink value";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Blink sequence";
            // 
            // textBoxOnOffSequence
            // 
            this.textBoxOnOffSequence.Location = new System.Drawing.Point(104, 38);
            this.textBoxOnOffSequence.Name = "textBoxOnOffSequence";
            this.textBoxOnOffSequence.Size = new System.Drawing.Size(143, 20);
            this.textBoxOnOffSequence.TabIndex = 2;
            // 
            // BlinkModifier
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxOnOffSequence);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxBlinkValue);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(400, 0);
            this.Name = "BlinkModifier";
            this.Size = new System.Drawing.Size(400, 61);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxBlinkValue;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxOnOffSequence;
    }
}
