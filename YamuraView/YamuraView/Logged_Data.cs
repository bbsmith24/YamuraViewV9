using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YamuraView
{
    /// <summary>
    /// run header - one per run, contains global data for run
    /// </summary>
    public class DataLogger
    {
        public float[] minMaxTimestamp = new float[] { float.MaxValue, float.MinValue };
        public float[]   minMaxLong = new float[] { float.MaxValue, float.MinValue };
        public float[]   minMaxLat = new float[] { float.MaxValue, float.MinValue };
        public float[] minMaxSpeed = new float[] { float.MaxValue, float.MinValue };

        public float[][] minMaxAccel = new float[][] {new float[] {float.MaxValue, float.MinValue},
                                                      new float[] {float.MaxValue, float.MinValue},
                                                      new float[] {float.MaxValue, float.MinValue}};

        public List<RunData> runData = new List<RunData>();
        public Dictionary<String, float[]> channelRanges = new Dictionary<String, float[]>();
        public void UpdateChannelRange(String channelName, float curVal)
        {
            channelRanges[channelName][0] = curVal < channelRanges[channelName][0] ? curVal : channelRanges[channelName][0];
            channelRanges[channelName][1] = curVal > channelRanges[channelName][1] ? curVal : channelRanges[channelName][1];
        }
        public void Reset()
        {
            runData.Clear();
            channelRanges.Clear();
            minMaxTimestamp = new float[] { float.MaxValue, float.MinValue };
            minMaxLong = new float[] { float.MaxValue, float.MinValue };
            minMaxLat = new float[] { float.MaxValue, float.MinValue };
            minMaxSpeed = new float[] { float.MaxValue, float.MinValue };

            minMaxAccel = new float[][] {new float[] {float.MaxValue, float.MinValue},
                                                          new float[] {float.MaxValue, float.MinValue},
                                                          new float[] {float.MaxValue, float.MinValue}};
    }
}
    /// <summary>
    /// 
    /// </summary>
    public class RunData
    {
        public Dictionary<String, float[]> channelRanges = new Dictionary<String, float[]>();
        public Dictionary<String, DataChannel> channels = new Dictionary<String, DataChannel>();
        public String dateStr = "";
        public String timeStr = "";
        public String fileName = "";
        public String runName = "";
        public float[] minMaxTimestamp = new float[] { float.MaxValue, float.MinValue };
        public float DistanceOffset { get; set; } = 0.0F;
        public float TimeOffset { get; set; } = 0.0F;
        public RunData(String run)
        {
            runName = run;
        }
        public void AddChannel(String name, String desc, String src, String run, float scl)
        {
            if(channels.ContainsKey(name))
            {
                return;
            }
            channelRanges.Add(name, new float[2] { float.MaxValue, float.MinValue });
            channels.Add(name, new DataChannel(name, desc, src, run, scl));
        }
        public void UpdateChannelRange(String channelName, float time,  float curVal)
        {
            channelRanges[channelName][0] = curVal < channelRanges[channelName][0] ? curVal : channelRanges[channelName][0];
            channelRanges[channelName][1] = curVal > channelRanges[channelName][1] ? curVal : channelRanges[channelName][1];
        }
        public void AddChannelData(String channelName, float time, float value)
        {
            UpdateChannelRange(channelName, time, value);
            channels[channelName].DataPoints.Add(time, value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class DataChannel
    {
        String channelName;
        String channelDescription;
        String channelSource;
        float channelScale;
        private object runName;
        float[] xRange = new float[] { float.MaxValue, float.MinValue, 0.0F };
        float[] yRange = new float[] { float.MaxValue, float.MinValue, 0.0F };
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
        public DataChannel(String name, String desc, String src, String run, float scale)
        {
            channelName = name;
            channelDescription = desc;
            channelSource = src;
            channelScale = scale;
            runName = run;
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
        public void AddPoint(float timeStamp, float value)
        {
            DataPoints[timeStamp] = value;
            xRange[0] = timeStamp < xRange[0] ? timeStamp : xRange[0];
            xRange[1] = timeStamp > xRange[1] ? timeStamp : xRange[1];
            xRange[2] = xRange[1] - xRange[0];
            yRange[0] = value < yRange[0] ? value : yRange[0];
            yRange[1] = value > yRange[1] ? value : yRange[1];
            yRange[2] = yRange[1] - yRange[0];
        }
    }
}
