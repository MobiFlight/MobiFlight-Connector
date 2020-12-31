namespace MobiFlight.UI.Panels
{
    partial class InputConfigPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputConfigPanel));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.inputsDataGridViewContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.duplicateInputsRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteInputsRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataSetInputs = new System.Data.DataSet();
            this.inputsDataTable = new System.Data.DataTable();
            this.inputsActiveDataColumn = new System.Data.DataColumn();
            this.inputsDescriptionDataColumn = new System.Data.DataColumn();
            this.inputsGuidDataColumn = new System.Data.DataColumn();
            this.inputsSettingsDataColumn = new System.Data.DataColumn();
            this.inputsDataGridView = new System.Windows.Forms.DataGridView();
            this.inputActive = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.inputDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.inputsGuid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.inputName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.inputType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.inputEditButtonColumn = new System.Windows.Forms.DataGridViewButtonColumn();
            this.inputsDataGridViewContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetInputs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputsDataTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // inputsDataGridViewContextMenuStrip
            // 
            this.inputsDataGridViewContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.duplicateInputsRowToolStripMenuItem,
            this.deleteInputsRowToolStripMenuItem});
            this.inputsDataGridViewContextMenuStrip.Name = "inputsDataGridViewContextMenuStrip";
            resources.ApplyResources(this.inputsDataGridViewContextMenuStrip, "inputsDataGridViewContextMenuStrip");
            // 
            // duplicateInputsRowToolStripMenuItem
            // 
            this.duplicateInputsRowToolStripMenuItem.Name = "duplicateInputsRowToolStripMenuItem";
            resources.ApplyResources(this.duplicateInputsRowToolStripMenuItem, "duplicateInputsRowToolStripMenuItem");
            this.duplicateInputsRowToolStripMenuItem.Click += new System.EventHandler(this.duplicateInputsRowToolStripMenuItem_Click);
            // 
            // deleteInputsRowToolStripMenuItem
            // 
            this.deleteInputsRowToolStripMenuItem.Name = "deleteInputsRowToolStripMenuItem";
            resources.ApplyResources(this.deleteInputsRowToolStripMenuItem, "deleteInputsRowToolStripMenuItem");
            this.deleteInputsRowToolStripMenuItem.Click += new System.EventHandler(this.deleteInputsRowToolStripMenuItem_Click);
            // 
            // dataSetInputs
            // 
            this.dataSetInputs.DataSetName = "inputs";
            this.dataSetInputs.Tables.AddRange(new System.Data.DataTable[] {
            this.inputsDataTable});
            // 
            // inputsDataTable
            // 
            this.inputsDataTable.Columns.AddRange(new System.Data.DataColumn[] {
            this.inputsActiveDataColumn,
            this.inputsDescriptionDataColumn,
            this.inputsGuidDataColumn,
            this.inputsSettingsDataColumn});
            this.inputsDataTable.TableName = "config";
            this.inputsDataTable.RowChanged += new System.Data.DataRowChangeEventHandler(this.configDataTable_RowChanged_1);
            this.inputsDataTable.TableNewRow += new System.Data.DataTableNewRowEventHandler(this.configDataTable_TableNewRow);
            // 
            // inputsActiveDataColumn
            // 
            this.inputsActiveDataColumn.Caption = "Active";
            this.inputsActiveDataColumn.ColumnName = "active";
            this.inputsActiveDataColumn.DataType = typeof(bool);
            this.inputsActiveDataColumn.DefaultValue = false;
            // 
            // inputsDescriptionDataColumn
            // 
            this.inputsDescriptionDataColumn.AllowDBNull = false;
            this.inputsDescriptionDataColumn.Caption = "Description";
            this.inputsDescriptionDataColumn.ColumnName = "description";
            this.inputsDescriptionDataColumn.DefaultValue = "";
            // 
            // inputsGuidDataColumn
            // 
            this.inputsGuidDataColumn.ColumnMapping = System.Data.MappingType.Attribute;
            this.inputsGuidDataColumn.ColumnName = "guid";
            this.inputsGuidDataColumn.DataType = typeof(System.Guid);
            // 
            // inputsSettingsDataColumn
            // 
            this.inputsSettingsDataColumn.ColumnName = "settings";
            this.inputsSettingsDataColumn.DataType = typeof(object);
            // 
            // inputsDataGridView
            // 
            this.inputsDataGridView.AllowUserToResizeColumns = false;
            this.inputsDataGridView.AllowUserToResizeRows = false;
            this.inputsDataGridView.AutoGenerateColumns = false;
            this.inputsDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            resources.ApplyResources(this.inputsDataGridView, "inputsDataGridView");
            this.inputsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.inputsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.inputActive,
            this.inputDescription,
            this.inputsGuid,
            this.inputName,
            this.inputType,
            this.inputEditButtonColumn});
            this.inputsDataGridView.DataMember = "config";
            this.inputsDataGridView.DataSource = this.dataSetInputs;
            this.inputsDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2;
            this.inputsDataGridView.Name = "inputsDataGridView";
            this.inputsDataGridView.RowTemplate.ContextMenuStrip = this.inputsDataGridViewContextMenuStrip;
            this.inputsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.inputsDataGridView.ShowEditingIcon = false;
            this.inputsDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.inputsDataGridView_CellContentClick);
            this.inputsDataGridView.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.inputsDataGridView_CellContentDoubleClick);
            this.inputsDataGridView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.inputsDataGridView_CellEnter);
            this.inputsDataGridView.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.inputsDataGridView_DataBindingComplete);
            this.inputsDataGridView.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.inputsDataGridViewConfig_DefaultValuesNeeded);
            this.inputsDataGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridViewConfig_KeyUp);
            // 
            // inputActive
            // 
            this.inputActive.DataPropertyName = "active";
            resources.ApplyResources(this.inputActive, "inputActive");
            this.inputActive.Name = "inputActive";
            this.inputActive.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // inputDescription
            // 
            this.inputDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.inputDescription.DataPropertyName = "description";
            resources.ApplyResources(this.inputDescription, "inputDescription");
            this.inputDescription.Name = "inputDescription";
            // 
            // inputsGuid
            // 
            this.inputsGuid.DataPropertyName = "guid";
            resources.ApplyResources(this.inputsGuid, "inputsGuid");
            this.inputsGuid.Name = "inputsGuid";
            this.inputsGuid.ReadOnly = true;
            // 
            // inputName
            // 
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.ControlLight;
            this.inputName.DefaultCellStyle = dataGridViewCellStyle4;
            resources.ApplyResources(this.inputName, "inputName");
            this.inputName.Name = "inputName";
            this.inputName.ReadOnly = true;
            this.inputName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.inputName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // inputType
            // 
            this.inputType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.ControlLight;
            this.inputType.DefaultCellStyle = dataGridViewCellStyle5;
            resources.ApplyResources(this.inputType, "inputType");
            this.inputType.Name = "inputType";
            this.inputType.ReadOnly = true;
            this.inputType.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // inputEditButtonColumn
            // 
            this.inputEditButtonColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.ControlLight;
            dataGridViewCellStyle6.NullValue = "...";
            this.inputEditButtonColumn.DefaultCellStyle = dataGridViewCellStyle6;
            resources.ApplyResources(this.inputEditButtonColumn, "inputEditButtonColumn");
            this.inputEditButtonColumn.Name = "inputEditButtonColumn";
            this.inputEditButtonColumn.Text = "...";
            this.inputEditButtonColumn.UseColumnTextForButtonValue = true;
            // 
            // InputConfigPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.inputsDataGridView);
            this.Name = "InputConfigPanel";
            this.inputsDataGridViewContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataSetInputs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputsDataTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputsDataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip inputsDataGridViewContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem duplicateInputsRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteInputsRowToolStripMenuItem;
        private System.Data.DataSet dataSetInputs;
        private System.Data.DataTable inputsDataTable;
        private System.Data.DataColumn inputsActiveDataColumn;
        private System.Data.DataColumn inputsDescriptionDataColumn;
        private System.Data.DataColumn inputsGuidDataColumn;
        private System.Data.DataColumn inputsSettingsDataColumn;
        private System.Windows.Forms.DataGridView inputsDataGridView;
        private System.Windows.Forms.DataGridViewCheckBoxColumn inputActive;
        private System.Windows.Forms.DataGridViewTextBoxColumn inputDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn inputsGuid;
        private System.Windows.Forms.DataGridViewTextBoxColumn inputName;
        private System.Windows.Forms.DataGridViewTextBoxColumn inputType;
        private System.Windows.Forms.DataGridViewButtonColumn inputEditButtonColumn;
    }
}
