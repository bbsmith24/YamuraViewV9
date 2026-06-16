using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YamuraViewControls
{
    public partial class ChartProperties : UserControl
    {
        public event AxisOffsetUpdate AxisOffsetUpdateEvent;
        public event ClearGraphicsPath ClearGraphicsPathEvent;

        //public DataLogger Logger
        //{
        //    get
        //    {
        //        return YamuraView.YamuraViewMain.dataLogger;
        //    }
        //}

        public event ChartXAxisChange ChartXAxisChangeEvent;

        Chart chartOwner;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Chart ChartOwner
        {
            get { return chartOwner; }
            set { chartOwner = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TriStateTreeView AxisChannelTree
        {
            get { return axisChannelTree; }
            set { axisChannelTree = value; }
        }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ComboBox CmbXAxis
        {
            get { return cmbXAxis; }
            set { cmbXAxis = value; }
        }
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //public ComboBox CmbAlignAxis
        //{
        //    get { return cmbAlignAxis; }
        //    set { cmbAlignAxis = value; }
        //}
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //public TextBox TxtAutoThreshold
        //{
        //    get { return txtAutoThreshold; }
        //    set { txtAutoThreshold = value; }
        //}
        //Dictionary<string, Axis> chartAxes;// = new Dictionary<string, Axis>();
        //public Dictionary<string, Axis> ChartAxes
        //{
        //    get { return chartAxes; }
        //    set
        //    {
        //        chartAxes = value;
        //        if (chartAxes == null)
        //        {
        //            return;
        //        }
        //        axisChannelTree.Nodes.Clear();
        //        cmbXAxis.Items.Clear();
        //        cmbAlignAxis.Items.Clear();
        //        txtAutoThreshold.Text = "0.0";
        //        foreach (KeyValuePair<String, Axis> curAxis in chartAxes)
        //        {
        //            cmbXAxis.Items.Add(curAxis.Key);
        //            cmbAlignAxis.Items.Add(curAxis.Key);

        //            bool axisFound = false;
        //            foreach (TreeNode axisItem in axisChannelTree.Nodes)
        //            {
        //                axisFound = false;
        //                if (axisItem.Name == curAxis.Key)
        //                {
        //                    axisFound = true;
        //                    break;
        //                }
        //            }
        //            if (!axisFound)
        //            {
        //                axisChannelTree.Nodes.Add(curAxis.Key, curAxis.Key, 0);
        //                axisChannelTree.Nodes[curAxis.Key].Checked = curAxis.Value.ShowAxis;
        //            }
        //            foreach (KeyValuePair<String, ChannelInfo> associatedChannel in curAxis.Value.AssociatedChannels)
        //            {
        //                if (axisChannelTree.Nodes[curAxis.Key].Nodes.ContainsKey(associatedChannel.Key))
        //                {
        //                    continue;
        //                }
        //                axisChannelTree.Nodes[curAxis.Key].Nodes.Add(associatedChannel.Key, associatedChannel.Key, 1);
        //                axisChannelTree.Nodes[curAxis.Key].Nodes[associatedChannel.Key].Checked = associatedChannel.Value.ShowChannel;

        //            }
        //        }
        //    }
        //}
        /// <summary>
        /// 
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string XAxisName
        {
            get { return cmbXAxis.Text; }
            set
            {
                cmbXAxis.Text = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public ChartProperties()
        {
            InitializeComponent();

            ChannelColorSelector embeddedColorDialog = new ChannelColorSelector();

            embeddedColorDialog.ColorSelectEvent += EmbeddedColorDialog_ColorSelectEvent;

            //ToolStripControlHost colorDialogHost = new ToolStripControlHost(embeddedColorDialog as Control);
            //colorDialogHost.Text = "Channel Color";
            //colorDialogHost.Name = "channelColor";
            //channelsContext.Items.Add(colorDialogHost);
            //channelsContext.Items["channelColor"].Visible = true;
            //axisOffsetsGrid.Columns["axisOffset"].Visible = false;
            //axisOffsetsGrid.Columns["axisChannel"].AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            //axisOffsetsGrid.CellEndEdit += AxisOffsetsGrid_CellEndEdit;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EmbeddedColorDialog_ColorSelectEvent(object sender, ColorSelectEventArgs e)
        {
            //String axisName = axisChannelTree.SelectedNode.Parent.Text;
            //String channelName = axisChannelTree.SelectedNode.Text;
            //ChartOwner.Y_Axes[axisName].AssociatedChannels[channelName].ChannelColor = e.SelectedColor;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axisChannelTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
            {
                return;
            }
            // this channel in all data sets is shown/hidden, so update all channel in all data sets
            if (e.Node.Parent == null)
            {
                foreach (TreeNode channelNode in e.Node.Nodes)
                {
                    if ((channelNode.Tag != null) && (channelNode.Tag is ChartChannel chartChannel))
                    {
                        chartChannel.ShowChannel = e.Node.Checked;
                    }
                }
            }
            // only selected channel is shown/hidden, so update only selected channel
            else
            {
                if (e.Node.Tag is ChartChannel chartChannel)
                {
                    chartChannel.ShowChannel = e.Node.Checked;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void channelsContext_Opening(object sender, CancelEventArgs e)
        {
            if (axisChannelTree.SelectedNode == null)
            {
                e.Cancel = true;
                return;
            }
            if (axisChannelTree.SelectedNode.Parent == null)
            {
                String axisName = axisChannelTree.SelectedNode.Text;

                channelsContext.Items["traceColorMenuItem"].Enabled = false;
                channelsContext.Items["traceColorMenuItem"].Visible = false;

                channelsContext.Items["lblAxisMin"].Visible = true;
                channelsContext.Items["lblAxisMax"].Visible = true;

                channelsContext.Items["txtAxisMinValue"].Visible = true;
                channelsContext.Items["txtAxisMaxValue"].Visible = true;

                //channelsContext.Items["txtAxisMinValue"].Text = ChartOwner.Y_Axes[axisName].AxisValueRange[0].ToString();
                //channelsContext.Items["txtAxisMaxValue"].Text = ChartOwner.Y_Axes[axisName].AxisValueRange[1].ToString();

            }
            else
            {
                channelsContext.Items["lblAxisMin"].Visible = false;
                channelsContext.Items["lblAxisMax"].Visible = false;
                channelsContext.Items["txtAxisMinValue"].Visible = false;
                channelsContext.Items["txtAxisMaxValue"].Visible = false;

                channelsContext.Items["traceColorMenuItem"].Visible = true;
                channelsContext.Items["traceColorMenuItem"].Enabled = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbXAxis_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChartControlXAxisChangeEventArgs changeEventArgs = new ChartControlXAxisChangeEventArgs();
            changeEventArgs.XAxisName = cmbXAxis.Text;
            ChartOwner.X_Axes["X Axis"].AssociatedChannels.Clear();
            foreach (DisplayDataSet dataSet in ChartOwner.dataSets)
            {
                ChartOwner.X_Axes["X Axis"].AxisDisplayRange[0] = dataSet.channels[cmbXAxis.Text].XRange[0];
                ChartOwner.X_Axes["X Axis"].AxisDisplayRange[1] = dataSet.channels[cmbXAxis.Text].XRange[1];
                ChartOwner.X_Axes["X Axis"].AxisDisplayRange[2] = dataSet.channels[cmbXAxis.Text].XRange[2];
                ChartOwner.X_Axes["X Axis"].AxisValueRange[0] = dataSet.channels[cmbXAxis.Text].XRange[0];
                ChartOwner.X_Axes["X Axis"].AxisValueRange[1] = dataSet.channels[cmbXAxis.Text].XRange[1];
                ChartOwner.X_Axes["X Axis"].AxisValueRange[2] = dataSet.channels[cmbXAxis.Text].XRange[2];
                ChartOwner.X_Axes["X Axis"].AssociatedChannels.Add(dataSet.channels[cmbXAxis.Text]);
            }

            ChartXAxisChangeEvent(this, changeEventArgs);

            //if (cmbXAxis.Text == "Time")
            //{
            //    axisOffsetsGrid.Columns["axisOffset"].Visible = true;
            //    axisOffsetsGrid.Columns["axisOffset"].AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            //    cmbAlignAxis.Enabled = true;
            //    txtAutoThreshold.Enabled = true;
            //    btnDoAutoAlign.Enabled = true;
            //}
            //else
            //{
            //    // all 'true' where 'false'
            //    axisOffsetsGrid.Columns["axisOffset"].Visible = true;
            //    axisOffsetsGrid.Columns["axisChannel"].AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            //    cmbAlignAxis.Enabled = true;
            //    txtAutoThreshold.Enabled = true;
            //    btnDoAutoAlign.Enabled = true;
            //}

            for (int yAxisIdx = 0; yAxisIdx < ChartOwner.Y_Axes.Count; yAxisIdx++)
            {
                if (ChartOwner.Y_Axes.ElementAt(yAxisIdx).Key == cmbXAxis.Text)
                {
                    continue;
                }
                foreach (ChartChannel channel in ChartOwner.Y_Axes.ElementAt(yAxisIdx).Value.AssociatedChannels)
                {
                    channel.ChannelPath.Reset();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void channelsContext_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            String nodeName = axisChannelTree.SelectedNode.Text;

            //if (axisChannelTree.SelectedNode.Parent == null)
            //{
            //ChartOwner.Y_Axes[nodeName].AxisValueRange[0] = Convert.ToSingle(channelsContext.Items["txtAxisMinValue"].Text);
            //ChartOwner.Y_Axes[nodeName].AxisValueRange[1] = Convert.ToSingle(channelsContext.Items["txtAxisMaxValue"].Text);
            //ChartOwner.Y_Axes[nodeName].AxisValueRange[2] = ChartOwner.Y_Axes[nodeName].AxisValueRange[1] - ChartOwner.Y_Axes[nodeName].AxisValueRange[0];
            //ChartOwner.Y_Axes[nodeName].AxisDisplayRange = ChartOwner.Y_Axes[nodeName].AxisValueRange;
            //foreach (ChartChannel channelInfo in ChartOwner.Y_Axes[nodeName].AssociatedChannels)
            //{
            //    channelInfo.AxisRange[0] = Convert.ToSingle(channelsContext.Items["txtAxisMinValue"].Text);
            //    channelInfo.AxisRange[1] = Convert.ToSingle(channelsContext.Items["txtAxisMaxValue"].Text);
            //    channelInfo.AxisRange[2] = ChartOwner.Y_Axes[nodeName].AxisValueRange[1] - ChartOwner.Y_Axes[nodeName].AxisValueRange[0];
            //}
            //}
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            channelsContext.Close();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void invertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //// selected channel
            //if (axisChannelTree.SelectedNode.Parent != null)
            //{
            //    string channelName = axisChannelTree.SelectedNode.Text;
            //    int runIdx = Convert.ToInt32(channelName.Substring(0, channelName.IndexOf('-')));
            //    channelName = channelName.Substring(channelName.IndexOf('-') + 1);
            //    foreach (KeyValuePair<float, DataPoint>kvp in ChartOwner.channels[channelName].DataPoints)
            //    {
            //        ChartOwner.channels[channelName].DataPoints[kvp.Key].PointValue = ChartOwner.channels[channelName].DataRange[1] - kvp.Value.PointValue + ChartOwner.channels[channelName].DataRange[0];
            //    }
            //    ClearGraphicsPathEvent(this, new EventArgs());
            //}
            //// all channels
            //else
            //{
            //    string channelName = axisChannelTree.SelectedNode.Text;
            //    for (int runIdx = 0; runIdx < ChartOwner.channels.Count; runIdx++)
            //    {
            //        if(!ChartOwner.channels.ContainsKey(channelName))
            //        {
            //            continue;
            //        }
            //        foreach (KeyValuePair<float, DataPoint> kvp in ChartOwner.channels[channelName].DataPoints)
            //        {
            //            ChartOwner.channels[channelName].DataPoints[kvp.Key].PointValue = ChartOwner.channels[channelName].DataRange[0];
            //        }
            //    }
            //    ClearGraphicsPathEvent(this, new EventArgs());
            //}

        }
        #region Auto align
        //private void btnDoAutoAlign_Click(object sender, EventArgs e)
        //{
        //    AutoAlign(Convert.ToSingle(txtAutoThreshold.Text), cmbAlignAxis.Text);
        //}
        /// <summary>
        /// estimate launch point offset from speed data
        /// find first speed > 30, walk back to first speed = 0
        /// </summary>
        private void AutoAlign(float launchThreshold, string alignAxis)
        {
            //List<float> launchPoints = new List<float>();
            //int runCount = 0;
            //foreach (RunData curRun in Logger.runData)
            //{
            //    foreach (KeyValuePair<float, DataPoint> curPoint in curRun.channels[alignAxis].DataPoints)
            //    {
            //        if (Math.Abs(curPoint.Value.PointValue) > launchThreshold)
            //        {
            //            launchPoints.Add(curPoint.Key);
            //            break;
            //        }
            //    }
            //    runCount++;
            //}
            //float minPoint = launchPoints.Min();
            //for (int launchIdx = 0; launchIdx < launchPoints.Count; launchIdx++)
            //{
            //    launchPoints[launchIdx] -= minPoint;
            //    if(axisOffsetsGrid.Rows.Count <= launchIdx)
            //    {
            //        axisOffsetsGrid.Rows.Add();
            //        axisOffsetsGrid.Rows[launchIdx].Cells["axisChannel"].Value = launchIdx.ToString() + "-" + Logger.runData[launchIdx].channels[XAxisName].ChannelName;
            //        axisOffsetsGrid.Rows[launchIdx].Cells["axisStart"].Value = Logger.runData[launchIdx].channels[XAxisName].DataRange[0];
            //        axisOffsetsGrid.Rows[launchIdx].Cells["axisEnd"].Value = Logger.runData[launchIdx].channels[XAxisName].DataRange[1];
            //    }
            //    axisOffsetsGrid.Rows[launchIdx].Cells["axisOffset"].Value = (-1 * launchPoints[launchIdx]).ToString();
            //    AxisOffsetUpdateEventArgs updateArgs = new AxisOffsetUpdateEventArgs("Time", launchIdx, 0, -1* launchPoints[launchIdx]);
            //    AxisOffsetUpdateEvent(this, updateArgs);
            //}
        }
        #endregion

        private void cmbChartMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChartOwner.chartView1.ChartMode = cmbChartMode.Text == "Absolute" ? ChartView.ChartViewMode.ABSOLUTE : ChartView.ChartViewMode.NORMALIZED;
            ChartOwner.chartView1.Invalidate();
        }
        /// <summary>
        /// set trace color for selected channel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void traceColorMenuItem_Click(object sender, EventArgs e)
        {
            if ((axisChannelTree.SelectedNode == null) ||
               (axisChannelTree.SelectedNode.Tag == null) ||
               (colorDialog1.ShowDialog() != DialogResult.OK))
            {
                return;
            }
            ((ChartChannel)axisChannelTree.SelectedNode.Tag).ChannelColor = colorDialog1.Color;
        }

        private void btnResetDistanceALign_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
    //public class AxisOffsetUpdateEventArgs : EventArgs
    //{
    //    string channelName;
    //    public string ChannelName
    //    {
    //        get { return channelName; }
    //        set { channelName = value; }
    //    }
    //    int axisIdx;
    //    public int AxisIdx
    //    {
    //        get { return axisIdx; }
    //        set { axisIdx = value; }
    //    }
    //    int runIdx;
    //    public int RunIdx
    //    {
    //        get { return runIdx; }
    //        set { runIdx = value; }
    //    }
    //    float offsetVal;
    //    public float OffsetVal
    //    {
    //        get { return offsetVal; }
    //        set { offsetVal = value; }
    //    }

    //    public AxisOffsetUpdateEventArgs(string name, int run, int axis, float offset)
    //    {
    //        ChannelName = name;
    //        RunIdx = run;
    //        AxisIdx = axis;
    //        OffsetVal = offset;
    //    }
    //}
}
