Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports CommandMessenger
Imports Microsoft.VisualBasic
Imports ZedGraph


Partial Public Class ChartForm
    Inherits Form
    ' In a small C# application all code would typically end up in this class.
    ' For a cleaner, MVP-like setup I moved higher logic to Datalogging.cs,        

    Private ReadOnly _dataLogging As DataLogging
    Private _previousChartUpdate As Long
    Private _analog1List As IPointListEdit
    Private _analog2List As IPointListEdit

    Public Sub New()
        InitializeComponent()
        _dataLogging = New DataLogging()
        _dataLogging.Setup(Me)
    End Sub

    ' ------------------  CHARTING ROUTINES ---------------------

    ' Set up the chart
    Public Sub SetupChart()
        ' get a reference to the GraphPane
        Dim myPane = chartControl.GraphPane

        ' Set the Titles
        myPane.Title.Text = "Data logging using CmdMessenger"
        myPane.XAxis.Title.Text = "Time (s)"
        myPane.YAxis.Title.Text = "Voltage (v)"

        ' Create data arrays for rolling points
        _analog1List = New RollingPointPairList(3000)
        _analog2List = New RollingPointPairList(3000)

        ' Create a smoothened red curve 
        Dim myCurve As LineItem = myPane.AddCurve("Analog 1", _analog1List, Color.Red, SymbolType.None)
        myCurve.Line.IsSmooth = True
        myCurve.Line.SmoothTension = 0.2F

        ' Create a smoothened blue curve 
        Dim myCurve2 As LineItem = myPane.AddCurve("Analog 2", _analog2List, Color.Blue, SymbolType.None)

        myCurve2.Line.IsSmooth = True
        myCurve2.Line.SmoothTension = 0.2F
        ' Tell ZedGraph to re-calculate the axes since the data have changed
        chartControl.AxisChange()
    End Sub

    ' Update the graph with the data points
    Public Sub UpdateGraph(ByVal time As Double, ByVal analog1 As Double, ByVal analog2 As Double)
        ' set window width
        Const windowWidth As Double = 30.0

        ' Add data points to the circular lists
        _analog1List.Add(time, analog1)
        _analog2List.Add(time, analog2)

        ' Because updating the chart is computationally expensive if 
        ' there are many data points, we do this only every 100 ms, that is 10 Hz
        If (Not TimeUtils.HasExpired(_previousChartUpdate, 100)) Then
            Return
        End If

        '''Console.WriteLine("Update chart");

        ' get and update x-scale to scroll with data with an certain window
        Dim xScale = chartControl.GraphPane.XAxis.Scale
        xScale.Max = time + xScale.MajorStep
        xScale.Min = xScale.Max - windowWidth

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
            _dataLogging.Exit()
            If components IsNot Nothing Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub
End Class