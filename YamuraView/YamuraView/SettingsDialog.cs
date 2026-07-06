using System;
using System.IO;
using System.Windows.Forms;

namespace YamuraView
{
    public partial class SettingsDialog : Form
    {
        public string FolderToWatch { get; private set; } = "";
        public string ConfigurationFile { get; private set; } = "";
        public string WifiSSID { get; private set; } = "";
        public string WifiPassword { get; private set; } = "";
        public string FileZillaServerPath { get; private set; } = "";

        public SettingsDialog()
        {
            InitializeComponent();
        }

        public void SetValues(string folderToWatch, string configurationFile, string wifiSSID, string wifiPassword, string fileZillaServerPath)
        {
            txtFolder.Text = folderToWatch;
            txtConfig.Text = configurationFile;
            txtSSID.Text = wifiSSID;
            txtPassword.Text = wifiPassword;
            txtFileZilla.Text = fileZillaServerPath;
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

        private void btnBrowseFileZilla_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dialog = new OpenFileDialog
            {
                FileName = txtFileZilla.Text,
                Filter = "Executable Files|*.exe"
            };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                txtFileZilla.Text = dialog.FileName;
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
            WifiSSID = txtSSID.Text.Trim();
            WifiPassword = txtPassword.Text;
            FileZillaServerPath = txtFileZilla.Text.Trim();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnTogglePasswordVis_Click(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
            if (txtPassword.UseSystemPasswordChar)
            {
                btnTogglePasswordVis.Text = "S";
            }
            else
            {
                btnTogglePasswordVis.Text = "H";
            }
        }
    }
}
