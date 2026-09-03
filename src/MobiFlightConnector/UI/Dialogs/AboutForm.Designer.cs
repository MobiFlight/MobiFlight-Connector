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
            label1 = new System.Windows.Forms.Label();
            button1 = new System.Windows.Forms.Button();
            panel1 = new System.Windows.Forms.Panel();
            linkLabel3 = new System.Windows.Forms.LinkLabel();
            label4 = new System.Windows.Forms.Label();
            linkLabel2 = new System.Windows.Forms.LinkLabel();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            linkLabel1 = new System.Windows.Forms.LinkLabel();
            panel2 = new System.Windows.Forms.Panel();
            licenseReferenceControl9 = new MobiFlight.UI.Panels.About.LicenseReferenceControl();
            licenseReferenceControl10 = new MobiFlight.UI.Panels.About.LicenseReferenceControl();
            licenseReferenceControl8 = new MobiFlight.UI.Panels.About.LicenseReferenceControl();
            licenseReferenceControl6 = new MobiFlight.UI.Panels.About.LicenseReferenceControl();
            licenseReferenceControl5 = new MobiFlight.UI.Panels.About.LicenseReferenceControl();
            licenseReferenceControl2 = new MobiFlight.UI.Panels.About.LicenseReferenceControl();
            licenseReferenceControl7 = new MobiFlight.UI.Panels.About.LicenseReferenceControl();
            licenseReferenceControl4 = new MobiFlight.UI.Panels.About.LicenseReferenceControl();
            licenseReferenceControl3 = new MobiFlight.UI.Panels.About.LicenseReferenceControl();
            licenseReferenceControl1 = new MobiFlight.UI.Panels.About.LicenseReferenceControl();
            label5 = new System.Windows.Forms.Label();
            panel3 = new System.Windows.Forms.Panel();
            label6 = new System.Windows.Forms.Label();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // button1
            // 
            resources.ApplyResources(button1, "button1");
            button1.Name = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(linkLabel3);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(linkLabel2);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(linkLabel1);
            resources.ApplyResources(panel1, "panel1");
            panel1.Name = "panel1";
            // 
            // linkLabel3
            // 
            resources.ApplyResources(linkLabel3, "linkLabel3");
            linkLabel3.Name = "linkLabel3";
            linkLabel3.TabStop = true;
            linkLabel3.LinkClicked += linkLabel1_LinkClicked;
            // 
            // label4
            // 
            resources.ApplyResources(label4, "label4");
            label4.Name = "label4";
            // 
            // linkLabel2
            // 
            resources.ApplyResources(linkLabel2, "linkLabel2");
            linkLabel2.Name = "linkLabel2";
            linkLabel2.TabStop = true;
            linkLabel2.LinkClicked += linkLabel1_LinkClicked;
            // 
            // label3
            // 
            resources.ApplyResources(label3, "label3");
            label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // linkLabel1
            // 
            resources.ApplyResources(linkLabel1, "linkLabel1");
            linkLabel1.Name = "linkLabel1";
            linkLabel1.TabStop = true;
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // panel2
            // 
            panel2.Controls.Add(label6);
            panel2.Controls.Add(licenseReferenceControl9);
            panel2.Controls.Add(licenseReferenceControl10);
            panel2.Controls.Add(licenseReferenceControl8);
            panel2.Controls.Add(licenseReferenceControl6);
            panel2.Controls.Add(licenseReferenceControl5);
            panel2.Controls.Add(licenseReferenceControl2);
            panel2.Controls.Add(licenseReferenceControl7);
            panel2.Controls.Add(licenseReferenceControl4);
            panel2.Controls.Add(licenseReferenceControl3);
            panel2.Controls.Add(licenseReferenceControl1);
            panel2.Controls.Add(label5);
            resources.ApplyResources(panel2, "panel2");
            panel2.Name = "panel2";
            // 
            // licenseReferenceControl9
            // 
            resources.ApplyResources(licenseReferenceControl9, "licenseReferenceControl9");
            licenseReferenceControl9.Library = "Serilog";
            licenseReferenceControl9.LibraryLink = "https://github.com/serilog/serilog";
            licenseReferenceControl9.LicenseLink = "https://github.com/serilog/serilog/blob/dev/LICENSE";
            licenseReferenceControl9.Name = "licenseReferenceControl9";
            // 
            // licenseReferenceControl10
            // 
            resources.ApplyResources(licenseReferenceControl10, "licenseReferenceControl10");
            licenseReferenceControl10.Library = "websocket-sharp";
            licenseReferenceControl10.LibraryLink = "https://github.com/PingmanTools/websocket-sharp/";
            licenseReferenceControl10.LicenseLink = "https://github.com/PingmanTools/websocket-sharp/blob/master/LICENSE.txt";
            licenseReferenceControl10.Name = "licenseReferenceControl10";
            // 
            // licenseReferenceControl8
            // 
            resources.ApplyResources(licenseReferenceControl8, "licenseReferenceControl8");
            licenseReferenceControl8.Library = "HidSharp";
            licenseReferenceControl8.LibraryLink = "https://www.nuget.org/packages/HidSharp";
            licenseReferenceControl8.LicenseLink = "https://www.zer7.com/files/oss/hidsharp/LICENSE.txt";
            licenseReferenceControl8.Name = "licenseReferenceControl8";
            // 
            // licenseReferenceControl6
            // 
            resources.ApplyResources(licenseReferenceControl6, "licenseReferenceControl6");
            licenseReferenceControl6.Library = "MidiSlicer";
            licenseReferenceControl6.LibraryLink = "https://github.com/codewitch-honey-crisis/MidiSlicer";
            licenseReferenceControl6.LicenseLink = "https://www.codeproject.com/Articles/5272315/Midi-A-Windows-MIDI-Library-in-Csharp";
            licenseReferenceControl6.Name = "licenseReferenceControl6";
            // 
            // licenseReferenceControl5
            // 
            resources.ApplyResources(licenseReferenceControl5, "licenseReferenceControl5");
            licenseReferenceControl5.Library = "X-Plane Connector";
            licenseReferenceControl5.LibraryLink = "https://www.nuget.org/packages/XPlaneConnector/1.3.0";
            licenseReferenceControl5.LicenseLink = "https://www.nuget.org/packages/XPlaneConnector/1.3.0/license";
            licenseReferenceControl5.Name = "licenseReferenceControl5";
            // 
            // licenseReferenceControl2
            // 
            resources.ApplyResources(licenseReferenceControl2, "licenseReferenceControl2");
            licenseReferenceControl2.Library = "SharpDX";
            licenseReferenceControl2.LibraryLink = "https://www.nuget.org/packages/SharpDX/";
            licenseReferenceControl2.LicenseLink = "https://github.com/sharpdx/SharpDX/blob/master/LICENSE";
            licenseReferenceControl2.Name = "licenseReferenceControl2";
            // 
            // licenseReferenceControl7
            // 
            resources.ApplyResources(licenseReferenceControl7, "licenseReferenceControl7");
            licenseReferenceControl7.Library = "NewtonSoft JSON.NET Schema";
            licenseReferenceControl7.LibraryLink = "https://www.nuget.org/packages/Newtonsoft.Json.Schema";
            licenseReferenceControl7.LicenseLink = "https://www.nuget.org/packages/Newtonsoft.Json.Schema/3.0.15/License";
            licenseReferenceControl7.Name = "licenseReferenceControl7";
            // 
            // licenseReferenceControl4
            // 
            resources.ApplyResources(licenseReferenceControl4, "licenseReferenceControl4");
            licenseReferenceControl4.Library = "NewtonSoft JSON";
            licenseReferenceControl4.LibraryLink = "https://www.nuget.org/packages/Newtonsoft.Json/";
            licenseReferenceControl4.LicenseLink = "https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md";
            licenseReferenceControl4.Name = "licenseReferenceControl4";
            // 
            // licenseReferenceControl3
            // 
            resources.ApplyResources(licenseReferenceControl3, "licenseReferenceControl3");
            licenseReferenceControl3.Library = "FSUIPC Client DLL";
            licenseReferenceControl3.LibraryLink = "https://www.nuget.org/packages/FSUIPCClientDLL/3.2.19";
            licenseReferenceControl3.LicenseLink = "https://www.nuget.org/packages/FSUIPCClientDLL/3.2.19/license";
            licenseReferenceControl3.Name = "licenseReferenceControl3";
            // 
            // licenseReferenceControl1
            // 
            resources.ApplyResources(licenseReferenceControl1, "licenseReferenceControl1");
            licenseReferenceControl1.Library = "CmdMessenger";
            licenseReferenceControl1.LibraryLink = "https://github.com/MobiFlight/Arduino-CmdMessenger/";
            licenseReferenceControl1.LicenseLink = "https://github.com/MobiFlight/Arduino-CmdMessenger/blob/master/LICENSE.md";
            licenseReferenceControl1.Name = "licenseReferenceControl1";
            // 
            // label5
            // 
            resources.ApplyResources(label5, "label5");
            label5.Name = "label5";
            // 
            // panel3
            // 
            panel3.Controls.Add(button1);
            resources.ApplyResources(panel3, "panel3");
            panel3.Name = "panel3";
            // 
            // label6
            // 
            resources.ApplyResources(label6, "label6");
            label6.Name = "label6";
            // 
            // AboutForm
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(label1);
            Name = "AboutForm";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            ResumeLayout(false);

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
        private Panels.About.LicenseReferenceControl licenseReferenceControl7;
        private Panels.About.LicenseReferenceControl licenseReferenceControl8;
        private Panels.About.LicenseReferenceControl licenseReferenceControl10;
        private Panels.About.LicenseReferenceControl licenseReferenceControl9;
        private System.Windows.Forms.Label label6;
    }
}