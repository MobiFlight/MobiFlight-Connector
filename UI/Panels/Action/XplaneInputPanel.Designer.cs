namespace MobiFlight.UI.Panels.Action
{
    partial class XplaneInputPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XplaneInputPanel));
            this.hubHopPresetPanel1 = new MobiFlight.UI.Panels.Config.HubHopPresetPanel();
            this.SuspendLayout();
            // 
            // hubHopPresetPanel1
            // 
            this.hubHopPresetPanel1.AutoSize = true;
            this.hubHopPresetPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hubHopPresetPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.hubHopPresetPanel1.FlightSimType = MobiFlight.FlightSimType.NONE;
            this.hubHopPresetPanel1.Location = new System.Drawing.Point(0, 0);
            this.hubHopPresetPanel1.LVars = ((System.Collections.Generic.List<string>)(resources.GetObject("hubHopPresetPanel1.LVars")));
            this.hubHopPresetPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.hubHopPresetPanel1.MinimumSize = new System.Drawing.Size(603, 0);
            this.hubHopPresetPanel1.Mode = MobiFlight.UI.Panels.Config.HubHopPanelMode.Output;
            this.hubHopPresetPanel1.Name = "hubHopPresetPanel1";
            this.hubHopPresetPanel1.PresetFile = "Presets\\msfs2020_hubhop_presets.json";
            this.hubHopPresetPanel1.PresetFileUser = "Presets\\msfs2020_simvars_user.cip";
            this.hubHopPresetPanel1.Size = new System.Drawing.Size(603, 189);
            this.hubHopPresetPanel1.TabIndex = 31;
            // 
            // XplaneInputPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.hubHopPresetPanel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(600, 200);
            this.Name = "XplaneInputPanel";
            this.Size = new System.Drawing.Size(600, 200);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Config.HubHopPresetPanel hubHopPresetPanel1;
    }
}
