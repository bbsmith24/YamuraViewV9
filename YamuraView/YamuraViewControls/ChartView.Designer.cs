namespace YamuraViewControls
{
    partial class ChartView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            chartPanel = new Panel();
            hScrollBar = new HScrollBar();
            SuspendLayout();
            //
            // chartPanel
            //
            chartPanel.BackColor = Color.Black;
            chartPanel.Location = new Point(0, 0);
            chartPanel.Name = "chartPanel";
            chartPanel.Size = new Size(150, 128);
            chartPanel.TabIndex = 0;
            chartPanel.Paint += chartPanel_Paint;
            chartPanel.MouseDoubleClick += chartPanel_MouseDoubleClick;
            chartPanel.MouseDown += chartPanel_MouseDown;
            chartPanel.MouseMove += OnChartMouseMove;
            chartPanel.MouseUp += chartPanel_MouseUp;
            chartPanel.Resize += chartPanel_Resize;
            //
            // hScrollBar
            //
            hScrollBar.Location = new Point(0, 128);
            hScrollBar.Name = "hScrollBar";
            hScrollBar.Size = new Size(150, 17);
            hScrollBar.TabIndex = 2;
            //
            // ChartView
            //
            AutoScaleMode = AutoScaleMode.None;
            Controls.Add(hScrollBar);
            Controls.Add(chartPanel);
            Name = "ChartView";
            this.Resize += ChartView_Resize;
            ResumeLayout(false);
        }

        #endregion

        private Panel chartPanel;
        private HScrollBar hScrollBar;
    }
}
