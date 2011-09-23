using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using DexterLib;
using System.Runtime.InteropServices;

namespace MovieBarCode
{
	public class VideoHelper : IDisposable
	{
		IMediaDet mediaDetInstance;
		_AMMediaType mediaTypeInstance;

		/// <summary>
		/// length in seconds of the current stream
		/// </summary>
		public double StreamLength
		{
			get
			{
				if (this._StreamLength == -42)
				{
					this._StreamLength = this.mediaDetInstance.StreamLength;
				}
				return this._StreamLength;
			}
		}
		private double _StreamLength = -42;

		/// <summary>
		/// Used for images extraction.
		/// The image will be scaled to the specified size. If Size.Empty is passed, the original video size will be used.
		/// /!\ Target size must be a multiple of 4! (will be resized to fit otherwise)
		/// </summary>
		public Size TargetSize
		{
			get
			{
				return this._TargetSize;
			}
			set
			{
				//calculates the REAL target size of our frame
				if (value == Size.Empty)
				{
					this._TargetSize = this.GetVideoSize();
				}
				else
				{
					this._TargetSize = Misc.scaleToFit(value, this.GetVideoSize());
					//ensures that the size is a multiple of 4 (required by the Bitmap constructor)
					this._TargetSize.Width -= this._TargetSize.Width % 4;
					this._TargetSize.Height -= this._TargetSize.Height % 4;
				}
			}
		}
		private Size _TargetSize = Size.Empty;

		public VideoHelper(string filePath)
		{
			if (!Misc.openVideoStream(filePath, out mediaDetInstance, out mediaTypeInstance))
			{
				throw new Exception(string.Format("Unable to load a valid video stream from file {0}.", filePath));
			}
			this._TargetSize = this.GetVideoSize();
		}

		/// <summary>
		/// Extracts a frame from videoFile at percentagePosition and returns it
		/// </summary>
		/// <param name="percentagePosition">Valid range is 0.0 .. 1.0</param>
		/// <param name="streamLength">will contain the length in seconds of the video stream</param>
		/// <returns>Bitmap of the extracted frame</returns>
		/// <exception cref="InvalidVideoFileException">thrown if the extraction fails</exception>
		/// <exception cref="ArgumentOutOfRangeException">thrown if an invalid percentagePosition is passed</exception>
		public Bitmap GetFrameFromVideo(double percentagePosition)
		{
			if (percentagePosition > 1 || percentagePosition < 0)
			{
				throw new ArgumentOutOfRangeException("percentagePosition", percentagePosition, "Valid range is 0.0 .. 1.0");
			}
			//if (target.Width % 4 != 0 || target.Height % 4 != 0)
			//    throw new ArgumentException("Target size must be a multiple of 4", "target");

			try
			{
				unsafe
				{
					Size s = this.GetVideoSize();
					int bmpinfoheaderSize = 40; //equals to sizeof(CommonClasses.BITMAPINFOHEADER);

					//get size for buffer
					int bufferSize = (((s.Width * s.Height) * 24) / 8) + bmpinfoheaderSize;	//equals to mediaDet.GetBitmapBits(0d, ref bufferSize, ref *buffer, target.Width, target.Height);	

					//allocates enough memory to store the frame
					IntPtr frameBuffer = System.Runtime.InteropServices.Marshal.AllocHGlobal(bufferSize);
					byte* frameBuffer2 = (byte*)frameBuffer.ToPointer();

					//gets bitmap, save in frameBuffer2
					this.mediaDetInstance.GetBitmapBits(this.StreamLength * percentagePosition, ref bufferSize, ref *frameBuffer2, this.TargetSize.Width, this.TargetSize.Height);

					//now in buffer2 we have a BITMAPINFOHEADER structure followed by the DIB bits

					Bitmap bmp = new Bitmap(this.TargetSize.Width, this.TargetSize.Height, this.TargetSize.Width * 3, System.Drawing.Imaging.PixelFormat.Format24bppRgb, new IntPtr(frameBuffer2 + bmpinfoheaderSize));

					bmp.RotateFlip(RotateFlipType.Rotate180FlipX);
					System.Runtime.InteropServices.Marshal.FreeHGlobal(frameBuffer);

					return bmp;
				}
			}
			catch (COMException ex)
			{
				throw new InvalidVideoFileException(Misc.getErrorMsg((uint)ex.ErrorCode), ex);
			}
		}

		/// <summary>
		/// Extracts a frame from videoFile at percentagePosition and saves it as .bmp in outputBitmapFile
		/// </summary>
		/// <param name="percentagePosition">Valid range is 0.0 .. 1.0</param>
		/// <param name="outputBitmapFilePath">Path to a file in which save the frame (bmp format)</param>		
		/// <exception cref="InvalidVideoFileException">thrown if the extraction fails</exception>
		/// <exception cref="ArgumentOutOfRangeException">thrown if an invalid percentagePosition is passed</exception>
		/// <returns>true for success, false for failure (no video stream, file not supported, ...)</returns>
		public void SaveFrameFromVideo(double percentagePosition, string outputBitmapFilePath)
		{
			if (percentagePosition > 1 || percentagePosition < 0)
			{
				throw new ArgumentOutOfRangeException("percentagePosition", percentagePosition, "Valid range is 0.0 .. 1.0");
			}

			try
			{
				this.mediaDetInstance.WriteBitmapBits(this.StreamLength * percentagePosition, this.TargetSize.Width, this.TargetSize.Height, outputBitmapFilePath);
			}
			catch (COMException ex)
			{
				throw new InvalidVideoFileException(Misc.getErrorMsg((uint)ex.ErrorCode), ex);
			}
		}

		/// <summary>
		/// Returns the Size of video frames
		/// </summary>
		/// <param name="videoFile">Path to the video file</param>
		/// <returns>Size of video. Size.Empty for failures (no video stream present, file not supported..)</returns>
		public Size GetVideoSize()
		{
			try
			{
				return Misc.getVideoSize(this.mediaTypeInstance);
			}
			catch
			{
				return Size.Empty;
			}
		}

		public void Dispose()
		{
			if (this.mediaDetInstance != null)
			{
				Marshal.ReleaseComObject(this.mediaDetInstance);
			}
		}
	}
}