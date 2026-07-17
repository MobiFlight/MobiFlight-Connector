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
            this.dontUpdateNowLink = new System.Windows.Forms.LinkLabel();
            this.disableBetaButton = new System.Windows.Forms.Button();
            this.titleLabel = new System.Windows.Forms.Label();
            this.panel = new System.Windows.Forms.Panel();
            this.openReleaseNotesLink = new System.Windows.Forms.LinkLabel();
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
            this.updateButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(125, 35);
            this.updateButton.TabIndex = 4;
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            //
            // dontUpdateNowLink
            //
            this.dontUpdateNowLink.ActiveLinkColor = System.Drawing.Color.Black;
            this.dontUpdateNowLink.LinkColor = System.Drawing.Color.Black;
            this.dontUpdateNowLink.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.dontUpdateNowLink.Dock = System.Windows.Forms.DockStyle.Right;
            this.dontUpdateNowLink.Name = "dontUpdateNowLink";
            this.dontUpdateNowLink.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.dontUpdateNowLink.Size = new System.Drawing.Size(180, 35);
            this.dontUpdateNowLink.TabIndex = 2;
            this.dontUpdateNowLink.TabStop = true;
            this.dontUpdateNowLink.Text = "Don't update now";
            this.dontUpdateNowLink.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.dontUpdateNowLink.VisitedLinkColor = System.Drawing.Color.Black;
            this.dontUpdateNowLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.dontUpdateNowLink_LinkClicked);
            //
            // disableBetaButton
            //
            this.disableBetaButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.disableBetaButton.Name = "disableBetaButton";
            this.disableBetaButton.Size = new System.Drawing.Size(185, 35);
            this.disableBetaButton.TabIndex = 3;
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
            this.panel.Controls.Add(this.openReleaseNotesLink);
            this.panel.Controls.Add(this.okButton);
            this.panel.Controls.Add(this.dontUpdateNowLink);
            this.panel.Controls.Add(this.disableBetaButton);
            this.panel.Controls.Add(this.updateButton);
            resources.ApplyResources(this.panel, "panel");
            this.panel.Name = "panel";
            // 
            // openReleaseNotesLink
            //
            this.openReleaseNotesLink.AutoSize = true;
            this.openReleaseNotesLink.ActiveLinkColor = System.Drawing.Color.Black;
            this.openReleaseNotesLink.LinkColor = System.Drawing.Color.Black;
            this.openReleaseNotesLink.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.openReleaseNotesLink.Location = new System.Drawing.Point(6, 13);
            this.openReleaseNotesLink.Name = "openReleaseNotesLink";
            this.openReleaseNotesLink.TabIndex = 1;
            this.openReleaseNotesLink.TabStop = true;
            this.openReleaseNotesLink.Text = "Open Release Notes in browser";
            this.openReleaseNotesLink.VisitedLinkColor = System.Drawing.Color.Black;
            this.openReleaseNotesLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.openReleaseNotesLink_LinkClicked);
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
        private System.Windows.Forms.LinkLabel dontUpdateNowLink;
        private System.Windows.Forms.Button disableBetaButton;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Panel panel;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView;
        private System.Windows.Forms.LinkLabel openReleaseNotesLink;
    }
}
