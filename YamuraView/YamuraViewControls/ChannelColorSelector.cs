using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YamuraViewControls
{
    public delegate void ColorSelect(object sender, ColorSelectEventArgs e);

    public partial class ChannelColorSelector : Form
    {
        //public event ColorSelect ColorSelectEvent;

        public Color selectedColor;
        public ChannelColorSelector()
        {
            //InitializeComponent();
        }

        //private void color0_Click(object sender, EventArgs e)
        //{
        //    selectedColor = color0.BackColor;
        //    ColorSelectEventArgs colorEventArgs = new ColorSelectEventArgs();
        //    colorEventArgs.SelectedColor = selectedColor;
        //    ColorSelectEvent(this, colorEventArgs);
        //}

        //private void color1_Click(object sender, EventArgs e)
        //{
        //    selectedColor = color1.BackColor;
        //    ColorSelectEventArgs colorEventArgs = new ColorSelectEventArgs();
        //    colorEventArgs.SelectedColor = selectedColor;
        //    ColorSelectEvent(this, colorEventArgs);
        //}

        //private void color2_Click(object sender, EventArgs e)
        //{
        //    selectedColor = color2.BackColor;
        //    ColorSelectEventArgs colorEventArgs = new ColorSelectEventArgs();
        //    colorEventArgs.SelectedColor = selectedColor;
        //    ColorSelectEvent(this, colorEventArgs);
        //}

        //private void color3_Click(object sender, EventArgs e)
        //{
        //    selectedColor = color3.BackColor;
        //    ColorSelectEventArgs colorEventArgs = new ColorSelectEventArgs();
        //    colorEventArgs.SelectedColor = selectedColor;
        //    ColorSelectEvent(this, colorEventArgs);
        //}

        //private void color4_Click(object sender, EventArgs e)
        //{
        //    selectedColor = color4.BackColor;
        //    ColorSelectEventArgs colorEventArgs = new ColorSelectEventArgs();
        //    colorEventArgs.SelectedColor = selectedColor;
        //    ColorSelectEvent(this, colorEventArgs);
        //}

        //private void color5_Click(object sender, EventArgs e)
        //{
        //    selectedColor = color5.BackColor;
        //    ColorSelectEventArgs colorEventArgs = new ColorSelectEventArgs();
        //    colorEventArgs.SelectedColor = selectedColor;
        //    ColorSelectEvent(this, colorEventArgs);
        //}

        //private void color6_Click(object sender, EventArgs e)
        //{
        //    selectedColor = color6.BackColor;
        //    ColorSelectEventArgs colorEventArgs = new ColorSelectEventArgs();
        //    colorEventArgs.SelectedColor = selectedColor;
        //    ColorSelectEvent(this, colorEventArgs);
        //}
    }
    public class ColorSelectEventArgs : EventArgs
    {
        Color selectedColor;
        public Color SelectedColor
        {
            get { return selectedColor; }
            set { selectedColor = value; }
        }
    }
}
