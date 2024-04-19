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
            this.onPressTabPage = new System.Windows.Forms.TabPage();
            this.onPressActionTypePanel = new MobiFlight.UI.Panels.Config.ActionTypePanel();
            this.onReleaseTabPage = new System.Windows.Forms.TabPage();
            this.onReleaseActionConfigPanel = new System.Windows.Forms.Panel();
            this.onReleaseActionTypePanel = new MobiFlight.UI.Panels.Config.ActionTypePanel();
            this.onHoldTabPage = new System.Windows.Forms.TabPage();
            this.onHoldActionConfigPanel = new System.Windows.Forms.Panel();
            this.onHoldActionTypePanel = new MobiFlight.UI.Panels.Config.ActionTypePanel();
            this.onHoldSettingsPanel = new System.Windows.Forms.Panel();
            this.msHoldLabel = new System.Windows.Forms.Label();
            this.repeatTextBox = new System.Windows.Forms.TextBox();
            this.repeatLabel = new System.Windows.Forms.Label();
            this.holdDelayLabel = new System.Windows.Forms.Label();
            this.holdDelayTextBox = new System.Windows.Forms.TextBox();
            this.onLongReleaseTabPage = new System.Windows.Forms.TabPage();
            this.onLongRelActionConfigPanel = new System.Windows.Forms.Panel();
            this.onLongReleaseActionTypePanel = new MobiFlight.UI.Panels.Config.ActionTypePanel();
            this.onLongReleaseSettingsPanel = new System.Windows.Forms.Panel();
            this.msLongReleaseLabel = new System.Windows.Forms.Label();
            this.longReleaseTextBox = new System.Windows.Forms.TextBox();
            this.longReleaseDelayLabel = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.onPressTabPage.SuspendLayout();
            this.onReleaseTabPage.SuspendLayout();
            this.onHoldTabPage.SuspendLayout();
            this.onHoldSettingsPanel.SuspendLayout();
            this.onLongReleaseTabPage.SuspendLayout();
            this.onLongReleaseSettingsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // onPressActionConfigPanel
            // 
            this.onPressActionConfigPanel.AutoSize = true;
            this.onPressActionConfigPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.onPressActionConfigPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.onPressActionConfigPanel.Location = new System.Drawing.Point(0, 59);
            this.onPressActionConfigPanel.Margin = new System.Windows.Forms.Padding(0);
            this.onPressActionConfigPanel.MinimumSize = new System.Drawing.Size(100, 154);
            this.onPressActionConfigPanel.Name = "onPressActionConfigPanel";
            this.onPressActionConfigPanel.Size = new System.Drawing.Size(592, 154);
            this.onPressActionConfigPanel.TabIndex = 19;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.onPressTabPage);
            this.tabControl1.Controls.Add(this.onReleaseTabPage);
            this.tabControl1.Controls.Add(this.onHoldTabPage);
            this.tabControl1.Controls.Add(this.onLongReleaseTabPage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(600, 154);
            this.tabControl1.TabIndex = 21;
            // 
            // onPressTabPage
            // 
            this.onPressTabPage.Controls.Add(this.onPressActionConfigPanel);
            this.onPressTabPage.Controls.Add(this.onPressActionTypePanel);
            this.onPressTabPage.Location = new System.Drawing.Point(4, 29);
            this.onPressTabPage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.onPressTabPage.Name = "onPressTabPage";
            this.onPressTabPage.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.onPressTabPage.Size = new System.Drawing.Size(592, 121);
            this.onPressTabPage.TabIndex = 1;
            this.onPressTabPage.Text = "On Press";
            this.onPressTabPage.UseVisualStyleBackColor = true;
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
            // onReleaseTabPage
            // 
            this.onReleaseTabPage.Controls.Add(this.onReleaseActionConfigPanel);
            this.onReleaseTabPage.Controls.Add(this.onReleaseActionTypePanel);
            this.onReleaseTabPage.Location = new System.Drawing.Point(4, 29);
            this.onReleaseTabPage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.onReleaseTabPage.Name = "onReleaseTabPage";
            this.onReleaseTabPage.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.onReleaseTabPage.Size = new System.Drawing.Size(592, 121);
            this.onReleaseTabPage.TabIndex = 2;
            this.onReleaseTabPage.Text = "On Release";
            this.onReleaseTabPage.UseVisualStyleBackColor = true;
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
            // onHoldTabPage
            // 
            this.onHoldTabPage.Controls.Add(this.onHoldActionConfigPanel);
            this.onHoldTabPage.Controls.Add(this.onHoldActionTypePanel);
            this.onHoldTabPage.Controls.Add(this.onHoldSettingsPanel);
            this.onHoldTabPage.Location = new System.Drawing.Point(4, 29);
            this.onHoldTabPage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.onHoldTabPage.Name = "onHoldTabPage";
            this.onHoldTabPage.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.onHoldTabPage.Size = new System.Drawing.Size(592, 121);
            this.onHoldTabPage.TabIndex = 3;
            this.onHoldTabPage.Text = "On Hold";
            this.onHoldTabPage.UseVisualStyleBackColor = true;
            // 
            // onHoldActionConfigPanel
            // 
            this.onHoldActionConfigPanel.AutoSize = true;
            this.onHoldActionConfigPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.onHoldActionConfigPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.onHoldActionConfigPanel.Location = new System.Drawing.Point(0, 115);
            this.onHoldActionConfigPanel.MinimumSize = new System.Drawing.Size(0, 154);
            this.onHoldActionConfigPanel.Name = "onHoldActionConfigPanel";
            this.onHoldActionConfigPanel.Size = new System.Drawing.Size(592, 154);
            this.onHoldActionConfigPanel.TabIndex = 2;
            // 
            // onHoldActionTypePanel
            // 
            this.onHoldActionTypePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.onHoldActionTypePanel.Location = new System.Drawing.Point(0, 59);
            this.onHoldActionTypePanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 9);
            this.onHoldActionTypePanel.Name = "onHoldActionTypePanel";
            this.onHoldActionTypePanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 9);
            this.onHoldActionTypePanel.Size = new System.Drawing.Size(592, 56);
            this.onHoldActionTypePanel.TabIndex = 1;
            // 
            // onHoldSettingsPanel
            // 
            this.onHoldSettingsPanel.BackColor = System.Drawing.Color.Transparent;
            this.onHoldSettingsPanel.Controls.Add(this.msHoldLabel);
            this.onHoldSettingsPanel.Controls.Add(this.repeatTextBox);
            this.onHoldSettingsPanel.Controls.Add(this.repeatLabel);
            this.onHoldSettingsPanel.Controls.Add(this.holdDelayLabel);
            this.onHoldSettingsPanel.Controls.Add(this.holdDelayTextBox);
            this.onHoldSettingsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.onHoldSettingsPanel.Location = new System.Drawing.Point(0, 5);
            this.onHoldSettingsPanel.Name = "onHoldSettingsPanel";
            this.onHoldSettingsPanel.Padding = new System.Windows.Forms.Padding(3);
            this.onHoldSettingsPanel.Size = new System.Drawing.Size(592, 54);
            this.onHoldSettingsPanel.TabIndex = 0;
            // 
            // msHoldLabel
            // 
            this.msHoldLabel.AutoSize = true;
            this.msHoldLabel.Location = new System.Drawing.Point(398, 18);
            this.msHoldLabel.Name = "msHoldLabel";
            this.msHoldLabel.Size = new System.Drawing.Size(30, 20);
            this.msHoldLabel.TabIndex = 4;
            this.msHoldLabel.Text = "ms";
            // 
            // repeatTextBox
            // 
            this.repeatTextBox.Location = new System.Drawing.Point(344, 15);
            this.repeatTextBox.Name = "repeatTextBox";
            this.repeatTextBox.Size = new System.Drawing.Size(51, 26);
            this.repeatTextBox.TabIndex = 3;
            this.repeatTextBox.Text = "0";
            this.repeatTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.repeatTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // repeatLabel
            // 
            this.repeatLabel.AutoSize = true;
            this.repeatLabel.Location = new System.Drawing.Point(213, 18);
            this.repeatLabel.Name = "repeatLabel";
            this.repeatLabel.Size = new System.Drawing.Size(125, 20);
            this.repeatLabel.TabIndex = 2;
            this.repeatLabel.Text = "ms, repeat every";
            // 
            // holdDelayLabel
            // 
            this.holdDelayLabel.Location = new System.Drawing.Point(31, 18);
            this.holdDelayLabel.Name = "holdDelayLabel";
            this.holdDelayLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.holdDelayLabel.Size = new System.Drawing.Size(123, 20);
            this.holdDelayLabel.TabIndex = 1;
            this.holdDelayLabel.Text = "Delay:";
            this.holdDelayLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // holdDelayTextBox
            // 
            this.holdDelayTextBox.Location = new System.Drawing.Point(160, 15);
            this.holdDelayTextBox.Name = "holdDelayTextBox";
            this.holdDelayTextBox.Size = new System.Drawing.Size(51, 26);
            this.holdDelayTextBox.TabIndex = 0;
            this.holdDelayTextBox.Text = "350";
            this.holdDelayTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.holdDelayTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // onLongReleaseTabPage
            // 
            this.onLongReleaseTabPage.Controls.Add(this.onLongRelActionConfigPanel);
            this.onLongReleaseTabPage.Controls.Add(this.onLongReleaseActionTypePanel);
            this.onLongReleaseTabPage.Controls.Add(this.onLongReleaseSettingsPanel);
            this.onLongReleaseTabPage.Location = new System.Drawing.Point(4, 29);
            this.onLongReleaseTabPage.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.onLongReleaseTabPage.Name = "onLongReleaseTabPage";
            this.onLongReleaseTabPage.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.onLongReleaseTabPage.Size = new System.Drawing.Size(592, 121);
            this.onLongReleaseTabPage.TabIndex = 4;
            this.onLongReleaseTabPage.Text = "On Long Release";
            this.onLongReleaseTabPage.UseVisualStyleBackColor = true;
            // 
            // onLongRelActionConfigPanel
            // 
            this.onLongRelActionConfigPanel.AutoSize = true;
            this.onLongRelActionConfigPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.onLongRelActionConfigPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.onLongRelActionConfigPanel.Location = new System.Drawing.Point(0, 112);
            this.onLongRelActionConfigPanel.MinimumSize = new System.Drawing.Size(0, 154);
            this.onLongRelActionConfigPanel.Name = "onLongRelActionConfigPanel";
            this.onLongRelActionConfigPanel.Size = new System.Drawing.Size(592, 154);
            this.onLongRelActionConfigPanel.TabIndex = 5;
            // 
            // onLongReleaseActionTypePanel
            // 
            this.onLongReleaseActionTypePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.onLongReleaseActionTypePanel.Location = new System.Drawing.Point(0, 59);
            this.onLongReleaseActionTypePanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 9);
            this.onLongReleaseActionTypePanel.Name = "onLongReleaseActionTypePanel";
            this.onLongReleaseActionTypePanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 9);
            this.onLongReleaseActionTypePanel.Size = new System.Drawing.Size(592, 53);
            this.onLongReleaseActionTypePanel.TabIndex = 4;
            // 
            // onLongReleaseSettingsPanel
            // 
            this.onLongReleaseSettingsPanel.Controls.Add(this.msLongReleaseLabel);
            this.onLongReleaseSettingsPanel.Controls.Add(this.longReleaseTextBox);
            this.onLongReleaseSettingsPanel.Controls.Add(this.longReleaseDelayLabel);
            this.onLongReleaseSettingsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.onLongReleaseSettingsPanel.Location = new System.Drawing.Point(0, 5);
            this.onLongReleaseSettingsPanel.Name = "onLongReleaseSettingsPanel";
            this.onLongReleaseSettingsPanel.Padding = new System.Windows.Forms.Padding(3);
            this.onLongReleaseSettingsPanel.Size = new System.Drawing.Size(592, 54);
            this.onLongReleaseSettingsPanel.TabIndex = 3;
            // 
            // msLongReleaseLabel
            // 
            this.msLongReleaseLabel.AutoSize = true;
            this.msLongReleaseLabel.Location = new System.Drawing.Point(213, 18);
            this.msLongReleaseLabel.Name = "msLongReleaseLabel";
            this.msLongReleaseLabel.Size = new System.Drawing.Size(30, 20);
            this.msLongReleaseLabel.TabIndex = 2;
            this.msLongReleaseLabel.Text = "ms";
            // 
            // longReleaseTextBox
            // 
            this.longReleaseTextBox.Location = new System.Drawing.Point(160, 15);
            this.longReleaseTextBox.Name = "longReleaseTextBox";
            this.longReleaseTextBox.Size = new System.Drawing.Size(51, 26);
            this.longReleaseTextBox.TabIndex = 1;
            this.longReleaseTextBox.Text = "350";
            this.longReleaseTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.longReleaseTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            // 
            // longReleaseDelayLabel
            // 
            this.longReleaseDelayLabel.Location = new System.Drawing.Point(6, 18);
            this.longReleaseDelayLabel.Name = "longReleaseDelayLabel";
            this.longReleaseDelayLabel.Size = new System.Drawing.Size(148, 23);
            this.longReleaseDelayLabel.TabIndex = 0;
            this.longReleaseDelayLabel.Text = "Delay:";
            this.longReleaseDelayLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
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
            this.onPressTabPage.ResumeLayout(false);
            this.onPressTabPage.PerformLayout();
            this.onReleaseTabPage.ResumeLayout(false);
            this.onReleaseTabPage.PerformLayout();
            this.onHoldTabPage.ResumeLayout(false);
            this.onHoldTabPage.PerformLayout();
            this.onHoldSettingsPanel.ResumeLayout(false);
            this.onHoldSettingsPanel.PerformLayout();
            this.onLongReleaseTabPage.ResumeLayout(false);
            this.onLongReleaseTabPage.PerformLayout();
            this.onLongReleaseSettingsPanel.ResumeLayout(false);
            this.onLongReleaseSettingsPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel onPressActionConfigPanel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage onPressTabPage;
        private MobiFlight.UI.Panels.Config.ActionTypePanel onPressActionTypePanel;
        private System.Windows.Forms.TabPage onReleaseTabPage;
        private System.Windows.Forms.Panel onReleaseActionConfigPanel;
        private MobiFlight.UI.Panels.Config.ActionTypePanel onReleaseActionTypePanel;
        private System.Windows.Forms.TabPage onLongReleaseTabPage;
        private System.Windows.Forms.TabPage onHoldTabPage;
        private System.Windows.Forms.Panel onHoldSettingsPanel;
        private Config.ActionTypePanel onHoldActionTypePanel;
        private System.Windows.Forms.Panel onHoldActionConfigPanel;    
        private System.Windows.Forms.TextBox holdDelayTextBox;
        private System.Windows.Forms.Label msHoldLabel;
        private System.Windows.Forms.TextBox repeatTextBox;
        private System.Windows.Forms.Label repeatLabel;
        private System.Windows.Forms.Label holdDelayLabel;
        private System.Windows.Forms.Panel onLongRelActionConfigPanel;
        private Config.ActionTypePanel onLongReleaseActionTypePanel;
        private System.Windows.Forms.Panel onLongReleaseSettingsPanel;
        private System.Windows.Forms.Label longReleaseDelayLabel;
        private System.Windows.Forms.TextBox longReleaseTextBox;
        private System.Windows.Forms.Label msLongReleaseLabel;
    }
}
