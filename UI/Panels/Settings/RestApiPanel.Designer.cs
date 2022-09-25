namespace MobiFlight.UI.Panels.Settings
{
    partial class RestApiPanel
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

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.RestServerPort = new System.Windows.Forms.TextBox();
            this.RestServerInterfaces = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.WebsocketServerPort = new System.Windows.Forms.TextBox();
            this.WebsocketServerInterfaces = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(258, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Configuration for the REST/WebSocket API Servers.";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.RestServerPort);
            this.groupBox1.Controls.Add(this.RestServerInterfaces);
            this.groupBox1.Location = new System.Drawing.Point(6, 41);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(528, 90);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "REST Server";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(39, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Listening Port";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Listening Interface";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // RestServerPort
            // 
            this.RestServerPort.Location = new System.Drawing.Point(116, 55);
            this.RestServerPort.Name = "RestServerPort";
            this.RestServerPort.Size = new System.Drawing.Size(69, 20);
            this.RestServerPort.TabIndex = 1;
            // 
            // RestServerInterfaces
            // 
            this.RestServerInterfaces.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RestServerInterfaces.FormattingEnabled = true;
            this.RestServerInterfaces.Location = new System.Drawing.Point(116, 19);
            this.RestServerInterfaces.Name = "RestServerInterfaces";
            this.RestServerInterfaces.Size = new System.Drawing.Size(162, 21);
            this.RestServerInterfaces.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.WebsocketServerPort);
            this.groupBox2.Controls.Add(this.WebsocketServerInterfaces);
            this.groupBox2.Location = new System.Drawing.Point(6, 137);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(528, 90);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Websocket Server";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(39, 58);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Listening Port";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 22);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Listening Interface";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // WebsocketServerPort
            // 
            this.WebsocketServerPort.Location = new System.Drawing.Point(116, 55);
            this.WebsocketServerPort.Name = "WebsocketServerPort";
            this.WebsocketServerPort.Size = new System.Drawing.Size(69, 20);
            this.WebsocketServerPort.TabIndex = 1;
            // 
            // WebsocketServerInterfaces
            // 
            this.WebsocketServerInterfaces.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.WebsocketServerInterfaces.FormattingEnabled = true;
            this.WebsocketServerInterfaces.Location = new System.Drawing.Point(116, 19);
            this.WebsocketServerInterfaces.Name = "WebsocketServerInterfaces";
            this.WebsocketServerInterfaces.Size = new System.Drawing.Size(162, 21);
            this.WebsocketServerInterfaces.TabIndex = 0;
            // 
            // RestApiPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Name = "RestApiPanel";
            this.Size = new System.Drawing.Size(537, 489);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox RestServerInterfaces;
        private System.Windows.Forms.TextBox RestServerPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox WebsocketServerPort;
        private System.Windows.Forms.ComboBox WebsocketServerInterfaces;
    }
}
