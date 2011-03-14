using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MovieBarCode
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (ofd1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtPath.Text = ofd1.FileName;
            }
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            GenerateMovieBarCode(txtPath.Text);
        }

        private void GenerateMovieBarCode(string videoPath)
        {
            VideoHelper v = new VideoHelper(videoPath);
            //for (int i = 0; i < 100; i++)
            //{
            //    v.SaveFrameFromVideo(((double)i) / 100.0, string.Format(@"C:\x{0}.bmp", i));
            //}
            //for (int i = 0; i < 100; i++)
            //{
            //     v.GetFrameFromVideo(((double)i) / 100.0).Save(string.Format(@"C:\x{0}.jpg", i),System.Drawing.Imaging.ImageFormat.Jpeg);
            //}
            v.Dispose();
        }


    }
}
