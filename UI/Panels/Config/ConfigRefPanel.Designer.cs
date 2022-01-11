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
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.addReferenceButton = new System.Windows.Forms.Button();
            this.noConfigRefsPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.configRefItemPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.noConfigRefsPanel.SuspendLayout();
            this.panel3.SuspendLayout();
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
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.panel2);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.addReferenceButton);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // addReferenceButton
            // 
            resources.ApplyResources(this.addReferenceButton, "addReferenceButton");
            this.addReferenceButton.Name = "addReferenceButton";
            this.addReferenceButton.UseVisualStyleBackColor = true;
            this.addReferenceButton.Click += new System.EventHandler(this.addReferenceButton_Click);
            // 
            // noConfigRefsPanel
            // 
            resources.ApplyResources(this.noConfigRefsPanel, "noConfigRefsPanel");
            this.noConfigRefsPanel.Controls.Add(this.label1);
            this.noConfigRefsPanel.Name = "noConfigRefsPanel";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.configRefItemPanel);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // ConfigRefPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.noConfigRefsPanel);
            this.Controls.Add(this.panel1);
            this.Name = "ConfigRefPanel";
            this.configRefItemPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.noConfigRefsPanel.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
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
        private System.Windows.Forms.Panel noConfigRefsPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button addReferenceButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
    }
}
