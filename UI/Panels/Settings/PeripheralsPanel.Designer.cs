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
            this.groupBoxActivation = new System.Windows.Forms.GroupBox();
            this.checkBoxMidiSupport = new System.Windows.Forms.CheckBox();
            this.checkBoxJoystickSupport = new System.Windows.Forms.CheckBox();
            this.listBoxJoysticks = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBoxMidiBoards = new System.Windows.Forms.GroupBox();
            this.listBoxMidiBoards = new System.Windows.Forms.ListBox();
            this.groupBoxJoysticks = new System.Windows.Forms.GroupBox();
            this.groupBoxActivation.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBoxMidiBoards.SuspendLayout();
            this.groupBoxJoysticks.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxActivation
            // 
            this.groupBoxActivation.Controls.Add(this.checkBoxMidiSupport);
            this.groupBoxActivation.Controls.Add(this.checkBoxJoystickSupport);
            resources.ApplyResources(this.groupBoxActivation, "groupBoxActivation");
            this.groupBoxActivation.Name = "groupBoxActivation";
            this.groupBoxActivation.TabStop = false;
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
            resources.ApplyResources(this.listBoxJoysticks, "listBoxJoysticks");
            this.listBoxJoysticks.FormattingEnabled = true;
            this.listBoxJoysticks.Name = "listBoxJoysticks";
            this.listBoxJoysticks.SelectionMode = System.Windows.Forms.SelectionMode.None;
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.groupBoxMidiBoards, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBoxJoysticks, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // groupBoxMidiBoards
            // 
            this.groupBoxMidiBoards.Controls.Add(this.listBoxMidiBoards);
            resources.ApplyResources(this.groupBoxMidiBoards, "groupBoxMidiBoards");
            this.groupBoxMidiBoards.Name = "groupBoxMidiBoards";
            this.groupBoxMidiBoards.TabStop = false;
            // 
            // listBoxMidiBoards
            // 
            resources.ApplyResources(this.listBoxMidiBoards, "listBoxMidiBoards");
            this.listBoxMidiBoards.FormattingEnabled = true;
            this.listBoxMidiBoards.Name = "listBoxMidiBoards";
            this.listBoxMidiBoards.SelectionMode = System.Windows.Forms.SelectionMode.None;
            // 
            // groupBoxJoysticks
            // 
            this.groupBoxJoysticks.Controls.Add(this.listBoxJoysticks);
            resources.ApplyResources(this.groupBoxJoysticks, "groupBoxJoysticks");
            this.groupBoxJoysticks.Name = "groupBoxJoysticks";
            this.groupBoxJoysticks.TabStop = false;
            // 
            // PeripheralsPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.groupBoxActivation);
            this.Name = "PeripheralsPanel";
            this.groupBoxActivation.ResumeLayout(false);
            this.groupBoxActivation.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBoxMidiBoards.ResumeLayout(false);
            this.groupBoxJoysticks.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxActivation;
        private System.Windows.Forms.CheckBox checkBoxJoystickSupport;
        private System.Windows.Forms.CheckBox checkBoxMidiSupport;
        private System.Windows.Forms.ListBox listBoxJoysticks;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBoxMidiBoards;
        private System.Windows.Forms.ListBox listBoxMidiBoards;
        private System.Windows.Forms.GroupBox groupBoxJoysticks;
    }
}
