using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace YamuraView
{
    public partial class ClearRunsDialog : Form
    {
        public List<int> SelectedIndices { get; private set; } = new List<int>();

        public ClearRunsDialog()
        {
            InitializeComponent();
        }

        public void PopulateRuns(List<RunData> runs)
        {
            clbRuns.Items.Clear();
            foreach (RunData run in runs)
            {
                string label = string.IsNullOrEmpty(run.fileName)
                    ? run.runName
                    : $"{run.runName}  -  {Path.GetFileName(run.fileName)}";
                clbRuns.Items.Add(label);
            }
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbRuns.Items.Count; i++)
            {
                clbRuns.SetItemChecked(i, true);
            }
        }

        private void btnSelectNone_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbRuns.Items.Count; i++)
            {
                clbRuns.SetItemChecked(i, false);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            List<int> checkedIndices = new List<int>();
            foreach (int idx in clbRuns.CheckedIndices)
            {
                checkedIndices.Add(idx);
            }
            if (checkedIndices.Count == 0)
            {
                MessageBox.Show("Select at least one run to remove.", "Clear Runs", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SelectedIndices = checkedIndices;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
