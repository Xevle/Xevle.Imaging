using System;
using Xevle.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;

namespace Xevle.Imaging.Image.Formats
{
	public static class FormatAdapter
	{
		#region FromFile
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
		#endregion

		#region FromStream
		public static IImage FromStreamFramework(Stream stream)
		{
			System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
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
		#endregion

		#region ToFile
		static ImageCodecInfo jpegImageCodecInfo = GetEncoderInfo("image/jpeg");

		static ImageCodecInfo GetEncoderInfo(string mimeType)
		{
			ImageCodecInfo[] encoders;
			encoders = ImageCodecInfo.GetImageEncoders();
			foreach (ImageCodecInfo encoder in encoders) if (encoder.MimeType == mimeType) return encoder;
			return null;
		}

		public static void SaveToFile(string filename, Image8i image)
		{
			string ext = Paths.GetExtension(filename).ToLower();

			switch (ext)
			{
				case "png":
					{
						SaveToPNG(filename, image);
						break;
					}
				case "jpg":
				case "jpeg":
				case "jpe":
				case "jif":
				case "jfi":
				case "jfif":
				case "psi":
				case "pmi":
					{
						SaveToJpeg(filename, -1, -1, image);
						break;
					}
				case "tga":
					{
						TargaImage.ToFile(filename, image);
						break;
					}
				case "bmp":
				case "dib":
					{
						WindowsBitmap.ToFile(filename, image);
						break;
					}
				case "tiff":
				case "tif":
					{
						SaveToTiff(filename, image);
						break;
					}
				default:
					{
						throw new NotSupportedException();
					}
			}
		}

		public static void SaveToJpeg(string filename, int exifWidth, int exifHeight, Image8i image)
		{
			if (image.ChannelFormat == ChannelFormat.BGR||image.ChannelFormat == ChannelFormat.GRAY)
			{
				SaveToJpeg(filename, exifWidth, exifHeight, image.ConvertToRGB());
				return;
			}

			if (image.ChannelFormat == ChannelFormat.BGRA||image.ChannelFormat == ChannelFormat.GRAYAlpha)
			{
				SaveToJpeg(filename, exifWidth, exifHeight, image.ConvertToRGBA());
				return;
			}

			Bitmap bmp = new Bitmap((int)image.Width, (int)image.Height, PixelFormat.Format24bppRgb);

			BitmapData data = bmp.LockBits(new Rectangle(0, 0, (int)image.Width, (int)image.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			if (((int)image.Width * 3) == data.Stride)
			{
				Marshal.Copy(image.ImageData, 0, data.Scan0, (int)(image.Width * image.Height * 3));
			}
			else
			{
				if (IntPtr.Size == 4)
				{
					for (uint i = 0; i < image.Height; i++)
					{
						Marshal.Copy(image.ImageData, (int)(image.Width * 3 * i), (IntPtr)(data.Scan0.ToInt32() + (int)(i * data.Stride)), (int)(image.Width * 3));
					}
				}
				else if (IntPtr.Size == 8)
				{
					for (uint i = 0; i < image.Height; i++)
					{
						Marshal.Copy(image.ImageData, (int)(image.Width * 3 * i), (IntPtr)(data.Scan0.ToInt64() + (long)(i * data.Stride)), (int)(image.Width * 3));
					}
				}
			}
			bmp.UnlockBits(data);
			data = null;

			if (exifWidth > 0 && exifHeight > 0)
			{
				PropertyItem o = (PropertyItem)Activator.CreateInstance(typeof(PropertyItem), true);

				byte[] buff = Encoding.ASCII.GetBytes("ASCII\0\0\0" + exifWidth + "x" + exifHeight);
				o.Id = 0x9286;
				o.Type = 7;
				o.Len = buff.Length;
				o.Value = buff;
				bmp.SetPropertyItem(o);

				o = (PropertyItem)Activator.CreateInstance(typeof(PropertyItem), true);
				o.Id = 0x100;
				o.Type = 4;
				o.Len = 4;
				o.Value = BitConverter.GetBytes((uint)exifWidth);
				bmp.SetPropertyItem(o);

				o = (PropertyItem)Activator.CreateInstance(typeof(PropertyItem), true);
				o.Id = 0x101;
				o.Type = 4;
				o.Len = 4;
				o.Value = BitConverter.GetBytes((uint)exifHeight);
				bmp.SetPropertyItem(o);
			}

			bmp.Save(filename, ImageFormat.Jpeg);
			bmp.Dispose();
			bmp = null;
		}

		public static void SaveToJpeg(string filename, int exifWidth, int exifHeight, byte quality, Image8i image)
		{
			if (image.ChannelFormat == ChannelFormat.BGR||image.ChannelFormat == ChannelFormat.GRAY)
			{
				SaveToJpeg(filename, exifWidth, exifHeight, quality, image.ConvertToRGB());
				return;
			}

			if (image.ChannelFormat == ChannelFormat.BGRA||image.ChannelFormat == ChannelFormat.GRAYAlpha)
			{
				SaveToJpeg(filename, exifWidth, exifHeight, image.ConvertToRGBA());
				return;
			}

			Bitmap bmp = new Bitmap((int)image.Width, (int)image.Height, PixelFormat.Format24bppRgb);

			BitmapData data = bmp.LockBits(new Rectangle(0, 0, (int)image.Width, (int)image.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

			if (((int)image.Width * 3) == data.Stride)
			{
				Marshal.Copy(image.ImageData, 0, data.Scan0, (int)(image.Width * image.Height * 3));
			}
			else
			{
				if (IntPtr.Size == 4)
				{
					for (uint i = 0; i < image.Height; i++)
					{
						Marshal.Copy(image.ImageData, (int)(image.Width * 3 * i), (IntPtr)(data.Scan0.ToInt32() + (int)(i * data.Stride)), (int)(image.Width * 3));
					}
				}
				else if (IntPtr.Size == 8)
				{
					for (uint i = 0; i < image.Height; i++)
					{
						Marshal.Copy(image.ImageData, (int)(image.Width * 3 * i), (IntPtr)(data.Scan0.ToInt64() + (long)(i * data.Stride)), (int)(image.Width * 3));
					}
				}
			}

			bmp.UnlockBits(data);
			data = null;

			if (exifWidth > 0 && exifHeight > 0)
			{
				PropertyItem o = (PropertyItem)Activator.CreateInstance(typeof(PropertyItem), true);

				byte[] buff = Encoding.ASCII.GetBytes("ASCII\0\0\0" + exifWidth + "x" + exifHeight);
				o.Id = 0x9286;
				o.Type = 7;
				o.Len = buff.Length;
				o.Value = buff;
				bmp.SetPropertyItem(o);

				o = (PropertyItem)Activator.CreateInstance(typeof(PropertyItem), true);
				o.Id = 0x100;
				o.Type = 4;
				o.Len = 4;
				o.Value = BitConverter.GetBytes((uint)exifWidth);
				bmp.SetPropertyItem(o);

				o = (PropertyItem)Activator.CreateInstance(typeof(PropertyItem), true);
				o.Id = 0x101;
				o.Type = 4;
				o.Len = 4;
				o.Value = BitConverter.GetBytes((uint)exifHeight);
				bmp.SetPropertyItem(o);
			}

			if (quality > 100) quality = 100;

			EncoderParameters encoderParameters = new EncoderParameters(1);
			encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)quality);
			bmp.Save(filename, jpegImageCodecInfo, encoderParameters);
			encoderParameters.Dispose();
			bmp.Dispose();
			bmp = null;
		}

		public static void SaveToPNG(string filename, Image8i image)
		{
			if (image.ChannelFormat == ChannelFormat.BGR||image.ChannelFormat == ChannelFormat.GRAY)
			{
				SaveToPNG(filename, image.ConvertToRGB());
				return;
			}

			if (image.ChannelFormat == ChannelFormat.BGRA||image.ChannelFormat == ChannelFormat.GRAYAlpha)
			{
				SaveToPNG(filename, image.ConvertToRGBA());
				return;
			}
				
			if (image.ChannelFormat == ChannelFormat.RGBA)
			{
				Bitmap bmp = new Bitmap((int)image.Width, (int)image.Height, PixelFormat.Format32bppArgb);
				BitmapData data = bmp.LockBits(new Rectangle(0, 0, (int)image.Width, (int)image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

				Marshal.Copy(image.ImageData, 0, data.Scan0, (int)(image.Width * image.Height * 4));

				bmp.UnlockBits(data);
				data = null;

				bmp.Save(filename, ImageFormat.Png);
				bmp.Dispose();
				bmp = null;
			}
			else
			{
				Bitmap bmp = new Bitmap((int)image.Width, (int)image.Height, PixelFormat.Format24bppRgb);
				BitmapData data = bmp.LockBits(new Rectangle(0, 0, (int)image.Width, (int)image.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

				if (((int)image.Width * 3) == data.Stride)
				{
					Marshal.Copy(image.ImageData, 0, data.Scan0, (int)(image.Width * image.Height * 3));
				}
				else
				{
					if (IntPtr.Size == 4)
					{
						for (uint i = 0; i < image.Height; i++)
						{
							Marshal.Copy(image.ImageData, (int)(image.Width * 3 * i), (IntPtr)(data.Scan0.ToInt32() + (int)(i * data.Stride)), (int)(image.Width * 3));
						}
					}
					else if (IntPtr.Size == 8)
					{
						for (uint i = 0; i < image.Height; i++)
						{
							Marshal.Copy(image.ImageData, (int)(image.Width * 3 * i), (IntPtr)(data.Scan0.ToInt64() + (long)(i * data.Stride)), (int)(image.Width * 3));
						}
					}
				}
				bmp.UnlockBits(data);
				data = null;

				bmp.Save(filename, ImageFormat.Png);
				bmp.Dispose();
				bmp = null;
			}
		}

		public static void SaveToTiff(string filename, Image8i image)
		{
			if (image.ChannelFormat == ChannelFormat.BGR||image.ChannelFormat == ChannelFormat.GRAY)
			{
				SaveToPNG(filename, image.ConvertToRGB());
				return;
			}

			if (image.ChannelFormat == ChannelFormat.BGRA||image.ChannelFormat == ChannelFormat.GRAYAlpha)
			{
				SaveToPNG(filename, image.ConvertToRGBA());
				return;
			}

			if (image.ChannelFormat == ChannelFormat.RGBA)
			{
				Bitmap bmp = new Bitmap((int)image.Width, (int)image.Height, PixelFormat.Format32bppArgb);
				BitmapData data = bmp.LockBits(new Rectangle(0, 0, (int)image.Width, (int)image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

				Marshal.Copy(image.ImageData, 0, data.Scan0, (int)(image.Width * image.Height * 4));

				bmp.UnlockBits(data);
				data = null;

				bmp.Save(filename, ImageFormat.Tiff);
				bmp.Dispose();
				bmp = null;
			}
			else
			{
				Bitmap bmp = new Bitmap((int)image.Width, (int)image.Height, PixelFormat.Format24bppRgb);
				BitmapData data = bmp.LockBits(new Rectangle(0, 0, (int)image.Width, (int)image.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

				if (((int)image.Width * 3) == data.Stride)
				{
					Marshal.Copy(image.ImageData, 0, data.Scan0, (int)(image.Width * image.Height * 3));
				}
				else
				{
					if (IntPtr.Size == 4)
					{
						for (uint i = 0; i < image.Height; i++)
						{
							Marshal.Copy(image.ImageData, (int)(image.Width * 3 * i), (IntPtr)(data.Scan0.ToInt32() + (int)(i * data.Stride)), (int)(image.Width * 3));
						}
					}
					else if (IntPtr.Size == 8)
					{
						for (uint i = 0; i < image.Height; i++)
						{
							Marshal.Copy(image.ImageData, (int)(image.Width * 3 * i), (IntPtr)(data.Scan0.ToInt64() + (long)(i * data.Stride)), (int)(image.Width * 3));
						}
					}
				}
				bmp.UnlockBits(data);
				data = null;

				bmp.Save(filename, ImageFormat.Tiff);
				bmp.Dispose();
				bmp = null;
			}
		}
		#endregion
	}
}