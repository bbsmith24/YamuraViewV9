using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GDI;
using Win32Interop.Methods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace YamuraViewControls
{
    public partial class ChartView : UserControl
    {
        public delegate void ChartMouseMove(object sender, ChartControlMouseMoveEventArgs e);
        public delegate void ChartXAxisChange(object sender, ChartControlXAxisChangeEventArgs e);
        public delegate void AxisOffsetUpdate(object sender, AxisOffsetUpdateEventArgs e);
        public delegate void ClearGraphicsPath(object sender, EventArgs e);

        public event ChartMouseMove ChartMouseMoveEvent;

        public enum DrawMode
        {
            R2_BLACK = 1,  // Pixel is always black.
            R2_NOTMERGEPEN = 2,  // Pixel is the inverse of the R2_MERGEPEN color (final pixel = NOT(pen OR screen pixel)).
            R2_MASKNOTPEN = 3,  // Pixel is a combination of the colors common to both the screen and the inverse of the pen (final pixel = (NOT pen) AND screen pixel).
            R2_NOTCOPYPEN = 4,  // Pixel is the inverse of the pen color.
            R2_MASKPENNOT = 5,  // Pixel is a combination of the colors common to both the pen and the inverse of the screen (final pixel = (NOT screen pixel) AND pen).
            R2_NOT = 6,  // Pixel is the inverse of the screen color.
            R2_XORPEN = 7,  // Pixel is a combination of the colors that are in the pen or in the screen, but not in both (final pixel = pen XOR screen pixel).
            R2_NOTMASKPEN = 8,  // Pixel is the inverse of the R2_MASKPEN color (final pixel = NOT(pen AND screen pixel)).
            R2_MASKPEN = 9,  // Pixel is a combination of the colors common to both the pen and the screen (final pixel = pen AND screen pixel).
            R2_NOTXORPEN = 10,  // Pixel is the inverse of the R2_XORPEN color (final pixel = NOT(pen XOR screen pixel)).
            R2_NOP = 11,  // Pixel remains unchanged.
            R2_MERGENOTPEN = 12,  // Pixel is a combination of the screen color and the inverse of the pen color (final pixel = (NOT pen) OR screen pixel).
            R2_COPYPEN = 13,  // Pixel is the pen color.
            R2_MERGEPENNOT = 14,  // Pixel is a combination of the pen color and the inverse of the screen color (final pixel = (NOT screen pixel) OR pen).
            R2_MERGEPEN = 15,  // Pixel is a combination of the pen color and the screen color (final pixel = pen OR screen pixel).
            R2_WHITE = 16,  // Pixel is always white.
            R2_LAST = 16
        }
        public enum CursorStyle
        {
            NONE,
            CROSSHAIRS,
            VERTICAL,
            HORIZONTAL,
            BOX,
            CIRCLE
        }
        public enum ChartViewMode
        {
            NORMALIZED,
            ABSOLUTE
        }
        Chart chartOwner;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Chart ChartOwner
        {
            get { return chartOwner; }
            set { chartOwner = value; }
        }

        string chartName = "Chart";
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ChartName
        {
            get { return chartName; }
            set
            {
                chartName = value;
                Text = chartName;
            }
        }

        protected int dragZoomPenWidth = 1;
        protected int chartBorder = 10;
        protected Rectangle chartBounds = new Rectangle(0, 0, 0, 0);
        protected List<bool> startMouseDrag = new List<bool>();
        protected List<bool> startMouseMove = new List<bool>();
        protected List<Point> chartLastCursorPos = new List<Point>();
        protected List<Point> chartStartCursorPos = new List<Point>();

        string xChannelName;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string XChannelName
        {
            get { return xChannelName; }
            set { xChannelName = value; }
        }
        //
        // chart display properties 
        //
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
        bool showHScroll = true;
        /// <summary>
        /// show the H scrollbar
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowHScroll
        {
            get { return showHScroll; }
            set
            {
                showHScroll = value;
                UpdateElementPositions();
            }
        }
        bool showVScroll = true;
        /// <summary>
        /// show the V scrollbar
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowVScroll
        {
            get { return showVScroll; }
            set
            {
                showVScroll = value;
                UpdateElementPositions();
            }
        }
        bool equalScale = false;
        // true if X and Y scale are equal regardless of window size
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EqualScale
        {
            get { return equalScale; }
            set { equalScale = value; }
        }
        ChartViewMode chartMode = ChartViewMode.ABSOLUTE;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartViewMode ChartMode
        {
            get { return chartMode; }
            set { chartMode = value; }
        }
        public ChartView()
        {
            InitializeComponent();
            xChannelName = "Time";
            CursorUpdateSource = true;

            chartLastCursorPos.Add(new Point(0, 0));
            chartStartCursorPos.Add(new Point(0, 0));
            startMouseMove.Add(false);
            startMouseDrag.Add(false);
            hScrollBar.Scroll += HScrollBar_Scroll;
        }
        #region control message handlers
        /// <summary>
        /// when the control sizes, update positions of the axes and chart it contains
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void ChartView_Resize(object sender, EventArgs e)
        {
            UpdateElementPositions();
        }
        #endregion

        #region chart message handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal virtual void chartPanel_Paint(object sender, PaintEventArgs e)
        {
            if (ChartOwner != null)
            {
                if (ChartOwner.ChartViewType == Chart.ChartType.Stripchart)
                {
                    DrawStripChart();
                }
                else if (ChartOwner.ChartViewType == Chart.ChartType.XYChart)
                {
                    DrawTractionCircle();
                }
            }
        }
        void DrawStripChart()
        {
            int idx = 0;
            #region initialize mouse/cursor moves
            for (int moveIdx = 0; moveIdx < startMouseMove.Count; moveIdx++)
            {
                startMouseMove[moveIdx] = false;
                startMouseDrag[moveIdx] = false;
                chartStartCursorPos[moveIdx] = new Point(0, 0);
                chartLastCursorPos[moveIdx] = new Point(0, 0);
            }
            #endregion
            #region nothing to display yet
            if ((ChartOwner.Y_Axes == null) || (ChartOwner.Y_Axes.Count == 0) || (ChartOwner.dataSets.Count == 0))
            {
                return;
            }
            #endregion
            #region initializations
            // has first point on channel been processed - used to set initial point of path without drawing line from 0,0
            bool initialValue = false;
            // x and y scale
            float[] displayScale = new float[] { 1.0F, 1.0F };
            // path points and pen for drawing channels
            PointF[] points = new PointF[] { new PointF(), new PointF() };
            Pen pathPen = new Pen(Color.Red);
            #endregion
            #region get X axis display range based on all visible channels
            // range can be time or distance
            ChartOwner.X_Axes.ElementAt(0).Value.AxisDisplayRange[0] = float.MaxValue;
            ChartOwner.X_Axes.ElementAt(0).Value.AxisDisplayRange[1] = float.MinValue;
            ChartOwner.X_Axes.ElementAt(0).Value.AxisDisplayRange[2] = 0;
            foreach(DisplayDataSet dataSet in ChartOwner.dataSets)
            {
                foreach (KeyValuePair<string, ChartChannel> chanInfo in dataSet.channels)
                {
                    if (!chanInfo.Value.ShowChannel)
                    {
                        continue;
                    }
                    // time based x axis
                    if (xChannelName == "Time")
                    {
                        ChartOwner.X_Axes.ElementAt(0).Value.AxisDisplayRange[0] = dataSet.channels["Time"].XRange[0] < ChartOwner.X_Axes.ElementAt(0).Value.AxisDisplayRange[0] ? dataSet.channels["Time"].XRange[0] : ChartOwner.X_Axes.ElementAt(0).Value.AxisDisplayRange[0];
                        ChartOwner.X_Axes.ElementAt(0).Value.AxisDisplayRange[1] = dataSet.channels["Time"].XRange[1] > ChartOwner.X_Axes.ElementAt(0).Value.AxisDisplayRange[1] ? dataSet.channels["Time"].XRange[1] : ChartOwner.X_Axes.ElementAt(0).Value.AxisDisplayRange[1];
                        ChartOwner.X_Axes.ElementAt(0).Value.AxisDisplayRange[2] = dataSet.channels["Time"].XRange[1] - dataSet.channels["Time"].XRange[0];
                    }
                    // distance based x axis
                    else if (xChannelName == "xTime")
                    {
                        ChartOwner.X_Axes.ElementAt(0).Value.AxisDisplayRange[0] = dataSet.channels["xDistance"].XRange[0] < ChartOwner.X_Axes.ElementAt(0).Value.AxisDisplayRange[0] ? dataSet.channels["xDistance"].XRange[0] : ChartOwner.X_Axes.ElementAt(0).Value.AxisDisplayRange[0];
                        ChartOwner.X_Axes.ElementAt(0).Value.AxisDisplayRange[1] = dataSet.channels["xDistance"].XRange[1] > ChartOwner.X_Axes.ElementAt(0).Value.AxisDisplayRange[1] ? dataSet.channels["xDistance"].XRange[1] : ChartOwner.X_Axes.ElementAt(0).Value.AxisDisplayRange[1];
                        ChartOwner.X_Axes.ElementAt(0).Value.AxisDisplayRange[2] = dataSet.channels["xDistance"].XRange[1] - dataSet.channels["xDistance"].XRange[0];
                    }
                    // only need to process 1 channel for x axis range since all channels use same x axis values
                    break;
                }
            }
            #endregion
            // process each axis
            foreach (KeyValuePair<string, Axis> yAxis in ChartOwner.Y_Axes)
            {
                #region absolute mode Y axis range
                // in absolute mode, get full range for Y axis on all displayed channels
                // update Y axis display range based on associated channels
                if (ChartMode == ChartViewMode.ABSOLUTE)
                {
                    yAxis.Value.AxisDisplayRange[0] = float.MaxValue;
                    yAxis.Value.AxisDisplayRange[1] = float.MinValue;
                    yAxis.Value.AxisDisplayRange[2] = 0;
                    foreach (ChartChannel curChanInfo in yAxis.Value.AssociatedChannels)
                    {
                        if (!curChanInfo.ShowChannel)
                        {
                            continue;
                        }
                        yAxis.Value.AxisDisplayRange[0] = curChanInfo.YRange[0] < yAxis.Value.AxisDisplayRange[0] ? curChanInfo.YRange[0] : yAxis.Value.AxisDisplayRange[0];
                        yAxis.Value.AxisDisplayRange[1] = curChanInfo.YRange[1] > yAxis.Value.AxisDisplayRange[1] ? curChanInfo.YRange[1] : yAxis.Value.AxisDisplayRange[1];
                        yAxis.Value.AxisDisplayRange[2] = curChanInfo.YRange[1] - curChanInfo.YRange[0];
                    }
                }
                #endregion

                // process each associated channel on Y axis
                foreach (ChartChannel curChanInfo in yAxis.Value.AssociatedChannels)
                {
                    #region Y axis range for normalized display
                    if (!curChanInfo.ShowChannel)
                    {
                        continue;
                    }
                    // if chart mode is normalized, get range for Y axis on this channel only and use that for display range so each channel is scaled to fill display area
                    if (ChartMode == ChartViewMode.NORMALIZED)
                    {
                        yAxis.Value.AxisDisplayRange[0] = yAxis.Value.AxisDisplayRange[0];
                        yAxis.Value.AxisDisplayRange[1] = yAxis.Value.AxisDisplayRange[1];
                        yAxis.Value.AxisDisplayRange[2] = curChanInfo.YRange[1] - curChanInfo.YRange[0];
                    }
                    #endregion
                    if(xChannelName == "Time")
                    { 
                        ChartOwner.X_Axes.ElementAt(0).Value.AxisOffset = ChartOwner.dataSets[curChanInfo.DataSetIndex].TimeOffset;
                    }
                    else if (xChannelName == "xTime")
                    {
                        ChartOwner.X_Axes.ElementAt(0).Value.AxisOffset = ChartOwner.dataSets[curChanInfo.DataSetIndex].DistanceOffset;
                    }
                    //if (xChannelName == "Time")
                    //{
                    //    //curChanInfo.Value.AxisOffset[0] = Logger.runData[curChanInfo.Value.RunIndex].TimeOffset;
                    //    curChanInfo.AxisOffset[0] = ChartOwner.dataSets[curChanInfo.RunIndex].TimeOffset;
                    //}
                    //else if (xChannelName == "xTime")
                    //{
                    //    //curChanInfo.Value.AxisOffset[0] = Logger.runData[curChanInfo.Value.RunIndex].DistanceOffset;
                    //    curChanInfo.AxisOffset[0] = ChartOwner.dataSets[curChanInfo.RunIndex].DistanceOffset;
                    //}
                    #region build unscaled path
                    if ((curChanInfo.ChannelPath == null) || (curChanInfo.ChannelPath.PointCount == 0))
                    {
                        initialValue = true;
                        foreach (KeyValuePair<float, float> curData in curChanInfo.DataPoints)
                        {
                            if(curChanInfo == null)
                            {
                                continue;
                            }
                            // x axis is time - direct lookup
                            // key is time, value is data value at time
                            if (xChannelName == "Time")
                            {
                                points[1] = new PointF(curData.Key, curData.Value);
                            }
                            // x axis is not time
                            // use data point time to look up distance in xTime channel
                            else if (xChannelName == "xTime")
                            {
                                // this point is time/value
                                float timeValue = ChartOwner.dataSets[curChanInfo.DataSetIndex].channels[xChannelName].dataPoints.FirstOrDefault(i => i.Key >= curData.Key).Value;
                                //if (timeValue == null)
                                //{
                                //    continue;
                                //}
                                // use time to find distance in xTime channel
                                float timeDistance = ChartOwner.dataSets[curChanInfo.DataSetIndex].channels["xTime"].dataPoints.FirstOrDefault(i => i.Key >= curData.Key).Value;
                                //if (timeDistance == null)
                                //{
                                //    continue;
                                //}
                                // timeDistance.PointValue is distance, curData.Value.PointValue is value
                                points[1] = new PointF(timeDistance, curData.Value);
                            }
                            // x axis is not time - use time from current channel point, find nearest time to that time in axis channel
                            else
                            {
                                float tst = ChartOwner.dataSets[curChanInfo.DataSetIndex].channels[xChannelName].dataPoints.FirstOrDefault(i => i.Key >= curData.Key).Value;
                                //if (tst == null)
                                //{
                                //    continue;
                                //}
                                points[1] = new PointF(tst, curData.Value);
                            }
                            if (initialValue)
                            {
                                initialValue = false;
                                points[0] = new PointF(points[1].X, points[1].Y);
                                continue;
                            }
                            curChanInfo.ChannelPath.AddLine(points[0], points[1]);
                            points[0] = new PointF(points[1].X, points[1].Y);
                        }
                    }
                    #endregion
                    #region draw to transformed graphic context
                    pathPen = new Pen(curChanInfo.ChannelColor);
                    using (Graphics chartGraphics = chartPanel.CreateGraphics())
                    {
                        displayScale[0] = (float)chartBounds.Width / ChartOwner.X_Axes.ElementAt(0).Value.AxisDisplayRange[2];
                        displayScale[1] = (float)chartBounds.Height / yAxis.Value.AxisDisplayRange[2];
                        if (EqualScale)
                        {
                            if (displayScale[0] < displayScale[1])
                            {
                                displayScale[1] = displayScale[0];
                            }
                            else
                            {
                                displayScale[0] = displayScale[1];
                            }
                        }
                        displayScale[1] *= -1.0F;
                        // translate to lower left corner of display area
                        chartGraphics.TranslateTransform(chartBorder, (float)chartBounds.Height + chartBorder);
                        // scale to display range in X and Y
                        chartGraphics.ScaleTransform(displayScale[0], displayScale[1]);
                        // translate by -1 * minimum display range + axis offset (scrolling)
                        chartGraphics.TranslateTransform(-1 * ChartOwner.X_Axes.ElementAt(0).Value.AxisDisplayRange[0] +
                                                         ChartOwner.X_Axes.ElementAt(0).Value.AxisOffset, // offset X
                                                         -1 * yAxis.Value.AxisDisplayRange[0] +
                                                         -1 * 0//yAxis.Value.AxisOffset// 0//yAxis.Value.AssociatedChannels[xChannelName].AxisOffset[1]);  // offset Y
                                                         );
                        // set pen width to 0 (1 pixel)
                        pathPen.Width = 0;
                        // draw the path
                        chartGraphics.DrawPath(pathPen, curChanInfo.ChannelPath);
                        // reset to original orientation
                        chartGraphics.ResetTransform();
                    }
                    #endregion
                }
            }
        }
        void DrawTractionCircle()
        {
            int idx = 0;
            //#region initialize mouse/cursor moves
            //for (int moveIdx = 0; moveIdx < startMouseMove.Count; moveIdx++)
            //{
            //    startMouseMove[moveIdx] = false;
            //    startMouseDrag[moveIdx] = false;
            //    chartStartCursorPos[moveIdx] = new Point(0, 0);
            //    chartLastCursorPos[moveIdx] = new Point(0, 0);
            //}
            //#endregion
            //// nothing to display yet
            //if ((ChartOwner.Y_Axes == null) || (ChartOwner.Y_Axes.Count == 0))
            //{
            //    return;
            //}
            //// x and y scale
            //float[] displayScale = new float[] { 1.0F, 1.0F };

            //PointF[] points = new PointF[] { new PointF(), new PointF() };
            //Pen pathPen = new Pen(Color.Red);

            //// process each axis
            //foreach (KeyValuePair<string, Axis> yAxis in ChartOwner.Y_Axes)
            //{
            //    #region skip axis if not displayed
            //    if (!yAxis.Value.ShowAxis)
            //    {
            //        continue;
            //    }
            //    #endregion
            //    // process each associated channel
            //    bool initialValue = true;
            //    foreach (ChannelInfo curChanInfo in yAxis.Value.AssociatedChannels)
            //    {
            //        #region skip if channel is not displayed
            //        if (!curChanInfo.ShowChannel)
            //        {
            //            continue;
            //        }
            //        #endregion
            //        #region build unscaled path
            //        if ((curChanInfo.ChannelPath == null) || (curChanInfo.ChannelPath.PointCount == 0))
            //        {
            //            ChartChannel curChannel = ChartOwner.dataSets[curChanInfo.RunIndex].channels[curChanInfo.ChannelName];
            //            initialValue = true;
            //            foreach (KeyValuePair<float, float> curData in curChannel.DataPoints)
            //            {
            //                // x axis is time - direct lookup
            //                if (XChannelName == "Time")
            //                {
            //                    points[1] = new PointF(curData.Key, curData.Value);
            //                }
            //                // x axis is not time - find nearest time in axis channel, 
            //                else
            //                {
            //                    float tst = ChartOwner.dataSets[curChanInfo.RunIndex].channels[XChannelName].dataPoints.FirstOrDefault(i => i.Key >= curData.Key).Value;
            //                    //if (tst == null)
            //                    //{
            //                    //    continue;
            //                    //}
            //                    points[1] = new PointF(tst, curData.Value);
            //                }
            //                if (initialValue)
            //                {
            //                    initialValue = false;
            //                    points[0] = new PointF(points[1].X, points[1].Y);
            //                    continue;
            //                }
            //                curChanInfo.ChannelPath.AddLine(points[0], points[1]);
            //                points[0] = new PointF(points[1].X, points[1].Y);
            //            }
            //        }
            //        #endregion
            //        #region draw to transformed graphic context
            //        pathPen = new Pen(Color.Black);
            //        using (Graphics chartGraphics = chartPanel.CreateGraphics())
            //        {
            //            displayScale[0] = (float)chartBounds.Width / ChartOwner.Y_Axes[XChannelName].AxisDisplayRange[2];
            //            displayScale[1] = (float)chartBounds.Height / yAxis.Value.AxisDisplayRange[2];
            //            if (EqualScale)
            //            {
            //                if (displayScale[0] < displayScale[1])
            //                {
            //                    displayScale[1] = displayScale[0];
            //                }
            //                else
            //                {
            //                    displayScale[0] = displayScale[1];
            //                }
            //            }
            //            displayScale[1] *= -1.0F;

            //            // translate to lower left corner of display area
            //            chartGraphics.TranslateTransform(chartBorder, (float)chartBounds.Height + chartBorder);
            //            // scale to display range in X and Y
            //            chartGraphics.ScaleTransform(displayScale[0], displayScale[1]);
            //            // translate by -1 * minimum display range + axis offset (scrolling)
            //            chartGraphics.TranslateTransform(-1 * ChartOwner.Y_Axes[XChannelName].AxisDisplayRange[0] +
            //                                                ChartOwner.Y_Axes[XChannelName].AssociatedChannels[XChannelName].AxisOffset[0],  // offset X
            //                                                -1 * yAxis.Value.AxisDisplayRange[0] +
            //                                                ChartOwner.Y_Axes[XChannelName].AssociatedChannels[XChannelName].AxisOffset[1]);  // offset Y
            //                                                                                                                                                                                // set pen width to 0 (1 pixel)
            //            pathPen.Width = 0;
            //            // draw the path
            //            chartGraphics.DrawLine(pathPen, -2, 0, 2, 0);
            //            chartGraphics.DrawLine(pathPen, 0, -2, 0, 2);
            //            chartGraphics.DrawEllipse(pathPen, -2, -2, 4, 4);
            //            chartGraphics.DrawPath(pathPen, curChanInfo.ChannelPath);
            //            // reset to original orientation
            //            chartGraphics.ResetTransform();
            //        }
            //        #endregion
            //    }
            //}
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void chartPanel_Resize(object sender, EventArgs e)
        {
            // update the paintable area
            chartBounds.X = chartBorder;
            chartBounds.Y = chartBorder;
            chartBounds.Width = chartPanel.Width - (2 * chartBorder);
            chartBounds.Height = chartPanel.Height - (2 * chartBorder);
            // redraw
            chartPanel.Invalidate();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void chartPanel_MouseMove(object sender, MouseEventArgs e)
        {
            //if (CursorUpdateSource == false)
            //{
            //    return;
            //}
            //chartPanel.Focus();
            //#region left mouse button down - dragging zoom region 
            //if ((e.Button == MouseButtons.Left) && AllowDrag)
            //{
            //    // starting mouse drag - erase old cursor if needed, save initial start and end locations
            //    if (!startMouseDrag[0])
            //    {
            //        // was moving mouse with left button up, erase cursor
            //        if (startMouseMove[0])
            //        {
            //            DrawCursorAt(chartLastCursorPos[0].X, chartLastCursorPos[0].Y);
            //        }
            //        // save location
            //        chartStartCursorPos[0] = new Point(e.Location.X, 0);
            //        chartLastCursorPos[0] = new Point(e.Location.X, chartPanel.Height);
            //    }
            //    // continue mouse drag - erase last box
            //    else
            //    {
            //        DrawSelectArea(chartStartCursorPos[0].X, chartStartCursorPos[0].Y, chartLastCursorPos[0].X, chartLastCursorPos[0].Y);
            //    }
            //    // continue mouse drag, save current end location and draw new box
            //    chartLastCursorPos[0] = new Point(e.Location.X, chartLastCursorPos[0].Y);
            //    DrawSelectArea(chartStartCursorPos[0].X, chartStartCursorPos[0].Y, chartLastCursorPos[0].X, chartLastCursorPos[0].Y);
            //    // mouse drag has started, mouse move has stopped
            //    startMouseDrag[0] = true;
            //    startMouseMove[0] = false;
            //}
            //#endregion
            //#region just moving the mouse with no buttons
            //else
            //{
            //    if (CursorMode != CursorStyle.NONE)
            //    {
            //        startMouseDrag[0] = false;
            //        #region erase if something was drawn
            //        if (!startMouseMove[0])
            //        {
            //            chartLastCursorPos[0] = e.Location;
            //        }
            //        else
            //        {
            //            DrawCursorAt(chartLastCursorPos[0].X, chartLastCursorPos[0].Y);
            //        }
            //        #endregion
            //        #region draw new cursor if in chart area
            //        if (((e.Location.X >= chartBorder) && (e.Location.X <= (chartPanel.Width - chartBorder))) &&
            //            ((e.Location.Y >= chartBorder) && (e.Location.Y <= (chartPanel.Height - chartBorder))))
            //        {
            //            startMouseMove[0] = true;
            //            chartLastCursorPos[0] = e.Location;
            //            DrawCursorAt(chartLastCursorPos[0].X, chartLastCursorPos[0].Y);
            //        }
            //        // outside of chart, don't draw and not started
            //        else
            //        {
            //            startMouseMove[0] = false;
            //        }
            //        #endregion
            //    }
            //}
            //#endregion
            //// up to now, dealing in screen units. convert current position to X axis value and active Y axis values and raise message
            //if (ChartOwner.Y_Axes.Count == 0)
            //{
            //    return;
            //}
            //// create event args
            //try
            //{
            //    ChartControlMouseMoveEventArgs moveEventArgs = new ChartControlMouseMoveEventArgs();

            //    PointF axisPoint = new PointF();
            //    // x and y scale
            //    float[] displayScale = new float[] { 1.0F, 1.0F };
            //    displayScale[0] = (float)chartBounds.Width / ChartOwner.Y_Axes[xChannelName].AxisDisplayRange[2];

            //    foreach (ChannelInfo channel in ChartOwner.Y_Axes[xChannelName].AssociatedChannels)
            //    {
            //        displayScale[1] = 1.0F;

            //        axisPoint.X = (float)e.Location.X;
            //        axisPoint.Y = (float)e.Location.Y;

            //        axisPoint = ScaleDisplayToData(axisPoint,
            //                            displayScale[0],
            //                            displayScale[1],
            //                            0.0F,
            //                            0.0F,
            //                            chartBounds);
            //        axisPoint.X -= (-1 * ChartOwner.Y_Axes[xChannelName].AxisDisplayRange[0] +
            //                                ChartOwner.Y_Axes[xChannelName].AssociatedChannels[channel.RunIndex.ToString() + "-" + xChannelName].AxisOffset[0]);  // offset X

            //        float timeAtCursor = 0.0F;
            //        if (channel.ChannelName == "Time")
            //        {
            //            timeAtCursor = axisPoint.X;
            //        }
            //        else if (channel.ChannelName == "xTime")
            //        {
            //            // find time from distance point
            //            float timeValue = ChartOwner.dataSets[channel.RunIndex].channels["xDistance"].dataPoints.FirstOrDefault(i => i.Key >= axisPoint.X).Value;


            //            timeAtCursor = timeValue;
            //        }
            //        moveEventArgs.XAxisValues.Add(channel.RunIndex.ToString() + "-" + "Time"/*channel.Value.ChannelName*/, timeAtCursor/*axisPoint.X*/);

            //        if (xChannelName == "xTime")
            //        {
            //            System.Diagnostics.Debug.WriteLine("xTime time at cursor: " + axisPoint.X);
            //            System.Diagnostics.Debug.WriteLine("xTime value at cursor: " + axisPoint.Y);
            //        }
            //        else
            //        {
            //            System.Diagnostics.Debug.WriteLine("Time at cursor: " + axisPoint.X);
            //            System.Diagnostics.Debug.WriteLine("Value at cursor: " + axisPoint.Y);
            //        }

            //    }
            //    foreach (KeyValuePair<string, Axis> axis in ChartOwner.Y_Axes)
            //    {
            //        if (!axis.Value.ShowAxis)
            //        {
            //            continue;
            //        }
            //        foreach (ChannelInfo channel in axis.Value.AssociatedChannels)
            //        {
            //            displayScale[1] = (float)chartBounds.Height / axis.Value.AxisDisplayRange[2];
            //            axisPoint.X = (float)e.Location.X;
            //            axisPoint.Y = (float)e.Location.Y;

            //            axisPoint = ScaleDisplayToData(axisPoint,
            //                                displayScale[0],
            //                                displayScale[1],
            //                                0.0F,
            //                                0.0F,
            //                                chartBounds);

            //            axisPoint.Y -= (channel.AxisRange[0] +
            //                                axis.Value.AxisOffset[1]);

            //            moveEventArgs.YAxisValues.Add(channel.RunIndex.ToString() + "-" + channel.ChannelName, axisPoint.Y);
            //        }
            //    }
            //    ChartMouseMoveEvent(this, moveEventArgs);
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debug.WriteLine("Error in chart mouse move event: " + ex.Message);
            //}
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void chartPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if ((startMouseDrag[0]) && AllowDrag)
            {
                // original scaling
                float[] displayScale = new float[] { 1.0F, 1.0F };
                displayScale[0] = (float)chartBounds.Width / ChartOwner.Y_Axes[xChannelName].AxisDisplayRange[2];

                PointF scaledStart = chartStartCursorPos[0];
                PointF scaledEnd = chartLastCursorPos[0];
                scaledStart = ScaleDisplayToData(scaledStart,
                                    displayScale[0],
                                    displayScale[1],
                                    0.0F,
                                    0.0F,
                                    chartBounds);
                scaledEnd = ScaleDisplayToData(scaledEnd,
                                    displayScale[0],
                                    displayScale[1],
                                    0.0F,
                                    0.0F,
                                    chartBounds);

                ChartOwner.Y_Axes[xChannelName].AxisDisplayRange[0] = scaledStart.X < scaledEnd.X ? scaledStart.X : scaledEnd.X;
                ChartOwner.Y_Axes[xChannelName].AxisDisplayRange[1] = scaledStart.X < scaledEnd.X ? scaledEnd.X : scaledStart.X;
                ChartOwner.Y_Axes[xChannelName].AxisDisplayRange[2] = ChartOwner.Y_Axes[xChannelName].AxisDisplayRange[1] - ChartOwner.Y_Axes[xChannelName].AxisDisplayRange[0];


                hScrollBar.Minimum = (int)ChartOwner.Y_Axes[xChannelName].AxisValueRange[0];
                hScrollBar.Maximum = (int)ChartOwner.Y_Axes[xChannelName].AxisValueRange[1];
                hScrollBar.Value = (int)ChartOwner.Y_Axes[xChannelName].AxisDisplayRange[0];
                hScrollBar.LargeChange = (int)ChartOwner.Y_Axes[xChannelName].AxisDisplayRange[2];
                startMouseDrag[0] = false;
                chartPanel.Invalidate();
            }
        }
        #endregion

        #region scollbar message handlers
        internal void HScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            // original scaling
            float[] displayScale = new float[] { 1.0F, 1.0F };
            displayScale[0] = (float)chartBounds.Width / ChartOwner.Y_Axes[xChannelName].AxisDisplayRange[2];

            PointF scaledStart = new PointF(e.NewValue, 0);
            ChartOwner.Y_Axes[xChannelName].AxisDisplayRange[0] = scaledStart.X;
            ChartOwner.Y_Axes[xChannelName].AxisDisplayRange[1] = scaledStart.X + ChartOwner.Y_Axes[xChannelName].AxisDisplayRange[2];
            chartPanel.Invalidate();

        }
        #endregion

        #region GDI support
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static uint RGB(Color color)
        {
            uint rgb = (uint)(color.R + (color.G << 8) + (color.B << 16));
            return rgb;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static uint NotRGB(Color color)
        {
            uint rgb = (uint)(color.R + (color.G << 8) + (color.B << 16));
            rgb = ~rgb & 0xFFFFFF;
            return rgb;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        internal void DrawCursorAt(int x, int y)
        {
            Rectangle locationBox = new Rectangle(0, 0, 0, 0);
            IntPtr lpPoint = new IntPtr();
            lpPoint = IntPtr.Zero;
            using (Graphics drawGraphics = chartPanel.CreateGraphics())
            {
                #region create GDI objects
                IntPtr hDC = drawGraphics.GetHdc();
                IntPtr newPen = Gdi32.CreatePen((int)PenStyles.PS_SOLID,
                                                dragZoomPenWidth,
                                                NotRGB(Color.Black));
                IntPtr newBrush = Gdi32.GetStockObject((int)StockObjects.BLACK_BRUSH);
                IntPtr oldPen = Gdi32.SelectObject(hDC, newPen);
                IntPtr oldBrush = Gdi32.SelectObject(hDC, newBrush);
                Gdi32.SetROP2(hDC, (int)DrawMode.R2_XORPEN);
                #endregion
                #region crosshairs
                if (cursorMode == CursorStyle.CROSSHAIRS)
                {
                    // horizontal line
                    Gdi32.MoveToEx(hDC, 0, y, lpPoint);
                    Gdi32.LineTo(hDC, chartPanel.Width, y);
                    // vertical line
                    Gdi32.MoveToEx(hDC, x, chartPanel.Height, lpPoint);
                    Gdi32.LineTo(hDC, x, 0);
                }
                #endregion
                #region box
                else if (cursorMode == CursorStyle.BOX)
                {
                    locationBox = new Rectangle(x - (cursorBoxSize / 2), y - (cursorBoxSize / 2), cursorBoxSize, cursorBoxSize);
                    Gdi32.Rectangle(hDC, locationBox.Left, locationBox.Top, locationBox.Left + cursorBoxSize, locationBox.Top + cursorBoxSize);
                }
                #endregion
                #region circle
                else if (cursorMode == CursorStyle.CIRCLE)
                {
                    locationBox = new Rectangle(x - (cursorBoxSize / 2), y - (cursorBoxSize / 2), cursorBoxSize, cursorBoxSize);
                    Gdi32.Ellipse(hDC, locationBox.Left, locationBox.Top, locationBox.Left + cursorBoxSize, locationBox.Top + cursorBoxSize);
                }
                #endregion
                #region horizontal line
                else if (cursorMode == CursorStyle.HORIZONTAL)
                {
                    // horizontal line
                    Gdi32.MoveToEx(hDC, 0, y, lpPoint);
                    Gdi32.LineTo(hDC, chartPanel.Width, y);
                }
                #endregion
                #region vertical line
                else if (cursorMode == CursorStyle.VERTICAL)
                {
                    // vertical line
                    Gdi32.MoveToEx(hDC, x, 0, lpPoint);
                    Gdi32.LineTo(hDC, x, chartPanel.Height);
                }
                #endregion
                #region clean up GDI, reset context
                Gdi32.SelectObject(hDC, oldPen);
                Gdi32.SelectObject(hDC, oldBrush);
                Gdi32.DeleteObject(newPen);
                Gdi32.DeleteObject(newBrush);
                drawGraphics.ReleaseHdc();
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromX"></param>
        /// <param name="fromY"></param>
        /// <param name="toX"></param>
        /// <param name="toY"></param>
        internal void DrawSelectArea(int fromX, int fromY, int toX, int toY)
        {
            using (Graphics drawGraphics = chartPanel.CreateGraphics())
            {
                #region create GDI objects
                IntPtr hDC = drawGraphics.GetHdc();
                IntPtr newPen = Gdi32.CreatePen((int)PenStyles.PS_SOLID,
                                                dragZoomPenWidth,
                                                NotRGB(Color.Gray));
                IntPtr newBrush = Gdi32.GetStockObject((int)StockObjects.GRAY_BRUSH);
                IntPtr oldPen = Gdi32.SelectObject(hDC, newPen);
                IntPtr oldBrush = Gdi32.SelectObject(hDC, newBrush);
                Gdi32.SetROP2(hDC, (int)DrawMode.R2_XORPEN);
                #endregion
                Gdi32.Rectangle(hDC, fromX, fromY, toX, toY);
                #region clean up GDI restore context
                Gdi32.SelectObject(hDC, oldPen);
                Gdi32.SelectObject(hDC, oldBrush);
                Gdi32.DeleteObject(newPen);
                Gdi32.DeleteObject(newBrush);
                drawGraphics.ReleaseHdc();
                #endregion
            }
        }
        #endregion

        #region scaling support
        /// <summary>
        /// convert sourcePt from data units to display units
        /// </summary>
        /// <param name="sourcePt"></param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        internal PointF ScaleDataToDisplay(PointF sourcePt, float scaleX, float scaleY, float offsetX, float offsetY, Rectangle bounds)
        {
            PointF rPt = new PointF(sourcePt.X, sourcePt.Y);
            rPt.X = (rPt.X + offsetX) * scaleX + bounds.X;
            rPt.Y = bounds.Height - ((rPt.Y - offsetY) * scaleY) + bounds.Y;
            return rPt;
        }
        /// <summary>
        /// convert sourcePt from display units to data units
        /// </summary>
        /// <param name="sourcePt"></param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        internal PointF ScaleDisplayToData(PointF sourcePt, float scaleX, float scaleY, float offsetX, float offsetY, Rectangle bounds)
        {
            PointF rPt = new PointF(sourcePt.X, sourcePt.Y);
            rPt.X = (rPt.X / scaleX) - offsetX;
            rPt.Y = -1.0F * ((rPt.Y - (float)bounds.Height) / scaleY) - offsetY;
            return rPt;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void UpdateElementPositions()
        {
            Rectangle chartRect = new Rectangle();
            // default shown positions
            hScrollBar.Height = 17;
            hScrollBar.Width = Width - 17;
            hScrollBar.Location = new Point(17, Height - 17);

            vScrollBar.Height = Height - 17;
            vScrollBar.Width = 17;
            vScrollBar.Location = new Point(0, 0);

            chartRect.X = 17;
            chartRect.Y = 0;
            chartRect.Width = Width - 17;
            chartRect.Height = Height - 17;

            if (!showHScroll)
            {
                hScrollBar.Visible = false;
                chartRect.Height = Height;
            }
            if (!showVScroll)
            {
                vScrollBar.Visible = false;
                chartRect.X = 0;
                chartRect.Width = Width;
            }

            chartPanel.Location = chartRect.Location;
            chartPanel.Width = chartRect.Width;
            chartPanel.Height = chartRect.Height;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void OnChartMouseMove(object sender, ChartControlMouseMoveEventArgs e)
        {
            //#region move cursor(s)
            //// position in event args is data - need to scale to screen
            //float[] displayScale = new float[] { 1.0F, 1.0F };
            //PointF endPt = new PointF();
            //int axisIdx = 0;
            //string axisFullName = "";
            //int cursorIdx = -1;
            //if (CursorMode != CursorStyle.NONE)
            //{
            //    // check each Y axis
            //    foreach (KeyValuePair<string, Axis> yAxis in ChartOwner.Y_Axes)
            //    {
            //        //ChartOwner.ChartAxes[e.Node.Name].ShowAxis = e.Node.Checked;
            //        // skip if axis is not displayed
            //        if (!yAxis.Value.ShowAxis)
            //        //if (!yAxis.Value.AssociatedChannels[0].ShowChannel)
            //        {
            //            continue;
            //        }
            //        axisFullName = axisIdx.ToString() + "-" + yAxis.Key;
            //        displayScale[0] = (float)chartBounds.Width / ChartOwner.Y_Axes[xChannelName].AxisDisplayRange[2];
            //        displayScale[1] = (float)chartBounds.Height / yAxis.Value.AxisDisplayRange[2];
            //        if (EqualScale)
            //        {
            //            if (displayScale[0] < displayScale[1])
            //            {
            //                displayScale[1] = displayScale[0];
            //            }
            //            else
            //            {
            //                displayScale[0] = displayScale[1];
            //            }
            //        }
            //        // check each associated channel
            //        foreach (KeyValuePair<String, ChannelInfo> curChanInfo in yAxis.Value.AssociatedChannels)
            //        {
            //            // skip if channel is not displayed
            //            if (!curChanInfo.Value.ShowChannel)
            //            {
            //                continue;
            //            }
            //            cursorIdx++;
            //            // add new cursor info if needed
            //            if (startMouseMove.Count <= cursorIdx)
            //            {
            //                startMouseMove.Add(false);
            //                startMouseDrag.Add(false);
            //                chartStartCursorPos.Add(new Point(0, 0));
            //                chartLastCursorPos.Add(new Point(0, 0));
            //            }
            //            ChartChannel curChannel = ChartOwner.dataSets[curChanInfo.Value.RunIndex].channels[curChanInfo.Value.ChannelName];

            //            // x axis is time - direct lookup
            //            if (axisFullName == (axisIdx.ToString() + "-Time"))
            //            {
            //                endPt.X = e.XAxisValues[xChannelName];// curChannel.DataPoints[].PointValue;
            //                endPt.Y = curChannel.DataPoints[endPt.X];
            //            }
            //            // x axis is not time - find nearest time in axis channel, 
            //            else
            //            {
            //                float tst = ChartOwner.dataSets[curChanInfo.Value.RunIndex].channels[xChannelName].dataPoints.FirstOrDefault(i => i.Key >= e.XAxisValues[curChanInfo.Value.RunIndex.ToString() + "-Time"]).Value;
            //                //if (tst == null)
            //                //{
            //                //    continue;
            //                //}
            //                endPt.X = tst;
            //                tst = curChannel.dataPoints.FirstOrDefault(i => i.Key >= e.XAxisValues[curChanInfo.Value.RunIndex.ToString() + "-Time"]).Value;
            //                //if (tst == null)
            //                //{
            //                //    continue;
            //                //}
            //                endPt.Y = tst;
            //            }
            //            endPt = ScaleDataToDisplay(endPt,                                                      // point
            //                                        displayScale[0],                              // scale X
            //                                        displayScale[1],                              // scale Y
            //                                        -1 * ChartOwner.Y_Axes[xChannelName].AxisDisplayRange[0] +
            //                                                    ChartOwner.Y_Axes[xChannelName].AssociatedChannels[curChanInfo.Value.RunIndex.ToString() + "-" + xChannelName].AxisOffset[0],  // offset X
            //                                        yAxis.Value.AxisDisplayRange[0] +
            //                                                    ChartOwner.Y_Axes[xChannelName].AssociatedChannels[curChanInfo.Value.RunIndex.ToString() + "-" + xChannelName].AxisOffset[1],  // offset Y
            //                                        chartBounds);                                 // graphics area boundary


            //            startMouseDrag[cursorIdx] = false;
            //            #region erase if something was drawn
            //            if (!startMouseMove[cursorIdx])
            //            {
            //                chartLastCursorPos[cursorIdx] = new Point((int)endPt.X, (int)endPt.Y);
            //            }
            //            else
            //            {
            //                DrawCursorAt(chartLastCursorPos[cursorIdx].X, chartLastCursorPos[cursorIdx].Y);
            //            }
            //            #endregion
            //            #region draw new cursor if in chart area
            //            if (((endPt.X >= chartBorder) && (endPt.X <= (chartPanel.Width - chartBorder))) &&
            //                ((endPt.Y >= chartBorder) && (endPt.Y <= (chartPanel.Height - chartBorder))))
            //            {
            //                startMouseMove[cursorIdx] = true;
            //                chartLastCursorPos[cursorIdx] = new Point((int)endPt.X, (int)endPt.Y);
            //                DrawCursorAt(chartLastCursorPos[cursorIdx].X, chartLastCursorPos[cursorIdx].Y);
            //            }
            //            // outside of chart, don't draw and not started
            //            else
            //            {
            //                startMouseMove[cursorIdx] = false;
            //            }
            //            #endregion
            //        }
            //    }
            //}
            //#endregion

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnChartXAxisChange(object sender, ChartControlXAxisChangeEventArgs e)
        {
            xChannelName = e.XAxisName;
            //hScrollBar.Minimum = (int)ChartOwner.Y_Axes[xChannelName].AxisValueRange[0];
            //hScrollBar.Maximum = (int)ChartOwner.Y_Axes[xChannelName].AxisValueRange[1];
            //hScrollBar.Value = (int)ChartOwner.Y_Axes[xChannelName].AxisDisplayRange[0];
            //hScrollBar.LargeChange = (int)ChartOwner.Y_Axes[xChannelName].AxisDisplayRange[2];
            chartPanel.Invalidate();
        }
        public void OnClearGraphicsPath(object sender, EventArgs e)
        {
            foreach (KeyValuePair<string, Axis> yAxis in ChartOwner.Y_Axes)
            {
                foreach (ChartChannel curChanInfo in yAxis.Value.AssociatedChannels)
                {
                    curChanInfo.ChannelPath = new GraphicsPath();
                }
            }
            chartPanel.Invalidate();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnAxisOffsetUpdate(object sender, AxisOffsetUpdateEventArgs e)
        {
            //// offset channel on X axis
            //string associatedAxisName = e.RunIdx.ToString() + "-" + e.ChannelName;
            //if (e.AxisIdx == 0)
            //{
            //    ChartOwner.Y_Axes[AxisIdx].AssociatedChannels[associatedAxisName].AxisOffset[0] = e.OffsetVal;
            //}
            //chartPanel.Invalidate();
        }
    }
    public class ChartControlMouseMoveEventArgs : EventArgs
    {
        Dictionary<string, float> xAxisValues = new Dictionary<string, float>();
        public Dictionary<string, float> XAxisValues
        {
            get { return xAxisValues; }
            set { xAxisValues = value; }
        }
        Dictionary<string, float> yAxisValues = new Dictionary<string, float>();
        public Dictionary<string, float> YAxisValues
        {
            get { return yAxisValues; }
            set { yAxisValues = value; }
        }

    }
    public class ChartControlXAxisChangeEventArgs : EventArgs
    {
        string xAxisName;
        public string XAxisName
        {
            get { return xAxisName; }
            set { xAxisName = value; }
        }
    }
    public class AxisOffsetUpdateEventArgs : EventArgs
    {
        string channelName;
        public string ChannelName
        {
            get { return channelName; }
            set { channelName = value; }
        }
        int axisIdx;
        public int AxisIdx
        {
            get { return axisIdx; }
            set { axisIdx = value; }
        }
        int runIdx;
        public int RunIdx
        {
            get { return runIdx; }
            set { runIdx = value; }
        }
        float offsetVal;
        public float OffsetVal
        {
            get { return offsetVal; }
            set { offsetVal = value; }
        }

        public AxisOffsetUpdateEventArgs(string name, int run, int axis, float offset)
        {
            ChannelName = name;
            RunIdx = run;
            AxisIdx = axis;
            OffsetVal = offset;
        }
    }
    public class DataChannel
    {
        String channelName;
        String channelDescription;
        String channelSource;
        float channelScale;
        float[] xRange = new float[] { float.MaxValue, float.MinValue };
        float[] yRange = new float[] { float.MaxValue, float.MinValue };
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
        public DataChannel(String name, String desc, String src, float scale)
        {
            channelName = name;
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
        public void AddPoint(float timeStamp, float value)
        {
            if (DataPoints.ContainsKey(timeStamp))
            {
                DataPoints[timeStamp] = value;
            }
            else
            {
                DataPoints.Add(timeStamp, value);
            }
            xRange[0] = timeStamp < xRange[0] ? timeStamp : xRange[0];
            xRange[1] = timeStamp > xRange[1] ? timeStamp : xRange[1];
            xRange[2] = xRange[1] - xRange[0];
            yRange[0] = value < yRange[0] ? value : yRange[0];
            yRange[1] = value > yRange[1] ? value : yRange[1];
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
}
