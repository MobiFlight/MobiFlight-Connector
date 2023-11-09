namespace MobiFlight.UI.Panels
{
    partial class OutputConfigPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OutputConfigPanel));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewConfig = new System.Windows.Forms.DataGridView();
            this.active = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.guid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.moduleSerial = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OutputName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OutputType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FsuipcOffset = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fsuipcValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.arcazeValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EditButtonColumn = new System.Windows.Forms.DataGridViewButtonColumn();
            this.dataGridViewContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.duplicateRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataSetConfig = new System.Data.DataSet();
            this.configDataTable = new System.Data.DataTable();
            this.activeDataColumn = new System.Data.DataColumn();
            this.fsuipcOffsetDataColumn = new System.Data.DataColumn();
            this.converterDataColumn = new System.Data.DataColumn();
            this.maskDataColumn = new System.Data.DataColumn();
            this.usbArcazePinDataColumn = new System.Data.DataColumn();
            this.typeDataColumn = new System.Data.DataColumn();
            this.durationDataColumn = new System.Data.DataColumn();
            this.comparisonDataColumn = new System.Data.DataColumn();
            this.comparisonValueDataColumn = new System.Data.DataColumn();
            this.descriptionDataColumn = new System.Data.DataColumn();
            this.fsuipcSizeDataColumn = new System.Data.DataColumn();
            this.triggerDataColumn = new System.Data.DataColumn();
            this.arcazeSerialDataColumn = new System.Data.DataColumn();
            this.settingsColumn = new System.Data.DataColumn();
            this.guidDataColumn = new System.Data.DataColumn();
            this.outputDataColumn = new System.Data.DataColumn();
            this.outputTypeDataColumn = new System.Data.DataColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewConfig)).BeginInit();
            this.dataGridViewContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetConfig)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.configDataTable)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewConfig
            // 
            resources.ApplyResources(this.dataGridViewConfig, "dataGridViewConfig");
            this.dataGridViewConfig.AllowUserToResizeRows = false;
            this.dataGridViewConfig.AutoGenerateColumns = false;
            this.dataGridViewConfig.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewConfig.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewConfig.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewConfig.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewConfig.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewConfig.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.active,
            this.guid,
            this.Description,
            this.moduleSerial,
            this.OutputName,
            this.OutputType,
            this.FsuipcOffset,
            this.fsuipcValueColumn,
            this.arcazeValueColumn,
            this.EditButtonColumn});
            this.dataGridViewConfig.ContextMenuStrip = this.dataGridViewContextMenuStrip;
            this.dataGridViewConfig.DataMember = "config";
            this.dataGridViewConfig.DataSource = this.dataSetConfig;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewConfig.DefaultCellStyle = dataGridViewCellStyle10;
            this.dataGridViewConfig.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2;
            this.dataGridViewConfig.Name = "dataGridViewConfig";
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewConfig.RowHeadersDefaultCellStyle = dataGridViewCellStyle11;
            this.dataGridViewConfig.RowTemplate.DefaultCellStyle.Padding = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.dataGridViewConfig.RowTemplate.Height = 26;
            this.dataGridViewConfig.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewConfig.ShowEditingIcon = false;
            this.dataGridViewConfig.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridViewConfig_CellBeginEdit);
            this.dataGridViewConfig.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewConfig_CellContentClick);
            this.dataGridViewConfig.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewConfig_CellContentDoubleClick);
            this.dataGridViewConfig.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewConfig_CellDoubleClick);
            this.dataGridViewConfig.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewConfig_CellEndEdit);
            this.dataGridViewConfig.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewConfig_CellEnter);
            this.dataGridViewConfig.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridViewConfig_CellMouseDown);
            this.dataGridViewConfig.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewConfig_CellValidated);
            this.dataGridViewConfig.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.DataGridViewConfig_CellValidating);
            this.dataGridViewConfig.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dataGridViewConfig_DataBindingComplete);
            this.dataGridViewConfig.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.DataGridViewConfig_DefaultValuesNeeded);
            this.dataGridViewConfig.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridViewConfig_EditingControlShowing);
            this.dataGridViewConfig.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DataGridViewConfig_KeyUp);
            // 
            // active
            // 
            this.active.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.active.DataPropertyName = "active";
            this.active.FillWeight = 50F;
            resources.ApplyResources(this.active, "active");
            this.active.Name = "active";
            // 
            // guid
            // 
            this.guid.DataPropertyName = "guid";
            resources.ApplyResources(this.guid, "guid");
            this.guid.Name = "guid";
            this.guid.ReadOnly = true;
            // 
            // Description
            // 
            this.Description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Description.DataPropertyName = "description";
            dataGridViewCellStyle2.NullValue = "Doppelklicken für neuen Eintrag...";
            this.Description.DefaultCellStyle = dataGridViewCellStyle2;
            this.Description.FillWeight = 1000F;
            resources.ApplyResources(this.Description, "Description");
            this.Description.Name = "Description";
            // 
            // moduleSerial
            // 
            this.moduleSerial.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.moduleSerial.DataPropertyName = "arcazeSerial";
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.ControlLight;
            this.moduleSerial.DefaultCellStyle = dataGridViewCellStyle3;
            this.moduleSerial.FillWeight = 150F;
            resources.ApplyResources(this.moduleSerial, "moduleSerial");
            this.moduleSerial.Name = "moduleSerial";
            this.moduleSerial.ReadOnly = true;
            this.moduleSerial.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // OutputName
            // 
            this.OutputName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.OutputName.DataPropertyName = "OutputName";
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.ControlLight;
            this.OutputName.DefaultCellStyle = dataGridViewCellStyle4;
            this.OutputName.FillWeight = 150F;
            resources.ApplyResources(this.OutputName, "OutputName");
            this.OutputName.Name = "OutputName";
            this.OutputName.ReadOnly = true;
            // 
            // OutputType
            // 
            this.OutputType.DataPropertyName = "OutputType";
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.ControlLight;
            this.OutputType.DefaultCellStyle = dataGridViewCellStyle5;
            resources.ApplyResources(this.OutputType, "OutputType");
            this.OutputType.Name = "OutputType";
            this.OutputType.ReadOnly = true;
            // 
            // FsuipcOffset
            // 
            this.FsuipcOffset.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.FsuipcOffset.DataPropertyName = "fsuipcOffset";
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.ControlLight;
            this.FsuipcOffset.DefaultCellStyle = dataGridViewCellStyle6;
            resources.ApplyResources(this.FsuipcOffset, "FsuipcOffset");
            this.FsuipcOffset.Name = "FsuipcOffset";
            this.FsuipcOffset.ReadOnly = true;
            // 
            // fsuipcValueColumn
            // 
            this.fsuipcValueColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.ControlLight;
            this.fsuipcValueColumn.DefaultCellStyle = dataGridViewCellStyle7;
            this.fsuipcValueColumn.FillWeight = 150F;
            resources.ApplyResources(this.fsuipcValueColumn, "fsuipcValueColumn");
            this.fsuipcValueColumn.Name = "fsuipcValueColumn";
            this.fsuipcValueColumn.ReadOnly = true;
            this.fsuipcValueColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.fsuipcValueColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // arcazeValueColumn
            // 
            this.arcazeValueColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.ControlLight;
            this.arcazeValueColumn.DefaultCellStyle = dataGridViewCellStyle8;
            this.arcazeValueColumn.FillWeight = 150F;
            resources.ApplyResources(this.arcazeValueColumn, "arcazeValueColumn");
            this.arcazeValueColumn.Name = "arcazeValueColumn";
            this.arcazeValueColumn.ReadOnly = true;
            this.arcazeValueColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // EditButtonColumn
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.ControlLight;
            dataGridViewCellStyle9.NullValue = "...";
            this.EditButtonColumn.DefaultCellStyle = dataGridViewCellStyle9;
            resources.ApplyResources(this.EditButtonColumn, "EditButtonColumn");
            this.EditButtonColumn.Name = "EditButtonColumn";
            this.EditButtonColumn.Text = "...";
            this.EditButtonColumn.UseColumnTextForButtonValue = true;
            // 
            // dataGridViewContextMenuStrip
            // 
            resources.ApplyResources(this.dataGridViewContextMenuStrip, "dataGridViewContextMenuStrip");
            this.dataGridViewContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripMenuItem1,
            this.duplicateRowToolStripMenuItem,
            this.deleteRowToolStripMenuItem});
            this.dataGridViewContextMenuStrip.Name = "dataGridViewContextMenuStrip";
            this.dataGridViewContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.DataGridViewContextMenuStrip_Opening);
            // 
            // copyToolStripMenuItem
            // 
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            resources.ApplyResources(this.pasteToolStripMenuItem, "pasteToolStripMenuItem");
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            // 
            // duplicateRowToolStripMenuItem
            // 
            resources.ApplyResources(this.duplicateRowToolStripMenuItem, "duplicateRowToolStripMenuItem");
            this.duplicateRowToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.duplicateRowToolStripMenuItem.Image = global::MobiFlight.Properties.Resources.star_yellow_new;
            this.duplicateRowToolStripMenuItem.Name = "duplicateRowToolStripMenuItem";
            this.duplicateRowToolStripMenuItem.Click += new System.EventHandler(this.DuplicateRowToolStripMenuItem_Click);
            // 
            // deleteRowToolStripMenuItem
            // 
            resources.ApplyResources(this.deleteRowToolStripMenuItem, "deleteRowToolStripMenuItem");
            this.deleteRowToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.deleteRowToolStripMenuItem.Image = global::MobiFlight.Properties.Resources.delete2;
            this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
            this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.DeleteRowToolStripMenuItem_Click);
            // 
            // dataSetConfig
            // 
            this.dataSetConfig.DataSetName = "outputs";
            this.dataSetConfig.Tables.AddRange(new System.Data.DataTable[] {
            this.configDataTable});
            // 
            // configDataTable
            // 
            this.configDataTable.Columns.AddRange(new System.Data.DataColumn[] {
            this.activeDataColumn,
            this.fsuipcOffsetDataColumn,
            this.converterDataColumn,
            this.maskDataColumn,
            this.usbArcazePinDataColumn,
            this.typeDataColumn,
            this.durationDataColumn,
            this.comparisonDataColumn,
            this.comparisonValueDataColumn,
            this.descriptionDataColumn,
            this.fsuipcSizeDataColumn,
            this.triggerDataColumn,
            this.arcazeSerialDataColumn,
            this.settingsColumn,
            this.guidDataColumn,
            this.outputDataColumn,
            this.outputTypeDataColumn});
            this.configDataTable.TableName = "config";
            this.configDataTable.RowChanged += new System.Data.DataRowChangeEventHandler(this.ConfigDataTable_RowChanged_1);
            this.configDataTable.TableNewRow += new System.Data.DataTableNewRowEventHandler(this.ConfigDataTable_TableNewRow);
            // 
            // activeDataColumn
            // 
            this.activeDataColumn.Caption = "Active";
            this.activeDataColumn.ColumnName = "active";
            this.activeDataColumn.DataType = typeof(bool);
            this.activeDataColumn.DefaultValue = true;
            // 
            // fsuipcOffsetDataColumn
            // 
            this.fsuipcOffsetDataColumn.Caption = "FsuipcOffset";
            this.fsuipcOffsetDataColumn.ColumnName = "fsuipcOffset";
            // 
            // converterDataColumn
            // 
            this.converterDataColumn.Caption = "Converter";
            this.converterDataColumn.ColumnName = "converter";
            this.converterDataColumn.DefaultValue = "Boolean";
            // 
            // maskDataColumn
            // 
            this.maskDataColumn.Caption = "Mask";
            this.maskDataColumn.ColumnName = "mask";
            // 
            // usbArcazePinDataColumn
            // 
            this.usbArcazePinDataColumn.Caption = "USBArcazePin";
            this.usbArcazePinDataColumn.ColumnName = "usbArcazePin";
            // 
            // typeDataColumn
            // 
            this.typeDataColumn.Caption = "Type";
            this.typeDataColumn.ColumnName = "type";
            // 
            // durationDataColumn
            // 
            this.durationDataColumn.Caption = "Duration";
            this.durationDataColumn.ColumnName = "duration";
            // 
            // comparisonDataColumn
            // 
            this.comparisonDataColumn.Caption = "Comparison";
            this.comparisonDataColumn.ColumnName = "comparison";
            this.comparisonDataColumn.DefaultValue = "=";
            // 
            // comparisonValueDataColumn
            // 
            this.comparisonValueDataColumn.Caption = "ComparisonValue";
            this.comparisonValueDataColumn.ColumnName = "comparisonValue";
            // 
            // descriptionDataColumn
            // 
            this.descriptionDataColumn.AllowDBNull = false;
            this.descriptionDataColumn.Caption = "Description";
            this.descriptionDataColumn.ColumnName = "description";
            this.descriptionDataColumn.DefaultValue = "";
            // 
            // fsuipcSizeDataColumn
            // 
            this.fsuipcSizeDataColumn.Caption = "Fsuipc Size";
            this.fsuipcSizeDataColumn.ColumnName = "fsuipcSize";
            this.fsuipcSizeDataColumn.DefaultValue = "1";
            this.fsuipcSizeDataColumn.MaxLength = 3;
            // 
            // triggerDataColumn
            // 
            this.triggerDataColumn.ColumnName = "trigger";
            this.triggerDataColumn.DefaultValue = "change";
            // 
            // arcazeSerialDataColumn
            // 
            this.arcazeSerialDataColumn.ColumnName = "arcazeSerial";
            this.arcazeSerialDataColumn.DefaultValue = "";
            // 
            // settingsColumn
            // 
            this.settingsColumn.Caption = "settings";
            this.settingsColumn.ColumnName = "settings";
            this.settingsColumn.DataType = typeof(object);
            // 
            // guidDataColumn
            // 
            this.guidDataColumn.ColumnMapping = System.Data.MappingType.Attribute;
            this.guidDataColumn.ColumnName = "guid";
            this.guidDataColumn.DataType = typeof(System.Guid);
            // 
            // outputDataColumn
            // 
            this.outputDataColumn.Caption = "Output";
            this.outputDataColumn.ColumnMapping = System.Data.MappingType.Hidden;
            this.outputDataColumn.ColumnName = "OutputName";
            // 
            // outputTypeDataColumn
            // 
            this.outputTypeDataColumn.Caption = "Type";
            this.outputTypeDataColumn.ColumnMapping = System.Data.MappingType.Hidden;
            this.outputTypeDataColumn.ColumnName = "OutputType";
            // 
            // OutputConfigPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridViewConfig);
            this.DoubleBuffered = true;
            this.Name = "OutputConfigPanel";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewConfig)).EndInit();
            this.dataGridViewContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataSetConfig)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.configDataTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.DataGridView dataGridViewConfig;
        private System.Windows.Forms.ContextMenuStrip dataGridViewContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem duplicateRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteRowToolStripMenuItem;
        private System.Data.DataSet dataSetConfig;
        private System.Data.DataColumn activeDataColumn;
        private System.Data.DataColumn fsuipcOffsetDataColumn;
        private System.Data.DataColumn converterDataColumn;
        private System.Data.DataColumn maskDataColumn;
        private System.Data.DataColumn usbArcazePinDataColumn;
        private System.Data.DataColumn typeDataColumn;
        private System.Data.DataColumn durationDataColumn;
        private System.Data.DataColumn comparisonDataColumn;
        private System.Data.DataColumn comparisonValueDataColumn;
        private System.Data.DataColumn descriptionDataColumn;
        private System.Data.DataColumn fsuipcSizeDataColumn;
        private System.Data.DataColumn triggerDataColumn;
        private System.Data.DataColumn arcazeSerialDataColumn;
        private System.Data.DataColumn settingsColumn;
        private System.Data.DataColumn guidDataColumn;

        private System.Data.DataTable configDataTable;
        private System.Data.DataColumn outputDataColumn;
        private System.Data.DataColumn outputTypeDataColumn;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn active;
        private System.Windows.Forms.DataGridViewTextBoxColumn guid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.DataGridViewTextBoxColumn moduleSerial;
        private System.Windows.Forms.DataGridViewTextBoxColumn OutputName;
        private System.Windows.Forms.DataGridViewTextBoxColumn OutputType;
        private System.Windows.Forms.DataGridViewTextBoxColumn FsuipcOffset;
        private System.Windows.Forms.DataGridViewTextBoxColumn fsuipcValueColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn arcazeValueColumn;
        private System.Windows.Forms.DataGridViewButtonColumn EditButtonColumn;
    }
}
