namespace MobiFlight
{
    partial class ArduinoIdePathForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ArduinoIdePathForm));
            this.label1 = new System.Windows.Forms.Label();
            this.arduinoDownloadLinkLabel = new System.Windows.Forms.LinkLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.firmwareArduinoIdeButton = new System.Windows.Forms.Button();
            this.firmwareArduinoIdePathTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // arduinoDownloadLinkLabel
            // 
            resources.ApplyResources(this.arduinoDownloadLinkLabel, "arduinoDownloadLinkLabel");
            this.arduinoDownloadLinkLabel.Name = "arduinoDownloadLinkLabel";
            this.arduinoDownloadLinkLabel.TabStop = true;
            this.arduinoDownloadLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.arduinoDownloadLinkLabel_LinkClicked);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.firmwareArduinoIdeButton);
            this.panel1.Controls.Add(this.firmwareArduinoIdePathTextBox);
            this.panel1.Controls.Add(this.label2);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // firmwareArduinoIdeButton
            // 
            this.firmwareArduinoIdeButton.Image = global::MobiFlight.Properties.Resources.folder1;
            resources.ApplyResources(this.firmwareArduinoIdeButton, "firmwareArduinoIdeButton");
            this.firmwareArduinoIdeButton.Name = "firmwareArduinoIdeButton";
            this.firmwareArduinoIdeButton.UseVisualStyleBackColor = true;
            this.firmwareArduinoIdeButton.Click += new System.EventHandler(this.firmwareArduinoIdeButton_Click);
            // 
            // firmwareArduinoIdePathTextBox
            // 
            resources.ApplyResources(this.firmwareArduinoIdePathTextBox, "firmwareArduinoIdePathTextBox");
            this.firmwareArduinoIdePathTextBox.Name = "firmwareArduinoIdePathTextBox";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // ArduinoIdePathForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.arduinoDownloadLinkLabel);
            this.Controls.Add(this.label1);
            this.Name = "ArduinoIdePathForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel arduinoDownloadLinkLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button firmwareArduinoIdeButton;
        private System.Windows.Forms.TextBox firmwareArduinoIdePathTextBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}