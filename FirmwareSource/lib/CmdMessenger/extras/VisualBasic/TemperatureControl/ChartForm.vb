Imports System
Imports System.Drawing
Imports System.Globalization
Imports System.Windows.Forms
Imports CommandMessenger
Imports Microsoft.VisualBasic
Imports ZedGraph


Partial Public Class ChartForm
    Inherits Form
    ' In a small C# application all code would typically end up in this class.
    ' For a cleaner, MVP-like setup I moved higher logic to TemperatureControl.cs,        

    Private ReadOnly _temperatureControl As TemperatureControl
    Private _previousChartUpdate As Long
    Private _analog1List As IPointListEdit
    Private _analog3List As IPointListEdit
    Private _temperaturePane As GraphPane
    Private _heaterPane As GraphPane
    Private _heaterList As RollingPointPairList
    Private _heaterPwmList As RollingPointPairList
    Private _connected As Boolean
    Private _goalTemperature As Double

    Public Sub New()
        InitializeComponent()
        _temperatureControl = New TemperatureControl()
    End Sub

    Private Sub ChartFormShown(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Shown
        ' Run setup of view model
        _temperatureControl.Setup(Me)
    End Sub


    ' ------------------  CHARTING ROUTINES ---------------------

    ''' <summary> Sets up the chart. </summary>
    Public Sub SetupChart()
        Dim masterPane As MasterPane = chartControl.MasterPane
        masterPane.PaneList.Clear()

        ' get a reference to the GraphPane

        _temperaturePane = New GraphPane(New Rectangle(5, 5, 890, 350), "Temperature controller", "Time (s)", "Temperature (C)")
        masterPane.Add(_temperaturePane)

        ' Create data arrays for rolling points
        _analog1List = New RollingPointPairList(3000)
        _analog3List = New RollingPointPairList(3000)
        _analog1List.Clear()
        _analog3List.Clear()

        ' Create a smoothened red curve for the current temperature
        Dim myCurve1 As LineItem = _temperaturePane.AddCurve("Current temperature", _analog1List, Color.Red, SymbolType.None)
        myCurve1.Line.Width = 2
        myCurve1.Line.IsSmooth = True
        myCurve1.Line.SmoothTension = 0.2F


        ' Create a smoothened blue curve for the goal temperature
        Dim myCurve3 As LineItem = _temperaturePane.AddCurve("Goal temperature", _analog3List, Color.Blue, SymbolType.None)
        myCurve3.Line.Width = 2
        myCurve3.Line.IsSmooth = True
        myCurve3.Line.SmoothTension = 0.2F
        ' Tell ZedGraph to re-calculate the axes since the data have changed
        chartControl.AxisChange()

        _heaterPane = New GraphPane(New Rectangle(5, 360, 890, 250), Nothing, Nothing, Nothing)
        masterPane.Add(_heaterPane)

        _heaterList = New RollingPointPairList(3000)
        _heaterPwmList = New RollingPointPairList(3000)
        _heaterList.Clear()
        _heaterPwmList.Clear()

        ' Create a red curve for the heater value
        Dim heaterCurve As LineItem = _heaterPane.AddCurve(Nothing, _heaterList, Color.YellowGreen, SymbolType.None)
        heaterCurve.Line.Width = 2
        heaterCurve.Line.IsSmooth = False

        ' Create a red curve for the current heater pwm value
        Dim heaterPwmCurve As LineItem = _heaterPane.AddCurve(Nothing, _heaterPwmList, Color.Blue, SymbolType.None)
        heaterPwmCurve.Line.Width = 2
        heaterPwmCurve.Line.IsSmooth = False

        SetChartScale(0)
    End Sub

    ' Update the graph with the data points
    Public Sub UpdateGraph(ByVal time As Double, ByVal currTemp As Double, ByVal goalTemp As Double, ByVal heaterValue As Double, ByVal heaterPwmValue As Boolean)
        ' Add data points to the circular lists
        _analog1List.Add(time, currTemp)
        _analog3List.Add(time, goalTemp)

        _heaterList.Add(time, heaterValue)
        _heaterPwmList.Add(time, If(heaterPwmValue, 1.05, 0.05))

        ' Because updating the chart is computationally expensive if 
        ' there are many data points, we do this only every 10 ms, that is 100 Hz
        If (Not TimeUtils.HasExpired(_previousChartUpdate, 10)) Then
            Return
        End If

        'Console.WriteLine(@"Update chart");
        SetChartScale(time)
    End Sub

    ' Update the graph with the data points
    Public Sub SetConnected()
        _connected = True
        UpdateUi()
    End Sub

    ' Update the graph with the data points
    Public Sub SetDisConnected()
        _connected = False
        UpdateUi()
    End Sub

    ''' <summary> Updates the user interface. </summary>
    Private Sub UpdateUi()
        buttonStartAcquisition.Enabled = _connected
        buttonStopAcquisition.Enabled = _connected
        chartControl.Enabled = _connected
        GoalTemperatureTrackBar.Enabled = _connected
        GoalTemperatureValue.Enabled = _connected
    End Sub

    ''' <summary> Sets the chart scale. </summary>
    ''' <param name="time"> The time scale to show. </param>
    Private Sub SetChartScale(ByVal time As Double)
        ' set window width
        Const windowWidth As Double = 30.0
        ' get and update x-scale to scroll with data with an certain window

        Dim xScaleTemp = _temperaturePane.XAxis.Scale

        If time < windowWidth Then
            xScaleTemp.Max = windowWidth
            xScaleTemp.Min = 0
        Else
            xScaleTemp.Max = time + xScaleTemp.MajorStep
            xScaleTemp.Min = xScaleTemp.Max - windowWidth
        End If

        Dim xScaleHeater = _heaterPane.XAxis.Scale
        xScaleHeater.Max = xScaleTemp.Max
        xScaleHeater.Min = xScaleTemp.Min

        ' Make sure the axes are rescaled to accommodate actual data
        chartControl.AxisChange()

        ' Force a redraw
        chartControl.Invalidate()
    End Sub

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            _temperatureControl.Exit()
            If components IsNot Nothing Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    ''' <summary> Update goal temperature as triggered by scrollbar. </summary>
    ''' <param name="sender"> Source of the event. </param>
    ''' <param name="e">      Event information. </param>
    Public Sub GoalTemperatureTrackBarScroll(ByVal sender As Object, ByVal e As EventArgs) Handles GoalTemperatureTrackBar.Scroll
        _goalTemperature = (CDbl(GoalTemperatureTrackBar.Value) / 10.0)
        GoalTemperatureValue.Text = _goalTemperature.ToString(CultureInfo.InvariantCulture)
        _temperatureControl.GoalTemperature = _goalTemperature
    End Sub

    ''' <summary>  Stop Acquisition. </summary>
    ''' <param name="sender"> Source of the event. </param>
    ''' <param name="e">      Event information. </param>
    Private Sub ButtonStopAcquisitionClick(ByVal sender As Object, ByVal e As EventArgs) Handles buttonStopAcquisition.Click
        _temperatureControl.StopAcquisition()
    End Sub

    ''' <summary>  Start Acquisition. </summary>
    ''' <param name="sender"> Source of the event. </param>
    ''' <param name="e">      Event information. </param>
    Private Sub ButtonStartAcquisitionClick(ByVal sender As Object, ByVal e As EventArgs) Handles buttonStartAcquisition.Click
        _temperatureControl.StartAcquisition()
    End Sub

    ''' <summary> Update status bar. </summary>
    ''' <param name="description"> The message to show on the status bar. </param>
    Public Sub SetStatus(ByVal description As String)
        toolStripStatusLabel1.Text = description
    End Sub

    Private Sub listView1_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)

    End Sub

    Private Sub loggingView1_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles loggingView1.SelectedIndexChanged

    End Sub

    Public Sub LogMessage(ByVal message As String)
        loggingView1.AddEntry(message)
    End Sub

End Class