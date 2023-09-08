namespace MobiFlight.UI.Panels.Modifier
{
    partial class PaddingModifierPanel
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
            this.labelCharacter = new System.Windows.Forms.Label();
            this.textBoxLength = new System.Windows.Forms.TextBox();
            this.labelLength = new System.Windows.Forms.Label();
            this.labelDirection = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBoxCharacter = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // labelCharacter
            // 
            this.labelCharacter.AutoSize = true;
            this.labelCharacter.Location = new System.Drawing.Point(3, 6);
            this.labelCharacter.Name = "labelCharacter";
            this.labelCharacter.Size = new System.Drawing.Size(53, 13);
            this.labelCharacter.TabIndex = 0;
            this.labelCharacter.Text = "Character";
            // 
            // textBoxLength
            // 
            this.textBoxLength.Location = new System.Drawing.Point(180, 3);
            this.textBoxLength.Name = "textBoxLength";
            this.textBoxLength.Size = new System.Drawing.Size(45, 20);
            this.textBoxLength.TabIndex = 3;
            // 
            // labelLength
            // 
            this.labelLength.AutoSize = true;
            this.labelLength.Location = new System.Drawing.Point(140, 6);
            this.labelLength.Name = "labelLength";
            this.labelLength.Size = new System.Drawing.Size(40, 13);
            this.labelLength.TabIndex = 2;
            this.labelLength.Text = "Length";
            // 
            // labelDirection
            // 
            this.labelDirection.AutoSize = true;
            this.labelDirection.Location = new System.Drawing.Point(237, 6);
            this.labelDirection.Name = "labelDirection";
            this.labelDirection.Size = new System.Drawing.Size(49, 13);
            this.labelDirection.TabIndex = 4;
            this.labelDirection.Text = "Direction";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(288, 2);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(92, 21);
            this.comboBox1.TabIndex = 5;
            // 
            // comboBoxBlinkValue
            // 
            this.comboBoxCharacter.FormattingEnabled = true;
            this.comboBoxCharacter.Location = new System.Drawing.Point(58, 3);
            this.comboBoxCharacter.Name = "comboBoxBlinkValue";
            this.comboBoxCharacter.Size = new System.Drawing.Size(72, 21);
            this.comboBoxCharacter.TabIndex = 10;
            // 
            // PaddingModifierPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.comboBoxCharacter);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.labelDirection);
            this.Controls.Add(this.textBoxLength);
            this.Controls.Add(this.labelLength);
            this.Controls.Add(this.labelCharacter);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(400, 0);
            this.Name = "PaddingModifierPanel";
            this.Size = new System.Drawing.Size(400, 27);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelCharacter;
        private System.Windows.Forms.TextBox textBoxLength;
        private System.Windows.Forms.Label labelLength;
        private System.Windows.Forms.Label labelDirection;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBoxCharacter;
    }
}
