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
using MobiFlight.Base;

namespace MobiFlight.UI.Panels
{
    public partial class InputConfigPanel : UserControl
    {
        public event EventHandler SettingsChanged;
        public event EventHandler SettingsDialogRequested;

        private int lastClickedRow = -1;
        private List<String> SelectedGuids = new List<String>();

        private object[] EditedItem = null;
        public ExecutionManager ExecutionManager { get; set; }
        public DataSet OutputDataSetConfig { get; set; }
        public DataSet InputDataSetConfig { get { return dataSetInputs; } }
        public DataTable ConfigDataTable { get { return inputsDataTable; } }
        public DataGridView DataGridViewConfig { get { return inputsDataGridView; } }

        public InputConfigPanel()
        {
            InitializeComponent();
            Init();
        }

        public DataGridView InputsDataGridView { get { return inputsDataGridView; } }

        void Init()
        {
            Helper.DoubleBufferedDGV(inputsDataGridView, true);

            inputsDataTable.RowChanged += new DataRowChangeEventHandler(configDataTable_RowChanged);
            inputsDataTable.RowDeleted += new DataRowChangeEventHandler(configDataTable_RowChanged);

            inputsDataGridView.Columns["inputDescription"].DefaultCellStyle.NullValue = i18n._tr("uiLabelDoubleClickToAddConfig");
            inputsDataGridView.Columns["inputEditButtonColumn"].DefaultCellStyle.NullValue = "...";
        }

        private void _editConfigWithInputWizard(DataRow dataRow, InputConfigItem cfg, bool create)
        {
            // refactor!!! dependency to arcaze cache etc not nice
            InputConfigWizard wizard = new InputConfigWizard(
                                ExecutionManager,
                                cfg,
#if ARCAZE
                                ExecutionManager.getModuleCache(),
                                ExecutionManager.getModuleCache().GetArcazeModuleSettings(),
#endif
                                OutputDataSetConfig,
                                dataRow["guid"].ToString()
                                );
            wizard.StartPosition = FormStartPosition.CenterParent;
            wizard.SettingsDialogRequested += new EventHandler(wizard_SettingsDialogRequested);
            if (wizard.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (dataRow == null) return;
                // do something special
                // Show used Button
                // Show Type of Output
                // Show last set value
                // do something special
                if (wizard.ConfigHasChanged())
                {
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
        void configDataTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Add || 
                e.Action == DataRowAction.Delete)
                SettingsChanged?.Invoke(sender, null);
        } //configDataTable_RowChanged

        /// <summary>
        /// click event when button in gridview gets clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inputsDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // handle clicks on header cells or row-header cells
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            switch (inputsDataGridView[e.ColumnIndex, e.RowIndex].OwningColumn.Name)
            {
                case "inputEditButtonColumn":
                    bool isNew = inputsDataGridView.Rows[e.RowIndex].IsNewRow;
                    if (isNew)
                    {
                        MessageBox.Show(i18n._tr("uiMessageConfigLineNotSavedYet"),
                                        i18n._tr("Hint"));
                        return;
                    } //if

                    InputConfigItem cfg;
                    bool create = false;
                    DataRow row = (inputsDataGridView.Rows[e.RowIndex].DataBoundItem as DataRowView)?.Row;

                    if (row == null) 
                        break;

                    // the row had been saved but no config object has been created
                    // TODO: move this logic to an appropriate event, e.g. when leaving the gridrow focus of the new row
                    if (row["settings"].GetType() == typeof(System.DBNull))
                    {
                        row["settings"] = new InputConfigItem();
                    }

                    cfg = row["settings"] as InputConfigItem;

                    _editConfigWithInputWizard(
                             row,
                             cfg,
                             create);

                    inputsDataGridView.EndEdit();
                    break;

                case "inputActive":
                    // always end editing to store changes
                    inputsDataGridView.EndEdit();
                    break;
            }
        }

        /// <summary>
        /// when using tab in the grid view, the focus ignores readonly cell and jumps ahead to the next cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inputsDataGridView_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (inputsDataGridView[e.ColumnIndex, e.RowIndex].ReadOnly)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void deleteInputsRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // do somehting here
            foreach (DataGridViewRow row in inputsDataGridView.SelectedRows)
            {
                // we cannot delete a row which hasn't been saved yet
                if (row.IsNewRow) continue;
                inputsDataGridView.Rows.Remove(row);
            }
        }

        private void inputsDataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // handle clicks on header cells or row-header cells
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            switch (inputsDataGridView[e.ColumnIndex, e.RowIndex].OwningColumn.Name)
            {
                case "inputDescription":
                    inputsDataGridView.CurrentCell = inputsDataGridView[e.ColumnIndex, e.RowIndex];
                    inputsDataGridView.BeginEdit(true);
                    break;
            }
        }

        private void duplicateInputsRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // this is called to ensure 
            // that all data has been stored in
            // the data table
            // otherwise there can occur strange inserts of new rows
            // at the first position instead of the expected one
            this.Validate();

            // do somehting here
            foreach (DataGridViewRow row in inputsDataGridView.SelectedRows)
            {
                // ignore new rows since they cannot be copied nor deleted
                if (row.IsNewRow) continue;

                // get current config item
                // duplicate it
                // duplicate row 
                // link to new config item 
                DataRow currentRow = (row.DataBoundItem as DataRowView)?.Row;
                if (currentRow == null) continue;

                DataRow newRow = inputsDataTable.NewRow();

                foreach (DataColumn col in inputsDataTable.Columns)
                {
                    newRow[col.ColumnName] = currentRow[col.ColumnName];
                }

                InputConfigItem cfg = ((row.DataBoundItem as DataRowView).Row["settings"] as InputConfigItem);
                if (cfg != null)
                {
                    cfg = (cfg.Clone() as InputConfigItem);
                }
                else
                {
                    cfg = new InputConfigItem();
                }

                newRow["description"] += " " + i18n._tr("suffixCopy");
                newRow["settings"] = cfg;
                newRow["guid"] = Guid.NewGuid();

                int currentPosition = inputsDataTable.Rows.IndexOf(currentRow);
                if (currentPosition == -1)
                {
                    currentPosition = 1;
                }

                inputsDataTable.Rows.InsertAt(newRow, currentPosition + 1);

                row.Selected = false;
            }
        }


        private void inputsDataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
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


        private void dataGridViewConfig_KeyUp(object sender, KeyEventArgs e)
        {
            DataGridView dgv = (sender as DataGridView);
            int cellIndex = 2;
            if (dgv.Name == inputsDataGridView.Name) cellIndex = 1;

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
                SettingsChanged?.Invoke(sender, null);
            }
            else if (e.KeyCode == Keys.Return)
            {
                // handle clicks on header cells or row-header cells
                if (dgv.CurrentRow.Index < 0 || dgv.CurrentCell.ColumnIndex < 0) return;

                e.Handled = true;
                e.SuppressKeyPress = true;

                if (!dgv.CurrentRow.Cells[cellIndex].IsInEditMode)
                {
                    if (dgv.Name == inputsDataGridView.Name)
                    {
                        dgv.CurrentCell = dgv[cellIndex, dgv.CurrentRow.Index];

                        InputConfigItem cfg;
                        DataRow row = null;
                        bool create = false;

                        if (inputsDataGridView.Rows[dgv.CurrentRow.Index].DataBoundItem == null)
                        {
                            return;
                        }

                        row = (inputsDataGridView.Rows[dgv.CurrentRow.Index].DataBoundItem as DataRowView).Row;

                        // the row had been saved but no config object has been created
                        // TODO: move this logic to an appropriate event, e.g. when leaving the gridrow focus of the new row
                        if (row["settings"].GetType() == typeof(System.DBNull))
                        {
                            row["settings"] = new InputConfigItem();
                        }

                        cfg = row["settings"] as InputConfigItem;

                        _editConfigWithInputWizard(
                                 row,
                                 cfg,
                                 create);
                        inputsDataGridView.EndEdit();
                    }
                }
            }
            else if (e.KeyCode == Keys.V && e.Control)
            {
                // handle clicks on header cells or row-header cells
                if (dgv.CurrentRow.Index < 0 || dgv.CurrentCell.ColumnIndex < 0) return;
                int index = dgv.CurrentRow.Index;

                PasteFromClipboard(index+1);
            }

            else if (e.KeyCode == Keys.C && e.Control)
            {
                // handle clicks on header cells or row-header cells
                if (dgv.CurrentRow.Index < 0 || dgv.CurrentCell.ColumnIndex < 0) return;

                if ((inputsDataGridView.Rows[dgv.CurrentRow.Index].DataBoundItem as DataRowView)?.Row["description"] != null)
                {
                    bool Active = (bool)(inputsDataGridView.Rows[dgv.CurrentRow.Index].DataBoundItem as DataRowView).Row["active"];
                    String Description = (inputsDataGridView.Rows[dgv.CurrentRow.Index].DataBoundItem as DataRowView).Row["description"].ToString();
                    InputConfigItem cfg = ((inputsDataGridView.Rows[dgv.CurrentRow.Index].DataBoundItem as DataRowView).Row["settings"] as InputConfigItem);
                    CopyToClipboard(Active, Description, cfg);
                }
            }
            else
            {
                // do nothing
            }
        }


        private void inputsDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            inputsDataGridView_CellContentClick(sender, e);
        }

        private void inputsDataGridViewConfig_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["inputsGuid"].Value = Guid.NewGuid();
        }

        private void configDataTable_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            e.Row["guid"] = Guid.NewGuid();
        }

        private void configDataTable_RowChanged_1(object sender, DataRowChangeEventArgs e)
        {
            if (e.Row["guid"] == DBNull.Value)
                e.Row["guid"] = Guid.NewGuid();
        }

        /// <summary>
        /// use the settings from the config object and initialize the grid cells 
        /// this is needed after loading and saving configs
        /// </summary>
        public void RestoreValuesInGridView()
        {
            foreach (DataRow row in ConfigDataTable.Rows)
            {
                InputConfigItem cfg = row["settings"] as InputConfigItem;
                row["inputName"] = "-";
                row["inputType"] = "-";
                row["moduleSerial"] = "-";

                if (cfg != null)
                {
                    if (cfg.ModuleSerial == null) continue;
                    var serialNumber = SerialNumber.ExtractSerial(cfg.ModuleSerial);
                    var moduleName = SerialNumber.ExtractDeviceName(cfg.ModuleSerial);
                    row["moduleSerial"] = moduleName;

                    if (cfg.Name=="") continue;

                    // Input shift registers show their name in the grid as the shifter name + configured pin for clarity.
                    if (cfg.Type == InputConfigItem.TYPE_INPUT_SHIFT_REGISTER)
                    {
                        row["inputName"] = $"{cfg.Name}:{cfg.inputShiftRegister.ExtPin}";
                    }
                    else if (cfg.Type == InputConfigItem.TYPE_INPUT_MULTIPLEXER) {
                        row["inputName"] = $"{cfg.Name}:{cfg.inputMultiplexer?.DataPin}";
                    }
                    else if (Joystick.IsJoystickSerial(cfg.ModuleSerial)) {
                        var j = ExecutionManager.GetJoystickManager().GetJoystickBySerial(serialNumber);
                        if (j != null)
                        {
                            row["inputName"] = j.MapDeviceNameToLabel(cfg.Name);
                        } else
                        {
                            row["inputName"] = cfg.Name;
                        }
                    }
                    else if (MidiBoard.IsMidiBoardSerial(cfg.ModuleSerial)) {
                        // Map not by board instance, but by midiboard type. Works also when board is not connected.
                        row["inputName"] = ExecutionManager.GetMidiBoardManager().MapDeviceNameToLabel(moduleName, cfg.Name);
                    }
                    else 
                    {
                        row["inputName"] = cfg.Name;
                    }
                    row["inputType"] = cfg.Type;                   
                }
            }
        } //_restoreValuesInGridView()

        private void inputsDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {           
            DataGridViewRow gridrow = inputsDataGridView.Rows[e.RowIndex];
            DataRowView rowview = (DataRowView)gridrow.DataBoundItem;

            // can be null on creating a new config (last line)
            if (rowview == null) return;

            DataRow row = rowview.Row;
            if (row.RowState==DataRowState.Detached)
                row.Table.Rows.Add(row);

            if (EditedItem != null &&
                (   // this is the checkbox
                    (bool)row.ItemArray[0] != (bool)EditedItem[0] ||
                    // this is the description text
                    row.ItemArray[1] as string != EditedItem[1] as string                    
                )
            )
            {
                SettingsChanged?.Invoke(sender, null);
            }
        }

        private void inputsDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridViewRow gridrow = inputsDataGridView.Rows[e.RowIndex];
            DataRowView rowview = (DataRowView)gridrow.DataBoundItem;

            // can be null on creating a new config (last line)
            if (rowview == null) return;

            if (rowview.Row.ItemArray != null)
                EditedItem = rowview.Row.ItemArray;
        }

        private void dataGridViewConfig_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.ContextMenu = new ContextMenu();
            //e.Control.ContextMenuStrip = inputsDataGridViewContextMenuStrip;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in inputsDataGridView.SelectedRows)
            {
                // ignore new rows since they cannot be copied nor deleted
                if (row.IsNewRow) continue;

                DataRow currentRow = (row.DataBoundItem as DataRowView)?.Row;
                if (currentRow == null) continue;

                bool Active = (bool) currentRow["active"];
                String Description = currentRow["description"] as String;
                InputConfigItem cfg = currentRow["settings"] as InputConfigItem;
                CopyToClipboard(Active, Description, cfg);
                return;
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in inputsDataGridView.SelectedRows)
            {
                int index = row.Index;
                PasteFromClipboard(index+1);
                return;
            }
        }

        private static void CopyToClipboard(bool Active, string Description, InputConfigItem cfg)
        {
            System.Windows.Forms.Clipboard.SetText(Description);
            Clipboard.Instance.InputConfigActive = Active;
            Clipboard.Instance.InputConfigName = Description;

            if (cfg != null)
            {
                Clipboard.Instance.InputConfigItem = cfg;
            }
        }

        private void PasteFromClipboard(int index)
        {
            this.Validate();
            DataRow currentRow = inputsDataTable.NewRow();
            currentRow["guid"] = Guid.NewGuid();
            currentRow["active"] = Clipboard.Instance.InputConfigActive;

            if (Clipboard.Instance.InputConfigName != null)
            {
                currentRow["description"] = Clipboard.Instance.InputConfigName.Clone() as String;
                currentRow["description"] += $" ({i18n._tr("suffixCopy")})";
            }

            if (Clipboard.Instance.InputConfigItem != null)
            {
                InputConfigItem cfg = Clipboard.Instance.InputConfigItem.Clone() as InputConfigItem;
                currentRow["settings"] = cfg;
            }

            if (currentRow.RowState == DataRowState.Detached)
            {
                inputsDataTable.Rows.InsertAt(currentRow, index);
            }

            RestoreValuesInGridView();
        }

        private void inputsDataGridViewContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            bool isNotLastRow = (lastClickedRow != inputsDataGridView.Rows.Count - 1);
            copyToolStripMenuItem.Enabled = isNotLastRow;
            pasteToolStripMenuItem.Enabled = Clipboard.Instance.InputConfigItem != null;
            duplicateInputsRowToolStripMenuItem.Enabled = isNotLastRow;
            deleteInputsRowToolStripMenuItem.Enabled = isNotLastRow;
        }

        private void inputsDataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            lastClickedRow = e.RowIndex;

            if (e.Button == MouseButtons.Right)
            {
                if (inputsDataGridView.IsCurrentCellInEditMode) return;

                inputsDataGridView.EndEdit();

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
            }
            else
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
    }
}
