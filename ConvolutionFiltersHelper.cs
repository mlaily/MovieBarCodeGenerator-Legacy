using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace MovieBarCode
{
    class ConvolutionFiltersHelper
    {
        public static bool Blur(ref Bitmap b, int intensity = 1)
        {
            int step0 = intensity;
            int step1 = intensity / 2;
            int step2 = intensity / 4;
            ConvMatrix blur = new ConvMatrix();
            //ConvMatrix blur = new ConvMatrix(
            //    new int[,] {
            //    { step2, step1, step2 },
            //    { step1, step0, step1 },
            //    { step2, step1, step2 }
            //    });
            //blur.Factor = intensity * intensity;
            blur.SetAll(1);
            blur.Pixel = intensity;
            blur.Factor = intensity + 8;
            return Conv3x3(ref b, blur);
        }

        public class ConvMatrix
        {
            public int TopLeft = 0, TopMid = 0, TopRight = 0;
            public int MidLeft = 0, Pixel = 1, MidRight = 0;
            public int BottomLeft = 0, BottomMid = 0, BottomRight = 0;
            public int Factor = 1;
            public int Offset = 0;
            public void SetAll(int nVal)
            {
                TopLeft = TopMid = TopRight = MidLeft = Pixel = MidRight =
                          BottomLeft = BottomMid = BottomRight = nVal;
            }
            public ConvMatrix(int[,] matrix3x3)
            {
                if (matrix3x3.GetLength(0) != matrix3x3.GetLength(1) || matrix3x3.GetLength(0) != 3)
                {
                    throw new ArgumentException("need a 3x3 matrix!");
                }
                TopLeft = matrix3x3[0, 0];
                TopMid = matrix3x3[1, 0];
                TopRight = matrix3x3[2, 0];
                MidLeft = matrix3x3[0, 1];
                Pixel = matrix3x3[2, 1];
                MidRight = matrix3x3[2, 2];
                BottomLeft = matrix3x3[0, 2];
                BottomMid = matrix3x3[1, 2];
                BottomRight = matrix3x3[2, 2];
                
            }
            public ConvMatrix()
            {

            }
        }

        public static bool Conv3x3(ref Bitmap b, ConvMatrix m)
        {
            // Avoid divide by zero errors

            if (0 == m.Factor)
                return false; Bitmap

            // GDI+ still lies to us - the return format is BGR, NOT RGB. 

            bSrc = (Bitmap)b.Clone();
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                                ImageLockMode.ReadWrite,
                                PixelFormat.Format24bppRgb);
            BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height),
                               ImageLockMode.ReadWrite,
                               PixelFormat.Format24bppRgb);
            int stride = bmData.Stride;
            int stride2 = stride * 2;

            System.IntPtr Scan0 = bmData.Scan0;
            System.IntPtr SrcScan0 = bmSrc.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                byte* pSrc = (byte*)(void*)SrcScan0;
                int nOffset = stride - b.Width * 3;
                int nWidth = b.Width - 2;
                int nHeight = b.Height - 2;

                int nPixel;

                for (int y = 0; y < nHeight; ++y)
                {
                    for (int x = 0; x < nWidth; ++x)
                    {
                        nPixel = ((((pSrc[2] * m.TopLeft) +
                            (pSrc[5] * m.TopMid) +
                            (pSrc[8] * m.TopRight) +
                            (pSrc[2 + stride] * m.MidLeft) +
                            (pSrc[5 + stride] * m.Pixel) +
                            (pSrc[8 + stride] * m.MidRight) +
                            (pSrc[2 + stride2] * m.BottomLeft) +
                            (pSrc[5 + stride2] * m.BottomMid) +
                            (pSrc[8 + stride2] * m.BottomRight))
                            / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;
                        p[5 + stride] = (byte)nPixel;

                        nPixel = ((((pSrc[1] * m.TopLeft) +
                            (pSrc[4] * m.TopMid) +
                            (pSrc[7] * m.TopRight) +
                            (pSrc[1 + stride] * m.MidLeft) +
                            (pSrc[4 + stride] * m.Pixel) +
                            (pSrc[7 + stride] * m.MidRight) +
                            (pSrc[1 + stride2] * m.BottomLeft) +
                            (pSrc[4 + stride2] * m.BottomMid) +
                            (pSrc[7 + stride2] * m.BottomRight))
                            / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;
                        p[4 + stride] = (byte)nPixel;

                        nPixel = ((((pSrc[0] * m.TopLeft) +
                                       (pSrc[3] * m.TopMid) +
                                       (pSrc[6] * m.TopRight) +
                                       (pSrc[0 + stride] * m.MidLeft) +
                                       (pSrc[3 + stride] * m.Pixel) +
                                       (pSrc[6 + stride] * m.MidRight) +
                                       (pSrc[0 + stride2] * m.BottomLeft) +
                                       (pSrc[3 + stride2] * m.BottomMid) +
                                       (pSrc[6 + stride2] * m.BottomRight))
                            / m.Factor) + m.Offset);

                        if (nPixel < 0) nPixel = 0;
                        if (nPixel > 255) nPixel = 255;
                        p[3 + stride] = (byte)nPixel;

                        p += 3;
                        pSrc += 3;
                    }

                    p += nOffset;
                    pSrc += nOffset;
                }
            }

            b.UnlockBits(bmData);
            bSrc.UnlockBits(bmSrc);
            return true;
        }
    }
}
