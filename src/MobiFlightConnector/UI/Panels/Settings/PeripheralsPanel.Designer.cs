namespace MobiFlight.UI.Panels.Settings
{
    partial class PeripheralsPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PeripheralsPanel));
            this.checkBoxMidiSupport = new System.Windows.Forms.CheckBox();
            this.checkBoxJoystickSupport = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panelJoysticks = new System.Windows.Forms.Panel();
            this.groupBoxJoystickSettings = new System.Windows.Forms.GroupBox();
            this.listBoxJoysticks = new System.Windows.Forms.CheckedListBox();
            this.labelJoysticks = new System.Windows.Forms.Label();
            this.groupBoxMidiSettings = new System.Windows.Forms.GroupBox();
            this.listBoxMidiBoards = new System.Windows.Forms.CheckedListBox();
            this.labelMidiBoards = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelJoysticks.SuspendLayout();
            this.groupBoxJoystickSettings.SuspendLayout();
            this.groupBoxMidiSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxMidiSupport
            // 
            resources.ApplyResources(this.checkBoxMidiSupport, "checkBoxMidiSupport");
            this.checkBoxMidiSupport.Name = "checkBoxMidiSupport";
            this.checkBoxMidiSupport.UseVisualStyleBackColor = true;
            // 
            // checkBoxJoystickSupport
            // 
            resources.ApplyResources(this.checkBoxJoystickSupport, "checkBoxJoystickSupport");
            this.checkBoxJoystickSupport.Name = "checkBoxJoystickSupport";
            this.checkBoxJoystickSupport.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.panelJoysticks, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxMidiSettings, 1, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // panelJoysticks
            // 
            this.panelJoysticks.Controls.Add(this.groupBoxJoystickSettings);
            resources.ApplyResources(this.panelJoysticks, "panelJoysticks");
            this.panelJoysticks.Name = "panelJoysticks";
            // 
            // groupBoxJoystickSettings
            // 
            this.groupBoxJoystickSettings.Controls.Add(this.listBoxJoysticks);
            this.groupBoxJoystickSettings.Controls.Add(this.labelJoysticks);
            this.groupBoxJoystickSettings.Controls.Add(this.checkBoxJoystickSupport);
            resources.ApplyResources(this.groupBoxJoystickSettings, "groupBoxJoystickSettings");
            this.groupBoxJoystickSettings.Name = "groupBoxJoystickSettings";
            this.groupBoxJoystickSettings.TabStop = false;
            // 
            // listBoxJoysticks
            // 
            this.listBoxJoysticks.CheckOnClick = true;
            resources.ApplyResources(this.listBoxJoysticks, "listBoxJoysticks");
            this.listBoxJoysticks.FormattingEnabled = true;
            this.listBoxJoysticks.Name = "listBoxJoysticks";
            // 
            // labelJoysticks
            // 
            resources.ApplyResources(this.labelJoysticks, "labelJoysticks");
            this.labelJoysticks.Name = "labelJoysticks";
            // 
            // groupBoxMidiSettings
            // 
            this.groupBoxMidiSettings.Controls.Add(this.listBoxMidiBoards);
            this.groupBoxMidiSettings.Controls.Add(this.labelMidiBoards);
            this.groupBoxMidiSettings.Controls.Add(this.checkBoxMidiSupport);
            resources.ApplyResources(this.groupBoxMidiSettings, "groupBoxMidiSettings");
            this.groupBoxMidiSettings.Name = "groupBoxMidiSettings";
            this.groupBoxMidiSettings.TabStop = false;
            // 
            // listBoxMidiBoards
            // 
            this.listBoxMidiBoards.CheckOnClick = true;
            resources.ApplyResources(this.listBoxMidiBoards, "listBoxMidiBoards");
            this.listBoxMidiBoards.FormattingEnabled = true;
            this.listBoxMidiBoards.Name = "listBoxMidiBoards";
            // 
            // labelMidiBoards
            // 
            resources.ApplyResources(this.labelMidiBoards, "labelMidiBoards");
            this.labelMidiBoards.Name = "labelMidiBoards";
            // 
            // PeripheralsPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PeripheralsPanel";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panelJoysticks.ResumeLayout(false);
            this.groupBoxJoystickSettings.ResumeLayout(false);
            this.groupBoxJoystickSettings.PerformLayout();
            this.groupBoxMidiSettings.ResumeLayout(false);
            this.groupBoxMidiSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.CheckBox checkBoxJoystickSupport;
        private System.Windows.Forms.CheckBox checkBoxMidiSupport;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBoxMidiSettings;
        private System.Windows.Forms.GroupBox groupBoxJoystickSettings;
        private System.Windows.Forms.Panel panelJoysticks;
        private System.Windows.Forms.Label labelJoysticks;
        private System.Windows.Forms.Label labelMidiBoards;
        private System.Windows.Forms.CheckedListBox listBoxMidiBoards;
        private System.Windows.Forms.CheckedListBox listBoxJoysticks;
    }
}
