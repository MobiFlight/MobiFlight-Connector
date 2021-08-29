Imports System
Imports Microsoft.VisualBasic

Partial Public Class ChartForm
    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing


#Region "Windows Form Designer generated code"

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.chartControl = New ZedGraph.ZedGraphControl()
        Me.GoalTemperatureValue = New System.Windows.Forms.Label()
        Me.GoalTemperatureLabel = New System.Windows.Forms.Label()
        Me.GoalTemperatureTrackBar = New System.Windows.Forms.TrackBar()
        Me.buttonStopAcquisition = New System.Windows.Forms.Button()
        Me.buttonStartAcquisition = New System.Windows.Forms.Button()
        Me.statusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.toolStripStatusLabelProgress = New System.Windows.Forms.ToolStripStatusLabel()
        Me.toolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.loggingView1 = New Tools.LoggingView()
        CType(Me.GoalTemperatureTrackBar, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.statusStrip1.SuspendLayout()
        Me.SuspendLayout()
        ' 
        ' chartControl
        ' 
        Me.chartControl.Anchor = (CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles))
        Me.chartControl.IsAntiAlias = True
        Me.chartControl.Location = New System.Drawing.Point(12, 12)
        Me.chartControl.Name = "chartControl"
        Me.chartControl.ScrollGrace = 0.0R
        Me.chartControl.ScrollMaxX = 0.0R
        Me.chartControl.ScrollMaxY = 0.0R
        Me.chartControl.ScrollMaxY2 = 0.0R
        Me.chartControl.ScrollMinX = 0.0R
        Me.chartControl.ScrollMinY = 0.0R
        Me.chartControl.ScrollMinY2 = 0.0R
        Me.chartControl.Size = New System.Drawing.Size(905, 631)
        Me.chartControl.TabIndex = 0
        ' 
        ' GoalTemperatureValue
        ' 
        Me.GoalTemperatureValue.AutoSize = True
        Me.GoalTemperatureValue.Location = New System.Drawing.Point(897, 651)
        Me.GoalTemperatureValue.Name = "GoalTemperatureValue"
        Me.GoalTemperatureValue.Size = New System.Drawing.Size(19, 13)
        Me.GoalTemperatureValue.TabIndex = 6
        Me.GoalTemperatureValue.Text = "20"
        ' 
        ' GoalTemperatureLabel
        ' 
        Me.GoalTemperatureLabel.AutoSize = True
        Me.GoalTemperatureLabel.Location = New System.Drawing.Point(23, 652)
        Me.GoalTemperatureLabel.Name = "GoalTemperatureLabel"
        Me.GoalTemperatureLabel.Size = New System.Drawing.Size(88, 13)
        Me.GoalTemperatureLabel.TabIndex = 5
        Me.GoalTemperatureLabel.Text = "Goal temperature"
        ' 
        ' GoalTemperatureTrackBar
        ' 
        Me.GoalTemperatureTrackBar.Location = New System.Drawing.Point(117, 649)
        Me.GoalTemperatureTrackBar.Maximum = 1000
        Me.GoalTemperatureTrackBar.Name = "GoalTemperatureTrackBar"
        Me.GoalTemperatureTrackBar.Size = New System.Drawing.Size(779, 45)
        Me.GoalTemperatureTrackBar.TabIndex = 2
        Me.GoalTemperatureTrackBar.Tag = ""
        Me.GoalTemperatureTrackBar.TickFrequency = 10
        Me.GoalTemperatureTrackBar.Value = 200
        '			Me.GoalTemperatureTrackBar.Scroll += New System.EventHandler(Me.GoalTemperatureTrackBarScroll);
        ' 
        ' buttonStopAcquisition
        ' 
        Me.buttonStopAcquisition.Location = New System.Drawing.Point(117, 677)
        Me.buttonStopAcquisition.Name = "buttonStopAcquisition"
        Me.buttonStopAcquisition.Size = New System.Drawing.Size(98, 35)
        Me.buttonStopAcquisition.TabIndex = 7
        Me.buttonStopAcquisition.Text = "Stop acquisition"
        Me.buttonStopAcquisition.UseVisualStyleBackColor = True
        '			Me.buttonStopAcquisition.Click += New System.EventHandler(Me.ButtonStopAcquisitionClick);
        ' 
        ' buttonStartAcquisition
        ' 
        Me.buttonStartAcquisition.Location = New System.Drawing.Point(13, 677)
        Me.buttonStartAcquisition.Name = "buttonStartAcquisition"
        Me.buttonStartAcquisition.Size = New System.Drawing.Size(98, 35)
        Me.buttonStartAcquisition.TabIndex = 8
        Me.buttonStartAcquisition.Text = "Start acquisition"
        Me.buttonStartAcquisition.UseVisualStyleBackColor = True
        '			Me.buttonStartAcquisition.Click += New System.EventHandler(Me.ButtonStartAcquisitionClick);
        ' 
        ' statusStrip1
        ' 
        Me.statusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripStatusLabelProgress, Me.toolStripStatusLabel1})
        Me.statusStrip1.Location = New System.Drawing.Point(0, 810)
        Me.statusStrip1.Name = "statusStrip1"
        Me.statusStrip1.Size = New System.Drawing.Size(929, 22)
        Me.statusStrip1.SizingGrip = False
        Me.statusStrip1.TabIndex = 9
        Me.statusStrip1.Text = "statusStrip1"
        ' 
        ' toolStripStatusLabelProgress
        ' 
        Me.toolStripStatusLabelProgress.Name = "toolStripStatusLabelProgress"
        Me.toolStripStatusLabelProgress.Size = New System.Drawing.Size(0, 17)
        ' 
        ' toolStripStatusLabel1
        ' 
        Me.toolStripStatusLabel1.Name = "toolStripStatusLabel1"
        Me.toolStripStatusLabel1.Size = New System.Drawing.Size(19, 17)
        Me.toolStripStatusLabel1.Text = "    "
        ' 
        ' loggingView1
        ' 
        Me.loggingView1.Anchor = (CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) Or System.Windows.Forms.AnchorStyles.Left) Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles))
        Me.loggingView1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        Me.loggingView1.FollowLastItem = True
        Me.loggingView1.FormattingEnabled = True
        Me.loggingView1.Items.AddRange(New Object() {"Logging"})
        Me.loggingView1.Location = New System.Drawing.Point(12, 722)
        Me.loggingView1.MaxEntriesInListBox = 3000
        Me.loggingView1.Name = "loggingView1"
        Me.loggingView1.Size = New System.Drawing.Size(905, 82)
        Me.loggingView1.TabIndex = 11
        '			Me.loggingView1.SelectedIndexChanged += New System.EventHandler(Me.loggingView1_SelectedIndexChanged);
        ' 
        ' ChartForm
        ' 
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0F, 13.0F)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(929, 832)
        Me.Controls.Add(Me.loggingView1)
        Me.Controls.Add(Me.statusStrip1)
        Me.Controls.Add(Me.buttonStartAcquisition)
        Me.Controls.Add(Me.buttonStopAcquisition)
        Me.Controls.Add(Me.GoalTemperatureValue)
        Me.Controls.Add(Me.GoalTemperatureLabel)
        Me.Controls.Add(Me.GoalTemperatureTrackBar)
        Me.Controls.Add(Me.chartControl)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "ChartForm"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.Text = "Temperature Controller"
        '			Me.Shown += New System.EventHandler(Me.ChartFormShown);
        CType(Me.GoalTemperatureTrackBar, System.ComponentModel.ISupportInitialize).EndInit()
        Me.statusStrip1.ResumeLayout(False)
        Me.statusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Public chartControl As ZedGraph.ZedGraphControl
    Private GoalTemperatureValue As System.Windows.Forms.Label
    Private GoalTemperatureLabel As System.Windows.Forms.Label
    Private WithEvents GoalTemperatureTrackBar As System.Windows.Forms.TrackBar
    Private WithEvents buttonStopAcquisition As System.Windows.Forms.Button
    Private WithEvents buttonStartAcquisition As System.Windows.Forms.Button
    Private statusStrip1 As System.Windows.Forms.StatusStrip
    Private toolStripStatusLabelProgress As System.Windows.Forms.ToolStripStatusLabel
    Private toolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
    Private WithEvents loggingView1 As Tools.LoggingView
End Class