using System;
using System.Windows.Forms;

namespace MobiFlight.UI.Dialogs
{
    public enum WelcomeDialogMode
    {
        BeforeUpdate,
        FirstStart
    }
    public partial class WelcomeDialog : Form
    {
        public event EventHandler<EventArgs> ReleaseNotesClicked;
        public event EventHandler<EventArgs> DisableBetaClicked;

        public string WebsiteUrl
        {
            get { return this.webView.Source.ToString(); }
            set
            {
                this.webView.Source = new System.Uri(value, System.UriKind.Absolute);
            }
        }

        private WelcomeDialogMode _mode;
        public WelcomeDialogMode Mode
        {
            get { return _mode; }
            set
            {
                if (_mode == value) return;
                _mode = value;
                okButton.Visible = _mode == WelcomeDialogMode.FirstStart;
                updateButton.Visible = _mode == WelcomeDialogMode.BeforeUpdate;
                disableBetaButton.Visible = _mode == WelcomeDialogMode.BeforeUpdate;
                dontUpdateNowLabel.Visible = _mode == WelcomeDialogMode.BeforeUpdate;
            }
        }

        public bool ShowDisableBetaButton
        {
            get { return disableBetaButton.Visible; }
            set { disableBetaButton.Visible = value; }
        }

        public WelcomeDialog()
        {
            InitializeComponent();

            // Default Setting
            Mode = WelcomeDialogMode.FirstStart;
            this.webView.NavigationCompleted += WebView21_NavigationCompleted;
        }

        private async void WebView21_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            await webView.ExecuteScriptAsync($"" +
                $"document.getElementById('repository-container-header').remove();" +
                $"document.getElementsByClassName('js-header-wrapper')[0].remove();" +
                $"document.getElementsByClassName('footer')[0].remove();" +
                @"document.getElementsByClassName('Box-footer')[0].childNodes.forEach((item, i)=>{{ if (i>0) item.remove(); }});"
                );
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void WelcomeDialog_Load(object sender, EventArgs e)
        {

        }

        private void transparentOverlay1_Click(object sender, EventArgs e)
        {
            ReleaseNotesClicked?.Invoke(this, EventArgs.Empty);
        }

        private void disableBetaButton_Click(object sender, EventArgs e)
        {
            DisableBetaClicked?.Invoke(this, EventArgs.Empty);
            DialogResult = DialogResult.No;
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        private void dontUpdateNowLabel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}