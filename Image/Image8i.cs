using System;

namespace Xevle.Imaging.Image
{
	/// <summary>
	/// Image class for 8bit integer images
	/// </summary>
	public class Image8i: IImage
	{
		#region Private variables
		/// <summary>
		/// Holds the image data
		/// </summary>
		byte[] imageData;
		#endregion

		#region Public properties
		/// <summary>
		/// Gets the channel format.
		/// </summary>
		/// <value>The channel format.</value>
		public ChannelFormat ChannelFormat { get; private set; }

		/// <summary>
		/// Gets the width.
		/// </summary>
		/// <value>The width.</value>
		public uint Width { get; private set; }

		/// <summary>
		/// Gets the height.
		/// </summary>
		/// <value>The height.</value>
		public uint Height { get; private set; }
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="Xevle.Imaging.Image.Image8i"/> class.
		/// </summary>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <param name="format">Format.</param>
		public Image8i(uint width, uint height, ChannelFormat format = ChannelFormat.RGB)
		{
			Width = width;
			Height = height;

			ChannelFormat = format;

			imageData = null;
	
			if (width * height > 0) imageData = new byte[width * height * GetBytePerPixelFromChannelFormat(format)];
		}
		#endregion

		#region Channel converter
		/// <summary>
		/// Converts to
		/// </summary>
		/// <returns>The to.</returns>
		/// <param name="trgformat">Trgformat.</param>
		public Image8i ConvertTo(ChannelFormat channelFormat)
		{
			switch (channelFormat)
			{
				case ChannelFormat.GRAY:
					{
						return ConvertToGray();
					}
				case ChannelFormat.GRAYAlpha:
					{
						return ConvertToGrayAlpha();
					}
				case ChannelFormat.RGB:
					{
						return ConvertToRGB();
					}
				case ChannelFormat.RGBA:
					{
						return ConvertToRGBA();
					}
				case ChannelFormat.BGR:
					{
						return ConvertToBGR();
					}
				case ChannelFormat.BGRA:
					{
						return ConvertToBGRA();
					}
			}

			return null;
		}

		/// <summary>
		/// Converts to gray.
		/// </summary>
		/// <returns>The to gray.</returns>
		public Image8i ConvertToGray()
		{
			if (ChannelFormat == ChannelFormat.GRAY) return this;
			Image8i ret = new Image8i(Width, Height, ChannelFormat.GRAY);
			if (ret.imageData == null) return ret;

			uint count = Width * Height;
			byte[] src = imageData;
			uint ind = 0;
			byte[] dst = ret.imageData;
			uint inds = 0;

			switch (ChannelFormat)
			{
				case ChannelFormat.GRAYAlpha:
					{
						for (uint i = 0; i < count; i++)
						{
							dst[ind++] = src[inds++];
							inds++;
						}
						break;
					}
				case ChannelFormat.BGR:
					{
						for (uint i = 0; i < count; i++)
						{
							byte r = src[inds++];
							byte g = src[inds++];
							byte b = src[inds++];
							dst[ind++] = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
						}
						break;
					}
				case ChannelFormat.RGB:
					{
						for (uint i = 0; i < count; i++)
						{
							byte b = src[inds++];
							byte g = src[inds++];
							byte r = src[inds++];
							dst[ind++] = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
						}
						break;
					}
				case ChannelFormat.BGRA:
					{
						for (uint i = 0; i < count; i++)
						{
							byte r = src[inds++];
							byte g = src[inds++];
							byte b = src[inds++];
							inds++;
							dst[ind++] = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
						}
						break;
					}
				case ChannelFormat.RGBA:
					{
						for (uint i = 0; i < count; i++)
						{
							byte b = src[inds++];
							byte g = src[inds++];
							byte r = src[inds++];
							inds++;
							dst[ind++] = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
						}
						break;
					}
			}

			return ret;
		}

		/// <summary>
		/// Converts to gray alpha.
		/// </summary>
		/// <returns>The to gray alpha.</returns>
		public Image8i ConvertToGrayAlpha()
		{
			if (ChannelFormat == ChannelFormat.GRAYAlpha) return this;
			Image8i ret = new Image8i(Width, Height, ChannelFormat.GRAYAlpha);
			if (ret.imageData == null) return ret;

			uint count = Width * Height;
			byte[] src = imageData;
			uint ind = 0;
			byte[] dst = ret.imageData;
			uint inds = 0;

			switch (ChannelFormat)
			{
				case ChannelFormat.GRAY:
					{
						for (uint i = 0; i < count; i++)
						{
							dst[ind++] = src[inds++];
							dst[ind++] = 255;
						}
						break;
					}
				case ChannelFormat.BGR:
					{
						for (uint i = 0; i < count; i++)
						{
							byte r = src[inds++];
							byte g = src[inds++];
							byte b = src[inds++];
							dst[ind++] = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
							dst[ind++] = 255;
						}
						break;
					}
				case ChannelFormat.RGB:
					{
						for (uint i = 0; i < count; i++)
						{
							byte b = src[inds++];
							byte g = src[inds++];
							byte r = src[inds++];
							dst[ind++] = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
							dst[ind++] = 255;
						}
						break;
					}
				case ChannelFormat.BGRA:
					{
						for (uint i = 0; i < count; i++)
						{
							byte r = src[inds++];
							byte g = src[inds++];
							byte b = src[inds++];
							dst[ind++] = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
							dst[ind++] = src[inds++];
						}
						break;
					}
				case ChannelFormat.RGBA:
					{
						for (uint i = 0; i < count; i++)
						{
							byte b = src[inds++];
							byte g = src[inds++];
							byte r = src[inds++];
							dst[ind++] = (byte)(0.299 * r + 0.587 * g + 0.114 * b);
							dst[ind++] = src[inds++];
						}
						break;
					}
			}

			return ret;
		}

		/// <summary>
		/// Converts to RGB.
		/// </summary>
		/// <returns>The to RG.</returns>
		public Image8i ConvertToRGB()
		{
			if (ChannelFormat == ChannelFormat.RGB) return this;
			Image8i ret = new Image8i(Width, Height, ChannelFormat.RGB);
			if (ret.imageData == null) return ret;

			uint count = Width * Height;
			byte[] src = imageData;
			uint ind = 0;
			byte[] dst = ret.imageData;
			uint inds = 0;

			switch (ChannelFormat)
			{
				case ChannelFormat.GRAY:
					{
						for (uint i = 0; i < count; i++)
						{
							byte g = src[inds++];
							dst[ind++] = g;
							dst[ind++] = g;
							dst[ind++] = g;
						}
						break;
					}
				case ChannelFormat.GRAYAlpha:
					{
						for (uint i = 0; i < count; i++)
						{
							byte g = src[inds++];
							dst[ind++] = g;
							dst[ind++] = g;
							dst[ind++] = g;
							inds++;
						}
						break;
					}
				case ChannelFormat.BGR:
					{
						for (uint i = 0; i < count; i++)
						{
							byte b = src[inds++];
							byte g = src[inds++];
							dst[ind++] = src[inds++];
							dst[ind++] = g;
							dst[ind++] = b;
						}
						break;
					}
				case ChannelFormat.BGRA:
					{
						for (uint i = 0; i < count; i++)
						{
							byte b = src[inds++];
							byte g = src[inds++];
							dst[ind++] = src[inds++];
							dst[ind++] = g;
							dst[ind++] = b;
							inds++;
						}
						break;
					}
				case ChannelFormat.RGBA:
					{
						for (uint i = 0; i < count; i++)
						{
							dst[ind++] = src[inds++];
							dst[ind++] = src[inds++];
							dst[ind++] = src[inds++];
							inds++;
						}
						break;
					}
			}

			return ret;
		}

		/// <summary>
		/// Converts to RGBA
		/// </summary>
		/// <returns>The to RGB.</returns>
		public Image8i ConvertToRGBA()
		{
			if (ChannelFormat == ChannelFormat.RGBA) return this;
			Image8i ret = new Image8i(Width, Height, ChannelFormat.RGBA);
			if (ret.imageData == null) return ret;

			uint count = Width * Height;
			byte[] src = imageData;
			uint ind = 0;
			byte[] dst = ret.imageData;
			uint inds = 0;

			switch (ChannelFormat)
			{
				case ChannelFormat.GRAY:
					{
						for (uint i = 0; i < count; i++)
						{
							byte g = src[inds++];
							dst[ind++] = g;
							dst[ind++] = g;
							dst[ind++] = g;
							dst[ind++] = 255;
						}
						break;
					}
				case ChannelFormat.GRAYAlpha:
					{
						for (uint i = 0; i < count; i++)
						{
							byte g = src[inds++];
							dst[ind++] = g;
							dst[ind++] = g;
							dst[ind++] = g;
							dst[ind++] = src[inds++];
						}
						break;
					}
				case ChannelFormat.BGR:
					{
						for (uint i = 0; i < count; i++)
						{
							byte b = src[inds++];
							byte g = src[inds++];
							dst[ind++] = src[inds++];
							dst[ind++] = g;
							dst[ind++] = b;
							dst[ind++] = 255;
						}
						break;
					}
				case ChannelFormat.BGRA:
					{
						for (uint i = 0; i < count; i++)
						{
							byte b = src[inds++];
							byte g = src[inds++];
							dst[ind++] = src[inds++];
							dst[ind++] = g;
							dst[ind++] = b;
							dst[ind++] = src[inds++];
						}
						break;
					}
				case ChannelFormat.RGB:
					{
						for (uint i = 0; i < count; i++)
						{
							dst[ind++] = src[inds++];
							dst[ind++] = src[inds++];
							dst[ind++] = src[inds++];
							dst[ind++] = 255;
						}
						break;
					}
			}
			return ret;
		}

		/// <summary>
		/// Converts to BGR.
		/// </summary>
		/// <returns>The to background.</returns>
		public Image8i ConvertToBGR()
		{
			if (ChannelFormat == ChannelFormat.BGR) return this;
			Image8i ret = new Image8i(Width, Height, ChannelFormat.BGR);
			if (ret.imageData == null) return ret;

			uint count = Width * Height;
			byte[] src = imageData;
			uint ind = 0;
			byte[] dst = ret.imageData;
			uint inds = 0;

			switch (ChannelFormat)
			{
				case ChannelFormat.GRAY:
					{
						for (uint i = 0; i < count; i++)
						{
							byte g = src[inds++];
							dst[ind++] = g;
							dst[ind++] = g;
							dst[ind++] = g;
						}
						break;
					}
				case ChannelFormat.GRAYAlpha:
					{
						for (uint i = 0; i < count; i++)
						{
							byte g = src[inds++];
							dst[ind++] = g;
							dst[ind++] = g;
							dst[ind++] = g;
							inds++;
						}
						break;
					}
				case ChannelFormat.RGB:
					{
						for (uint i = 0; i < count; i++)
						{
							byte r = src[inds++];
							byte g = src[inds++];
							dst[ind++] = src[inds++];
							dst[ind++] = g;
							dst[ind++] = r;
						}
						break;
					}
				case ChannelFormat.RGBA:
					{
						for (uint i = 0; i < count; i++)
						{
							byte r = src[inds++];
							byte g = src[inds++];
							dst[ind++] = src[inds++];
							dst[ind++] = g;
							dst[ind++] = r;
							inds++;
						}
						break;
					}
				case ChannelFormat.BGRA:
					{
						for (uint i = 0; i < count; i++)
						{
							dst[ind++] = src[inds++];
							dst[ind++] = src[inds++];
							dst[ind++] = src[inds++];
							inds++;
						}
						break;
					}
			}
			return ret;
		}

		/// <summary>
		/// Converts ToBGR
		/// </summary>
		/// <returns>The to BGR.</returns>
		public Image8i ConvertToBGRA()
		{
			if (ChannelFormat == ChannelFormat.BGRA) return this;
			Image8i ret = new Image8i(Width, Height, ChannelFormat.BGRA);
			if (ret.imageData == null) return ret;

			uint count = Width * Height;
			byte[] src = imageData;
			uint ind = 0;
			byte[] dst = ret.imageData;
			uint inds = 0;
			switch (ChannelFormat)
			{
				case ChannelFormat.GRAY:
					{
						for (uint i = 0; i < count; i++)
						{
							byte g = src[inds++];
							dst[ind++] = g;
							dst[ind++] = g;
							dst[ind++] = g;
							dst[ind++] = 255;
						}
						break;
					}
				case ChannelFormat.GRAYAlpha:
					{
						for (uint i = 0; i < count; i++)
						{
							byte g = src[inds++];
							dst[ind++] = g;
							dst[ind++] = g;
							dst[ind++] = g;
							dst[ind++] = src[inds++];
						}
						break;
					}
				case ChannelFormat.RGB:
					{
						for (uint i = 0; i < count; i++)
						{
							byte r = src[inds++];
							byte g = src[inds++];
							dst[ind++] = src[inds++];
							dst[ind++] = g;
							dst[ind++] = r;
							dst[ind++] = 255;
						}
						break;
					}
				case ChannelFormat.RGBA:
					{
						for (uint i = 0; i < count; i++)
						{
							byte r = src[inds++];
							byte g = src[inds++];
							dst[ind++] = src[inds++];
							dst[ind++] = g;
							dst[ind++] = r;
							dst[ind++] = src[inds++];
						}
						break;
					}
				case ChannelFormat.BGR:
					{
						for (uint i = 0; i < count; i++)
						{
							dst[ind++] = src[inds++];
							dst[ind++] = src[inds++];
							dst[ind++] = src[inds++];
							dst[ind++] = 255;
						}
						break;
					}
			}

			return ret;
		}

		/// <summary>
		/// Converts to black white.
		/// </summary>
		/// <returns>The to black white.</returns>
		/// <param name="threshold">Threshold.</param>
		public Image8i ConvertToBlackWhite(byte threshold)
		{
			Image8i ret = ConvertToGray();
			if (ret.imageData == null) return ret;

			uint count = Width * Height;
			for (uint i = 0; i < count; i++) ret.imageData[i] = (ret.imageData[i] < threshold) ? (byte)0 : (byte)255;

			return ret;
		}
		#endregion

		#region Drawing methods
		/// <summary>
		/// Draw the specified x, y and source.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="source">Source.</param>
		public void Draw(int x, int y, Image8i source)
		{
			if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException("x or y", "Out of image.");
			if (x + source.Width < 0 || y + source.Height < 0) throw new ArgumentOutOfRangeException("x or y", "Out of image.");

			Image8i srcimg = source.ConvertTo(ChannelFormat);
			if (srcimg == null) return;

			uint bytePerPixel = GetBytePerPixelFromChannelFormat(ChannelFormat);

			unsafe
			{
				fixed(byte* src_=srcimg.imageData, dst_=imageData)
				{
					uint start = (uint)System.Math.Max(-x, 0) * bytePerPixel;
					uint end = (uint)System.Math.Min(source.Width, Width - x) * bytePerPixel;

					uint jstart = (uint)System.Math.Max(-y, 0);
					uint jend = (uint)System.Math.Min(source.Height, Height - y);

					byte* src__ = src_ + start;
					byte* dst__ = dst_ + x * bytePerPixel + start;

					uint sw = source.Width * bytePerPixel;
					uint dw = Width * bytePerPixel;

					for (uint j = jstart; j < jend; j++)
					{
						byte* src = src__ + sw * j;
						byte* dst = dst__ + dw * (y + j);

						for (uint i = start; i < end; i++) *dst++ = *src++;
					}
				}
			}
		}

		/// <summary>
		/// Draw the specified x, y, source and considerAlpha.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="source">Source.</param>
		/// <param name="considerAlpha">If set to <c>true</c> consider alpha.</param>
		public void Draw(int x, int y, Image8i source, bool considerAlpha)
		{
			if (!considerAlpha || source.ChannelFormat == ChannelFormat.BGR ||
			    source.ChannelFormat == ChannelFormat.RGB || source.ChannelFormat == ChannelFormat.GRAY)
			{
				Draw(x, y, source);
				return;
			}

			if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException("x or y", "Out of image.");
			if (x + source.Width < 0 || y + source.Height < 0) throw new ArgumentOutOfRangeException("x or y", "Out of image.");

			Image8i sourceImage = null;

			switch (ChannelFormat)
			{
				case ChannelFormat.GRAY:
					{
						sourceImage = source.ConvertToGrayAlpha();
						break;
					}
				case ChannelFormat.GRAYAlpha:
					{
						sourceImage = source.ConvertToGrayAlpha();
						break;
					}
				case ChannelFormat.RGB:
					{
						sourceImage = source.ConvertToRGBA();
						break;
					}
				case ChannelFormat.RGBA:
					{
						sourceImage = source.ConvertToRGBA();
						break;
					}
				case ChannelFormat.BGR:
					{
						sourceImage = source.ConvertToBGRA();
						break;
					}
				case ChannelFormat.BGRA:
					{
						sourceImage = source.ConvertToBGRA();
						break;
					}
			}

			uint bpp = GetBytePerPixelFromChannelFormat(source.ChannelFormat);

			unsafe
			{
				fixed(byte* src_=sourceImage.imageData, dst_=imageData)
				{
					uint start = (uint)System.Math.Max(-x, 0);
					uint end = (uint)System.Math.Min(source.Width, Width - x);

					uint jstart = (uint)System.Math.Max(-y, 0);
					uint jend = (uint)System.Math.Min(source.Height, Height - y);

					if (ChannelFormat == ChannelFormat.BGR || ChannelFormat == ChannelFormat.RGB || ChannelFormat == ChannelFormat.GRAY)
					{
						uint dbpp = GetBytePerPixelFromChannelFormat(ChannelFormat);

						byte* src__ = src_ + start * bpp;
						byte* dst__ = dst_ + x * dbpp + start * dbpp;

						uint sw = source.Width * bpp;
						uint dw = Width * dbpp;

						if (ChannelFormat == ChannelFormat.BGR || ChannelFormat == ChannelFormat.RGB)
						{
							for (uint j = jstart; j < jend; j++)
							{
								byte* src = src__ + sw * j;
								byte* dst = dst__ + dw * (y + j);

								for (uint i = start; i < end; i++)
								{
									byte sr = *src++;
									byte sg = *src++;
									byte sb = *src++;
									byte sa = *src++;

									if (sa != 0)
									{
										byte dr = *dst++;
										byte dg = *dst++;
										byte db = *dst++;
										dst -= 3;

										double a2 = sa / 255.0;
										double a1 = 1 - a2;
										*dst++ = (byte)(dr * a1 + sr * a2);
										*dst++ = (byte)(dg * a1 + sg * a2);
										*dst++ = (byte)(db * a1 + sb * a2);
									}
									else dst += 3;
								}
							}
						}
						else // GRAY
						{
							for (uint j = jstart; j < jend; j++)
							{
								byte* src = src__ + sw * j;
								byte* dst = dst__ + dw * (y + j);

								for (uint i = start; i < end; i++)
								{
									byte sg = *src++;
									byte sa = *src++;

									if (sa != 0)
									{
										byte dg = *dst;

										double a2 = sa / 255.0;
										double a1 = 1 - a2;
										*dst++ = (byte)(dg * a1 + sg * a2);
									}
									else dst++;
								}
							}
						} // end if RGB || BGR
					}
					else // 2x alpha image
					{
						byte* src__ = src_ + start * bpp;
						byte* dst__ = dst_ + x * bpp + start * bpp;

						uint sw = source.Width * bpp;
						uint dw = Width * bpp;

						if (ChannelFormat == ChannelFormat.BGRA || ChannelFormat == ChannelFormat.RGBA)
						{
							for (uint j = jstart; j < jend; j++)
							{
								byte* src = src__ + sw * j;
								byte* dst = dst__ + dw * (y + j);

								for (uint i = start; i < end; i++)
								{
									byte sr = *src++;
									byte sg = *src++;
									byte sb = *src++;
									byte sa = *src++;

									if (sa != 0)
									{
										byte dr = *dst++;
										byte dg = *dst++;
										byte db = *dst++;
										byte da = *dst++;
										dst -= 4;

										double a2 = sa / 255.0;
										double a1 = 1 - a2;
										*dst++ = (byte)(dr * a1 + sr * a2);
										*dst++ = (byte)(dg * a1 + sg * a2);
										*dst++ = (byte)(db * a1 + sb * a2);
										*dst++ = da;
									}
									else dst += 4;
								}
							}
						}
						else // GRAYALPHA
						{
							for (uint j = jstart; j < jend; j++)
							{
								byte* src = src__ + sw * j;
								byte* dst = dst__ + dw * (y + j);

								for (uint i = start; i < end; i++)
								{
									byte sg = *src++;
									byte sa = *src++;

									if (sa != 0)
									{
										byte dg = *dst++;
										byte da = *dst++;
										dst -= 2;

										double a2 = sa / 255.0;
										double a1 = 1 - a2;
										*dst++ = (byte)(dg * a1 + sg * a2);
										*dst++ = da;
									}
									else dst += 2;
								}
							}
						} // end if RGBA || BGRA
					} // end if 2x Alpha-Bild
				} // fixed
			} // unsafe
		}
		#endregion

		#region GetImage and GetSubImage methods
		/// <summary>
		/// Gets the image.
		/// </summary>
		/// <returns>The image.</returns>
		public Image8i GetImage()
		{
			return GetSubImage(0, 0, Width, Height);
		}

		/// <summary>
		/// Gets a subimage from the image.
		/// </summary>
		/// <returns>The sub image.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="w">The width.</param>
		/// <param name="h">The height.</param>
		public Image8i GetSubImage(uint x, uint y, uint w, uint h)
		{
			if (x >= Width || y >= Height) throw new ArgumentOutOfRangeException("x or y", "Out of image.");

			Image8i ret = new Image8i(w, h, ChannelFormat);
			ret.Draw(-(int)x, -(int)y, this);
			return ret;
		}
		#endregion

		#region Helper methods
		/// <summary>
		/// Gets the byte per pixel from channel format.
		/// </summary>
		/// <returns>The byte per pixel from channel format.</returns>
		/// <param name="format">Format.</param>
		public uint GetBytePerPixelFromChannelFormat(ChannelFormat format)
		{
			switch (format)
			{
				case ChannelFormat.GRAY:
					{
						return 1;
					}
				case ChannelFormat.GRAYAlpha:
					{
						return 2;
					}
				case ChannelFormat.RGB:
					{
						return 3;
					}
				case ChannelFormat.BGR:
					{
						return 3;
					}
				case ChannelFormat.RGBA:
					{
						return 4;
					}
				case ChannelFormat.BGRA:
					{
						return 4;
					}
				default:
					{
						return 1;
					}
			}
		}
		#endregion

		#region Pixel methods
		/// <summary>
		/// Gets the pixel.
		/// </summary>
		/// <returns>The pixel.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public Color8i GetPixel(uint x, uint y)
		{
			if (x >= Width || y >= Height) return new Color8i();

			ulong pos = y * Width + x;
			pos *= GetBytePerPixelFromChannelFormat(ChannelFormat);

			switch (ChannelFormat)
			{
				case ChannelFormat.GRAY:
					{
						byte color = imageData[pos];
						return new Color8i(color, color, color);
					}
				case ChannelFormat.GRAYAlpha:
					{
						byte color = imageData[pos];
						return new Color8i(imageData[pos + 1], color, color, color);
					}
				case ChannelFormat.BGR:
					{
						byte r = imageData[pos];
						byte g = imageData[pos + 1];
						byte b = imageData[pos + 2];
						return new Color8i(r, g, b);
					}
				case ChannelFormat.BGRA:
					{
						byte r = imageData[pos];
						byte g = imageData[pos + 1];
						byte b = imageData[pos + 2];
						byte a = imageData[pos + 3];
						return new Color8i(r, g, b, a);
					}
				case ChannelFormat.RGB:
					{
						byte b = imageData[pos];
						byte g = imageData[pos + 1];
						byte r = imageData[pos + 2];
						return new Color8i(r, g, b);
					}
				case ChannelFormat.RGBA:
					{
						byte b = imageData[pos];
						byte g = imageData[pos + 1];
						byte r = imageData[pos + 2];
						byte a = imageData[pos + 3];
						return new Color8i(a, r, g, b);
					}
			}

			return new Color8i();
		}

		/// <summary>
		/// Sets the pixel.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">Color.</param>
		public void SetPixel(uint x, uint y, Color8i color)
		{
			if (x >= Width || y >= Height) return;

			ulong pos = y * Width + x;
			pos *= GetBytePerPixelFromChannelFormat(ChannelFormat);

			switch (ChannelFormat)
			{
				case ChannelFormat.GRAY:
					{
						imageData[pos] = color.R;
						break;
					}
				case ChannelFormat.GRAYAlpha:
					{
						imageData[pos] = color.R;
						imageData[pos + 1] = color.A;
						break;
					}
				case ChannelFormat.BGR:
					{
						imageData[pos] = color.R;
						imageData[pos + 1] = color.G;
						imageData[pos + 2] = color.B;
						break;
					}
				case ChannelFormat.BGRA:
					{
						imageData[pos] = color.R;
						imageData[pos + 1] = color.G;
						imageData[pos + 2] = color.B;
						imageData[pos + 3] = color.A;
						break;
					}
				case ChannelFormat.RGB:
					{
						imageData[pos] = color.B;
						imageData[pos + 1] = color.G;
						imageData[pos + 2] = color.R;
						break;
					}
				case ChannelFormat.RGBA:
					{
						imageData[pos] = color.B;
						imageData[pos + 1] = color.G;
						imageData[pos + 2] = color.R;
						imageData[pos + 3] = color.A;
						break;
					}
			}
		}
		#endregion
	}
}