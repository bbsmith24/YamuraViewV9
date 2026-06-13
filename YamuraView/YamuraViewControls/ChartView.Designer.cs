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
            vScrollBar = new VScrollBar();
            hScrollBar = new HScrollBar();
            SuspendLayout();
            // 
            // chartPanel
            // 
            chartPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            chartPanel.BackColor = SystemColors.ControlDarkDark;
            chartPanel.Location = new Point(20, 3);
            chartPanel.Name = "chartPanel";
            chartPanel.Size = new Size(130, 127);
            chartPanel.TabIndex = 0;
            chartPanel.Paint += chartPanel_Paint;
            chartPanel.Resize += chartPanel_Resize;
            // 
            // vScrollBar
            // 
            vScrollBar.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            vScrollBar.Location = new Point(0, 3);
            vScrollBar.Name = "vScrollBar";
            vScrollBar.Size = new Size(17, 127);
            vScrollBar.TabIndex = 1;
            // 
            // hScrollBar
            // 
            hScrollBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            hScrollBar.Location = new Point(20, 131);
            hScrollBar.Name = "hScrollBar";
            hScrollBar.Size = new Size(130, 17);
            hScrollBar.TabIndex = 2;
            // 
            // ChartView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(hScrollBar);
            Controls.Add(vScrollBar);
            Controls.Add(chartPanel);
            Name = "ChartView";
            ResumeLayout(false);
        }

        #endregion

        private Panel chartPanel;
        private VScrollBar vScrollBar;
        private HScrollBar hScrollBar;
    }
}
