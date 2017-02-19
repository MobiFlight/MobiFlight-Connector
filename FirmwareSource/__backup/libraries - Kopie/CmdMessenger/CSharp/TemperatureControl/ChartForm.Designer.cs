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
            this.GoalTemperatureValue = new System.Windows.Forms.Label();
            this.GoalTemperatureLabel = new System.Windows.Forms.Label();
            this.GoalTemperatureTrackBar = new System.Windows.Forms.TrackBar();
            this.buttonStopAcquisition = new System.Windows.Forms.Button();
            this.buttonStartAcquisition = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.GoalTemperatureTrackBar)).BeginInit();
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
            this.chartControl.Size = new System.Drawing.Size(896, 601);
            this.chartControl.TabIndex = 0;
            // 
            // GoalTemperatureValue
            // 
            this.GoalTemperatureValue.AutoSize = true;
            this.GoalTemperatureValue.Location = new System.Drawing.Point(889, 616);
            this.GoalTemperatureValue.Name = "GoalTemperatureValue";
            this.GoalTemperatureValue.Size = new System.Drawing.Size(19, 13);
            this.GoalTemperatureValue.TabIndex = 6;
            this.GoalTemperatureValue.Text = "20";
            // 
            // GoalTemperatureLabel
            // 
            this.GoalTemperatureLabel.AutoSize = true;
            this.GoalTemperatureLabel.Location = new System.Drawing.Point(10, 619);
            this.GoalTemperatureLabel.Name = "GoalTemperatureLabel";
            this.GoalTemperatureLabel.Size = new System.Drawing.Size(88, 13);
            this.GoalTemperatureLabel.TabIndex = 5;
            this.GoalTemperatureLabel.Text = "Goal temperature";
            // 
            // GoalTemperatureTrackBar
            // 
            this.GoalTemperatureTrackBar.Location = new System.Drawing.Point(104, 616);
            this.GoalTemperatureTrackBar.Maximum = 1000;
            this.GoalTemperatureTrackBar.Name = "GoalTemperatureTrackBar";
            this.GoalTemperatureTrackBar.Size = new System.Drawing.Size(779, 45);
            this.GoalTemperatureTrackBar.TabIndex = 2;
            this.GoalTemperatureTrackBar.Tag = "";
            this.GoalTemperatureTrackBar.TickFrequency = 10;
            this.GoalTemperatureTrackBar.Value = 200;
            this.GoalTemperatureTrackBar.Scroll += new System.EventHandler(this.GoalTemperatureTrackBarScroll);
            // 
            // buttonStopAcquisition
            // 
            this.buttonStopAcquisition.Location = new System.Drawing.Point(117, 657);
            this.buttonStopAcquisition.Name = "buttonStopAcquisition";
            this.buttonStopAcquisition.Size = new System.Drawing.Size(98, 35);
            this.buttonStopAcquisition.TabIndex = 7;
            this.buttonStopAcquisition.Text = "Stop acquisition";
            this.buttonStopAcquisition.UseVisualStyleBackColor = true;
            this.buttonStopAcquisition.Click += new System.EventHandler(this.buttonStopAcquisition_Click);
            // 
            // buttonStartAcquisition
            // 
            this.buttonStartAcquisition.Location = new System.Drawing.Point(13, 657);
            this.buttonStartAcquisition.Name = "buttonStartAcquisition";
            this.buttonStartAcquisition.Size = new System.Drawing.Size(98, 35);
            this.buttonStartAcquisition.TabIndex = 8;
            this.buttonStartAcquisition.Text = "Start acquisition";
            this.buttonStartAcquisition.UseVisualStyleBackColor = true;
            this.buttonStartAcquisition.Click += new System.EventHandler(this.buttonStartAcquisition_Click);
            // 
            // ChartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 704);
            this.Controls.Add(this.buttonStartAcquisition);
            this.Controls.Add(this.buttonStopAcquisition);
            this.Controls.Add(this.GoalTemperatureValue);
            this.Controls.Add(this.GoalTemperatureLabel);
            this.Controls.Add(this.GoalTemperatureTrackBar);
            this.Controls.Add(this.chartControl);
            this.Name = "ChartForm";
            this.Text = "Temperature Controller";
            this.Shown += new System.EventHandler(this.ChartFormShown);
            ((System.ComponentModel.ISupportInitialize)(this.GoalTemperatureTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public ZedGraph.ZedGraphControl chartControl;
        private System.Windows.Forms.Label GoalTemperatureValue;
        private System.Windows.Forms.Label GoalTemperatureLabel;
        private System.Windows.Forms.TrackBar GoalTemperatureTrackBar;
        private System.Windows.Forms.Button buttonStopAcquisition;
        private System.Windows.Forms.Button buttonStartAcquisition;
    }
}

