using System;
using System.IO;

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
		/// <param name="imageData">Image data.</param>
		public Image8i(uint width, uint height, ChannelFormat format = ChannelFormat.RGB, byte[] imageData = null)
		{
			Width = width;
			Height = height;

			ChannelFormat = format;

			this.imageData = imageData;
	
			if (width * height > 0) imageData = new byte[width * height * GetBytePerPixelFromChannelFormat(format)];
		}
		#endregion

		#region Channel converter
		/// <summary>
		/// Converts to.
		/// </summary>
		/// <returns>The to.</returns>
		/// <param name="channelFormat">Channel format.</param>
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

		#region Channel swap methods
		/// <summary>
		/// Swaps the channels to BRG.
		/// </summary>
		/// <returns>The channels to BR.</returns>
		public Image8i SwapChannelsToBRG()
		{
			if (ChannelFormat != ChannelFormat.RGB) return this.ConvertToRGB().SwapChannelsToBRG();
			Image8i ret = new Image8i(Width, Height, ChannelFormat.RGB);
			if (ret.imageData == null) return ret;

			uint count = Width * Height;
			byte[] src = imageData;
			uint ind = 0;
			byte[] dst = ret.imageData;
			uint inds = 0;

			// Channel Swap RGB
			for (uint i = 0; i < count; i++)
			{
				byte r = src[inds++];
				byte g = src[inds++];
				byte b = src[inds++];
				dst[ind++] = b;
				dst[ind++] = r;
				dst[ind++] = g;
			}

			return ret;
		}

		/// <summary>
		/// Swaps the channels to RBB
		/// </summary>
		/// <returns>The channels to RB.</returns>
		public Image8i SwapChannelsToRBG()
		{
			if (ChannelFormat != ChannelFormat.RGB) return this.ConvertToRGB().SwapChannelsToBRG();
			Image8i ret = new Image8i(Width, Height, ChannelFormat.RGB);
			if (ret.imageData == null) return ret;

			uint count = Width * Height;
			byte[] src = imageData;
			uint ind = 0;
			byte[] dst = ret.imageData;
			uint inds = 0;

			//Channel Swap RGB
			for (uint i = 0; i < count; i++)
			{
				byte r = src[inds++];
				byte g = src[inds++];
				byte b = src[inds++];
				dst[ind++] = r;
				dst[ind++] = b;
				dst[ind++] = g;
			}

			return ret;
		}

		/// <summary>
		/// Swaps the channels to GRB.
		/// </summary>
		/// <returns>The channels to GR.</returns>
		public Image8i SwapChannelsToGRB()
		{
			if (ChannelFormat != ChannelFormat.RGB) return this.ConvertToRGB().SwapChannelsToBRG();
			Image8i ret = new Image8i(Width, Height, ChannelFormat.RGB);
			if (ret.imageData == null) return ret;

			uint count = Width * Height;
			byte[] src = imageData;
			uint ind = 0;
			byte[] dst = ret.imageData;
			uint inds = 0;

			//Channel Swap RGB
			for (uint i = 0; i < count; i++)
			{
				byte r = src[inds++];
				byte g = src[inds++];
				byte b = src[inds++];
				dst[ind++] = g;
				dst[ind++] = r;
				dst[ind++] = b;
			}

			return ret;
		}

		/// <summary>
		/// Swaps the channels to BGR.
		/// </summary>
		/// <returns>The channels to background.</returns>
		public Image8i SwapChannelsToBGR()
		{
			if (ChannelFormat != ChannelFormat.RGB) return this.ConvertToRGB().SwapChannelsToBGR();
			Image8i ret = new Image8i(Width, Height, ChannelFormat.RGB);
			if (ret.imageData == null) return ret;

			uint count = Width * Height;
			byte[] src = imageData;
			uint ind = 0;
			byte[] dst = ret.imageData;
			uint inds = 0;

			// Channel Swap RGB
			for (uint i = 0; i < count; i++)
			{
				byte r = src[inds++];
				byte g = src[inds++];
				byte b = src[inds++];
				dst[ind++] = b;
				dst[ind++] = g;
				dst[ind++] = r;
			}

			return ret;
		}

		/// <summary>
		/// Swaps the channels to GBR;
		/// </summary>
		/// <returns>The channels to GB.</returns>
		public Image8i SwapChannelsToGBR()
		{
			if (ChannelFormat != ChannelFormat.RGB) return this.ConvertToRGB().SwapChannelsToBRG();
			Image8i ret = new Image8i(Width, Height, ChannelFormat.RGB);
			if (ret.imageData == null) return ret;

			uint count = Width * Height;
			byte[] src = imageData;
			uint ind = 0;
			byte[] dst = ret.imageData;
			uint inds = 0;

			// Channel Swap RGB
			for (uint i = 0; i < count; i++)
			{
				byte r = src[inds++];
				byte g = src[inds++];
				byte b = src[inds++];
				dst[ind++] = g;
				dst[ind++] = b;
				dst[ind++] = r;
			}

			return ret;
		}
		#endregion

		#region Downsampling
		public Image8i Downsample(uint w, uint h)
		{
			if (Width < w || Height < h) throw new Exception("Don't upsample an image using 'Downsample()'");

			if (Width == w && Height == h) return this;
			if ((Width * Height) == 0) return new Image8i(0, 0, ChannelFormat);
			if ((w * h) == 0) return new Image8i(0, 0, ChannelFormat);

			if (Width != w)
			{
				if (Height != h)
				{
					if ((Height * w) > (Width * h)) return DownsampleVertical(h).DownsampleHorizontal(w);
					return DownsampleHorizontal(w).DownsampleVertical(h);
				}
				return DownsampleHorizontal(w);
			}

			return DownsampleVertical(h);
		}

		unsafe Image8i DownsampleVertical(uint h)
		{
			if ((Width * Height) == 0) return new Image8i(0, 0, ChannelFormat);
			if (h == 0) return new Image8i(0, 0, ChannelFormat);

			if (Height > h && Height % h == 0) return ReduceByVertical(Height / h);
			double delta = ((double)Height) / h;

			if (ChannelFormat == ChannelFormat.GRAY)
			{
				Image8i ret = new Image8i(Width, h, ChannelFormat);
				if (ret.imageData == null) return ret;

				for (uint x = 0; x < Width; x++)
				{
					fixed(byte* _dst=ret.imageData, _src=imageData)
					{
						byte* dst = _dst + x;
						byte* src = _src + x;

						for (uint y = 0; y < h; y++)
						{
							double deltay = y * delta;
							double dy = 1 - (deltay - ((uint)deltay));
							byte* s = src + ((uint)deltay) * Width;
							double deltasum = dy;

							double gsum = *s * dy;
							s += Width;

							while ((delta - deltasum) > 0.0001)
							{
								dy = delta - deltasum;
								if (dy >= 1)
								{
									deltasum += 1;
									gsum += *s;
									s += Width;
								}
								else
								{
									gsum += *s * dy;
									break;
								}
							}

							*dst = (byte)(gsum / delta + 0.5);
							dst += Width;
						}
					}
				}
				return ret;
			}

			if (ChannelFormat == ChannelFormat.RGB || ChannelFormat == ChannelFormat.BGR)
			{
				Image8i ret = new Image8i(Width, h, ChannelFormat);
				if (ret.imageData == null) return ret;

				int wb = (int)Width * 3;
				int wb2 = wb - 2;

				for (uint x = 0; x < Width; x++)
				{
					fixed(byte* _dst=ret.imageData, _src=imageData)
					{
						byte* dst = _dst + x * 3;
						byte* src = _src + x * 3;

						for (uint y = 0; y < h; y++)
						{
							double deltay = y * delta;
							double dy = 1 - (deltay - ((uint)deltay));
							byte* s = src + ((uint)deltay) * wb;
							double deltasum = dy;

							double rsum = *(s++) * dy;
							double gsum = *(s++) * dy;
							double bsum = *s * dy;
							s += wb2;

							while ((delta - deltasum) > 0.0001)
							{
								dy = delta - deltasum;
								if (dy >= 1)
								{
									deltasum += 1;
									rsum += *(s++);
									gsum += *(s++);
									bsum += *s;
									s += wb2;
								}
								else
								{
									rsum += *(s++) * dy;
									gsum += *(s++) * dy;
									bsum += *s * dy;
									break;
								}
							}

							*(dst++) = (byte)(rsum / delta + 0.5);
							*(dst++) = (byte)(gsum / delta + 0.5);
							*dst = (byte)(bsum / delta + 0.5);
							dst += wb2;
						}
					}
				}
				return ret;
			}

			if (ChannelFormat == ChannelFormat.RGBA || ChannelFormat == ChannelFormat.BGRA)
			{
				Image8i ret = new Image8i(Width, h, ChannelFormat);
				if (ret.imageData == null) return ret;

				int wb = (int)Width * 4;
				int wb3 = wb - 3;

				for (uint x = 0; x < Width; x++)
				{
					fixed(byte* _dst=ret.imageData, _src=imageData)
					{
						byte* dst = _dst + x * 4;
						byte* src = _src + x * 4;

						for (uint y = 0; y < h; y++)
						{
							double deltay = y * delta;
							double dy = 1 - (deltay - ((uint)deltay));
							byte* s = src + ((uint)deltay) * wb;
							double deltasum = dy;

							byte r = *(s++), g = *(s++), b = *(s++);
							uint a = *s;
							s += wb3;

							double ady = a * dy;
							double rsum = r * ady;
							double gsum = g * ady;
							double bsum = b * ady;
							double asum = ady;

							while ((delta - deltasum) > 0.0001)
							{
								r = *(s++);
								g = *(s++);
								b = *(s++);
								a = *s;
								s += wb3;

								dy = delta - deltasum;
								if (dy >= 1)
								{
									deltasum += 1;
									rsum += r * a;
									gsum += g * a;
									bsum += b * a;
									asum += a;
								}
								else
								{
									ady = a * dy;
									rsum += r * ady;
									gsum += g * ady;
									bsum += b * ady;
									asum += ady;
									break;
								}
							}

							*(dst++) = (byte)(rsum / asum + 0.5);
							*(dst++) = (byte)(gsum / asum + 0.5);
							*(dst++) = (byte)(bsum / asum + 0.5);
							*dst = (byte)(asum / delta + 0.5);
							dst += wb3;
						}
					}
				}

				return ret;
			}

			if (ChannelFormat == ChannelFormat.GRAYAlpha)
			{
				Image8i ret = new Image8i(Width, h, ChannelFormat);
				if (ret.imageData == null) return ret;

				int wb = (int)Width * 2;
				int wb1 = wb - 1;

				for (uint x = 0; x < Width; x++)
				{
					fixed(byte* _dst=ret.imageData, _src=imageData)
					{
						byte* dst = _dst + x * 2;
						byte* src = _src + x * 2;

						for (uint y = 0; y < h; y++)
						{
							double deltay = y * delta;
							double dy = 1 - (deltay - ((uint)deltay));
							byte* s = src + ((uint)deltay) * wb;
							double deltasum = dy;

							byte g = *(s++);
							uint a = *s;
							s += wb1;

							double ady = a * dy;
							double gsum = g * ady;
							double asum = ady;

							while ((delta - deltasum) > 0.0001)
							{
								g = *(s++);
								a = *s;
								s += wb1;

								dy = delta - deltasum;
								if (dy >= 1)
								{
									deltasum += 1;
									gsum += g * a;
									asum += a;
								}
								else
								{
									ady = a * dy;
									gsum += g * ady;
									asum += ady;
									break;
								}
							}

							*(dst++) = (byte)(gsum / asum + 0.5);
							*dst = (byte)(asum / delta + 0.5);
							dst += wb1;
						}
					}
				}

				return ret;
			}

			return new Image8i(0, 0, ChannelFormat);
		}

		unsafe Image8i DownsampleHorizontal(uint w)
		{
			if ((Width * Height) == 0) return new Image8i(0, 0, ChannelFormat);
			if (w == 0) return new Image8i(0, 0, ChannelFormat);

			if (Width > w && Width % w == 0) return ReduceByHorizontal(Width / w);

			double delta = ((double)Width) / w;

			if (ChannelFormat == ChannelFormat.GRAY)
			{
				Image8i ret = new Image8i(w, Height, ChannelFormat);
				if (ret.imageData == null) return ret;

				fixed(byte* _dst=ret.imageData, _src=imageData)
				{
					byte* dst = _dst;

					for (uint y = 0; y < Height; y++)
					{
						byte* src = _src + y * Width;

						for (uint x = 0; x < w; x++)
						{
							double deltax = x * delta;
							double dx = 1 - (deltax - ((uint)deltax));
							byte* s = src + ((uint)deltax);
							double deltasum = dx;

							double gsum = *(s++) * dx;

							while ((delta - deltasum) > 0.0001)
							{
								dx = delta - deltasum;
								if (dx >= 1)
								{
									deltasum += 1;
									gsum += *(s++);
								}
								else
								{
									gsum += *s * dx;
									break;
								}
							}

							*(dst++) = (byte)(gsum / delta + 0.5);
						}
					}
				}

				return ret;
			}

			if (ChannelFormat == ChannelFormat.RGB || ChannelFormat == ChannelFormat.BGR)
			{
				Image8i ret = new Image8i(w, Height, ChannelFormat);
				if (ret.imageData == null) return ret;

				fixed(byte* _dst=ret.imageData, _src=imageData)
				{
					byte* dst = _dst;
					uint wb = Width * 3;

					for (uint y = 0; y < Height; y++)
					{
						byte* src = _src + y * wb;

						for (uint x = 0; x < w; x++)
						{
							double deltax = x * delta;
							double dx = 1 - (deltax - ((uint)deltax));
							byte* s = src + ((uint)deltax) * 3;
							double deltasum = dx;

							double rsum = *(s++) * dx;
							double gsum = *(s++) * dx;
							double bsum = *(s++) * dx;

							while ((delta - deltasum) > 0.0001)
							{
								dx = delta - deltasum;
								if (dx >= 1)
								{
									deltasum += 1;
									rsum += *(s++);
									gsum += *(s++);
									bsum += *(s++);
								}
								else
								{
									rsum += *(s++) * dx;
									gsum += *(s++) * dx;
									bsum += *s * dx;
									break;
								}
							}

							*(dst++) = (byte)(rsum / delta + 0.5);
							*(dst++) = (byte)(gsum / delta + 0.5);
							*(dst++) = (byte)(bsum / delta + 0.5);
						}
					}
				}

				return ret;
			}

			if (ChannelFormat == ChannelFormat.RGBA || ChannelFormat == ChannelFormat.BGRA)
			{
				Image8i ret = new Image8i(w, Height, ChannelFormat);
				if (ret.imageData == null) return ret;

				fixed(byte* _dst=ret.imageData, _src=imageData)
				{
					byte* dst = _dst;
					uint wb = Width * 4;

					for (uint y = 0; y < Height; y++)
					{
						byte* src = _src + y * wb;

						for (uint x = 0; x < w; x++)
						{
							double deltax = x * delta;
							double dx = 1 - (deltax - ((uint)deltax));
							byte* s = src + ((uint)deltax) * 4;
							double deltasum = dx;

							byte r = *(s++), g = *(s++), b = *(s++);
							uint a = *(s++);

							double adx = a * dx;
							double rsum = r * adx;
							double gsum = g * adx;
							double bsum = b * adx;
							double asum = adx;

							while ((delta - deltasum) > 0.0001)
							{
								dx = delta - deltasum;
								r = *(s++);
								g = *(s++);
								b = *(s++);
								a = *(s++);
								if (dx >= 1)
								{
									deltasum += 1;
									rsum += r * a;
									gsum += g * a;
									bsum += b * a;
									asum += a;
								}
								else
								{
									adx = a * dx;
									rsum += r * adx;
									gsum += g * adx;
									bsum += b * adx;
									asum += adx;
									break;
								}
							}

							*(dst++) = (byte)(rsum / asum + 0.5);
							*(dst++) = (byte)(gsum / asum + 0.5);
							*(dst++) = (byte)(bsum / asum + 0.5);
							*(dst++) = (byte)(asum / delta + 0.5);
						}
					}
				}

				return ret;
			}

			if (ChannelFormat == ChannelFormat.GRAYAlpha)
			{
				Image8i ret = new Image8i(w, Height, ChannelFormat);
				if (ret.imageData == null) return ret;

				fixed(byte* _dst=ret.imageData, _src=imageData)
				{
					byte* dst = _dst;
					uint wb = Width * 2;

					for (uint y = 0; y < Height; y++)
					{
						byte* src = _src + y * wb;

						for (uint x = 0; x < w; x++)
						{
							double deltax = x * delta;
							double dx = 1 - (deltax - ((uint)deltax));
							byte* s = src + ((uint)deltax) * 2;
							double deltasum = dx;

							byte g = *(s++);
							uint a = *(s++);

							double gsum = g * dx * a;
							double asum = a * dx;

							while ((delta - deltasum) > 0.0001)
							{
								dx = delta - deltasum;
								g = *(s++);
								a = *(s++);
								if (dx >= 1)
								{
									deltasum += 1;
									gsum += g * a;
									asum += a;
								}
								else
								{
									double adx = a * dx;
									gsum += g * adx;
									asum += adx;
									break;
								}
							}

							*(dst++) = (byte)(gsum / asum + 0.5);
							*(dst++) = (byte)(asum / delta + 0.5);
						}
					}
				}
				return ret;
			}

			return new Image8i(0, 0, ChannelFormat);
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

		#region NearestPixelResize
		public Image8i NearestPixelResize(uint w, uint h)
		{
			if (Width == w && Height == h) return this;
			if ((Width * Height) == 0) return new Image8i(0, 0, ChannelFormat);
			if ((w * h) == 0) return new Image8i(0, 0, ChannelFormat);

			if (Width == w) return NearestPixelResizeVertical(h);
			if (Height == h) return NearestPixelResizeHorizontal(w);
			return NearestPixelResizeVerticalHorizontal(w, h);
		}

		Image8i NearestPixelResizeVertical(uint h)
		{
			double delta = (double)Height / h;

			Image8i ret = new Image8i(Width, h, ChannelFormat);
			if (ret.imageData == null) return ret;

			uint bpp = GetBytePerPixelFromChannelFormat(ChannelFormat);
			uint bw = Width * bpp;

			uint dst = 0;

			for (uint y = 0; y < h; y++)
			{
				uint src = ((uint)(y * delta + delta / 2)) * bw;
				for (uint i = 0; i < bw; i++) ret.imageData[dst++] = imageData[src++];
			}

			return ret;
		}

		Image8i NearestPixelResizeHorizontal(uint w)
		{
			double delta = (double)Width / w;

			uint bpp = GetBytePerPixelFromChannelFormat(ChannelFormat);

			uint[] dx = new uint[w];
			for (uint x = 0; x < w; x++) dx[x] = (uint)(x * delta + delta / 2);

			if (bpp == 1)
			{
				Image8i ret = new Image8i(w, Height, ChannelFormat);
				if (ret.imageData == null) return ret;

				uint dst = 0;

				for (uint y = 0; y < Height; y++)
				{
					uint src = y * Width;
					for (uint x = 0; x < w; x++) ret.imageData[dst++] = imageData[src + dx[x]];
				}

				return ret;
			}

			if (bpp == 4)
			{
				Image8i ret = new Image8i(w, Height, ChannelFormat);
				if (ret.imageData == null) return ret;

				uint bw = Width * 4;
				uint dst = 0;

				for (uint y = 0; y < Height; y++)
				{
					uint src = y * bw;

					for (uint x = 0; x < w; x++)
					{
						uint s = src + dx[x] * 4;
						ret.imageData[dst++] = imageData[s++];
						ret.imageData[dst++] = imageData[s++];
						ret.imageData[dst++] = imageData[s++];
						ret.imageData[dst++] = imageData[s];
					}
				}

				return ret;
			}

			if (bpp == 3)
			{
				Image8i ret = new Image8i(w, Height, ChannelFormat);
				if (ret.imageData == null) return ret;

				uint bw = Width * 3;
				uint dst = 0;

				for (uint y = 0; y < Height; y++)
				{
					uint src = y * bw;

					for (uint x = 0; x < w; x++)
					{
						uint s = src + dx[x] * 3;
						ret.imageData[dst++] = imageData[s++];
						ret.imageData[dst++] = imageData[s++];
						ret.imageData[dst++] = imageData[s];
					}
				}

				return ret;
			}

			if (bpp == 2)
			{
				Image8i ret = new Image8i(w, Height, ChannelFormat);
				if (ret.imageData == null) return ret;

				uint bw = Width * 2;
				uint dst = 0;

				for (uint y = 0; y < Height; y++)
				{
					uint src = y * bw;

					for (uint x = 0; x < w; x++)
					{
						uint s = src + dx[x] * 2;
						ret.imageData[dst++] = imageData[s++];
						ret.imageData[dst++] = imageData[s];
					}
				}

				return ret;
			}

			return new Image8i(0, 0, ChannelFormat);
		}

		Image8i NearestPixelResizeVerticalHorizontal(uint w, uint h)
		{
			double deltah = (double)Height / h;
			double deltaw = (double)Width / w;

			uint bpp = GetBytePerPixelFromChannelFormat(ChannelFormat);

			uint[] dx = new uint[w];
			for (uint x = 0; x < w; x++) dx[x] = (uint)(x * deltaw + deltaw / 2);

			if (bpp == 1)
			{
				Image8i ret = new Image8i(w, h, ChannelFormat);
				if (ret.imageData == null) return ret;

				uint dst = 0;

				for (uint y = 0; y < h; y++)
				{
					uint src = (uint)(y * deltah + deltah / 2) * Width;
					for (uint x = 0; x < w; x++) ret.imageData[dst++] = imageData[src + dx[x]];
				}

				return ret;
			}

			if (bpp == 4)
			{
				Image8i ret = new Image8i(w, h, ChannelFormat);
				if (ret.imageData == null) return ret;

				uint dst = 0;
				uint w4 = Width * 4;

				for (uint y = 0; y < h; y++)
				{
					uint src = (uint)(y * deltah + deltah / 2) * w4;

					for (uint x = 0; x < w; x++)
					{
						uint s = src + dx[x] * 4;
						ret.imageData[dst++] = imageData[s++];
						ret.imageData[dst++] = imageData[s++];
						ret.imageData[dst++] = imageData[s++];
						ret.imageData[dst++] = imageData[s];
					}
				}

				return ret;
			}

			if (bpp == 3)
			{
				Image8i ret = new Image8i(w, h, ChannelFormat);
				if (ret.imageData == null) return ret;

				uint dst = 0;
				uint w3 = Width * 3;

				for (uint y = 0; y < h; y++)
				{
					uint src = (uint)(y * deltah + deltah / 2) * w3;

					for (uint x = 0; x < w; x++)
					{
						uint s = src + dx[x] * 3;
						ret.imageData[dst++] = imageData[s++];
						ret.imageData[dst++] = imageData[s++];
						ret.imageData[dst++] = imageData[s];
					}
				}

				return ret;
			}

			if (bpp == 2)
			{
				Image8i ret = new Image8i(w, h, ChannelFormat);
				if (ret.imageData == null) return ret;

				uint dst = 0;
				uint w2 = Width * 2;

				for (uint y = 0; y < h; y++)
				{
					uint src = (uint)(y * deltah + deltah / 2) * w2;

					for (uint x = 0; x < w; x++)
					{
						uint s = src + dx[x] * 2;
						ret.imageData[dst++] = imageData[s++];
						ret.imageData[dst++] = imageData[s];
					}
				}

				return ret;
			}

			return new Image8i(0, 0, ChannelFormat);
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

		#region ReduceByN
		public Image8i ReduceBy(uint m, uint n)
		{
			if (m == 1 && n == 1) return this;
			if ((m * n) == 0) return new Image8i(0, 0, ChannelFormat);
			if ((Width * Height) == 0) return new Image8i(0, 0, ChannelFormat);

			uint wr = Width % m, hr = Height % n;
			uint wm = Width / m, hn = Height / n;

			if (wm == 0) wm = 1;

			if (hn == 0) hn = 1;

			if (wr != 0)
			{
				if (hr != 0) return Downsample(wm, hn);
				if (n == 1) return DownsampleHorizontal(wm);
				return ReduceByVertical(n).DownsampleHorizontal(wm); // ReduceBy is usually faster, so ist down first
			}

			if (hr != 0)
			{
				if (m == 1) return DownsampleVertical(hn);
				return ReduceByHorizontal(m).DownsampleVertical(hn); // ReduceBy is usually faster, so ist down first
			}

			if (m == 1) return ReduceByVertical(n);
			if (n == 1) return ReduceByHorizontal(m);

			if ((hn * Width) > (wm * Height)) return ReduceByHorizontal(m).ReduceByVertical(n);
			return ReduceByVertical(n).ReduceByHorizontal(m);
		}

		public Image8i ReduceBy(uint m)
		{
			if (m == 1) return this;
			if (m == 0) return new Image8i(0, 0, ChannelFormat);
			if ((Width * Height) == 0) return new Image8i(0, 0, ChannelFormat);

			uint wr = Width % m, hr = Height % m;
			uint wm = Width / m, hn = Height / m;

			if (wm == 0) wm = 1;

			if (hn == 0) hn = 1;

			if (wr != 0)
			{
				if (hr != 0) return Downsample(wm, hn);
				if (m == 1) return DownsampleHorizontal(wm);
				return ReduceByVertical(m).DownsampleHorizontal(wm); // ReduceBy is usually faster, so ist down first
			}

			if (hr != 0)
			{
				if (m == 1) return DownsampleVertical(hn);
				return ReduceByHorizontal(m).DownsampleVertical(hn); // ReduceBy is usually faster, so ist down first
			}

			if ((hn * Width) > (wm * Height)) return ReduceByHorizontal(m).ReduceByVertical(m);
			return ReduceByVertical(m).ReduceByHorizontal(m);
		}

		Image8i ReduceByVertical(uint n)
		{
			if ((Width * Height) == 0) return new Image8i(0, 0, ChannelFormat);
			uint h = Height / n;

			if (ChannelFormat == ChannelFormat.GRAY)
			{
				Image8i ret = new Image8i(Width, h, ChannelFormat);
				if (ret.imageData == null) return ret;

				for (uint x = 0; x < Width; x++)
				{
					byte[] dst = ret.imageData;
					uint ind = x;
					byte[] src = imageData;
					uint inds = x;

					for (uint y = 0; y < h; y++)
					{
						uint sum = 0;
						for (uint z = 0; z < n; z++)
						{
							sum += src[inds];
							inds += Width;
						}

						dst[ind] = (byte)(sum / n);
						ind += Width;
					}
				}

				return ret;
			}

			if (ChannelFormat == ChannelFormat.RGB || ChannelFormat == ChannelFormat.BGR)
			{
				Image8i ret = new Image8i(Width, h, ChannelFormat);
				if (ret.imageData == null) return ret;

				uint wb = 3 * Width - 2;

				for (uint x = 0; x < Width; x++)
				{
					byte[] dst = ret.imageData;
					uint ind = x * 3;
					byte[] src = imageData;
					uint inds = x * 3;

					for (uint y = 0; y < h; y++)
					{
						uint sumr = 0, sumg = 0, sumb = 0;

						for (uint z = 0; z < n; z++)
						{
							sumr += src[inds++];
							sumg += src[inds++];
							sumb += src[inds];
							inds += wb;
						}

						dst[ind++] = (byte)(sumr / n);
						dst[ind++] = (byte)(sumg / n);
						dst[ind] = (byte)(sumb / n);
						ind += wb;
					}
				}

				return ret;
			}

			if (ChannelFormat == ChannelFormat.RGBA || ChannelFormat == ChannelFormat.BGRA)
			{
				Image8i ret = new Image8i(Width, h, ChannelFormat);
				if (ret.imageData == null) return ret;

				uint wb = 4 * Width - 3;

				for (uint x = 0; x < Width; x++)
				{
					byte[] dst = ret.imageData;
					uint ind = x * 4;
					byte[] src = imageData;
					uint inds = x * 4;

					for (uint y = 0; y < h; y++)
					{
						uint sumr = 0, sumg = 0, sumb = 0, suma = 0;

						for (uint z = 0; z < n; z++)
						{
							byte r = src[inds++];
							byte g = src[inds++];
							byte b = src[inds++];
							uint a = src[inds];
							inds += wb;
							sumr += r * a;
							sumg += g * a;
							sumb += b * a;
							suma += a;
						}

						if (suma == 0)
						{
							dst[ind++] = 0;
							dst[ind++] = 0;
							dst[ind++] = 0;
							dst[ind] = 0;
						}
						else
						{
							dst[ind++] = (byte)(sumr / suma);
							dst[ind++] = (byte)(sumg / suma);
							dst[ind++] = (byte)(sumb / suma);
							dst[ind] = (byte)(suma / n);
						}

						ind += wb;
					}
				}

				return ret;
			}

			if (ChannelFormat == ChannelFormat.GRAYAlpha)
			{
				Image8i ret = new Image8i(Width, h, ChannelFormat);
				if (ret.imageData == null) return ret;

				uint wb = 2 * Width - 1;

				for (uint x = 0; x < Width; x++)
				{
					byte[] dst = ret.imageData;
					uint ind = x * 2;
					byte[] src = imageData;
					uint inds = x * 2;

					for (uint y = 0; y < h; y++)
					{
						uint sumg = 0, suma = 0;

						for (uint z = 0; z < n; z++)
						{
							byte g = src[inds++];
							uint a = src[inds];
							inds += wb;
							sumg += g * a;
							suma += a;
						}

						if (suma == 0)
						{
							dst[ind++] = 0;
							dst[ind] = 0;
						}
						else
						{
							dst[ind++] = (byte)(sumg / suma);
							dst[ind] = (byte)(suma / n);
						}

						ind += wb;
					}
				}

				return ret;
			}

			return new Image8i(0, 0, ChannelFormat);
		}

		Image8i ReduceByHorizontal(uint m)
		{
			if ((Width * Height) == 0) return new Image8i(0, 0, ChannelFormat);
			uint w = Width / m;
			uint wh = w * Height;

			if (ChannelFormat == ChannelFormat.GRAY)
			{
				Image8i ret = new Image8i(w, Height, ChannelFormat);
				if (ret.imageData == null) return ret;

				byte[] dst = ret.imageData;
				uint ind = 0;
				byte[] src = imageData;
				uint inds = 0;

				for (uint y = 0; y < wh; y++)
				{
					uint sum = 0;
					for (uint z = 0; z < m; z++) sum += src[inds++];
					dst[ind++] = (byte)(sum / m);
				}

				return ret;
			}

			if (ChannelFormat == ChannelFormat.RGB || ChannelFormat == ChannelFormat.BGR)
			{
				Image8i ret = new Image8i(w, Height, ChannelFormat);
				if (ret.imageData == null) return ret;

				byte[] dst = ret.imageData;
				uint ind = 0;
				byte[] src = imageData;
				uint inds = 0;

				for (uint y = 0; y < wh; y++)
				{
					uint sumr = 0, sumg = 0, sumb = 0;

					for (uint z = 0; z < m; z++)
					{
						sumr += src[inds++];
						sumg += src[inds++];
						sumb += src[inds++];
					}

					dst[ind++] = (byte)(sumr / m);
					dst[ind++] = (byte)(sumg / m);
					dst[ind++] = (byte)(sumb / m);
				}

				return ret;
			}

			if (ChannelFormat == ChannelFormat.RGBA || ChannelFormat == ChannelFormat.BGRA)
			{
				Image8i ret = new Image8i(w, Height, ChannelFormat);
				if (ret.imageData == null) return ret;

				byte[] dst = ret.imageData;
				uint ind = 0;
				byte[] src = imageData;
				uint inds = 0;

				for (uint y = 0; y < wh; y++)
				{
					uint sumr = 0, sumg = 0, sumb = 0, suma = 0;

					for (uint z = 0; z < m; z++)
					{
						byte r = src[inds++];
						byte g = src[inds++];
						byte b = src[inds++];
						uint a = src[inds++];
						sumr += r * a;
						sumg += g * a;
						sumb += b * a;
						suma += a;
					}

					if (suma == 0)
					{
						dst[ind++] = 0;
						dst[ind++] = 0;
						dst[ind++] = 0;
						dst[ind++] = 0;
					}
					else
					{
						dst[ind++] = (byte)(sumr / suma);
						dst[ind++] = (byte)(sumg / suma);
						dst[ind++] = (byte)(sumb / suma);
						dst[ind++] = (byte)(suma / m);
					}
				}

				return ret;
			}

			if (ChannelFormat == ChannelFormat.GRAYAlpha)
			{
				Image8i ret = new Image8i(w, Height, ChannelFormat);
				if (ret.imageData == null) return ret;

				byte[] dst = ret.imageData;
				uint ind = 0;
				byte[] src = imageData;
				uint inds = 0;

				for (uint y = 0; y < wh; y++)
				{
					uint sumg = 0, suma = 0;

					for (uint z = 0; z < m; z++)
					{
						byte g = src[inds++];
						uint a = src[inds++];
						sumg += g * a;
						suma += a;
					}

					if (suma == 0)
					{
						dst[ind++] = 0;
						dst[ind++] = 0;
					}
					else
					{
						dst[ind++] = (byte)(sumg / suma);
						dst[ind++] = (byte)(suma / m);
					}
				}

				return ret;
			}

			return new Image8i(0, 0, ChannelFormat);
		}
		#endregion
	}
}