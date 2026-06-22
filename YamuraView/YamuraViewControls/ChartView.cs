using GDI;
using Microsoft.VisualBasic.Devices;
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
using System.Windows.Forms.PropertyGridInternal;
using Win32Interop.Methods;
using Win32Interop.Structs;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace YamuraViewControls
{
    public partial class ChartView : UserControl
    {
        #region delegates
        //public delegate void ChartMouseMove(object sender, ChartControlMouseTrackEventArgs e);
        public delegate void ChartMouseTrack(object sender, ChartControlMouseTrackEventArgs e);
        public delegate void ChartXAxisChange(object sender, ChartControlXAxisChangeEventArgs e);
        public delegate void AxisOffsetUpdate(object sender, AxisOffsetUpdateEventArgs e);
        public delegate void ClearGraphicsPath(object sender, EventArgs e);
        #endregion

        #region events
        public event ChartMouseTrack ChartMouseTrackEvent;
        #endregion

        #region enums
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
        #endregion

        #region members
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

        protected int dragZoomPenWidth = 2;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<bool> StartMouseDrag { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<bool> StartMouseMove { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<Point> ChartLastCursorPos { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<Point> ChartStartCursorPos { get; set; }

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
        ChartViewMode chartMode = ChartViewMode.ABSOLUTE;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartViewMode ChartMode
        {
            get { return chartMode; }
            set { chartMode = value; }
        }
        // overlay state (for paint-overlay cursor and selection)
        private bool isSelecting = false;
        private Rectangle selectionRect = Rectangle.Empty;
        private Point cursorPos = Point.Empty;
        private bool cursorVisible = false;

        public float[] displayScale = new float[2];

        // cursor overlay state
        private SortedList<string, SortedList<string, float>> _cursorValues = null;
        private float _cursorDataX = float.NaN;
        private Panel _overlayPanel;
        private bool _hasCursorPos = false;
        private Point _savedCursorPos = Point.Empty;
        static public SolidBrush overlayBrush = new SolidBrush(Color.White);
        ChartControlMouseTrackEventArgs outArgs = new ChartControlMouseTrackEventArgs();
        #endregion
        /// <summary>
        /// 
        /// </summary>
        public ChartView()
        {
            InitializeComponent();
            ChartLastCursorPos = new List<Point>();
            ChartLastCursorPos.Add(new Point(0, 0));
            ChartStartCursorPos = new List<Point>();
            ChartStartCursorPos.Add(new Point(0, 0));
            StartMouseMove = new List<bool>();
            StartMouseMove.Add(false);
            StartMouseDrag = new List<bool>();
            StartMouseDrag.Add(false);
            hScrollBar.Scroll += HScrollBar_Scroll;
            chartPanel.MouseLeave += ChartPanel_MouseLeave;
            _overlayPanel = new Panel { BackColor = Color.Black, Visible = false, Enabled = false };
            _overlayPanel.Paint += OverlayPanel_Paint;
            chartPanel.Controls.Add(_overlayPanel);
            _overlayPanel.BringToFront();
            // double buffer for draw speed
            // removed now for cursor ghosting when update panel moves 
            //chartPanel.GetType()
            //    .GetProperty("DoubleBuffered",
            //        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            //    ?.SetValue(chartPanel, true);
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
            if (ChartOwner == null)
            {
                return;
            }
            DrawChartView(e.Graphics);
            // after every repaint, restore XOR cursor if mouse is still over chart
            // DrawChartView resets ChartLastCursorPos to (0,0), so use _savedCursorPos
            if (_hasCursorPos && StartMouseMove.Count > 0)
            {
                DrawCursorAtScreenPoint(_savedCursorPos);
                StartMouseMove[0] = true;
                ChartLastCursorPos[0] = _savedCursorPos;
            }
        }
        /// <summary>
        /// returns number of stacked graph bands needed (max GraphIndex across visible channels + 1)
        /// </summary>
        private int ComputeGraphCount()
        {
            int max = 0;
            foreach (var yAxisKvp in ChartOwner.Y_Axes)
                foreach (var chan in yAxisKvp.Value.AssociatedChannels)
                    if (chan.ShowChannel && chan.GraphIndex > max)
                        max = chan.GraphIndex;
            return max + 1;
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="chartGraphics"></param>
        void DrawChartView(Graphics chartGraphics)
        {
            #region initialize mouse/cursor moves
            for (int moveIdx = 0; moveIdx < StartMouseMove.Count; moveIdx++)
            {
                StartMouseMove[moveIdx] = false;
                StartMouseDrag[moveIdx] = false;
                ChartStartCursorPos[moveIdx] = new Point(0, 0);
                ChartLastCursorPos[moveIdx] = new Point(0, 0);
            }
            #endregion
            #region nothing to display yet
            if ((ChartOwner.Y_Axes == null) || (ChartOwner.Y_Axes.Count == 0) || (ChartOwner.dataSets.Count == 0))
            {
                return;
            }
            #endregion
            #region initializations
            bool initialValue = false;
            PointF[] points = new PointF[] { new PointF(), new PointF() };
            #endregion
            #region get X axis display range based on all visible channels
            Axis primaryX = ChartOwner.X_Axes.ElementAtOrDefault(0).Value;
            if (primaryX == null)
            {
                return;
            }
            primaryX.AxisDisplayRange[0] = float.MaxValue;
            primaryX.AxisDisplayRange[1] = float.MinValue;
            primaryX.AxisDisplayRange[2] = 0;
            foreach (DisplayDataSet dataSet in ChartOwner.dataSets)
            {
                if (ChartOwner.XChannelName == "Time")
                {
                    float tMin = dataSet.channels["Time"].XRange[0] + dataSet.TimeOffset;
                    float tMax = dataSet.channels["Time"].XRange[1] + dataSet.TimeOffset;
                    if (tMin < primaryX.AxisDisplayRange[0]) primaryX.AxisDisplayRange[0] = tMin;
                    if (tMax > primaryX.AxisDisplayRange[1]) primaryX.AxisDisplayRange[1] = tMax;
                    primaryX.AxisDisplayRange[2] = dataSet.channels["Time"].XRange[1] - dataSet.channels["Time"].XRange[0];
                }
                else if (ChartOwner.XChannelName == "Distance")
                {
                    float dMin = dataSet.channels["xDistance"].XRange[0];
                    float dMax = dataSet.channels["xDistance"].XRange[1];
                    if (dMin < primaryX.AxisDisplayRange[0]) primaryX.AxisDisplayRange[0] = dMin;
                    if (dMax > primaryX.AxisDisplayRange[1]) primaryX.AxisDisplayRange[1] = dMax;
                    primaryX.AxisDisplayRange[2] = dMax - dMin;
                }
                else
                {
                    float xMin = dataSet.channels[ChartOwner.XChannelName].YRange[0];
                    float xMax = dataSet.channels[ChartOwner.XChannelName].YRange[1];
                    if (xMin < primaryX.AxisDisplayRange[0]) primaryX.AxisDisplayRange[0] = xMin;
                    if (xMax > primaryX.AxisDisplayRange[1]) primaryX.AxisDisplayRange[1] = xMax;
                    primaryX.AxisDisplayRange[2] = primaryX.AxisDisplayRange[1] - primaryX.AxisDisplayRange[0];
                }
            }
            #endregion

            int border = ChartOwner.ChartBorder;
            int drawW = ChartOwner.ChartWidth - 2 * border;
            int drawH = ChartOwner.ChartHeight - 2 * border;
            float xRange = primaryX.AxisDisplayRange[2] == 0 ? 1 : primaryX.AxisDisplayRange[2];
            float scaleX = (float)drawW / xRange;

            // stacked graph layout
            int graphCount = ComputeGraphCount();
            int graphGap = graphCount > 1 ? 4 : 0;
            int subH = (drawH - graphGap * (graphCount - 1)) / Math.Max(1, graphCount);

            // pre-compute absolute Y ranges per graph
            float[] graphYMin = new float[graphCount];
            float[] graphYMax = new float[graphCount];
            for (int g = 0; g < graphCount; g++) { graphYMin[g] = float.MaxValue; graphYMax[g] = float.MinValue; }
            if (ChartMode == ChartViewMode.ABSOLUTE)
            {
                foreach (var yAxisKvp in ChartOwner.Y_Axes)
                    foreach (var chan in yAxisKvp.Value.AssociatedChannels)
                        if (chan.ShowChannel && chan.GraphIndex < graphCount)
                        {
                            if (chan.YRange[0] < graphYMin[chan.GraphIndex]) graphYMin[chan.GraphIndex] = chan.YRange[0];
                            if (chan.YRange[1] > graphYMax[chan.GraphIndex]) graphYMax[chan.GraphIndex] = chan.YRange[1];
                        }
            }

            // draw each stacked graph band
            for (int graphIdx = 0; graphIdx < graphCount; graphIdx++)
            {
                int subTop    = border + graphIdx * (subH + graphGap);
                int subBottom = subTop + subH;

                // separator line between bands
                if (graphIdx > 0)
                {
                    using (Pen sepPen = new Pen(Color.FromArgb(80, 80, 90)))
                        chartGraphics.DrawLine(sepPen, 0, subTop - graphGap / 2, ChartOwner.ChartWidth, subTop - graphGap / 2);
                }

                float graphYRange = graphYMax[graphIdx] - graphYMin[graphIdx];
                if (graphYRange == 0) graphYRange = 1;
                float scaleY = (float)subH / graphYRange;

                // equal scale only applies to single-graph XY views (e.g. TrackMap)
                float localScaleX = scaleX;
                if (ChartOwner.EqualScale && graphCount == 1)
                {
                    if (localScaleX < scaleY) scaleY = localScaleX;
                    else localScaleX = scaleY;
                }

                // store graph-0 scales for DrawCursorAtScaledPoint (XY charts are always single-graph)
                if (graphIdx == 0)
                {
                    displayScale[0] = localScaleX;
                    displayScale[1] = -scaleY;
                }

                foreach (KeyValuePair<string, Axis> yAxisKvp in ChartOwner.Y_Axes)
                {
                    Axis yAxis = yAxisKvp.Value;

                    foreach (ChartChannel curChanInfo in yAxis.AssociatedChannels)
                    {
                        if (!curChanInfo.ShowChannel || curChanInfo.GraphIndex != graphIdx)
                            continue;

                        // per-channel Y range (normalized) or per-graph (absolute)
                        float chanYMin, chanScaleY;
                        if (ChartMode == ChartViewMode.NORMALIZED)
                        {
                            chanYMin = curChanInfo.YRange[0];
                            float chanYRange = curChanInfo.YRange[1] - curChanInfo.YRange[0];
                            if (chanYRange == 0) chanYRange = 1;
                            chanScaleY = (float)subH / chanYRange;
                            yAxis.AxisDisplayRange[0] = curChanInfo.YRange[0];
                            yAxis.AxisDisplayRange[1] = curChanInfo.YRange[1];
                            yAxis.AxisDisplayRange[2] = chanYRange;
                        }
                        else
                        {
                            chanYMin = graphYMin[graphIdx];
                            chanScaleY = scaleY;
                            yAxis.AxisDisplayRange[0] = graphYMin[graphIdx];
                            yAxis.AxisDisplayRange[1] = graphYMax[graphIdx];
                            yAxis.AxisDisplayRange[2] = graphYRange;
                        }

                        #region X axis display offset
                        if (ChartOwner.XChannelName == "Time")
                            primaryX.AxisOffset = ChartOwner.dataSets[curChanInfo.DataSetIndex].TimeOffset;
                        else if (ChartOwner.XChannelName == "Distance")
                            primaryX.AxisOffset = ChartOwner.dataSets[curChanInfo.DataSetIndex].DistanceOffset;
                        else
                            primaryX.AxisOffset = 0.0F;
                        #endregion

                        #region build unscaled path
                        if ((curChanInfo.ChannelPath == null) || (curChanInfo.ChannelPath.PointCount == 0))
                        {
                            initialValue = true;
                            foreach (KeyValuePair<float, float> curData in curChanInfo.DataPoints)
                            {
                                if (ChartOwner.XChannelName == "Time")
                                    points[1] = new PointF(curData.Key, curData.Value);
                                else if (ChartOwner.XChannelName == "Distance")
                                {
                                    float timeDistance;
                                    FindNearestValueToKey(ChartOwner.dataSets[curChanInfo.DataSetIndex].channels["Distance"].dataPoints, curData.Key, out timeDistance);
                                    points[1] = new PointF(timeDistance, curData.Value);
                                }
                                else
                                {
                                    float tst;
                                    FindNearestValueToKey(ChartOwner.dataSets[curChanInfo.DataSetIndex].channels[ChartOwner.XChannelName].dataPoints, curData.Key, out tst);
                                    points[1] = new PointF(tst, curData.Value);
                                }
                                if (initialValue)
                                {
                                    initialValue = false;
                                    points[0] = new PointF(points[1].X, points[1].Y);
                                    continue;
                                }
                                if (ChartOwner.ChartName == "Traction Circle")
                                    curChanInfo.ChannelPath.AddEllipse(new RectangleF(points[0], new SizeF(.001F, .001F)));
                                else
                                    curChanInfo.ChannelPath.AddLine(points[0], points[1]);
                                points[0] = new PointF(points[1].X, points[1].Y);
                            }
                        }
                        #endregion

                        #region draw to transformed graphic context
                        using (Pen pathPen = new Pen(curChanInfo.ChannelColor))
                        {
                            float negScaleY = -chanScaleY;
                            float adjustXBorder = (-(primaryX.AxisDisplayRange[0] - primaryX.AxisOffset)) * localScaleX;
                            float adjustYBorder = (-chanYMin) * negScaleY;

                            chartGraphics.TranslateTransform((float)border + adjustXBorder, (float)subBottom + adjustYBorder);
                            chartGraphics.ScaleTransform(localScaleX, negScaleY);
                            pathPen.Width = 0;
                            chartGraphics.DrawPath(pathPen, curChanInfo.ChannelPath);
                            if (ChartOwner.ChartName == "Traction Circle")
                            {
                                pathPen.Color = Color.DarkGray;
                                chartGraphics.DrawEllipse(pathPen, new RectangleF(-0.5F, -0.5F, 1.0F, 1.0F));
                                chartGraphics.DrawEllipse(pathPen, new RectangleF(-1.0F, -1.0F, 2.0F, 2.0F));
                                chartGraphics.DrawEllipse(pathPen, new RectangleF(-1.5F, -1.5F, 3.0F, 3.0F));
                                chartGraphics.DrawLine(pathPen, new PointF(0, -1.75F), new PointF(0, 1.75F));
                                chartGraphics.DrawLine(pathPen, new PointF(-1.75F, 0), new PointF(1.75F, 0));
                            }
                            chartGraphics.ResetTransform();
                        }
                        #endregion
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void chartPanel_Resize(object sender, EventArgs e)
        {
            if (ChartOwner != null)
            {
                // update the paintable area
                ChartOwner.SetChartBounds(ChartOwner.ChartBorder,
                                          ChartOwner.ChartBorder,
                                          Width - (2 * ChartOwner.ChartBorder),
                                          Height - (2 * ChartOwner.ChartBorder));
            }
            // redraw
            chartPanel.Invalidate();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal void chartPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if ((StartMouseDrag[0]) && ChartOwner.AllowDrag)
            {
                // original scaling
                //float[] displayScale = new float[] { 1.0F, 1.0F };
                //displayScale[0] = (float)ChartOwner.ChartWidth / xAxis.AxisDisplayRange[2];
                Axis xAxis = ChartOwner.X_Axes["X Axis"];

                PointF scaledStart = ChartStartCursorPos[0];
                PointF scaledEnd = ChartLastCursorPos[0];
                scaledStart = ScaleDisplayToData(scaledStart,
                                    displayScale[0],
                                    displayScale[1],
                                    0.0F,
                                    0.0F,
                                    ChartOwner.ChartBounds);
                scaledEnd = ScaleDisplayToData(scaledEnd,
                                    displayScale[0],
                                    displayScale[1],
                                    0.0F,
                                    0.0F,
                                    ChartOwner.ChartBounds);

                xAxis.AxisDisplayRange[0] = scaledStart.X < scaledEnd.X ? scaledStart.X : scaledEnd.X;
                xAxis.AxisDisplayRange[1] = scaledStart.X < scaledEnd.X ? scaledEnd.X : scaledStart.X;
                xAxis.AxisDisplayRange[2] = xAxis.AxisDisplayRange[1] - xAxis.AxisDisplayRange[0];

                hScrollBar.Minimum = (int)xAxis.AxisValueRange[0];
                hScrollBar.Maximum = (int)xAxis.AxisValueRange[1];
                hScrollBar.Value = (int)xAxis.AxisDisplayRange[0];
                hScrollBar.LargeChange = (int)xAxis.AxisDisplayRange[2];
                StartMouseDrag[0] = false;
                chartPanel.Invalidate();
            }
        }
        #endregion

        #region scollbar message handlers
        internal void HScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            Axis xAxis = ChartOwner.X_Axes["X Axis"];
            xAxis.AxisDisplayRange[0] = e.NewValue;
            xAxis.AxisDisplayRange[1] = e.NewValue + xAxis.AxisDisplayRange[2];
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
        #endregion
        
        #region cursor moves
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        internal void DrawCursorAtScreenPoint(Point drawPoint, Color cursorColor = default)
        {
            Rectangle locationBox = new Rectangle(0, 0, 0, 0);
            IntPtr lpPoint = new IntPtr();
            lpPoint = IntPtr.Zero;
            using (Graphics drawGraphics = chartPanel.CreateGraphics())
            {
                #region create GDI objects
                IntPtr hDC = drawGraphics.GetHdc();
                Color penColor = cursorColor == default ? Color.Black : cursorColor;
                IntPtr newPen = Gdi32.CreatePen((int)PenStyles.PS_SOLID,
                                                dragZoomPenWidth,
                                                NotRGB(penColor));
                IntPtr newBrush = Gdi32.GetStockObject((int)StockObjects.BLACK_BRUSH);
                IntPtr oldPen = Gdi32.SelectObject(hDC, newPen);
                IntPtr oldBrush = Gdi32.SelectObject(hDC, newBrush);
                Gdi32.SetROP2(hDC, (int)DrawMode.R2_XORPEN);
                #endregion
                #region crosshairs
                if (ChartOwner.CursorMode == CursorStyle.CROSSHAIRS)
                {
                    // horizontal line
                    Gdi32.MoveToEx(hDC, 0, drawPoint.Y, lpPoint);
                    Gdi32.LineTo(hDC, chartPanel.Width, drawPoint.Y);
                    // vertical line
                    Gdi32.MoveToEx(hDC, drawPoint.X, chartPanel.Height, lpPoint);
                    Gdi32.LineTo(hDC, drawPoint.X, 0);
                }
                #endregion
                #region box
                else if (ChartOwner.CursorMode == CursorStyle.BOX)
                {
                    Point point1 = new Point(drawPoint.X - (ChartOwner.CursorBoxSize / 2), drawPoint.Y - (ChartOwner.CursorBoxSize / 2));
                    Size boxSize = new Size(ChartOwner.CursorBoxSize, ChartOwner.CursorBoxSize);
                    //locationBox = new Rectangle(drawPoint.X - (ChartOwner.CursorBoxSize / 2), 
                    //                            drawPoint.Y - (ChartOwner.CursorBoxSize / 2), 
                    //                            ChartOwner.CursorBoxSize, 
                    //                            ChartOwner.CursorBoxSize);
                    locationBox = new Rectangle(point1, boxSize);
                    Gdi32.Rectangle(hDC, locationBox.Left, locationBox.Top, locationBox.Right, locationBox.Bottom);// locationBox.Left + ChartOwner.CursorBoxSize, locationBox.Top + ChartOwner.CursorBoxSize);
                }
                #endregion
                #region circle
                else if (ChartOwner.CursorMode == CursorStyle.CIRCLE)
                {
                    locationBox = new Rectangle(drawPoint.X - (ChartOwner.CursorBoxSize / 2), drawPoint.Y - (ChartOwner.CursorBoxSize / 2), ChartOwner.CursorBoxSize, ChartOwner.CursorBoxSize);
                    Gdi32.Ellipse(hDC, locationBox.Left, locationBox.Top, locationBox.Left + ChartOwner.CursorBoxSize, locationBox.Top + ChartOwner.CursorBoxSize);
                }
                #endregion
                #region horizontal line
                else if (ChartOwner.CursorMode == CursorStyle.HORIZONTAL)
                {
                    // horizontal line
                    Gdi32.MoveToEx(hDC, 0, drawPoint.Y, lpPoint);
                    Gdi32.LineTo(hDC, chartPanel.Width, drawPoint.Y);
                }
                #endregion
                #region vertical line
                else if (ChartOwner.CursorMode == CursorStyle.VERTICAL)
                {
                    // vertical line
                    Gdi32.MoveToEx(hDC, drawPoint.X, 0, lpPoint);
                    Gdi32.LineTo(hDC, drawPoint.X, chartPanel.Height);
                }
                #endregion
                #region clean up GDI, reset context
                Gdi32.SelectObject(hDC, oldPen);
                Gdi32.SelectObject(hDC, oldBrush);
                Gdi32.DeleteObject(newPen);
                // newBrush is a stock object — must not be passed to DeleteObject
                drawGraphics.ReleaseHdc();
                #endregion
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scaledPoint"></param>
        /// <param name="runIdx"></param>
        public void DrawCursorAtScaledPoint(PointF scaledPoint, int runIdx, Color cursorColor = default)
        {
            if(StartMouseMove.Count < (runIdx + 1))
            {
                StartMouseMove.Add(false);
                ChartStartCursorPos.Add(new Point());
                ChartLastCursorPos.Add(new Point());
                StartMouseDrag.Add(false);
            }
            Axis primaryX = ChartOwner.X_Axes.ElementAtOrDefault(0).Value;
            Point screenPoint = new Point(0,0);
            if (primaryX == null)
            {
                return;
            }
            // scale to screen conversion
            int border = ChartOwner.ChartBorder;
            Axis primaryY = ChartOwner.Y_Axes.ElementAtOrDefault(0).Value;
            screenPoint.X = border + (int)((scaledPoint.X - primaryX.AxisDisplayRange[0]) * displayScale[0]);
            screenPoint.Y = (ChartOwner.ChartHeight - border) + (int)((scaledPoint.Y - (primaryY?.AxisDisplayRange[0] ?? 0)) * displayScale[1]);
            if (StartMouseMove[runIdx])
            {
                DrawCursorAtScreenPoint(ChartLastCursorPos[runIdx], cursorColor);
                StartMouseMove[runIdx] = false;
            }
            // Only draw if inside chart area (within border)
            if ((screenPoint.X >= border) &&
                (screenPoint.Y >= border) &&
                (screenPoint.X <= chartPanel.Width - border) &&
                (screenPoint.Y <= chartPanel.Height - border))
            {
                DrawCursorAtScreenPoint(screenPoint, cursorColor);
                StartMouseMove[runIdx] = true;
                ChartLastCursorPos[runIdx] = screenPoint;
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
            PointF scaledPoint = new PointF(sourcePt.X, sourcePt.Y);
            scaledPoint.X = (scaledPoint.X / scaleX) - offsetX;
            scaledPoint.Y = -1.0F * ((scaledPoint.Y - (float)bounds.Height) / scaleY) - offsetY;
            return scaledPoint;
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
        public void OnChartMouseMove(object sender, EventArgs e)
        {
            if (ChartOwner.ChartViewType != Chart.ChartType.Stripchart)
            {
                return; 
            }
            float timeAtCursor = 0.0F;
            float distanceAtCursor = 0.0F;
            //SortedList<string, SortedList<string, float>> valuesAtCursor = new SortedList<string, SortedList<string, float>>();
            try
            {
                #region no data, return
                if (ChartOwner == null || ChartOwner.X_Axes == null || ChartOwner.X_Axes.Count == 0)
                {
                    return;
                }
                #endregion

                #region cursor track in local view
                // Mouse position relative to chartPanel
                Point mousePt = chartPanel.PointToClient(Cursor.Position);

                // erase previous XOR cursor
                if (StartMouseMove[0])
                {
                    DrawCursorAtScreenPoint(ChartLastCursorPos[0]);
                    StartMouseMove[0] = false;
                }
                // draw XOR cursor at current position
                if ((mousePt.X >= 0) && (mousePt.Y >= 0) &&
                    (mousePt.X <= chartPanel.Width) && (mousePt.Y <= chartPanel.Height))
                {
                    DrawCursorAtScreenPoint(mousePt);
                    StartMouseMove[0] = true;
                    ChartLastCursorPos[0] = mousePt;
                    _hasCursorPos = true;
                    _savedCursorPos = mousePt;
                }
                #endregion

                // time works, distance isn't right yet
                #region send points to draw in views
                // Map mouse X -> data-space X using the primary X axis transform
                Axis primaryXAxis = ChartOwner.X_Axes.ElementAtOrDefault(0).Value;

                // panel X relative to chart area (subtract border)
                float panelX = mousePt.X - ChartOwner.ChartBorder;
                float dataX = 0.0F;
                //ChartControlMouseTrackEventArgs outArgs = new ChartControlMouseTrackEventArgs();
                outArgs.XAxisValues.Clear();
                outArgs.YAxisValues.Clear();

                // For each dataset compute dataset-specific time corresponding to dataX
                for (int dataSetIdx = 0; dataSetIdx < ChartOwner.dataSets.Count; dataSetIdx++)
                {
                    // Compute data-space X (axis units)
                    dataX = (panelX / displayScale[0]) + primaryXAxis.AxisDisplayRange[0]/* + primaryXAxis.AxisOffset*/;
                    var curDataSet = ChartOwner.dataSets[dataSetIdx];
                    // x axis is time, no need to get distance
                    if (ChartOwner.XChannelName == "Time")
                    {
                        timeAtCursor = dataX - curDataSet.TimeOffset;
                    }
                    // have distance, need to get time from the xDistance channel of current data set
                    else if (ChartOwner.XChannelName == "Distance")
                    {
                        distanceAtCursor = dataX - curDataSet.DistanceOffset;
                        //timeAtCursor = curDataSet.channels["xDistance"].dataPoints.FirstOrDefault(i => i.Key >= distanceAtCursor).Value;
                        FindNearestValueToKey(curDataSet.channels["xDistance"].dataPoints, distanceAtCursor, out timeAtCursor);
                    }
                    // strip charts with x axis other than time or distance not supported yet
                    //else
                    //{
                    //    if (curDataSet.channels != null && curDataSet.channels.TryGetValue(ChartOwner.XChannelName, out ChartChannel axisChan) && axisChan.DataPoints != null && axisChan.DataPoints.Count > 0)
                    //    {
                    //        if (FindNearestKeyToValue(axisChan.DataPoints, dataX, out float foundKey))
                    //        {
                    //            timeAtCursor = foundKey;
                    //        }
                    //        if (curDataSet.channels.TryGetValue("xDistance", out ChartChannel xDistChan2) &&
                    //            //FindNearestKeyToValue(xDistChan2.DataPoints, timeAtCursor, out foundKey))
                    //            FindNearestKeyToValue(curDataSet.channels["xTime"].dataPoints, timeAtCursor, out foundKey))
                    //        {
                    //            distanceAtCursor = xDistChan2.DataPoints[foundKey];
                    //        }
                    //    }
                    //}
                    if (!outArgs.XAxisValues.ContainsKey(curDataSet.DataSetName))
                    {
                        outArgs.XAxisValues.Add(curDataSet.DataSetName, timeAtCursor);
                    }
                    //}

                    // For each displayed channel, find Y value at the computed dataset time using TryFindNearestValue
                    foreach (KeyValuePair<string, Axis> axisKvp in ChartOwner.Y_Axes)
                    {
                        Axis yAxis = axisKvp.Value;
                        if (yAxis == null)
                        {
                            continue;
                        }
                        foreach (ChartChannel chan in yAxis.AssociatedChannels)
                        {
                            if(chan.DataSetName != curDataSet.DataSetName)
                            {
                                continue;
                            }
                            // skip hidden channels unless source is GPS or IMU
                            // (for track map and traction circle updates)
                            // will need to change this if I allow anything on X axis in other charts
                            if ((!chan.ShowChannel) && ((chan.ChannelSource != "IMU") &&
                                                        (chan.ChannelSource != "GPS") &&
                                                        (chan.ChannelSource != "Calculated")))
                            {
                                continue;
                            }
                            //float foundY = chan.dataPoints.FirstOrDefault(i => i.Key >= timeAtCursor).Value;
                            float foundY;
                            FindNearestValueToKey(chan.dataPoints, timeAtCursor, out foundY);
                            if (!outArgs.YAxisValues.ContainsKey(chan.DataSetName))
                            {
                                outArgs.YAxisValues.Add(chan.DataSetName, new SortedList<string, float>());
                            }
                            if (!outArgs.YAxisValues[chan.DataSetName].ContainsKey(chan.ChannelName))
                            {
                                outArgs.YAxisValues[chan.DataSetName].Add(chan.ChannelName, foundY);
                            }
                        }
                        if (!outArgs.YAxisValues[curDataSet.DataSetName].ContainsKey("Time"))
                        {
                            outArgs.YAxisValues[curDataSet.DataSetName].Add("Time", outArgs.XAxisValues[curDataSet.DataSetName]);
                        }
                    }
                }
                // update overlay panel (only the small panel repaints, not the main chart)
                _cursorDataX = dataX;
                _cursorValues = outArgs.YAxisValues;
                UpdateOverlayPanel(mousePt);
                // Raise event for listeners (other charts) with mapped X and channel values
                ChartMouseTrackEvent?.Invoke(this, outArgs);
                #endregion
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("OnChartMouseMove error: " + ex.Message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnChartXAxisChange(object sender, ChartControlXAxisChangeEventArgs e)
        {
            ChartOwner.XChannelName = e.XAxisName;
            chartPanel.Invalidate();
        }
        public void OnClearGraphicsPath(object sender, EventArgs e)
        {
            foreach (KeyValuePair<string, Axis> yAxis in ChartOwner.Y_Axes)
            {
                foreach (ChartChannel curChanInfo in yAxis.Value.AssociatedChannels)
                {
                    curChanInfo.ChannelPath?.Dispose();
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
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChartPanel_MouseLeave(object sender, EventArgs e)
        {
            _hasCursorPos = false;
            _cursorValues = null;
            _overlayPanel.Visible = false;
            if (StartMouseMove.Count > 0 && StartMouseMove[0])
            {
                DrawCursorAtScreenPoint(ChartLastCursorPos[0]);
                StartMouseMove[0] = false;
            }
        }
        bool firstUpdate = true;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cursorPt"></param>
        private void UpdateOverlayPanel(Point cursorPt)
        {
            if (ChartOwner == null) return;
            if (!ChartOwner.ShowOverlay)
            {
                _overlayPanel.Visible = false;
                return;
            }
            int border = ChartOwner.ChartBorder;
            float lineHeight = Font.Height + 3;
            int lineCount = 1; // x-axis header
            if (_cursorValues != null)
                foreach (var ds in _cursorValues)
                    foreach (var ch in ds.Value)
                        foreach (var axis in ChartOwner.Y_Axes.Values)
                            foreach (var chan in axis.AssociatedChannels)
                                if (chan.DataSetName == ds.Key && chan.ChannelName == ch.Key && chan.ShowChannel)
                                    lineCount++;
            int panelW = 195;
            int panelH = (int)(lineHeight * lineCount) + 6;
            // place on right side when cursor is in left half, left side when in right half
            int x = cursorPt.X < chartPanel.Width / 2
                ? chartPanel.Width - border - 4 - panelW
                : border + 4;
            bool overlayPanelMoved = false;
            if (_overlayPanel.Location.X != x)
            {
                overlayPanelMoved = true;
            }
            _overlayPanel.Location = new Point(x, border + 4);
            _overlayPanel.Size = new Size(panelW, panelH);
            _overlayPanel.Visible = true;
            _overlayPanel.Invalidate();
            // removing this ghosts the cursor
            // cursor ghosts when chartPanel is set to double buffer
            if (overlayPanelMoved)
            {
                //DrawCursorAtScreenPoint(ChartLastCursorPos[0]);
                chartPanel.Invalidate();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OverlayPanel_Paint(object sender, PaintEventArgs e)
        {
            if (_cursorValues == null || ChartOwner == null)
                return;

            Graphics g = e.Graphics;
            float lineHeight = Font.Height + 3;
            float x = 3f;
            float y = 3f;

            string xLabel = float.IsNaN(_cursorDataX) ? "" :
                ChartOwner.XChannelName == "Distance" ? $"Dist: {_cursorDataX:F2} ft" :
                                                         $"Time: {_cursorDataX:F3} s";

            //using (Brush w = new SolidBrush(Color.White))
            overlayBrush.Color = Color.White;
            g.DrawString(xLabel, Font, overlayBrush, x, y);
            y += lineHeight;

            foreach (var ds in _cursorValues)
            {
                foreach (var ch in ds.Value)
                {
                    Color channelColor = Color.White;
                    bool visible = false;
                    foreach (var axis in ChartOwner.Y_Axes.Values)
                        foreach (var chan in axis.AssociatedChannels)
                            if (chan.DataSetName == ds.Key && chan.ChannelName == ch.Key && chan.ShowChannel)
                            {
                                channelColor = chan.ChannelColor;
                                visible = true;
                            }
                    if (!visible)
                        continue;
                    string valueStr = float.IsNaN(ch.Value) ? "---" : ch.Value.ToString("G4");
                    string label = _cursorValues.Count > 1
                        ? $"{ch.Key} ({ds.Key}): {valueStr}"
                        : $"{ch.Key}: {valueStr}";
                    //using (Brush overlaybrush = new SolidBrush(channelColor))
                        overlayBrush.Color = channelColor;
                        g.DrawString(label, Font, overlayBrush, x, y);
                    y += lineHeight;
                }
            }
        }
        /// <summary>
        /// binary-search nearest-key lookup for SortedList<float,float>
        /// </summary>
        /// <param name="sorted"></param>
        /// <param name="key"></param>
        /// <param name="foundValue"></param>
        /// <returns></returns>
        public static bool FindNearestValueToKey(SortedList<float, float> sorted, float key, out float foundValue)
        {
            foundValue = 0f;
            if (sorted == null || sorted.Count == 0)
            {
                return false;
            }
            IList<float> keys = sorted.Keys;
            int lo = 0;
            int hi = keys.Count - 1;

            while (lo <= hi)
            {
                int mid = (lo + hi) >> 1;
                float k = keys[mid];
                if (k == key)
                {
                    foundValue = sorted[k];
                    return true;
                }
                if (k < key) lo = mid + 1;
                else hi = mid - 1;
            }

            if (lo == 0)
            {
                foundValue = sorted[keys[0]];
                return true;
            }
            if (lo >= keys.Count)
            {
                foundValue = sorted[keys[keys.Count - 1]];
                return true;
            }

            float prevKey = keys[lo - 1];
            float nextKey = keys[lo];
            foundValue = (key - prevKey) <= (nextKey - key) ? sorted[prevKey] : sorted[nextKey];
            return true;
        }
        /// <summary>
        /// Helper: nearest-key lookup by searching values (returns key whose value is nearest to targetValue) 
        /// </summary>
        /// <param name="sorted"></param>
        /// <param name="targetValue"></param>
        /// <param name="foundKey"></param>
        /// <returns></returns>
        public bool FindNearestKeyToValue(SortedList<float, float> sorted, float targetValue, out float foundKey)
        {
            foundKey = 0f;
            if (sorted == null || sorted.Count == 0)
            {
                return false;
            }
            float bestDiff = float.MaxValue;
            bool found = false;

            foreach (var kv in sorted)
            {
                float diff = Math.Abs(kv.Value - targetValue);
                if (diff < bestDiff)
                {
                    bestDiff = diff;
                    foundKey = kv.Key;
                    found = true;
                }
            }
            return found;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ChartControlMouseTrackEventArgs : EventArgs
    {
        Dictionary<string, float> xAxisValues = new Dictionary<string, float>();
        public Dictionary<string, float> XAxisValues
        {
            get { return xAxisValues; }
            set { xAxisValues = value; }
        }
        // list of channel names and value for each data set
        SortedList<string, SortedList<string, float>> yAxisValues = new SortedList<string, SortedList<string, float>>();
        public SortedList<string, SortedList<string, float>> YAxisValues
        {
            get { return yAxisValues; }
            set { yAxisValues = value; }
        }

    }
    /// <summary>
    /// 
    /// </summary>
    public class ChartControlXAxisChangeEventArgs : EventArgs
    {
        string xAxisName = "";
        public string XAxisName
        {
            get { return xAxisName; }
            set { xAxisName = value; }
        }
    }
    /// <summary>
    /// 
    /// </summary>
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
    /// <summary>
    /// 
    /// </summary>
    public class DataChannel
    {
        String channelName;
        String channelDescription;
        String channelSource;
        float channelScale;
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
}



