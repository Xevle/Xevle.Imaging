using System;
using Xevle.Imaging.Image;
using System.IO;

namespace Xevle.Imaging.Image.Formats
{
	/// <summary>
	/// Reader and writer class for Targa Image
	/// </summary>
	public static class TargaImage
	{
		public static Image8i FromFile(string filename)
		{
			ChannelFormat format;
			byte[] imageData;

			uint width = 0;
			uint height = 0;

			using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{
				uint fsize = (uint)fs.Length;
				if (fsize < 19) throw new EndOfStreamException("File to small to be a Truevision Image.");

				BinaryReader br = new BinaryReader(fs);

				// Read header (18 bytes)
				byte idLength = br.ReadByte();
				byte colorMapType = br.ReadByte();
				byte imageType = br.ReadByte();
				br.ReadUInt16(); // first entry index
				ushort colorMapLength = br.ReadUInt16();
				byte colorMapEntrySize = br.ReadByte();
				br.ReadUInt16(); // x origin
				br.ReadUInt16(); // y origin
				ushort headerWidth = br.ReadUInt16();
				ushort headerHeight = br.ReadUInt16();
				byte pixelDepth = br.ReadByte();
				byte imageDescriptor = br.ReadByte();

				// Offsets
				uint imageDataOffset = 0;
				uint extensionAreaOffset = 0;
				uint scanLineTableOffset = 0;

				// Variables
				uint bitsPerPixel = 0;	// 8 | 16 | 24 | 32
				bool isAlpha = false;
				bool horizontalFlipped = false;
				bool verticalFlipped = false;
				bool rle = false;

				uint[] scanLineTable;

				imageDataOffset = 18u + idLength;

				if (colorMapType != 0)
				{
					if (colorMapLength == 0) throw new Exception("Color Map Type=1, but Length=0.");

					if (colorMapEntrySize == 15 || colorMapEntrySize == 16) imageDataOffset += colorMapLength * 2u;
					else if (colorMapEntrySize == 24) imageDataOffset += colorMapLength * 3u;
					else if (colorMapEntrySize == 32) imageDataOffset += colorMapLength * 4u;
					else throw new Exception("Illegal Color Map Entry Size.");
				}

				rle = false;

				switch (imageType)
				{
					case 10:
						{
							rle = true;

							if (pixelDepth == 16) bitsPerPixel = 2;
							else if (pixelDepth == 24) bitsPerPixel = 3;
							else if (pixelDepth == 32)
							{
								bitsPerPixel = 4;
								isAlpha = true;
							}
							else throw new InvalidDataException("Illegal or unsupported Pixel Depth (RGB).");

							break;
						}
					case 2:
						{
							if (pixelDepth == 16) bitsPerPixel = 2;
							else if (pixelDepth == 24) bitsPerPixel = 3;
							else if (pixelDepth == 32)
							{
								bitsPerPixel = 4;
								isAlpha = true;
							}
							else throw new InvalidDataException("Illegal or unsupported Pixel Depth (RGB).");

							break;
						}
					case 9:
						{
							rle = true;

							if (pixelDepth == 8) bitsPerPixel = 1;
							else if (pixelDepth == 16)
							{
								bitsPerPixel = 2;
								isAlpha = true;
							}
							else throw new InvalidDataException("Illegal or unsupported Pixel Depth (GRAY).");

							break;
						}
					case 11:
						{
							rle = true;

							if (pixelDepth == 8) bitsPerPixel = 1;
							else if (pixelDepth == 16)
							{
								bitsPerPixel = 2;
								isAlpha = true;
							}
							else throw new InvalidDataException("Illegal or unsupported Pixel Depth (GRAY).");

							break;
						}
					case 1:
						{
							if (pixelDepth == 8) bitsPerPixel = 1;
							else if (pixelDepth == 16)
							{
								bitsPerPixel = 2;
								isAlpha = true;
							}
							else throw new InvalidDataException("Illegal or unsupported Pixel Depth (GRAY).");

							break;
						}
					case 3:
						{
							if (pixelDepth == 8) bitsPerPixel = 1;
							else if (pixelDepth == 16)
							{
								bitsPerPixel = 2;
								isAlpha = true;
							}
							else throw new InvalidDataException("Illegal or unsupported Pixel Depth (GRAY).");

							break;
						}
					default:
						{
							throw new Exception("Illegal or unsupported Image Type.");
						}
				}

				// Check header width and height an set to image variables
				if (headerWidth == 0) throw new InvalidDataException("Illegal Image Width.");
				if (headerHeight == 0) throw new InvalidDataException("Illegal Image Height.");

				width = headerWidth;
				height = headerHeight;

				// Check image descriptor
				if ((imageDescriptor & 0x0F) != (isAlpha ? 8 : 0)) throw new Exception("Illegal Alpha Channel Bits Count.");
				if ((imageDescriptor & 0xC0) != 0) throw new Exception("Unsed Bits in Image Description not zero.");

				horizontalFlipped = (imageDescriptor & 0x20) != 0;
				verticalFlipped = (imageDescriptor & 0x10) != 0;

				// check offset
				if ((imageDataOffset + height) >= fsize) throw new Exception("File to small to fold the complete Image Data.");

				// check extensions
				fs.Seek(fsize - 26, SeekOrigin.Begin);
				extensionAreaOffset = br.ReadUInt32();
				br.ReadUInt32(); // Developer directory offset
				String sig = br.ReadChars(17).ToString();

				scanLineTable = new uint[height];

				if (sig == "TRUEVISION-XFILE.")	// check signature
				{
					if (extensionAreaOffset != 0)
					{
						fs.Seek(extensionAreaOffset, SeekOrigin.Begin);
						scanLineTableOffset = 0;
						ushort Extension_Size = br.ReadUInt16();

						if (Extension_Size >= 495)
						{
							fs.Seek(extensionAreaOffset + 490, SeekOrigin.Begin);
							scanLineTableOffset = br.ReadUInt32();
						}

						if (scanLineTableOffset != 0)
						{
							fs.Seek(scanLineTableOffset, SeekOrigin.Begin);
							for (int i = 0; i < height; i++) scanLineTable[i] = br.ReadUInt32();
						}
					}
				}
				else extensionAreaOffset = 0;

				if (scanLineTableOffset == 0)
				{
					if (!rle)
					{
						for (uint i = 0; i < height; i++) scanLineTable[i] = imageDataOffset + i * width * bitsPerPixel;
					}
					else
					{
						fs.Seek(imageDataOffset, SeekOrigin.Begin);

						for (uint i = 0; i < height; i++)
						{
							scanLineTable[i] = (uint)fs.Position;

							uint internalwidth = 0;
							while (width > internalwidth)
							{
								try
								{
									byte ph = br.ReadByte();
									uint count = (uint)((ph & 0x7F) + 1);

									if ((ph & 0x80) > 0) // rle packet
									{ 
										if (br.ReadBytes((int)bitsPerPixel).Length < bitsPerPixel) throw new Exception("Error reading rle-packed Image Data.");
									}
									else // raw packet
									{ 
										if (br.ReadBytes((int)(count * bitsPerPixel)).Length < (count * bitsPerPixel)) throw new Exception("Error reading rle-packed Image Data.");
									}

									internalwidth += count;
								}
								catch (Exception)
								{
									throw new EndOfStreamException("Error reading rle-packed Image Data.");
								}
							}

							if (internalwidth > width) throw new Exception("Error reading rle-packed Image Data. (Line too long.)");
						}
					}
				}

				uint bitsPerPixel_ = bitsPerPixel;

				if (!isAlpha && bitsPerPixel == 2) bitsPerPixel_ = 3; // RGB24 statt RGB15

				uint lineLen = width * bitsPerPixel_;

				imageData = new byte[height * lineLen];

				for (uint y = 0; y < height; y++)
				{
					uint p = y * lineLen;
					uint dest = p;

					uint end = width;

					if (verticalFlipped) dest -= (width + 1) * bitsPerPixel_;

					uint startOfLine;
					if (horizontalFlipped) startOfLine = scanLineTable[y];
					else startOfLine = scanLineTable[height - y - 1];

					if (!rle)
					{
						uint blocksize = end * bitsPerPixel;
						fs.Seek(startOfLine, SeekOrigin.Begin);
						byte[] buffer = br.ReadBytes((int)blocksize);
						if (buffer.Length < blocksize) throw new EndOfStreamException();

						int ind = 0;
						for (uint myX = 0; myX < end; myX++)
						{
							if (bitsPerPixel < 3)
							{
								if (bitsPerPixel == 1 || isAlpha)
								{
									imageData[dest++] = buffer[ind++];
									if (isAlpha) imageData[dest++] = buffer[ind++];
								}
								else
								{
									ushort w1 = buffer[ind++];
									ushort w2 = buffer[ind++];
									ushort w = (ushort)((w2 << 8) & w1);
									ushort r = (ushort)(w & 0x7c00);
									r >>= 7;
									ushort g = (ushort)(w & 0x03e0);
									g >>= 2;
									ushort b = (ushort)(w & 0x001f);
									b <<= 3;

									imageData[dest++] = (byte)b;
									imageData[dest++] = (byte)g;
									imageData[dest++] = (byte)r;
								}
							}
							else
							{
								imageData[dest++] = buffer[ind++];
								imageData[dest++] = buffer[ind++];
								imageData[dest++] = buffer[ind++];
								if (bitsPerPixel > 3) imageData[dest++] = buffer[ind++];
							}

							if (verticalFlipped) dest -= 2 * bitsPerPixel_;
						}
					}
					else
					{
						fs.Seek(startOfLine, SeekOrigin.Begin);

						byte[] buffer = new byte[width * bitsPerPixel];
						uint ind = 0;

						uint internalwidth = 0;
						while (width > internalwidth)
						{
							try
							{
								byte ph = br.ReadByte();
								uint count = (uint)((ph & 0x7F) + 1);
								if ((ph & 0x80) > 0) // rle packet
								{ 
									byte[] tbuffer = br.ReadBytes((int)bitsPerPixel);
									if (tbuffer.Length < bitsPerPixel) throw new Exception("Error reading rle-packed Image Data.");

									for (uint i = 0; i < count; i++)
									{
										tbuffer.CopyTo(buffer, ind);
										ind += bitsPerPixel;
									}
								}
								else // raw packet
								{ 
									byte[] tbuffer = br.ReadBytes((int)(count * bitsPerPixel));
									if (tbuffer.Length < (count * bitsPerPixel)) throw new Exception("Error reading rle-packed Image Data.");
									tbuffer.CopyTo(buffer, ind);
									ind += count * bitsPerPixel;
								}

								internalwidth += count;
							}
							catch (Exception)
							{
								throw new EndOfStreamException("Error reading rle-packed Image Data.");
							}
						}

						if (internalwidth > width) throw new EndOfStreamException("Error reading rle-packed Image Data. (Line too long.)");

						ind = 0;
						for (uint myX = 0; myX < end; myX++)
						{
							if (bitsPerPixel < 3)
							{ 
								if (bitsPerPixel == 1 || isAlpha)
								{
									imageData[dest++] = buffer[ind++];
									if (isAlpha) imageData[dest++] = buffer[ind++];
								}
								else
								{
									ushort w1 = buffer[ind++];
									ushort w2 = buffer[ind++];
									ushort w = (ushort)((w2 << 8) & w1);
									ushort r = (ushort)(w & 0x7c00);
									r >>= 7;
									ushort g = (ushort)(w & 0x03e0);
									g >>= 2;
									ushort b = (ushort)(w & 0x001f);
									b <<= 3;

									imageData[dest++] = (byte)b;
									imageData[dest++] = (byte)g;
									imageData[dest++] = (byte)r;
								}
							}
							else
							{
								imageData[dest++] = buffer[ind++];
								imageData[dest++] = buffer[ind++];
								imageData[dest++] = buffer[ind++];
								if (bitsPerPixel > 3) imageData[dest++] = buffer[ind++];
							}

							if (verticalFlipped) dest -= 2 * bitsPerPixel_;
						}
					}
				}

				switch (bitsPerPixel_)
				{
					case 1:
						format = ChannelFormat.GRAY;
						break;
					case 2:
						format = ChannelFormat.GRAYAlpha;
						break;
					case 3:
						format = ChannelFormat.RGB;
						break;
					case 4:
						format = ChannelFormat.RGBA;
						break;
					default:
						format = ChannelFormat.GRAY;
						break;
				}

				br.Close();
				fs.Close();
			}

			return new Image8i(width, height, format, imageData);
		}

		public static void ToFile(string filename, Image8i image)
		{
			if (filename == null) throw new Exception();
			if (filename == "") throw new Exception();
			if (image.Width == 0 || image.Height == 0) throw new Exception();
			if (image.Width > 0xFFFF || image.Height > 0xFFFF) throw new Exception();

			if (image.ChannelFormat == ChannelFormat.BGR)
			{
				ToFile(filename, image.ConvertToRGB());
				return;
			}

			if (image.ChannelFormat == ChannelFormat.BGRA)
			{
				ToFile(filename, image.ConvertToRGBA());
				return;
			}

			bool isRGB = (image.ChannelFormat == ChannelFormat.BGR || image.ChannelFormat == ChannelFormat.RGB || image.ChannelFormat == ChannelFormat.BGRA || image.ChannelFormat == ChannelFormat.RGBA);
			bool isAlpha = (image.ChannelFormat == ChannelFormat.BGRA || image.ChannelFormat == ChannelFormat.RGBA || image.ChannelFormat == ChannelFormat.GRAYAlpha);

			ulong size = (ulong)(18 + ((isRGB) ? (isAlpha ? 4 : 3) : (isAlpha ? 2 : 1)) * image.Width * image.Height); // Length of data
			if (size > 0xFFFFFFFF) throw new Exception(); // image is to big

			using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
			{
				BinaryWriter bw = new BinaryWriter(fs);

				byte Pixel_Depth = (byte)(isRGB ? (isAlpha ? 32 : 24) : (isAlpha ? 16 : 8));
				byte Image_Descriptor = (byte)(isAlpha ? 0x28 : 0x20);	// Field 5.6

				// Write header (18 bytes)
				bw.Write((byte)0);					// ID_Length
				bw.Write((byte)0);					// Color_Map_Type
				bw.Write((byte)(isRGB ? 2 : 3));	// Image_Type
				bw.Write((ushort)0);				// First_Entry_Index
				bw.Write((ushort)0);				// Color_Map_Length
				bw.Write((byte)0);					// Color_Map_Entry_Size
				bw.Write((ushort)0);				// X_Origin
				bw.Write((ushort)0);				// Y_Origin
				bw.Write((ushort)image.Width);		// Width
				bw.Write((ushort)image.Height);		// Height
				bw.Write(Pixel_Depth);				// Pixel_Depth
				bw.Write(Image_Descriptor);			// Image_Descriptor

				bw.Write(image.ImageData);
				bw.Close();
				fs.Close();
			}
		}
	}
}