using System;
using Xevle.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Xevle.Imaging.Image.Formats
{
	public static class FormatAdapter
	{
		public static IImage FromFile(string filename)
		{
			if (filename.Length < 5) return null;
			
			string extension = Paths.GetExtension(filename).ToLower();

			if (extension == "bmp") return WindowsBitmap.FromFile(filename);
			if (extension == "tga") return TargaImage.FromFile(filename);
			
			return FromFileFramework(filename);
		}

		public static IImage FromFileFramework(string filename)
		{
			System.Drawing.Image img = System.Drawing.Image.FromFile(filename);
			uint width = (uint)img.Size.Width;
			uint height = (uint)img.Size.Height;

			Image8i ret = null;

			if ((img.PixelFormat & PixelFormat.Alpha) == PixelFormat.Alpha)
			{
				ret = new Image8i(width, height, ChannelFormat.RGBA);

				Bitmap bmp = new Bitmap(img);
				BitmapData data = bmp.LockBits(new Rectangle(0, 0, (int)width, (int)height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
				Marshal.Copy(data.Scan0, ret.ImageData, 0, (int)(width * height * 4));
				bmp.UnlockBits(data);
			}
			else
			{
				ret = new Image8i(width, height, ChannelFormat.RGB);

				Bitmap bmp = new Bitmap(img);
				BitmapData data = bmp.LockBits(new Rectangle(0, 0, (int)width, (int)height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
				if (((int)width * 3) == data.Stride)
				{
					Marshal.Copy(data.Scan0, ret.ImageData, 0, (int)(width * height * 3));
				}
				else
				{
					if (IntPtr.Size == 4)
					{
						for (uint i = 0; i < height; i++)
						{
							Marshal.Copy((IntPtr)(data.Scan0.ToInt32() + (int)(i * data.Stride)), ret.ImageData, (int)(width * 3 * i), (int)(width * 3));
						}
					}
					else if (IntPtr.Size == 8)
					{
						for (uint i = 0; i < height; i++)
						{
							Marshal.Copy((IntPtr)(data.Scan0.ToInt64() + (long)(i * data.Stride)), ret.ImageData, (int)(width * 3 * i), (int)(width * 3));
						}
					}
				}

				bmp.UnlockBits(data);
				data = null;
				bmp.Dispose();
				bmp = null;
			}

			img.Dispose();
			img = null;

			return ret;
		}
	}
}