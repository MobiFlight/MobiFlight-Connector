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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.inputsDataGridViewContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.duplicateInputsRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteInputsRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataSetInputs = new System.Data.DataSet();
            this.inputsDataTable = new System.Data.DataTable();
            this.inputsActiveDataColumn = new System.Data.DataColumn();
            this.inputsDescriptionDataColumn = new System.Data.DataColumn();
            this.inputsGuidDataColumn = new System.Data.DataColumn();
            this.inputsSettingsDataColumn = new System.Data.DataColumn();
            this.inputNameDataColumn = new System.Data.DataColumn();
            this.inputTypeDataColumn = new System.Data.DataColumn();
            this.moduleSerialDataColumn = new System.Data.DataColumn();
            this.inputsDataGridView = new System.Windows.Forms.DataGridView();
            this.inputActive = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.inputDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.inputsGuid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.moduleSerial = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripMenuItem1,
            this.duplicateInputsRowToolStripMenuItem,
            this.deleteInputsRowToolStripMenuItem});
            this.inputsDataGridViewContextMenuStrip.Name = "inputsDataGridViewContextMenuStrip";
            resources.ApplyResources(this.inputsDataGridViewContextMenuStrip, "inputsDataGridViewContextMenuStrip");
            this.inputsDataGridViewContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.inputsDataGridViewContextMenuStrip_Opening);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            resources.ApplyResources(this.pasteToolStripMenuItem, "pasteToolStripMenuItem");
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
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
            this.inputsSettingsDataColumn,
            this.inputNameDataColumn,
            this.inputTypeDataColumn,
            this.moduleSerialDataColumn});
            this.inputsDataTable.TableName = "config";
            this.inputsDataTable.RowChanged += new System.Data.DataRowChangeEventHandler(this.configDataTable_RowChanged_1);
            this.inputsDataTable.TableNewRow += new System.Data.DataTableNewRowEventHandler(this.configDataTable_TableNewRow);
            // 
            // inputsActiveDataColumn
            // 
            this.inputsActiveDataColumn.Caption = "Active";
            this.inputsActiveDataColumn.ColumnName = "active";
            this.inputsActiveDataColumn.DataType = typeof(bool);
            this.inputsActiveDataColumn.DefaultValue = true;
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
            // inputNameDataColumn
            // 
            this.inputNameDataColumn.ColumnMapping = System.Data.MappingType.Hidden;
            this.inputNameDataColumn.ColumnName = "inputName";
            // 
            // inputTypeDataColumn
            // 
            this.inputTypeDataColumn.ColumnMapping = System.Data.MappingType.Hidden;
            this.inputTypeDataColumn.ColumnName = "inputType";
            // 
            // moduleSerialDataColumn
            // 
            this.moduleSerialDataColumn.ColumnMapping = System.Data.MappingType.Hidden;
            this.moduleSerialDataColumn.ColumnName = "moduleSerial";
            // 
            // inputsDataGridView
            // 
            this.inputsDataGridView.AllowUserToResizeRows = false;
            this.inputsDataGridView.AutoGenerateColumns = false;
            this.inputsDataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
            this.inputsDataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            resources.ApplyResources(this.inputsDataGridView, "inputsDataGridView");
            this.inputsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.inputsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.inputActive,
            this.inputDescription,
            this.inputsGuid,
            this.moduleSerial,
            this.inputName,
            this.inputType,
            this.inputEditButtonColumn});
            this.inputsDataGridView.ContextMenuStrip = this.inputsDataGridViewContextMenuStrip;
            this.inputsDataGridView.DataMember = "config";
            this.inputsDataGridView.DataSource = this.dataSetInputs;
            this.inputsDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2;
            this.inputsDataGridView.Name = "inputsDataGridView";
            this.inputsDataGridView.RowTemplate.DefaultCellStyle.Padding = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.inputsDataGridView.RowTemplate.Height = 26;
            this.inputsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.inputsDataGridView.ShowEditingIcon = false;
            this.inputsDataGridView.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.inputsDataGridView_CellBeginEdit);
            this.inputsDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.inputsDataGridView_CellContentClick);
            this.inputsDataGridView.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.inputsDataGridView_CellContentDoubleClick);
            this.inputsDataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.inputsDataGridView_CellEndEdit);
            this.inputsDataGridView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.inputsDataGridView_CellEnter);
            this.inputsDataGridView.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.inputsDataGridView_CellMouseDown);
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
            this.inputDescription.FillWeight = 1000F;
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
            // moduleSerial
            // 
            this.moduleSerial.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.moduleSerial.DataPropertyName = "moduleSerial";
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.moduleSerial.DefaultCellStyle = dataGridViewCellStyle1;
            this.moduleSerial.FillWeight = 150F;
            resources.ApplyResources(this.moduleSerial, "moduleSerial");
            this.moduleSerial.Name = "moduleSerial";
            this.moduleSerial.ReadOnly = true;
            this.moduleSerial.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // inputName
            // 
            this.inputName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.inputName.DataPropertyName = "inputName";
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.ControlLight;
            this.inputName.DefaultCellStyle = dataGridViewCellStyle2;
            this.inputName.FillWeight = 150F;
            resources.ApplyResources(this.inputName, "inputName");
            this.inputName.Name = "inputName";
            this.inputName.ReadOnly = true;
            this.inputName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // inputType
            // 
            this.inputType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.inputType.DataPropertyName = "inputType";
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.ControlLight;
            this.inputType.DefaultCellStyle = dataGridViewCellStyle3;
            this.inputType.FillWeight = 150F;
            resources.ApplyResources(this.inputType, "inputType");
            this.inputType.Name = "inputType";
            this.inputType.ReadOnly = true;
            // 
            // inputEditButtonColumn
            // 
            this.inputEditButtonColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.ControlLight;
            dataGridViewCellStyle4.NullValue = "...";
            this.inputEditButtonColumn.DefaultCellStyle = dataGridViewCellStyle4;
            resources.ApplyResources(this.inputEditButtonColumn, "inputEditButtonColumn");
            this.inputEditButtonColumn.Name = "inputEditButtonColumn";
            this.inputEditButtonColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.inputEditButtonColumn.Text = "...";
            this.inputEditButtonColumn.UseColumnTextForButtonValue = true;
            // 
            // InputConfigPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.inputsDataGridView);
            this.DoubleBuffered = true;
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
        private System.Data.DataColumn inputNameDataColumn;
        private System.Data.DataColumn inputTypeDataColumn;
        private System.Data.DataColumn moduleSerialDataColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn inputActive;
        private System.Windows.Forms.DataGridViewTextBoxColumn inputDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn inputsGuid;
        private System.Windows.Forms.DataGridViewTextBoxColumn moduleSerial;
        private System.Windows.Forms.DataGridViewTextBoxColumn inputName;
        private System.Windows.Forms.DataGridViewTextBoxColumn inputType;
        private System.Windows.Forms.DataGridViewButtonColumn inputEditButtonColumn;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
    }
}
