Imports System
Imports Microsoft.VisualBasic

Partial Public Class ControllerForm
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
        Me.EnableLedCheckBox = New System.Windows.Forms.CheckBox()
        Me.LedFrequencyLabelTrackBar = New System.Windows.Forms.TrackBar()
        Me.LedFrequencyLabel = New System.Windows.Forms.Label()
        Me.LedFrequencyValue = New System.Windows.Forms.Label()
        CType(Me.LedFrequencyLabelTrackBar, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        ' 
        ' EnableLedCheckBox
        ' 
        Me.EnableLedCheckBox.AutoSize = True
        Me.EnableLedCheckBox.Checked = True
        Me.EnableLedCheckBox.CheckState = System.Windows.Forms.CheckState.Checked
        Me.EnableLedCheckBox.Location = New System.Drawing.Point(30, 12)
        Me.EnableLedCheckBox.Name = "EnableLedCheckBox"
        Me.EnableLedCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.EnableLedCheckBox.Size = New System.Drawing.Size(80, 17)
        Me.EnableLedCheckBox.TabIndex = 0
        Me.EnableLedCheckBox.Text = "Enable Led"
        Me.EnableLedCheckBox.UseVisualStyleBackColor = True
        '			Me.EnableLedCheckBox.CheckedChanged += New System.EventHandler(Me.EnableLedCheckBoxCheckedChanged);
        ' 
        ' LedFrequencyLabelTrackBar
        ' 
        Me.LedFrequencyLabelTrackBar.Location = New System.Drawing.Point(90, 35)
        Me.LedFrequencyLabelTrackBar.Maximum = 240
        Me.LedFrequencyLabelTrackBar.Name = "LedFrequencyLabelTrackBar"
        Me.LedFrequencyLabelTrackBar.Size = New System.Drawing.Size(208, 45)
        Me.LedFrequencyLabelTrackBar.TabIndex = 1
        Me.LedFrequencyLabelTrackBar.Tag = ""
        Me.LedFrequencyLabelTrackBar.TickFrequency = 10
        '			Me.LedFrequencyLabelTrackBar.Scroll += New System.EventHandler(Me.LedFrequencyTrackBarScroll);
        '			Me.LedFrequencyLabelTrackBar.ValueChanged += New System.EventHandler(Me.LedFrequencyLabelTrackBarValueChanged);
        ' 
        ' LedFrequencyLabel
        ' 
        Me.LedFrequencyLabel.AutoSize = True
        Me.LedFrequencyLabel.Location = New System.Drawing.Point(14, 36)
        Me.LedFrequencyLabel.Name = "LedFrequencyLabel"
        Me.LedFrequencyLabel.Size = New System.Drawing.Size(78, 13)
        Me.LedFrequencyLabel.TabIndex = 2
        Me.LedFrequencyLabel.Text = "Led Frequency"
        ' 
        ' LedFrequencyValue
        ' 
        Me.LedFrequencyValue.AutoSize = True
        Me.LedFrequencyValue.Location = New System.Drawing.Point(304, 38)
        Me.LedFrequencyValue.Name = "LedFrequencyValue"
        Me.LedFrequencyValue.Size = New System.Drawing.Size(13, 13)
        Me.LedFrequencyValue.TabIndex = 3
        Me.LedFrequencyValue.Text = "0"
        ' 
        ' ControllerForm
        ' 
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0F, 13.0F)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(344, 85)
        Me.Controls.Add(Me.LedFrequencyValue)
        Me.Controls.Add(Me.LedFrequencyLabel)
        Me.Controls.Add(Me.LedFrequencyLabelTrackBar)
        Me.Controls.Add(Me.EnableLedCheckBox)
        Me.Name = "ControllerForm"
        Me.Text = "Arduino Controller"
        CType(Me.LedFrequencyLabelTrackBar, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private WithEvents EnableLedCheckBox As System.Windows.Forms.CheckBox
    Private WithEvents LedFrequencyLabelTrackBar As System.Windows.Forms.TrackBar
    Private LedFrequencyLabel As System.Windows.Forms.Label
    Private LedFrequencyValue As System.Windows.Forms.Label
End Class