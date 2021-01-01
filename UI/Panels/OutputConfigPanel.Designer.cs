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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            this.MappingConfigGroupBox = new System.Windows.Forms.GroupBox();
            this.dataGridViewConfig = new System.Windows.Forms.DataGridView();
            this.active = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.guid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FsuipcOffset = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.FsuipcSize = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Converter = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Mask = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.comparison = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.comparisonValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.arcazePin = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.trigger = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Typ = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.arcazeSerial = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.fsuipcValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.arcazeValueColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EditButtonColumn = new System.Windows.Forms.DataGridViewButtonColumn();
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
            this.guidDataColumn = new System.Data.DataColumn();
            this.dataGridViewContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.duplicateRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsColumn = new System.Data.DataColumn();
            this.MappingConfigGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewConfig)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetConfig)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.configDataTable)).BeginInit();
            this.dataGridViewContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // MappingConfigGroupBox
            // 
            resources.ApplyResources(this.MappingConfigGroupBox, "MappingConfigGroupBox");
            this.MappingConfigGroupBox.Controls.Add(this.dataGridViewConfig);
            this.MappingConfigGroupBox.Name = "MappingConfigGroupBox";
            this.MappingConfigGroupBox.TabStop = false;
            // 
            // dataGridViewConfig
            // 
            resources.ApplyResources(this.dataGridViewConfig, "dataGridViewConfig");
            this.dataGridViewConfig.AllowUserToResizeColumns = false;
            this.dataGridViewConfig.AllowUserToResizeRows = false;
            this.dataGridViewConfig.AutoGenerateColumns = false;
            this.dataGridViewConfig.BackgroundColor = System.Drawing.SystemColors.Window;
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
            this.FsuipcOffset,
            this.FsuipcSize,
            this.Converter,
            this.Mask,
            this.comparison,
            this.comparisonValue,
            this.arcazePin,
            this.trigger,
            this.Typ,
            this.arcazeSerial,
            this.fsuipcValueColumn,
            this.arcazeValueColumn,
            this.EditButtonColumn});
            this.dataGridViewConfig.DataMember = "config";
            this.dataGridViewConfig.DataSource = this.dataSetConfig;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewConfig.DefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridViewConfig.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2;
            this.dataGridViewConfig.Name = "dataGridViewConfig";
            this.dataGridViewConfig.RowTemplate.ContextMenuStrip = this.dataGridViewContextMenuStrip;
            this.dataGridViewConfig.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewConfig.ShowEditingIcon = false;
            this.dataGridViewConfig.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewConfig_CellContentClick);
            this.dataGridViewConfig.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewConfig_CellContentDoubleClick);
            this.dataGridViewConfig.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewConfig_CellDoubleClick);
            this.dataGridViewConfig.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewConfig_CellEnter);
            this.dataGridViewConfig.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridViewConfig_CellMouseDown);
            this.dataGridViewConfig.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewConfig_CellValidated);
            this.dataGridViewConfig.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.DataGridViewConfig_CellValidating);
            this.dataGridViewConfig.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DataGridViewConfig_DataError);
            this.dataGridViewConfig.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.DataGridViewConfig_DefaultValuesNeeded);
            this.dataGridViewConfig.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DataGridViewConfig_KeyUp);
            // 
            // active
            // 
            this.active.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.active.DataPropertyName = "active";
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
            resources.ApplyResources(this.Description, "Description");
            this.Description.Name = "Description";
            // 
            // FsuipcOffset
            // 
            this.FsuipcOffset.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.FsuipcOffset.DataPropertyName = "fsuipcOffset";
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.ControlLight;
            this.FsuipcOffset.DefaultCellStyle = dataGridViewCellStyle3;
            this.FsuipcOffset.FillWeight = 1000F;
            resources.ApplyResources(this.FsuipcOffset, "FsuipcOffset");
            this.FsuipcOffset.Name = "FsuipcOffset";
            this.FsuipcOffset.ReadOnly = true;
            // 
            // FsuipcSize
            // 
            this.FsuipcSize.DataPropertyName = "fsuipcSize";
            this.FsuipcSize.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            resources.ApplyResources(this.FsuipcSize, "FsuipcSize");
            this.FsuipcSize.Items.AddRange(new object[] {
            "1",
            "2",
            "4"});
            this.FsuipcSize.Name = "FsuipcSize";
            // 
            // Converter
            // 
            this.Converter.DataPropertyName = "converter";
            this.Converter.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            resources.ApplyResources(this.Converter, "Converter");
            this.Converter.Items.AddRange(new object[] {
            "Boolean",
            "StrobeBCD"});
            this.Converter.Name = "Converter";
            // 
            // Mask
            // 
            this.Mask.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Mask.DataPropertyName = "mask";
            this.Mask.FillWeight = 50F;
            resources.ApplyResources(this.Mask, "Mask");
            this.Mask.Name = "Mask";
            // 
            // comparison
            // 
            this.comparison.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.comparison.DataPropertyName = "comparison";
            this.comparison.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            resources.ApplyResources(this.comparison, "comparison");
            this.comparison.Items.AddRange(new object[] {
            "=",
            "<",
            ">"});
            this.comparison.Name = "comparison";
            this.comparison.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.comparison.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // comparisonValue
            // 
            this.comparisonValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.comparisonValue.DataPropertyName = "comparisonValue";
            this.comparisonValue.FillWeight = 50F;
            resources.ApplyResources(this.comparisonValue, "comparisonValue");
            this.comparisonValue.Name = "comparisonValue";
            // 
            // arcazePin
            // 
            this.arcazePin.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.arcazePin.DataPropertyName = "usbArcazePin";
            this.arcazePin.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            resources.ApplyResources(this.arcazePin, "arcazePin");
            this.arcazePin.Name = "arcazePin";
            this.arcazePin.ReadOnly = true;
            // 
            // trigger
            // 
            this.trigger.DataPropertyName = "Trigger";
            this.trigger.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            resources.ApplyResources(this.trigger, "trigger");
            this.trigger.Items.AddRange(new object[] {
            "change",
            "falling",
            "rising"});
            this.trigger.Name = "trigger";
            // 
            // Typ
            // 
            this.Typ.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Typ.DataPropertyName = "type";
            this.Typ.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            resources.ApplyResources(this.Typ, "Typ");
            this.Typ.Items.AddRange(new object[] {
            "Konstant",
            "Periode"});
            this.Typ.Name = "Typ";
            // 
            // arcazeSerial
            // 
            this.arcazeSerial.DataPropertyName = "arcazeSerial";
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.ControlLight;
            this.arcazeSerial.DefaultCellStyle = dataGridViewCellStyle4;
            this.arcazeSerial.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            resources.ApplyResources(this.arcazeSerial, "arcazeSerial");
            this.arcazeSerial.Items.AddRange(new object[] {
            "none"});
            this.arcazeSerial.Name = "arcazeSerial";
            this.arcazeSerial.ReadOnly = true;
            this.arcazeSerial.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // fsuipcValueColumn
            // 
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.ControlLight;
            this.fsuipcValueColumn.DefaultCellStyle = dataGridViewCellStyle5;
            resources.ApplyResources(this.fsuipcValueColumn, "fsuipcValueColumn");
            this.fsuipcValueColumn.Name = "fsuipcValueColumn";
            this.fsuipcValueColumn.ReadOnly = true;
            this.fsuipcValueColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.fsuipcValueColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // arcazeValueColumn
            // 
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.ControlLight;
            this.arcazeValueColumn.DefaultCellStyle = dataGridViewCellStyle6;
            resources.ApplyResources(this.arcazeValueColumn, "arcazeValueColumn");
            this.arcazeValueColumn.Name = "arcazeValueColumn";
            this.arcazeValueColumn.ReadOnly = true;
            this.arcazeValueColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.arcazeValueColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // EditButtonColumn
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.ControlLight;
            dataGridViewCellStyle7.NullValue = "...";
            this.EditButtonColumn.DefaultCellStyle = dataGridViewCellStyle7;
            resources.ApplyResources(this.EditButtonColumn, "EditButtonColumn");
            this.EditButtonColumn.Name = "EditButtonColumn";
            this.EditButtonColumn.Text = "...";
            this.EditButtonColumn.UseColumnTextForButtonValue = true;
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
            this.guidDataColumn});
            this.configDataTable.TableName = "config";
            this.configDataTable.RowChanged += new System.Data.DataRowChangeEventHandler(this.ConfigDataTable_RowChanged_1);
            this.configDataTable.TableNewRow += new System.Data.DataTableNewRowEventHandler(this.ConfigDataTable_TableNewRow);
            // 
            // activeDataColumn
            // 
            this.activeDataColumn.Caption = "Active";
            this.activeDataColumn.ColumnName = "active";
            this.activeDataColumn.DataType = typeof(bool);
            this.activeDataColumn.DefaultValue = false;
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
            // guidDataColumn
            // 
            this.guidDataColumn.ColumnMapping = System.Data.MappingType.Attribute;
            this.guidDataColumn.ColumnName = "guid";
            this.guidDataColumn.DataType = typeof(System.Guid);
            // 
            // dataGridViewContextMenuStrip
            // 
            resources.ApplyResources(this.dataGridViewContextMenuStrip, "dataGridViewContextMenuStrip");
            this.dataGridViewContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.duplicateRowToolStripMenuItem,
            this.deleteRowToolStripMenuItem});
            this.dataGridViewContextMenuStrip.Name = "dataGridViewContextMenuStrip";
            this.dataGridViewContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.DataGridViewContextMenuStrip_Opening);
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
            // settingsColumn
            // 
            this.settingsColumn.Caption = "settings";
            this.settingsColumn.ColumnName = "settings";
            this.settingsColumn.DataType = typeof(object);
            // 
            // OutputConfigPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MappingConfigGroupBox);
            this.Name = "OutputConfigPanel";
            this.MappingConfigGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewConfig)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetConfig)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.configDataTable)).EndInit();
            this.dataGridViewContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox MappingConfigGroupBox;
        public System.Windows.Forms.DataGridView dataGridViewConfig;
        private System.Windows.Forms.DataGridViewCheckBoxColumn active;
        private System.Windows.Forms.DataGridViewTextBoxColumn guid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.DataGridViewTextBoxColumn FsuipcOffset;
        private System.Windows.Forms.DataGridViewComboBoxColumn FsuipcSize;
        private System.Windows.Forms.DataGridViewComboBoxColumn Converter;
        private System.Windows.Forms.DataGridViewTextBoxColumn Mask;
        private System.Windows.Forms.DataGridViewComboBoxColumn comparison;
        private System.Windows.Forms.DataGridViewTextBoxColumn comparisonValue;
        private System.Windows.Forms.DataGridViewComboBoxColumn arcazePin;
        private System.Windows.Forms.DataGridViewComboBoxColumn trigger;
        private System.Windows.Forms.DataGridViewComboBoxColumn Typ;
        private System.Windows.Forms.DataGridViewComboBoxColumn arcazeSerial;
        private System.Windows.Forms.DataGridViewTextBoxColumn fsuipcValueColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn arcazeValueColumn;
        private System.Windows.Forms.DataGridViewButtonColumn EditButtonColumn;
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
    }
}
