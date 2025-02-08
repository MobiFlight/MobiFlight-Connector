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
            this.FrontendWebView = new ThreadSafeWebView2();
            ((System.ComponentModel.ISupportInitialize)(this.FrontendWebView)).BeginInit();
            this.SuspendLayout();
            // 
            // FrontendWebView
            // 
            this.FrontendWebView.AllowExternalDrop = true;
            this.FrontendWebView.CreationProperties = null;
            this.FrontendWebView.DefaultBackgroundColor = System.Drawing.Color.White;
            this.FrontendWebView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FrontendWebView.Location = new System.Drawing.Point(0, 0);
            this.FrontendWebView.Name = "FrontendWebView";
            this.FrontendWebView.Size = new System.Drawing.Size(864, 663);
            this.FrontendWebView.TabIndex = 0;
            this.FrontendWebView.ZoomFactor = 1D;
            // 
            // FrontendPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.FrontendWebView);
            this.Name = "FrontendPanel";
            this.Size = new System.Drawing.Size(864, 663);
            ((System.ComponentModel.ISupportInitialize)(this.FrontendWebView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ThreadSafeWebView2 FrontendWebView;
    }
}
