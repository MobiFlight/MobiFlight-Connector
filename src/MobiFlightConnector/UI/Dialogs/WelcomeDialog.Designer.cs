using System;

namespace MobiFlight.UI.Dialogs
{
    partial class WelcomeDialog
    {
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
            panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(webView)).BeginInit();
            this.SuspendLayout();

            // okButton
            resources.ApplyResources(okButton, "okButton");
            okButton.Click += new System.EventHandler(button1_Click);

            // updateButton
            updateButton.Click += new System.EventHandler(updateButton_Click);

            // dontUpdateNowLink
            dontUpdateNowLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(dontUpdateNowLink_LinkClicked);

            // disableBetaButton
            disableBetaButton.Click += new System.EventHandler(disableBetaButton_Click);

            // titleLabel
            resources.ApplyResources(titleLabel, "titleLabel");

            // panel
            panel.Controls.Add(openReleaseNotesLink);
            panel.Controls.Add(okButton);
            panel.Controls.Add(dontUpdateNowLink);
            panel.Controls.Add(disableBetaButton);
            panel.Controls.Add(updateButton);
            resources.ApplyResources(panel, "panel");

            // openReleaseNotesLink
            openReleaseNotesLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(openReleaseNotesLink_LinkClicked);

            // webView
            resources.ApplyResources(webView, "webView");

            // WelcomeDialog
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(webView);
            Controls.Add(titleLabel);
            Controls.Add(panel);
            this.AcceptButton = updateButton;
            this.Name = "WelcomeDialog";
            this.Load += new System.EventHandler(this.WelcomeDialog_Load);
            panel.ResumeLayout(false);
            panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(webView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton = new System.Windows.Forms.Button
        {
            Name = "okButton",
            UseVisualStyleBackColor = true
        };

        private System.Windows.Forms.Button updateButton = new System.Windows.Forms.Button
        {
            Dock = System.Windows.Forms.DockStyle.Right,
            Name = "updateButton",
            Size = new System.Drawing.Size(125, 35),
            TabIndex = 0,
            UseVisualStyleBackColor = true
        };

        private System.Windows.Forms.LinkLabel dontUpdateNowLink = new System.Windows.Forms.LinkLabel
        {
            Dock = System.Windows.Forms.DockStyle.Right,
            Name = "dontUpdateNowLink",
            Padding = new System.Windows.Forms.Padding(0, 0, 10, 0),
            Size = new System.Drawing.Size(180, 35),
            TabIndex = 2,
            TabStop = true,
            Text = "Don't update now",
            TextAlign = System.Drawing.ContentAlignment.MiddleRight
        };

        private System.Windows.Forms.Button disableBetaButton = new System.Windows.Forms.Button
        {
            Dock = System.Windows.Forms.DockStyle.Right,
            Name = "disableBetaButton",
            Size = new System.Drawing.Size(185, 35),
            TabIndex = 3,
            UseVisualStyleBackColor = true
        };

        private System.Windows.Forms.Label titleLabel = new System.Windows.Forms.Label
        {
            Name = "titleLabel"
        };

        private System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel
        {
            Name = "panel",
            Padding = new System.Windows.Forms.Padding(0, 4, 0, 0)
        };

        private Microsoft.Web.WebView2.WinForms.WebView2 webView = new Microsoft.Web.WebView2.WinForms.WebView2
        {
            AllowExternalDrop = true,
            CreationProperties = null,
            DefaultBackgroundColor = System.Drawing.Color.White,
            Name = "webView",
            ZoomFactor = 1D
        };

        private System.Windows.Forms.LinkLabel openReleaseNotesLink = new System.Windows.Forms.LinkLabel
        {
            AutoSize = true,
            Location = new System.Drawing.Point(6, 13),
            Name = "openReleaseNotesLink",
            TabIndex = 1,
            TabStop = true,
            Text = "Open Release Notes in browser"
        };

    }
}
