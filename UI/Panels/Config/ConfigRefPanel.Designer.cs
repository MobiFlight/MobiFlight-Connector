namespace MobiFlight.UI.Panels.Config
{
    partial class ConfigRefPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigRefPanel));
            this.configRefItemPanel = new System.Windows.Forms.Panel();
            this.configRefPanelItem4 = new MobiFlight.UI.Panels.Config.ConfigRefPanelItem();
            this.configRefPanelItem3 = new MobiFlight.UI.Panels.Config.ConfigRefPanelItem();
            this.configRefPanelItem2 = new MobiFlight.UI.Panels.Config.ConfigRefPanelItem();
            this.configRefPanelItem1 = new MobiFlight.UI.Panels.Config.ConfigRefPanelItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.noConfigRefsPanel = new System.Windows.Forms.Panel();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.configRefItemPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.noConfigRefsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // configRefItemPanel
            // 
            resources.ApplyResources(this.configRefItemPanel, "configRefItemPanel");
            this.configRefItemPanel.Controls.Add(this.configRefPanelItem4);
            this.configRefItemPanel.Controls.Add(this.configRefPanelItem3);
            this.configRefItemPanel.Controls.Add(this.configRefPanelItem2);
            this.configRefItemPanel.Controls.Add(this.configRefPanelItem1);
            this.configRefItemPanel.Name = "configRefItemPanel";
            // 
            // configRefPanelItem4
            // 
            resources.ApplyResources(this.configRefPanelItem4, "configRefPanelItem4");
            this.configRefPanelItem4.Name = "configRefPanelItem4";
            // 
            // configRefPanelItem3
            // 
            resources.ApplyResources(this.configRefPanelItem3, "configRefPanelItem3");
            this.configRefPanelItem3.Name = "configRefPanelItem3";
            // 
            // configRefPanelItem2
            // 
            resources.ApplyResources(this.configRefPanelItem2, "configRefPanelItem2");
            this.configRefPanelItem2.Name = "configRefPanelItem2";
            // 
            // configRefPanelItem1
            // 
            resources.ApplyResources(this.configRefPanelItem1, "configRefPanelItem1");
            this.configRefPanelItem1.Name = "configRefPanelItem1";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Name = "panel1";
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.TabStop = false;
            // 
            // noConfigRefsPanel
            // 
            resources.ApplyResources(this.noConfigRefsPanel, "noConfigRefsPanel");
            this.noConfigRefsPanel.Controls.Add(this.textBox2);
            this.noConfigRefsPanel.Name = "noConfigRefsPanel";
            // 
            // textBox2
            // 
            resources.ApplyResources(this.textBox2, "textBox2");
            this.textBox2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Cursor = System.Windows.Forms.Cursors.Default;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.TabStop = false;
            // 
            // ConfigRefPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.configRefItemPanel);
            this.Controls.Add(this.noConfigRefsPanel);
            this.Controls.Add(this.panel1);
            this.Name = "ConfigRefPanel";
            this.Load += new System.EventHandler(this.ConfigRefPanel_Load);
            this.configRefItemPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.noConfigRefsPanel.ResumeLayout(false);
            this.noConfigRefsPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel configRefItemPanel;
        private ConfigRefPanelItem configRefPanelItem4;
        private ConfigRefPanelItem configRefPanelItem3;
        private ConfigRefPanelItem configRefPanelItem2;
        private ConfigRefPanelItem configRefPanelItem1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Panel noConfigRefsPanel;
        private System.Windows.Forms.TextBox textBox2;
    }
}
