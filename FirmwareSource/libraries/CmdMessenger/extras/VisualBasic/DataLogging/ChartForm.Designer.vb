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
        Me.SuspendLayout()
        ' 
        ' chartControl
        ' 
        Me.chartControl.Anchor = (CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) Or System.Windows.Forms.AnchorStyles.Left) Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles))
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
        Me.chartControl.Size = New System.Drawing.Size(521, 442)
        Me.chartControl.TabIndex = 0
        ' 
        ' ChartForm
        ' 
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0F, 13.0F)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(545, 466)
        Me.Controls.Add(Me.chartControl)
        Me.Name = "ChartForm"
        Me.Text = "Data Logging and Charting"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Public chartControl As ZedGraph.ZedGraphControl
End Class