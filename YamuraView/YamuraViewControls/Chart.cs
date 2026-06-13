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

namespace YamuraViewControls
{
    public delegate void ChartMouseMove(object sender, ChartControlMouseMoveEventArgs e);
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
        /// <summary>
        /// 
        /// </summary>
        public Chart()
        {
            InitializeComponent();
            xAxes.Add("X Axis", new Axis());
            yAxes.Add("Y Axis", new Axis());
            chartView1.ChartOwner = this;
            chartProperties1.ChartOwner = this;
        }
        /// <summary>
        /// 
        /// </summary>
        public void UpdateData()
        {
            chartProperties1.AxisChannelTree.Nodes.Clear();
            chartProperties1.CmbXAxis.Items.Clear();
            chartProperties1.CmbAlignAxis.Items.Clear();
            chartProperties1.TxtAutoThreshold.Text = "0.0";
            //foreach (KeyValuePair<String, Axis> curAxis in chartAxes)
            String curAxisName = yAxes.ElementAt(0).Key;
            Axis curAxis = yAxes.ElementAt(0).Value;

            //chartProperties1.CmbXAxis.Items.Add(xAxes.ElementAt(0).Key);
            chartProperties1.CmbAlignAxis.Items.Add(xAxes.ElementAt(0).Key);

            bool axisFound = false;
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
