using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Geometric2
{
    public partial class BezierPatchTube : Form
    {
        private float[] values;

        public BezierPatchTube(float[] values)
        {
            this.values = values;
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            textBox7.Text = trackBar2.Value.ToString();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            textBox6.Text = trackBar1.Value.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            float res;
            //if (float.TryParse(textBox1.Text, out res))
            {
                values[0] = 1.0f;
            }

            if (float.TryParse(textBox5.Text, out res))
            {
                values[1] = res;
            }


            if (float.TryParse(textBox2.Text, out res))
            {
                values[4] = res;
            }

            values[2] = trackBar2.Value;
            values[3] = trackBar1.Value;

            this.Close();
        }

        //private void trackBar2_Scroll(object sender, EventArgs e)
        //{
        //    textBox7.Text = trackBar2.Value.ToString();
        //}

        //private void trackBar1_Scroll(object sender, EventArgs e)
        //{
        //    textBox6.Text = trackBar1.Value.ToString();
        //}

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    float res;
        //    if (float.TryParse(textBox1.Text, out res))
        //    {
        //        values[0] = res;
        //    }

        //    if (float.TryParse(textBox5.Text, out res))
        //    {
        //        values[1] = res;
        //    }

        //    values[2] = trackBar2.Value;
        //    values[3] = trackBar1.Value;

        //    this.Close();
        //}
    }
}
