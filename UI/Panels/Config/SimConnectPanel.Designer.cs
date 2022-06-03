
namespace MobiFlight.UI.Panels.Config
{
    partial class SimConnectPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimConnectPanel));
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.transformOptionsGroup1 = new MobiFlight.UI.Panels.Config.TransformOptionsGroup();
            this.HubHopPresetPanel = new MobiFlight.UI.Panels.Config.HubHopPresetPanel();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(4, 24);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.label2.Size = new System.Drawing.Size(898, 40);
            this.label2.TabIndex = 2;
            this.label2.Text = "Define the sim variable name that you would like to read from MSFS2020.";
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.transformOptionsGroup1);
            this.groupBox1.Controls.Add(this.HubHopPresetPanel);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(906, 815);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MSFS2020 (WASM)";
            // 
            // transformOptionsGroup1
            // 
            this.transformOptionsGroup1.AutoSize = true;
            this.transformOptionsGroup1.Dock = System.Windows.Forms.DockStyle.Top;
            this.transformOptionsGroup1.Location = new System.Drawing.Point(4, 354);
            this.transformOptionsGroup1.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.transformOptionsGroup1.Name = "transformOptionsGroup1";
            this.transformOptionsGroup1.Size = new System.Drawing.Size(898, 139);
            this.transformOptionsGroup1.TabIndex = 7;
            // 
            // HubHopPresetPanel
            // 
            this.HubHopPresetPanel.AutoSize = true;
            this.HubHopPresetPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.HubHopPresetPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.HubHopPresetPanel.Location = new System.Drawing.Point(4, 64);
            this.HubHopPresetPanel.LVars = ((System.Collections.Generic.List<string>)(resources.GetObject("HubHopPresetPanel.LVars")));
            this.HubHopPresetPanel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.HubHopPresetPanel.MinimumSize = new System.Drawing.Size(904, 0);
            this.HubHopPresetPanel.Mode = MobiFlight.UI.Panels.Config.HubHopPanelMode.Output;
            this.HubHopPresetPanel.Name = "HubHopPresetPanel";
            this.HubHopPresetPanel.PresetFile = "Presets\\msfs2020_hubhop_presets.json";
            this.HubHopPresetPanel.PresetFileUser = "Presets\\msfs2020_simvars_user.cip";
            this.HubHopPresetPanel.Size = new System.Drawing.Size(904, 290);
            this.HubHopPresetPanel.TabIndex = 8;
            // 
            // SimConnectPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "SimConnectPanel";
            this.Size = new System.Drawing.Size(906, 815);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private TransformOptionsGroup transformOptionsGroup1;
        public HubHopPresetPanel HubHopPresetPanel;
    }
}
