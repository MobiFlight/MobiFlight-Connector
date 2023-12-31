using MobiFlight.Base;
using MobiFlight.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels
{
    public partial class InputConfigPanel : UserControl
    {
        public event EventHandler SettingsChanged;
        public event EventHandler SettingsDialogRequested;

        private int lastClickedRow = -1;
        private List<String> SelectedGuids = new List<String>();
    
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
            inputsDataGridView.DataMember = null;
            inputsDataGridView.DataSource = inputsDataTable;
            Helper.DoubleBufferedDGV(inputsDataGridView, true);

            inputsDataTable.RowChanged += new DataRowChangeEventHandler(configDataTable_RowChanged);
            inputsDataTable.RowDeleted += new DataRowChangeEventHandler(configDataTable_RowChanged);
            inputsDataTable.TableCleared += new DataTableClearEventHandler((o, a) => { SettingsChanged?.Invoke(this, null); });

            inputsDataGridView.Columns["inputDescription"].DefaultCellStyle.NullValue = i18n._tr("uiLabelDoubleClickToAddConfig");
            inputsDataGridView.Columns["inputEditButtonColumn"].DefaultCellStyle.NullValue = "...";

            DropTimer.Interval = 400;
            DropTimer.Tick += DropTimer_Tick;
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

                if (wizard.ConfigHasChanged())
                {
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
        
        /// <summary>
        /// Is sorting in DataGridView active.
        /// </summary>        
        public bool IsSortingActive()
        {
            return inputsDataGridView.SortOrder != SortOrder.None;
        }

        /// <summary>
        /// Reset sorting in DataGridView
        /// </summary>
        public void ResetSorting()
        {
            LastSortingColumnName = string.Empty;
            LastSortingOrder = SortOrder.None;
            inputsDataTable.DefaultView.Sort = string.Empty;
        }

        private void inputsDataGridView_Sorted(object sender, EventArgs e)
        {
            if (IsSortingActive())
            {
                string name = inputsDataGridView.SortedColumn.Name;
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
                    LastSortingOrder = InputsDataGridView.SortOrder;
                }
            }
        }

        private void inputsDataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        { 
            if (e.ListChangedType == ListChangedType.Reset)
            {
                inputsDataGridView.ClearSelection(); // necessary because on sorting reset, callback is called twice
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

        private void ActivateAutoColumnWidth()
        {       
            inputsDataGridView.Columns["moduleSerial"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            inputsDataGridView.Columns["inputName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            inputsDataGridView.Columns["inputType"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }

        private void DeactivateAutoColumnWidth()
        {
            // Deactivate and adjust FillWeights
            string[] columnNames = { "moduleSerial", "inputName", "inputType" };

            // inputDescription has FillWeight 1000
            var descColumn = inputsDataGridView.Columns["inputDescription"];
            foreach (string name in columnNames)
            {
                int currentWidth = inputsDataGridView.Columns[name].Width;
                inputsDataGridView.Columns[name].FillWeight = (float)currentWidth / (float)descColumn.Width * 1450;
                inputsDataGridView.Columns[name].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
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
            // Auto adjust column width
            ActivateAutoColumnWidth();

            // Deactivate again, to make them resizable
            DeactivateAutoColumnWidth();
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

        internal List<InputConfigItem> GetConfigItems()
        {
            List<InputConfigItem> result = new List<InputConfigItem>();

            foreach (DataRow row in ConfigDataTable.Rows)
            {
                InputConfigItem cfg = row["settings"] as InputConfigItem;
                result.Add(cfg);
            }

            return result;
        }

        private void ChangeRowBackgroundColor(int rowIndex, Color color)
        {
            if (rowIndex > -1)
            {
                foreach (DataGridViewCell cell in inputsDataGridView.Rows[rowIndex].Cells)
                {
                    cell.Style.BackColor = color;
                    cell.Style.Padding = new Padding(0, 0, 0, 0);
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

            if (rowIndex > 0 && rowIndex < (inputsDataGridView.Rows.Count - 1))
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

        private void inputsDataGridView_MouseDown(object sender, MouseEventArgs e)
        {
            RowIndexMouseDown = inputsDataGridView.HitTest(e.X, e.Y).RowIndex;

            // Handle row "Double-click...." which has no underlying data
            if (RowIndexMouseDown != -1 && (inputsDataTable.Rows.Count >= (RowIndexMouseDown + 1)))
            {
                Size dragSize = SystemInformation.DragSize;
                dragSize.Width = dragSize.Width * 5;
                dragSize.Height = dragSize.Height * 5;               
                Point location = new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2));
                RectangleMouseDown = new Rectangle(location, dragSize);
                DataGridTopLeftPoint.X = inputsDataGridView.Location.X;
                DataGridTopLeftPoint.Y = inputsDataGridView.Location.Y;
                DataGridBottomRightPoint.X = inputsDataGridView.Location.X + inputsDataGridView.Width;
                DataGridBottomRightPoint.Y = inputsDataGridView.Location.Y + inputsDataGridView.Height;
            }
            else
            {
                // Reset if not on datagrid
                RectangleMouseDown = Rectangle.Empty;
            }
        }

        private void inputsDataGridView_MouseMove(object sender, MouseEventArgs e)
        {
            if (MouseButtons.Left == (e.Button & MouseButtons.Left))
            {
                // When mouse did not leave rectangle return, otherwise start drag and drop
                if (RectangleMouseDown == Rectangle.Empty || RectangleMouseDown.Contains(e.X, e.Y)) return;

                if (!IsSortingActive())
                {                    
                    // Only select Row which is to be moved, needed because of active multiselect
                    inputsDataGridView.ClearSelection();
                    inputsDataGridView.Rows[RowIndexMouseDown].Selected = true;
                    inputsDataGridView.CurrentCell = inputsDataGridView.Rows[RowIndexMouseDown].Cells["inputDescription"];
                    // Start drag and drop
                    inputsDataGridView.DoDragDrop(inputsDataTable.Rows[RowIndexMouseDown], DragDropEffects.Move);           
                }
                else
                {
                    // Show message box no drag and drop on sorted list
                    MessageBox.Show(i18n._tr("uiMessageDragDropNotAllowed"), i18n._tr("Hint"));
                }
            }
        }

        private void inputsDataGridView_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
            Point clientPoint = inputsDataGridView.PointToClient(new Point(e.X, e.Y));
            var hti = inputsDataGridView.HitTest(clientPoint.X, clientPoint.Y);
            int currentRow = hti.RowIndex;

            if (hti.RowIndex != -1 && hti.ColumnIndex != -1)
            {
                Rectangle myRect = inputsDataGridView.GetCellDisplayRectangle(hti.ColumnIndex, hti.RowIndex, false);
                IsInCurrentRowTopHalf = clientPoint.Y < (myRect.Top + (myRect.Height / 2));
            }

            if (currentRow > -1)
            {
                AdjustDragTargetHighlight(currentRow);
            }

            // Autoscroll
            int autoScrollMargin = 20;
            int headerHeight = inputsDataGridView.ColumnHeadersHeight;
            if ((e.Y <= PointToScreen(DataGridTopLeftPoint).Y + headerHeight + autoScrollMargin) &&
                (inputsDataGridView.FirstDisplayedScrollingRowIndex > 0))
            {
                // Scroll up
                inputsDataGridView.FirstDisplayedScrollingRowIndex -= 1;
            }
            if (e.Y >= PointToScreen(DataGridBottomRightPoint).Y - autoScrollMargin)
            {
                // Scroll down
                inputsDataGridView.FirstDisplayedScrollingRowIndex += 1;
            }
        }

        private void inputsDataGridView_DragDrop(object sender, DragEventArgs e)
        {
            Point clientPoint = inputsDataGridView.PointToClient(new Point(e.X, e.Y));
            int rowIndexDrop = inputsDataGridView.HitTest(clientPoint.X, clientPoint.Y).RowIndex;
            if (rowIndexDrop < (inputsDataGridView.Rows.Count - 1) && !IsInCurrentRowTopHalf)
            {
                rowIndexDrop++;
            }

            // If move operation, remove row at start position and insert at drop position
            if (e.Effect == DragDropEffects.Move && rowIndexDrop != -1)
            {
                DataRow rowToRemove = (DataRow)e.Data.GetData(typeof(DataRow));
                DataRow rowToInsert = inputsDataTable.NewRow();
                rowToInsert.ItemArray = rowToRemove.ItemArray; // needs to be cloned
                inputsDataGridView.ClearSelection();
                inputsDataTable.Rows.InsertAt(rowToInsert, rowIndexDrop);
                EditedItem = null; // safety measure, otherwise on some circumstances exception can be provoked
                inputsDataTable.Rows.Remove(rowToRemove);
                int newIndex = inputsDataTable.Rows.IndexOf(rowToInsert);
                inputsDataGridView.CurrentCell = inputsDataGridView.Rows[newIndex].Cells["inputDescription"];
                // Used to keep the two rows colored for a short period of time after drop
                if (newIndex > 0)
                    RowNeighbourDragHighlight = newIndex - 1;
                if (newIndex < (inputsDataGridView.Rows.Count - 1))
                    RowCurrentDragHighlight = newIndex + 1;
            }
        }

        private void inputsDataGridView_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            // Sets the custom cursor based upon the effect.
            e.UseDefaultCursors = false;
            if ((e.Effect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                // For performance reason we want to only
                // calculate the bitmap for the cursor
                // once when it changes from default
                if (Cursor.Current == Cursors.Default || Cursor.Current == Cursors.No)
                {
                    int offsetX = CalculateCorrectCursorOffset();

                    // This creates the bitmap of the row and creates assigns it as the new cursor
                    Cursor.Current = CreateCursor(inputsDataGridView.Rows[RowIndexMouseDown]);

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
            Rectangle rowRectangle = inputsDataGridView.GetRowDisplayRectangle(RowCurrentDragHighlight, true);

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
                Size clientSize = inputsDataGridView.ClientSize;
                Rectangle rowRectangle = inputsDataGridView.GetRowDisplayRectangle(RowIndexMouseDown, true);
                var scalingFactor = GetScalingFactor(this.Handle);

                using (Bitmap dataGridViewBmp = new Bitmap(clientSize.Width, clientSize.Height))
                using (Bitmap rowBmp = new Bitmap(rowRectangle.Width, rowRectangle.Height))
                {
                    inputsDataGridView.DrawToBitmap(dataGridViewBmp, new Rectangle(Point.Empty, clientSize));
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

        private void inputsDataGridView_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
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
