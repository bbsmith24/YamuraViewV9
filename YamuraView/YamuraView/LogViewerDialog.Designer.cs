namespace YamuraView
{
    partial class LogViewerDialog : Form
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            txtLog = new TextBox();
            btnRefresh = new Button();
            btnClose = new Button();
            SuspendLayout();

            // txtLog
            txtLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtLog.Multiline = true;
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Both;
            txtLog.WordWrap = false;
            txtLog.Font = new System.Drawing.Font("Consolas", 9F);
            txtLog.Location = new System.Drawing.Point(12, 12);
            txtLog.Size = new System.Drawing.Size(560, 360);
            txtLog.TabIndex = 0;

            // btnRefresh
            btnRefresh.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnRefresh.Location = new System.Drawing.Point(412, 384);
            btnRefresh.Size = new System.Drawing.Size(80, 30);
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;

            // btnClose
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.Location = new System.Drawing.Point(492, 384);
            btnClose.Size = new System.Drawing.Size(80, 30);
            btnClose.TabIndex = 2;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;

            // LogViewerDialog
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(584, 426);
            Controls.Add(txtLog);
            Controls.Add(btnRefresh);
            Controls.Add(btnClose);
            MinimumSize = new System.Drawing.Size(400, 300);
            Name = "LogViewerDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Application Log";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtLog;
        private Button btnRefresh;
        private Button btnClose;
    }
}
