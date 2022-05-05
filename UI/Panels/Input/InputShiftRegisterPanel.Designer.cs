namespace MobiFlight.UI.Panels.Input
{
    partial class InputShiftRegisterPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputShiftRegisterPanel));
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
            resources.ApplyResources(this.onPressActionConfigPanel, "onPressActionConfigPanel");
            this.onPressActionConfigPanel.Name = "onPressActionConfigPanel";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.onPressActionConfigPanel);
            this.tabPage2.Controls.Add(this.onPressActionTypePanel);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // onPressActionTypePanel
            // 
            resources.ApplyResources(this.onPressActionTypePanel, "onPressActionTypePanel");
            this.onPressActionTypePanel.Name = "onPressActionTypePanel";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.onReleaseActionConfigPanel);
            this.tabPage1.Controls.Add(this.onReleaseActionTypePanel);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // onReleaseActionConfigPanel
            // 
            resources.ApplyResources(this.onReleaseActionConfigPanel, "onReleaseActionConfigPanel");
            this.onReleaseActionConfigPanel.Name = "onReleaseActionConfigPanel";
            // 
            // onReleaseActionTypePanel
            // 
            resources.ApplyResources(this.onReleaseActionTypePanel, "onReleaseActionTypePanel");
            this.onReleaseActionTypePanel.Name = "onReleaseActionTypePanel";
            // 
            // InputShiftRegisterPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.DoubleBuffered = true;
            this.Name = "InputShiftRegisterPanel";
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
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