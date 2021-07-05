namespace MobiFlight.UI.Panels.Input
{
    partial class AnalogPanel
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

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.onChangeActionConfigPanel = new System.Windows.Forms.Panel();
            this.onChangeActionTypePanel = new MobiFlight.UI.Panels.Config.ActionTypePanel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.Controls.Add(this.onChangeActionConfigPanel);
            this.tabPage2.Controls.Add(this.onChangeActionTypePanel);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.tabPage2.Size = new System.Drawing.Size(445, 279);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "On Change";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // onChangeActionConfigPanel
            // 
            this.onChangeActionConfigPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.onChangeActionConfigPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.onChangeActionConfigPanel.Location = new System.Drawing.Point(0, 38);
            this.onChangeActionConfigPanel.Margin = new System.Windows.Forms.Padding(0);
            this.onChangeActionConfigPanel.Name = "onChangeActionConfigPanel";
            this.onChangeActionConfigPanel.Size = new System.Drawing.Size(445, 237);
            this.onChangeActionConfigPanel.TabIndex = 19;
            // 
            // onChangeActionTypePanel
            // 
            this.onChangeActionTypePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.onChangeActionTypePanel.Location = new System.Drawing.Point(0, 4);
            this.onChangeActionTypePanel.Margin = new System.Windows.Forms.Padding(5);
            this.onChangeActionTypePanel.Name = "onChangeActionTypePanel";
            this.onChangeActionTypePanel.Size = new System.Drawing.Size(445, 34);
            this.onChangeActionTypePanel.TabIndex = 20;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(453, 308);
            this.tabControl1.TabIndex = 20;
            // 
            // AnalogPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(400, 0);
            this.Name = "AnalogPanel";
            this.Size = new System.Drawing.Size(453, 308);
            this.tabPage2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion        

        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel onChangeActionConfigPanel;
        private Config.ActionTypePanel onChangeActionTypePanel;
        private System.Windows.Forms.TabControl tabControl1;
    }
}
