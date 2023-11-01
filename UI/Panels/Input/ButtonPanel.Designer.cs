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
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.onReleaseActionConfigPanel = new System.Windows.Forms.Panel();
            this.onPressActionTypePanel = new MobiFlight.UI.Panels.Config.ActionTypePanel();
            this.onReleaseActionTypePanel = new MobiFlight.UI.Panels.Config.ActionTypePanel();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.onLongReleaseActionConfigPanel = new System.Windows.Forms.Panel();
            this.onLongReleaseActionTypePanel = new MobiFlight.UI.Panels.Config.ActionTypePanel();
            this.onLongRelActionConfigPanel = new System.Windows.Forms.Panel();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // onPressActionConfigPanel
            // 
            this.onPressActionConfigPanel.AutoSize = true;
            this.onPressActionConfigPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.onPressActionConfigPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.onPressActionConfigPanel.Location = new System.Drawing.Point(0, 59);
            this.onPressActionConfigPanel.Margin = new System.Windows.Forms.Padding(0);
            this.onPressActionConfigPanel.MinimumSize = new System.Drawing.Size(0, 154);
            this.onPressActionConfigPanel.Name = "onPressActionConfigPanel";
            this.onPressActionConfigPanel.Size = new System.Drawing.Size(592, 154);
            this.onPressActionConfigPanel.TabIndex = 19;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(600, 154);
            this.tabControl1.TabIndex = 21;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.onPressActionConfigPanel);
            this.tabPage2.Controls.Add(this.onPressActionTypePanel);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.tabPage2.Size = new System.Drawing.Size(592, 121);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "On Press";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.onReleaseActionConfigPanel);
            this.tabPage1.Controls.Add(this.onReleaseActionTypePanel);
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.tabPage1.Size = new System.Drawing.Size(592, 121);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "On Release";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // onReleaseActionConfigPanel
            // 
            this.onReleaseActionConfigPanel.AutoSize = true;
            this.onReleaseActionConfigPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.onReleaseActionConfigPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.onReleaseActionConfigPanel.Location = new System.Drawing.Point(0, 59);
            this.onReleaseActionConfigPanel.Margin = new System.Windows.Forms.Padding(0);
            this.onReleaseActionConfigPanel.MinimumSize = new System.Drawing.Size(0, 154);
            this.onReleaseActionConfigPanel.Name = "onReleaseActionConfigPanel";
            this.onReleaseActionConfigPanel.Size = new System.Drawing.Size(592, 154);
            this.onReleaseActionConfigPanel.TabIndex = 19;
            // 
            // onPressActionTypePanel
            // 
            this.onPressActionTypePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.onPressActionTypePanel.Location = new System.Drawing.Point(0, 5);
            this.onPressActionTypePanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 9);
            this.onPressActionTypePanel.Name = "onPressActionTypePanel";
            this.onPressActionTypePanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 9);
            this.onPressActionTypePanel.Size = new System.Drawing.Size(592, 54);
            this.onPressActionTypePanel.TabIndex = 20;
            // 
            // onReleaseActionTypePanel
            // 
            this.onReleaseActionTypePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.onReleaseActionTypePanel.Location = new System.Drawing.Point(0, 5);
            this.onReleaseActionTypePanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 9);
            this.onReleaseActionTypePanel.Name = "onReleaseActionTypePanel";
            this.onReleaseActionTypePanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 9);
            this.onReleaseActionTypePanel.Size = new System.Drawing.Size(592, 54);
            this.onReleaseActionTypePanel.TabIndex = 20;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.onLongRelActionConfigPanel);
            this.tabPage3.Controls.Add(this.onLongReleaseActionTypePanel);
            this.tabPage3.Controls.Add(this.onLongReleaseActionConfigPanel);
            this.tabPage3.Location = new System.Drawing.Point(4, 29);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.tabPage3.Size = new System.Drawing.Size(592, 121);
            this.tabPage3.TabIndex = 3;
            this.tabPage3.Text = "On Long Release";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // onLongReleaseActionConfigPanel
            // 
            this.onLongReleaseActionConfigPanel.AutoSize = true;
            this.onLongReleaseActionConfigPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.onLongReleaseActionConfigPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.onLongReleaseActionConfigPanel.Location = new System.Drawing.Point(0, 5);
            this.onLongReleaseActionConfigPanel.Name = "onLongReleaseActionConfigPanel";
            this.onLongReleaseActionConfigPanel.Size = new System.Drawing.Size(592, 0);
            this.onLongReleaseActionConfigPanel.TabIndex = 0;
            // 
            // onLongReleaseActionTypePanel
            // 
            this.onLongReleaseActionTypePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.onLongReleaseActionTypePanel.Location = new System.Drawing.Point(0, 5);
            this.onLongReleaseActionTypePanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 9);
            this.onLongReleaseActionTypePanel.Name = "onLongReleaseActionTypePanel";
            this.onLongReleaseActionTypePanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 9);
            this.onLongReleaseActionTypePanel.Size = new System.Drawing.Size(592, 55);
            this.onLongReleaseActionTypePanel.TabIndex = 1;
            // 
            // onLongRelActionConfigPanel
            // 
            this.onLongRelActionConfigPanel.AutoSize = true;
            this.onLongRelActionConfigPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.onLongRelActionConfigPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.onLongRelActionConfigPanel.Location = new System.Drawing.Point(0, 60);
            this.onLongRelActionConfigPanel.MinimumSize = new System.Drawing.Size(0, 154);
            this.onLongRelActionConfigPanel.Name = "onLongRelActionConfigPanel";
            this.onLongRelActionConfigPanel.Size = new System.Drawing.Size(592, 154);
            this.onLongRelActionConfigPanel.TabIndex = 2;
            // 
            // ButtonPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tabControl1);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimumSize = new System.Drawing.Size(600, 154);
            this.Name = "ButtonPanel";
            this.Size = new System.Drawing.Size(600, 154);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
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
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Panel onLongReleaseActionConfigPanel;
        private System.Windows.Forms.Panel onLongRelActionConfigPanel;
        private Config.ActionTypePanel onLongReleaseActionTypePanel;
    }
}
