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
            lblAxisMin = new ToolStripTextBox();
            txtAxisMinValue = new ToolStripTextBox();
            lblAxisMax = new ToolStripTextBox();
            txtAxisMaxValue = new ToolStripTextBox();
            closeToolStripMenuItem = new ToolStripMenuItem();
            traceColorMenuItem = new ToolStripMenuItem();
            cmbXAxis = new ComboBox();
            label2 = new Label();
            label3 = new Label();
            axisOffsetsGrid = new DataGridView();
            axisChannel = new DataGridViewTextBoxColumn();
            axisStart = new DataGridViewTextBoxColumn();
            axisEnd = new DataGridViewTextBoxColumn();
            axisOffset = new DataGridViewTextBoxColumn();
            cmbAlignAxis = new ComboBox();
            label1 = new Label();
            label4 = new Label();
            txtAutoThreshold = new TextBox();
            btnDoAutoAlign = new Button();
            cmbChartMode = new ComboBox();
            label5 = new Label();
            colorDialog1 = new ColorDialog();
            channelsContext.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)axisOffsetsGrid).BeginInit();
            SuspendLayout();
            // 
            // axisChannelTree
            // 
            axisChannelTree.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            axisChannelTree.ContextMenuStrip = channelsContext;
            axisChannelTree.Location = new Point(14, 202);
            axisChannelTree.Margin = new Padding(4, 3, 4, 3);
            axisChannelTree.Name = "axisChannelTree";
            axisChannelTree.Size = new Size(405, 241);
            axisChannelTree.TabIndex = 30;
            axisChannelTree.AfterCheck += axisChannelTree_AfterCheck;
            // 
            // channelsContext
            // 
            channelsContext.Items.AddRange(new ToolStripItem[] { invertToolStripMenuItem, lblAxisMin, txtAxisMinValue, lblAxisMax, txtAxisMaxValue, closeToolStripMenuItem, traceColorMenuItem });
            channelsContext.Name = "channelsContext";
            channelsContext.Size = new Size(161, 170);
            channelsContext.Closed += channelsContext_Closed;
            channelsContext.Opening += channelsContext_Opening;
            // 
            // invertToolStripMenuItem
            // 
            invertToolStripMenuItem.Name = "invertToolStripMenuItem";
            invertToolStripMenuItem.Size = new Size(160, 22);
            invertToolStripMenuItem.Text = "Invert";
            invertToolStripMenuItem.Click += invertToolStripMenuItem_Click;
            // 
            // lblAxisMin
            // 
            lblAxisMin.Name = "lblAxisMin";
            lblAxisMin.Size = new Size(100, 23);
            lblAxisMin.Text = "Axis Minimum";
            // 
            // txtAxisMinValue
            // 
            txtAxisMinValue.Name = "txtAxisMinValue";
            txtAxisMinValue.Size = new Size(100, 23);
            txtAxisMinValue.Text = "Axis Minimum Value";
            // 
            // lblAxisMax
            // 
            lblAxisMax.Name = "lblAxisMax";
            lblAxisMax.Size = new Size(100, 23);
            lblAxisMax.Text = "Axis Maximum";
            // 
            // txtAxisMaxValue
            // 
            txtAxisMaxValue.Name = "txtAxisMaxValue";
            txtAxisMaxValue.Size = new Size(100, 23);
            txtAxisMaxValue.Text = "Axis Maximum Value";
            // 
            // closeToolStripMenuItem
            // 
            closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            closeToolStripMenuItem.Size = new Size(160, 22);
            closeToolStripMenuItem.Text = "Close";
            closeToolStripMenuItem.Click += closeToolStripMenuItem_Click;
            // 
            // traceColorMenuItem
            // 
            traceColorMenuItem.Name = "traceColorMenuItem";
            traceColorMenuItem.Size = new Size(160, 22);
            traceColorMenuItem.Text = "Trace Color";
            traceColorMenuItem.Click += traceColorMenuItem_Click;
            // 
            // cmbXAxis
            // 
            cmbXAxis.FormattingEnabled = true;
            cmbXAxis.Location = new Point(83, 14);
            cmbXAxis.Margin = new Padding(4, 3, 4, 3);
            cmbXAxis.Name = "cmbXAxis";
            cmbXAxis.Size = new Size(104, 23);
            cmbXAxis.TabIndex = 32;
            cmbXAxis.SelectedIndexChanged += cmbXAxis_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(14, 17);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(38, 15);
            label2.TabIndex = 31;
            label2.Text = "X Axis";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(14, 183);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(56, 15);
            label3.TabIndex = 33;
            label3.Text = "Channels";
            // 
            // axisOffsetsGrid
            // 
            axisOffsetsGrid.AllowUserToAddRows = false;
            axisOffsetsGrid.AllowUserToDeleteRows = false;
            axisOffsetsGrid.AllowUserToOrderColumns = true;
            axisOffsetsGrid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            axisOffsetsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            axisOffsetsGrid.Columns.AddRange(new DataGridViewColumn[] { axisChannel, axisStart, axisEnd, axisOffset });
            axisOffsetsGrid.Location = new Point(17, 116);
            axisOffsetsGrid.Margin = new Padding(4, 3, 4, 3);
            axisOffsetsGrid.Name = "axisOffsetsGrid";
            axisOffsetsGrid.RowHeadersVisible = false;
            axisOffsetsGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            axisOffsetsGrid.Size = new Size(402, 64);
            axisOffsetsGrid.TabIndex = 34;
            // 
            // axisChannel
            // 
            axisChannel.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            axisChannel.HeaderText = "Channel";
            axisChannel.Name = "axisChannel";
            axisChannel.Width = 76;
            // 
            // axisStart
            // 
            axisStart.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            axisStart.HeaderText = "Start";
            axisStart.Name = "axisStart";
            axisStart.Width = 56;
            // 
            // axisEnd
            // 
            axisEnd.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            axisEnd.HeaderText = "End";
            axisEnd.Name = "axisEnd";
            axisEnd.Width = 52;
            // 
            // axisOffset
            // 
            axisOffset.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            axisOffset.HeaderText = "Offset";
            axisOffset.Name = "axisOffset";
            // 
            // cmbAlignAxis
            // 
            cmbAlignAxis.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbAlignAxis.Enabled = false;
            cmbAlignAxis.FormattingEnabled = true;
            cmbAlignAxis.Location = new Point(73, 79);
            cmbAlignAxis.Margin = new Padding(4, 3, 4, 3);
            cmbAlignAxis.Name = "cmbAlignAxis";
            cmbAlignAxis.Size = new Size(104, 23);
            cmbAlignAxis.TabIndex = 36;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 84);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(54, 15);
            label1.TabIndex = 35;
            label1.Text = "Align On";
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Location = new Point(185, 84);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(60, 15);
            label4.TabIndex = 37;
            label4.Text = "Threshold";
            // 
            // txtAutoThreshold
            // 
            txtAutoThreshold.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtAutoThreshold.Enabled = false;
            txtAutoThreshold.Location = new Point(254, 79);
            txtAutoThreshold.Margin = new Padding(4, 3, 4, 3);
            txtAutoThreshold.Name = "txtAutoThreshold";
            txtAutoThreshold.Size = new Size(116, 23);
            txtAutoThreshold.TabIndex = 38;
            // 
            // btnDoAutoAlign
            // 
            btnDoAutoAlign.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDoAutoAlign.Enabled = false;
            btnDoAutoAlign.Location = new Point(378, 78);
            btnDoAutoAlign.Margin = new Padding(4, 3, 4, 3);
            btnDoAutoAlign.Name = "btnDoAutoAlign";
            btnDoAutoAlign.Size = new Size(41, 27);
            btnDoAutoAlign.TabIndex = 39;
            btnDoAutoAlign.Text = "Go";
            btnDoAutoAlign.UseVisualStyleBackColor = true;
            btnDoAutoAlign.Click += btnDoAutoAlign_Click;
            // 
            // cmbChartMode
            // 
            cmbChartMode.FormattingEnabled = true;
            cmbChartMode.Items.AddRange(new object[] { "Absolute", "Normalized" });
            cmbChartMode.Location = new Point(83, 43);
            cmbChartMode.Margin = new Padding(4, 3, 4, 3);
            cmbChartMode.Name = "cmbChartMode";
            cmbChartMode.Size = new Size(104, 23);
            cmbChartMode.TabIndex = 41;
            cmbChartMode.Text = "Absolute";
            cmbChartMode.SelectedIndexChanged += cmbChartMode_SelectedIndexChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(14, 46);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(70, 15);
            label5.TabIndex = 40;
            label5.Text = "Chart Mode";
            // 
            // ChartProperties
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(cmbChartMode);
            Controls.Add(label5);
            Controls.Add(btnDoAutoAlign);
            Controls.Add(txtAutoThreshold);
            Controls.Add(label4);
            Controls.Add(cmbAlignAxis);
            Controls.Add(label1);
            Controls.Add(axisOffsetsGrid);
            Controls.Add(label3);
            Controls.Add(cmbXAxis);
            Controls.Add(label2);
            Controls.Add(axisChannelTree);
            Margin = new Padding(4, 3, 4, 3);
            Name = "ChartProperties";
            Size = new Size(434, 457);
            channelsContext.ResumeLayout(false);
            channelsContext.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)axisOffsetsGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private TriStateTreeView axisChannelTree;
        private System.Windows.Forms.ComboBox cmbXAxis;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ContextMenuStrip channelsContext;
        private System.Windows.Forms.ToolStripTextBox lblAxisMin;
        private System.Windows.Forms.ToolStripTextBox txtAxisMinValue;
        private System.Windows.Forms.ToolStripTextBox lblAxisMax;
        private System.Windows.Forms.ToolStripTextBox txtAxisMaxValue;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.DataGridView axisOffsetsGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn axisChannel;
        private System.Windows.Forms.DataGridViewTextBoxColumn axisStart;
        private System.Windows.Forms.DataGridViewTextBoxColumn axisEnd;
        private System.Windows.Forms.DataGridViewTextBoxColumn axisOffset;
        private System.Windows.Forms.ComboBox cmbAlignAxis;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtAutoThreshold;
        private System.Windows.Forms.Button btnDoAutoAlign;
        private System.Windows.Forms.ToolStripMenuItem invertToolStripMenuItem;
        private ComboBox cmbChartMode;
        private Label label5;
        private ToolStripMenuItem traceColorMenuItem;
        private ColorDialog colorDialog1;
    }
}