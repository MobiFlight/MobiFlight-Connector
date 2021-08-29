using System;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using CommandMessenger;
using ZedGraph;

namespace DataLogging
{
    public partial class ChartForm : Form
    {
        // In a small C# application all code would typically end up in this class.
        // For a cleaner, MVP-like setup I moved higher logic to Datalogging.cs,        
        
        private readonly TemperatureControl _temperatureControl;
        //private long _previousChartUpdate;
        private IPointListEdit _analog1List;
        private IPointListEdit _analog3List;
        private GraphPane _temperaturePane;
        private GraphPane _heaterPane;
        private RollingPointPairList _heaterList;
        private RollingPointPairList _heaterPwmList;

        private double _goalTemperature;
        
        

        public ChartForm()
        {
            InitializeComponent();
            _temperatureControl = new TemperatureControl();   
        }

        private void ChartFormShown(object sender, EventArgs e)
        {
            // Run setup of view model
            _temperatureControl.Setup(this);
        }


        // ------------------  CHARTING ROUTINES ---------------------

        // Set up the chart
        public void SetupChart()
        {
            MasterPane masterPane = chartControl.MasterPane;
            masterPane.PaneList.Clear();

            // get a reference to the GraphPane

            _temperaturePane = new GraphPane(new Rectangle(5, 5, 890, 350),
                "Temperature controller",
                "Time (s)",
                "Temperature (C)");
            masterPane.Add(_temperaturePane);

            //var temperaturePane = chartControl.GraphPane;

            // Set the Titles
            //temperaturePane.Title.Text = "Temperature controller";
            //temperaturePane.XAxis.Title.Text = "Time (s)";
            //temperaturePane.YAxis.Title.Text = "Temperature (C)";

            // Create data arrays for rolling points
            _analog1List = new RollingPointPairList(3000);
            _analog3List = new RollingPointPairList(3000);
            _analog1List.Clear();
            _analog3List.Clear();

            // Create a smoothened red curve for the current temperature
            LineItem myCurve1 = _temperaturePane.AddCurve("Current temperature", _analog1List, Color.Red, SymbolType.None);
            myCurve1.Line.Width = 2;
            myCurve1.Line.IsSmooth = true;
            myCurve1.Line.SmoothTension = 0.2f;


            // Create a smoothened blue curve for the goal temperature
            LineItem myCurve3 = _temperaturePane.AddCurve("Goal temperature", _analog3List, Color.Blue, SymbolType.None);
            myCurve3.Line.Width = 2;
            myCurve3.Line.IsSmooth = true;
            myCurve3.Line.SmoothTension = 0.2f;
            // Tell ZedGraph to re-calculate the axes since the data have changed
            chartControl.AxisChange();

            _heaterPane = new GraphPane(new Rectangle(5, 360, 890, 250),
                null,
                null,
                null);
            masterPane.Add(_heaterPane);
            
            _heaterList = new RollingPointPairList(3000);
            _heaterPwmList = new RollingPointPairList(3000);
            _heaterList.Clear();
            _heaterPwmList.Clear();

            // Create a red curve for the heater value
            LineItem heaterCurve = _heaterPane.AddCurve(null, _heaterList, Color.YellowGreen, SymbolType.None);
            heaterCurve.Line.Width = 2;
            heaterCurve.Line.IsSmooth = false;

            // Create a red curve for the current heater pwm value
            LineItem heaterPwmCurve = _heaterPane.AddCurve(null, _heaterPwmList, Color.Blue, SymbolType.None);
            heaterPwmCurve.Line.Width = 2;
            heaterPwmCurve.Line.IsSmooth = false;

            SetChartScale(0);
        }

        // Update the graph with the data points
        public void UpdateGraph(double time, double currTemp,  double goalTemp, double heaterValue, bool heaterPwmValue)
        {
            // Add data points to the circular lists
            _analog1List.Add(time, currTemp);
            _analog3List.Add(time, goalTemp);

            _heaterList.Add(time, heaterValue);
            _heaterPwmList.Add(time, heaterPwmValue?1.05:0.05);

            // Because updating the chart is computationally expensive if 
            // there are many data points, we do this only every 100 ms, that is 10 Hz
            //if (!TimeUtils.HasExpired(ref _previousChartUpdate, 100)) return;
            
            Console.WriteLine(@"Update chart");
            SetChartScale(time);
        }
        
        private void SetChartScale(double time)
        {
            // set window width
            const double windowWidth = 30.0;
            // get and update x-scale to scroll with data with an certain window


            var xScaleTemp = _temperaturePane.XAxis.Scale;
   

            if (time < windowWidth)
            {
                xScaleTemp.Max = windowWidth;
                xScaleTemp.Min = 0;
                
            }
            else
            {
                xScaleTemp.Max = time + xScaleTemp.MajorStep;
                xScaleTemp.Min = xScaleTemp.Max - windowWidth;
            }

            var xScaleHeater = _heaterPane.XAxis.Scale;
            xScaleHeater.Max = xScaleTemp.Max;
            xScaleHeater.Min = xScaleTemp.Min;

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
                _temperatureControl.Exit();
                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        public void GoalTemperatureTrackBarScroll(object sender, EventArgs e)
        {
            _goalTemperature = ((double)GoalTemperatureTrackBar.Value/10.0);
            GoalTemperatureValue.Text = _goalTemperature.ToString(CultureInfo.InvariantCulture);
            _temperatureControl.GoalTemperature = _goalTemperature;
        }

        private void buttonStopAcquisition_Click(object sender, EventArgs e)
        {
            _temperatureControl.StopAcquisition();
        }

        private void buttonStartAcquisition_Click(object sender, EventArgs e)
        {
            _temperatureControl.StartAcquisition();
        }
    }
}
