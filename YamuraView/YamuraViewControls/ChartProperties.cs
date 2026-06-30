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
            set
            {
                chartOwner = value;
                if (chartOwner != null)
                    chkShowOverlay.Checked = chartOwner.ShowOverlay;
            }
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowOverlay
        {
            get { return chkShowOverlay.Checked; }
            set { chkShowOverlay.Checked = value; }
        }
        private void chkShowOverlay_CheckedChanged(object sender, EventArgs e)
        {
            if (ChartOwner != null)
                ChartOwner.ShowOverlay = chkShowOverlay.Checked;
        }
        /// <summary>
        /// 
        /// </summary>
        public ChartProperties()
        {
            InitializeComponent();

            ChannelColorSelector embeddedColorDialog = new ChannelColorSelector();

            //embeddedColorDialog.ColorSelectEvent += EmbeddedColorDialog_ColorSelectEvent;

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
        //private void EmbeddedColorDialog_ColorSelectEvent(object sender, ColorSelectEventArgs e)
        //{
        //    //String axisName = axisChannelTree.SelectedNode.Parent.Text;
        //    //String channelName = axisChannelTree.SelectedNode.Text;
        //    //ChartOwner.Y_Axes[axisName].AssociatedChannels[channelName].ChannelColor = e.SelectedColor;
        //}
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
                channelsContext.Items["traceColorMenuItem"].Enabled = false;
                channelsContext.Items["traceColorMenuItem"].Visible = false;
                channelsContext.Items["assignGraphMenuItem"].Visible = true;
                PopulateAssignGraphMenu();

                // show invert with state from first child channel
                bool curInvert = axisChannelTree.SelectedNode.Nodes.Count > 0
                    && axisChannelTree.SelectedNode.Nodes[0].Tag is ChartChannel ch && ch.InvertChannel;
                invertToolStripMenuItem.Text = curInvert ? "Invert (on)" : "Invert";
            }
            else
            {
                channelsContext.Items["traceColorMenuItem"].Visible = true;
                channelsContext.Items["traceColorMenuItem"].Enabled = true;

                channelsContext.Items["assignGraphMenuItem"].Visible = true;
                PopulateAssignGraphMenu();

                bool curInvert = axisChannelTree.SelectedNode.Tag is ChartChannel selChan && selChan.InvertChannel;
                invertToolStripMenuItem.Text = curInvert ? "Invert (on)" : "Invert";
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
            // "Distance" channel keys are timestamps; use "xDistance" (keyed by distance) for axis range
            string xRangeKey = cmbXAxis.Text == "Distance" ? "xDistance" : cmbXAxis.Text;
            foreach (DisplayDataSet dataSet in ChartOwner.dataSets)
            {
                if (!dataSet.channels.TryGetValue(xRangeKey, out ChartChannel xRangeChan)) continue;
                ChartOwner.X_Axes["X Axis"].AxisDisplayRange[0] = xRangeChan.XRange[0];
                ChartOwner.X_Axes["X Axis"].AxisDisplayRange[1] = xRangeChan.XRange[1];
                ChartOwner.X_Axes["X Axis"].AxisDisplayRange[2] = xRangeChan.XRange[2];
                ChartOwner.X_Axes["X Axis"].AxisValueRange[0] = xRangeChan.XRange[0];
                ChartOwner.X_Axes["X Axis"].AxisValueRange[1] = xRangeChan.XRange[1];
                ChartOwner.X_Axes["X Axis"].AxisValueRange[2] = xRangeChan.XRange[2];
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
            if (axisChannelTree.SelectedNode == null || ChartOwner == null) return;

            // use .Name (permanent key) — never .Text which may include display suffixes
            TreeNode parentNode = axisChannelTree.SelectedNode.Parent ?? axisChannelTree.SelectedNode;
            string channelName = parentNode.Name;

            // toggle from current state of first matching instance
            bool newInvert = true;
            if (parentNode.Nodes.Count > 0 && parentNode.Nodes[0].Tag is ChartChannel firstChan)
                newInvert = !firstChan.InvertChannel;

            // update InvertChannel and ChannelDisplayName on every dataset instance
            foreach (var axis in ChartOwner.Y_Axes.Values)
                foreach (var chan in axis.AssociatedChannels)
                    if (chan.ChannelName == channelName)
                    {
                        chan.InvertChannel = newInvert;
                        chan.ChannelDisplayName = newInvert ? channelName + " (inv)" : channelName;
                    }

            // reflect in tree node text from the field — no string stripping needed
            parentNode.Text = newInvert ? channelName + " (inv)" : channelName;

            // inversion is applied in the transform, no path rebuild needed
            ChartOwner.Invalidate(true);
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
        /// populate "Assign to Graph" submenu with current graph numbers + option to create a new graph
        /// </summary>
        private void PopulateAssignGraphMenu()
        {
            assignGraphMenuItem.DropDownItems.Clear();
            int maxGraph = 0;
            if (ChartOwner != null)
                foreach (var axis in ChartOwner.Y_Axes.Values)
                    foreach (var chan in axis.AssociatedChannels)
                        if (chan.GraphIndex > maxGraph)
                            maxGraph = chan.GraphIndex;

            for (int g = 0; g <= maxGraph; g++)
            {
                int capturedG = g;
                var item = new ToolStripMenuItem($"Graph {g + 1}");
                item.Click += (s, e) => AssignChannelToGraph(capturedG);
                assignGraphMenuItem.DropDownItems.Add(item);
            }
            assignGraphMenuItem.DropDownItems.Add(new ToolStripSeparator());
            var newItem = new ToolStripMenuItem("New Graph");
            newItem.Click += (s, e) => AssignChannelToGraph(maxGraph + 1);
            assignGraphMenuItem.DropDownItems.Add(newItem);
        }
        /// <summary>
        /// assign all ChartChannel instances for the selected channel name to graphIndex, then clear paths to redraw
        /// </summary>
        private void AssignChannelToGraph(int graphIndex)
        {
            string channelName = "";
            if (axisChannelTree.SelectedNode?.Parent == null)// return;
            {
                channelName = axisChannelTree.SelectedNode.Name;
                if (ChartOwner != null)
                    foreach (var axis in ChartOwner.Y_Axes.Values)
                        foreach (var chan in axis.AssociatedChannels)
                            if (chan.ChannelName == channelName)
                                chan.GraphIndex = graphIndex;
            }
            else
            {
                (axisChannelTree.SelectedNode.Tag as ChartChannel).GraphIndex = graphIndex;
            }
            ClearGraphicsPathEvent?.Invoke(this, EventArgs.Empty);
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnResetDistanceALign_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbChartDefaultPenWidth_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var axis in ChartOwner.Y_Axes.Values)
            {
                foreach (var chan in axis.AssociatedChannels)
                {
                    chan.ChannelPenWidth = (float)cmbChartDefaultPenWidth.SelectedIndex;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void penWidth0MenuItem_Click(object sender, EventArgs e)
        {
            SetPenWidth(0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void penWidth1MenuItem_Click(object sender, EventArgs e)
        {
            SetPenWidth(1);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void penWidth2MenuItem_Click(object sender, EventArgs e)
        {
            SetPenWidth(2);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void penWidth3MenuItem_Click(object sender, EventArgs e)
        {
            SetPenWidth(3);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="penWidth"></param>
        public void SetPenWidth(float penWidth)
        {
            string channelName = "";
            if (axisChannelTree.SelectedNode?.Parent == null)// return;
            {
                channelName = axisChannelTree.SelectedNode.Name;
                if (ChartOwner != null)
                    foreach (var axis in ChartOwner.Y_Axes.Values)
                        foreach (var chan in axis.AssociatedChannels)
                            if (chan.ChannelName == channelName)
                                chan.ChannelPenWidth = penWidth;
            }
            else
            {
                channelName = axisChannelTree.SelectedNode.Parent.Name;
                (axisChannelTree.SelectedNode.Tag as ChartChannel).ChannelPenWidth = penWidth;
            }
        }
    }
}
