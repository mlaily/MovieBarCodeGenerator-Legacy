using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MovieBarCode
{
	public class ParallelGeneration
	{

		public string InputPath { get; protected set; }
		public string OutputPath { get; protected set; }
		public int TotalIterations { get; protected set; }
		public int BarWidth { get; protected set; }
		public int Width { get; protected set; }
		public int Height { get; protected set; }

		protected bool locked = false;

		private int completedIterations;
		/// <summary>
		/// range: 0.0-1.0
		/// </summary>
		public double Progression
		{
			get { return (double)completedIterations / (double)TotalIterations; }
		}
		/// <summary>
		/// progression, reported by working threads, in frames
		/// (total = iterations)
		/// </summary>

		public event EventHandler GenerationComplete;
		public event EventHandler<ProgressCHangedEventHandler> ProgressChanged;

		public class ProgressCHangedEventHandler : EventArgs
		{
			public double Progression { get; protected set; }

			public int Percentage { get { return (int)(Progression * 100.0); } }

			public ProgressCHangedEventHandler(double progression)
			{
				this.Progression = progression;
			}
		}

		protected class ThreadParameters
		{
			public int ThreadId { get; set; }
			public int StartPoint { get; set; }
			public int EndPoint { get; set; }
		}

		public ParallelGeneration(string inputPath, string outputPath, int width, int height, int iterations, int barWidth)
		{
			this.InputPath = inputPath;
			this.OutputPath = outputPath;
			this.Width = width;
			this.Height = height;
			this.TotalIterations = iterations;
			this.BarWidth = barWidth;
		}

		private void Core(object objectArgs)
		{
			ThreadParameters args = objectArgs as ThreadParameters;
			if (args == null)
			{
				throw new ArgumentNullException();
			}
			int sliceWidth = this.Width / Environment.ProcessorCount;
			Bitmap slice = new Bitmap(sliceWidth, this.Height);
			System.Drawing.Graphics g = Graphics.FromImage(slice);
			VideoHelper v = new VideoHelper(this.InputPath);
			for (int i = args.StartPoint; i < args.EndPoint; i++)
			{
				Bitmap frame = v.GetFrameFromVideo(((double)i) / (double)this.TotalIterations);
				g.DrawImage(frame, (i - args.StartPoint) * this.BarWidth, 0, this.BarWidth, this.Height);
				if (i % 10 == 0)
				{
					//once every 10 iteration should not be too much
					//but is enough to keep the memory footprint as low as possible
					//(only cosmetic change ?)
					GC.Collect();
				}
				completedIterations++;
				if (this.ProgressChanged != null)
				{
					this.ProgressChanged(this, new ProgressCHangedEventHandler(this.Progression));
				}
			}
			v.Dispose();
			ThreadedSlices.Add(args.ThreadId, slice);
		}

		/// <summary>
		/// stock temporary bitmap computed in working threads (must be accessed in a thread safe way),
		/// each bitmap is associated with its thread number (in the right order)
		/// </summary>
		protected Dictionary<int, Bitmap> ThreadedSlices = null;

		public void GenerateMovieBarCode()
		{
			if (locked)
			{
				throw new InvalidOperationException();
			}
			locked = true;
			completedIterations = 0;
			//multithread : on calcul séparement x bitmap qu'on réassemble à la fin
			//VideoHelper v = new VideoHelper(videoPath);
			try
			{
				Bitmap finalBitmap = new Bitmap(this.Width, this.Height);
				ThreadedSlices = new Dictionary<int, Bitmap>();
#if DEBUG
				DateTime start = DateTime.Now;
#endif
				List<System.Threading.Thread> threads = new List<System.Threading.Thread>();
				//distribute work load
				for (int i = 0; i < Environment.ProcessorCount; i++)
				{
					/*
					 * example:
					 * 125 images
					 * 3 threads:
					 * 0-41
					 * 41-82
					 * 82-123
					 * remains : 123-125
					 */
					ThreadParameters args = new ThreadParameters()
					{
						ThreadId = i,
						StartPoint = i * (this.TotalIterations / Environment.ProcessorCount),
						EndPoint = (i + 1) * (this.TotalIterations / Environment.ProcessorCount),
					};
					System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Core));
					t.Name = string.Format("Core {0}", i + 1);
					threads.Add(t);
					t.Start(args);
				}
				int remaining = this.TotalIterations % Environment.ProcessorCount;
				if (remaining > 0)
				{
					//start a last thread for the remaining frames
					ThreadParameters args = new ThreadParameters()
					{
						ThreadId = Environment.ProcessorCount,
						StartPoint = Environment.ProcessorCount * (this.TotalIterations / Environment.ProcessorCount),
						EndPoint = Environment.ProcessorCount * (this.TotalIterations / Environment.ProcessorCount) + remaining,
					};
					System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(Core));
					t.Name = string.Format("Core {0}", Environment.ProcessorCount + 1);
					threads.Add(t);
					t.Start(args);
				}
				//wait for each thread to complete
				foreach (var item in threads)
				{
					item.Join();
				}
				System.Drawing.Graphics g = Graphics.FromImage(finalBitmap);
				foreach (var slice in ThreadedSlices)
				{
					if (slice.Key == Environment.ProcessorCount)
					{
						//remaining
						g.DrawImage(slice.Value, new Point(Environment.ProcessorCount * (this.TotalIterations / Environment.ProcessorCount) + remaining, 0));
					}
					else
					{
						g.DrawImage(slice.Value, new Point(slice.Key * (this.TotalIterations / Environment.ProcessorCount), 0));
					}
#if DEBUG
					slice.Value.Save(string.Format(@"C:\{0:000}.jpg", slice.Key));
#endif
				}
#if DEBUG
				DateTime end = DateTime.Now;
				var total = end - start;
				Console.WriteLine(total);
#endif
				System.Drawing.Imaging.ImageFormat format;
				switch (System.IO.Path.GetExtension(this.OutputPath).Trim(".".ToCharArray()).ToLowerInvariant())
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
				finalBitmap.Save(this.OutputPath, format);
				if (GenerationComplete != null)
				{
					GenerationComplete(this, new EventArgs());
				}
			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				locked = false;
			}
		}
	}
}
