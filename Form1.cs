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
            this.Text += Application.ProductVersion;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (ofd1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtPathIn.Text = ofd1.FileName;
            }
        }


        private void btnBrowseOut_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ofd1.FileName))
            {
                sfd1.FileName = System.IO.Path.GetFileNameWithoutExtension(ofd1.FileName) + ".png";
            }
            if (sfd1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtPathOut.Text = sfd1.FileName;
            }
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(txtPathOut.Text))
            {
                if (MessageBox.Show(string.Format(@"The file '{0}' already exists. do you want to overwrite it?",txtPathOut.Text),"Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
                {
                    return;
                }
            }
            try
            {
                var fs = new System.IO.FileStream(txtPathOut.Text, System.IO.FileMode.Create);
                fs.Dispose();
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid output path!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (System.IO.File.Exists(txtPathIn.Text))
            {
                int width;
                int height;
                if (int.TryParse(txtWidth.Text, out width) && int.TryParse(txtHeight.Text, out height))
                {
                    this.Enabled = false;
                    GenerateMovieBarCode(txtPathIn.Text, width, height,txtPathOut.Text);
                    this.Enabled = true;
                }
                else
                {
                    MessageBox.Show("The input values must be valid integers!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                MessageBox.Show("File not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void GenerateMovieBarCode(string videoPath, int width, int height, string outputPath)
        {
            progressBar1.Value = progressBar1.Minimum;
            progressBar1.Maximum = width;
            VideoHelper v = new VideoHelper(videoPath);
            Bitmap b = new Bitmap(width, height);
            System.Drawing.Graphics g = Graphics.FromImage(b);
            for (int i = 0; i < width; i++)
            {
                //v.GetFrameFromVideo(((double)i) / 100.0).Save(string.Format(@"C:\x{0}.jpg", i), System.Drawing.Imaging.ImageFormat.Jpeg);
                Bitmap tempB = v.GetFrameFromVideo(((double)i) / (double)width);
                g.DrawImage(tempB, i, 0, 1, height);
                progressBar1.PerformStep();
                Application.DoEvents();
            }
            System.Drawing.Imaging.ImageFormat format;
            switch (System.IO.Path.GetExtension(outputPath).Trim(".".ToCharArray()).ToLowerInvariant())
            {
                case "bmp":
                    format = System.Drawing.Imaging.ImageFormat.Bmp;
                    break;
                case "jpg":
                case "jpeg":
                    format = System.Drawing.Imaging.ImageFormat.Jpeg;
                    break;
                case "gif":
                    format = System.Drawing.Imaging.ImageFormat.Gif;
                    break;
                case "png":
                    format = System.Drawing.Imaging.ImageFormat.Png;
                    break;
                default:
                    format = System.Drawing.Imaging.ImageFormat.Png;
                    break;
            }
            b.Save(outputPath, format);
            v.Dispose();
        }

        private void link1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadedStart));
            t.Start();
        }

        private void ThreadedStart()
        {
            System.Diagnostics.Process.Start(@"http://arcanesanctum.net");
        }

    }
}
