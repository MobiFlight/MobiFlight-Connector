using MobiFlight.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels
{
    public partial class OutputConfigPanel : UserControl
    {
        public event EventHandler SettingsChanged;
        public event EventHandler SettingsDialogRequested;
        private object[] EditedItem = null;

        private int lastClickedRow = -1;
        private List<String> SelectedGuids = new List<String>();
        //private DataTable configDataTable;
        
        public OutputConfigPanel()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            dataGridViewConfig.Columns["Description"].DefaultCellStyle.NullValue = i18n._tr("uiLabelDoubleClickToAddConfig");
            dataGridViewConfig.Columns["EditButtonColumn"].DefaultCellStyle.NullValue = "...";

            dataGridViewConfig.RowsAdded += new DataGridViewRowsAddedEventHandler(DataGridViewConfig_RowsAdded);
            
            configDataTable.RowChanged += new DataRowChangeEventHandler(ConfigDataTable_RowChanged);
            configDataTable.RowDeleted += new DataRowChangeEventHandler(ConfigDataTable_RowChanged);

            Helper.DoubleBufferedDGV(dataGridViewConfig, true);
        }

        public ExecutionManager ExecutionManager { get; set; }

        public DataSet DataSetConfig { get { return dataSetConfig; } }
        public DataTable ConfigDataTable { get { return configDataTable; } }

        public DataGridView DataGridViewConfig { get { return dataGridViewConfig; } }

        void DataGridViewConfig_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
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
        private void DataGridViewConfig_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
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

        } //DataGridViewConfig_CellValidating()

        /// <summary>
        /// click event when button in gridview gets clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridViewConfig_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // handle clicks on header cells or row-header cells
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            switch (dataGridViewConfig[e.ColumnIndex, e.RowIndex].OwningColumn.Name)
            {
                case "FsuipcOffset":
                case "fsuipcValueColumn":
                case "arcazeValueColumn":
                case "moduleSerial":
                case "EditButtonColumn":
                case "OutputName":
                case "OutputType":
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
                    EditConfigWithWizard(
                             row,
                             cfg,
                             create);

                    dataGridViewConfig.EndEdit();
                    break;
                case "active":
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
        private void DataGridViewConfig_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewConfig[e.ColumnIndex, e.RowIndex].ReadOnly)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void DeleteRowToolStripMenuItem_Click(object sender, EventArgs e)
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
        private void DataGridViewConfig_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            lastClickedRow = e.RowIndex;

            if (e.Button == MouseButtons.Right) 
            {
                if (dataGridViewConfig.IsCurrentCellInEditMode) return; 

                dataGridViewConfig.EndEdit();
                
                if (e.RowIndex != -1)
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
            } else
            {
                if (e.RowIndex == -1)
                {
                    // we know that we have clicked on the header area for sorting
                    SelectedGuids.Clear();
                    foreach (DataGridViewRow row in (sender as DataGridView).SelectedRows)
                    {
                        DataRow currentRow = (row.DataBoundItem as DataRowView).Row;
                        if (currentRow != null)
                            SelectedGuids.Add(currentRow["guid"].ToString());
                    }
                }
            }
        }

        private void DataGridViewConfig_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            dataGridViewConfig.EndEdit();
        }

        private void DataGridViewConfig_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
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

        private void DuplicateRowToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void DataGridViewContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            // do not show up context menu
            // id there is only the new line visible
            // e.Cancel = (dataGridViewConfig.Rows.Count == 1 || (lastClickedRow == dataGridViewConfig.Rows.Count - 1));
            bool isNotLastRow = (lastClickedRow != dataGridViewConfig.Rows.Count - 1);
            copyToolStripMenuItem.Enabled = isNotLastRow;
            pasteToolStripMenuItem.Enabled = Clipboard.Instance.OutputConfigItem != null;
            duplicateRowToolStripMenuItem.Enabled = isNotLastRow;
            deleteRowToolStripMenuItem.Enabled = isNotLastRow;
        }

        private void DataGridViewConfig_KeyUp(object sender, KeyEventArgs e)
        {
            DataGridView dgv = (sender as DataGridView);
            int cellIndex = 2;
            
            // do something
            // toggle active if current key is a simple character
            if (e.KeyCode.ToString().Length == 1 && !e.Control)
            {

                // handle clicks on header cells or row-header cells
                if (dgv.CurrentRow.Index < 0 || dgv.CurrentCell.ColumnIndex < 0) return;

                dgv.CurrentCell = dgv[cellIndex, dgv.CurrentRow.Index];

                if (!dgv.CurrentRow.Cells[cellIndex].IsInEditMode)
                {
                    dgv.BeginEdit(true);
                    if (e.KeyCode != Keys.F2)
                    {
                        
                        (dgv.EditingControl as TextBox).Text = (e.Shift || Control.IsKeyLocked(Keys.CapsLock)) ? e.KeyCode.ToString() : e.KeyCode.ToString().ToLower();
                        (dgv.EditingControl as TextBox).Select(1, 0);
                    }
                }
            }
            // un/check all rows if key is a space
            else if (e.KeyCode == Keys.Space)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                bool isChecked = false;
                if ((sender as DataGridView).SelectedRows.Count == 0) return;

                // it is assumed that the first cell is the one with the checkbox
                isChecked = Boolean.Parse((sender as DataGridView).SelectedRows[0].Cells[0].Value.ToString());

                foreach (DataGridViewRow row in (sender as DataGridView).SelectedRows)
                {
                    row.Cells[0].Value = !isChecked;
                }

                SettingsChanged?.Invoke(sender, null);
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

                        if (dataGridViewConfig.Rows[dgv.CurrentRow.Index].DataBoundItem == null)
                        {
                            return;
                        }

                        row = (dataGridViewConfig.Rows[dgv.CurrentRow.Index].DataBoundItem as DataRowView).Row;

                        // the row had been saved but no config object has been created
                        // TODO: move this logic to an appropriate event, e.g. when leaving the gridrow focus of the new row
                        if ((dataGridViewConfig.Rows[dgv.CurrentRow.Index].DataBoundItem as DataRowView).Row["settings"].GetType() == typeof(System.DBNull))
                        {
                            (dataGridViewConfig.Rows[dgv.CurrentRow.Index].DataBoundItem as DataRowView).Row["settings"] = new OutputConfigItem();
                        }

                        cfg = ((dataGridViewConfig.Rows[dgv.CurrentRow.Index].DataBoundItem as DataRowView).Row["settings"] as OutputConfigItem);

                        EditConfigWithWizard(
                                 row,
                                 cfg,
                                 create);

                        dataGridViewConfig.EndEdit();
                    }
                }
            }
            else if (e.KeyCode == Keys.V && e.Control)
            {
                // handle clicks on header cells or row-header cells
                if (dgv.CurrentRow.Index < 0 || dgv.CurrentCell.ColumnIndex < 0) return;
                int index = dgv.CurrentRow.Index;

                PasteFromClipboard(index + 1);
            }

            else if (e.KeyCode == Keys.C && e.Control)
            {
                // handle clicks on header cells or row-header cells
                if (dgv.CurrentRow.Index < 0 || dgv.CurrentCell.ColumnIndex < 0) return;

                if ((dataGridViewConfig.Rows[dgv.CurrentRow.Index].DataBoundItem as DataRowView).Row["description"] != null)
                {
                    bool Active = (bool)(dataGridViewConfig.Rows[dgv.CurrentRow.Index].DataBoundItem as DataRowView).Row["active"];
                    String Description = (dataGridViewConfig.Rows[dgv.CurrentRow.Index].DataBoundItem as DataRowView).Row["description"].ToString();
                    OutputConfigItem cfg = ((dataGridViewConfig.Rows[dgv.CurrentRow.Index].DataBoundItem as DataRowView).Row["settings"] as OutputConfigItem);

                    CopyToClipboard(Active, Description, cfg);
                }
            }
            else
            {
                // do nothing
            }
        }

        private static void CopyToClipboard(bool Active, string Description, OutputConfigItem cfg)
        {
            System.Windows.Forms.Clipboard.SetText(Description);
            Clipboard.Instance.OutputConfigActive = Active;
            Clipboard.Instance.OutputConfigName = Description;

            if (cfg != null)
            {
                Clipboard.Instance.OutputConfigItem = cfg;
            }
        }

        private void PasteFromClipboard(int index)
        {
            this.Validate();
            DataRow currentRow = configDataTable.NewRow();
            currentRow["guid"] = Guid.NewGuid();
            currentRow["active"] = Clipboard.Instance.OutputConfigActive;

            if (Clipboard.Instance.OutputConfigName != null)
            {
                currentRow["description"] = Clipboard.Instance.OutputConfigName.Clone() as String;
                currentRow["description"] += $" ({i18n._tr("suffixCopy")})";
            }

            if (Clipboard.Instance.OutputConfigItem != null)
            {
                OutputConfigItem cfg = Clipboard.Instance.OutputConfigItem.Clone() as OutputConfigItem;
                currentRow["settings"] = cfg;
            }

            if (currentRow.RowState == DataRowState.Detached)
            {
                configDataTable.Rows.InsertAt(currentRow, index);
            }

            RestoreValuesInGridView();
        }

        private void DataGridViewConfig_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewConfig_CellContentClick(sender, e);
        }

        /// <summary>
        /// initializes the config wizard and shows the modal dialog.
        /// afterwards stores some values in the data set so that the visible grid columns are filled with data.
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="cfg"></param>
        /// <param name="create"></param>
        private void EditConfigWithWizard(DataRow dataRow, OutputConfigItem cfg, bool create)
        {
            // refactor!!! dependency to arcaze cache etc not nice
            ConfigWizard wizard = new ConfigWizard(ExecutionManager,
                                            cfg,
#if ARCAZE
                                            ExecutionManager.getModuleCache(),
                                            ExecutionManager.getModuleCache().GetArcazeModuleSettings(),
#endif
                                            dataSetConfig,
                                            dataRow["guid"].ToString()
                                          );

            wizard.StartPosition = FormStartPosition.CenterParent;
            wizard.SettingsDialogRequested += new EventHandler(wizard_SettingsDialogRequested);

            if (wizard.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (dataRow == null) return;

                // do something special
                if (wizard.ConfigHasChanged()) {
                    SettingsChanged?.Invoke(cfg, null);
                    RestoreValuesInGridView();
                }
            };
        }

        void wizard_SettingsDialogRequested(object sender, EventArgs e)
        {
            //(sender as InputConfigWizard).Close();
            SettingsDialogRequested?.Invoke(sender, null);

        }

        /// <summary>
        /// enables the save button in toolbar after the user has changed config data
        /// </summary>        
        void ConfigDataTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action==DataRowAction.Add ||
                //e.Action == DataRowAction.Change ||
                e.Action == DataRowAction.Delete) 
                    SettingsChanged?.Invoke(sender, null);

        } //configDataTable_RowChanged

        private void ConfigDataTable_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            e.Row["guid"] = Guid.NewGuid();
        }

        private void ConfigDataTable_RowChanged_1(object sender, DataRowChangeEventArgs e)
        {
            if (e.Row["guid"] == DBNull.Value)
                e.Row["guid"] = Guid.NewGuid();
        }

        private void DataGridViewConfig_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
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
                row["OutputName"] = "-";
                row["OutputType"] = "-";
                row["arcazeSerial"] = "-";

                if (cfgItem != null)
                {
                    row["fsuipcOffset"] = "0x" + cfgItem.FSUIPC.Offset.ToString("X4");
                    if (cfgItem.DisplaySerial != null && cfgItem.DisplaySerial != "-")
                    {
                        row["arcazeSerial"] = cfgItem.DisplaySerial.ToString().Split('/')[0];
                    
                        row["OutputType"] = cfgItem.DisplayType;

                        // only exception for the type label
                        if (cfgItem.DisplayType == MobiFlightOutput.TYPE)
                            row["OutputType"] = "LED / Output";

                        switch (cfgItem.DisplayType)
                        {
                            case MobiFlightLedModule.TYPE:
                                row["OutputName"] = cfgItem.LedModule.DisplayLedAddress;
                                break;
                            case MobiFlightOutput.TYPE:
                                row["OutputName"] = cfgItem.Pin.DisplayPin;
                                break;
                            case MobiFlightLcdDisplay.TYPE:
                                row["OutputName"] = cfgItem.LcdDisplay.Address;
                                break;
                            case MobiFlightServo.TYPE:
                                row["OutputName"] = cfgItem.Servo.Address;
                                break;
                            case MobiFlightStepper.TYPE:
                                row["OutputName"] = cfgItem.Stepper.Address;
                                break;
                            case MobiFlightShiftRegister.TYPE:
                                row["OutputName"] = cfgItem.ShiftRegister.ToString();
                                break;

                        }
                    } else if(cfgItem.DisplayType=="InputAction")
                    {
                        row["OutputType"] = cfgItem.DisplayType;
                        if (cfgItem.ButtonInputConfig!=null)
                        {
                            if (cfgItem.ButtonInputConfig.onRelease!=null)
                                row["OutputName"] = cfgItem.ButtonInputConfig.onRelease.GetType().ToString().Replace("MobiFlight.InputConfig.", "");
                            if (cfgItem.ButtonInputConfig.onPress != null)
                                row["OutputName"] = cfgItem.ButtonInputConfig.onPress.GetType().ToString().Replace("MobiFlight.InputConfig.", "");
                        }
                        if (cfgItem.AnalogInputConfig != null)
                        {
                            row["OutputName"] = cfgItem.AnalogInputConfig.onChange.GetType().ToString().Replace("MobiFlight.InputConfig.", "");
                        }
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



        private void DataGridViewConfig_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                e.Handled = true;
            }
        }

        private void dataGridViewConfig_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow gridrow = dataGridViewConfig.Rows[e.RowIndex];
            DataRowView rowview = (DataRowView)gridrow.DataBoundItem;


            // can be null on creating a new config (last line)
            if (rowview == null) return;

            DataRow row = rowview.Row;
            if (row.RowState == DataRowState.Detached)
                row.Table.Rows.Add(row);

            if (EditedItem != null &&
                (   // this is the checkbox
                    (bool)row.ItemArray[0]!= (bool)EditedItem[0] || 
                    // this is the description text
                    row.ItemArray[9] as string != EditedItem[9] as string)
            )
            {
                SettingsChanged?.Invoke(sender, null);
            }
        }

        private void dataGridViewConfig_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridViewRow gridrow = dataGridViewConfig.Rows[e.RowIndex];
            DataRowView rowview = (DataRowView)gridrow.DataBoundItem;

            // can be null on creating a new config (last line)
            if (rowview == null) return;

            if (rowview.Row.ItemArray != null)
                EditedItem = rowview.Row.ItemArray;
        }

        private void dataGridViewConfig_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.ContextMenu = new ContextMenu();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // do somehting here
            foreach (DataGridViewRow row in dataGridViewConfig.SelectedRows)
            {
                // ignore new rows since they cannot be copied nor deleted
                if (row.IsNewRow) continue;

                DataRow currentRow = (row.DataBoundItem as DataRowView).Row;
                bool Active = (bool)currentRow["active"];
                String Description = currentRow["description"] as String;
                OutputConfigItem cfg = currentRow["settings"] as OutputConfigItem;
                CopyToClipboard(Active, Description, cfg);
                return;
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // do somehting here
            foreach (DataGridViewRow row in dataGridViewConfig.SelectedRows)
            {
                // ignore new rows since they cannot be copied nor deleted
                //if (row.IsNewRow) continue;

                int index = row.Index;
                PasteFromClipboard(index+1);
                return;
            }
        }

        private void dataGridViewConfig_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.Reset)
            {
                foreach (DataGridViewRow row in (sender as DataGridView).Rows)
                {
                    if (row.DataBoundItem == null) continue;

                    DataRow currentRow = (row.DataBoundItem as DataRowView).Row;
                    String guid = currentRow["guid"].ToString();

                    if (SelectedGuids.Contains(guid))
                        row.Selected = true;
                }
            }
        }
    }
}
