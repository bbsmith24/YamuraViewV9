using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YamuraViewControls
{
    public class Axis
    {
        string axisName = "unnamed";
        public string AxisName
        {
            get { return axisName; }
            set { axisName = value; }
        }

        float[] axisValueRange = new float[] { float.MaxValue, float.MinValue, 0.0F };
        public float[] AxisValueRange
        {
            get { return axisValueRange; }
            set { axisValueRange = value; }
        }

        float[] axisDisplayRange = new float[] { float.MaxValue, float.MinValue, 0.0F };
        public float[] AxisDisplayRange
        {
            get { return axisDisplayRange; }
            set { axisDisplayRange = value; }
        }

        float axisOffset = 0.0F;
        public float AxisOffset
        {
            get { return axisOffset; }
            set { axisOffset = value; }
        }
        List<ChartChannel> associatedChannels = new List<ChartChannel>();
        public List<ChartChannel> AssociatedChannels
        {
            get { return associatedChannels; }
            set { associatedChannels = value; }
        }

        bool showAxis = false;
        public bool ShowAxis
        {
            get { return showAxis; }
            set { showAxis = value; }
        }
    }
    //public class ChannelInfo
    //{
    //    int runIndex = 0;
    //    public int RunIndex
    //    {
    //        get { return runIndex; }
    //        set { runIndex = value; }
    //    }

    //    String channelName = "unnamed";
    //    public String ChannelName
    //    {
    //        get { return channelName; }
    //        set { channelName = value; }
    //    }
    //    String dataSetName = "unnamed";
    //    public String DataSetName
    //    {
    //        get { return dataSetName; }
    //        set { dataSetName = value; }
    //    }

    //    bool showChannel = false;
    //    public bool ShowChannel
    //    {
    //        get { return showChannel; }
    //        set { showChannel = value; }
    //    }

    //    Color channelColor = Color.Black;
    //    public Color ChannelColor
    //    {
    //        get { return channelColor; }
    //        set { channelColor = value; }
    //    }

    //    float[] axisRange = new float[] { 0.0F, 0.0F, 0.0F };
    //    public float[] AxisRange
    //    {
    //        get { return axisRange; }
    //        set { axisRange = value; }
    //    }

    //    float[] axisOffset = new float[] { 0.0F, 0.0F };
    //    /// <summary>
    //    /// axis offset value
    //    /// </summary>
    //    public float[] AxisOffset
    //    {
    //        get { return axisOffset; }
    //        set { axisOffset = value; }
    //    }

    //    GraphicsPath channelPath = new GraphicsPath();
    //    public GraphicsPath ChannelPath
    //    {
    //        get { return channelPath; }
    //        set { channelPath = value; }
    //    }
    //    public ChannelInfo(int runIdx, String chanName, String dataName)
    //    {
    //        runIndex = runIdx;
    //        channelName = chanName;
    //        dataSetName = dataName;
    //    }
    //}
}
