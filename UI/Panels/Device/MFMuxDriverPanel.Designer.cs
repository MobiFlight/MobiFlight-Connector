namespace MobiFlight.UI.Panels.Settings
{
    partial class MFMuxDriverPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MFMuxDriverPanel));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.mfPinS0ComboBox = new System.Windows.Forms.ComboBox();
            this.mfPinS1ComboBox = new System.Windows.Forms.ComboBox();
            this.mfPinS2ComboBox = new System.Windows.Forms.ComboBox();
            this.mfPinS3ComboBox = new System.Windows.Forms.ComboBox();
            this.mfPinS0Label = new System.Windows.Forms.Label();
            this.mfPinS1Label = new System.Windows.Forms.Label();
            this.mfPinS2Label = new System.Windows.Forms.Label();
            this.mfPinS3Label = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.mfPinS0Label);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.mfPinS0ComboBox);
            this.groupBox1.Controls.Add(this.mfPinS1ComboBox);
            this.groupBox1.Controls.Add(this.mfPinS2ComboBox);
            this.groupBox1.Controls.Add(this.mfPinS3ComboBox);
            this.groupBox1.Controls.Add(this.mfPinS1Label);
            this.groupBox1.Controls.Add(this.mfPinS2Label);
            this.groupBox1.Controls.Add(this.mfPinS3Label);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // mfPinS0ComboBox
            // 
            this.mfPinS0ComboBox.BackColor = System.Drawing.SystemColors.Window;
            this.mfPinS0ComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.mfPinS0ComboBox, "mfPinS0ComboBox");
            this.mfPinS0ComboBox.Name = "mfPinS0ComboBox";
            this.mfPinS0ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            this.mfPinS0ComboBox.SelectedValueChanged += new System.EventHandler(this.value_Changed);
            // 
            // mfPinS1ComboBox
            // 
            this.mfPinS1ComboBox.BackColor = System.Drawing.SystemColors.Window;
            this.mfPinS1ComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.mfPinS1ComboBox, "mfPinS1ComboBox");
            this.mfPinS1ComboBox.Name = "mfPinS1ComboBox";
            this.mfPinS1ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            this.mfPinS1ComboBox.SelectedValueChanged += new System.EventHandler(this.value_Changed);
            // 
            // mfPinS2ComboBox
            // 
            this.mfPinS2ComboBox.BackColor = System.Drawing.SystemColors.Window;
            this.mfPinS2ComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.mfPinS2ComboBox, "mfPinS2ComboBox");
            this.mfPinS2ComboBox.Name = "mfPinS2ComboBox";
            this.mfPinS2ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            this.mfPinS2ComboBox.SelectedValueChanged += new System.EventHandler(this.value_Changed);
            // 
            // mfPinS3ComboBox
            // 
            this.mfPinS3ComboBox.BackColor = System.Drawing.SystemColors.Window;
            this.mfPinS3ComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.mfPinS3ComboBox, "mfPinS3ComboBox");
            this.mfPinS3ComboBox.Name = "mfPinS3ComboBox";
            this.mfPinS3ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            this.mfPinS3ComboBox.SelectedValueChanged += new System.EventHandler(this.value_Changed);
            // 
            // mfPinS0Label
            // 
            resources.ApplyResources(this.mfPinS0Label, "mfPinS0Label");
            this.mfPinS0Label.Name = "mfPinS0Label";
            // 
            // mfPinS1Label
            // 
            resources.ApplyResources(this.mfPinS1Label, "mfPinS1Label");
            this.mfPinS1Label.Name = "mfPinS1Label";
            // 
            // mfPinS2Label
            // 
            resources.ApplyResources(this.mfPinS2Label, "mfPinS2Label");
            this.mfPinS2Label.Name = "mfPinS2Label";
            // 
            // mfPinS3Label
            // 
            resources.ApplyResources(this.mfPinS3Label, "mfPinS3Label");
            this.mfPinS3Label.Name = "mfPinS3Label";
            // 
            // MFMuxDriverPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "MFMuxDriverPanel";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label mfPinS0Label;
        private System.Windows.Forms.Label mfPinS1Label;
        private System.Windows.Forms.Label mfPinS2Label;
        private System.Windows.Forms.Label mfPinS3Label;
        private System.Windows.Forms.ComboBox mfPinS0ComboBox;
        private System.Windows.Forms.ComboBox mfPinS1ComboBox;
        private System.Windows.Forms.ComboBox mfPinS2ComboBox;
        private System.Windows.Forms.ComboBox mfPinS3ComboBox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}