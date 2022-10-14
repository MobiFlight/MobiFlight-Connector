using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace MobiFlight.UI.Dialogs
{
    public partial class WelcomeDialog : Form
    {
        public event EventHandler<EventArgs> ReleaseNotesClicked;

        public string WebsiteUrl {
            get { return this.webView.Source.ToString(); }
            set { 
                this.webView.Source = new System.Uri(value, System.UriKind.Absolute);
            } 
        }

        public WelcomeDialog()
        {
            InitializeComponent();
            WebsiteUrl = "https://www.mobiflight.com/en/release-notes.html";
            this.webView.NavigationCompleted += WebView21_NavigationCompleted;
        }

        private async void WebView21_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            await webView.ExecuteScriptAsync($"" +
                $"document.getElementById('repository-container-header').remove();" +
                $"document.getElementsByClassName('js-header-wrapper')[0].remove();" +
                $"document.getElementsByClassName('footer')[0].remove();");
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
    }
}
