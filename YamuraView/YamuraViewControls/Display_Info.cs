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
}
