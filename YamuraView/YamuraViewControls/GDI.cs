using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GDI
{
    #region Enumerations
    public enum RasterOps
    {
        R2_BLACK = 1,
        R2_NOTMERGEPEN,
        R2_MASKNOTPEN,
        R2_NOTCOPYPEN,
        R2_MASKPENNOT,
        R2_NOT,
        R2_XORPEN,
        R2_NOTMASKPEN,
        R2_MASKPEN,
        R2_NOTXORPEN,
        R2_NOP,
        R2_MERGENOTPEN,
        R2_COPYPEN,
        R2_MERGEPENNOT,
        R2_MERGEPEN,
        R2_WHITE,
        R2_LAST
    }
    public enum BrushStyles
    {
        BS_SOLID = 0,
        BS_NULL = 1,
        BS_HATCHED = 2,
        BS_PATTERN = 3,
        BS_INDEXED = 4,
        BS_DIBPATTERN = 5,
        BS_DIBPATTERNPT = 6,
        BS_PATTERN8X8 = 7,
        BS_MONOPATTERN = 9
    }
    public enum PenStyles
    {
        PS_SOLID = 0,
        PS_DASH = 1,
        PS_DOT = 2,
        PS_DASHDOT = 3,
        PS_DASHDOTDOT = 4
    }
    public enum StockObjects
    {
        WHITE_BRUSH = 0,
        LTGRAY_BRUSH = 1,
        GRAY_BRUSH = 2,
        DKGRAY_BRUSH = 3,
        BLACK_BRUSH = 4,
        NULL_BRUSH = 5,
        HOLLOW_BRUSH = 5,
        WHITE_PEN = 6,
        BLACK_PEN = 7,
        NULL_PEN = 8,
        OEM_FIXED_FONT = 10,
        ANSI_FIXED_FONT = 11,
        ANSI_VAR_FONT = 12,
        SYSTEM_FONT = 13,
        DEVICE_DEFAULT_FONT = 14,
        DEFAULT_PALETTE = 15,
        SYSTEM_FIXED_FONT = 16,
    }
    #endregion enumerations

    public static class Gdi32
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern IntPtr CreatePen(int fnPenStyle, int nWidth, uint crColor);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern IntPtr GetStockObject(int fnObject);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern int SetROP2(IntPtr hdc, int fnDrawMode);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool MoveToEx(IntPtr hdc, int x, int y, IntPtr lpPoint);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool LineTo(IntPtr hdc, int nXEnd, int nYEnd);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool Rectangle(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool Ellipse(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
    }

    /// <summary>
    ///
    /// </summary>
    public class GDI32
    {
        #region InteropCalls
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern bool Ellipse(IntPtr hdc, int x1, int y1, int x2, int y2);
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern bool Rectangle(IntPtr hdc, int X1, int Y1, int X2, int Y2);
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern IntPtr MoveToEx(IntPtr hdc, int x, int y, IntPtr lpPoint);
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern bool LineTo(IntPtr hdc, int x, int y);
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern IntPtr CreatePen(PenStyles enPenStyle, int nWidth, int crColor);
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern IntPtr CreateSolidBrush(BrushStyles enBrushStyle, int crColor);
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern IntPtr GetStockObject(StockObjects brStyle);
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern int SetROP2(IntPtr hdc, int enDrawMode);
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern bool Polyline(IntPtr hdc, Point[] lppt, int cPoints);
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern bool BitBlt(IntPtr targetHDC, int targetX, int targetY, int targetWidth, int targetHeight,
                                            IntPtr sourceHDC, int sourceX, int sourceY, int drawOp);
        #endregion

        #region Variables
        protected Color borderColor;
        protected Color fillColor;
        protected int lineWidth;
        protected IntPtr hdc, oldBrush, oldPen, gdiPen, gdiBrush;
        protected BrushStyles brushStyle;
        protected StockObjects stockObject;
        protected PenStyles penStyle;
        #endregion

        #region Methods
        /// <summary>
        /// Constructor for GDI32 Graphics Library
        /// </summary>
        public GDI32()
        {   // Set up for XOR drawing to begin with
            borderColor = Color.Transparent;
            fillColor = Color.White;
            lineWidth = 1;
            brushStyle = BrushStyles.BS_NULL;
            stockObject = StockObjects.NULL_BRUSH;
            penStyle = PenStyles.PS_SOLID;
        }
        /// <summary>
        /// The current BrushColor
        /// </summary>
        public Color BrushColor
        {
            get { return fillColor; }
            set { fillColor = value; }
        }
        /// <summary>
        /// The current BrushStyle.  Set to BS_NULL for no brush.
        /// </summary>
        public BrushStyles BrushStyle
        {
            get { return brushStyle; }
            set { brushStyle = value; }
        }
        /// <summary>
        /// The current PenColor.  Set to Color.Transparent for a XOR line.
        /// </summary>
        public Color PenColor
        {
            get { return borderColor; }
            set { borderColor = value; }
        }
        /// <summary>
        /// The current PenStyle.
        /// </summary>
        public PenStyles PenStyle
        {
            get { return penStyle; }
            set { penStyle = value; }
        }
        /// <summary>
        /// The current PenWidth.
        /// </summary>
        public int PenWidth
        {
            get { return lineWidth; }
            set { lineWidth = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromColor"></param>
        /// <returns></returns>
        public int GetRGBFromColor(Color fromColor)
        {
            int rColor = fromColor.ToArgb() & 0xFFFFFF;
            return rColor;
        }
        /// <summary>
        /// Draws a line with the pen that has been set by the user.  Uses gdi32->MoveToEx and gdi32->LineTo
        /// </summary>
        /// <param name="g">Graphics object.  You can use CreateGraphics().</param>
        /// <param name="p1">Initial point of line.</param>
        /// <param name="p2">Termination point of line.</param>
        public void DrawLine(Graphics g, Point p1, Point p2)
        {
            InitPenAndBrush(g);
            MoveToEx(hdc, p1.X, p1.Y, (IntPtr)null);
            LineTo(hdc, p2.X, p2.Y);
            Dispose(g);
        }
        public void EraseLine(Graphics g, Point p1, Point p2)
        {
            InitErasePenAndBrush(g);
            MoveToEx(hdc, p1.X, p1.Y, (IntPtr)null);
            LineTo(hdc, p2.X, p2.Y);
            Dispose(g);
        }
        /// <summary>
        /// Draws a line with the pen that has been set by the user.  Uses gdi32->MoveToEx and gdi32->LineTo
        /// </summary>
        /// <param name="g">Graphics object.  You can use CreateGraphics().</param>
        /// <param name="p1">Initial point of line.</param>
        /// <param name="p2">Termination point of line.</param>
        public void DrawPolyLine(Graphics g, params Point[] points)
        {
            if (points == null)
            {
                Dispose(g);
                return;
            }
            InitPenAndBrush(g);
            Polyline(hdc, points, (int)points.Count());
            Dispose(g);
        }
        public void ErasePolyLine(Graphics g, params Point[] points)
        {
            if (points == null)
            {
                Dispose(g);
                return;
            }
            InitErasePenAndBrush(g);
            Polyline(hdc, points, (int)points.Count());
            Dispose(g);
        }
        /// <summary>
        /// Draws a rectangle with the pen and brush that have been set by the user.  Uses gdi32->Rectangle
        /// </summary>
        /// <param name="g">Graphics object.  You can use CreateGraphics().</param>
        /// <param name="p1">First corner of rectangle.</param>
        /// <param name="p2">Second corner of rectangle.</param>
        public void DrawRectangle(Graphics g, Point p1, Point p2)
        {
            InitPenAndBrush(g);
            Rectangle(hdc, p1.X, p1.Y, p2.X, p2.Y);
            Dispose(g);
        }
        /// <summary>
        /// Draws a rectangle with the pen and brush that have been set by the user.  Uses gdi32->Rectangle
        /// </summary>
        /// <param name="g">Graphics object.  You can use CreateGraphics().</param>
        /// <param name="p1">First corner of rectangle.</param>
        /// <param name="p2">Second corner of rectangle.</param>
        public void DrawRectangle(Graphics g, Rectangle rect)
        {
            InitPenAndBrush(g);
            Rectangle(hdc, rect.Left, rect.Top, rect.Right, rect.Bottom);
            Dispose(g);
        }
        /// <summary>
        /// Draws a list of rectangles with the pen and brush that have been set by the user.  Uses gdi32->Rectangle
        /// </summary>
        /// <param name="g">Graphics object.  You can use CreateGraphics().</param>
        /// <param name="p1">First corner of rectangle.</param>
        /// <param name="p2">Second corner of rectangle.</param>
        public void DrawRectangleList(Graphics g, List<Rectangle> rectList)
        {
            InitPenAndBrush(g);
            for (int rectIdx = 0; rectIdx < rectList.Count; rectIdx++)
            {
                Rectangle(hdc, rectList[rectIdx].Left, rectList[rectIdx].Top, rectList[rectIdx].Right, rectList[rectIdx].Bottom);
            }
            Dispose(g);
        }
        /// <summary>
        /// Draws a rectangle with the pen and brush that have been set by the user.  Uses gdi32->Rectangle
        /// </summary>
        /// <param name="g">Graphics object.  You can use CreateGraphics().</param>
        /// <param name="p1">First corner of rectangle.</param>
        /// <param name="p2">Second corner of rectangle.</param>
        public void EraseRectangle(Graphics g, Point p1, Point p2)
        {
            InitErasePenAndBrush(g);
            Rectangle(hdc, p1.X, p1.Y, p2.X, p2.Y);
            Dispose(g);
        }
        /// <summary>
        /// Draws a rectangle with the pen and brush that have been set by the user.  Uses gdi32->Rectangle
        /// </summary>
        /// <param name="g">Graphics object.  You can use CreateGraphics().</param>
        /// <param name="p1">First corner of rectangle.</param>
        /// <param name="p2">Second corner of rectangle.</param>
        public void EraseRectangle(Graphics g, Rectangle rect)
        {
            InitErasePenAndBrush(g);
            Rectangle(hdc, rect.Left, rect.Top, rect.Right, rect.Bottom);
            Dispose(g);
        }
        /// <summary>
        /// Draws a rectangle with the pen and brush that have been set by the user.  Uses gdi32->Rectangle
        /// </summary>
        /// <param name="g">Graphics object.  You can use CreateGraphics().</param>
        /// <param name="p1">First corner of rectangle.</param>
        /// <param name="p2">Second corner of rectangle.</param>
        public void EraseRectangleList(Graphics g, List<Rectangle> rectList)
        {
            InitErasePenAndBrush(g);
            for (int rectIdx = 0; rectIdx < rectList.Count; rectIdx++)
            {
                Rectangle(hdc, rectList[rectIdx].Left, rectList[rectIdx].Top, rectList[rectIdx].Right, rectList[rectIdx].Bottom);
            }
            Dispose(g);
        }
        /// <summary>
        /// Draws an ellipse with the pen and brush that have been set by the user.  Uses gdi32->Ellipse
        /// </summary>
        /// <param name="g">Graphics object.  You can use CreateGraphics().</param>
        /// <param name="p1">First corner of ellipse (if you imagine its size as a rectangle).</param>
        /// <param name="p2">Second corner of ellipse (if you imagine its size as a rectangle).</param>
        public void DrawEllipse(Graphics g, Point p1, Point p2)
        {
            InitPenAndBrush(g);
            Ellipse(hdc, p1.X, p1.Y, p2.X, p2.Y);
            Dispose(g);
        }
        /// <summary>
        /// Initializes the pen and brush objects.  Stores the old pen and brush so they can be recovered later.
        /// </summary>
        public void InitPenAndBrush(Graphics g)
        {
            hdc = g.GetHdc();
            gdiPen = CreatePen(penStyle, lineWidth, GetRGBFromColor(PenColor));
            //gdiBrush = CreateSolidBrush(brushStyle, GetRGBFromColor(fillColor));
            gdiBrush = GetStockObject(stockObject);
            //if (PenColor == Color.Transparent)
            //{
            SetROP2(hdc, (int)RasterOps.R2_XORPEN);
            //}
            oldPen = SelectObject(hdc, gdiPen);
            oldBrush = SelectObject(hdc, gdiBrush);
        }
        /// <summary>
        /// Initializes the pen and brush objects.  Stores the old pen and brush so they can be recovered later.
        /// </summary>
        public void InitErasePenAndBrush(Graphics g)
        {
            hdc = g.GetHdc();
            gdiPen = CreatePen(penStyle, lineWidth, GetRGBFromColor(PenColor));
            //gdiBrush = CreateSolidBrush(brushStyle, GetRGBFromColor(fillColor));
            gdiBrush = GetStockObject(stockObject);
            //if (PenColor == Color.Transparent)
            //{
            SetROP2(hdc, (int)RasterOps.R2_XORPEN);
            //}
            oldPen = SelectObject(hdc, gdiPen);
            oldBrush = SelectObject(hdc, gdiBrush);
        }
        public bool Bit_Blt(Graphics gTarget, int targetX, int targetY, int targetWidth, int targetHeight,
                            Graphics gSource, int sourceX, int sourceY, int drawOp)
        {
            IntPtr targetHDC = gTarget.GetHdc();
            IntPtr sourceHDC = gSource.GetHdc();
            bool rVal = BitBlt(targetHDC, targetX, targetY, targetWidth, targetHeight, sourceHDC, sourceX, sourceY, drawOp);
            gTarget.ReleaseHdc(targetHDC);
            gSource.ReleaseHdc(sourceHDC);
            return rVal;
        }
        //public bool Bit_Blt(Graphics gTarget, int targetX, int targetY, int targetWidth, int targetHeight,
        //                    Bitmap bmpSource, int sourceX, int sourceY, int drawOp)
        //{


        //    IntPtr targetHDC = gTarget.GetHdc();
        //    IntPtr sourceHDC = bmpSource.GetHdc();
        //    bool rVal = BitBlt(targetHDC, targetX, targetY, targetWidth, targetHeight, sourceHDC, sourceX, sourceY, drawOp);
        //    gTarget.ReleaseHdc(targetHDC);
        //    gSource.ReleaseHdc(sourceHDC);
        //    return rVal;
        //}
        /// <summary>
        /// Reloads the old pen and brush.
        /// Deletes the pen that was created by InitPenAndBrush(g).
        /// Releases the handle to the device context and then disposes of the Graphics object.
        /// </summary>
        protected void Dispose(Graphics g)
        {
            SelectObject(hdc, oldBrush);
            SelectObject(hdc, oldPen);
            DeleteObject(gdiPen);
            DeleteObject(gdiBrush);
            g.ReleaseHdc(hdc);
            g.Dispose();
        }
        #endregion
    }

}

