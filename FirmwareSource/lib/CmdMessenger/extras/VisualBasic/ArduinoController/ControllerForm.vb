Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Globalization
Imports System.Linq
Imports System.Text
Imports System.Windows.Forms
Imports Microsoft.VisualBasic


Partial Public Class ControllerForm
    Inherits Form
    Private ReadOnly _arduinoController As ArduinoController
    Private _ledFrequency As Double

    Public Sub New()
        InitializeComponent()
        _arduinoController = New ArduinoController()
        _arduinoController.Setup(Me)
    End Sub

    ' Update arduinoController on value checkbox checked/unchecked
    Private Sub EnableLedCheckBoxCheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles EnableLedCheckBox.CheckedChanged
        _arduinoController.SetLedState(EnableLedCheckBox.Checked)
    End Sub

    ' Update value label and arduinoController on value changed using slider
    Private Sub LedFrequencyTrackBarScroll(ByVal sender As Object, ByVal e As EventArgs) Handles LedFrequencyLabelTrackBar.Scroll
        _ledFrequency = 0.4 + (CDbl(LedFrequencyLabelTrackBar.Value)) / 25.0
        LedFrequencyValue.Text = _ledFrequency.ToString(CultureInfo.InvariantCulture)
        _arduinoController.SetLedFrequency(_ledFrequency)
    End Sub

    ' Set ledState checkbox
    Public Sub SetLedState(ByVal ledState As Boolean)
        EnableLedCheckBox.Checked = ledState
    End Sub

    ' Set frequency slider
    Public Sub SetFrequency(ByVal ledFrequency As Double)
        LedFrequencyLabelTrackBar.Value = CInt(Fix((ledFrequency - 0.4) * 2.5))
    End Sub

    ' Update value label and arduinoController on value changed
    Private Sub LedFrequencyLabelTrackBarValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles LedFrequencyLabelTrackBar.ValueChanged
        LedFrequencyTrackBarScroll(sender, e)
    End Sub

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            _arduinoController.Exit()
            If components IsNot Nothing Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub
End Class