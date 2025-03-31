namespace MobiFlight.UI.Panels.Action
{
    partial class ProSimInputPanel
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
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.proSimDatarefPanel1 = new MobiFlight.UI.Panels.Config.ProSimDatarefPanel();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Value:";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(68, 80);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(240, 20);
            this.textBox2.TabIndex = 3;
            // 
            // proSimDatarefPanel1
            // 
            this.proSimDatarefPanel1.Location = new System.Drawing.Point(3, 3);
            this.proSimDatarefPanel1.Name = "proSimDatarefPanel1";
            this.proSimDatarefPanel1.Size = new System.Drawing.Size(388, 71);
            this.proSimDatarefPanel1.TabIndex = 4;
            // 
            // ProSimInputPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.proSimDatarefPanel1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label2);
            this.Name = "ProSimInputPanel";
            this.Size = new System.Drawing.Size(394, 113);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox2;
        private Config.ProSimDatarefPanel proSimDatarefPanel1;
    }
}
