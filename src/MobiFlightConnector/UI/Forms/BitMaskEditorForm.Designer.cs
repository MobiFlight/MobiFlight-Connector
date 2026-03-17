using MobiFlight.UI.Panels.Config;

namespace MobiFlight.UI.Forms
{
    partial class BitMaskEditorForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.panel = new System.Windows.Forms.Panel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.bytePanel8 = new MobiFlight.UI.Panels.Config.BytePanel();
            this.bytePanel7 = new MobiFlight.UI.Panels.Config.BytePanel();
            this.bytePanel6 = new MobiFlight.UI.Panels.Config.BytePanel();
            this.bytePanel5 = new MobiFlight.UI.Panels.Config.BytePanel();
            this.bytePanel4 = new MobiFlight.UI.Panels.Config.BytePanel();
            this.bytePanel3 = new MobiFlight.UI.Panels.Config.BytePanel();
            this.bytePanel2 = new MobiFlight.UI.Panels.Config.BytePanel();
            this.bytePanel1 = new MobiFlight.UI.Panels.Config.BytePanel();
            this.flowLayoutPanel.SuspendLayout();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.AutoSize = true;
            this.flowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel.Controls.Add(this.bytePanel8);
            this.flowLayoutPanel.Controls.Add(this.bytePanel7);
            this.flowLayoutPanel.Controls.Add(this.bytePanel6);
            this.flowLayoutPanel.Controls.Add(this.bytePanel5);
            this.flowLayoutPanel.Controls.Add(this.bytePanel4);
            this.flowLayoutPanel.Controls.Add(this.bytePanel3);
            this.flowLayoutPanel.Controls.Add(this.bytePanel2);
            this.flowLayoutPanel.Controls.Add(this.bytePanel1);
            this.flowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel.MaximumSize = new System.Drawing.Size(866, 0);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(864, 192);
            this.flowLayoutPanel.TabIndex = 4;
            // 
            // panel
            // 
            this.panel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel.Controls.Add(this.cancelButton);
            this.panel.Controls.Add(this.button1);
            this.panel.Location = new System.Drawing.Point(350, 201);
            this.panel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(164, 31);
            this.panel.TabIndex = 5;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(84, 4);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // bytePanel8
            // 
            this.bytePanel8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bytePanel8.Byte = ((byte)(8));
            this.bytePanel8.Location = new System.Drawing.Point(3, 3);
            this.bytePanel8.Name = "bytePanel8";
            this.bytePanel8.Size = new System.Drawing.Size(210, 90);
            this.bytePanel8.TabIndex = 7;
            this.bytePanel8.Value = ((byte)(0));
            // 
            // bytePanel7
            // 
            this.bytePanel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bytePanel7.Byte = ((byte)(7));
            this.bytePanel7.Location = new System.Drawing.Point(219, 3);
            this.bytePanel7.Name = "bytePanel7";
            this.bytePanel7.Size = new System.Drawing.Size(210, 90);
            this.bytePanel7.TabIndex = 6;
            this.bytePanel7.Value = ((byte)(0));
            // 
            // bytePanel6
            // 
            this.bytePanel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bytePanel6.Byte = ((byte)(6));
            this.bytePanel6.Location = new System.Drawing.Point(435, 3);
            this.bytePanel6.Name = "bytePanel6";
            this.bytePanel6.Size = new System.Drawing.Size(210, 90);
            this.bytePanel6.TabIndex = 5;
            this.bytePanel6.Value = ((byte)(0));
            // 
            // bytePanel5
            // 
            this.bytePanel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bytePanel5.Byte = ((byte)(5));
            this.bytePanel5.Location = new System.Drawing.Point(651, 3);
            this.bytePanel5.Name = "bytePanel5";
            this.bytePanel5.Size = new System.Drawing.Size(210, 90);
            this.bytePanel5.TabIndex = 4;
            this.bytePanel5.Value = ((byte)(0));
            // 
            // bytePanel4
            // 
            this.bytePanel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bytePanel4.Byte = ((byte)(4));
            this.bytePanel4.Location = new System.Drawing.Point(3, 99);
            this.bytePanel4.Name = "bytePanel4";
            this.bytePanel4.Size = new System.Drawing.Size(210, 90);
            this.bytePanel4.TabIndex = 2;
            this.bytePanel4.Value = ((byte)(0));
            // 
            // bytePanel3
            // 
            this.bytePanel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bytePanel3.Byte = ((byte)(3));
            this.bytePanel3.Location = new System.Drawing.Point(219, 99);
            this.bytePanel3.Name = "bytePanel3";
            this.bytePanel3.Size = new System.Drawing.Size(210, 90);
            this.bytePanel3.TabIndex = 0;
            this.bytePanel3.Value = ((byte)(0));
            // 
            // bytePanel2
            // 
            this.bytePanel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bytePanel2.Byte = ((byte)(2));
            this.bytePanel2.Location = new System.Drawing.Point(435, 99);
            this.bytePanel2.Name = "bytePanel2";
            this.bytePanel2.Size = new System.Drawing.Size(210, 90);
            this.bytePanel2.TabIndex = 1;
            this.bytePanel2.Value = ((byte)(0));
            // 
            // bytePanel1
            // 
            this.bytePanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bytePanel1.Byte = ((byte)(1));
            this.bytePanel1.Location = new System.Drawing.Point(651, 99);
            this.bytePanel1.Name = "bytePanel1";
            this.bytePanel1.Size = new System.Drawing.Size(210, 90);
            this.bytePanel1.TabIndex = 3;
            this.bytePanel1.Value = ((byte)(0));
            // 
            // BitMaskEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(869, 241);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.flowLayoutPanel);
            this.Name = "BitMaskEditorForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Bit-Mask Editor";
            this.Load += new System.EventHandler(this.BitMaskEditorForm_Load);
            this.flowLayoutPanel.ResumeLayout(false);
            this.panel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BytePanel bytePanel3;
        private BytePanel bytePanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button button1;
        private BytePanel bytePanel8;
        private BytePanel bytePanel7;
        private BytePanel bytePanel6;
        private BytePanel bytePanel5;
        private BytePanel bytePanel4;
        private BytePanel bytePanel2;

    }
}