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
            AutoScaleMode = AutoScaleMode.None;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(380, 100);
            MaximumSize = new Size(400, 140);

            Label label = new Label() { Text = message, Left = 10, Top = 10, Width = 360, Height = 80, AutoSize = false };
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
