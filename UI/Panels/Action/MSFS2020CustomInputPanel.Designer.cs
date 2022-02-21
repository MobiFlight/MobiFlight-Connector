namespace MobiFlight.UI.Panels.Action
{
    partial class MSFS2020CustomInputPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MSFS2020CustomInputPanel));
            this.DescriptionLabel = new System.Windows.Forms.Label();
            this.HintLabel = new System.Windows.Forms.Label();
            this.hubHopPresetPanel1 = new MobiFlight.UI.Panels.Config.HubHopPresetPanel();
            this.SuspendLayout();
            // 
            // DescriptionLabel
            // 
            resources.ApplyResources(this.DescriptionLabel, "DescriptionLabel");
            this.DescriptionLabel.Name = "DescriptionLabel";
            // 
            // HintLabel
            // 
            resources.ApplyResources(this.HintLabel, "HintLabel");
            this.HintLabel.Name = "HintLabel";
            // 
            // hubHopPresetPanel1
            // 
            resources.ApplyResources(this.hubHopPresetPanel1, "hubHopPresetPanel1");
            this.hubHopPresetPanel1.LVars = ((System.Collections.Generic.List<string>)(resources.GetObject("hubHopPresetPanel1.LVars")));
            this.hubHopPresetPanel1.Name = "hubHopPresetPanel1";
            this.hubHopPresetPanel1.PresetFile = "Presets\\msfs2020_hubhop_presets.json";
            this.hubHopPresetPanel1.PresetFileUser = "Presets\\msfs2020_simvars_user.cip";
            // 
            // MSFS2020CustomInputPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.HintLabel);
            this.Controls.Add(this.hubHopPresetPanel1);
            this.Controls.Add(this.DescriptionLabel);
            this.Name = "MSFS2020CustomInputPanel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label DescriptionLabel;
        private System.Windows.Forms.Label HintLabel;
        private Config.HubHopPresetPanel hubHopPresetPanel1;
    }
}
