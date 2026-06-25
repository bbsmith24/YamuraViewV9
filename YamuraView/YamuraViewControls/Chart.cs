using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using YamuraViewControls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static YamuraViewControls.ChartView;

namespace YamuraViewControls
{
    public delegate void ChartMouseTrack(object sender, ChartControlMouseTrackEventArgs e);
    public delegate void ChartXAxisChange(object sender, ChartControlXAxisChangeEventArgs e);
    public delegate void AxisOffsetUpdate(object sender, AxisOffsetUpdateEventArgs e);
    public delegate void ClearGraphicsPath(object sender, EventArgs e);
    /// <summary>
    /// 
    /// </summary>
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
            set { yAxes = value; }
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowOverlay { get; set; } = true;
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
            set 
            {
                chartName = value;
                ChartTabs.TabPages[0].Text = chartName;
            }
        }

        // graph index / invert state loaded from XML, applied in UpdateData when ChartChannels are tagged to nodes
        Dictionary<string, int> pendingGraphIndices = new Dictionary<string, int>();
        HashSet<string> pendingInvertChannels = new HashSet<string>();

        List<Color> autoColors = new List<Color> { Color.Red,
                                                   Color.Green,
                                                   Color.Blue,
                                                   Color.Yellow,
                                                   Color.Cyan,
                                                   Color.Magenta,
                                                   Color.Gray };
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<Color> AutoColors
        {
            get { return autoColors; }
            set { autoColors = value; }
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
            String curAxisName = yAxes.ElementAt(0).Key;
            Axis curAxis = yAxes.ElementAt(0).Value;

            foreach (ChartChannel associatedChannel in curAxis.AssociatedChannels)
            {
                if (((ChartName == "Strip Chart") &&
                     ((associatedChannel.ChannelName != "xDistance") && (associatedChannel.ChannelName != "Distance") && (associatedChannel.ChannelName != "Time"))) ||
                    ((ChartName == "Track Map") &&
                     ((associatedChannel.ChannelName == "Longitude") || (associatedChannel.ChannelName == "Latitude"))) ||
                    ((ChartName == "Traction Circle") &&
                     ((associatedChannel.ChannelName == "gX") || (associatedChannel.ChannelName == "gY") || (associatedChannel.ChannelName == "gZ"))))
                {
                    if (!chartProperties1.AxisChannelTree.Nodes.ContainsKey(associatedChannel.ChannelName))
                    {
                        chartProperties1.AxisChannelTree.Nodes.Add(associatedChannel.ChannelName, associatedChannel.ChannelDisplayName, 1);
                    }
                    String proposedName = associatedChannel.ChannelName + " (" + associatedChannel.DataSetName + ")";
                    if (!chartProperties1.AxisChannelTree.Nodes[associatedChannel.ChannelName].Nodes.ContainsKey(proposedName))
                    {
                        TreeNode newNode = chartProperties1.AxisChannelTree.Nodes[associatedChannel.ChannelName].Nodes.Add(proposedName,
                                                                                                    proposedName,
                                                                                                    1);
                        if (chartProperties1.AxisChannelTree.Nodes[associatedChannel.ChannelName].Checked)
                        {
                            associatedChannel.ShowChannel = true;
                        }
                        newNode.Tag = associatedChannel;
                        newNode.Checked = associatedChannel.ShowChannel;
                        if (pendingGraphIndices.TryGetValue(associatedChannel.ChannelName, out int pendingG))
                            associatedChannel.GraphIndex = pendingG;
                        if (pendingInvertChannels.Contains(associatedChannel.ChannelName))
                        {
                            associatedChannel.InvertChannel = true;
                            associatedChannel.ChannelDisplayName = associatedChannel.ChannelName + " (inv)";
                            chartProperties1.AxisChannelTree.Nodes[associatedChannel.ChannelName].Text = associatedChannel.ChannelDisplayName;
                        }
                    }
                }
            }
            if (chartProperties1.CmbXAxis.Items.Count == 0)
            {
                if (ChartName == "Strip Chart")
                {
                    chartProperties1.CmbXAxis.Items.Add("Time");
                    chartProperties1.CmbXAxis.Items.Add("Distance");
                    chartProperties1.CmbXAxis.Text = "Distance";
                }
                else if (ChartName == "Track Map")
                {
                    chartProperties1.CmbXAxis.Items.Add("Longitude");
                    chartProperties1.CmbXAxis.Items.Add("Latitude");
                    chartProperties1.CmbXAxis.Text = "Longitude";
                }
                else if (ChartName == "Traction Circle")
                {
                    chartProperties1.CmbXAxis.Items.Add("gX");
                    chartProperties1.CmbXAxis.Items.Add("gY");
                    chartProperties1.CmbXAxis.Items.Add("gZ");
                    chartProperties1.CmbXAxis.Text = "gX";
                }
            }
            // sync scrollbar range/position whenever data is updated
            if (X_Axes.Count > 0)
                chartView1.InitScrollbar();
            Invalidate(true);
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
        /// <summary>
        /// 
        /// </summary>
        public int ChartWidth
        { get { return ChartBounds.Width; } }
        /// <summary>
        /// 
        /// </summary>
        public int ChartHeight
        { get { return ChartBounds.Height; } }
        /// <summary>
        /// mirror mouse moves in stripchart in other views (trackmap, traction circle)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnChartMouseTrack(object sender, ChartControlMouseTrackEventArgs e)
        {
            PointF scaledPoint = new PointF(0, 0);
            int runIdx = 0;
            foreach (KeyValuePair<string, SortedList<string, float>> channels in e.YAxisValues)
            {
                for(int dataSetIdx = 0; dataSetIdx < dataSets.Count; dataSetIdx++)
                {
                    if(channels.Key == dataSets[dataSetIdx].DataSetName)
                    {
                        runIdx = dataSetIdx;
                        break;
                    }
                }
                
                foreach (KeyValuePair<string, float> channel in channels.Value)
                {
                    if (Name == "TractionCircle")
                    {
                        if (channel.Key == "gX")
                        {
                            scaledPoint.X = channel.Value;
                        }
                        if (channel.Key == "gY")
                        {
                            scaledPoint.Y = channel.Value;
                        }
                    }
                    if (Name == "TrackMap")
                    {
                        if (channel.Key == "Longitude")
                        {
                            scaledPoint.X = channel.Value;
                        }
                        if (channel.Key == "Latitude")
                        {
                            scaledPoint.Y = channel.Value;
                        }
                    }
                }
                Color runColor = runIdx < autoColors.Count ? autoColors[runIdx % autoColors.Count] : Color.White;
                chartView1.DrawCursorAtScaledPoint(scaledPoint, runIdx, runColor);
                runIdx++;
            }
        }
        /// <summary>
        /// generate XML file for settings to save
        /// </summary>
        /// <returns></returns>
        public void SaveSetup(XDocument xmlDoc)
        {
            String localName = ChartName.Replace(' ', '_');
            xmlDoc.Element("Setup")?.Add(new XElement(localName));
            xmlDoc.Element("Setup")?.Element(localName)?.Add(new XElement("X_Axis", 
                       new XAttribute("Name", chartProperties1.CmbXAxis.Text), 
                       new XAttribute("ID", chartProperties1.CmbXAxis.SelectedIndex)));
            
            for(int itemIdx = 0; itemIdx < chartProperties1.CmbXAxis.Items.Count; itemIdx++)
            {
                xmlDoc.Element("Setup")?.Element(localName)?.Element("X_Axis").Add(new XElement("Item" + itemIdx.ToString(), chartProperties1.CmbXAxis.Items[itemIdx].ToString()));
            }

            xmlDoc.Element("Setup")?.Element(localName)?.Add(new XElement("DisplayMode",
                       new XAttribute("Name", chartProperties1.cmbChartMode.Text),
                       new XAttribute("ID", chartProperties1.cmbChartMode.SelectedIndex),
                       new XAttribute("ShowOverlay", chartProperties1.ShowOverlay.ToString())));
            xmlDoc.Element("Setup")?.Element(localName)?.Add(new XElement("Channels"));
            foreach (TreeNode channelNode in chartProperties1.axisChannelTree.Nodes)
            {
                int gIdx = 0;
                bool inverted = false;
                if (channelNode.Nodes.Count > 0 && channelNode.Nodes[0].Tag is ChartChannel savedChan)
                {
                    gIdx = savedChan.GraphIndex;
                    inverted = savedChan.InvertChannel;
                }
                XElement channelElement = new XElement(channelNode.Text,
                    new XAttribute("Show", channelNode.Checked.ToString()),
                    new XAttribute("Graph", gIdx.ToString()),
                    new XAttribute("Invert", inverted.ToString()));
                xmlDoc.Element("Setup")?.Element(localName)?.Element("Channels")?.Add(channelElement);
            }
        }
        /// <summary>
        /// apply settings from XML file
        /// </summary>
        /// <param name="xmlDoc"></param>
        public void ApplySetup(XDocument xmlDoc)
        {
            String localName = ChartName.Replace(' ', '_');
            chartProperties1.CmbXAxis.Items.Clear();
            foreach (XElement axisItem in xmlDoc.Element("Setup")?.Element(localName)?.Element("X_Axis").Elements())
            {
                if(!chartProperties1.CmbXAxis.Items.Contains(axisItem.Value))
                {
                    chartProperties1.CmbXAxis.Items.Add(axisItem.Value);
                }
            }
            chartProperties1.CmbXAxis.Text = xmlDoc.Element("Setup")?.Element(localName)?.Element("X_Axis")?.Attribute("Name")?.Value;
            chartProperties1.cmbChartMode.Text = xmlDoc.Element("Setup")?.Element(localName)?.Element("DisplayMode")?.Attribute("Name")?.Value;
            bool showOverlay = (bool?)xmlDoc.Element("Setup")?.Element(localName)?.Element("DisplayMode")?.Attribute("ShowOverlay") ?? true;
            chartProperties1.ShowOverlay = showOverlay;
            ShowOverlay = showOverlay;
            pendingGraphIndices.Clear();
            pendingInvertChannels.Clear();
            if (xmlDoc.Element("Setup")?.Element(localName)?.Element("Channels")?.Elements() != null)
            {
                foreach (XElement channelElement in xmlDoc?.Element("Setup")?.Element(localName)?.Element("Channels")?.Elements())
                {
                    String nodeName = channelElement.Name.ToString();
                    chartProperties1.axisChannelTree.Nodes.Add(nodeName, nodeName);
                    if((bool?)channelElement?.Attribute("Show") == true)
                    {
                        chartProperties1.axisChannelTree.Nodes[nodeName].Checked = true;
                    }
                    if (int.TryParse(channelElement.Attribute("Graph")?.Value, out int gIdx))
                        pendingGraphIndices[nodeName] = gIdx;
                    if (bool.TryParse(channelElement.Attribute("Invert")?.Value, out bool inv) && inv)
                        pendingInvertChannels.Add(nodeName);
                }
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
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
        int graphIndex = 0;
        public int GraphIndex
        {
            get { return graphIndex; }
            set { graphIndex = value; }
        }
        bool invertChannel = false;
        public bool InvertChannel
        {
            get { return invertChannel; }
            set { invertChannel = value; }
        }
        string channelDisplayName = "";
        public string ChannelDisplayName
        {
            get { return channelDisplayName.Length > 0 ? channelDisplayName : channelName; }
            set { channelDisplayName = value; }
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
            channelDisplayName = name;
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
            xRange[0] = valueX < xRange[0] ? valueX : xRange[0];
            xRange[1] = valueX > xRange[1] ? valueX : xRange[1];
            xRange[2] = xRange[1] - xRange[0];
            yRange[0] = valueY < yRange[0] ? valueY : yRange[0];
            yRange[1] = valueY > yRange[1] ? valueY : yRange[1];
            yRange[2] = yRange[1] - yRange[0];
        }
        //public bool FindPointAtTime(float timeStamp, ref float foundValue)
        //{
        //    float priorTime = dataPoints.LastOrDefault(i => i.Key <= timeStamp).Key;
        //    float nextTime = dataPoints.FirstOrDefault(i => i.Key >= timeStamp).Key;
        //    // exact match
        //    if (priorTime == timeStamp)
        //    {
        //        foundValue = dataPoints[priorTime];
        //    }
        //    // check for window size?
        //    // prior time is nearest
        //    else if ((timeStamp - priorTime) < (nextTime - timeStamp))
        //    {
        //        foundValue = dataPoints[priorTime];
        //    }
        //    // next time is nearest
        //    else
        //    {
        //        foundValue = dataPoints[nextTime];
        //    }
        //    return true;
        //}
    }
    /// <summary>
    /// 
    /// </summary>
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
