using MobiFlight.RestWebSocketApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MobiFlight.UI.Panels.Settings
{
    public partial class RestApiPanel : UserControl
    {

        private RestApiServerSettings settings;

        public RestApiPanel()
        {
            InitializeComponent();
        }

        public void LoadSettings()
        {
            settings = RestApiServerSettings.Load();
            RestServerPort.Text = settings.restPort;
            WebsocketServerPort.Text = settings.websocketPort;

            RestServerInterfaces.Items.Add("127.0.0.1");
            RestServerInterfaces.Items.Add("All Interfaces");

            WebsocketServerInterfaces.Items.Add("127.0.0.1");
            WebsocketServerInterfaces.Items.Add("All Interfaces");


            if (settings.websocketServerInterface == "127.0.0.1")
            {
                WebsocketServerInterfaces.SelectedIndex = 0;
            } else
            {
                WebsocketServerInterfaces.SelectedIndex = 1;
            }


            if (settings.restServerInterface == "127.0.0.1")
            {
                RestServerInterfaces.SelectedIndex = 0;
            }
            else
            {
                RestServerInterfaces.SelectedIndex = 1;
            }
        }


        public void SaveSettings()
        {
            settings.restPort = RestServerPort.Text;
            settings.websocketPort = WebsocketServerPort.Text;
            settings.websocketServerInterface = WebsocketServerInterfaces.SelectedIndex == 0 ? "127.0.0.1" : "0.0.0.0";
            settings.restServerInterface = RestServerInterfaces.SelectedIndex == 0 ? "127.0.0.1" : "0.0.0.0";

            if (settings.HasChanged)
            {
                settings.Save();
            }
        }
    }
}
