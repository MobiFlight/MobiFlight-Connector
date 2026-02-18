using MobiFlight.WebView;

namespace MobiFlight.UI.Panels
{
    partial class FrontendPanel
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
            this.FrontendWebView = new MobiFlight.WebView.ThreadSafeWebView2();
            this.UserAuthenticationWebView = new MobiFlight.WebView.ThreadSafeWebView2();
            ((System.ComponentModel.ISupportInitialize)(this.FrontendWebView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.UserAuthenticationWebView)).BeginInit();
            this.SuspendLayout();
            // 
            // FrontendWebView
            // 
            this.FrontendWebView.AllowExternalDrop = true;
            this.FrontendWebView.CreationProperties = null;
            this.FrontendWebView.DefaultBackgroundColor = System.Drawing.Color.White;
            this.FrontendWebView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FrontendWebView.Location = new System.Drawing.Point(0, 0);
            this.FrontendWebView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.FrontendWebView.Name = "FrontendWebView";
            this.FrontendWebView.Size = new System.Drawing.Size(1296, 1020);
            this.FrontendWebView.TabIndex = 0;
            this.FrontendWebView.ZoomFactor = 1D;
            // 
            // UserAuthenticationWebView
            // 
            this.UserAuthenticationWebView.AllowExternalDrop = true;
            this.UserAuthenticationWebView.CreationProperties = null;
            this.UserAuthenticationWebView.DefaultBackgroundColor = System.Drawing.Color.White;
            this.UserAuthenticationWebView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UserAuthenticationWebView.Location = new System.Drawing.Point(0, 0);
            this.UserAuthenticationWebView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.UserAuthenticationWebView.Name = "UserAuthenticationWebView";
            this.UserAuthenticationWebView.Size = new System.Drawing.Size(1296, 1020);
            this.UserAuthenticationWebView.TabIndex = 1;
            this.UserAuthenticationWebView.Visible = false;
            this.UserAuthenticationWebView.ZoomFactor = 1D;
            // 
            // FrontendPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.UserAuthenticationWebView);
            this.Controls.Add(this.FrontendWebView);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "FrontendPanel";
            this.Size = new System.Drawing.Size(1296, 1020);
            ((System.ComponentModel.ISupportInitialize)(this.FrontendWebView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.UserAuthenticationWebView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ThreadSafeWebView2 FrontendWebView;
        private ThreadSafeWebView2 UserAuthenticationWebView;
    }
}
