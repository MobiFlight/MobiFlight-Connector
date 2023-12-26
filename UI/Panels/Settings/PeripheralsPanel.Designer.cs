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
            this.listBoxJoysticks = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxMidiSettings = new System.Windows.Forms.GroupBox();
            this.listBoxMidiBoards = new System.Windows.Forms.ListBox();
            this.groupBoxJoystickSettings = new System.Windows.Forms.GroupBox();
            this.panelJoysticks = new System.Windows.Forms.Panel();
            this.labelDetectedJoysticks = new System.Windows.Forms.Label();
            this.labelDetectedMidiBoards = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBoxMidiSettings.SuspendLayout();
            this.groupBoxJoystickSettings.SuspendLayout();
            this.panelJoysticks.SuspendLayout();
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
            // listBoxJoysticks
            // 
            this.listBoxJoysticks.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.listBoxJoysticks, "listBoxJoysticks");
            this.listBoxJoysticks.FormattingEnabled = true;
            this.listBoxJoysticks.Name = "listBoxJoysticks";
            this.listBoxJoysticks.SelectionMode = System.Windows.Forms.SelectionMode.None;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.panelJoysticks, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxMidiSettings, 1, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // groupBoxMidiSettings
            // 
            this.groupBoxMidiSettings.Controls.Add(this.listBoxMidiBoards);
            this.groupBoxMidiSettings.Controls.Add(this.labelDetectedMidiBoards);
            this.groupBoxMidiSettings.Controls.Add(this.checkBoxMidiSupport);
            resources.ApplyResources(this.groupBoxMidiSettings, "groupBoxMidiSettings");
            this.groupBoxMidiSettings.Name = "groupBoxMidiSettings";
            this.groupBoxMidiSettings.TabStop = false;
            // 
            // listBoxMidiBoards
            // 
            this.listBoxMidiBoards.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.listBoxMidiBoards, "listBoxMidiBoards");
            this.listBoxMidiBoards.FormattingEnabled = true;
            this.listBoxMidiBoards.Name = "listBoxMidiBoards";
            this.listBoxMidiBoards.SelectionMode = System.Windows.Forms.SelectionMode.None;
            // 
            // groupBoxJoystickSettings
            // 
            this.groupBoxJoystickSettings.Controls.Add(this.listBoxJoysticks);
            this.groupBoxJoystickSettings.Controls.Add(this.labelDetectedJoysticks);
            this.groupBoxJoystickSettings.Controls.Add(this.checkBoxJoystickSupport);
            resources.ApplyResources(this.groupBoxJoystickSettings, "groupBoxJoystickSettings");
            this.groupBoxJoystickSettings.Name = "groupBoxJoystickSettings";
            this.groupBoxJoystickSettings.TabStop = false;
            // 
            // panelJoysticks
            // 
            this.panelJoysticks.Controls.Add(this.groupBoxJoystickSettings);
            resources.ApplyResources(this.panelJoysticks, "panelJoysticks");
            this.panelJoysticks.Name = "panelJoysticks";
            // 
            // labelDetectedJoysticks
            // 
            resources.ApplyResources(this.labelDetectedJoysticks, "labelDetectedJoysticks");
            this.labelDetectedJoysticks.Name = "labelDetectedJoysticks";
            // 
            // labelDetectedMidiBoards
            // 
            resources.ApplyResources(this.labelDetectedMidiBoards, "labelDetectedMidiBoards");
            this.labelDetectedMidiBoards.Name = "labelDetectedMidiBoards";
            // 
            // PeripheralsPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PeripheralsPanel";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBoxMidiSettings.ResumeLayout(false);
            this.groupBoxMidiSettings.PerformLayout();
            this.groupBoxJoystickSettings.ResumeLayout(false);
            this.groupBoxJoystickSettings.PerformLayout();
            this.panelJoysticks.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.CheckBox checkBoxJoystickSupport;
        private System.Windows.Forms.CheckBox checkBoxMidiSupport;
        private System.Windows.Forms.ListBox listBoxJoysticks;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBoxMidiSettings;
        private System.Windows.Forms.ListBox listBoxMidiBoards;
        private System.Windows.Forms.GroupBox groupBoxJoystickSettings;
        private System.Windows.Forms.Panel panelJoysticks;
        private System.Windows.Forms.Label labelDetectedJoysticks;
        private System.Windows.Forms.Label labelDetectedMidiBoards;
    }
}
