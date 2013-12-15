namespace ArcazeUSB
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.konfigurationSpeichernToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AfterFileActionsToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.recentDocumentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentDocsToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.beendenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extrasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.orphanedSerialsFinderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hilfeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.überToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelMain = new System.Windows.Forms.Panel();
            this.MappingConfigGroupBox = new System.Windows.Forms.GroupBox();
            this.dataGridViewConfig = new System.Windows.Forms.DataGridView();
            this.guid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Active = new System.Windows.Forms.DataGridViewCheckBoxColumn();
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
            this.durationDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.typeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
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
            this.settingsColumn = new System.Data.DataColumn();
            this.guidDataColumn = new System.Data.DataColumn();
            this.dataGridViewContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.duplicateRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripFsuipcOffset = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStripNotifyIcon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDivider = new System.Windows.Forms.ToolStripSeparator();
            this.wiederherstellenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.saveToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.runToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.runTestToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.stopToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.stopTestToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.autoRunToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.donateToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.statusStripPanel = new System.Windows.Forms.Panel();
            this.statusStrip2 = new System.Windows.Forms.StatusStrip();
            this.ArcazeUSBTtoolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.arcazeUsbStatusToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.arcazeUsbToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.dividerToolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.fsuipcToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.fsuipcStatusToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.fsuipcOffsetValueLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.MappingConfigGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewConfig)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetConfig)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.configDataTable)).BeginInit();
            this.dataGridViewContextMenuStrip.SuspendLayout();
            this.contextMenuStripFsuipcOffset.SuspendLayout();
            this.contextMenuStripNotifyIcon.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStripPanel.SuspendLayout();
            this.statusStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            resources.ApplyResources(this.menuStrip, "menuStrip");
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.extrasToolStripMenuItem,
            this.hilfeToolStripMenuItem});
            this.menuStrip.Name = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem3,
            this.toolStripMenuItem2,
            this.openToolStripMenuItem,
            this.konfigurationSpeichernToolStripMenuItem,
            this.AfterFileActionsToolStripMenuItem,
            this.recentDocumentsToolStripMenuItem,
            this.recentDocsToolStripSeparator,
            this.beendenToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            // 
            // toolStripMenuItem3
            // 
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // toolStripMenuItem2
            // 
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            // 
            // openToolStripMenuItem
            // 
            resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // konfigurationSpeichernToolStripMenuItem
            // 
            resources.ApplyResources(this.konfigurationSpeichernToolStripMenuItem, "konfigurationSpeichernToolStripMenuItem");
            this.konfigurationSpeichernToolStripMenuItem.Name = "konfigurationSpeichernToolStripMenuItem";
            this.konfigurationSpeichernToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // AfterFileActionsToolStripMenuItem
            // 
            resources.ApplyResources(this.AfterFileActionsToolStripMenuItem, "AfterFileActionsToolStripMenuItem");
            this.AfterFileActionsToolStripMenuItem.Name = "AfterFileActionsToolStripMenuItem";
            // 
            // recentDocumentsToolStripMenuItem
            // 
            resources.ApplyResources(this.recentDocumentsToolStripMenuItem, "recentDocumentsToolStripMenuItem");
            this.recentDocumentsToolStripMenuItem.Name = "recentDocumentsToolStripMenuItem";
            // 
            // recentDocsToolStripSeparator
            // 
            resources.ApplyResources(this.recentDocsToolStripSeparator, "recentDocsToolStripSeparator");
            this.recentDocsToolStripSeparator.Name = "recentDocsToolStripSeparator";
            // 
            // beendenToolStripMenuItem
            // 
            resources.ApplyResources(this.beendenToolStripMenuItem, "beendenToolStripMenuItem");
            this.beendenToolStripMenuItem.Name = "beendenToolStripMenuItem";
            this.beendenToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // extrasToolStripMenuItem
            // 
            resources.ApplyResources(this.extrasToolStripMenuItem, "extrasToolStripMenuItem");
            this.extrasToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.orphanedSerialsFinderToolStripMenuItem,
            this.toolStripMenuItem4,
            this.settingsToolStripMenuItem});
            this.extrasToolStripMenuItem.Name = "extrasToolStripMenuItem";
            // 
            // orphanedSerialsFinderToolStripMenuItem
            // 
            resources.ApplyResources(this.orphanedSerialsFinderToolStripMenuItem, "orphanedSerialsFinderToolStripMenuItem");
            this.orphanedSerialsFinderToolStripMenuItem.Name = "orphanedSerialsFinderToolStripMenuItem";
            this.orphanedSerialsFinderToolStripMenuItem.Click += new System.EventHandler(this.orphanedSerialsFinderToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            // 
            // settingsToolStripMenuItem
            // 
            resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // hilfeToolStripMenuItem
            // 
            resources.ApplyResources(this.hilfeToolStripMenuItem, "hilfeToolStripMenuItem");
            this.hilfeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem,
            this.toolStripMenuItem1,
            this.überToolStripMenuItem});
            this.hilfeToolStripMenuItem.Name = "hilfeToolStripMenuItem";
            // 
            // helpToolStripMenuItem
            // 
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            // 
            // überToolStripMenuItem
            // 
            resources.ApplyResources(this.überToolStripMenuItem, "überToolStripMenuItem");
            this.überToolStripMenuItem.Name = "überToolStripMenuItem";
            this.überToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // panelMain
            // 
            resources.ApplyResources(this.panelMain, "panelMain");
            this.panelMain.Controls.Add(this.MappingConfigGroupBox);
            this.panelMain.Name = "panelMain";
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
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewConfig.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewConfig.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewConfig.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.guid,
            this.Active,
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
            this.durationDataGridViewTextBoxColumn,
            this.typeDataGridViewTextBoxColumn,
            this.arcazeSerial,
            this.fsuipcValueColumn,
            this.arcazeValueColumn,
            this.EditButtonColumn});
            this.dataGridViewConfig.DataMember = "config";
            this.dataGridViewConfig.DataSource = this.dataSetConfig;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewConfig.DefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridViewConfig.Name = "dataGridViewConfig";
            this.dataGridViewConfig.RowTemplate.ContextMenuStrip = this.dataGridViewContextMenuStrip;
            this.dataGridViewConfig.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewConfig.ShowEditingIcon = false;
            this.dataGridViewConfig.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewConfig_CellContentClick);
            this.dataGridViewConfig.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewConfig_CellContentDoubleClick);
            this.dataGridViewConfig.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewConfig_CellDoubleClick);
            this.dataGridViewConfig.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewConfig_CellEnter);
            this.dataGridViewConfig.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridViewConfig_CellMouseDown);
            this.dataGridViewConfig.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewConfig_CellValidated);
            this.dataGridViewConfig.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridViewConfig_CellValidating);
            this.dataGridViewConfig.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridViewConfig_DataError);
            this.dataGridViewConfig.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridViewConfig_DefaultValuesNeeded);
            // 
            // guid
            // 
            this.guid.DataPropertyName = "guid";
            resources.ApplyResources(this.guid, "guid");
            this.guid.Name = "guid";
            this.guid.ReadOnly = true;
            this.guid.ToolTipText = global::ArcazeUSB.ProjectMessages.conf;
            // 
            // Active
            // 
            this.Active.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Active.DataPropertyName = "active";
            this.Active.FalseValue = "false";
            resources.ApplyResources(this.Active, "Active");
            this.Active.Name = "Active";
            this.Active.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Active.TrueValue = "true";
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
            this.FsuipcSize.ToolTipText = global::ArcazeUSB.ProjectMessages.conf;
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
            this.comparisonValue.ToolTipText = global::ArcazeUSB.ProjectMessages.conf;
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
            this.trigger.ToolTipText = global::ArcazeUSB.ProjectMessages.conf;
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
            // durationDataGridViewTextBoxColumn
            // 
            this.durationDataGridViewTextBoxColumn.DataPropertyName = "duration";
            resources.ApplyResources(this.durationDataGridViewTextBoxColumn, "durationDataGridViewTextBoxColumn");
            this.durationDataGridViewTextBoxColumn.Name = "durationDataGridViewTextBoxColumn";
            // 
            // typeDataGridViewTextBoxColumn
            // 
            this.typeDataGridViewTextBoxColumn.DataPropertyName = "type";
            this.typeDataGridViewTextBoxColumn.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.Nothing;
            resources.ApplyResources(this.typeDataGridViewTextBoxColumn, "typeDataGridViewTextBoxColumn");
            this.typeDataGridViewTextBoxColumn.Items.AddRange(new object[] {
            "Default",
            "Blink"});
            this.typeDataGridViewTextBoxColumn.Name = "typeDataGridViewTextBoxColumn";
            this.typeDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.typeDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.typeDataGridViewTextBoxColumn.ToolTipText = global::ArcazeUSB.ProjectMessages.conf;
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
            this.arcazeSerial.ToolTipText = global::ArcazeUSB.ProjectMessages.conf;
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
            this.fsuipcValueColumn.ToolTipText = global::ArcazeUSB.ProjectMessages.conf;
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
            this.arcazeValueColumn.ToolTipText = global::ArcazeUSB.ProjectMessages.conf;
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
            this.EditButtonColumn.ToolTipText = global::ArcazeUSB.ProjectMessages.conf;
            this.EditButtonColumn.UseColumnTextForButtonValue = true;
            // 
            // dataSetConfig
            // 
            this.dataSetConfig.DataSetName = "ArcazeUsbConnector";
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
            this.configDataTable.RowChanged += new System.Data.DataRowChangeEventHandler(this.configDataTable_RowChanged_1);
            this.configDataTable.TableNewRow += new System.Data.DataTableNewRowEventHandler(this.configDataTable_TableNewRow);
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
            this.descriptionDataColumn.DefaultValue = global::ArcazeUSB.ProjectMessages.conf;
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
            this.arcazeSerialDataColumn.DefaultValue = global::ArcazeUSB.ProjectMessages.conf;
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
            // dataGridViewContextMenuStrip
            // 
            resources.ApplyResources(this.dataGridViewContextMenuStrip, "dataGridViewContextMenuStrip");
            this.dataGridViewContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.duplicateRowToolStripMenuItem,
            this.deleteRowToolStripMenuItem});
            this.dataGridViewContextMenuStrip.Name = "dataGridViewContextMenuStrip";
            this.dataGridViewContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.dataGridViewContextMenuStrip_Opening);
            // 
            // duplicateRowToolStripMenuItem
            // 
            resources.ApplyResources(this.duplicateRowToolStripMenuItem, "duplicateRowToolStripMenuItem");
            this.duplicateRowToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.duplicateRowToolStripMenuItem.Image = global::ArcazeUSB.Properties.Resources.star_yellow_new;
            this.duplicateRowToolStripMenuItem.Name = "duplicateRowToolStripMenuItem";
            this.duplicateRowToolStripMenuItem.Click += new System.EventHandler(this.duplicateRowToolStripMenuItem_Click);
            // 
            // deleteRowToolStripMenuItem
            // 
            resources.ApplyResources(this.deleteRowToolStripMenuItem, "deleteRowToolStripMenuItem");
            this.deleteRowToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.deleteRowToolStripMenuItem.Image = global::ArcazeUSB.Properties.Resources.delete2;
            this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
            this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
            // 
            // contextMenuStripFsuipcOffset
            // 
            resources.ApplyResources(this.contextMenuStripFsuipcOffset, "contextMenuStripFsuipcOffset");
            this.contextMenuStripFsuipcOffset.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBox1});
            this.contextMenuStripFsuipcOffset.Name = "contextMenuStripFsuipcOffset";
            // 
            // toolStripComboBox1
            // 
            resources.ApplyResources(this.toolStripComboBox1, "toolStripComboBox1");
            this.toolStripComboBox1.Items.AddRange(new object[] {
            resources.GetString("toolStripComboBox1.Items"),
            resources.GetString("toolStripComboBox1.Items1"),
            resources.GetString("toolStripComboBox1.Items2"),
            resources.GetString("toolStripComboBox1.Items3")});
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Text = global::ArcazeUSB.ProjectMessages.conf;
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipText = global::ArcazeUSB.ProjectMessages.conf;
            this.notifyIcon.BalloonTipTitle = global::ArcazeUSB.ProjectMessages.conf;
            this.notifyIcon.ContextMenuStrip = this.contextMenuStripNotifyIcon;
            resources.ApplyResources(this.notifyIcon, "notifyIcon");
            this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            // 
            // contextMenuStripNotifyIcon
            // 
            resources.ApplyResources(this.contextMenuStripNotifyIcon, "contextMenuStripNotifyIcon");
            this.contextMenuStripNotifyIcon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.toolStripMenuItemDivider,
            this.wiederherstellenToolStripMenuItem});
            this.contextMenuStripNotifyIcon.Name = "contextMenuStripNotifyIcon";
            // 
            // startToolStripMenuItem
            // 
            resources.ApplyResources(this.startToolStripMenuItem, "startToolStripMenuItem");
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.buttonToggleStart_Click);
            // 
            // stopToolStripMenuItem
            // 
            resources.ApplyResources(this.stopToolStripMenuItem, "stopToolStripMenuItem");
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.buttonToggleStart_Click);
            // 
            // toolStripMenuItemDivider
            // 
            resources.ApplyResources(this.toolStripMenuItemDivider, "toolStripMenuItemDivider");
            this.toolStripMenuItemDivider.Name = "toolStripMenuItemDivider";
            // 
            // wiederherstellenToolStripMenuItem
            // 
            resources.ApplyResources(this.wiederherstellenToolStripMenuItem, "wiederherstellenToolStripMenuItem");
            this.wiederherstellenToolStripMenuItem.Name = "wiederherstellenToolStripMenuItem";
            this.wiederherstellenToolStripMenuItem.Click += new System.EventHandler(this.restoreToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripButton,
            this.toolStripSeparator1,
            this.runToolStripButton,
            this.runTestToolStripButton,
            this.stopToolStripButton,
            this.stopTestToolStripButton,
            this.toolStripSeparator2,
            this.autoRunToolStripButton,
            this.toolStripSeparator3,
            this.donateToolStripButton,
            this.toolStripSeparator4,
            this.exitToolStripButton});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // saveToolStripButton
            // 
            resources.ApplyResources(this.saveToolStripButton, "saveToolStripButton");
            this.saveToolStripButton.Image = global::ArcazeUSB.Properties.Resources.disk_blue;
            this.saveToolStripButton.Name = "saveToolStripButton";
            this.saveToolStripButton.Click += new System.EventHandler(this.saveToolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // runToolStripButton
            // 
            resources.ApplyResources(this.runToolStripButton, "runToolStripButton");
            this.runToolStripButton.Image = global::ArcazeUSB.Properties.Resources.media_play_green;
            this.runToolStripButton.Name = "runToolStripButton";
            this.runToolStripButton.Click += new System.EventHandler(this.buttonToggleStart_Click);
            // 
            // runTestToolStripButton
            // 
            resources.ApplyResources(this.runTestToolStripButton, "runTestToolStripButton");
            this.runTestToolStripButton.Image = global::ArcazeUSB.Properties.Resources.media_play;
            this.runTestToolStripButton.Name = "runTestToolStripButton";
            this.runTestToolStripButton.Click += new System.EventHandler(this.runTestToolStripLabel_Click);
            // 
            // stopToolStripButton
            // 
            resources.ApplyResources(this.stopToolStripButton, "stopToolStripButton");
            this.stopToolStripButton.Image = global::ArcazeUSB.Properties.Resources.media_stop_red;
            this.stopToolStripButton.Name = "stopToolStripButton";
            this.stopToolStripButton.Click += new System.EventHandler(this.buttonToggleStart_Click);
            // 
            // stopTestToolStripButton
            // 
            resources.ApplyResources(this.stopTestToolStripButton, "stopTestToolStripButton");
            this.stopTestToolStripButton.Image = global::ArcazeUSB.Properties.Resources.media_stop;
            this.stopTestToolStripButton.Name = "stopTestToolStripButton";
            this.stopTestToolStripButton.Click += new System.EventHandler(this.stopTestToolStripButton_Click);
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // autoRunToolStripButton
            // 
            resources.ApplyResources(this.autoRunToolStripButton, "autoRunToolStripButton");
            this.autoRunToolStripButton.Image = global::ArcazeUSB.Properties.Resources.lightbulb;
            this.autoRunToolStripButton.Name = "autoRunToolStripButton";
            this.autoRunToolStripButton.Click += new System.EventHandler(this.autoRunToolStripButton_Click);
            // 
            // toolStripSeparator3
            // 
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // donateToolStripButton
            // 
            resources.ApplyResources(this.donateToolStripButton, "donateToolStripButton");
            this.donateToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.donateToolStripButton.Image = global::ArcazeUSB.Properties.Resources.btn_donate_SM;
            this.donateToolStripButton.Name = "donateToolStripButton";
            this.donateToolStripButton.Click += new System.EventHandler(this.donateToolStripButton_Click);
            // 
            // toolStripSeparator4
            // 
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            // 
            // exitToolStripButton
            // 
            resources.ApplyResources(this.exitToolStripButton, "exitToolStripButton");
            this.exitToolStripButton.Image = global::ArcazeUSB.Properties.Resources.door2;
            this.exitToolStripButton.Name = "exitToolStripButton";
            this.exitToolStripButton.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // statusStripPanel
            // 
            resources.ApplyResources(this.statusStripPanel, "statusStripPanel");
            this.statusStripPanel.Controls.Add(this.statusStrip2);
            this.statusStripPanel.MinimumSize = new System.Drawing.Size(0, 24);
            this.statusStripPanel.Name = "statusStripPanel";
            // 
            // statusStrip2
            // 
            resources.ApplyResources(this.statusStrip2, "statusStrip2");
            this.statusStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ArcazeUSBTtoolStripStatusLabel,
            this.arcazeUsbStatusToolStripStatusLabel,
            this.arcazeUsbToolStripDropDownButton,
            this.dividerToolStripStatusLabel1,
            this.fsuipcToolStripStatusLabel,
            this.fsuipcStatusToolStripStatusLabel,
            this.fsuipcOffsetValueLabel,
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel});
            this.statusStrip2.Name = "statusStrip2";
            this.statusStrip2.SizingGrip = false;
            // 
            // ArcazeUSBTtoolStripStatusLabel
            // 
            resources.ApplyResources(this.ArcazeUSBTtoolStripStatusLabel, "ArcazeUSBTtoolStripStatusLabel");
            this.ArcazeUSBTtoolStripStatusLabel.Name = "ArcazeUSBTtoolStripStatusLabel";
            // 
            // arcazeUsbStatusToolStripStatusLabel
            // 
            resources.ApplyResources(this.arcazeUsbStatusToolStripStatusLabel, "arcazeUsbStatusToolStripStatusLabel");
            this.arcazeUsbStatusToolStripStatusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.arcazeUsbStatusToolStripStatusLabel.Image = global::ArcazeUSB.Properties.Resources.warning;
            this.arcazeUsbStatusToolStripStatusLabel.Name = "arcazeUsbStatusToolStripStatusLabel";
            // 
            // arcazeUsbToolStripDropDownButton
            // 
            resources.ApplyResources(this.arcazeUsbToolStripDropDownButton, "arcazeUsbToolStripDropDownButton");
            this.arcazeUsbToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.arcazeUsbToolStripDropDownButton.Name = "arcazeUsbToolStripDropDownButton";
            // 
            // dividerToolStripStatusLabel1
            // 
            resources.ApplyResources(this.dividerToolStripStatusLabel1, "dividerToolStripStatusLabel1");
            this.dividerToolStripStatusLabel1.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.dividerToolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.dividerToolStripStatusLabel1.Name = "dividerToolStripStatusLabel1";
            // 
            // fsuipcToolStripStatusLabel
            // 
            resources.ApplyResources(this.fsuipcToolStripStatusLabel, "fsuipcToolStripStatusLabel");
            this.fsuipcToolStripStatusLabel.Name = "fsuipcToolStripStatusLabel";
            // 
            // fsuipcStatusToolStripStatusLabel
            // 
            resources.ApplyResources(this.fsuipcStatusToolStripStatusLabel, "fsuipcStatusToolStripStatusLabel");
            this.fsuipcStatusToolStripStatusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.fsuipcStatusToolStripStatusLabel.Image = global::ArcazeUSB.Properties.Resources.warning;
            this.fsuipcStatusToolStripStatusLabel.Name = "fsuipcStatusToolStripStatusLabel";
            // 
            // fsuipcOffsetValueLabel
            // 
            resources.ApplyResources(this.fsuipcOffsetValueLabel, "fsuipcOffsetValueLabel");
            this.fsuipcOffsetValueLabel.Name = "fsuipcOffsetValueLabel";
            // 
            // toolStripStatusLabel1
            // 
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            this.toolStripStatusLabel1.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel
            // 
            resources.ApplyResources(this.toolStripStatusLabel, "toolStripStatusLabel");
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Spring = true;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.statusStripPanel);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            this.MappingConfigGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewConfig)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetConfig)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.configDataTable)).EndInit();
            this.dataGridViewContextMenuStrip.ResumeLayout(false);
            this.contextMenuStripFsuipcOffset.ResumeLayout(false);
            this.contextMenuStripNotifyIcon.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStripPanel.ResumeLayout(false);
            this.statusStrip2.ResumeLayout(false);
            this.statusStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem konfigurationSpeichernToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator AfterFileActionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem beendenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hilfeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem überToolStripMenuItem;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.GroupBox MappingConfigGroupBox;
        private System.Windows.Forms.DataGridView dataGridViewConfig;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripNotifyIcon;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItemDivider;
        private System.Windows.Forms.ToolStripMenuItem wiederherstellenToolStripMenuItem;
        private System.Data.DataSet dataSetConfig;
        private System.Data.DataTable configDataTable;
        private System.Data.DataColumn activeDataColumn;
        private System.Data.DataColumn fsuipcOffsetDataColumn;
        private System.Data.DataColumn converterDataColumn;
        private System.Data.DataColumn maskDataColumn;
        private System.Data.DataColumn usbArcazePinDataColumn;
        private System.Data.DataColumn typeDataColumn;
        private System.Data.DataColumn durationDataColumn;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripFsuipcOffset;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private System.Data.DataColumn comparisonDataColumn;
        private System.Data.DataColumn comparisonValueDataColumn;
        private System.Data.DataColumn descriptionDataColumn;
        private System.Data.DataColumn fsuipcSizeDataColumn;
        private System.Data.DataColumn triggerDataColumn;
        private System.Data.DataColumn arcazeSerialDataColumn;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Panel statusStripPanel;
        private System.Windows.Forms.StatusStrip statusStrip2;
        private System.Windows.Forms.ToolStripStatusLabel fsuipcToolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel fsuipcStatusToolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel dividerToolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel ArcazeUSBTtoolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel arcazeUsbStatusToolStripStatusLabel;
        private System.Windows.Forms.ToolStripDropDownButton arcazeUsbToolStripDropDownButton;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripButton saveToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton stopToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton exitToolStripButton;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripSeparator recentDocsToolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem recentDocumentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton runToolStripButton;
        private System.Windows.Forms.ToolStripButton stopTestToolStripButton;
        private System.Windows.Forms.ToolStripButton runTestToolStripButton;
        private System.Windows.Forms.ToolStripStatusLabel fsuipcOffsetValueLabel;
        private System.Data.DataColumn settingsColumn;
        private System.Windows.Forms.ToolStripMenuItem extrasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton autoRunToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.DataGridViewTextBoxColumn guid;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Active;
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
        private System.Windows.Forms.DataGridViewTextBoxColumn durationDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn typeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn arcazeSerial;
        private System.Windows.Forms.DataGridViewTextBoxColumn fsuipcValueColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn arcazeValueColumn;
        private System.Windows.Forms.DataGridViewButtonColumn EditButtonColumn;
        private System.Data.DataColumn guidDataColumn;
        private System.Windows.Forms.ContextMenuStrip dataGridViewContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem deleteRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem duplicateRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem orphanedSerialsFinderToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripButton donateToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;


    }
}

