namespace YamuraViewControls
{
    partial class ChartProperties
    {
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            layoutPanel = new TableLayoutPanel();
            label2 = new Label();
            cmbXAxis = new ComboBox();
            label5 = new Label();
            cmbChartMode = new ComboBox();
            label1 = new Label();
            cmbChartDefaultPenWidth = new ComboBox();
            chkShowOverlay = new CheckBox();
            axisChannelTree = new TriStateTreeView();
            channelsContext = new ContextMenuStrip(components);
            invertToolStripMenuItem = new ToolStripMenuItem();
            traceColorMenuItem = new ToolStripMenuItem();
            penWidthMenuItem = new ToolStripMenuItem();
            penWidth0MenuItem = new ToolStripMenuItem();
            penWidth1MenuItem = new ToolStripMenuItem();
            penWidth2MenuItem = new ToolStripMenuItem();
            penWidth3MenuItem = new ToolStripMenuItem();
            assignGraphMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            closeToolStripMenuItem = new ToolStripMenuItem();
            colorDialog1 = new ColorDialog();
            layoutPanel.SuspendLayout();
            channelsContext.SuspendLayout();
            SuspendLayout();
            // 
            // layoutPanel
            // 
            layoutPanel.ColumnCount = 2;
            layoutPanel.ColumnStyles.Add(new ColumnStyle());
            layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutPanel.Controls.Add(label2, 0, 0);
            layoutPanel.Controls.Add(cmbXAxis, 1, 0);
            layoutPanel.Controls.Add(label5, 0, 1);
            layoutPanel.Controls.Add(cmbChartMode, 1, 1);
            layoutPanel.Controls.Add(label1, 0, 2);
            layoutPanel.Controls.Add(cmbChartDefaultPenWidth, 1, 2);
            layoutPanel.Controls.Add(chkShowOverlay, 0, 3);
            layoutPanel.Controls.Add(axisChannelTree, 0, 4);
            layoutPanel.Dock = DockStyle.Fill;
            layoutPanel.Location = new Point(0, 0);
            layoutPanel.Name = "layoutPanel";
            layoutPanel.Padding = new Padding(4);
            layoutPanel.RowCount = 5;
            layoutPanel.RowStyles.Add(new RowStyle());
            layoutPanel.RowStyles.Add(new RowStyle());
            layoutPanel.RowStyles.Add(new RowStyle());
            layoutPanel.RowStyles.Add(new RowStyle());
            layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutPanel.Size = new Size(260, 450);
            layoutPanel.TabIndex = 50;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(7, 4);
            label2.Name = "label2";
            label2.Padding = new Padding(0, 0, 6, 0);
            label2.Size = new Size(98, 27);
            label2.TabIndex = 31;
            label2.Text = "X Axis";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cmbXAxis
            // 
            cmbXAxis.Dock = DockStyle.Fill;
            cmbXAxis.Font = new Font("Segoe UI", 12F);
            cmbXAxis.FormattingEnabled = true;
            cmbXAxis.Location = new Point(108, 6);
            cmbXAxis.Margin = new Padding(0, 2, 4, 2);
            cmbXAxis.Name = "cmbXAxis";
            cmbXAxis.Size = new Size(144, 29);
            cmbXAxis.TabIndex = 32;
            cmbXAxis.SelectedIndexChanged += cmbXAxis_SelectedIndexChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Dock = DockStyle.Fill;
            label5.Font = new Font("Segoe UI", 12F);
            label5.Location = new Point(7, 31);
            label5.Name = "label5";
            label5.Padding = new Padding(0, 0, 6, 0);
            label5.Size = new Size(98, 27);
            label5.TabIndex = 40;
            label5.Text = "Chart Mode";
            label5.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cmbChartMode
            // 
            cmbChartMode.Dock = DockStyle.Fill;
            cmbChartMode.Font = new Font("Segoe UI", 12F);
            cmbChartMode.FormattingEnabled = true;
            cmbChartMode.Items.AddRange(new object[] { "Absolute", "Normalized" });
            cmbChartMode.Location = new Point(108, 33);
            cmbChartMode.Margin = new Padding(0, 2, 4, 2);
            cmbChartMode.Name = "cmbChartMode";
            cmbChartMode.Size = new Size(144, 29);
            cmbChartMode.TabIndex = 41;
            cmbChartMode.Text = "Absolute";
            cmbChartMode.SelectedIndexChanged += cmbChartMode_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(7, 58);
            label1.Name = "label1";
            label1.Padding = new Padding(0, 0, 6, 0);
            label1.Size = new Size(98, 27);
            label1.TabIndex = 44;
            label1.Text = "Pen Width";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cmbChartDefaultPenWidth
            // 
            cmbChartDefaultPenWidth.Dock = DockStyle.Fill;
            cmbChartDefaultPenWidth.Font = new Font("Segoe UI", 12F);
            cmbChartDefaultPenWidth.FormattingEnabled = true;
            cmbChartDefaultPenWidth.Items.AddRange(new object[] { "0", "1", "2", "3" });
            cmbChartDefaultPenWidth.Location = new Point(108, 60);
            cmbChartDefaultPenWidth.Margin = new Padding(0, 2, 4, 2);
            cmbChartDefaultPenWidth.Name = "cmbChartDefaultPenWidth";
            cmbChartDefaultPenWidth.Size = new Size(144, 29);
            cmbChartDefaultPenWidth.TabIndex = 43;
            cmbChartDefaultPenWidth.Text = "0";
            cmbChartDefaultPenWidth.SelectedIndexChanged += cmbChartDefaultPenWidth_SelectedIndexChanged;
            // 
            // chkShowOverlay
            // 
            chkShowOverlay.AutoSize = true;
            chkShowOverlay.Checked = true;
            chkShowOverlay.CheckState = CheckState.Checked;
            layoutPanel.SetColumnSpan(chkShowOverlay, 2);
            chkShowOverlay.Font = new Font("Segoe UI", 12F);
            chkShowOverlay.Location = new Point(4, 87);
            chkShowOverlay.Margin = new Padding(0, 2, 0, 2);
            chkShowOverlay.Name = "chkShowOverlay";
            chkShowOverlay.Size = new Size(175, 25);
            chkShowOverlay.TabIndex = 42;
            chkShowOverlay.Text = "Show Values Overlay";
            chkShowOverlay.UseVisualStyleBackColor = true;
            chkShowOverlay.CheckedChanged += chkShowOverlay_CheckedChanged;
            // 
            // axisChannelTree
            // 
            layoutPanel.SetColumnSpan(axisChannelTree, 2);
            axisChannelTree.ContextMenuStrip = channelsContext;
            axisChannelTree.Dock = DockStyle.Fill;
            axisChannelTree.Font = new Font("Segoe UI", 12F);
            axisChannelTree.Location = new Point(4, 116);
            axisChannelTree.Margin = new Padding(0, 2, 0, 0);
            axisChannelTree.Name = "axisChannelTree";
            axisChannelTree.Size = new Size(252, 330);
            axisChannelTree.TabIndex = 30;
            axisChannelTree.AfterCheck += axisChannelTree_AfterCheck;
            // 
            // channelsContext
            // 
            channelsContext.Font = new Font("Segoe UI", 9F);
            channelsContext.Items.AddRange(new ToolStripItem[] { invertToolStripMenuItem, traceColorMenuItem, penWidthMenuItem, assignGraphMenuItem, toolStripSeparator1, closeToolStripMenuItem });
            channelsContext.Name = "channelsContext";
            channelsContext.Size = new Size(159, 120);
            channelsContext.Closed += channelsContext_Closed;
            channelsContext.Opening += channelsContext_Opening;
            // 
            // invertToolStripMenuItem
            // 
            invertToolStripMenuItem.Name = "invertToolStripMenuItem";
            invertToolStripMenuItem.Size = new Size(158, 22);
            invertToolStripMenuItem.Text = "Invert";
            invertToolStripMenuItem.Click += invertToolStripMenuItem_Click;
            // 
            // traceColorMenuItem
            // 
            traceColorMenuItem.Name = "traceColorMenuItem";
            traceColorMenuItem.Size = new Size(158, 22);
            traceColorMenuItem.Text = "Trace Color";
            traceColorMenuItem.Click += traceColorMenuItem_Click;
            // 
            // penWidthMenuItem
            // 
            penWidthMenuItem.DropDownItems.AddRange(new ToolStripItem[] { penWidth0MenuItem, penWidth1MenuItem, penWidth2MenuItem, penWidth3MenuItem });
            penWidthMenuItem.Name = "penWidthMenuItem";
            penWidthMenuItem.Size = new Size(158, 22);
            penWidthMenuItem.Text = "Pen Width";
            // 
            // penWidth0MenuItem
            // 
            penWidth0MenuItem.Name = "penWidth0MenuItem";
            penWidth0MenuItem.Size = new Size(80, 22);
            penWidth0MenuItem.Text = "0";
            penWidth0MenuItem.Click += penWidth0MenuItem_Click;
            // 
            // penWidth1MenuItem
            // 
            penWidth1MenuItem.Name = "penWidth1MenuItem";
            penWidth1MenuItem.Size = new Size(80, 22);
            penWidth1MenuItem.Text = "1";
            penWidth1MenuItem.Click += penWidth1MenuItem_Click;
            // 
            // penWidth2MenuItem
            // 
            penWidth2MenuItem.Name = "penWidth2MenuItem";
            penWidth2MenuItem.Size = new Size(80, 22);
            penWidth2MenuItem.Text = "2";
            penWidth2MenuItem.Click += penWidth2MenuItem_Click;
            // 
            // penWidth3MenuItem
            // 
            penWidth3MenuItem.Name = "penWidth3MenuItem";
            penWidth3MenuItem.Size = new Size(80, 22);
            penWidth3MenuItem.Text = "3";
            penWidth3MenuItem.Click += penWidth3MenuItem_Click;
            // 
            // assignGraphMenuItem
            // 
            assignGraphMenuItem.Name = "assignGraphMenuItem";
            assignGraphMenuItem.Size = new Size(158, 22);
            assignGraphMenuItem.Text = "Assign to Graph";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(155, 6);
            // 
            // closeToolStripMenuItem
            // 
            closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            closeToolStripMenuItem.Size = new Size(158, 22);
            closeToolStripMenuItem.Text = "Close";
            closeToolStripMenuItem.Click += closeToolStripMenuItem_Click;
            // 
            // ChartProperties
            // 
            AutoScaleMode = AutoScaleMode.None;
            AutoScroll = true;
            Controls.Add(layoutPanel);
            ForeColor = SystemColors.ControlText;
            Margin = new Padding(4, 3, 4, 3);
            Name = "ChartProperties";
            Size = new Size(260, 450);
            layoutPanel.ResumeLayout(false);
            layoutPanel.PerformLayout();
            channelsContext.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel layoutPanel;
        private System.Windows.Forms.ComboBox cmbXAxis;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkShowOverlay;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem invertToolStripMenuItem;
        private Label label5;
        private ToolStripMenuItem traceColorMenuItem;
        private ToolStripMenuItem assignGraphMenuItem;
        private ColorDialog colorDialog1;
        public TriStateTreeView axisChannelTree;
        public ComboBox cmbChartMode;
        private ToolStripSeparator toolStripSeparator1;
        private Label label1;
        private ToolStripMenuItem penWidth0MenuItem;
        private ToolStripMenuItem penWidth1MenuItem;
        private ToolStripMenuItem penWidth2MenuItem;
        private ToolStripMenuItem penWidth3MenuItem;
        public ContextMenuStrip channelsContext;
        public ToolStripMenuItem penWidthMenuItem;
        public ComboBox cmbChartDefaultPenWidth;
    }
}
