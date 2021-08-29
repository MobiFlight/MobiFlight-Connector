namespace DataLogging
{
    partial class ChartForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.chartControl = new ZedGraph.ZedGraphControl();
            this.SuspendLayout();
            // 
            // chartControl
            // 
            this.chartControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chartControl.IsAntiAlias = true;
            this.chartControl.Location = new System.Drawing.Point(12, 12);
            this.chartControl.Name = "chartControl";
            this.chartControl.ScrollGrace = 0D;
            this.chartControl.ScrollMaxX = 0D;
            this.chartControl.ScrollMaxY = 0D;
            this.chartControl.ScrollMaxY2 = 0D;
            this.chartControl.ScrollMinX = 0D;
            this.chartControl.ScrollMinY = 0D;
            this.chartControl.ScrollMinY2 = 0D;
            this.chartControl.Size = new System.Drawing.Size(521, 442);
            this.chartControl.TabIndex = 0;
            // 
            // ChartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 466);
            this.Controls.Add(this.chartControl);
            this.Name = "ChartForm";
            this.Text = "Data Logging and Charting";
            this.ResumeLayout(false);

        }

        #endregion

        public ZedGraph.ZedGraphControl chartControl;
    }
}

