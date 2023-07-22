namespace MobiFlight.UI.Panels.Settings.Device
{
    partial class MFCustomDevicePanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MFCustomDevicePanel));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.mfCustomDevicePanelPin1 = new MobiFlight.UI.Panels.Settings.Device.MFCustomDevicePanelPin();
            this.mfCustomDevicePanelPin2 = new MobiFlight.UI.Panels.Settings.Device.MFCustomDevicePanelPin();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.mfCustomDevicePanelPin2);
            this.groupBox1.Controls.Add(this.mfCustomDevicePanelPin1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox1);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            this.textBox1.TextChanged += new System.EventHandler(this.value_Changed);
            // 
            // mfCustomDevicePanelPin1
            // 
            resources.ApplyResources(this.mfCustomDevicePanelPin1, "mfCustomDevicePanelPin1");
            this.mfCustomDevicePanelPin1.Name = "mfCustomDevicePanelPin1";
            // 
            // mfCustomDevicePanelPin2
            // 
            resources.ApplyResources(this.mfCustomDevicePanelPin2, "mfCustomDevicePanelPin2");
            this.mfCustomDevicePanelPin2.Name = "mfCustomDevicePanelPin2";
            // 
            // MFCustomDevicePanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MFCustomDevicePanel";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox1;
        private MFCustomDevicePanelPin mfCustomDevicePanelPin2;
        private MFCustomDevicePanelPin mfCustomDevicePanelPin1;
    }
}
