namespace MobiFlight.UI.Panels.Settings
{
    partial class JoystickMidiPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JoystickMidiPanel));
            this.groupBoxActivation = new System.Windows.Forms.GroupBox();
            this.checkBoxMidiSupport = new System.Windows.Forms.CheckBox();
            this.checkBoxJoystickSupport = new System.Windows.Forms.CheckBox();
            this.groupBoxActivation.SuspendLayout();
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
            // JoystickMidiPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxActivation);
            this.Name = "JoystickMidiPanel";
            this.groupBoxActivation.ResumeLayout(false);
            this.groupBoxActivation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxActivation;
        private System.Windows.Forms.CheckBox checkBoxJoystickSupport;
        private System.Windows.Forms.CheckBox checkBoxMidiSupport;
    }
}
