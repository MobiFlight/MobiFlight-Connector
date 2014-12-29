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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle30 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle25 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle26 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle27 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle28 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle29 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle31 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle32 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle33 = new System.Windows.Forms.DataGridViewCellStyle();
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
            this.inputsTabControl = new System.Windows.Forms.TabControl();
            this.OutputTabPage = new System.Windows.Forms.TabPage();
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
            this.settingsColumn = new System.Data.DataColumn();
            this.guidDataColumn = new System.Data.DataColumn();
            this.dataGridViewContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.duplicateRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.InputTabPage = new System.Windows.Forms.TabPage();
            this.inputsDataGridView = new System.Windows.Forms.DataGridView();
            this.activeDataGridViewCheckBoxColumn2 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.inputDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.inputsGuid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.inputName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.inputType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.inputEditButtonColumn = new System.Windows.Forms.DataGridViewButtonColumn();
            this.dataSetInputs = new System.Data.DataSet();
            this.inputsDataTable = new System.Data.DataTable();
            this.inputsActiveDataColumn = new System.Data.DataColumn();
            this.inputsDescriptionDataColumn = new System.Data.DataColumn();
            this.inputsGuidDataColumn = new System.Data.DataColumn();
            this.inputsSettingsDataColumn = new System.Data.DataColumn();
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
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.activeDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dataTable1 = new System.Data.DataTable();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.menuStrip.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.inputsTabControl.SuspendLayout();
            this.OutputTabPage.SuspendLayout();
            this.MappingConfigGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewConfig)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetConfig)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.configDataTable)).BeginInit();
            this.dataGridViewContextMenuStrip.SuspendLayout();
            this.InputTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.inputsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetInputs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputsDataTable)).BeginInit();
            this.contextMenuStripFsuipcOffset.SuspendLayout();
            this.contextMenuStripNotifyIcon.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStripPanel.SuspendLayout();
            this.statusStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.extrasToolStripMenuItem,
            this.hilfeToolStripMenuItem});
            resources.ApplyResources(this.menuStrip, "menuStrip");
            this.menuStrip.Name = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
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
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            this.toolStripMenuItem3.Click += new System.EventHandler(this.newFileToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
            this.openToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // konfigurationSpeichernToolStripMenuItem
            // 
            this.konfigurationSpeichernToolStripMenuItem.Name = "konfigurationSpeichernToolStripMenuItem";
            resources.ApplyResources(this.konfigurationSpeichernToolStripMenuItem, "konfigurationSpeichernToolStripMenuItem");
            this.konfigurationSpeichernToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // AfterFileActionsToolStripMenuItem
            // 
            this.AfterFileActionsToolStripMenuItem.Name = "AfterFileActionsToolStripMenuItem";
            resources.ApplyResources(this.AfterFileActionsToolStripMenuItem, "AfterFileActionsToolStripMenuItem");
            // 
            // recentDocumentsToolStripMenuItem
            // 
            this.recentDocumentsToolStripMenuItem.Name = "recentDocumentsToolStripMenuItem";
            resources.ApplyResources(this.recentDocumentsToolStripMenuItem, "recentDocumentsToolStripMenuItem");
            // 
            // recentDocsToolStripSeparator
            // 
            this.recentDocsToolStripSeparator.Name = "recentDocsToolStripSeparator";
            resources.ApplyResources(this.recentDocsToolStripSeparator, "recentDocsToolStripSeparator");
            // 
            // beendenToolStripMenuItem
            // 
            this.beendenToolStripMenuItem.Name = "beendenToolStripMenuItem";
            resources.ApplyResources(this.beendenToolStripMenuItem, "beendenToolStripMenuItem");
            this.beendenToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // extrasToolStripMenuItem
            // 
            this.extrasToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.orphanedSerialsFinderToolStripMenuItem,
            this.toolStripMenuItem4,
            this.settingsToolStripMenuItem});
            this.extrasToolStripMenuItem.Name = "extrasToolStripMenuItem";
            resources.ApplyResources(this.extrasToolStripMenuItem, "extrasToolStripMenuItem");
            // 
            // orphanedSerialsFinderToolStripMenuItem
            // 
            this.orphanedSerialsFinderToolStripMenuItem.Name = "orphanedSerialsFinderToolStripMenuItem";
            resources.ApplyResources(this.orphanedSerialsFinderToolStripMenuItem, "orphanedSerialsFinderToolStripMenuItem");
            this.orphanedSerialsFinderToolStripMenuItem.Click += new System.EventHandler(this.orphanedSerialsFinderToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // hilfeToolStripMenuItem
            // 
            this.hilfeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpToolStripMenuItem,
            this.toolStripMenuItem1,
            this.überToolStripMenuItem});
            this.hilfeToolStripMenuItem.Name = "hilfeToolStripMenuItem";
            resources.ApplyResources(this.hilfeToolStripMenuItem, "hilfeToolStripMenuItem");
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // überToolStripMenuItem
            // 
            this.überToolStripMenuItem.Name = "überToolStripMenuItem";
            resources.ApplyResources(this.überToolStripMenuItem, "überToolStripMenuItem");
            this.überToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // panelMain
            // 
            resources.ApplyResources(this.panelMain, "panelMain");
            this.panelMain.Controls.Add(this.inputsTabControl);
            this.panelMain.Name = "panelMain";
            // 
            // inputsTabControl
            // 
            this.inputsTabControl.Controls.Add(this.OutputTabPage);
            this.inputsTabControl.Controls.Add(this.InputTabPage);
            resources.ApplyResources(this.inputsTabControl, "inputsTabControl");
            this.inputsTabControl.Multiline = true;
            this.inputsTabControl.Name = "inputsTabControl";
            this.inputsTabControl.SelectedIndex = 0;
            this.inputsTabControl.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            // 
            // OutputTabPage
            // 
            this.OutputTabPage.Controls.Add(this.MappingConfigGroupBox);
            resources.ApplyResources(this.OutputTabPage, "OutputTabPage");
            this.OutputTabPage.Name = "OutputTabPage";
            this.OutputTabPage.UseVisualStyleBackColor = true;
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
            this.dataGridViewConfig.AllowUserToResizeColumns = false;
            this.dataGridViewConfig.AllowUserToResizeRows = false;
            this.dataGridViewConfig.AutoGenerateColumns = false;
            this.dataGridViewConfig.BackgroundColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle23.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
            dataGridViewCellStyle23.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle23.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle23.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle23.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle23.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle23.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewConfig.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle23;
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
            dataGridViewCellStyle30.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle30.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle30.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            dataGridViewCellStyle30.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle30.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle30.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle30.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewConfig.DefaultCellStyle = dataGridViewCellStyle30;
            resources.ApplyResources(this.dataGridViewConfig, "dataGridViewConfig");
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
            dataGridViewCellStyle24.NullValue = "Doppelklicken für neuen Eintrag...";
            this.Description.DefaultCellStyle = dataGridViewCellStyle24;
            resources.ApplyResources(this.Description, "Description");
            this.Description.Name = "Description";
            // 
            // FsuipcOffset
            // 
            this.FsuipcOffset.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.FsuipcOffset.DataPropertyName = "fsuipcOffset";
            dataGridViewCellStyle25.BackColor = System.Drawing.SystemColors.ControlLight;
            this.FsuipcOffset.DefaultCellStyle = dataGridViewCellStyle25;
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
            dataGridViewCellStyle26.BackColor = System.Drawing.SystemColors.ControlLight;
            this.arcazeSerial.DefaultCellStyle = dataGridViewCellStyle26;
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
            dataGridViewCellStyle27.BackColor = System.Drawing.SystemColors.ControlLight;
            this.fsuipcValueColumn.DefaultCellStyle = dataGridViewCellStyle27;
            resources.ApplyResources(this.fsuipcValueColumn, "fsuipcValueColumn");
            this.fsuipcValueColumn.Name = "fsuipcValueColumn";
            this.fsuipcValueColumn.ReadOnly = true;
            this.fsuipcValueColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.fsuipcValueColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // arcazeValueColumn
            // 
            dataGridViewCellStyle28.BackColor = System.Drawing.SystemColors.ControlLight;
            this.arcazeValueColumn.DefaultCellStyle = dataGridViewCellStyle28;
            resources.ApplyResources(this.arcazeValueColumn, "arcazeValueColumn");
            this.arcazeValueColumn.Name = "arcazeValueColumn";
            this.arcazeValueColumn.ReadOnly = true;
            this.arcazeValueColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.arcazeValueColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // EditButtonColumn
            // 
            dataGridViewCellStyle29.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle29.BackColor = System.Drawing.SystemColors.ControlLight;
            dataGridViewCellStyle29.NullValue = "...";
            this.EditButtonColumn.DefaultCellStyle = dataGridViewCellStyle29;
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
            this.dataGridViewContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.duplicateRowToolStripMenuItem,
            this.deleteRowToolStripMenuItem});
            this.dataGridViewContextMenuStrip.Name = "dataGridViewContextMenuStrip";
            resources.ApplyResources(this.dataGridViewContextMenuStrip, "dataGridViewContextMenuStrip");
            this.dataGridViewContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.dataGridViewContextMenuStrip_Opening);
            // 
            // duplicateRowToolStripMenuItem
            // 
            this.duplicateRowToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.duplicateRowToolStripMenuItem.Image = global::ArcazeUSB.Properties.Resources.star_yellow_new;
            this.duplicateRowToolStripMenuItem.Name = "duplicateRowToolStripMenuItem";
            resources.ApplyResources(this.duplicateRowToolStripMenuItem, "duplicateRowToolStripMenuItem");
            this.duplicateRowToolStripMenuItem.Click += new System.EventHandler(this.duplicateRowToolStripMenuItem_Click);
            // 
            // deleteRowToolStripMenuItem
            // 
            this.deleteRowToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.deleteRowToolStripMenuItem.Image = global::ArcazeUSB.Properties.Resources.delete2;
            this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
            resources.ApplyResources(this.deleteRowToolStripMenuItem, "deleteRowToolStripMenuItem");
            this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
            // 
            // InputTabPage
            // 
            this.InputTabPage.Controls.Add(this.inputsDataGridView);
            resources.ApplyResources(this.InputTabPage, "InputTabPage");
            this.InputTabPage.Name = "InputTabPage";
            this.InputTabPage.UseVisualStyleBackColor = true;
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
            this.activeDataGridViewCheckBoxColumn2,
            this.inputDescription,
            this.inputsGuid,
            this.inputName,
            this.inputType,
            this.inputEditButtonColumn});
            this.inputsDataGridView.DataMember = "config";
            this.inputsDataGridView.DataSource = this.dataSetInputs;
            this.inputsDataGridView.Name = "inputsDataGridView";
            this.inputsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.inputsDataGridView.ShowEditingIcon = false;
            this.inputsDataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.inputsDataGridView_CellContentClick);
            this.inputsDataGridView.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.inputsDataGridView_CellContentDoubleClick);
            this.inputsDataGridView.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.inputsDataGridView_CellEnter);
            this.inputsDataGridView.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.inputsDataGridViewConfig_DefaultValuesNeeded);
            // 
            // activeDataGridViewCheckBoxColumn2
            // 
            this.activeDataGridViewCheckBoxColumn2.DataPropertyName = "active";
            resources.ApplyResources(this.activeDataGridViewCheckBoxColumn2, "activeDataGridViewCheckBoxColumn2");
            this.activeDataGridViewCheckBoxColumn2.Name = "activeDataGridViewCheckBoxColumn2";
            this.activeDataGridViewCheckBoxColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
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
            dataGridViewCellStyle31.BackColor = System.Drawing.SystemColors.ControlLight;
            this.inputName.DefaultCellStyle = dataGridViewCellStyle31;
            resources.ApplyResources(this.inputName, "inputName");
            this.inputName.Name = "inputName";
            this.inputName.ReadOnly = true;
            this.inputName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.inputName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // inputType
            // 
            this.inputType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            dataGridViewCellStyle32.BackColor = System.Drawing.SystemColors.ControlLight;
            this.inputType.DefaultCellStyle = dataGridViewCellStyle32;
            resources.ApplyResources(this.inputType, "inputType");
            this.inputType.Name = "inputType";
            this.inputType.ReadOnly = true;
            this.inputType.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // inputEditButtonColumn
            // 
            this.inputEditButtonColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle33.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle33.BackColor = System.Drawing.SystemColors.ControlLight;
            dataGridViewCellStyle33.NullValue = "...";
            this.inputEditButtonColumn.DefaultCellStyle = dataGridViewCellStyle33;
            resources.ApplyResources(this.inputEditButtonColumn, "inputEditButtonColumn");
            this.inputEditButtonColumn.Name = "inputEditButtonColumn";
            this.inputEditButtonColumn.Text = "...";
            this.inputEditButtonColumn.UseColumnTextForButtonValue = true;
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
            this.inputsDescriptionDataColumn.DefaultValue = global::ArcazeUSB.ProjectMessages.conf;
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
            // contextMenuStripFsuipcOffset
            // 
            this.contextMenuStripFsuipcOffset.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBox1});
            this.contextMenuStripFsuipcOffset.Name = "contextMenuStripFsuipcOffset";
            resources.ApplyResources(this.contextMenuStripFsuipcOffset, "contextMenuStripFsuipcOffset");
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.Items.AddRange(new object[] {
            resources.GetString("toolStripComboBox1.Items"),
            resources.GetString("toolStripComboBox1.Items1"),
            resources.GetString("toolStripComboBox1.Items2"),
            resources.GetString("toolStripComboBox1.Items3")});
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            resources.ApplyResources(this.toolStripComboBox1, "toolStripComboBox1");
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStripNotifyIcon;
            resources.ApplyResources(this.notifyIcon, "notifyIcon");
            this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            // 
            // contextMenuStripNotifyIcon
            // 
            this.contextMenuStripNotifyIcon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.toolStripMenuItemDivider,
            this.wiederherstellenToolStripMenuItem});
            this.contextMenuStripNotifyIcon.Name = "contextMenuStripNotifyIcon";
            resources.ApplyResources(this.contextMenuStripNotifyIcon, "contextMenuStripNotifyIcon");
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            resources.ApplyResources(this.startToolStripMenuItem, "startToolStripMenuItem");
            this.startToolStripMenuItem.Click += new System.EventHandler(this.buttonToggleStart_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            resources.ApplyResources(this.stopToolStripMenuItem, "stopToolStripMenuItem");
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.buttonToggleStart_Click);
            // 
            // toolStripMenuItemDivider
            // 
            this.toolStripMenuItemDivider.Name = "toolStripMenuItemDivider";
            resources.ApplyResources(this.toolStripMenuItemDivider, "toolStripMenuItemDivider");
            // 
            // wiederherstellenToolStripMenuItem
            // 
            this.wiederherstellenToolStripMenuItem.Name = "wiederherstellenToolStripMenuItem";
            resources.ApplyResources(this.wiederherstellenToolStripMenuItem, "wiederherstellenToolStripMenuItem");
            this.wiederherstellenToolStripMenuItem.Click += new System.EventHandler(this.restoreToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
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
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
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
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // runToolStripButton
            // 
            this.runToolStripButton.Image = global::ArcazeUSB.Properties.Resources.media_play_green;
            resources.ApplyResources(this.runToolStripButton, "runToolStripButton");
            this.runToolStripButton.Name = "runToolStripButton";
            this.runToolStripButton.Click += new System.EventHandler(this.buttonToggleStart_Click);
            // 
            // runTestToolStripButton
            // 
            this.runTestToolStripButton.Image = global::ArcazeUSB.Properties.Resources.media_play;
            this.runTestToolStripButton.Name = "runTestToolStripButton";
            resources.ApplyResources(this.runTestToolStripButton, "runTestToolStripButton");
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
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // autoRunToolStripButton
            // 
            this.autoRunToolStripButton.Image = global::ArcazeUSB.Properties.Resources.lightbulb;
            resources.ApplyResources(this.autoRunToolStripButton, "autoRunToolStripButton");
            this.autoRunToolStripButton.Name = "autoRunToolStripButton";
            this.autoRunToolStripButton.Click += new System.EventHandler(this.autoRunToolStripButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // donateToolStripButton
            // 
            this.donateToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.donateToolStripButton.Image = global::ArcazeUSB.Properties.Resources.btn_donate_SM;
            resources.ApplyResources(this.donateToolStripButton, "donateToolStripButton");
            this.donateToolStripButton.Name = "donateToolStripButton";
            this.donateToolStripButton.Click += new System.EventHandler(this.donateToolStripButton_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // exitToolStripButton
            // 
            this.exitToolStripButton.Image = global::ArcazeUSB.Properties.Resources.door2;
            resources.ApplyResources(this.exitToolStripButton, "exitToolStripButton");
            this.exitToolStripButton.Name = "exitToolStripButton";
            this.exitToolStripButton.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // statusStripPanel
            // 
            resources.ApplyResources(this.statusStripPanel, "statusStripPanel");
            this.statusStripPanel.Controls.Add(this.statusStrip2);
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
            this.ArcazeUSBTtoolStripStatusLabel.Name = "ArcazeUSBTtoolStripStatusLabel";
            resources.ApplyResources(this.ArcazeUSBTtoolStripStatusLabel, "ArcazeUSBTtoolStripStatusLabel");
            // 
            // arcazeUsbStatusToolStripStatusLabel
            // 
            this.arcazeUsbStatusToolStripStatusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.arcazeUsbStatusToolStripStatusLabel.Image = global::ArcazeUSB.Properties.Resources.warning;
            this.arcazeUsbStatusToolStripStatusLabel.Name = "arcazeUsbStatusToolStripStatusLabel";
            resources.ApplyResources(this.arcazeUsbStatusToolStripStatusLabel, "arcazeUsbStatusToolStripStatusLabel");
            // 
            // arcazeUsbToolStripDropDownButton
            // 
            this.arcazeUsbToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            resources.ApplyResources(this.arcazeUsbToolStripDropDownButton, "arcazeUsbToolStripDropDownButton");
            this.arcazeUsbToolStripDropDownButton.Name = "arcazeUsbToolStripDropDownButton";
            // 
            // dividerToolStripStatusLabel1
            // 
            this.dividerToolStripStatusLabel1.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.dividerToolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.dividerToolStripStatusLabel1.Name = "dividerToolStripStatusLabel1";
            resources.ApplyResources(this.dividerToolStripStatusLabel1, "dividerToolStripStatusLabel1");
            // 
            // fsuipcToolStripStatusLabel
            // 
            this.fsuipcToolStripStatusLabel.Name = "fsuipcToolStripStatusLabel";
            resources.ApplyResources(this.fsuipcToolStripStatusLabel, "fsuipcToolStripStatusLabel");
            // 
            // fsuipcStatusToolStripStatusLabel
            // 
            this.fsuipcStatusToolStripStatusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.fsuipcStatusToolStripStatusLabel.Image = global::ArcazeUSB.Properties.Resources.warning;
            this.fsuipcStatusToolStripStatusLabel.Name = "fsuipcStatusToolStripStatusLabel";
            resources.ApplyResources(this.fsuipcStatusToolStripStatusLabel, "fsuipcStatusToolStripStatusLabel");
            // 
            // fsuipcOffsetValueLabel
            // 
            this.fsuipcOffsetValueLabel.Name = "fsuipcOffsetValueLabel";
            resources.ApplyResources(this.fsuipcOffsetValueLabel, "fsuipcOffsetValueLabel");
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            resources.ApplyResources(this.toolStripStatusLabel, "toolStripStatusLabel");
            this.toolStripStatusLabel.Spring = true;
            // 
            // logTextBox
            // 
            resources.ApplyResources(this.logTextBox, "logTextBox");
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            // 
            // activeDataGridViewCheckBoxColumn
            // 
            this.activeDataGridViewCheckBoxColumn.DataPropertyName = "active";
            resources.ApplyResources(this.activeDataGridViewCheckBoxColumn, "activeDataGridViewCheckBoxColumn");
            this.activeDataGridViewCheckBoxColumn.Name = "activeDataGridViewCheckBoxColumn";
            // 
            // dataTable1
            // 
            this.dataTable1.TableName = "config";
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "settings";
            resources.ApplyResources(this.dataGridViewTextBoxColumn1, "dataGridViewTextBoxColumn1");
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "guid";
            resources.ApplyResources(this.dataGridViewTextBoxColumn3, "dataGridViewTextBoxColumn3");
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "guid";
            resources.ApplyResources(this.dataGridViewTextBoxColumn2, "dataGridViewTextBoxColumn2");
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "guid";
            resources.ApplyResources(this.dataGridViewTextBoxColumn5, "dataGridViewTextBoxColumn5");
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "guid";
            resources.ApplyResources(this.dataGridViewTextBoxColumn4, "dataGridViewTextBoxColumn4");
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.logTextBox);
            this.Controls.Add(this.statusStripPanel);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.inputsTabControl.ResumeLayout(false);
            this.OutputTabPage.ResumeLayout(false);
            this.OutputTabPage.PerformLayout();
            this.MappingConfigGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewConfig)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetConfig)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.configDataTable)).EndInit();
            this.dataGridViewContextMenuStrip.ResumeLayout(false);
            this.InputTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.inputsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetInputs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inputsDataTable)).EndInit();
            this.contextMenuStripFsuipcOffset.ResumeLayout(false);
            this.contextMenuStripNotifyIcon.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStripPanel.ResumeLayout(false);
            this.statusStripPanel.PerformLayout();
            this.statusStrip2.ResumeLayout(false);
            this.statusStrip2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).EndInit();
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
        public System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.TabControl inputsTabControl;
        private System.Windows.Forms.TabPage OutputTabPage;
        private System.Windows.Forms.TabPage InputTabPage;
        private System.Windows.Forms.DataGridViewCheckBoxColumn activeDataGridViewCheckBoxColumn;
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
        private System.Windows.Forms.DataGridView inputsDataGridView;
        private System.Windows.Forms.DataGridViewCheckBoxColumn activeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn settingsDataGridViewTextBoxColumn;
        private System.Data.DataTable dataTable1;
        private System.Data.DataSet dataSetInputs;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Data.DataTable inputsDataTable;
        private System.Data.DataColumn inputsActiveDataColumn;
        private System.Data.DataColumn inputsDescriptionDataColumn;
        private System.Data.DataColumn inputsSettingsDataColumn;
        private System.Data.DataColumn inputsGuidDataColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewCheckBoxColumn inputActive;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewCheckBoxColumn activeDataGridViewCheckBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn inputDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn inputsGuid;
        private System.Windows.Forms.DataGridViewTextBoxColumn inputName;
        private System.Windows.Forms.DataGridViewTextBoxColumn inputType;
        private System.Windows.Forms.DataGridViewButtonColumn inputEditButtonColumn;


    }
}

