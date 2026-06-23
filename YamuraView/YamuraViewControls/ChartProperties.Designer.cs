namespace YamuraViewControls
{
    partial class ChartProperties
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            axisChannelTree = new TriStateTreeView();
            channelsContext = new ContextMenuStrip(components);
            invertToolStripMenuItem = new ToolStripMenuItem();
            closeToolStripMenuItem = new ToolStripMenuItem();
            traceColorMenuItem = new ToolStripMenuItem();
            assignGraphMenuItem = new ToolStripMenuItem();
            chkShowOverlay = new CheckBox();
            cmbXAxis = new ComboBox();
            label2 = new Label();
            cmbChartMode = new ComboBox();
            label5 = new Label();
            colorDialog1 = new ColorDialog();
            toolStripSeparator1 = new ToolStripSeparator();
            channelsContext.SuspendLayout();
            SuspendLayout();
            // 
            // axisChannelTree
            // 
            axisChannelTree.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            axisChannelTree.ContextMenuStrip = channelsContext;
            axisChannelTree.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            axisChannelTree.Location = new Point(14, 115);
            axisChannelTree.Margin = new Padding(4, 3, 4, 3);
            axisChannelTree.Name = "axisChannelTree";
            axisChannelTree.Size = new Size(405, 341);
            axisChannelTree.TabIndex = 30;
            axisChannelTree.AfterCheck += axisChannelTree_AfterCheck;
            // 
            // channelsContext
            // 
            channelsContext.Items.AddRange(new ToolStripItem[] { invertToolStripMenuItem, traceColorMenuItem, assignGraphMenuItem, toolStripSeparator1, closeToolStripMenuItem });
            channelsContext.Name = "channelsContext";
            channelsContext.Size = new Size(181, 120);
            channelsContext.Closed += channelsContext_Closed;
            channelsContext.Opening += channelsContext_Opening;
            // 
            // invertToolStripMenuItem
            // 
            invertToolStripMenuItem.Name = "invertToolStripMenuItem";
            invertToolStripMenuItem.Size = new Size(180, 22);
            invertToolStripMenuItem.Text = "Invert";
            invertToolStripMenuItem.Click += invertToolStripMenuItem_Click;
            // 
            // closeToolStripMenuItem
            // 
            closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            closeToolStripMenuItem.Size = new Size(180, 22);
            closeToolStripMenuItem.Text = "Close";
            closeToolStripMenuItem.Click += closeToolStripMenuItem_Click;
            // 
            // traceColorMenuItem
            // 
            traceColorMenuItem.Name = "traceColorMenuItem";
            traceColorMenuItem.Size = new Size(180, 22);
            traceColorMenuItem.Text = "Trace Color";
            traceColorMenuItem.Click += traceColorMenuItem_Click;
            // 
            // assignGraphMenuItem
            // 
            assignGraphMenuItem.Name = "assignGraphMenuItem";
            assignGraphMenuItem.Size = new Size(180, 22);
            assignGraphMenuItem.Text = "Assign to Graph";
            // 
            // chkShowOverlay
            // 
            chkShowOverlay.AutoSize = true;
            chkShowOverlay.Checked = true;
            chkShowOverlay.CheckState = CheckState.Checked;
            chkShowOverlay.Font = new Font("Segoe UI", 12F);
            chkShowOverlay.Location = new Point(19, 84);
            chkShowOverlay.Name = "chkShowOverlay";
            chkShowOverlay.Size = new Size(175, 25);
            chkShowOverlay.TabIndex = 42;
            chkShowOverlay.Text = "Show Values Overlay";
            chkShowOverlay.UseVisualStyleBackColor = true;
            chkShowOverlay.CheckedChanged += chkShowOverlay_CheckedChanged;
            // 
            // cmbXAxis
            // 
            cmbXAxis.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmbXAxis.FormattingEnabled = true;
            cmbXAxis.Location = new Point(114, 14);
            cmbXAxis.Margin = new Padding(4, 3, 4, 3);
            cmbXAxis.Name = "cmbXAxis";
            cmbXAxis.Size = new Size(104, 29);
            cmbXAxis.TabIndex = 32;
            cmbXAxis.SelectedIndexChanged += cmbXAxis_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(19, 17);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(51, 21);
            label2.TabIndex = 31;
            label2.Text = "X Axis";
            // 
            // cmbChartMode
            // 
            cmbChartMode.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmbChartMode.FormattingEnabled = true;
            cmbChartMode.Items.AddRange(new object[] { "Absolute", "Normalized" });
            cmbChartMode.Location = new Point(114, 49);
            cmbChartMode.Margin = new Padding(4, 3, 4, 3);
            cmbChartMode.Name = "cmbChartMode";
            cmbChartMode.Size = new Size(104, 29);
            cmbChartMode.TabIndex = 41;
            cmbChartMode.Text = "Absolute";
            cmbChartMode.SelectedIndexChanged += cmbChartMode_SelectedIndexChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F);
            label5.Location = new Point(14, 52);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(92, 21);
            label5.TabIndex = 40;
            label5.Text = "Chart Mode";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(177, 6);
            // 
            // ChartProperties
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(cmbChartMode);
            Controls.Add(label5);
            Controls.Add(chkShowOverlay);
            Controls.Add(cmbXAxis);
            Controls.Add(label2);
            Controls.Add(axisChannelTree);
            ForeColor = SystemColors.ControlText;
            Margin = new Padding(4, 3, 4, 3);
            Name = "ChartProperties";
            Size = new Size(434, 457);
            channelsContext.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox cmbXAxis;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkShowOverlay;
        private System.Windows.Forms.ContextMenuStrip channelsContext;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem invertToolStripMenuItem;
        private Label label5;
        private ToolStripMenuItem traceColorMenuItem;
        private ToolStripMenuItem assignGraphMenuItem;
        private ColorDialog colorDialog1;
        public TriStateTreeView axisChannelTree;
        public ComboBox cmbChartMode;
        private ToolStripSeparator toolStripSeparator1;
    }
}