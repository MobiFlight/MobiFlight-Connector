using System;
using System.Drawing;
using System.Windows.Forms;
using CommandMessenger;
using ZedGraph;

namespace DataLogging
{
    public partial class ChartForm : Form
    {
        // In a small C# application all code would typically end up in this class.
        // For a cleaner, MVP-like setup I moved higher logic to Datalogging.cs,        
        
        private readonly DataLogging _dataLogging;
        private long _previousChartUpdate;
        private IPointListEdit _analog1List;
        private IPointListEdit _analog2List;

        public ChartForm()
        {
            InitializeComponent();
            _dataLogging = new DataLogging();
            _dataLogging.Setup(this);
        }

        // ------------------  CHARTING ROUTINES ---------------------

        // Set up the chart
        public void SetupChart()
        {
            // get a reference to the GraphPane
            var myPane = chartControl.GraphPane;

            // Set the Titles
            myPane.Title.Text = "Data logging using CmdMessenger";
            myPane.XAxis.Title.Text = "Time (s)";
            myPane.YAxis.Title.Text = "Voltage (v)";

            // Create data arrays for rolling points
            _analog1List = new RollingPointPairList(3000);
            _analog2List = new RollingPointPairList(3000);

            // Create a smoothened red curve 
            LineItem myCurve = myPane.AddCurve("Analog 1", _analog1List, Color.Red, SymbolType.None);
            myCurve.Line.IsSmooth = true;
            myCurve.Line.SmoothTension = 0.2f;

            // Create a smoothened blue curve 
            LineItem myCurve2 = myPane.AddCurve("Analog 2", _analog2List, Color.Blue, SymbolType.None);

            myCurve2.Line.IsSmooth = true;
            myCurve2.Line.SmoothTension = 0.2f;
            // Tell ZedGraph to re-calculate the axes since the data have changed
            chartControl.AxisChange();
        }

        // Update the graph with the data points
        public void UpdateGraph(double time, double analog1, double analog2)
        {
            // set window width
            const double windowWidth = 30.0;

            // Add data points to the circular lists
            _analog1List.Add(time, analog1);
            _analog2List.Add(time, analog2);

            // Because updating the chart is computationally expensive if 
            // there are many data points, we do this only every 100 ms, that is 10 Hz
            if (!TimeUtils.HasExpired(ref _previousChartUpdate, 100)) return;

            ///Console.WriteLine("Update chart");

            // get and update x-scale to scroll with data with an certain window
            var xScale = chartControl.GraphPane.XAxis.Scale;
            xScale.Max = time + xScale.MajorStep;
            xScale.Min = xScale.Max - windowWidth;

            // Make sure the axes are rescaled to accommodate actual data
            chartControl.AxisChange();

            // Force a redraw
            chartControl.Invalidate();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dataLogging.Exit();
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
