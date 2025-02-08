using Microsoft.Web.WebView2.WinForms;
using System;

namespace MobiFlight
{
    public class ThreadSafeWebView2 : WebView2
    {
        public void PostWebMessageAsJsonThreadSafe(string jsonMessage)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => CoreWebView2.PostWebMessageAsJson(jsonMessage)));
            }
            else
            {
                CoreWebView2.PostWebMessageAsJson(jsonMessage);
            }
        }
    }
}
