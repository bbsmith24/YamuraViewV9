namespace YamuraView
{
    partial class YamuraView
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            splitContainer1 = new SplitContainer();
            StripChart = new YamuraViewControls.Chart();
            splitContainer2 = new SplitContainer();
            TrackMap = new YamuraViewControls.Chart();
            TractionCircle = new YamuraViewControls.Chart();
            openLogFile = new OpenFileDialog();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            addRunsMenuItem = new ToolStripMenuItem();
            clearRunsToolStripMenuItem = new ToolStripMenuItem();
            setAutoloadFolderToolStripMenuItem = new ToolStripMenuItem();
            timeAlignSetupToolStripMenuItem = new ToolStripMenuItem();
            distanceAlignSetupToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 24);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(StripChart);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Size = new Size(800, 426);
            splitContainer1.SplitterDistance = 266;
            splitContainer1.TabIndex = 0;
            // 
            // StripChart
            // 
            StripChart.ChartName = "StripChart";
            StripChart.Dock = DockStyle.Fill;
            StripChart.Location = new Point(0, 0);
            StripChart.Name = "StripChart";
            StripChart.Size = new Size(266, 426);
            StripChart.TabIndex = 0;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(TrackMap);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(TractionCircle);
            splitContainer2.Size = new Size(530, 426);
            splitContainer2.SplitterDistance = 166;
            splitContainer2.TabIndex = 0;
            // 
            // TrackMap
            // 
            TrackMap.ChartName = "Track Map";
            TrackMap.Dock = DockStyle.Fill;
            TrackMap.Location = new Point(0, 0);
            TrackMap.Name = "TrackMap";
            TrackMap.Size = new Size(530, 166);
            TrackMap.TabIndex = 0;
            // 
            // TractionCircle
            // 
            TractionCircle.ChartName = "Traction Circle";
            TractionCircle.Dock = DockStyle.Fill;
            TractionCircle.Location = new Point(0, 0);
            TractionCircle.Name = "TractionCircle";
            TractionCircle.Size = new Size(530, 256);
            TractionCircle.TabIndex = 0;
            // 
            // openLogFile
            // 
            openLogFile.FileName = "openFileDialog1";
            openLogFile.Filter = "YamuraLog YL5 files|*.yl5|YamuraLog YLG files|*.ylg|Text files|*.txt";
            openLogFile.Multiselect = true;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { addRunsMenuItem, clearRunsToolStripMenuItem, setAutoloadFolderToolStripMenuItem, timeAlignSetupToolStripMenuItem, distanceAlignSetupToolStripMenuItem, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // addRunsMenuItem
            // 
            addRunsMenuItem.Name = "addRunsMenuItem";
            addRunsMenuItem.Size = new Size(182, 22);
            addRunsMenuItem.Text = "Add Runs";
            addRunsMenuItem.Click += addRunsMenuItem_Click;
            // 
            // clearRunsToolStripMenuItem
            // 
            clearRunsToolStripMenuItem.Name = "clearRunsToolStripMenuItem";
            clearRunsToolStripMenuItem.Size = new Size(182, 22);
            clearRunsToolStripMenuItem.Text = "Clear Runs";
            // 
            // setAutoloadFolderToolStripMenuItem
            // 
            setAutoloadFolderToolStripMenuItem.Name = "setAutoloadFolderToolStripMenuItem";
            setAutoloadFolderToolStripMenuItem.Size = new Size(182, 22);
            setAutoloadFolderToolStripMenuItem.Text = "Set Autoload folder";
            // 
            // timeAlignSetupToolStripMenuItem
            // 
            timeAlignSetupToolStripMenuItem.Name = "timeAlignSetupToolStripMenuItem";
            timeAlignSetupToolStripMenuItem.Size = new Size(182, 22);
            timeAlignSetupToolStripMenuItem.Text = "Time Align setup";
            timeAlignSetupToolStripMenuItem.Click += timeAlignSetupToolStripMenuItem_Click;
            // 
            // distanceAlignSetupToolStripMenuItem
            // 
            distanceAlignSetupToolStripMenuItem.Name = "distanceAlignSetupToolStripMenuItem";
            distanceAlignSetupToolStripMenuItem.Size = new Size(182, 22);
            distanceAlignSetupToolStripMenuItem.Text = "Distance Align setup";
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(182, 22);
            exitToolStripMenuItem.Text = "Exit";
            // 
            // YamuraView
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(splitContainer1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "YamuraView";
            Text = "Form1";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private OpenFileDialog openLogFile;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem addRunsMenuItem;
        private ToolStripMenuItem clearRunsToolStripMenuItem;
        private ToolStripMenuItem setAutoloadFolderToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private YamuraViewControls.Chart StripChart;
        private YamuraViewControls.Chart TrackMap;
        private YamuraViewControls.Chart TractionCircle;
        private ToolStripMenuItem timeAlignSetupToolStripMenuItem;
        private ToolStripMenuItem distanceAlignSetupToolStripMenuItem;
    }
}
