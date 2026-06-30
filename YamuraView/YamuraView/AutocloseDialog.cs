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
    public partial class AutoCloseDialog : Form
    {
        string TitleText {  get; set; }
        public AutoCloseDialog(string caption, string message)
        {
            TitleText = caption;
            Text = TitleText;
            Width = 400;
            Height = 150;

            Label label = new Label() { Text = message, Left = 10, Top = 10, Width = 380, Height = 130 };
            Controls.Add(label);

            // Trigger the automatic close when the form loads
            Load += async (s, e) => await CloseAfterDelayAsync(3000); // 3 seconds
        }

        private async Task CloseAfterDelayAsync(int milliseconds)
        {
            await Task.Delay(milliseconds);
            this.Close(); // Safely closes the dialog without user interaction
        }
    }
}
