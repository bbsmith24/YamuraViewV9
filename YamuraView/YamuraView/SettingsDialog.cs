using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace YamuraView
{
    public partial class SettingsDialog : Form
    {
        public string FolderToWatch { get; private set; } = "";
        public string ConfigurationFile { get; private set; } = "";
        public List<Color> AutoColors { get; private set; } = new List<Color>();

        private Button[] ColorButtons => new[] { btnColor1, btnColor2, btnColor3, btnColor4, btnColor5, btnColor6, btnColor7 };

        public SettingsDialog()
        {
            InitializeComponent();
        }

        public void SetValues(string folderToWatch, string configurationFile, IEnumerable<Color> autoColors)
        {
            txtFolder.Text = folderToWatch;
            txtConfig.Text = configurationFile;
            Button[] colorButtons = ColorButtons;
            int idx = 0;
            foreach (Color color in autoColors)
            {
                if (idx >= colorButtons.Length)
                {
                    break;
                }
                colorButtons[idx].BackColor = color;
                idx++;
            }
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            if (sender is not Button button)
            {
                return;
            }
            using ColorDialog dialog = new ColorDialog { Color = button.BackColor };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                button.BackColor = dialog.Color;
            }
        }

        private void btnBrowseFolder_Click(object sender, EventArgs e)
        {
            using FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (!string.IsNullOrEmpty(txtFolder.Text) && Directory.Exists(txtFolder.Text))
            {
                dialog.SelectedPath = txtFolder.Text;
            }
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                txtFolder.Text = dialog.SelectedPath;
            }
        }

        private void btnBrowseConfig_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dialog = new OpenFileDialog
            {
                FileName = txtConfig.Text,
                Filter = "YamuraView Config|*.xml"
            };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                txtConfig.Text = dialog.FileName;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFolder.Text))
            {
                MessageBox.Show("Enter an autoload folder.", "Settings", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            FolderToWatch = txtFolder.Text.Trim();
            ConfigurationFile = txtConfig.Text.Trim();
            AutoColors = new List<Color>();
            foreach (Button button in ColorButtons)
            {
                AutoColors.Add(button.BackColor);
            }
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
