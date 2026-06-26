namespace YamuraViewControls
{
    partial class Chart : UserControl
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
            ChartTabs = new TabControl();
            tabPage1 = new TabPage();
            chartView1 = new ChartView();
            tabPage2 = new TabPage();
            chartProperties1 = new ChartProperties();
            ChartTabs.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // ChartTabs
            // 
            ChartTabs.Controls.Add(tabPage1);
            ChartTabs.Controls.Add(tabPage2);
            ChartTabs.Dock = DockStyle.Fill;
            ChartTabs.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ChartTabs.Location = new Point(0, 0);
            ChartTabs.Name = "ChartTabs";
            ChartTabs.SelectedIndex = 0;
            ChartTabs.Size = new Size(851, 531);
            ChartTabs.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.BackColor = Color.Transparent;
            tabPage1.Controls.Add(chartView1);
            tabPage1.Location = new Point(4, 30);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(843, 497);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Chart";
            // 
            // chartView1
            // 
            chartView1.Dock = DockStyle.Fill;
            chartView1.Location = new Point(3, 3);
            chartView1.Name = "chartView1";
            chartView1.Size = new Size(837, 491);
            chartView1.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.BackColor = Color.Transparent;
            tabPage2.Controls.Add(chartProperties1);
            tabPage2.Location = new Point(4, 30);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(843, 497);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Properties";
            // 
            // chartProperties1
            // 
            chartProperties1.AutoScroll = true;
            chartProperties1.Dock = DockStyle.Fill;
            chartProperties1.ForeColor = SystemColors.ControlText;
            chartProperties1.Location = new Point(3, 3);
            chartProperties1.Margin = new Padding(4, 3, 4, 3);
            chartProperties1.Name = "chartProperties1";
            chartProperties1.Size = new Size(837, 491);
            chartProperties1.TabIndex = 0;
            // 
            // Chart
            // 
            Controls.Add(ChartTabs);
            Name = "Chart";
            Size = new Size(851, 531);
            ChartTabs.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        public TabControl ChartTabs;
        public TabPage tabPage1;
        public TabPage tabPage2;
        public ChartView chartView1;
        public ChartProperties chartProperties1;
        //public ChartView chartView;
        //public ChartProperties chartProperties;
    }
}
