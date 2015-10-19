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
            this.label1 = new System.Windows.Forms.Label();
            this.arduinoDownloadLinkLabel = new System.Windows.Forms.LinkLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.firmwareArduinoIdeButton = new System.Windows.Forms.Button();
            this.firmwareArduinoIdePathTextBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(517, 66);
            this.label1.TabIndex = 0;
            this.label1.Text = "The path to your Arduino IDE is not set. This is needed to upload the MobiFlight " +
    "Firmware. In case you have not downloaded the Arduino IDE yet:";
            // 
            // arduinoDownloadLinkLabel
            // 
            this.arduinoDownloadLinkLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.arduinoDownloadLinkLabel.Location = new System.Drawing.Point(3, 69);
            this.arduinoDownloadLinkLabel.Name = "arduinoDownloadLinkLabel";
            this.arduinoDownloadLinkLabel.Size = new System.Drawing.Size(517, 23);
            this.arduinoDownloadLinkLabel.TabIndex = 1;
            this.arduinoDownloadLinkLabel.TabStop = true;
            this.arduinoDownloadLinkLabel.Text = "Download Arduino IDE now";
            this.arduinoDownloadLinkLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.firmwareArduinoIdeButton);
            this.panel1.Controls.Add(this.firmwareArduinoIdePathTextBox);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 92);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(517, 65);
            this.panel1.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(179, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Set path to Arduino IDE";
            // 
            // firmwareArduinoIdeButton
            // 
            this.firmwareArduinoIdeButton.Image = global::MobiFlight.Properties.Resources.folder1;
            this.firmwareArduinoIdeButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.firmwareArduinoIdeButton.Location = new System.Drawing.Point(470, 19);
            this.firmwareArduinoIdeButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.firmwareArduinoIdeButton.Name = "firmwareArduinoIdeButton";
            this.firmwareArduinoIdeButton.Size = new System.Drawing.Size(40, 35);
            this.firmwareArduinoIdeButton.TabIndex = 4;
            this.firmwareArduinoIdeButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.firmwareArduinoIdeButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.firmwareArduinoIdeButton.UseVisualStyleBackColor = true;
            this.firmwareArduinoIdeButton.Click += new System.EventHandler(this.firmwareArduinoIdeButton_Click);
            // 
            // firmwareArduinoIdePathTextBox
            // 
            this.firmwareArduinoIdePathTextBox.Location = new System.Drawing.Point(195, 22);
            this.firmwareArduinoIdePathTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.firmwareArduinoIdePathTextBox.Name = "firmwareArduinoIdePathTextBox";
            this.firmwareArduinoIdePathTextBox.Size = new System.Drawing.Size(267, 26);
            this.firmwareArduinoIdePathTextBox.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(263, 167);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(121, 36);
            this.button1.TabIndex = 3;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(390, 167);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(121, 36);
            this.button2.TabIndex = 4;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // ArduinoIdePathForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(523, 210);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.arduinoDownloadLinkLabel);
            this.Controls.Add(this.label1);
            this.Name = "ArduinoIdePathForm";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Automatic Firmware Update";
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