using MobiFlight.Base;
using MobiFlight.Config;
using MobiFlight.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Config
{
    public partial class ConfigRefPanel : UserControl
    {
        List<OutputConfigItem> outputConfigs;
        // static byte MAX_CONFIG_REFS = 8;
        static string[] CONFIG_REFS_PLACEHOLDER = { "#", "!", "?", "@", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M" };

        public ConfigRefPanel()
        {
            InitializeComponent();

        }

        public void SetConfigRefsDataView(List<OutputConfigItem> outputConfigs, String filterGuid)
        {
            this.outputConfigs = outputConfigs.Where(c => c.GUID != filterGuid).ToList();

            // don't show the panel as long as we have no configs for reference
            noConfigRefsPanel.Visible = (outputConfigs.Count == 0);

            // disable the button in case there are no more configs to add as reference
            addReferenceButton.Enabled = (outputConfigs.Count > 0);
        }

        internal void syncFromConfig(IConfigRefConfigItem config)
        {
            configRefItemPanel.Controls.Clear();

            foreach (ConfigRef configRef in config.ConfigRefs)
            {
                if (outputConfigs.Where(c => c.GUID == configRef.Ref).ToList().Count == 0) continue;

                ConfigRefPanelItem p = new ConfigRefPanelItem();
                p.SetDataView(outputConfigs);
                p.syncFromConfig(configRef);

                p.ConfigRemoved += P_ConfigRemoved;

                p.Dock = DockStyle.Top;
                configRefItemPanel.Controls.Add(p);
            }
        }

        internal IConfigRefConfigItem syncToConfig(IConfigRefConfigItem config)
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

        private void addReferenceButton_Click(object sender, EventArgs e)
        {
            if (configRefItemPanel.Controls.Count >= outputConfigs.Count)
            {
                String msg = i18n._tr("uiMessageNoMoreConfigReferenceAvailable");
                TimeoutMessageDialog.Show(msg, i18n._tr("Hint"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ConfigRefPanelItem p = new ConfigRefPanelItem();

            p.SetPlaceholder(FindAvailablePlaceholder());
            p.SetDataView(outputConfigs);
            p.configRefComboBox.SelectedIndex = configRefItemPanel.Controls.Count;

            p.ConfigRemoved += P_ConfigRemoved;

            p.Dock = DockStyle.Top;
            configRefItemPanel.Controls.Add(p);
        }

        private void P_ConfigRemoved(object sender, EventArgs e)
        {
            configRefItemPanel.Controls.Remove(sender as ConfigRefPanelItem);
        }

        private String FindAvailablePlaceholder()
        {
            String result = "";
            List<String> usedPlaceholder = new List<String>();

            foreach (var cfgRefPanel in configRefItemPanel.Controls)
            {
                ConfigRef configRef = new ConfigRef();
                (cfgRefPanel as ConfigRefPanelItem).syncToConfig(configRef);
                usedPlaceholder.Add(configRef.Placeholder);
            }
            foreach (var ph in CONFIG_REFS_PLACEHOLDER)
            {
                if (usedPlaceholder.Contains(ph)) continue;
                result = ph;
                break;
            }

            return result;
        }
    }
}
