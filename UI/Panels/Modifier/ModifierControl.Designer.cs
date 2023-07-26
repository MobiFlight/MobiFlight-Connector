namespace MobiFlight.UI.Panels.Modifier
{
    partial class ModifierControl
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panelDragHandle = new System.Windows.Forms.Panel();
            this.checkBoxActive = new System.Windows.Forms.CheckBox();
            this.labelModifier = new System.Windows.Forms.Label();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.Controls.Add(this.panelDragHandle, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.checkBoxActive, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelModifier, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonUp, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonDown, 4, 0);
            this.tableLayoutPanel1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(252, 22);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panelDragHandle
            // 
            this.panelDragHandle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelDragHandle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDragHandle.Location = new System.Drawing.Point(1, 1);
            this.panelDragHandle.Margin = new System.Windows.Forms.Padding(1);
            this.panelDragHandle.Name = "panelDragHandle";
            this.panelDragHandle.Size = new System.Drawing.Size(10, 20);
            this.panelDragHandle.TabIndex = 4;
            // 
            // checkBoxActive
            // 
            this.checkBoxActive.AutoSize = true;
            this.checkBoxActive.Dock = System.Windows.Forms.DockStyle.Left;
            this.checkBoxActive.Location = new System.Drawing.Point(15, 3);
            this.checkBoxActive.Name = "checkBoxActive";
            this.checkBoxActive.Size = new System.Drawing.Size(15, 16);
            this.checkBoxActive.TabIndex = 1;
            this.checkBoxActive.UseVisualStyleBackColor = true;
            // 
            // labelModifier
            // 
            this.labelModifier.AutoEllipsis = true;
            this.labelModifier.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelModifier.Location = new System.Drawing.Point(39, 0);
            this.labelModifier.Name = "labelModifier";
            this.labelModifier.Size = new System.Drawing.Size(146, 22);
            this.labelModifier.TabIndex = 0;
            this.labelModifier.Text = "label1";
            this.labelModifier.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buttonUp
            // 
            this.buttonUp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonUp.FlatAppearance.BorderSize = 0;
            this.buttonUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonUp.Location = new System.Drawing.Point(188, 0);
            this.buttonUp.Margin = new System.Windows.Forms.Padding(0);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(32, 22);
            this.buttonUp.TabIndex = 2;
            this.buttonUp.Text = "▲";
            this.buttonUp.UseVisualStyleBackColor = true;
            // 
            // buttonDown
            // 
            this.buttonDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonDown.FlatAppearance.BorderSize = 0;
            this.buttonDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDown.Location = new System.Drawing.Point(220, 0);
            this.buttonDown.Margin = new System.Windows.Forms.Padding(0);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(32, 22);
            this.buttonDown.TabIndex = 3;
            this.buttonDown.Text = "▼";
            this.buttonDown.UseVisualStyleBackColor = true;
            // 
            // ModifierControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ModifierControl";
            this.Size = new System.Drawing.Size(252, 22);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelModifier;
        private System.Windows.Forms.CheckBox checkBoxActive;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.Panel panelDragHandle;
    }
}
