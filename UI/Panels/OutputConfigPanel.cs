using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MobiFlight.UI.Dialogs;

namespace MobiFlight.UI.Panels
{
    public partial class OutputConfigPanel : UserControl
    {
        public event EventHandler SettingsChanged;

        private int lastClickedRow = -1;
        //private DataTable configDataTable;
        public DataGridViewComboBoxColumn ArcazeSerial {  get { return arcazeSerial;  } }

        public OutputConfigPanel()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            arcazeSerial.Items.Clear();
            arcazeSerial.Items.Add(i18n._tr("none"));

            dataGridViewConfig.Columns["Description"].DefaultCellStyle.NullValue = i18n._tr("uiLabelDoubleClickToAddConfig");
            dataGridViewConfig.Columns["EditButtonColumn"].DefaultCellStyle.NullValue = "...";

            dataGridViewConfig.RowsAdded += new DataGridViewRowsAddedEventHandler(dataGridViewConfig_RowsAdded);

            configDataTable.RowChanged += new DataRowChangeEventHandler(configDataTable_RowChanged);
            configDataTable.RowDeleted += new DataRowChangeEventHandler(configDataTable_RowChanged);


            Helper.DoubleBufferedDGV(dataGridViewConfig, true);
        }

        public ExecutionManager ExecutionManager { get; set; }

        public DataSet DataSetConfig { get { return dataSetConfig; } }
        public DataTable ConfigDataTable { get { return configDataTable; } }

        public DataGridView DataGridViewConfig { get { return dataGridViewConfig; } }

        void dataGridViewConfig_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            // if datagridviewconfig.RowCount == 1 this means that only the "new line" is added yet
            /*
            if (e.RowIndex != -1 && dataGridViewConfig.RowCount != 1)
            {
                (sender as DataGridView).Rows[e.RowIndex].Cells["active"].Style.BackColor
                       = (sender as DataGridView).DefaultCellStyle.BackColor;             

                (sender as DataGridView).Rows[e.RowIndex].Cells["description"].Style.BackColor
                       = (sender as DataGridView).DefaultCellStyle.BackColor;             
            }
             * */
        }

        /// <summary>
        /// gets triggered if user changes values in the data grid
        /// </summary>
        private void dataGridViewConfig_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if ((e.FormattedValue as String) == "") return;

            switch (dataGridViewConfig[e.ColumnIndex, e.RowIndex].OwningColumn.Name)
            {
                case "FsuipcOffset":
                    try
                    {
                        Int32 tmp = Int32.Parse((e.FormattedValue as String).Replace("0x", ""), System.Globalization.NumberStyles.HexNumber);
                    }
                    catch
                    {
                        e.Cancel = true;
                        MessageBox.Show(
                                i18n._tr("uiMessageInvalidValueHexOnly"),
                                i18n._tr("InputValidation"),
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                    }
                    break;
            }

        } //dataGridViewConfig_CellValidating()

        /// <summary>
        /// handles errors when user submits data to the datagrid
        /// </summary>
        private void dataGridViewConfig_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            switch (dataGridViewConfig[e.ColumnIndex, e.RowIndex].OwningColumn.Name)
            {
                case "arcazeSerial":
                    // when loading config and not beeing connected to arcaze modules
                    // we actually do not know whether the serial infos are correct or not
                    // so in this case we add the new value to our items
                    //
                    // otherwise we would get an error due to validation issues
                    arcazeSerial.Items.Add(dataGridViewConfig[e.ColumnIndex, e.RowIndex].Value.ToString());
                    e.Cancel = false;
                    break;
            }
        } //dataGridViewConfig_DataError()

        /// <summary>
        /// click event when button in gridview gets clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewConfig_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // handle clicks on header cells or row-header cells
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            switch (dataGridViewConfig[e.ColumnIndex, e.RowIndex].OwningColumn.Name)
            {
                case "FsuipcOffset":
                case "fsuipcValueColumn":
                case "arcazeValueColumn":
                case "arcazeSerial":
                case "EditButtonColumn":
                    bool isNew = dataGridViewConfig.Rows[e.RowIndex].IsNewRow;
                    if (isNew)
                    {
                        MessageBox.Show(i18n._tr("uiMessageConfigLineNotSavedYet"),
                                        i18n._tr("Hint"));
                        return;
                    } //if

                    OutputConfigItem cfg;
                    DataRow row = null;
                    bool create = false;
                    if (isNew)
                    {
                        cfg = new OutputConfigItem();
                    }
                    else
                    {
                        row = (dataGridViewConfig.Rows[e.RowIndex].DataBoundItem as DataRowView).Row;

                        // the row had been saved but no config object has been created
                        // TODO: move this logic to an appropriate event, e.g. when leaving the gridrow focus of the new row
                        if ((dataGridViewConfig.Rows[e.RowIndex].DataBoundItem as DataRowView).Row["settings"].GetType() == typeof(System.DBNull))
                        {
                            (dataGridViewConfig.Rows[e.RowIndex].DataBoundItem as DataRowView).Row["settings"] = new OutputConfigItem();
                        }

                        cfg = ((dataGridViewConfig.Rows[e.RowIndex].DataBoundItem as DataRowView).Row["settings"] as OutputConfigItem);
                    }
                    _editConfigWithWizard(
                             row,
                             cfg,
                             create);

                    dataGridViewConfig.EndEdit();
                    break;
                case "Active":
                    // always end editing to store changes
                    dataGridViewConfig.EndEdit();
                    break;
            }
        }

        /// <summary>
        /// when using tab in the grid view, the focus ignores readonly cell and jumps ahead to the next cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewConfig_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewConfig[e.ColumnIndex, e.RowIndex].ReadOnly)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // do somehting here
            foreach (DataGridViewRow row in dataGridViewConfig.SelectedRows)
            {
                // we cannot delete a row which hasn't been saved yet
                if (row.IsNewRow) continue;
                dataGridViewConfig.Rows.Remove(row);
            }
        }

        /// <summary>
        /// this method is used to select the current row so that the context menu events may detect the current row
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewConfig_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                dataGridViewConfig.EndEdit();
                lastClickedRow = e.RowIndex;

                if (e.RowIndex != -1 && e.RowIndex != (sender as DataGridView).Rows.Count - 1)
                {
                    if (!(sender as DataGridView).Rows[e.RowIndex].Selected)
                    {
                        // reset all rows since we are not right clicking on a currently
                        // selected one
                        foreach (DataGridViewRow row in (sender as DataGridView).SelectedRows)
                        {
                            row.Selected = false;
                        }
                    }

                    // the current one becomes selected in any case
                    (sender as DataGridView).Rows[e.RowIndex].Selected = true;
                }
            }
        }

        private void dataGridViewConfig_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            dataGridViewConfig.EndEdit();
        }

        private void dataGridViewConfig_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // handle clicks on header cells or row-header cells
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            switch (dataGridViewConfig[e.ColumnIndex, e.RowIndex].OwningColumn.Name)
            {
                case "Description":
                    dataGridViewConfig.CurrentCell = dataGridViewConfig[e.ColumnIndex, e.RowIndex];
                    dataGridViewConfig.BeginEdit(true);
                    break;
            }
        }

        private void duplicateRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // this is called to ensure 
            // that all data has been stored in
            // the data table
            // otherwise there can occur strange inserts of new rows
            // at the first position instead of the expected one
            this.Validate();

            // do somehting here
            foreach (DataGridViewRow row in dataGridViewConfig.SelectedRows)
            {
                // ignore new rows since they cannot be copied nor deleted
                if (row.IsNewRow) continue;

                // get current config item
                // duplicate it
                // duplicate row 
                // link to new config item 
                DataRow currentRow = (row.DataBoundItem as DataRowView).Row;
                DataRow newRow = configDataTable.NewRow();

                foreach (DataColumn col in configDataTable.Columns)
                {
                    newRow[col.ColumnName] = currentRow[col.ColumnName];
                }

                OutputConfigItem cfg = ((row.DataBoundItem as DataRowView).Row["settings"] as OutputConfigItem);
                if (cfg != null)
                {
                    cfg = (cfg.Clone() as OutputConfigItem);
                }
                else
                {
                    cfg = new OutputConfigItem();
                }

                newRow["description"] += " " + i18n._tr("suffixCopy");
                newRow["settings"] = cfg;
                newRow["guid"] = Guid.NewGuid();

                int currentPosition = configDataTable.Rows.IndexOf(currentRow);
                if (currentPosition == -1)
                {
                    currentPosition = 1;
                }

                configDataTable.Rows.InsertAt(newRow, currentPosition + 1);

                row.Selected = false;
            }
        }

        private void dataGridViewContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            // do not show up context menu
            // id there is only the new line visible
            e.Cancel = (dataGridViewConfig.Rows.Count == 1 || (lastClickedRow == dataGridViewConfig.Rows.Count - 1));
        }

        private void dataGridViewConfig_KeyUp(object sender, KeyEventArgs e)
        {
            DataGridView dgv = (sender as DataGridView);
            int cellIndex = 2;
            
            // do something
            // toggle active if current key is a simple character
            if (e.KeyCode.ToString().Length == 1)
            {

                // handle clicks on header cells or row-header cells
                if (dgv.CurrentRow.Index < 0 || dgv.CurrentCell.ColumnIndex < 0) return;

                dgv.CurrentCell = dgv[cellIndex, dgv.CurrentRow.Index];

                if (!dgv.CurrentRow.Cells[cellIndex].IsInEditMode)
                {
                    dgv.BeginEdit(true);
                    if (e.KeyCode != Keys.F2)
                    {
                        (dgv.EditingControl as TextBox).Text = (e.Shift) ? e.KeyCode.ToString() : e.KeyCode.ToString().ToLower();
                        (dgv.EditingControl as TextBox).Select(1, 0);
                    }
                }
                return;
            }
            // un/check all rows if key is a space
            else if (e.KeyCode == Keys.Space)
            {
                e.SuppressKeyPress = true;

                bool isChecked = false;
                if ((sender as DataGridView).SelectedRows.Count == 0) return;

                // it is assumed that the first cell is the one with the checkbox
                isChecked = Boolean.Parse((sender as DataGridView).SelectedRows[0].Cells[0].Value.ToString());

                foreach (DataGridViewRow row in (sender as DataGridView).SelectedRows)
                {
                    row.Cells[0].Value = !isChecked;
                }

                dgv.RefreshEdit();
            }
            else if (e.KeyCode == Keys.Return)
            {
                // handle clicks on header cells or row-header cells
                if (dgv.CurrentRow.Index < 0 || dgv.CurrentCell.ColumnIndex < 0) return;

                e.Handled = true;
                e.SuppressKeyPress = true;

                if (!dgv.CurrentRow.Cells[cellIndex].IsInEditMode)
                {
                    if (dgv.Name == dataGridViewConfig.Name)
                    {
                        dgv.CurrentCell = dgv[cellIndex, dgv.CurrentRow.Index];

                        OutputConfigItem cfg;
                        DataRow row = null;
                        bool create = false;

                        row = (dataGridViewConfig.Rows[dgv.CurrentRow.Index].DataBoundItem as DataRowView).Row;

                        // the row had been saved but no config object has been created
                        // TODO: move this logic to an appropriate event, e.g. when leaving the gridrow focus of the new row
                        if ((dataGridViewConfig.Rows[dgv.CurrentRow.Index].DataBoundItem as DataRowView).Row["settings"].GetType() == typeof(System.DBNull))
                        {
                            (dataGridViewConfig.Rows[dgv.CurrentRow.Index].DataBoundItem as DataRowView).Row["settings"] = new OutputConfigItem();
                        }

                        cfg = ((dataGridViewConfig.Rows[dgv.CurrentRow.Index].DataBoundItem as DataRowView).Row["settings"] as OutputConfigItem);

                        _editConfigWithWizard(
                                 row,
                                 cfg,
                                 create);

                        dataGridViewConfig.EndEdit();
                    }
                }
            }
            else
            {
                // do nothing
            }
        }

        private void dataGridViewConfig_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridViewConfig_CellContentClick(sender, e);
        }

        /// <summary>
        /// initializes the config wizard and shows the modal dialog.
        /// afterwards stores some values in the data set so that the visible grid columns are filled with data.
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="cfg"></param>
        /// <param name="create"></param>
        private void _editConfigWithWizard(DataRow dataRow, OutputConfigItem cfg, bool create)
        {
            // refactor!!! dependency to arcaze cache etc not nice
            Form wizard = new ConfigWizard( ExecutionManager,
                                            cfg,
#if ARCAZE
                                            execManager.getModuleCache(), 
                                            getArcazeModuleSettings(), 
#endif
                                            dataSetConfig,
                                            dataRow["guid"].ToString()
                                          );
            wizard.StartPosition = FormStartPosition.CenterParent;
            if (wizard.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (dataRow == null) return;
                // do something special
                dataRow["fsuipcOffset"] = "0x" + cfg.FSUIPCOffset.ToString("X4");
                dataRow["arcazeSerial"] = cfg.DisplaySerial;
            };
        }

        /// <summary>
        /// enables the save button in toolbar after the user has changed config data
        /// </summary>        
        void configDataTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            SettingsChanged?.Invoke(sender, null);
        } //configDataTable_RowChanged

        private void configDataTable_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            e.Row["guid"] = Guid.NewGuid();
        }

        private void configDataTable_RowChanged_1(object sender, DataRowChangeEventArgs e)
        {
            if (e.Row["guid"] == DBNull.Value)
                e.Row["guid"] = Guid.NewGuid();
        }

        private void dataGridViewConfig_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["guid"].Value = Guid.NewGuid();
        }

        /// <summary>
        /// use the settings from the config object and initialize the grid cells 
        /// this is needed after loading and saving configs
        /// </summary>
        public void RestoreValuesInGridView()
        {
            foreach (DataRow row in ConfigDataTable.Rows)
            {
                OutputConfigItem cfgItem = row["settings"] as OutputConfigItem;
                if (cfgItem != null)
                {
                    row["fsuipcOffset"] = "0x" + cfgItem.FSUIPCOffset.ToString("X4");
                    if (cfgItem.DisplaySerial != null)
                    {
                        row["arcazeSerial"] = cfgItem.DisplaySerial.ToString();
                    }
                }
            }
        } //_restoreValuesInGridView()

        /// <summary>
        /// removes unnecessary information that is now stored in the settings node itself
        /// </summary>
        /// <remarks>DEPRECATED</remarks>
        public void ApplyBackwardCompatibilitySaving()
        {
            // delete old values from config that are part of the new settings-node now
            foreach (DataRow row in ConfigDataTable.Rows)
            {
                if (row["settings"].GetType() != typeof(System.DBNull))
                {
                    row["converter"] = null;
                    row["trigger"] = null;
                    row["fsuipcOffset"] = null;
                    row["fsuipcSize"] = null;
                    row["mask"] = null;
                    row["comparison"] = null;
                    row["comparisonValue"] = null;
                    row["usbArcazePin"] = null;
                    row["arcazeSerial"] = null;
                }
            }
        } //_applyBackwardCompatibilitySaving()



        private void dataGridViewConfig_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                e.Handled = true;
            }
        }
    }
}
