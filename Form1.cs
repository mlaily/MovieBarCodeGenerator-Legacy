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
            if (System.IO.File.Exists(txtPathIn.Text))
            {
                int width;
                int height;
                int iterations;
                int barWidth;
                try
                {
                    width = int.Parse(txtWidth.Text);
                    if (width <= 0)
                    {
                        throw new ArgumentException();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Width not valid!");
                    return;
                }
                try
                {
                    height = int.Parse(txtHeight.Text);
                    if (height <= 0)
                    {
                        throw new ArgumentException();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Height not valid!");
                    return;
                }
                try
                {
                    iterations = int.Parse(txtIterations.Text);
                    if (iterations <= 0)
                    {
                        throw new ArgumentException();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Iterations value not valid!");
                    return;
                }
                try
                {
                    barWidth = int.Parse(txtBarWidth.Text);
                    if (barWidth <= 0)
                    {
                        throw new ArgumentException();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Bar-width value not valid!");
                    return;
                }

                if (System.IO.File.Exists(txtPathOut.Text))
                {
                    if (MessageBox.Show(string.Format(@"The file '{0}' already exists. are you sure you want to overwrite it?", txtPathOut.Text), "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.Yes)
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

                this.Enabled = false;
                GenerateMovieBarCode(txtPathIn.Text, width, height, txtPathOut.Text, iterations, barWidth);
                this.Enabled = true;
            }
            else
            {
                MessageBox.Show("File not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void GenerateMovieBarCode(string videoPath, int width, int height, string outputPath, int iterations, int barWidth)
        {
            progressBar1.Value = progressBar1.Minimum;
            progressBar1.Maximum = iterations;
            VideoHelper v = new VideoHelper(videoPath);
            Bitmap b = new Bitmap(width, height);
            System.Drawing.Graphics g = Graphics.FromImage(b);
#if DEBUG
            DateTime start = DateTime.Now;
#endif
            for (int i = 0; i < iterations; i++)
            {
                Bitmap tempB = v.GetFrameFromVideo(((double)i) / (double)iterations);
                g.DrawImage(tempB, i * barWidth, 0, barWidth, height);
                if (i % 10 == 0)
                {
                    //once every 10 iteration should not be too much
                    //but is enough to keep the memory footprint as low as possible
                    //(almost only cosmetic change)
                    GC.Collect();
                }
                progressBar1.PerformStep();
                Application.DoEvents();
            }
#if DEBUG
            DateTime end = DateTime.Now;
            var total = end - start;
            Console.WriteLine(total);
#endif
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
            new System.Threading.Thread(
                new System.Threading.ThreadStart(
                    () => System.Diagnostics.Process.Start(@"http://arcanesanctum.net"))).Start();
        }

        private void txtIterations_TextChanged(object sender, EventArgs e)
        {
            if (!chkAutoCorrect.Checked)
            {
                return;
            }
            //barwidth = width/iterations
            int width;
            int iterations;
            try
            {
                width = int.Parse(txtWidth.Text);
            }
            catch (Exception)
            {
                return;
            }
            try
            {
                iterations = int.Parse(txtIterations.Text);
            }
            catch (Exception)
            {
                return;
            }
            try
            {
                int barWidth = width / iterations;
                txtBarWidth.Text = barWidth.ToString();
            }
            catch (Exception)
            {
            }
        }

        private void txtBarWidth_TextChanged(object sender, EventArgs e)
        {
            if (!chkAutoCorrect.Checked)
            {
                return;
            }
            //iterations = width/barwidth
            int width;
            int barWidth;
            try
            {
                width = int.Parse(txtWidth.Text);
            }
            catch (Exception)
            {
                return;
            }
            try
            {
                barWidth = int.Parse(txtBarWidth.Text);
            }
            catch (Exception)
            {
                return;
            }

            try
            {
                int iterations = width / barWidth;
                txtIterations.Text = iterations.ToString();
            }
            catch (Exception)
            {
            }
        }

    }
}