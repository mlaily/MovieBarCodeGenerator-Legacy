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

        public struct ThreadParameters
        {
            public int threadNumber;
            public int start;
            public int stop;
            public int totalIterations;
            public string vPath;
            public int barWidth;
            public int width;
            public int height;
        }

        private void GenerationCore(object objectArgs)
        {
            ThreadParameters args = (ThreadParameters)objectArgs;
            Bitmap slice = new Bitmap(args.width, args.height);
            System.Drawing.Graphics g = Graphics.FromImage(slice);
            VideoHelper v = new VideoHelper(args.vPath);
            for (int i = args.start; i < args.stop; i++)
            {
                Bitmap frame = v.GetFrameFromVideo(((double)i) / (double)args.totalIterations);
                g.DrawImage(frame, (i-args.start) * args.barWidth, 0, args.barWidth, args.height);
                if (i % 10 == 0)
                {
                    //once every 10 iteration should not be too much
                    //but is enough to keep the memory footprint as low as possible
                    //(almost only cosmetic change)
                    GC.Collect();
                }
                progression++;
                //progressBar1.PerformStep();
                //Application.DoEvents();
            }
            v.Dispose();
            ThreadedSlices.Add(args.threadNumber, slice);
        }
        /// <summary>
        /// stock temporary bitmap computed in working threads (must be accessed in a thread safe way),
        /// each bitmap is associated with its thread number (in the right order)
        /// </summary>
        private Dictionary<int, Bitmap> ThreadedSlices = null;
        /// <summary>
        /// progression, reported by working threads, in frames
        /// (total = iterations)
        /// </summary>
        private int progression = 0;
        private void GenerateMovieBarCode(string videoPath, int width, int height, string outputPath, int iterations, int barWidth)
        {
            //multithread : on calcul séparement x bitmap qu'on réassemble à la fin
            progressBar1.Value = progressBar1.Minimum;
            progressBar1.Maximum = iterations;
            //VideoHelper v = new VideoHelper(videoPath);
            Bitmap finalBitmap = new Bitmap(width, height);
            System.Drawing.Graphics g = Graphics.FromImage(finalBitmap);
            ThreadedSlices = new Dictionary<int, Bitmap>();
#if DEBUG
            DateTime start = DateTime.Now;
#endif
            List<System.Threading.Thread> threads = new List<System.Threading.Thread>();
            for (int i = 0; i < Environment.ProcessorCount; i++)
            {
                ThreadParameters args = new ThreadParameters();
                args.threadNumber = i;
                args.start = i * (iterations / Environment.ProcessorCount);
                args.stop = (i + 1) * (iterations / Environment.ProcessorCount);
                args.totalIterations = iterations;
                args.vPath = videoPath;
                args.barWidth = barWidth;
                args.width = width / Environment.ProcessorCount;//must handle modulo if not a multiple of processorCount
                args.height = height;
                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(GenerationCore));
                t.Name = string.Format("Core {0}",i+1);
                threads.Add(t);
                t.Start(args);
            }
            foreach (var item in threads)
            {
                item.Join();
            }
            foreach (var slice in ThreadedSlices)
            {
                g.DrawImage(slice.Value, new Point(slice.Key * (iterations / Environment.ProcessorCount), 0));
                slice.Value.Save(string.Format(@"C:\{0:000}.jpg",slice.Key));
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
            finalBitmap.Save(outputPath, format);
            //v.Dispose();
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