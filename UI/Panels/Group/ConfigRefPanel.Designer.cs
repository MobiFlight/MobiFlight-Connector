namespace MobiFlight.UI.Panels.Group
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
            this.configRefPanelItem4 = new MobiFlight.UI.Panels.Group.ConfigRefPanelItem();
            this.configRefPanelItem3 = new MobiFlight.UI.Panels.Group.ConfigRefPanelItem();
            this.configRefPanelItem2 = new MobiFlight.UI.Panels.Group.ConfigRefPanelItem();
            this.configRefPanelItem1 = new MobiFlight.UI.Panels.Group.ConfigRefPanelItem();
            this.configRefItemPanel.SuspendLayout();
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
            // ConfigRefPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.configRefItemPanel);
            this.Name = "ConfigRefPanel";
            this.Load += new System.EventHandler(this.ConfigRefPanel_Load);
            this.configRefItemPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel configRefItemPanel;
        private ConfigRefPanelItem configRefPanelItem4;
        private ConfigRefPanelItem configRefPanelItem3;
        private ConfigRefPanelItem configRefPanelItem2;
        private ConfigRefPanelItem configRefPanelItem1;
    }
}
