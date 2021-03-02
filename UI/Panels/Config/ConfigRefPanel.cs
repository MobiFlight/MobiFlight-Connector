using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight.Base;

namespace MobiFlight.UI.Panels.Config
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

            noConfigRefsPanel.Visible = (dv.Count == 0);
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
                int SelectedIndex = 0;
                SelectedIndex = (dv.Count > MAX_CONFIG_REFS) ? 
                                (MAX_CONFIG_REFS - configRefItemPanel.Controls.Count) - 1: 
                                (dv.Count - configRefItemPanel.Controls.Count) - 1;

                p.SetDataView(dv);
                p.SetPlaceholder(CONFIG_REFS_PLACEHOLDER[SelectedIndex]);
                p.configRefComboBox.SelectedIndex = SelectedIndex;

                p.Dock = DockStyle.Top;
                configRefItemPanel.Controls.Add(p);
            }
        }

        internal void syncFromConfig(InputConfigItem config)
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
                int SelectedIndex = 0;
                SelectedIndex = (dv.Count > MAX_CONFIG_REFS) ?
                                (MAX_CONFIG_REFS - configRefItemPanel.Controls.Count) - 1 :
                                (dv.Count - configRefItemPanel.Controls.Count) - 1;

                p.SetDataView(dv);
                p.SetPlaceholder(CONFIG_REFS_PLACEHOLDER[SelectedIndex]);
                p.configRefComboBox.SelectedIndex = SelectedIndex;

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

        internal InputConfigItem syncToConfig(InputConfigItem config)
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
