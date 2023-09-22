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
            this.checkBoxActive = new System.Windows.Forms.CheckBox();
            this.labelModifier = new System.Windows.Forms.Label();
            this.buttonUp = new System.Windows.Forms.Button();
            this.buttonDown = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panelDetails = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelColor = new System.Windows.Forms.Panel();
            this.labelModifierType = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.interpolationModifier1 = new MobiFlight.UI.Panels.Modifier.InterpolationModifierPanel();
            this.panelDetails.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBoxActive
            // 
            this.checkBoxActive.Location = new System.Drawing.Point(0, 0);
            this.checkBoxActive.Name = "checkBoxActive";
            this.checkBoxActive.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.checkBoxActive.Size = new System.Drawing.Size(51, 26);
            this.checkBoxActive.TabIndex = 1;
            this.checkBoxActive.Text = "use";
            this.checkBoxActive.UseVisualStyleBackColor = true;
            this.checkBoxActive.CheckedChanged += new System.EventHandler(this.checkBoxActive_CheckedChanged);
            // 
            // labelModifier
            // 
            this.labelModifier.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelModifier.AutoEllipsis = true;
            this.labelModifier.Location = new System.Drawing.Point(183, 0);
            this.labelModifier.Margin = new System.Windows.Forms.Padding(3);
            this.labelModifier.Name = "labelModifier";
            this.labelModifier.Padding = new System.Windows.Forms.Padding(3, 1, 3, 3);
            this.labelModifier.Size = new System.Drawing.Size(69, 26);
            this.labelModifier.TabIndex = 0;
            this.labelModifier.Text = "label1";
            this.labelModifier.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelModifier.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ModifierControl_MouseUp);
            // 
            // buttonUp
            // 
            this.buttonUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUp.FlatAppearance.BorderSize = 0;
            this.buttonUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonUp.Location = new System.Drawing.Point(322, 2);
            this.buttonUp.Name = "buttonUp";
            this.buttonUp.Size = new System.Drawing.Size(21, 21);
            this.buttonUp.TabIndex = 2;
            this.buttonUp.Text = "▲";
            this.buttonUp.UseVisualStyleBackColor = true;
            this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
            this.buttonUp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonDown_MouseDown);
            // 
            // buttonDown
            // 
            this.buttonDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDown.FlatAppearance.BorderSize = 0;
            this.buttonDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDown.Location = new System.Drawing.Point(346, 2);
            this.buttonDown.Margin = new System.Windows.Forms.Padding(0);
            this.buttonDown.Name = "buttonDown";
            this.buttonDown.Size = new System.Drawing.Size(21, 21);
            this.buttonDown.TabIndex = 3;
            this.buttonDown.Text = "▼";
            this.buttonDown.UseVisualStyleBackColor = true;
            this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
            this.buttonDown.MouseDown += new System.Windows.Forms.MouseEventHandler(this.buttonDown_MouseDown);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(378, 2);
            this.button1.Margin = new System.Windows.Forms.Padding(0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(21, 21);
            this.button1.TabIndex = 5;
            this.button1.Text = "X";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panelDetails
            // 
            this.panelDetails.AutoSize = true;
            this.panelDetails.Controls.Add(this.interpolationModifier1);
            this.panelDetails.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelDetails.Location = new System.Drawing.Point(0, 24);
            this.panelDetails.Name = "panelDetails";
            this.panelDetails.Padding = new System.Windows.Forms.Padding(185, 0, 0, 0);
            this.panelDetails.Size = new System.Drawing.Size(400, 71);
            this.panelDetails.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panelColor);
            this.panel1.Controls.Add(this.labelModifierType);
            this.panel1.Controls.Add(this.labelModifier);
            this.panel1.Controls.Add(this.checkBoxActive);
            this.panel1.Controls.Add(this.buttonUp);
            this.panel1.Controls.Add(this.buttonDown);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(400, 24);
            this.panel1.TabIndex = 1;
            // 
            // panelColor
            // 
            this.panelColor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.panelColor.Location = new System.Drawing.Point(50, 4);
            this.panelColor.Name = "panelColor";
            this.panelColor.Size = new System.Drawing.Size(16, 16);
            this.panelColor.TabIndex = 1;
            // 
            // labelModifierType
            // 
            this.labelModifierType.AutoEllipsis = true;
            this.labelModifierType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelModifierType.Location = new System.Drawing.Point(72, 0);
            this.labelModifierType.Margin = new System.Windows.Forms.Padding(3);
            this.labelModifierType.Name = "labelModifierType";
            this.labelModifierType.Padding = new System.Windows.Forms.Padding(3, 1, 3, 3);
            this.labelModifierType.Size = new System.Drawing.Size(117, 26);
            this.labelModifierType.TabIndex = 6;
            this.labelModifierType.Text = "Transformation";
            this.labelModifierType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelModifierType.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ModifierControl_MouseUp);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 95);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(400, 1);
            this.panel2.TabIndex = 7;
            // 
            // interpolationModifier1
            // 
            this.interpolationModifier1.AutoSize = true;
            this.interpolationModifier1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.interpolationModifier1.Dock = System.Windows.Forms.DockStyle.Top;
            this.interpolationModifier1.Location = new System.Drawing.Point(185, 0);
            this.interpolationModifier1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.interpolationModifier1.Name = "interpolationModifier1";
            this.interpolationModifier1.Size = new System.Drawing.Size(215, 71);
            this.interpolationModifier1.TabIndex = 0;
            // 
            // ModifierControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panelDetails);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(400, 2);
            this.Name = "ModifierControl";
            this.Size = new System.Drawing.Size(400, 96);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ModifierControl_MouseUp);
            this.panelDetails.ResumeLayout(false);
            this.panelDetails.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labelModifier;
        private System.Windows.Forms.CheckBox checkBoxActive;
        private System.Windows.Forms.Button buttonUp;
        private System.Windows.Forms.Button buttonDown;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panelDetails;
        private InterpolationModifierPanel interpolationModifier1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelModifierType;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panelColor;
    }
}
