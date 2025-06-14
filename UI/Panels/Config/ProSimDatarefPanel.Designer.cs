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
            this.simProGroupBox1 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DataType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.searchLabel = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.transformOptionsGroup1 = new MobiFlight.UI.Panels.Config.TransformOptionsGroup();
            this.simProGroupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // DatarefPathTextBox
            // 
            this.DatarefPathTextBox.Location = new System.Drawing.Point(59, 14);
            this.DatarefPathTextBox.Name = "DatarefPathTextBox";
            this.DatarefPathTextBox.Size = new System.Drawing.Size(240, 20);
            this.DatarefPathTextBox.TabIndex = 1;
            // 
            // DataRef
            // 
            this.DataRef.AutoSize = true;
            this.DataRef.Location = new System.Drawing.Point(3, 17);
            this.DataRef.Name = "DataRef";
            this.DataRef.Size = new System.Drawing.Size(50, 13);
            this.DataRef.TabIndex = 3;
            this.DataRef.Text = "DataRef:";
            // 
            // simProGroupBox1
            // 
            this.simProGroupBox1.AutoSize = true;
            this.simProGroupBox1.Controls.Add(this.panel1);
            this.simProGroupBox1.Controls.Add(this.searchLabel);
            this.simProGroupBox1.Controls.Add(this.textBox1);
            this.simProGroupBox1.Controls.Add(this.DataRef);
            this.simProGroupBox1.Controls.Add(this.DatarefPathTextBox);
            this.simProGroupBox1.Location = new System.Drawing.Point(3, 3);
            this.simProGroupBox1.Name = "simProGroupBox1";
            this.simProGroupBox1.Padding = new System.Windows.Forms.Padding(0);
            this.simProGroupBox1.Size = new System.Drawing.Size(612, 347);
            this.simProGroupBox1.TabIndex = 7;
            this.simProGroupBox1.TabStop = false;
            this.simProGroupBox1.Text = "SimPro";
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Location = new System.Drawing.Point(6, 83);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(603, 248);
            this.panel1.TabIndex = 10;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameColumn,
            this.Description,
            this.DataType});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(0);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.ShowEditingIcon = false;
            this.dataGridView1.Size = new System.Drawing.Size(603, 248);
            this.dataGridView1.TabIndex = 7;
            this.dataGridView1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_RowEnter);
            // 
            // NameColumn
            // 
            this.NameColumn.DataPropertyName = "Name";
            this.NameColumn.HeaderText = "Name";
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.ReadOnly = true;
            this.NameColumn.Width = 130;
            // 
            // Description
            // 
            this.Description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Description.DataPropertyName = "Description";
            this.Description.HeaderText = "Description";
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            // 
            // DataType
            // 
            this.DataType.DataPropertyName = "DataType";
            this.DataType.HeaderText = "Data Type";
            this.DataType.Name = "DataType";
            this.DataType.ReadOnly = true;
            // 
            // searchLabel
            // 
            this.searchLabel.AutoSize = true;
            this.searchLabel.Location = new System.Drawing.Point(3, 37);
            this.searchLabel.Name = "searchLabel";
            this.searchLabel.Size = new System.Drawing.Size(44, 13);
            this.searchLabel.TabIndex = 9;
            this.searchLabel.Text = "Search:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 56);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(603, 20);
            this.textBox1.TabIndex = 8;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // transformOptionsGroup1
            // 
            this.transformOptionsGroup1.AutoSize = true;
            this.transformOptionsGroup1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.transformOptionsGroup1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.transformOptionsGroup1.Location = new System.Drawing.Point(0, 337);
            this.transformOptionsGroup1.MinimumSize = new System.Drawing.Size(350, 0);
            this.transformOptionsGroup1.Name = "transformOptionsGroup1";
            this.transformOptionsGroup1.Size = new System.Drawing.Size(618, 81);
            this.transformOptionsGroup1.TabIndex = 2;
            // 
            // ProSimDatarefPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.simProGroupBox1);
            this.Controls.Add(this.transformOptionsGroup1);
            this.Name = "ProSimDatarefPanel";
            this.Size = new System.Drawing.Size(618, 418);
            this.simProGroupBox1.ResumeLayout(false);
            this.simProGroupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox DatarefPathTextBox;
        private TransformOptionsGroup transformOptionsGroup1;
        private System.Windows.Forms.Label DataRef;
        private System.Windows.Forms.GroupBox simProGroupBox1;
        private System.Windows.Forms.Label searchLabel;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataType;
        private System.Windows.Forms.Panel panel1;
    }
} 