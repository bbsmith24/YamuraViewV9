namespace YamuraView
{
    partial class ClearRunsDialog : Form
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
            lblInstructions = new Label();
            clbRuns = new CheckedListBox();
            btnSelectAll = new Button();
            btnSelectNone = new Button();
            btnOK = new Button();
            btnCancel = new Button();
            SuspendLayout();

            // lblInstructions
            lblInstructions.AutoSize = true;
            lblInstructions.Location = new System.Drawing.Point(12, 9);
            lblInstructions.Size = new System.Drawing.Size(300, 15);
            lblInstructions.Text = "Select runs to remove from the log data and charts:";

            // clbRuns
            clbRuns.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            clbRuns.CheckOnClick = true;
            clbRuns.IntegralHeight = false;
            clbRuns.Location = new System.Drawing.Point(12, 32);
            clbRuns.Size = new System.Drawing.Size(400, 260);
            clbRuns.TabIndex = 0;

            // btnSelectAll
            btnSelectAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSelectAll.Location = new System.Drawing.Point(12, 300);
            btnSelectAll.Size = new System.Drawing.Size(90, 30);
            btnSelectAll.TabIndex = 1;
            btnSelectAll.Text = "Select All";
            btnSelectAll.UseVisualStyleBackColor = true;
            btnSelectAll.Click += btnSelectAll_Click;

            // btnSelectNone
            btnSelectNone.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnSelectNone.Location = new System.Drawing.Point(108, 300);
            btnSelectNone.Size = new System.Drawing.Size(90, 30);
            btnSelectNone.TabIndex = 2;
            btnSelectNone.Text = "Select None";
            btnSelectNone.UseVisualStyleBackColor = true;
            btnSelectNone.Click += btnSelectNone_Click;

            // btnOK
            btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOK.Location = new System.Drawing.Point(242, 300);
            btnOK.Size = new System.Drawing.Size(80, 30);
            btnOK.TabIndex = 3;
            btnOK.Text = "Remove";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;

            // btnCancel
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.Location = new System.Drawing.Point(332, 300);
            btnCancel.Size = new System.Drawing.Size(80, 30);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;

            // ClearRunsDialog
            AcceptButton = btnOK;
            CancelButton = btnCancel;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(424, 342);
            Controls.Add(lblInstructions);
            Controls.Add(clbRuns);
            Controls.Add(btnSelectAll);
            Controls.Add(btnSelectNone);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            MinimumSize = new System.Drawing.Size(320, 260);
            Name = "ClearRunsDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Clear Runs";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblInstructions;
        private CheckedListBox clbRuns;
        private Button btnSelectAll;
        private Button btnSelectNone;
        private Button btnOK;
        private Button btnCancel;
    }
}
