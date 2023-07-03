namespace MobiFlight.UI.Dialogs
{
    partial class AboutForm
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

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.licenseReferenceControl5 = new MobiFlight.UI.Panels.About.LicenseReferenceControl();
            this.licenseReferenceControl2 = new MobiFlight.UI.Panels.About.LicenseReferenceControl();
            this.licenseReferenceControl4 = new MobiFlight.UI.Panels.About.LicenseReferenceControl();
            this.licenseReferenceControl3 = new MobiFlight.UI.Panels.About.LicenseReferenceControl();
            this.licenseReferenceControl1 = new MobiFlight.UI.Panels.About.LicenseReferenceControl();
            this.label5 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.licenseReferenceControl6 = new MobiFlight.UI.Panels.About.LicenseReferenceControl();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.linkLabel3);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.linkLabel2);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.linkLabel1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // linkLabel3
            // 
            resources.ApplyResources(this.linkLabel3, "linkLabel3");
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.TabStop = true;
            this.linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // linkLabel2
            // 
            resources.ApplyResources(this.linkLabel2, "linkLabel2");
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.TabStop = true;
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // linkLabel1
            // 
            resources.ApplyResources(this.linkLabel1, "linkLabel1");
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.TabStop = true;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.licenseReferenceControl6);
            this.panel2.Controls.Add(this.licenseReferenceControl5);
            this.panel2.Controls.Add(this.licenseReferenceControl2);
            this.panel2.Controls.Add(this.licenseReferenceControl4);
            this.panel2.Controls.Add(this.licenseReferenceControl3);
            this.panel2.Controls.Add(this.licenseReferenceControl1);
            this.panel2.Controls.Add(this.label5);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // licenseReferenceControl5
            // 
            resources.ApplyResources(this.licenseReferenceControl5, "licenseReferenceControl5");
            this.licenseReferenceControl5.Library = "X-Plane Connector";
            this.licenseReferenceControl5.LibraryLink = "https://www.nuget.org/packages/XPlaneConnector/1.3.0";
            this.licenseReferenceControl5.LicenseLink = "https://www.nuget.org/packages/XPlaneConnector/1.3.0/license";
            this.licenseReferenceControl5.Name = "licenseReferenceControl5";
            // 
            // licenseReferenceControl2
            // 
            resources.ApplyResources(this.licenseReferenceControl2, "licenseReferenceControl2");
            this.licenseReferenceControl2.Library = "SharpDX";
            this.licenseReferenceControl2.LibraryLink = "https://www.nuget.org/packages/SharpDX/";
            this.licenseReferenceControl2.LicenseLink = "http://sharpdx.org/License.txt";
            this.licenseReferenceControl2.Name = "licenseReferenceControl2";
            // 
            // licenseReferenceControl4
            // 
            resources.ApplyResources(this.licenseReferenceControl4, "licenseReferenceControl4");
            this.licenseReferenceControl4.Library = "NewtonSoft JSON";
            this.licenseReferenceControl4.LibraryLink = "https://www.nuget.org/packages/Newtonsoft.Json/";
            this.licenseReferenceControl4.LicenseLink = "https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md";
            this.licenseReferenceControl4.Name = "licenseReferenceControl4";
            // 
            // licenseReferenceControl3
            // 
            resources.ApplyResources(this.licenseReferenceControl3, "licenseReferenceControl3");
            this.licenseReferenceControl3.Library = "FSUIPC Client DLL";
            this.licenseReferenceControl3.LibraryLink = "https://www.nuget.org/packages/FSUIPCClientDLL/3.2.19";
            this.licenseReferenceControl3.LicenseLink = "https://www.nuget.org/packages/FSUIPCClientDLL/3.2.19/license";
            this.licenseReferenceControl3.Name = "licenseReferenceControl3";
            // 
            // licenseReferenceControl1
            // 
            resources.ApplyResources(this.licenseReferenceControl1, "licenseReferenceControl1");
            this.licenseReferenceControl1.Library = "CmdMessenger";
            this.licenseReferenceControl1.LibraryLink = "https://github.com/MobiFlight/Arduino-CmdMessenger/";
            this.licenseReferenceControl1.LicenseLink = "https://github.com/MobiFlight/Arduino-CmdMessenger/blob/master/LICENSE.md";
            this.licenseReferenceControl1.Name = "licenseReferenceControl1";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.button1);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // licenseReferenceControl6
            // 
            resources.ApplyResources(this.licenseReferenceControl6, "licenseReferenceControl6");
            this.licenseReferenceControl6.Library = "MidiSlicer";
            this.licenseReferenceControl6.LibraryLink = "https://github.com/codewitch-honey-crisis/MidiSlicer";
            this.licenseReferenceControl6.LicenseLink = "https://www.codeproject.com/Articles/5272315/Midi-A-Windows-MIDI-Library-in-Cshar" +
    "p";
            this.licenseReferenceControl6.Name = "licenseReferenceControl6";
            // 
            // AboutForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Name = "AboutForm";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private Panels.About.LicenseReferenceControl licenseReferenceControl1;
        private Panels.About.LicenseReferenceControl licenseReferenceControl2;
        private Panels.About.LicenseReferenceControl licenseReferenceControl3;
        private Panels.About.LicenseReferenceControl licenseReferenceControl4;
        private Panels.About.LicenseReferenceControl licenseReferenceControl5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel3;
        private Panels.About.LicenseReferenceControl licenseReferenceControl6;
    }
}