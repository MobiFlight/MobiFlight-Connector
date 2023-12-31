using MobiFlight.Base;
using MobiFlight.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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

        private Point DataGridTopLeftPoint = new Point();
        private Point DataGridBottomRightPoint = new Point();
        private Rectangle RectangleMouseDown;
        private int RowIndexMouseDown;
        private int RowCurrentDragHighlight = 0;
        private int RowNeighbourDragHighlight = 0;
        private bool IsInCurrentRowTopHalf;
        private System.Windows.Forms.Timer DropTimer = new Timer();
        private Bitmap CurrentCursorBitmap;
        private string LastSortingColumnName = string.Empty;
        private SortOrder LastSortingOrder = SortOrder.None;

        public OutputConfigPanel()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            dataGridViewConfig.DataMember = null;
            dataGridViewConfig.DataSource = configDataTable;
            dataGridViewConfig.Columns["Description"].DefaultCellStyle.NullValue = i18n._tr("uiLabelDoubleClickToAddConfig");
            dataGridViewConfig.Columns["EditButtonColumn"].DefaultCellStyle.NullValue = "...";

            dataGridViewConfig.RowsAdded += new DataGridViewRowsAddedEventHandler(DataGridViewConfig_RowsAdded);
            
            configDataTable.RowChanged += new DataRowChangeEventHandler(ConfigDataTable_RowChanged);
            configDataTable.RowDeleted += new DataRowChangeEventHandler(ConfigDataTable_RowChanged);
            configDataTable.TableCleared += new DataTableClearEventHandler((o,a)=> { SettingsChanged?.Invoke(this, null); });

            Helper.DoubleBufferedDGV(dataGridViewConfig, true);

            DropTimer.Interval = 400;
            DropTimer.Tick += DropTimer_Tick;
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
                    var Row = dataGridViewConfig.Rows[e.RowIndex];
                    bool isNew = Row.IsNewRow;
                    if (isNew)
                    {
                        MessageBox.Show(i18n._tr("uiMessageConfigLineNotSavedYet"),
                                        i18n._tr("Hint"));
                        return;
                    } //if

                    OutputConfigItem cfg;
                    DataRow DataRow = null;
                    bool create = false;
                    if (isNew)
                    {
                        cfg = new OutputConfigItem();
                    }
                    else
                    {
                        DataRow = (Row.DataBoundItem as DataRowView).Row;

                        // the row had been saved but no config object has been created
                        // TODO: move this logic to an appropriate event, e.g. when leaving the gridrow focus of the new row
                        if (DataRow["settings"].GetType() == typeof(System.DBNull))
                        {
                            DataRow["settings"] = new OutputConfigItem();
                        }

                        cfg = DataRow["settings"] as OutputConfigItem;
                    }
                    EditConfigWithWizard(
                             DataRow,
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
                        DataRow currentRow = (row.DataBoundItem as DataRowView)?.Row;
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
                DataRow currentRow = (row.DataBoundItem as DataRowView)?.Row;
                if (currentRow == null) continue; 
                
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
                        if (row["settings"].GetType() == typeof(System.DBNull))
                        {
                            row["settings"] = new OutputConfigItem();
                        }

                        cfg = row["settings"] as OutputConfigItem;

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

                if ((dataGridViewConfig.Rows[dgv.CurrentRow.Index].DataBoundItem as DataRowView)?.Row["description"] != null)
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
                                            dataRow["guid"].ToString(),
                                            dataRow["description"].ToString()
                                          )
            {
                StartPosition = FormStartPosition.CenterParent
            };
            wizard.SettingsDialogRequested += new EventHandler(wizard_SettingsDialogRequested);

            if (wizard.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (dataRow == null) return;

                if (wizard.ConfigHasChanged()) {
                    // we have to update the config
                    // using the duplicated config 
                    // that the user edited with the wizard
                    dataRow["settings"] = wizard.Config;
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

        private void ActivateAutoColumnWidth()
        {                   
            dataGridViewConfig.Columns["moduleSerial"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewConfig.Columns["OutputName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewConfig.Columns["OutputType"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void DeactivateAutoColumnWidth()
        {
            // Deactivate and adjust FillWeights
            string[] columnNames = { "moduleSerial", "OutputName", "OutputType" };

            // description has FillWeight 1000
            var descColumn = dataGridViewConfig.Columns["description"];
            foreach (string name in columnNames)
            {
                int currentWidth = dataGridViewConfig.Columns[name].Width;
                dataGridViewConfig.Columns[name].FillWeight = (float)currentWidth / (float)descColumn.Width * 1450;
                dataGridViewConfig.Columns[name].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        /// <summary>
        /// use the settings from the config object and initialize the grid cells 
        /// this is needed after loading and saving configs
        /// </summary>
        public void RestoreValuesInGridView()
        {
            // Needed for performance reasons
            DeactivateAutoColumnWidth();
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
                        row["arcazeSerial"] = SerialNumber.ExtractDeviceName(cfgItem.DisplaySerial);
                    
                        row["OutputType"] = cfgItem.DisplayType;

                        // only exception for the type label
                        if (cfgItem.DisplayType == MobiFlightOutput.TYPE)
                            row["OutputType"] = "LED / Output";

                        if (cfgItem.DisplayType == MobiFlightCustomDevice.TYPE)
                            row["OutputType"] = cfgItem.CustomDevice.CustomType;

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
                            case MobiFlightCustomDevice.TYPE:
                                row["OutputName"] = cfgItem.CustomDevice.CustomName;
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
            // Auto adjust column width
            ActivateAutoColumnWidth();

            // Deactivate again, to make them resizable
            DeactivateAutoColumnWidth();
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

                DataRow currentRow = (row.DataBoundItem as DataRowView)?.Row;
                if (currentRow == null) continue; 
                
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
                int index = row.Index;
                PasteFromClipboard(index+1);
                return;
            }
        }

        /// <summary>
        /// Is sorting in DataGridView active.
        /// </summary>        
        public bool IsSortingActive()
        {
            return dataGridViewConfig.SortOrder != SortOrder.None;
        }

        /// <summary>
        /// Reset sorting in DataGridView
        /// </summary>
        public void ResetSorting()
        {
            LastSortingColumnName = string.Empty;
            LastSortingOrder = SortOrder.None;
            configDataTable.DefaultView.Sort = string.Empty;
        }

        private void dataGridViewConfig_Sorted(object sender, EventArgs e)
        {
            if (IsSortingActive())
            {
                string name = dataGridViewConfig.SortedColumn.Name;
                // It is always (Ascending) -> (Descending) -> (Ascending)
                // Reset sorting after previous Descending
                if (LastSortingColumnName == name && LastSortingOrder == SortOrder.Descending)
                {
                    ResetSorting();
                }
                else
                {
                    // Store current sorting
                    LastSortingColumnName = name;
                    LastSortingOrder = dataGridViewConfig.SortOrder;
                }
            }
        }

        private void dataGridViewConfig_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.Reset)
            {
                dataGridViewConfig.ClearSelection(); // necessary because on sorting reset, callback is called twice
                foreach (DataGridViewRow row in (sender as DataGridView).Rows)
                {
                    if (row.DataBoundItem as DataRowView == null) continue;

                    DataRow currentRow = (row.DataBoundItem as DataRowView).Row;
                    String guid = currentRow["guid"].ToString();

                    if (SelectedGuids.Contains(guid))
                        row.Selected = true;
                }
            }
        }

        internal List<OutputConfigItem> GetConfigItems()
        {
            List<OutputConfigItem> result = new List<OutputConfigItem>();

            foreach (DataRow row in ConfigDataTable.Rows)
            {
                OutputConfigItem cfg = row["settings"] as OutputConfigItem;
                result.Add(cfg);
            }

            return result;
        }

        private void ChangeRowBackgroundColor(int rowIndex, Color color)
        {
            if (rowIndex > -1)
            {
                foreach (DataGridViewCell cell in dataGridViewConfig.Rows[rowIndex].Cells)
                {
                    cell.Style.BackColor = color;
                    cell.Style.Padding = new Padding(0,0,0,0);
                }
            }            
        }

        private void AdjustDragTargetHighlight(int rowIndex)
        {
            var color = Color.LightBlue;
            if (RowCurrentDragHighlight != rowIndex)
            {
                ChangeRowBackgroundColor(RowCurrentDragHighlight, Color.Empty);
                RowCurrentDragHighlight = rowIndex;
                ChangeRowBackgroundColor(RowCurrentDragHighlight, color);
            }

            if (rowIndex > 0 && rowIndex < (dataGridViewConfig.Rows.Count - 1))
            {
                int neighbourRow = IsInCurrentRowTopHalf ? (rowIndex - 1) : (rowIndex + 1);
                if (RowNeighbourDragHighlight != neighbourRow)
                {
                    if (RowNeighbourDragHighlight != RowCurrentDragHighlight)
                    {
                        ChangeRowBackgroundColor(RowNeighbourDragHighlight, Color.Empty);
                    }
                    RowNeighbourDragHighlight = neighbourRow;
                    ChangeRowBackgroundColor(RowNeighbourDragHighlight, color);
                }
            }
        }

        private void RemoveDragTargetHighlight()
        {
            ChangeRowBackgroundColor(RowCurrentDragHighlight, Color.Empty);
            ChangeRowBackgroundColor(RowNeighbourDragHighlight, Color.Empty);
        }

        private void dataGridViewConfig_MouseDown(object sender, MouseEventArgs e)
        {
            RowIndexMouseDown = dataGridViewConfig.HitTest(e.X, e.Y).RowIndex;

            // Handle row "Double-click...." which has no underlying data
            if (RowIndexMouseDown != -1 && (configDataTable.Rows.Count >= (RowIndexMouseDown + 1)))
            {
                Size dragSize = SystemInformation.DragSize;
                dragSize.Width = dragSize.Width * 5;
                dragSize.Height = dragSize.Height * 5;
                Point location = new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2));
                RectangleMouseDown = new Rectangle(location, dragSize);
                DataGridTopLeftPoint = dataGridViewConfig.Location;           
                DataGridBottomRightPoint.X = dataGridViewConfig.Location.X + dataGridViewConfig.Width;
                DataGridBottomRightPoint.Y = dataGridViewConfig.Location.Y + dataGridViewConfig.Height;                
            }
            else
            {
                // Reset if not on datagrid
                RectangleMouseDown = Rectangle.Empty;
            }
        }

        private void dataGridViewConfig_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseButtons.Left == (e.Button & MouseButtons.Left))
            {
                // When mouse did not leave rectangle return, otherwise start drag and drop
                if (RectangleMouseDown == Rectangle.Empty || RectangleMouseDown.Contains(e.X, e.Y)) return;

                if (!IsSortingActive())
                {
                    // Only select Row which is to be moved, needed because of active multiselect
                    dataGridViewConfig.ClearSelection();
                    dataGridViewConfig.Rows[RowIndexMouseDown].Selected = true;
                    dataGridViewConfig.CurrentCell = dataGridViewConfig.Rows[RowIndexMouseDown].Cells["description"];
                    // Start drag and drop
                    dataGridViewConfig.DoDragDrop(configDataTable.Rows[RowIndexMouseDown], DragDropEffects.Move);
                }
                else
                {
                    // Show message box no drag and drop on sorted list
                    MessageBox.Show(i18n._tr("uiMessageDragDropNotAllowed"), i18n._tr("Hint"));
                }
            }
        }

        private void dataGridViewConfig_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
            Point clientPoint = dataGridViewConfig.PointToClient(new Point(e.X, e.Y));
            var hti = dataGridViewConfig.HitTest(clientPoint.X, clientPoint.Y);
            int currentRow = hti.RowIndex;

            if (hti.RowIndex != -1 && hti.ColumnIndex != -1)
            {
                Rectangle myRect = dataGridViewConfig.GetCellDisplayRectangle(hti.ColumnIndex, hti.RowIndex, false);
                IsInCurrentRowTopHalf = clientPoint.Y < (myRect.Top + (myRect.Height / 2));
            }

            if (currentRow > -1)
            {
                AdjustDragTargetHighlight(currentRow);
            }

            // Autoscroll
            int autoScrollMargin = 20;
            int headerHeight = dataGridViewConfig.ColumnHeadersHeight;
            if ((e.Y <= PointToScreen(DataGridTopLeftPoint).Y + headerHeight + autoScrollMargin) &&
                (dataGridViewConfig.FirstDisplayedScrollingRowIndex > 0))
            {
                // Scroll up
                dataGridViewConfig.FirstDisplayedScrollingRowIndex -= 1;
            }
            if (e.Y >= PointToScreen(DataGridBottomRightPoint).Y - autoScrollMargin)
            {
                // Scroll down
                dataGridViewConfig.FirstDisplayedScrollingRowIndex += 1;
            }
        }

        private void dataGridViewConfig_DragDrop(object sender, DragEventArgs e)
        {
            Point clientPoint = dataGridViewConfig.PointToClient(new Point(e.X, e.Y));
            int rowIndexDrop = dataGridViewConfig.HitTest(clientPoint.X, clientPoint.Y).RowIndex;
            if (rowIndexDrop < (dataGridViewConfig.Rows.Count - 1) && !IsInCurrentRowTopHalf)
            {
                rowIndexDrop++;
            }

            // If move operation, remove row at start position and insert at drop position
            if (e.Effect == DragDropEffects.Move && rowIndexDrop != -1)
            {
                DataRow rowToRemove = (DataRow)e.Data.GetData(typeof(DataRow));
                DataRow rowToInsert = configDataTable.NewRow();
                rowToInsert.ItemArray = rowToRemove.ItemArray; // needs to be cloned
                dataGridViewConfig.ClearSelection();
                configDataTable.Rows.InsertAt(rowToInsert, rowIndexDrop);
                EditedItem = null; // safety measure, otherwise on some circumstances exception can be provoked
                configDataTable.Rows.Remove(rowToRemove);
                int newIndex = configDataTable.Rows.IndexOf(rowToInsert);
                dataGridViewConfig.CurrentCell = dataGridViewConfig.Rows[newIndex].Cells["description"];
                // Used to keep the two rows colored for a short period of time after drop
                if (newIndex > 0)
                    RowNeighbourDragHighlight = newIndex - 1;
                if (newIndex < (dataGridViewConfig.Rows.Count - 1))
                    RowCurrentDragHighlight = newIndex + 1;
            }
        }

        private void dataGridViewConfig_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            // Sets the custom cursor based upon the effect.
            e.UseDefaultCursors = false;
            if ((e.Effect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                // For performance reason we want to only
                // calculate the bitmap for the cursor
                // once when it changes from default
                // or from No, which happens after leaving the client area and returning
                if (Cursor.Current == Cursors.Default || Cursor.Current == Cursors.No)
                {
                    int offsetX = CalculateCorrectCursorOffset();

                    // This creates the bitmap of the row and creates assigns it as the new cursor
                    Cursor.Current = CreateCursor(dataGridViewConfig.Rows[RowIndexMouseDown]);

                    // This now corrects the position of the cursor using the offset,
                    // so that the cursor won't show displaced
                    Cursor.Position = new Point(Cursor.Position.X - offsetX, Cursor.Position.Y);
                }                
            }
                
            else
                Cursor.Current = Cursors.Default;
        }

        private int CalculateCorrectCursorOffset()
        {
            // Transform cursor position from screen coords to control coords
            var localCoords = PointToClient(Cursor.Position);

            // Get the size of the row which is equivalent to the cursor bitmap
            Rectangle rowRectangle = dataGridViewConfig.GetRowDisplayRectangle(RowCurrentDragHighlight, true);

            double cursorWidth = rowRectangle.Width;
            // if we grab the row exactly in the center, then the offset is 0
            // if we grab it at the very left, then the offset must be -(half size of cursor)
            // if we grab it at the very right, then the offset must be +(half size of cursor)
            return (int)(localCoords.X - cursorWidth / 2);
        }

        private Cursor CreateCursor(DataGridViewRow row)
        {
            if (CurrentCursorBitmap == null)
            {
                Size clientSize = dataGridViewConfig.ClientSize;
                Rectangle rowRectangle = dataGridViewConfig.GetRowDisplayRectangle(RowIndexMouseDown, true);
                var scalingFactor = GetScalingFactor(this.Handle);

                using (Bitmap dataGridViewBmp = new Bitmap(clientSize.Width, clientSize.Height))
                using (Bitmap rowBmp = new Bitmap(rowRectangle.Width, rowRectangle.Height))
                {
                    dataGridViewConfig.DrawToBitmap(dataGridViewBmp, new Rectangle(Point.Empty, clientSize));
                    using (Graphics G = Graphics.FromImage(rowBmp))
                    {
                        G.DrawImage(dataGridViewBmp, new Rectangle(Point.Empty, rowRectangle.Size), rowRectangle, GraphicsUnit.Pixel);
                    }

                    var scaledX = (int)Math.Round(rowRectangle.Width * scalingFactor, 0);
                    var scaledY = (int)Math.Round(rowRectangle.Height * scalingFactor, 0);

                    CurrentCursorBitmap = new Bitmap(rowBmp, new Size(scaledX, scaledY));
                }
            }
            return new Cursor(CurrentCursorBitmap.GetHicon());
        }

        private double GetScalingFactor(IntPtr handle)
        {
            using (Graphics g = Graphics.FromHwnd(handle))
            {
                // Get the current display scaling factor.
                return DPIUtil.GetWindowsScreenScalingFactor(this, false);
            }
        }

        private void dataGridViewConfig_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.Action == DragAction.Cancel || e.Action == DragAction.Drop)
            {
                if (CurrentCursorBitmap != null)
                {
                    CurrentCursorBitmap.Dispose();
                    CurrentCursorBitmap = null;
                }
                DropTimer.Start();
            }
        }

        private void DropTimer_Tick(object sender, EventArgs e)
        {
            DropTimer.Stop();
            RemoveDragTargetHighlight();
        }
    }
}
