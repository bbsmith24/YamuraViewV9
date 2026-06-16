using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YamuraViewControls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static YamuraViewControls.ChartView;

namespace YamuraViewControls
{
    public delegate void ChartMouseTrack(object sender, ChartControlMouseTrackEventArgs e);
    public delegate void ChartXAxisChange(object sender, ChartControlXAxisChangeEventArgs e);
    public delegate void AxisOffsetUpdate(object sender, AxisOffsetUpdateEventArgs e);
    public delegate void ClearGraphicsPath(object sender, EventArgs e);

    public partial class Chart : UserControl
    {
        public List<DisplayDataSet> dataSets = new List<DisplayDataSet>();

        public enum ChartType
        {
            Stripchart,     // generic X/Y plot, time or distance on X axis, value on Y axis
            XYChart,        // X/Y plot with 'target' drawn, any channel on X axis, any channel on Y axis
            Histogram       // histogram chart for selected channel
        }
        ChartType chartViewType;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartType ChartViewType
        {
            get { return chartViewType; }
            set { chartViewType = value; }
        }
        Dictionary<string, Axis> xAxes = new Dictionary<string, Axis>();
        Dictionary<string, Axis> yAxes = new Dictionary<string, Axis>();
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Dictionary<string, Axis> X_Axes
        {
            get { return xAxes; }
            set { xAxes = value; }
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Dictionary<string, Axis> Y_Axes
        {
            get { return yAxes; }
            set
            {
                yAxes = value;
                //if (chartAxes == null)
                //{
                //    return;
                //}
                //chartPropertiesForm.AxisChannelTree.Nodes.Clear();
                //chartPropertiesForm.CmbXAxis.Items.Clear();
                //chartPropertiesForm.CmbAlignAxis.Items.Clear();
                //chartPropertiesForm.TxtAutoThreshold.Text = "0.0";
                //foreach (KeyValuePair<String, Axis> curAxis in chartAxes)
                //{
                //    chartPropertiesForm.CmbXAxis.Items.Add(curAxis.Key);
                //    chartPropertiesForm.CmbAlignAxis.Items.Add(curAxis.Key);

                //    bool axisFound = false;
                //    foreach (TreeNode axisItem in chartPropertiesForm.AxisChannelTree.Nodes)
                //    {
                //        axisFound = false;
                //        if (axisItem.Name == curAxis.Key)
                //        {
                //            axisFound = true;
                //            break;
                //        }
                //    }
                //    if (!axisFound)
                //    {
                //        chartPropertiesForm.AxisChannelTree.Nodes.Add(curAxis.Key, curAxis.Key, 0);
                //        chartPropertiesForm.AxisChannelTree.Nodes[curAxis.Key].Checked = curAxis.Value.ShowAxis;
                //    }
                //    foreach (KeyValuePair<String, ChannelInfo> associatedChannel in curAxis.Value.AssociatedChannels)
                //    {
                //        if (chartPropertiesForm.AxisChannelTree.Nodes[curAxis.Key].Nodes.ContainsKey(associatedChannel.Key))
                //        {
                //            continue;
                //        }
                //        chartPropertiesForm.AxisChannelTree.Nodes[curAxis.Key].Nodes.Add(associatedChannel.Key, associatedChannel.Key, 1);
                //        chartPropertiesForm.AxisChannelTree.Nodes[curAxis.Key].Nodes[associatedChannel.Key].Checked = associatedChannel.Value.ShowChannel;

                //    }
                //}
            }
        }
        // true if X and Y scale are equal regardless of window size
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EqualScale { get; set; }

        bool allowDrag = true;
        /// <summary>
        /// chart allows drag area with left mouse button
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        /// <summary>
        /// chart allows drag area with left mouse button
        /// </summary>
        public bool AllowDrag
        {
            get
            {
                return allowDrag;
            }

            set
            {
                allowDrag = value;
            }
        }
        // crosshairs, box, circle work
        // horizontal and vertical look weird
        CursorStyle cursorMode = CursorStyle.CROSSHAIRS;
        /// <summary>
        /// type of mouse move cursor - CROSSHAIRS, HORIZONTAL, VERTICAL, BOX, CIRCLE
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CursorStyle CursorMode
        {
            get { return cursorMode; }
            set { cursorMode = value; }
        }
        int cursorBoxSize = 10;
        /// <summary>
        /// size of box and circle cursor
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int CursorBoxSize
        {
            get { return cursorBoxSize; }
            set { cursorBoxSize = value; }
        }
        bool cursorUpdateSource = true;
        /// <summary>
        /// chart accepts mouse moves and raises cursor update events for listeners
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CursorUpdateSource
        {
            get
            {
                return cursorUpdateSource;
            }
            set
            {
                cursorUpdateSource = value;
            }
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle ChartBounds = new Rectangle(0, 0, 0, 0);  
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string XChannelName { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ChartBorder { get; set; }
        String chartName = "Data";
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Chart Name displayed on chart tab in control"), Category("Data")]
        public string ChartName { 
            get 
            {
                chartName = ChartTabs.TabPages[0].Text;
                return chartName;  
            } 
            set { 
                chartName = value;
                ChartTabs.TabPages[0].Text = chartName;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public Chart()
        {
            InitializeComponent();
            chartView1.ChartOwner = this;
            chartProperties1.ChartOwner = this;

            CursorUpdateSource = true;
            xAxes.Add("X Axis", new Axis());
            yAxes.Add("Y Axis", new Axis());
            ChartBounds = new Rectangle(0, 0, 0, 0);
            XChannelName = "Time";
            ChartBorder = 10;
        }
        /// <summary>
        /// 
        /// </summary>
        public void UpdateData()
        {
            chartProperties1.AxisChannelTree.Nodes.Clear();
            chartProperties1.CmbXAxis.Items.Clear();
            //chartProperties1.CmbAlignAxis.Items.Clear();
            //chartProperties1.TxtAutoThreshold.Text = "0.0";
            //foreach (KeyValuePair<String, Axis> curAxis in chartAxes)
            String curAxisName = yAxes.ElementAt(0).Key;
            Axis curAxis = yAxes.ElementAt(0).Value;

            //chartProperties1.CmbXAxis.Items.Add(xAxes.ElementAt(0).Key);
            //chartProperties1.CmbAlignAxis.Items.Add(xAxes.ElementAt(0).Key);

            //foreach (TreeNode axisItem in chartProperties1.AxisChannelTree.Nodes)
            //{
            //    axisFound = false;
            //    if (axisItem.Name == curAxisName)
            //    {
            //        axisFound = true;
            //        break;
            //    }
            //}
            //if (!axisFound)
            //{
            //    chartProperties1.AxisChannelTree.Nodes.Add(curAxisName, curAxisName, 0);
            //    chartProperties1.AxisChannelTree.Nodes[curAxisName].Checked = curAxis.ShowAxis;
            //}
            foreach (ChartChannel associatedChannel in curAxis.AssociatedChannels)
            {
                if (!chartProperties1.AxisChannelTree.Nodes.ContainsKey(associatedChannel.ChannelName))
                {
                    chartProperties1.AxisChannelTree.Nodes.Add(associatedChannel.ChannelName, associatedChannel.ChannelName,1);
                }
                TreeNode newNode = chartProperties1.AxisChannelTree.Nodes[associatedChannel.ChannelName].Nodes.Add(associatedChannel.ChannelName, 
                                                                                                associatedChannel.ChannelName + " (" + associatedChannel.DataSetName + ")", 
                                                                                                1);
                newNode.Tag = associatedChannel;
                newNode.Checked = associatedChannel.ShowChannel;
                //if (chartProperties1.AxisChannelTree.Nodes[curAxisName].Nodes.ContainsKey(associatedChannel.ChannelName))
                //{
                //    continue;
                //}
                if (!chartProperties1.CmbXAxis.Items.Contains(associatedChannel.ChannelName))
                {
                    chartProperties1.CmbXAxis.Items.Add(associatedChannel.ChannelName);
                }
                //chartProperties1.AxisChannelTree.Nodes[curAxisName].Nodes.Add(associatedChannel.ChannelName, associatedChannel.ChannelName, 1);
                //chartProperties1.AxisChannelTree.Nodes[curAxisName].Nodes[associatedChannel.ChannelName].Checked = associatedChannel.ShowChannel;

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetChartBounds(int x, int y, int width, int height)
        {
            ChartBounds.X = x;
            ChartBounds.Y = y;
            ChartBounds.Width = width;
            ChartBounds.Height = height;

        }
        public int ChartWidth
        { get { return ChartBounds.Width; } }
        public int ChartHeight
        { get { return ChartBounds.Height; } }
        /// <summary>
        /// mirror mouse moves in stripchart in other views (trackmap, traction circle)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnChartMouseTrack(object sender, ChartControlMouseTrackEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(Name);
            System.Diagnostics.Debug.WriteLine("At time " + e.XAxisValues.ElementAt(0).Value.ToString());
            PointF scaledPoint = new PointF(0, 0);
            foreach (KeyValuePair<string, SortedList<string, float>> channels in e.YAxisValues)
            {
                foreach (KeyValuePair<string, float> values in channels.Value)
                {
                    if (Name == "TractionCircle")
                    {
                        if (values.Key == "gX")
                        {
                            scaledPoint.X = values.Value;
                        }
                        if (values.Key == "gY")
                        {
                            scaledPoint.Y = values.Value;
                        }
                    }
                    if (Name == "TrackMap")
                    {
                        if (values.Key == "Longitude")
                        {
                            scaledPoint.X = values.Value;
                        }
                        if (values.Key == "Latitude")
                        {
                            scaledPoint.Y = values.Value;
                        }
                    }
                    System.Diagnostics.Debug.WriteLine("Chart " + ChartName + " draw cursor at " + scaledPoint.ToString());
                    chartView1.DrawCursorAtScaledPoint(scaledPoint);
                }
            }
            //float timeAtCursor = 0.0F;
            //Dictionary<string, SortedList<string, float>> valuesAtCursor = new Dictionary<string, SortedList<string, float>>();
            //try
            //{
            //    // Ensure cursor storage exists for index 0
            //    if (chartView1.StartMouseMove.Count == 0)
            //    {
            //        chartView1.StartMouseMove.Add(false);
            //        chartView1.StartMouseDrag.Add(false);
            //        chartView1.ChartStartCursorPos.Add(new Point(0, 0));
            //        chartView1.ChartLastCursorPos.Add(new Point(0, 0));
            //    }

            //    // Mouse position relative to chartView1
            //    Point mousePt = chartView1.PointToClient(Cursor.Position);
            //    int mx = mousePt.X;
            //    int my = mousePt.Y;

            //    // Erase previous cursor if drawn
            //    if (chartView1.StartMouseMove[0])
            //    {
            //        chartView1.DrawCursorAt(chartView1.ChartLastCursorPos[0].X, chartView1.ChartLastCursorPos[0].Y);
            //        chartView1.StartMouseMove[0] = false;
            //    }

            //    // Draw horizontal + vertical lines at current mouse point using existing GDI helper
            //    CursorMode = CursorStyle.CROSSHAIRS;
            //    // Only draw if inside panel area
            //    if (mx >= 0 && my >= 0 && mx <= chartView1.Width && my <= chartView1.Height)
            //    {
            //        chartView1.DrawCursorAt(mx, my);
            //        chartView1.StartMouseMove[0] = true;
            //        chartView1.ChartLastCursorPos[0] = new Point(mx, my);
            //    }

            //    // Map mouse X -> data-space X using the primary X axis transform
            //    Axis primaryXAxis = X_Axes.ElementAtOrDefault(0).Value;
            //    if (primaryXAxis == null || primaryXAxis.AxisDisplayRange == null || primaryXAxis.AxisDisplayRange.Length < 3)
            //        return;

            //    float xRange = primaryXAxis.AxisDisplayRange[2];
            //    if (xRange == 0.0f || ChartBounds.Width <= 0)
            //        return;

            //    // displayScaleX matches the drawing transform used elsewhere
            //    float displayScaleX = (float)ChartBounds.Width / xRange;

            //    // panel X relative to chart area (subtract border)
            //    float panelX = mx - ChartBorder;

            //    // Compute data-space X (axis units)
            //    float dataX = (panelX / displayScaleX) + primaryXAxis.AxisDisplayRange[0] - primaryXAxis.AxisOffset;

            //    var outArgs = new ChartControlMouseMoveEventArgs();

            //    // For each dataset compute dataset-specific time corresponding to dataX
            //    for (int dsIdx = 0; dsIdx < dataSets.Count; dsIdx++)
            //    {
            //        var ds = dataSets[dsIdx];
            //        timeAtCursor = dataX;

            //        if (XChannelName == "Time")
            //        {
            //            timeAtCursor = dataX;
            //        }
            //        else if (XChannelName == "xTime")
            //        {
            //            if (ds.channels != null && ds.channels.TryGetValue("xTime", out ChartChannel xt) && xt.DataPoints != null && xt.DataPoints.Count > 0)
            //            {
            //                if (chartView1.TryFindNearestKeyByValue(xt.DataPoints, dataX, out float foundKey))
            //                    timeAtCursor = foundKey;
            //            }
            //        }
            //        else
            //        {
            //            if (ds.channels != null && ds.channels.TryGetValue(XChannelName, out ChartChannel axisChan) && axisChan.DataPoints != null && axisChan.DataPoints.Count > 0)
            //            {
            //                if (chartView1.TryFindNearestKeyByValue(axisChan.DataPoints, dataX, out float foundKey))
            //                    timeAtCursor = foundKey;
            //            }
            //        }

            //        outArgs.XAxisValues.Add(dsIdx.ToString() + "-Time", timeAtCursor);
            //    }

            //    // For each displayed channel, find Y value at the computed dataset time using TryFindNearestValue
            //    foreach (KeyValuePair<string, Axis> axisKvp in Y_Axes)
            //    {
            //        Axis yAxis = axisKvp.Value;
            //        if (yAxis == null)
            //        {
            //            continue;
            //        }
            //        foreach (ChartChannel chan in yAxis.AssociatedChannels)
            //        {
            //            if (!chan.ShowChannel) continue;

            //            string dsKey = chan.DataSetIndex.ToString() + "-Time";
            //            float sampleTime = outArgs.XAxisValues.TryGetValue(dsKey, out float tval) ? tval : dataX;

            //            if (chan.DataPoints != null && chan.DataPoints.Count > 0 && TryFindNearestValue(chan.DataPoints, sampleTime, out float foundY))
            //            {
            //                if (!valuesAtCursor.ContainsKey(chan.ChannelName))
            //                {
            //                    valuesAtCursor.Add(chan.ChannelName, new SortedList<string, float>);
            //                }
            //                valuesAtCursor[chan.ChannelName].Add(chan.DataSetName, foundY);
            //                if (!outArgs.YAxisValues.ContainsKey(chan.ChannelName))
            //                {
            //                    outArgs.YAxisValues.Add(chan.ChannelName, new SortedList<string, float>);
            //                }
            //                outArgs.YAxisValues[chan.ChannelName].Add(chan.DataSetName, foundY);
            //            }
            //            else
            //            {
            //                valuesAtCursor.Add(chan.ChannelName, float.NaN);
            //                outArgs.YAxisValues.Add(chan.DataSetIndex.ToString() + "-" + chan.ChannelName, float.NaN);
            //            }
            //        }
            //    }

            //    // Raise event for listeners (other charts) with mapped X and channel values
            //    //ChartMouseMoveEvent?.Invoke(this, outArgs);
            //    System.Diagnostics.Debug.WriteLine("Time : " + timeAtCursor.ToString());
            //    foreach (KeyValuePair<string, SortedList<string, float>> channelAtCursor in valuesAtCursor)
            //    {
            //        System.Diagnostics.Debug.WriteLine(channelAtCursor.Key);
            //        foreach (KeyValuePair<string, float> value in channelAtCursor.Value)
            //        {
            //            System.Diagnostics.Debug.WriteLine("\t" + value.Value);
            //        }
            //    }
            //    Graphics g = chartView1.CreateGraphics();
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debug.WriteLine("OnChartMouseMove error: " + ex.Message);
            //}
        }
    }
    public class ChartChannel
    {
        String channelName;
        String channelDescription;
        String channelSource;
        float channelScale;
        float[] xRange = new float[] { float.MaxValue, float.MinValue , 0.0F };
        float[] yRange = new float[] { float.MaxValue, float.MinValue , 0.0F };
        public SortedList<float, float> dataPoints = new SortedList<float, float>();
        public String ChannelName
        {
            get
            {
                return channelName;
            }

            set
            {
                channelName = value;
            }
        }
        string dataSetName;
        public string DataSetName
        {
            get
            {
                return dataSetName;
            }
            set
            {
                dataSetName = value;
            }
        }
        int dataSetIndex = 0;
        public int DataSetIndex
        {
            get { return dataSetIndex; }
            set { dataSetIndex = value; }
        }
        public String ChannelDescription
        {
            get
            {
                return channelDescription;
            }

            set
            {
                channelDescription = value;
            }
        }
        public String ChannelSource
        {
            get
            {
                return channelSource;
            }

            set
            {
                channelSource = value;
            }
        }
        public float ChannelScale
        {
            get
            {
                return channelScale;
            }

            set
            {
                channelScale = value;
            }
        }
        public float[] XRange
        {
            get { return xRange; }
            set { xRange = value; }
        }
        public float[] YRange
        {
            get { return yRange; }
            set { yRange = value; }
        }
        bool showChannel = false;
        public bool ShowChannel
        {
            get { return showChannel; }
            set { showChannel = value; }
        }
        Color channelColor = Color.Black;
        public Color ChannelColor
        {
            get { return channelColor; }
            set { channelColor = value; }
        }
        public ChartChannel(String name, String desc, String src, string dataSet, float scale)
        {
            channelName = name;
            dataSetName = dataSet;
            channelDescription = desc;
            channelSource = src;
            channelScale = scale;
            dataPoints = new SortedList<float, float>();
        }
        public SortedList<float, float> DataPoints
        {
            get
            {
                return dataPoints;
            }

            set
            {
                dataPoints = value;
            }
        }
        GraphicsPath channelPath = new GraphicsPath();
        public GraphicsPath ChannelPath
        {
            get { return channelPath; }
            set { channelPath = value; }
        }
        public void AddPoint(float valueX, float valueY)
        {
            if (DataPoints.ContainsKey(valueX))
            {
                DataPoints[valueX] = valueY;
            }
            else
            {
                DataPoints.Add(valueX, valueY);
            }
            xRange[0] = valueX < xRange[0] ? valueY : xRange[0];
            xRange[1] = valueX > xRange[1] ? valueY : xRange[1];
            xRange[2] = xRange[1] - xRange[0];
            yRange[0] = valueY < yRange[0] ? valueY : yRange[0];
            yRange[1] = valueY > yRange[1] ? valueY : yRange[1];
            yRange[2] = yRange[1] - yRange[0];
        }
        public bool FindPointAtTime(float timeStamp, ref float foundValue)
        {
            float priorTime = dataPoints.LastOrDefault(i => i.Key <= timeStamp).Key;
            float nextTime = dataPoints.FirstOrDefault(i => i.Key >= timeStamp).Key;
            // exact match
            if (priorTime == timeStamp)
            {
                foundValue = dataPoints[priorTime];
            }
            // check for window size?
            // prior time is nearest
            else if ((timeStamp - priorTime) < (nextTime - timeStamp))
            {
                foundValue = dataPoints[priorTime];
            }
            // next time is nearest
            else
            {
                foundValue = dataPoints[nextTime];
            }
            return true;
        }
    }
    public partial class DisplayDataSet
    {
        public Dictionary<String, ChartChannel> channels = new Dictionary<String, ChartChannel>();
        public string DataSetName { get; set; }
        public float TimeOffset { get; set; }
        public float DistanceOffset { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public DisplayDataSet(string name)
        {
            DataSetName = name;
            TimeOffset = 0.0f;
            DistanceOffset = 0.0f;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="description"></param>
        /// <param name="source"></param>
        /// <param name="scale"></param>
        /// <param name="channelValues"></param>
        public void AddChannel(String channelName, String description, String source, String dataSet, float scale, SortedList<float, float> channelValues)
        {
            channels.Add(channelName, new ChartChannel(channelName, description, source, dataSet, scale));
            foreach (KeyValuePair<float, float> dataPoint in channelValues)
            {
                channels[channelName].AddPoint(dataPoint.Key, dataPoint.Value);
            }
        }
    }
}
