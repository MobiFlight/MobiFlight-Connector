namespace MobiFlight.UI.Panels.Input
{
    partial class EncoderPanel
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
            this.onLeftActionConfigPanel = new System.Windows.Forms.Panel();
            this.encoderTabControl = new System.Windows.Forms.TabControl();
            this.onLeftTabPage = new System.Windows.Forms.TabPage();
            this.onLeftActionTypePanel = new MobiFlight.UI.Panels.Config.ActionTypePanel();
            this.onLeftFastTabPage = new System.Windows.Forms.TabPage();
            this.onLeftFastActionConfigPanel = new System.Windows.Forms.Panel();
            this.onLeftFastActionTypePanel = new MobiFlight.UI.Panels.Config.ActionTypePanel();
            this.onRightTabPage = new System.Windows.Forms.TabPage();
            this.onRightActionConfigPanel = new System.Windows.Forms.Panel();
            this.onRightActionTypePanel = new MobiFlight.UI.Panels.Config.ActionTypePanel();
            this.onRightFastTabPage = new System.Windows.Forms.TabPage();
            this.onRightFastActionConfigPanel = new System.Windows.Forms.Panel();
            this.onRightFastActionTypePanel = new MobiFlight.UI.Panels.Config.ActionTypePanel();
            this.encoderTabControl.SuspendLayout();
            this.onLeftTabPage.SuspendLayout();
            this.onLeftFastTabPage.SuspendLayout();
            this.onRightTabPage.SuspendLayout();
            this.onRightFastTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // onLeftActionConfigPanel
            // 
            this.onLeftActionConfigPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.onLeftActionConfigPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.onLeftActionConfigPanel.Location = new System.Drawing.Point(0, 38);
            this.onLeftActionConfigPanel.Margin = new System.Windows.Forms.Padding(0);
            this.onLeftActionConfigPanel.Name = "onLeftActionConfigPanel";
            this.onLeftActionConfigPanel.Size = new System.Drawing.Size(422, 213);
            this.onLeftActionConfigPanel.TabIndex = 19;
            // 
            // encoderTabControl
            // 
            this.encoderTabControl.Controls.Add(this.onLeftTabPage);
            this.encoderTabControl.Controls.Add(this.onLeftFastTabPage);
            this.encoderTabControl.Controls.Add(this.onRightTabPage);
            this.encoderTabControl.Controls.Add(this.onRightFastTabPage);
            this.encoderTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.encoderTabControl.Location = new System.Drawing.Point(0, 0);
            this.encoderTabControl.Name = "encoderTabControl";
            this.encoderTabControl.SelectedIndex = 0;
            this.encoderTabControl.Size = new System.Drawing.Size(430, 280);
            this.encoderTabControl.TabIndex = 20;
            // 
            // onLeftTabPage
            // 
            this.onLeftTabPage.AutoScroll = true;
            this.onLeftTabPage.Controls.Add(this.onLeftActionConfigPanel);
            this.onLeftTabPage.Controls.Add(this.onLeftActionTypePanel);
            this.onLeftTabPage.Location = new System.Drawing.Point(4, 22);
            this.onLeftTabPage.Name = "onLeftTabPage";
            this.onLeftTabPage.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.onLeftTabPage.Size = new System.Drawing.Size(422, 254);
            this.onLeftTabPage.TabIndex = 1;
            this.onLeftTabPage.Text = "On Left";
            this.onLeftTabPage.UseVisualStyleBackColor = true;
            // 
            // onLeftActionTypePanel
            // 
            this.onLeftActionTypePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.onLeftActionTypePanel.Location = new System.Drawing.Point(0, 3);
            this.onLeftActionTypePanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.onLeftActionTypePanel.Name = "onLeftActionTypePanel";
            this.onLeftActionTypePanel.Size = new System.Drawing.Size(422, 35);
            this.onLeftActionTypePanel.TabIndex = 20;
            // 
            // onLeftFastTabPage
            // 
            this.onLeftFastTabPage.AutoScroll = true;
            this.onLeftFastTabPage.Controls.Add(this.onLeftFastActionConfigPanel);
            this.onLeftFastTabPage.Controls.Add(this.onLeftFastActionTypePanel);
            this.onLeftFastTabPage.Location = new System.Drawing.Point(4, 22);
            this.onLeftFastTabPage.Name = "onLeftFastTabPage";
            this.onLeftFastTabPage.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.onLeftFastTabPage.Size = new System.Drawing.Size(422, 254);
            this.onLeftFastTabPage.TabIndex = 3;
            this.onLeftFastTabPage.Text = "On Left (Fast)";
            this.onLeftFastTabPage.UseVisualStyleBackColor = true;
            // 
            // onLeftFastActionConfigPanel
            // 
            this.onLeftFastActionConfigPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.onLeftFastActionConfigPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.onLeftFastActionConfigPanel.Location = new System.Drawing.Point(0, 38);
            this.onLeftFastActionConfigPanel.Margin = new System.Windows.Forms.Padding(0);
            this.onLeftFastActionConfigPanel.Name = "onLeftFastActionConfigPanel";
            this.onLeftFastActionConfigPanel.Size = new System.Drawing.Size(422, 213);
            this.onLeftFastActionConfigPanel.TabIndex = 19;
            // 
            // onLeftFastActionTypePanel
            // 
            this.onLeftFastActionTypePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.onLeftFastActionTypePanel.Location = new System.Drawing.Point(0, 3);
            this.onLeftFastActionTypePanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.onLeftFastActionTypePanel.Name = "onLeftFastActionTypePanel";
            this.onLeftFastActionTypePanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.onLeftFastActionTypePanel.Size = new System.Drawing.Size(422, 35);
            this.onLeftFastActionTypePanel.TabIndex = 20;
            // 
            // onRightTabPage
            // 
            this.onRightTabPage.AutoScroll = true;
            this.onRightTabPage.Controls.Add(this.onRightActionConfigPanel);
            this.onRightTabPage.Controls.Add(this.onRightActionTypePanel);
            this.onRightTabPage.Location = new System.Drawing.Point(4, 22);
            this.onRightTabPage.Name = "onRightTabPage";
            this.onRightTabPage.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.onRightTabPage.Size = new System.Drawing.Size(422, 254);
            this.onRightTabPage.TabIndex = 2;
            this.onRightTabPage.Text = "On Right";
            this.onRightTabPage.UseVisualStyleBackColor = true;
            // 
            // onRightActionConfigPanel
            // 
            this.onRightActionConfigPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.onRightActionConfigPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.onRightActionConfigPanel.Location = new System.Drawing.Point(0, 38);
            this.onRightActionConfigPanel.Margin = new System.Windows.Forms.Padding(0);
            this.onRightActionConfigPanel.Name = "onRightActionConfigPanel";
            this.onRightActionConfigPanel.Size = new System.Drawing.Size(422, 213);
            this.onRightActionConfigPanel.TabIndex = 19;
            // 
            // onRightActionTypePanel
            // 
            this.onRightActionTypePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.onRightActionTypePanel.Location = new System.Drawing.Point(0, 3);
            this.onRightActionTypePanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.onRightActionTypePanel.Name = "onRightActionTypePanel";
            this.onRightActionTypePanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.onRightActionTypePanel.Size = new System.Drawing.Size(422, 35);
            this.onRightActionTypePanel.TabIndex = 20;
            // 
            // onRightFastTabPage
            // 
            this.onRightFastTabPage.AutoScroll = true;
            this.onRightFastTabPage.Controls.Add(this.onRightFastActionConfigPanel);
            this.onRightFastTabPage.Controls.Add(this.onRightFastActionTypePanel);
            this.onRightFastTabPage.Location = new System.Drawing.Point(4, 22);
            this.onRightFastTabPage.Name = "onRightFastTabPage";
            this.onRightFastTabPage.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.onRightFastTabPage.Size = new System.Drawing.Size(422, 254);
            this.onRightFastTabPage.TabIndex = 4;
            this.onRightFastTabPage.Text = "On Right (Fast)";
            this.onRightFastTabPage.UseVisualStyleBackColor = true;
            // 
            // onRightFastActionConfigPanel
            // 
            this.onRightFastActionConfigPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.onRightFastActionConfigPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.onRightFastActionConfigPanel.Location = new System.Drawing.Point(0, 38);
            this.onRightFastActionConfigPanel.Margin = new System.Windows.Forms.Padding(0);
            this.onRightFastActionConfigPanel.Name = "onRightFastActionConfigPanel";
            this.onRightFastActionConfigPanel.Size = new System.Drawing.Size(422, 213);
            this.onRightFastActionConfigPanel.TabIndex = 19;
            // 
            // onRightFastActionTypePanel
            // 
            this.onRightFastActionTypePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.onRightFastActionTypePanel.Location = new System.Drawing.Point(0, 3);
            this.onRightFastActionTypePanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.onRightFastActionTypePanel.Name = "onRightFastActionTypePanel";
            this.onRightFastActionTypePanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.onRightFastActionTypePanel.Size = new System.Drawing.Size(422, 35);
            this.onRightFastActionTypePanel.TabIndex = 20;
            // 
            // EncoderPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.encoderTabControl);
            this.MinimumSize = new System.Drawing.Size(300, 0);
            this.Name = "EncoderPanel";
            this.Size = new System.Drawing.Size(430, 280);
            this.encoderTabControl.ResumeLayout(false);
            this.onLeftTabPage.ResumeLayout(false);
            this.onLeftFastTabPage.ResumeLayout(false);
            this.onRightTabPage.ResumeLayout(false);
            this.onRightFastTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel onLeftActionConfigPanel;
        private System.Windows.Forms.TabControl encoderTabControl;
        private System.Windows.Forms.TabPage onLeftTabPage;
        private MobiFlight.UI.Panels.Config.ActionTypePanel onLeftActionTypePanel;
        private System.Windows.Forms.TabPage onRightTabPage;
        private System.Windows.Forms.Panel onRightActionConfigPanel;
        private MobiFlight.UI.Panels.Config.ActionTypePanel onRightActionTypePanel;
        private System.Windows.Forms.TabPage onLeftFastTabPage;
        private System.Windows.Forms.Panel onLeftFastActionConfigPanel;
        private MobiFlight.UI.Panels.Config.ActionTypePanel onLeftFastActionTypePanel;
        private System.Windows.Forms.TabPage onRightFastTabPage;
        private System.Windows.Forms.Panel onRightFastActionConfigPanel;
        private MobiFlight.UI.Panels.Config.ActionTypePanel onRightFastActionTypePanel;
    }
}
