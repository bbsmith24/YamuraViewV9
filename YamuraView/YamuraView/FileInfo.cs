using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YamuraView
{
    public partial class FileInfo : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public String FileInfoText
        {
            set { fileInfoText.Text = value; }
        }
        public FileInfo()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
