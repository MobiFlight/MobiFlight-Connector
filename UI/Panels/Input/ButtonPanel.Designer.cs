namespace MobiFlight.UI.Panels.Input
{
    partial class ButtonPanel
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
            this.onPressActionConfigPanel = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.onPressActionTypePanel = new MobiFlight.UI.Panels.Config.ActionTypePanel();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.onReleaseActionConfigPanel = new System.Windows.Forms.Panel();
            this.onReleaseActionTypePanel = new MobiFlight.UI.Panels.Config.ActionTypePanel();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // onPressActionConfigPanel
            // 
            this.onPressActionConfigPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.onPressActionConfigPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.onPressActionConfigPanel.Location = new System.Drawing.Point(0, 31);
            this.onPressActionConfigPanel.Margin = new System.Windows.Forms.Padding(0);
            this.onPressActionConfigPanel.Name = "onPressActionConfigPanel";
            this.onPressActionConfigPanel.Size = new System.Drawing.Size(332, 220);
            this.onPressActionConfigPanel.TabIndex = 19;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(340, 280);
            this.tabControl1.TabIndex = 20;
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.Controls.Add(this.onPressActionConfigPanel);
            this.tabPage2.Controls.Add(this.onPressActionTypePanel);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.tabPage2.Size = new System.Drawing.Size(332, 254);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "On Press";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // onPressActionTypePanel
            // 
            this.onPressActionTypePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.onPressActionTypePanel.Location = new System.Drawing.Point(0, 3);
            this.onPressActionTypePanel.Name = "onPressActionTypePanel";
            this.onPressActionTypePanel.Size = new System.Drawing.Size(332, 28);
            this.onPressActionTypePanel.TabIndex = 20;
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.Controls.Add(this.onReleaseActionConfigPanel);
            this.tabPage1.Controls.Add(this.onReleaseActionTypePanel);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.tabPage1.Size = new System.Drawing.Size(332, 224);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "On Release";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // onReleaseActionConfigPanel
            // 
            this.onReleaseActionConfigPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.onReleaseActionConfigPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.onReleaseActionConfigPanel.Location = new System.Drawing.Point(0, 31);
            this.onReleaseActionConfigPanel.Margin = new System.Windows.Forms.Padding(0);
            this.onReleaseActionConfigPanel.Name = "onReleaseActionConfigPanel";
            this.onReleaseActionConfigPanel.Size = new System.Drawing.Size(332, 190);
            this.onReleaseActionConfigPanel.TabIndex = 19;
            // 
            // onReleaseActionTypePanel
            // 
            this.onReleaseActionTypePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.onReleaseActionTypePanel.Location = new System.Drawing.Point(0, 3);
            this.onReleaseActionTypePanel.Name = "onReleaseActionTypePanel";
            this.onReleaseActionTypePanel.Size = new System.Drawing.Size(332, 28);
            this.onReleaseActionTypePanel.TabIndex = 20;
            // 
            // ButtonPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.MinimumSize = new System.Drawing.Size(300, 0);
            this.Name = "ButtonPanel";
            this.Size = new System.Drawing.Size(340, 280);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel onPressActionConfigPanel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage2;
        private MobiFlight.UI.Panels.Config.ActionTypePanel onPressActionTypePanel;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Panel onReleaseActionConfigPanel;
        private MobiFlight.UI.Panels.Config.ActionTypePanel onReleaseActionTypePanel;
    }
}
