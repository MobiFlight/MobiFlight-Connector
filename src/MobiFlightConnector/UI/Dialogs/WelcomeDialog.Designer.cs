using System;

namespace MobiFlight.UI.Dialogs
{
    partial class WelcomeDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WelcomeDialog));
            this.okButton = new System.Windows.Forms.Button();
            this.updateButton = new System.Windows.Forms.Button();
            this.doNotUpdateButton = new System.Windows.Forms.Button();
            this.disableBetaButton = new System.Windows.Forms.Button();
            this.titleLabel = new System.Windows.Forms.Label();
            this.panel = new System.Windows.Forms.Panel();
            this.openReleaseNotesinBrowserLabel = new System.Windows.Forms.Label();
            this.webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            this.panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.button1_Click);
            //
            // updateButton
            //
            this.updateButton.Location = new System.Drawing.Point(321, 2);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(125, 35);
            this.updateButton.TabIndex = 3;
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            //
            // doNotUpdateButton
            //
            this.doNotUpdateButton.Location = new System.Drawing.Point(452, 2);
            this.doNotUpdateButton.Name = "doNotUpdateButton";
            this.doNotUpdateButton.Size = new System.Drawing.Size(145, 35);
            this.doNotUpdateButton.TabIndex = 4;
            this.doNotUpdateButton.UseVisualStyleBackColor = true;
            this.doNotUpdateButton.Click += new System.EventHandler(this.doNotUpdateButton_Click);
            //
            // disableBetaButton
            //
            this.disableBetaButton.Location = new System.Drawing.Point(600, 2);
            this.disableBetaButton.Name = "disableBetaButton";
            this.disableBetaButton.Size = new System.Drawing.Size(185, 35);
            this.disableBetaButton.TabIndex = 5;
            this.disableBetaButton.UseVisualStyleBackColor = true;
            this.disableBetaButton.Click += new System.EventHandler(this.disableBetaButton_Click);
            // 
            // titleLabel
            // 
            resources.ApplyResources(this.titleLabel, "titleLabel");
            this.titleLabel.Name = "titleLabel";
            // 
            // panel
            // 
            this.panel.Controls.Add(this.openReleaseNotesinBrowserLabel);
            this.panel.Controls.Add(this.okButton);
            this.panel.Controls.Add(this.updateButton);
            this.panel.Controls.Add(this.doNotUpdateButton);
            this.panel.Controls.Add(this.disableBetaButton);
            resources.ApplyResources(this.panel, "panel");
            this.panel.Name = "panel";
            // 
            // openReleaseNotesinBrowserLabel
            // 
            resources.ApplyResources(this.openReleaseNotesinBrowserLabel, "openReleaseNotesinBrowserLabel");
            this.openReleaseNotesinBrowserLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.openReleaseNotesinBrowserLabel.Name = "openReleaseNotesinBrowserLabel";
            this.openReleaseNotesinBrowserLabel.Click += new System.EventHandler(this.transparentOverlay1_Click);
            // 
            // webView
            // 
            this.webView.AllowExternalDrop = true;
            this.webView.CreationProperties = null;
            this.webView.DefaultBackgroundColor = System.Drawing.Color.White;
            resources.ApplyResources(this.webView, "webView");
            this.webView.Name = "webView";
            this.webView.ZoomFactor = 1D;
            // 
            // WelcomeDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.webView);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.panel);
            this.Name = "WelcomeDialog";
            this.Load += new System.EventHandler(this.WelcomeDialog_Load);
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.Button doNotUpdateButton;
        private System.Windows.Forms.Button disableBetaButton;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Panel panel;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private System.Windows.Forms.Label openReleaseNotesinBrowserLabel;
    }
}
