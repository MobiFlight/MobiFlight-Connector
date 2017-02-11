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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelProgress = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.loggingView1 = new Tools.LoggingView();
            ((System.ComponentModel.ISupportInitialize)(this.GoalTemperatureTrackBar)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chartControl
            // 
            this.chartControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
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
            this.chartControl.Size = new System.Drawing.Size(905, 631);
            this.chartControl.TabIndex = 0;
            // 
            // GoalTemperatureValue
            // 
            this.GoalTemperatureValue.AutoSize = true;
            this.GoalTemperatureValue.Location = new System.Drawing.Point(897, 651);
            this.GoalTemperatureValue.Name = "GoalTemperatureValue";
            this.GoalTemperatureValue.Size = new System.Drawing.Size(19, 13);
            this.GoalTemperatureValue.TabIndex = 6;
            this.GoalTemperatureValue.Text = "20";
            // 
            // GoalTemperatureLabel
            // 
            this.GoalTemperatureLabel.AutoSize = true;
            this.GoalTemperatureLabel.Location = new System.Drawing.Point(23, 652);
            this.GoalTemperatureLabel.Name = "GoalTemperatureLabel";
            this.GoalTemperatureLabel.Size = new System.Drawing.Size(88, 13);
            this.GoalTemperatureLabel.TabIndex = 5;
            this.GoalTemperatureLabel.Text = "Goal temperature";
            // 
            // GoalTemperatureTrackBar
            // 
            this.GoalTemperatureTrackBar.Location = new System.Drawing.Point(117, 649);
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
            this.buttonStopAcquisition.Location = new System.Drawing.Point(117, 677);
            this.buttonStopAcquisition.Name = "buttonStopAcquisition";
            this.buttonStopAcquisition.Size = new System.Drawing.Size(98, 35);
            this.buttonStopAcquisition.TabIndex = 7;
            this.buttonStopAcquisition.Text = "Stop acquisition";
            this.buttonStopAcquisition.UseVisualStyleBackColor = true;
            this.buttonStopAcquisition.Click += new System.EventHandler(this.ButtonStopAcquisitionClick);
            // 
            // buttonStartAcquisition
            // 
            this.buttonStartAcquisition.Location = new System.Drawing.Point(13, 677);
            this.buttonStartAcquisition.Name = "buttonStartAcquisition";
            this.buttonStartAcquisition.Size = new System.Drawing.Size(98, 35);
            this.buttonStartAcquisition.TabIndex = 8;
            this.buttonStartAcquisition.Text = "Start acquisition";
            this.buttonStartAcquisition.UseVisualStyleBackColor = true;
            this.buttonStartAcquisition.Click += new System.EventHandler(this.ButtonStartAcquisitionClick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelProgress,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 810);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(929, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelProgress
            // 
            this.toolStripStatusLabelProgress.Name = "toolStripStatusLabelProgress";
            this.toolStripStatusLabelProgress.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(19, 17);
            this.toolStripStatusLabel1.Text = "    ";
            // 
            // loggingView1
            // 
            this.loggingView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.loggingView1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.loggingView1.FollowLastItem = true;
            this.loggingView1.FormattingEnabled = true;
            this.loggingView1.Items.AddRange(new object[] {
            "Logging"});
            this.loggingView1.Location = new System.Drawing.Point(12, 722);
            this.loggingView1.MaxEntriesInListBox = 3000;
            this.loggingView1.Name = "loggingView1";
            this.loggingView1.Size = new System.Drawing.Size(905, 82);
            this.loggingView1.TabIndex = 11;
            this.loggingView1.SelectedIndexChanged += new System.EventHandler(this.loggingView1_SelectedIndexChanged);
            // 
            // ChartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(929, 832);
            this.Controls.Add(this.loggingView1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.buttonStartAcquisition);
            this.Controls.Add(this.buttonStopAcquisition);
            this.Controls.Add(this.GoalTemperatureValue);
            this.Controls.Add(this.GoalTemperatureLabel);
            this.Controls.Add(this.GoalTemperatureTrackBar);
            this.Controls.Add(this.chartControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChartForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Temperature Controller";
            this.Shown += new System.EventHandler(this.ChartFormShown);
            ((System.ComponentModel.ISupportInitialize)(this.GoalTemperatureTrackBar)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
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
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelProgress;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private Tools.LoggingView loggingView1;
    }
}

