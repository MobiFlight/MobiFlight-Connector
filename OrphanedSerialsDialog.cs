using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight;

namespace ArcazeUSB
{
    public partial class OrphanedSerialsDialog : Form
    {
        ArcazeCache arcazeCache = null;
        DataTable configDataTable = null;
        DataTable realConfigDataTable = null;
        bool changed = false;

        public OrphanedSerialsDialog(ArcazeCache arcazeCache, DataTable dataTable)
        {
            this.arcazeCache = arcazeCache;
            this.configDataTable = dataTable.Copy();
            this.realConfigDataTable = dataTable;
            InitializeComponent();
            updateOrphanedList();
        }

        protected void updateOrphanedList()
        {
            List<String> configSerials = new List<string>();
            List<String> arcazeSerials = new List<string>();

            connectedModulesComboBox.Items.Clear();
            orphanedSerialsListBox.Items.Clear();

            foreach (IModuleInfo module in arcazeCache.getModuleInfo())
            {
                arcazeSerials.Add(module.Name + "/ " + module.Serial);
                connectedModulesComboBox.Items.Add(module.Name + "/ " + module.Serial);                
            }

            foreach (DataRow row in configDataTable.Rows) {
                ArcazeConfigItem cfg = row["settings"] as ArcazeConfigItem;
                if (cfg.DisplaySerial != "" && 
                    cfg.DisplaySerial  != "-" && 
                    !configSerials.Contains(cfg.DisplaySerial) && 
                    !arcazeSerials.Contains(cfg.DisplaySerial))
                {
                    configSerials.Add(cfg.DisplaySerial);
                    orphanedSerialsListBox.Items.Add(cfg.DisplaySerial);
                }
            }

            if (connectedModulesComboBox.Items.Count > 0) connectedModulesComboBox.SelectedIndex = 0;
        }

        protected void replaceSerialBySerial(String oldSerial, String newSerial)
        {
            foreach (DataRow row in configDataTable.Rows)
            {
                ArcazeConfigItem cfg = row["settings"] as ArcazeConfigItem;
                if (cfg.DisplaySerial == oldSerial)
                {
                    cfg.DisplaySerial = newSerial;
                    row["settings"] = cfg;
                }
            }            
        }

        public bool HasOrphanedSerials()
        {
            return (orphanedSerialsListBox.Items.Count > 0);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            for (int i=0; i!=realConfigDataTable.Rows.Count;++i)
            {
                realConfigDataTable.Rows[i]["settings"] = configDataTable.Rows[i]["settings"];                
            }
            //realConfigDataTable = configDataTable;
            DialogResult = DialogResult.OK;
        }

        private void orphanedSerialsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            connectedModulesAssignButton.Enabled = (orphanedSerialsListBox.Items.Count > 0);
        }

        private void connectedModulesAssignButton_Click(object sender, EventArgs e)
        {            
            replaceSerialBySerial(orphanedSerialsListBox.SelectedItem.ToString(), 
                                  connectedModulesComboBox.SelectedItem.ToString());
            changed = true;
            updateOrphanedList();
            (sender as Button).Enabled = false;
        }

        public bool HasChanged()
        {
            return changed;
        }
    }
}
