using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace MobiFlight.UI.Dialogs
{
    public partial class WelcomeDialog : Form
    {
        public event EventHandler<EventArgs> ReleaseNotesClicked;
        public event EventHandler<EventArgs> DisableBetaUpdatesClicked;

        public string WebsiteUrl {
            get { return this.webView.Source.ToString(); }
            set { 
                this.webView.Source = new System.Uri(value, System.UriKind.Absolute);
            } 
        }

        public string ReleaseNotes
        {
            get; set;
        }

        public bool ShowUpdateButtons
        {
            get { return updateButton.Visible; }
            set
            {
                updateButton.Visible = value;
                doNotUpdateButton.Visible = value;
                okButton.Visible = !value;
            }
        }

        public bool ShowDisableBetaButton
        {
            get { return disableBetaButton.Visible; }
            set
            {
                disableBetaButton.Visible = value;
            }
        }

        public WelcomeDialog()
        {
            InitializeComponent();
            updateButton.Text = "Update now";
            doNotUpdateButton.Text = "Next time";
            disableBetaButton.Text = "Disable BETA updates";
            ShowUpdateButtons = false;
            ShowDisableBetaButton = false;
            WebsiteUrl = "about:blank";
            this.webView.NavigationCompleted += WebView21_NavigationCompleted;
        }

        private async void WebView21_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            if (!e.IsSuccess)
            {
                webView.NavigateToString("<html><body style='font-family:Segoe UI;padding:12px'>Release notes are currently unavailable. You can open them in your browser using the button below.</body></html>");
                return;
            }

            await webView.ExecuteScriptAsync(@"(() => {
                document.getElementById('repository-container-header')?.remove();
                document.getElementsByClassName('js-header-wrapper')[0]?.remove();
                document.getElementsByClassName('footer')[0]?.remove();
                document.getElementsByClassName('Box-footer')[0]?.childNodes.forEach((item, i) => { if (i > 0) item.remove(); });
            })();");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        private void doNotUpdateButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
        }

        private void disableBetaButton_Click(object sender, EventArgs e)
        {
            DisableBetaUpdatesClicked?.Invoke(this, EventArgs.Empty);
            DialogResult = DialogResult.No;
        }

        private void WelcomeDialog_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ReleaseNotes))
            {
                webView.NavigateToString(
                    "<html><body style='font-family:Segoe UI;font-size:10pt;white-space:pre-wrap;padding:12px'>" +
                    WebUtility.HtmlEncode(ReleaseNotes).Replace("\r\n", "\n") +
                    "</body></html>");
            }
        }

        private void transparentOverlay1_Click(object sender, EventArgs e)
        {
            ReleaseNotesClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
