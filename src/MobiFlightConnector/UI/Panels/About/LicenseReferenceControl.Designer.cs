namespace MobiFlight.UI.Panels.About
{
    partial class LicenseReferenceControl
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
            this.LicenseLinkLabel = new System.Windows.Forms.LinkLabel();
            this.LibraryLinkLabel = new System.Windows.Forms.LinkLabel();
            this.LibraryLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LicenseLinkLabel
            // 
            this.LicenseLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LicenseLinkLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LicenseLinkLabel.Location = new System.Drawing.Point(324, 5);
            this.LicenseLinkLabel.Name = "LicenseLinkLabel";
            this.LicenseLinkLabel.Size = new System.Drawing.Size(44, 14);
            this.LicenseLinkLabel.TabIndex = 7;
            this.LicenseLinkLabel.TabStop = true;
            this.LicenseLinkLabel.Text = "License";
            this.LicenseLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LibraryLinkLabel
            // 
            this.LibraryLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LibraryLinkLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LibraryLinkLabel.Location = new System.Drawing.Point(281, 5);
            this.LibraryLinkLabel.Name = "LibraryLinkLabel";
            this.LibraryLinkLabel.Size = new System.Drawing.Size(40, 14);
            this.LibraryLinkLabel.TabIndex = 6;
            this.LibraryLinkLabel.TabStop = true;
            this.LibraryLinkLabel.Text = "Project";
            this.LibraryLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LibraryLabel
            // 
            this.LibraryLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.LibraryLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.LibraryLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LibraryLabel.Location = new System.Drawing.Point(4, 5);
            this.LibraryLabel.Name = "LibraryLabel";
            this.LibraryLabel.Size = new System.Drawing.Size(259, 14);
            this.LibraryLabel.TabIndex = 5;
            this.LibraryLabel.Text = "CmdMessenger";
            this.LibraryLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LicenseReferenceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LicenseLinkLabel);
            this.Controls.Add(this.LibraryLinkLabel);
            this.Controls.Add(this.LibraryLabel);
            this.Name = "LicenseReferenceControl";
            this.Size = new System.Drawing.Size(383, 24);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.LinkLabel LicenseLinkLabel;
        private System.Windows.Forms.LinkLabel LibraryLinkLabel;
        private System.Windows.Forms.Label LibraryLabel;
    }
}
