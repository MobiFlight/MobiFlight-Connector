namespace MobiFlight.UI.Panels.Config
{
    partial class XplaneDataRefPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XplaneDataRefPanel));
            this.transformOptionsGroup1 = new MobiFlight.UI.Panels.Config.TransformOptionsGroup();
            this.hubHopPresetPanel1 = new MobiFlight.UI.Panels.Config.HubHopPresetPanel();
            this.SuspendLayout();
            // 
            // transformOptionsGroup1
            // 
            this.transformOptionsGroup1.Dock = System.Windows.Forms.DockStyle.Top;
            this.transformOptionsGroup1.Location = new System.Drawing.Point(0, 189);
            this.transformOptionsGroup1.Name = "transformOptionsGroup1";
            this.transformOptionsGroup1.Size = new System.Drawing.Size(635, 94);
            this.transformOptionsGroup1.TabIndex = 1;
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
            this.hubHopPresetPanel1.Size = new System.Drawing.Size(635, 189);
            this.hubHopPresetPanel1.TabIndex = 2;
            // 
            // XplaneDataRefPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.transformOptionsGroup1);
            this.Controls.Add(this.hubHopPresetPanel1);
            this.Name = "XplaneDataRefPanel";
            this.Size = new System.Drawing.Size(635, 387);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private TransformOptionsGroup transformOptionsGroup1;
        private HubHopPresetPanel hubHopPresetPanel1;
    }
}
