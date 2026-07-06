using System;
using System.Windows.Forms;

namespace YamuraView
{
    public partial class LogViewerDialog : Form
    {
        public LogViewerDialog()
        {
            InitializeComponent();
            LoadLog();
        }

        private void LoadLog()
        {
            txtLog.Text = AppLogger.ReadAll();
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadLog();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
