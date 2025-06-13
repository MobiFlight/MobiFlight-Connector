namespace MobiFlight.UI.Panels.Config
{
    partial class ProSimDatarefPanel
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
            this.DatarefPathTextBox = new System.Windows.Forms.TextBox();
            this.DataRef = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dataRefDescriptionsComboBox = new MobiFlight.UI.Panels.Config.AutoCompleteComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.transformOptionsGroup1 = new MobiFlight.UI.Panels.Config.TransformOptionsGroup();
            this.simProGroupBox1 = new System.Windows.Forms.GroupBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.searchLabel = new System.Windows.Forms.Label();
            this.simProGroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // DatarefPathTextBox
            // 
            this.DatarefPathTextBox.Location = new System.Drawing.Point(62, 17);
            this.DatarefPathTextBox.Name = "DatarefPathTextBox";
            this.DatarefPathTextBox.Size = new System.Drawing.Size(240, 20);
            this.DatarefPathTextBox.TabIndex = 1;
            // 
            // DataRef
            // 
            this.DataRef.AutoSize = true;
            this.DataRef.Location = new System.Drawing.Point(6, 20);
            this.DataRef.Name = "DataRef";
            this.DataRef.Size = new System.Drawing.Size(50, 13);
            this.DataRef.TabIndex = 3;
            this.DataRef.Text = "DataRef:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Preset:";
            // 
            // dataRefDescriptionsComboBox
            // 
            this.dataRefDescriptionsComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.dataRefDescriptionsComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.dataRefDescriptionsComboBox.FormattingEnabled = true;
            this.dataRefDescriptionsComboBox.Location = new System.Drawing.Point(62, 44);
            this.dataRefDescriptionsComboBox.Name = "dataRefDescriptionsComboBox";
            this.dataRefDescriptionsComboBox.Size = new System.Drawing.Size(240, 21);
            this.dataRefDescriptionsComboBox.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(308, 42);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Refresh";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // transformOptionsGroup1
            // 
            this.transformOptionsGroup1.AutoSize = true;
            this.transformOptionsGroup1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.transformOptionsGroup1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.transformOptionsGroup1.Location = new System.Drawing.Point(0, 284);
            this.transformOptionsGroup1.MinimumSize = new System.Drawing.Size(350, 0);
            this.transformOptionsGroup1.Name = "transformOptionsGroup1";
            this.transformOptionsGroup1.Size = new System.Drawing.Size(573, 81);
            this.transformOptionsGroup1.TabIndex = 2;
            // 
            // simProGroupBox1
            // 
            this.simProGroupBox1.AutoSize = true;
            this.simProGroupBox1.Controls.Add(this.searchLabel);
            this.simProGroupBox1.Controls.Add(this.textBox1);
            this.simProGroupBox1.Controls.Add(this.dataGridView1);
            this.simProGroupBox1.Controls.Add(this.DataRef);
            this.simProGroupBox1.Controls.Add(this.button1);
            this.simProGroupBox1.Controls.Add(this.DatarefPathTextBox);
            this.simProGroupBox1.Controls.Add(this.dataRefDescriptionsComboBox);
            this.simProGroupBox1.Controls.Add(this.label2);
            this.simProGroupBox1.Location = new System.Drawing.Point(3, 3);
            this.simProGroupBox1.Name = "simProGroupBox1";
            this.simProGroupBox1.Size = new System.Drawing.Size(567, 275);
            this.simProGroupBox1.TabIndex = 7;
            this.simProGroupBox1.TabStop = false;
            this.simProGroupBox1.Text = "SimPro";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(9, 101);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(540, 145);
            this.dataGridView1.TabIndex = 7;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(9, 76);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(540, 20);
            this.textBox1.TabIndex = 8;
            // 
            // searchLabel
            // 
            this.searchLabel.AutoSize = true;
            this.searchLabel.Location = new System.Drawing.Point(9, 60);
            this.searchLabel.Name = "searchLabel";
            this.searchLabel.Size = new System.Drawing.Size(44, 13);
            this.searchLabel.TabIndex = 9;
            this.searchLabel.Text = "Search:";
            // 
            // ProSimDatarefPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.simProGroupBox1);
            this.Controls.Add(this.transformOptionsGroup1);
            this.Name = "ProSimDatarefPanel";
            this.Size = new System.Drawing.Size(573, 365);
            this.Load += new System.EventHandler(this.ProSimDatarefPanel_Load);
            this.simProGroupBox1.ResumeLayout(false);
            this.simProGroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox DatarefPathTextBox;
        private TransformOptionsGroup transformOptionsGroup1;
        private System.Windows.Forms.Label DataRef;
        private System.Windows.Forms.Label label2;
        private AutoCompleteComboBox dataRefDescriptionsComboBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox simProGroupBox1;
        private System.Windows.Forms.Label searchLabel;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
} 