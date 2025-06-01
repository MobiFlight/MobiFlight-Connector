namespace MobiFlight.UI
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
            this.panelMain = new System.Windows.Forms.Panel();
            this.tabPageImageList = new System.Windows.Forms.ImageList(this.components);
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStripNotifyIcon = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemDivider = new System.Windows.Forms.ToolStripSeparator();
            this.wiederherstellenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStripPanel = new System.Windows.Forms.Panel();
            this.statusStrip2 = new System.Windows.Forms.StatusStrip();
            this.connectedDevicesToolStripLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripConnectedDevicesIcon = new System.Windows.Forms.ToolStripStatusLabel();
            this.connectedDevicesToolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.joysticksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noneToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.MIDIDevicesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modulesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noneToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.dividerToolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.SimStatusToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.SimConnectionIconStatusToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.simStatusToolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.SimProcessDetectedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.separatorToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.FsuipcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.simConnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xPlaneDirectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripAircraftDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.linkCurrentConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openLinkedConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openLinkFilenameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeLinkConfigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.autoloadToggleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelHubHop = new System.Windows.Forms.ToolStripStatusLabel();
            this.activeDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dataTable1 = new System.Data.DataTable();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.logSplitter = new System.Windows.Forms.Splitter();
            this.frontendPanel1 = new MobiFlight.UI.Panels.FrontendPanel();
            this.logPanel1 = new MobiFlight.UI.Panels.LogPanel();
            this.contextMenuStripNotifyIcon.SuspendLayout();
            this.statusStripPanel.SuspendLayout();
            this.statusStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            resources.ApplyResources(this.panelMain, "panelMain");
            this.panelMain.Name = "panelMain";
            // 
            // tabPageImageList
            // 
            this.tabPageImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("tabPageImageList.ImageStream")));
            this.tabPageImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.tabPageImageList.Images.SetKeyName(0, "mf-output.png");
            this.tabPageImageList.Images.SetKeyName(1, "mf-output-inactive.png");
            this.tabPageImageList.Images.SetKeyName(2, "mf-input.png");
            this.tabPageImageList.Images.SetKeyName(3, "mf-input-inactive.png");
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStripNotifyIcon;
            resources.ApplyResources(this.notifyIcon, "notifyIcon");
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
            // 
            // contextMenuStripNotifyIcon
            // 
            this.contextMenuStripNotifyIcon.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStripNotifyIcon.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.toolStripMenuItemDivider,
            this.wiederherstellenToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.contextMenuStripNotifyIcon.Name = "contextMenuStripNotifyIcon";
            resources.ApplyResources(this.contextMenuStripNotifyIcon, "contextMenuStripNotifyIcon");
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            resources.ApplyResources(this.startToolStripMenuItem, "startToolStripMenuItem");
            this.startToolStripMenuItem.Click += new System.EventHandler(this.TaskBar_StartProjectExecution);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            resources.ApplyResources(this.stopToolStripMenuItem, "stopToolStripMenuItem");
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.TaskBar_StopExecution);
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
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
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
            this.connectedDevicesToolStripLabel,
            this.toolStripConnectedDevicesIcon,
            this.connectedDevicesToolStripDropDownButton,
            this.dividerToolStripStatusLabel2,
            this.SimStatusToolStripStatusLabel,
            this.toolStripStatusLabel3,
            this.SimConnectionIconStatusToolStripStatusLabel,
            this.simStatusToolStripDropDownButton1,
            this.toolStripStatusLabel1,
            this.toolStripAircraftDropDownButton,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel,
            this.toolStripStatusLabel4,
            this.toolStripStatusLabelHubHop});
            this.statusStrip2.Name = "statusStrip2";
            this.statusStrip2.ShowItemToolTips = true;
            this.statusStrip2.SizingGrip = false;
            // 
            // connectedDevicesToolStripLabel
            // 
            this.connectedDevicesToolStripLabel.Name = "connectedDevicesToolStripLabel";
            resources.ApplyResources(this.connectedDevicesToolStripLabel, "connectedDevicesToolStripLabel");
            // 
            // toolStripConnectedDevicesIcon
            // 
            this.toolStripConnectedDevicesIcon.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripConnectedDevicesIcon.Image = global::MobiFlight.Properties.Resources.warning;
            this.toolStripConnectedDevicesIcon.Name = "toolStripConnectedDevicesIcon";
            resources.ApplyResources(this.toolStripConnectedDevicesIcon, "toolStripConnectedDevicesIcon");
            // 
            // connectedDevicesToolStripDropDownButton
            // 
            this.connectedDevicesToolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.connectedDevicesToolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.joysticksToolStripMenuItem,
            this.MIDIDevicesToolStripMenuItem,
            this.modulesToolStripMenuItem});
            resources.ApplyResources(this.connectedDevicesToolStripDropDownButton, "connectedDevicesToolStripDropDownButton");
            this.connectedDevicesToolStripDropDownButton.Name = "connectedDevicesToolStripDropDownButton";
            // 
            // joysticksToolStripMenuItem
            // 
            this.joysticksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noneToolStripMenuItem1});
            this.joysticksToolStripMenuItem.Name = "joysticksToolStripMenuItem";
            resources.ApplyResources(this.joysticksToolStripMenuItem, "joysticksToolStripMenuItem");
            // 
            // noneToolStripMenuItem1
            // 
            resources.ApplyResources(this.noneToolStripMenuItem1, "noneToolStripMenuItem1");
            this.noneToolStripMenuItem1.Name = "noneToolStripMenuItem1";
            // 
            // MIDIDevicesToolStripMenuItem
            // 
            this.MIDIDevicesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noneToolStripMenuItem});
            this.MIDIDevicesToolStripMenuItem.Name = "MIDIDevicesToolStripMenuItem";
            resources.ApplyResources(this.MIDIDevicesToolStripMenuItem, "MIDIDevicesToolStripMenuItem");
            // 
            // noneToolStripMenuItem
            // 
            resources.ApplyResources(this.noneToolStripMenuItem, "noneToolStripMenuItem");
            this.noneToolStripMenuItem.Name = "noneToolStripMenuItem";
            // 
            // modulesToolStripMenuItem
            // 
            this.modulesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noneToolStripMenuItem2});
            this.modulesToolStripMenuItem.Name = "modulesToolStripMenuItem";
            resources.ApplyResources(this.modulesToolStripMenuItem, "modulesToolStripMenuItem");
            // 
            // noneToolStripMenuItem2
            // 
            resources.ApplyResources(this.noneToolStripMenuItem2, "noneToolStripMenuItem2");
            this.noneToolStripMenuItem2.Name = "noneToolStripMenuItem2";
            // 
            // dividerToolStripStatusLabel2
            // 
            this.dividerToolStripStatusLabel2.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.dividerToolStripStatusLabel2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.dividerToolStripStatusLabel2.Name = "dividerToolStripStatusLabel2";
            resources.ApplyResources(this.dividerToolStripStatusLabel2, "dividerToolStripStatusLabel2");
            // 
            // SimStatusToolStripStatusLabel
            // 
            this.SimStatusToolStripStatusLabel.Name = "SimStatusToolStripStatusLabel";
            resources.ApplyResources(this.SimStatusToolStripStatusLabel, "SimStatusToolStripStatusLabel");
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            resources.ApplyResources(this.toolStripStatusLabel3, "toolStripStatusLabel3");
            // 
            // SimConnectionIconStatusToolStripStatusLabel
            // 
            this.SimConnectionIconStatusToolStripStatusLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SimConnectionIconStatusToolStripStatusLabel.Image = global::MobiFlight.Properties.Resources.warning;
            this.SimConnectionIconStatusToolStripStatusLabel.Name = "SimConnectionIconStatusToolStripStatusLabel";
            resources.ApplyResources(this.SimConnectionIconStatusToolStripStatusLabel, "SimConnectionIconStatusToolStripStatusLabel");
            // 
            // simStatusToolStripDropDownButton1
            // 
            this.simStatusToolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.simStatusToolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SimProcessDetectedToolStripMenuItem,
            this.separatorToolStripMenuItem,
            this.FsuipcToolStripMenuItem,
            this.simConnectToolStripMenuItem,
            this.xPlaneDirectToolStripMenuItem});
            resources.ApplyResources(this.simStatusToolStripDropDownButton1, "simStatusToolStripDropDownButton1");
            this.simStatusToolStripDropDownButton1.Name = "simStatusToolStripDropDownButton1";
            // 
            // SimProcessDetectedToolStripMenuItem
            // 
            this.SimProcessDetectedToolStripMenuItem.Name = "SimProcessDetectedToolStripMenuItem";
            resources.ApplyResources(this.SimProcessDetectedToolStripMenuItem, "SimProcessDetectedToolStripMenuItem");
            // 
            // separatorToolStripMenuItem
            // 
            this.separatorToolStripMenuItem.Name = "separatorToolStripMenuItem";
            resources.ApplyResources(this.separatorToolStripMenuItem, "separatorToolStripMenuItem");
            // 
            // FsuipcToolStripMenuItem
            // 
            this.FsuipcToolStripMenuItem.Name = "FsuipcToolStripMenuItem";
            resources.ApplyResources(this.FsuipcToolStripMenuItem, "FsuipcToolStripMenuItem");
            // 
            // simConnectToolStripMenuItem
            // 
            this.simConnectToolStripMenuItem.Name = "simConnectToolStripMenuItem";
            resources.ApplyResources(this.simConnectToolStripMenuItem, "simConnectToolStripMenuItem");
            // 
            // xPlaneDirectToolStripMenuItem
            // 
            this.xPlaneDirectToolStripMenuItem.Name = "xPlaneDirectToolStripMenuItem";
            resources.ApplyResources(this.xPlaneDirectToolStripMenuItem, "xPlaneDirectToolStripMenuItem");
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            // 
            // toolStripAircraftDropDownButton
            // 
            this.toolStripAircraftDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.linkCurrentConfigToolStripMenuItem,
            this.openLinkedConfigToolStripMenuItem,
            this.removeLinkConfigToolStripMenuItem,
            this.toolStripMenuItem5,
            this.autoloadToggleToolStripMenuItem});
            resources.ApplyResources(this.toolStripAircraftDropDownButton, "toolStripAircraftDropDownButton");
            this.toolStripAircraftDropDownButton.Name = "toolStripAircraftDropDownButton";
            this.toolStripAircraftDropDownButton.TextDirection = System.Windows.Forms.ToolStripTextDirection.Horizontal;
            // 
            // linkCurrentConfigToolStripMenuItem
            // 
            this.linkCurrentConfigToolStripMenuItem.Name = "linkCurrentConfigToolStripMenuItem";
            resources.ApplyResources(this.linkCurrentConfigToolStripMenuItem, "linkCurrentConfigToolStripMenuItem");
            this.linkCurrentConfigToolStripMenuItem.Click += new System.EventHandler(this.linkCurrentConfigToolStripMenuItem_Click);
            // 
            // openLinkedConfigToolStripMenuItem
            // 
            this.openLinkedConfigToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openLinkFilenameToolStripMenuItem});
            this.openLinkedConfigToolStripMenuItem.Name = "openLinkedConfigToolStripMenuItem";
            resources.ApplyResources(this.openLinkedConfigToolStripMenuItem, "openLinkedConfigToolStripMenuItem");
            // 
            // openLinkFilenameToolStripMenuItem
            // 
            this.openLinkFilenameToolStripMenuItem.Name = "openLinkFilenameToolStripMenuItem";
            resources.ApplyResources(this.openLinkFilenameToolStripMenuItem, "openLinkFilenameToolStripMenuItem");
            this.openLinkFilenameToolStripMenuItem.Click += new System.EventHandler(this.openLinkedConfigToolStripMenuItem_Click);
            // 
            // removeLinkConfigToolStripMenuItem
            // 
            this.removeLinkConfigToolStripMenuItem.Name = "removeLinkConfigToolStripMenuItem";
            resources.ApplyResources(this.removeLinkConfigToolStripMenuItem, "removeLinkConfigToolStripMenuItem");
            this.removeLinkConfigToolStripMenuItem.Click += new System.EventHandler(this.unlinkConfigToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            resources.ApplyResources(this.toolStripMenuItem5, "toolStripMenuItem5");
            // 
            // autoloadToggleToolStripMenuItem
            // 
            this.autoloadToggleToolStripMenuItem.Checked = true;
            this.autoloadToggleToolStripMenuItem.CheckOnClick = true;
            this.autoloadToggleToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoloadToggleToolStripMenuItem.Name = "autoloadToggleToolStripMenuItem";
            resources.ApplyResources(this.autoloadToggleToolStripMenuItem, "autoloadToggleToolStripMenuItem");
            this.autoloadToggleToolStripMenuItem.Click += new System.EventHandler(this.autoloadToggleToolStripMenuItem_Click);
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusLabel2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            resources.ApplyResources(this.toolStripStatusLabel2, "toolStripStatusLabel2");
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            resources.ApplyResources(this.toolStripStatusLabel, "toolStripStatusLabel");
            this.toolStripStatusLabel.Spring = true;
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.toolStripStatusLabel4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            resources.ApplyResources(this.toolStripStatusLabel4, "toolStripStatusLabel4");
            // 
            // toolStripStatusLabelHubHop
            // 
            resources.ApplyResources(this.toolStripStatusLabelHubHop, "toolStripStatusLabelHubHop");
            this.toolStripStatusLabelHubHop.Name = "toolStripStatusLabelHubHop";
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
            // logSplitter
            // 
            resources.ApplyResources(this.logSplitter, "logSplitter");
            this.logSplitter.Name = "logSplitter";
            this.logSplitter.TabStop = false;
            // 
            // frontendPanel1
            // 
            resources.ApplyResources(this.frontendPanel1, "frontendPanel1");
            this.frontendPanel1.Name = "frontendPanel1";
            // 
            // logPanel1
            // 
            resources.ApplyResources(this.logPanel1, "logPanel1");
            this.logPanel1.Name = "logPanel1";
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.logSplitter);
            this.Controls.Add(this.frontendPanel1);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.logPanel1);
            this.Controls.Add(this.statusStripPanel);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.contextMenuStripNotifyIcon.ResumeLayout(false);
            this.statusStripPanel.ResumeLayout(false);
            this.statusStripPanel.PerformLayout();
            this.statusStrip2.ResumeLayout(false);
            this.statusStrip2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripNotifyIcon;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItemDivider;
        private System.Windows.Forms.ToolStripMenuItem wiederherstellenToolStripMenuItem;
        private System.Windows.Forms.Panel statusStripPanel;
        private System.Windows.Forms.StatusStrip statusStrip2;
        private System.Windows.Forms.ToolStripStatusLabel SimConnectionIconStatusToolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel connectedDevicesToolStripLabel;
        private System.Windows.Forms.ToolStripDropDownButton connectedDevicesToolStripDropDownButton;
        private System.Windows.Forms.ToolStripStatusLabel dividerToolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.DataGridViewCheckBoxColumn activeDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn activeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn settingsDataGridViewTextBoxColumn;
        private System.Data.DataTable dataTable1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.Splitter logSplitter;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel SimStatusToolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripDropDownButton simStatusToolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem FsuipcToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem simConnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SimProcessDetectedToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator separatorToolStripMenuItem;
        private Panels.LogPanel logPanel1;
        private System.Windows.Forms.ImageList tabPageImageList;
        private System.Windows.Forms.ToolStripMenuItem xPlaneDirectToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripDropDownButton toolStripAircraftDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem removeLinkConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoloadToggleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem linkCurrentConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openLinkedConfigToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem openLinkFilenameToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelHubHop;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripMenuItem joysticksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MIDIDevicesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modulesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripConnectedDevicesIcon;
        private Panels.FrontendPanel frontendPanel1;
    }
}

