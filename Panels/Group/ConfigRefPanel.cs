using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight.Base;

namespace MobiFlight.Panels.Group
{
    public partial class ConfigRefPanel : UserControl
    {
        DataView dv;
        static byte MAX_CONFIG_REFS = 6;
        static string[] CONFIG_REFS_PLACEHOLDER = { "#", "§", "?", "@", "A", "B", "C" };

        public ConfigRefPanel()
        {
            InitializeComponent();
        }

        private void ConfigRefPanel_Load(object sender, EventArgs e)
        {

        }

        public void SetConfigRefsDataView(DataView dv, String filterGuid)
        {
            this.dv = dv;
            dv.RowFilter = "guid <> '" + filterGuid + "'";
        }

        internal void syncFromConfig(OutputConfigItem config)
        {
            configRefItemPanel.Controls.Clear();

            foreach (ConfigRef configRef in config.ConfigRefs)
            {
                ConfigRefPanelItem p = new ConfigRefPanelItem();
                p.SetDataView(dv);
                p.syncFromConfig(configRef);

                p.Dock = DockStyle.Top;
                configRefItemPanel.Controls.Add(p);
            }

            while (configRefItemPanel.Controls.Count < dv.Count &&
                   configRefItemPanel.Controls.Count < MAX_CONFIG_REFS)
            {
                ConfigRefPanelItem p = new ConfigRefPanelItem();
                p.SetDataView(dv);
                p.SetPlaceholder(CONFIG_REFS_PLACEHOLDER[configRefItemPanel.Controls.Count]);

                p.Dock = DockStyle.Top;
                configRefItemPanel.Controls.Add(p);
            }
        }

        internal OutputConfigItem syncToConfig(OutputConfigItem config)
        {
            config.ConfigRefs.Clear();

            // sync the config ref settings back to the config
            foreach (ConfigRefPanelItem p in configRefItemPanel.Controls.OfType<ConfigRefPanelItem>())
            {
                ConfigRef configRef = new ConfigRef();
                p.syncToConfig(configRef);
                config.ConfigRefs.Add(configRef);
            }

            return config;
        }
    }
}
