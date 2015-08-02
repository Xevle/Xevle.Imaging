using System;
using System.Collections.Generic;
using System.IO;

namespace Xevle.Imaging.Image.Formats
{
	public static class WindowsBitmap
	{
		enum BitmapBitCompression
		{
			BI_RGB = 0,
			BI_RLE8 = 1,
			BI_RLE4 = 2,
			BI_BITFIELDS = 3,
			BI_JPEG = 4,
			BI_PNG = 5,
			BI_ALPHABITFIELDS = 6
		}

		public static IImage FromFile(string filename)
		{
			#region Variables
			// Header
			int bfOffBits;		    				// Offset of image data in byte from start of file

			// Information block
			uint biSize;							// Size of information block in byte
			int biWidth;							// Width of bitmap in pixel
			int biHeight;							// Height of bitmap in pixel
			short biPlanes;							// on PCX number of color layers on windows bitmap always 1 (not used)
			short biBitCount;						// Color depth (1, 4, 8, 16, 24, 32 Bit)
			BitmapBitCompression biCompression;		// Compression method
			uint biClrUsed;							// Colors

			// Misc things in info block
			uint bmRed = 0;		// Color mask red
			uint bmGreen = 0;	// Color mask green
			uint bmBlue = 0;	// Color mask blue
			uint bmAlpha = 0;	// Color mask alpha

			// Color table
			List<Color8i> ColorTable = new List<Color8i>();
			#endregion

			// Binary reader to open and read the file
			BinaryReader fileReader = new BinaryReader(File.OpenRead(filename));

			try
			{
				#region Header auslesen
				byte[] buffer = new byte[2];
				fileReader.Read(buffer, 0, 2);
				if (buffer[0] != 'B' && buffer[0] != 'M') throw new InvalidDataException();

				fileReader.ReadInt32(); // bfSize
				fileReader.BaseStream.Seek(10, SeekOrigin.Begin);	// skip bfReserved
				bfOffBits = fileReader.ReadInt32();
				#endregion

				#region Information block (BITMAPINFOHEADER)
				biSize = fileReader.ReadUInt32();

				if (biSize == 12) // OS/2 1.x
				{ 
					biWidth = fileReader.ReadInt16();
					biHeight = fileReader.ReadInt16();
					biPlanes = fileReader.ReadInt16();
					biBitCount = fileReader.ReadInt16();

					// static defined
					biCompression = BitmapBitCompression.BI_RGB;
		
					if (biBitCount == 1 || biBitCount == 4 || biBitCount == 8)
					{
						int CountColors = 1 << biBitCount; // 2^biBitCount

						for (int i = 0; i < CountColors; i++)
						{
							byte blue = fileReader.ReadByte();
							byte green = fileReader.ReadByte();
							byte red = fileReader.ReadByte();
							ColorTable.Add(new Color8i(red, green, blue));
						}
					}
					else if (biBitCount == 16 || biBitCount == 24 || biBitCount == 32)
					{
						// do nothing
					}
					else throw new InvalidDataException();
				}
				else if (biSize == 40 || biSize == 56 || biSize == 64)
				{
					// 40: Windows 3.1x, 95, NT
					// 56: Adobe Photoshop BMP with bit mask (not conform with Bitmap standard!)
					// 64: OS/2 2.x

					biWidth = fileReader.ReadInt32();
					biHeight = fileReader.ReadInt32();
					biPlanes = fileReader.ReadInt16();
					biBitCount = fileReader.ReadInt16();

					biCompression = (BitmapBitCompression)fileReader.ReadUInt32();

					fileReader.ReadUInt32(); // size image
					fileReader.ReadInt32(); // resolution x
					fileReader.ReadInt32(); // resolution y

					biClrUsed = fileReader.ReadUInt32();
					fileReader.ReadUInt32(); // important color

					if (biSize == 64) fileReader.BaseStream.Seek(24, SeekOrigin.Current); // Rest of OS/2 2.x header

					if (biBitCount == 16 || biBitCount == 24 || biBitCount == 32)
					{
						if (biCompression == BitmapBitCompression.BI_BITFIELDS && (biBitCount == 16 || biBitCount == 32))
						{
							bmRed = fileReader.ReadUInt32();
							bmGreen = fileReader.ReadUInt32();
							bmBlue = fileReader.ReadUInt32();
						}
						if (biCompression == BitmapBitCompression.BI_ALPHABITFIELDS && (biBitCount == 16 || biBitCount == 32))
						{
							bmRed = fileReader.ReadUInt32();
							bmGreen = fileReader.ReadUInt32();
							bmBlue = fileReader.ReadUInt32();
							bmAlpha = fileReader.ReadUInt32();
						}
					}
					else if (biBitCount == 1 || biBitCount == 4 || biBitCount == 8)
					{
						uint CountColors = biClrUsed;
						if (biClrUsed == 0) CountColors = 1u << biBitCount; // 2^biBitCount

						for (uint i = 0; i < CountColors; i++)
						{
							byte blue = fileReader.ReadByte();
							byte green = fileReader.ReadByte();
							byte red = fileReader.ReadByte();
							fileReader.BaseStream.Seek(1, SeekOrigin.Current);
							ColorTable.Add(new Color8i(red, green, blue));
						}
					}
					else throw new InvalidDataException(); 
				}
				else
				{
					// 108: Type Windows V4 not supported
					// 124: Type Windows V5 not supported
					throw new NotSupportedException();
				}

				// Consistency check
				if (biPlanes != 1 || biWidth <= 0 || biHeight == 0) throw new InvalidDataException();
				if (biBitCount != 16 && biBitCount != 32 && (biCompression == BitmapBitCompression.BI_ALPHABITFIELDS ||	biCompression == BitmapBitCompression.BI_BITFIELDS)) throw new InvalidDataException();

				// Downgrade to standard handling if bitfields are standard
				if (biCompression == BitmapBitCompression.BI_BITFIELDS)
				{
					if (biBitCount == 32 && bmRed == 0xFF0000 && bmGreen == 0xFF00 && bmBlue == 0xFF) biCompression = BitmapBitCompression.BI_RGB;
				}
				if (biCompression == BitmapBitCompression.BI_ALPHABITFIELDS)
				{
					if (biBitCount == 32 && bmRed == 0xFF0000 && bmGreen == 0xFF00 && bmBlue == 0xFF && bmAlpha == 0xFF000000) biCompression = BitmapBitCompression.BI_RGB;
				}

				int absHeight = System.Math.Abs(biHeight);
				#endregion

				#region Read image data
				fileReader.BaseStream.Seek(bfOffBits, SeekOrigin.Begin); // seek to image data

				if (biCompression == BitmapBitCompression.BI_RGB)
				{
					#region Uncompressed image data
					if (biBitCount == 1) // 1 Bit
					{
						#region 1 Bit
						// create return image
						Image8i ret;
						if (biBitCount == 32 || (biBitCount == 16 && (biCompression == BitmapBitCompression.BI_ALPHABITFIELDS || biCompression == BitmapBitCompression.BI_BITFIELDS))) ret = new Image8i((uint)biWidth, (uint)absHeight, ChannelFormat.RGBA);
						else ret = new Image8i((uint)biWidth, (uint)absHeight, ChannelFormat.RGB);

						int bytesPerRow = (int)Statics.Align((uint)(biWidth + 7) / 8, 4);

						int ind = 0;
						buffer = new byte[bytesPerRow];

						while (ColorTable.Count < 2) ColorTable.Add(Color8i.Black);

						if (biHeight > 0) // buttom-up image
						{
							for (int i = absHeight - 1; i >= 0; i--)
							{
								ind = i * biWidth * 3;
								fileReader.Read(buffer, 0, buffer.Length);
								byte pixel = 0;
								for (int a = 0; a < biWidth; a++)
								{
									if (a % 8 == 0) pixel = buffer[a / 8];
									int bit = (pixel & 0x80) == 0x80 ? 1 : 0;
									ret.ImageData[ind++] = ColorTable[bit].B;
									ret.ImageData[ind++] = ColorTable[bit].G;
									ret.ImageData[ind++] = ColorTable[bit].R;
									pixel <<= 1;
								}
							}
						}
						else if (biHeight < 0) // top-down image
						{
							for (int i = 0; i < absHeight; i++)
							{
								fileReader.Read(buffer, 0, buffer.Length);
								byte pixel = 0;

								for (int a = 0; a < biWidth; a++)
								{
									if (a % 8 == 0) pixel = buffer[a / 8];
									int bit = (pixel & 0x80) == 0x80 ? 1 : 0;
									ret.ImageData[ind++] = ColorTable[bit].B;
									ret.ImageData[ind++] = ColorTable[bit].G;
									ret.ImageData[ind++] = ColorTable[bit].R;
									pixel <<= 1;
								}
							}
						}

						return ret;
						#endregion
					}
					else if (biBitCount == 4) // 4 Bit
					{
						#region 4 Bit
						// create return image
						Image8i ret;
						if (biBitCount == 32 || (biBitCount == 16 && (biCompression == BitmapBitCompression.BI_ALPHABITFIELDS || biCompression == BitmapBitCompression.BI_BITFIELDS))) ret = new Image8i((uint)biWidth, (uint)absHeight, ChannelFormat.RGBA);
						else ret = new Image8i((uint)biWidth, (uint)absHeight, ChannelFormat.RGB);

						int BytesPerRow = (int)Statics.Align((uint)(biWidth * 4 + 7) / 8, 4);

						int ind = 0;
						buffer = new byte[BytesPerRow];

						while (ColorTable.Count < 16) ColorTable.Add(Color8i.Black);

						if (biHeight > 0) // buttom-up image
						{
							for (int i = absHeight - 1; i >= 0; i--)
							{
								ind = i * biWidth * 3;
								fileReader.Read(buffer, 0, buffer.Length);
								byte pixel = 0;
								for (int a = 0; a < biWidth; a++)
								{
									if (a % 2 == 0) pixel = buffer[a / 2];
									int bit = (pixel & 0xF0) >> 4;
									ret.ImageData[ind++] = ColorTable[bit].B;
									ret.ImageData[ind++] = ColorTable[bit].G;
									ret.ImageData[ind++] = ColorTable[bit].R;
									pixel <<= 4;
								}
							}
						}
						else if (biHeight < 0) // top-down image
						{
							for (int i = 0; i < absHeight; i++)
							{
								fileReader.Read(buffer, 0, buffer.Length);
								byte pixel = 0;

								for (int a = 0; a < biWidth; a++)
								{
									if (a % 2 == 0) pixel = buffer[a / 2];
									int bit = (pixel & 0xF0) >> 4;
									ret.ImageData[ind++] = ColorTable[bit].B;
									ret.ImageData[ind++] = ColorTable[bit].G;
									ret.ImageData[ind++] = ColorTable[bit].R;
									pixel <<= 4;
								}
							}
						}
						#endregion
					}
					else if (biBitCount == 8) // 8 Bit
					{
						#region 8 Bit
						// create return image
						Image8i ret;
						if (biBitCount == 32 || (biBitCount == 16 && (biCompression == BitmapBitCompression.BI_ALPHABITFIELDS || biCompression == BitmapBitCompression.BI_BITFIELDS))) ret = new Image8i((uint)biWidth, (uint)absHeight, ChannelFormat.RGBA);
						else ret = new Image8i((uint)biWidth, (uint)absHeight, ChannelFormat.RGB);

						int bytesPerRow = (int)Statics.Align((uint)biWidth, 4);

						int ind = 0;
						buffer = new byte[bytesPerRow];

						while (ColorTable.Count < 256) ColorTable.Add(Color8i.Black);

						if (biHeight > 0) // buttom-up image
						{
							for (int i = absHeight - 1; i >= 0; i--)
							{
								ind = i * biWidth * 3;
								fileReader.Read(buffer, 0, buffer.Length);
								for (int a = 0; a < biWidth; a++)
								{
									byte pixel = buffer[a];
									ret.ImageData[ind++] = ColorTable[pixel].B;
									ret.ImageData[ind++] = ColorTable[pixel].G;
									ret.ImageData[ind++] = ColorTable[pixel].R;
								}
							}
						}
						else if (biHeight < 0) // top-down image
						{
							for (int i = 0; i < absHeight; i++)
							{
								fileReader.Read(buffer, 0, buffer.Length);
								for (int a = 0; a < biWidth; a++)
								{
									byte pixel = buffer[a];
									ret.ImageData[ind++] = ColorTable[pixel].B;
									ret.ImageData[ind++] = ColorTable[pixel].G;
									ret.ImageData[ind++] = ColorTable[pixel].R;
								}
							}
						}
						#endregion
					}
					else if (biBitCount == 16) // 16 Bit
					{
						#region 16 Bit
						// create return image
						Image8i ret;
						if (biBitCount == 32 || (biBitCount == 16 && (biCompression == BitmapBitCompression.BI_ALPHABITFIELDS || biCompression == BitmapBitCompression.BI_BITFIELDS))) ret = new Image8i((uint)biWidth, (uint)absHeight, ChannelFormat.RGBA);
						else ret = new Image8i((uint)biWidth, (uint)absHeight, ChannelFormat.RGB);

						int bytesPerRow = (biWidth * 2);

						int ind = 0;
						buffer = new byte[bytesPerRow];

						if (biHeight > 0) // buttom-up image
						{
							for (int i = absHeight - 1; i >= 0; i--)
							{
								ind = i * biWidth * 3;
								fileReader.Read(buffer, 0, buffer.Length);
								for (int a = 0; a < biWidth; a++)
								{
									byte pixelA = buffer[a * 2];
									byte pixelB = buffer[a * 2 + 1];

									int b = (pixelA & 0x1F) << 3;
									int g = ((pixelB & 0x3) << 6) + ((pixelA & 0xE0) >> 2);
									int r = (pixelB & 0x7C) << 1;

									ret.ImageData[ind++] = (byte)(b + b / 32);
									ret.ImageData[ind++] = (byte)(g + g / 32);
									ret.ImageData[ind++] = (byte)(r + r / 32);
								}
							}
						}
						else if (biHeight < 0) // top-down image
						{
							for (int i = absHeight - 1; i >= 0; i--)
							{
								fileReader.Read(buffer, 0, buffer.Length);
								for (int a = 0; a < biWidth; a++)
								{
									byte pixelA = buffer[a * 2];
									byte pixelB = buffer[a * 2 + 1];

									int b = (pixelA & 0x1F) << 3;
									int g = ((pixelB & 0x3) << 6) + ((pixelA & 0xE0) >> 2);
									int r = (pixelB & 0x7C) << 1;

									ret.ImageData[ind++] = (byte)(b + b / 32);
									ret.ImageData[ind++] = (byte)(g + g / 32);
									ret.ImageData[ind++] = (byte)(r + r / 32);
								}
							}
						}
						#endregion
					}
					else if (biBitCount == 24) // 24 Bit
					{
						#region 24 Bit
						// create return image
						Image8i ret;
						if (biBitCount == 32 || (biBitCount == 16 && (biCompression == BitmapBitCompression.BI_ALPHABITFIELDS || biCompression == BitmapBitCompression.BI_BITFIELDS))) ret = new Image8i((uint)biWidth, (uint)absHeight, ChannelFormat.RGBA);
						else ret = new Image8i((uint)biWidth, (uint)absHeight, ChannelFormat.RGB);

						int bytesPerRow = (biWidth * 3);
						int rest = (int)(Statics.Align((uint)bytesPerRow, 4) - bytesPerRow);

						if (biHeight > 0) // buttom-up image
						{
							for (int i = 0; i < absHeight; i++)
							{
								fileReader.Read(ret.ImageData, ret.ImageData.Length - (i + 1) * bytesPerRow, bytesPerRow);
								if (rest != 0) fileReader.BaseStream.Seek(rest, SeekOrigin.Current);
							}
						}
						else if (biHeight < 0) // top-down image
						{
							if (rest == 0)
							{
								fileReader.Read(ret.ImageData, 0, bytesPerRow * absHeight); // load the whole imahe
							}
							else
							{
								for (int i = 0; i < absHeight; i++)
								{
									fileReader.Read(ret.ImageData, i * bytesPerRow, bytesPerRow);
									if (rest != 0) fileReader.BaseStream.Seek(rest, SeekOrigin.Current);
								}
							}
						}
						#endregion
					}
					else if (biBitCount == 32) // 32 Bit
					{
						#region 32 Bit
						// create return image
						Image8i ret;
						if (biBitCount == 32 || (biBitCount == 16 && (biCompression == BitmapBitCompression.BI_ALPHABITFIELDS || biCompression == BitmapBitCompression.BI_BITFIELDS))) ret = new Image8i((uint)biWidth, (uint)absHeight, ChannelFormat.RGBA);
						else ret = new Image8i((uint)biWidth, (uint)absHeight, ChannelFormat.RGB);

						int bytesPerRow = (biWidth * 4);
						buffer = new byte[bytesPerRow];

						if (biHeight > 0) // buttom-up image
						{
							for (int i = 0; i < absHeight; i++)
							{
								fileReader.Read(ret.ImageData, ret.ImageData.Length - (i + 1) * bytesPerRow, bytesPerRow);
							}
						}
						else if (biHeight < 0) // top-down image
						{
							fileReader.Read(ret.ImageData, 0, bytesPerRow * absHeight); // load the whole image
						}
						#endregion
					}
					#endregion
				}
				else if (biCompression == BitmapBitCompression.BI_RLE8) // RLE 8 encoded data
				{
					#region RLE 8
					// create return image
					Image8i ret;
					if (biBitCount == 32 || (biBitCount == 16 && (biCompression == BitmapBitCompression.BI_ALPHABITFIELDS || biCompression == BitmapBitCompression.BI_BITFIELDS))) ret = new Image8i((uint)biWidth, (uint)absHeight, ChannelFormat.RGBA);
					else ret = new Image8i((uint)biWidth, (uint)absHeight, ChannelFormat.RGB);

					if (biBitCount != 8) throw new InvalidDataException();	// only images with 8 bit color depth allowed

					int bytesPerRow = (int)Statics.Align((uint)biWidth, 4);
					int rest = bytesPerRow - biWidth;
					byte[] bufferRLE = new byte[absHeight * bytesPerRow];
					int index = 0;
					int lineCount = 0;

					while (ColorTable.Count < 256) ColorTable.Add(Color8i.Black);

					#region Dekomprimiere RLE 8
					while (fileReader.BaseStream.Position < fileReader.BaseStream.Length && index < (absHeight * bytesPerRow))
					{
						if (rest != 0 && (index / bytesPerRow) != lineCount) throw new InvalidDataException("Bad RLE 8 coding.");
						if (rest == 0 && (index / bytesPerRow) != lineCount) if (index > bytesPerRow * (lineCount + 1)) throw new InvalidDataException("Bad RLE 8 coding.");

						byte cByte = fileReader.ReadByte();

						if (cByte == 0) // command byte
						{
							byte scByte = fileReader.ReadByte();
							switch (scByte)
							{
								case 0: // end of image line
									{
										lineCount++;
										index = lineCount * bytesPerRow;
										break;
									}
								case 1: // end of image
									{
										index = absHeight * bytesPerRow;
										lineCount = absHeight;
										fileReader.BaseStream.Position = fileReader.BaseStream.Length;
										break;
									}
								case 2: // move the corrent image position
									{
										byte vRight = fileReader.ReadByte();	// move right
										if (vRight >= biWidth) throw new InvalidDataException("Bad RLE 8 coding.");

										byte vDown = fileReader.ReadByte();	// move down

										int currentRow = index - lineCount * bytesPerRow;
										if ((currentRow + vRight) >= biWidth) throw new InvalidDataException("Bad RLE 8 coding.");
										if ((lineCount + vDown) >= absHeight) throw new InvalidDataException("Bad RLE 8 coding.");

										lineCount += vDown;
										index = lineCount * bytesPerRow + currentRow + vRight;
										break;
									}
								default: // read bytes 3-255 without changes
									{
										fileReader.Read(bufferRLE, index, scByte);
										index += scByte;

										// if offset odd add seek one byte
										if (scByte % 2 != 0) fileReader.BaseStream.Seek(1, SeekOrigin.Current);

										break;
									}
							}
						}
						else // write data cByte many times
						{
							byte dByte = fileReader.ReadByte();
							for (int i = 0; i < cByte; i++) bufferRLE[index++] = dByte;
						}
					}
					#endregion

					#region Create image an fill
					if (biHeight > 0) // buttom-up image
					{
						int fIndex = 0; // RLE Buffer Index
						index = 0;

						for (int i = absHeight - 1; i >= 0; i--)
						{
							index = i * biWidth * 3;

							for (int a = 0; a < biWidth; a++)
							{
								byte pixel = bufferRLE[fIndex++];
								ret.ImageData[index++] = ColorTable[pixel].B;
								ret.ImageData[index++] = ColorTable[pixel].G;
								ret.ImageData[index++] = ColorTable[pixel].R;
							}

							fIndex += rest;
						}
					}
					else if (biHeight < 0) // top-down image
					{
						int fIndex = 0;	// RLE buffer index
						index = 0;

						for (int i = 0; i < absHeight; i++)
						{
							for (int a = 0; a < biWidth; a++)
							{
								byte pixel = bufferRLE[fIndex++];
								ret.ImageData[index++] = ColorTable[pixel].B;
								ret.ImageData[index++] = ColorTable[pixel].G;
								ret.ImageData[index++] = ColorTable[pixel].R;
							}

							fIndex += rest;
						}
					}
					#endregion
					#endregion
				}
				else if (biCompression == BitmapBitCompression.BI_RLE4) // RLE 4 endocded data
				{
					#region RLE 4
					// create return image
					Image8i ret;
					if (biBitCount == 32 || (biBitCount == 16 && (biCompression == BitmapBitCompression.BI_ALPHABITFIELDS || biCompression == BitmapBitCompression.BI_BITFIELDS))) ret = new Image8i((uint)biWidth, (uint)absHeight, ChannelFormat.RGBA);
					else ret = new Image8i((uint)biWidth, (uint)absHeight, ChannelFormat.RGB);

					if (biBitCount != 4) throw new InvalidDataException(); // only images with 4 bit color depth allowed

					int BytesPerRow = (int)Statics.Align((uint)biWidth, 4);
					int rest = BytesPerRow - biWidth;
					byte[] bufferRLE = new byte[absHeight * BytesPerRow];
					int index = 0;
					int lineCount = 0;

					while (ColorTable.Count < 16) ColorTable.Add(Color8i.Black);

					#region Dekomprimiere RLE 4
					while (fileReader.BaseStream.Position < fileReader.BaseStream.Length && index < (absHeight * BytesPerRow))
					{
						if (rest != 0 && (index / BytesPerRow) != lineCount) throw new InvalidDataException("Bad RLE 4 coding.");
						if (rest == 0 && (index / BytesPerRow) != lineCount) if (index > BytesPerRow * (lineCount + 1)) throw new InvalidDataException("Bad RLE 4 coding.");

						byte cByte = fileReader.ReadByte();

						if (cByte == 0) // command follows in byte 2
						{
							byte scByte = fileReader.ReadByte();
							switch (scByte)
							{
								case 0: // end of image line
									{
										lineCount++;
										index = lineCount * BytesPerRow;
										break;
									}
								case 1: // end of bitmap
									{
										index = absHeight * BytesPerRow;
										lineCount = absHeight;
										fileReader.BaseStream.Position = fileReader.BaseStream.Length;
										break;
									}
								case 2: // move the current image position
									{
										byte vRight = fileReader.ReadByte();	// move right
										if (vRight >= biWidth) throw new InvalidDataException("Bad RLE 4 coding.");

										byte vDown = fileReader.ReadByte();	// move down

										int currentRow = index - lineCount * BytesPerRow;
										if ((currentRow + vRight) >= biWidth) throw new InvalidDataException("Bad RLE 4 coding.");
										if ((lineCount + vDown) >= absHeight) throw new InvalidDataException("Bad RLE 4 coding.");

										lineCount += vDown;
										index = lineCount * BytesPerRow + currentRow + vRight;
										break;
									}
								default: // read nibbles 3-255 without changes
									{
										byte[] nibbles = new byte[(scByte + 1) / 2];
										fileReader.Read(nibbles, 0, nibbles.Length);

										for (int i = 0; i < nibbles.Length - (cByte % 2); i++)
										{
											bufferRLE[index++] = (byte)(nibbles[i] >> 4);
											bufferRLE[index++] = (byte)(nibbles[i] & 0xF);
										}
										if (cByte % 2 != 0) bufferRLE[index++] = (byte)(nibbles[nibbles.Length - 1] >> 4);

										// if offset odd seek one byte forward
										if (((scByte + 1) / 2) % 2 != 0) fileReader.BaseStream.Seek(1, SeekOrigin.Current);

										break;
									}
							}
						}
						else // write data cByte many times
						{
							byte dByte = fileReader.ReadByte();
							byte aByte = (byte)(dByte >> 4);
							byte bByte = (byte)(dByte & 0xF);

							for (int i = 0; i < cByte / 2; i++)
							{
								bufferRLE[index++] = aByte;
								bufferRLE[index++] = bByte;
							}

							if (cByte % 2 != 0) bufferRLE[index++] = aByte;
						}
					}
					#endregion

					#region Fill image
					if (biHeight > 0) // buttom-up image
					{
						int fIndex = 0; // RLE buffer Index
						index = 0;

						for (int i = absHeight - 1; i >= 0; i--)
						{
							index = i * biWidth * 3;

							for (int a = 0; a < biWidth; a++)
							{
								byte pixel = bufferRLE[fIndex++];
								ret.ImageData[index++] = ColorTable[pixel].B;
								ret.ImageData[index++] = ColorTable[pixel].G;
								ret.ImageData[index++] = ColorTable[pixel].R;
							}

							fIndex += rest;
						}
					}
					else if (biHeight < 0) // top-down image
					{
						int fIndex = 0;	// RLE buffer Index
						index = 0;

						for (int i = 0; i < absHeight; i++)
						{
							for (int a = 0; a < biWidth; a++)
							{
								byte pixel = bufferRLE[fIndex++];
								ret.ImageData[index++] = ColorTable[pixel].B;
								ret.ImageData[index++] = ColorTable[pixel].G;
								ret.ImageData[index++] = ColorTable[pixel].R;
							}

							fIndex += rest;
						}
					}
					#endregion
					#endregion
				}
				else if (biCompression == BitmapBitCompression.BI_BITFIELDS || biCompression == BitmapBitCompression.BI_ALPHABITFIELDS)
				{
					#region BI_BITFIELDS
					// create return image
					Image8i ret;
					if (biBitCount == 32 || (biBitCount == 16 && (biCompression == BitmapBitCompression.BI_ALPHABITFIELDS || biCompression == BitmapBitCompression.BI_BITFIELDS))) ret = new Image8i((uint)biWidth, (uint)absHeight, ChannelFormat.RGBA);
					else ret = new Image8i((uint)biWidth, (uint)absHeight, ChannelFormat.RGB);

					if (biBitCount == 16)
					{
						#region Check bit masks
						bmRed &= 0xffff; // only 16 bit
						bmGreen &= 0xffff;
						bmBlue &= 0xffff;
						bmAlpha &= 0xffff;

						bool doRed = true;
						bool doGreen = true;
						bool doBlue = true;
						bool doAlpha = true;

						if ((bmRed & bmGreen) > 0) throw new InvalidDataException("Bad bit fields");
						if ((bmRed & bmBlue) > 0) throw new InvalidDataException("Bad bit fields");
						if ((bmGreen & bmBlue) > 0) throw new InvalidDataException("Bad bit fields");

						int rshifta = 0;
						while (((bmRed >> rshifta) & 0x1) == 0) rshifta++;
						int rshiftb = rshifta;
						while (((bmRed >> rshiftb) & 0x1) != 0) rshiftb++;
						for (int i = rshiftb; i < 16; i++) if ((bmRed & 1u << i) > 0) throw new InvalidDataException("Bad bit fields");

						int gshifta = 0;
						while (((bmGreen >> gshifta) & 0x1) == 0) gshifta++;
						int gshiftb = gshifta;
						while (((bmGreen >> gshiftb) & 0x1) != 0) gshiftb++;
						for (int i = gshiftb; i < 16; i++) if ((bmGreen & 1u << i) > 0) throw new InvalidDataException("Bad bit fields");

						int bshifta = 0;
						while (((bmBlue >> bshifta) & 0x1) == 0) bshifta++;
						int bshiftb = bshifta;
						while (((bmBlue >> bshiftb) & 0x1) != 0) bshiftb++;
						for (int i = bshiftb; i < 16; i++) if ((bmBlue & 1u << i) > 0) throw new InvalidDataException("Bad bit fields");

						int ashifta = 0;
						int ashiftb = 0;

						if (biCompression == BitmapBitCompression.BI_BITFIELDS)
						{
							bmAlpha = ~(bmRed | bmGreen | bmBlue);

							if (bmAlpha != 0xffff0000)
							{
								bmAlpha &= 0xffff;
								ashifta = 0;
								while (((bmAlpha >> ashifta) & 0x1) == 0) ashifta++;

								ashiftb = ashifta;
								while (((bmAlpha >> ashiftb) & 0x1) != 0) ashiftb++;

								bmAlpha = ~bmAlpha;
								for (int i = ashiftb; i < 16; i++) bmAlpha |= 1u << i;
								bmAlpha = ~bmAlpha;
							}
							else bmAlpha = 0;
						}
						else
						{
							if ((bmAlpha & bmRed) > 0) throw new InvalidDataException("Bad bit fields");
							if ((bmAlpha & bmGreen) > 0) throw new InvalidDataException("Bad bit fields");
							if ((bmAlpha & bmBlue) > 0) throw new InvalidDataException("Bad bit fields");

							ashifta = 0;
							while (((bmAlpha >> ashifta) & 0x1) == 0) ashifta++;
							ashiftb = ashifta;
							while (((bmAlpha >> ashiftb) & 0x1) != 0) ashiftb++;
							for (int i = ashiftb; i < 16; i++) if ((bmAlpha & 1u << i) > 0) throw new InvalidDataException("Bad bit fields");
						}

						if (bmRed == 0) doRed = false;
						if (bmGreen == 0) doGreen = false;
						if (bmBlue == 0) doBlue = false;
						if (bmAlpha == 0) doAlpha = false;

						if (!doRed && !doGreen && !doBlue && !doAlpha) throw new Exception("Bad bit fields");

						int redSize = rshiftb - rshifta;
						int greenSize = gshiftb - gshifta;
						int blueSize = bshiftb - bshifta;
						int alphaSize = ashiftb - ashifta;
						#endregion

						#region 16 Bit
						int BytesPerRow = (biWidth * 2);
						buffer = new byte[BytesPerRow * absHeight];

						if (biHeight > 0) // buttom-up image
						{
							for (int i = 0; i < absHeight; i++)
							{
								fileReader.Read(buffer, buffer.Length - (i + 1) * BytesPerRow, BytesPerRow);
							}
						}
						else if (biHeight < 0) // top-down image
						{
							fileReader.Read(buffer, 0, BytesPerRow * absHeight); // load the whole image
						}

						uint redDiv = 0, greenDiv = 0, blueDiv = 0, alphaDiv = 0;
						if (doRed && redSize < 8) redDiv = 1u << redSize;
						if (doGreen && greenSize < 8) greenDiv = 1u << greenSize;
						if (doBlue && blueSize < 8) blueDiv = 1u << blueSize;
						if (doAlpha && alphaSize < 8) alphaDiv = 1u << alphaSize;

						// Use bitmask
						for (int i = 0; i < biWidth * absHeight; i++)
						{
							int start = i * 4;
							uint color = ((uint)buffer[i * 2 + 1] << 8) + (uint)buffer[i * 2];

							if (doRed)
							{
								uint red = (color & bmRed) >> rshifta;
								if (redSize > 8) red >>= redSize - 8;
								if (redSize < 8)
								{
									red <<= 8 - redSize;
									red += red / redDiv;
								}

								ret.ImageData[start + 2] = (byte)red;
							}
							else ret.ImageData[start + 2] = 0;

							if (doGreen)
							{
								uint green = (color & bmGreen) >> gshifta;
								if (greenSize > 8) green >>= greenSize - 8;
								if (greenSize < 8)
								{
									green <<= 8 - greenSize;
									green += green / greenDiv;
								}
								ret.ImageData[start + 1] = (byte)green;
							}
							else ret.ImageData[start + 1] = 0;

							if (doBlue)
							{
								uint blue = (color & bmBlue) >> bshifta;
								if (blueSize > 8) blue >>= blueSize - 8;
								if (blueSize < 8)
								{
									blue <<= 8 - blueSize;
									blue += blue / blueDiv;
								}
								ret.ImageData[start] = (byte)blue;
							}
							else ret.ImageData[start] = 0;

							if (doAlpha)
							{
								uint alpha = (color & bmAlpha) >> ashifta;
								if (alphaSize > 8) alpha >>= alphaSize - 8;
								if (alphaSize < 8)
								{
									alpha <<= 8 - alphaSize;
									alpha += alpha / alphaDiv;
								}
								ret.ImageData[start + 3] = (byte)alpha;
							}
							else ret.ImageData[start + 3] = 0;
						}
						#endregion
					}
					else if (biBitCount == 32)
					{
						#region Check bit masks
						bool doRed = true;
						bool doGreen = true;
						bool doBlue = true;
						bool doAlpha = true;

						if ((bmRed & bmGreen) > 0) throw new InvalidDataException("Bad bit fields");
						if ((bmRed & bmBlue) > 0) throw new InvalidDataException("Bad bit fields");
						if ((bmGreen & bmBlue) > 0) throw new InvalidDataException("Bad bit fields");

						int rshifta = 0;
						while (((bmRed >> rshifta) & 0x1) == 0) rshifta++;
						int rshiftb = rshifta;
						while (((bmRed >> rshiftb) & 0x1) != 0) rshiftb++;
						for (int i = rshiftb; i < 32; i++) if ((bmRed & 1u << i) > 0) throw new InvalidDataException("Bad bit fields");

						int gshifta = 0;
						while (((bmGreen >> gshifta) & 0x1) == 0) gshifta++;
						int gshiftb = gshifta;
						while (((bmGreen >> gshiftb) & 0x1) != 0) gshiftb++;
						for (int i = gshiftb; i < 32; i++) if ((bmGreen & 1u << i) > 0) throw new InvalidDataException("Bad bit fields");

						int bshifta = 0;
						while (((bmBlue >> bshifta) & 0x1) == 0) bshifta++;
						int bshiftb = bshifta;
						while (((bmBlue >> bshiftb) & 0x1) != 0) bshiftb++;
						for (int i = bshiftb; i < 32; i++) if ((bmBlue & 1u << i) > 0) throw new InvalidDataException("Bad bit fields");

						int ashifta = 0;
						int ashiftb = 0;

						if (biCompression == BitmapBitCompression.BI_BITFIELDS)
						{
							bmAlpha = ~(bmRed | bmGreen | bmBlue);
							if (bmAlpha != 0)
							{
								ashifta = 0;
								while (((bmAlpha >> ashifta) & 0x1) == 0) ashifta++;

								ashiftb = ashifta;
								while (((bmAlpha >> ashiftb) & 0x1) != 0) ashiftb++;

								bmAlpha = ~bmAlpha;
								for (int i = ashiftb; i < 32; i++) bmAlpha |= 1u << i;
								bmAlpha = ~bmAlpha;
							}
						}
						else
						{
							if ((bmAlpha & bmRed) > 0) throw new Exception("Bad bit fields");
							if ((bmAlpha & bmGreen) > 0) throw new Exception("Bad bit fields");
							if ((bmAlpha & bmBlue) > 0) throw new Exception("Bad bit fields");

							ashifta = 0;
							while (((bmAlpha >> ashifta) & 0x1) == 0) ashifta++;
							ashiftb = ashifta;
							while (((bmAlpha >> ashiftb) & 0x1) != 0) ashiftb++;
							for (int i = ashiftb; i < 32; i++) if ((bmAlpha & 1u << i) > 0) throw new InvalidDataException("Bad bit fields");
						}

						if (bmRed == 0) doRed = false;
						if (bmGreen == 0) doGreen = false;
						if (bmBlue == 0) doBlue = false;
						if (bmAlpha == 0) doAlpha = false;

						if (!doRed && !doGreen && !doBlue && !doAlpha) throw new InvalidDataException("Bad bit fields");

						int redSize = rshiftb - rshifta;
						int greenSize = gshiftb - gshifta;
						int blueSize = bshiftb - bshifta;
						int alphaSize = ashiftb - ashifta;
						#endregion

						#region 32 Bit
						int BytesPerRow = (biWidth * 4);

						if (biHeight > 0) // buttom-up image
						{
							for (int i = 0; i < absHeight; i++)
							{
								fileReader.Read(ret.ImageData, ret.ImageData.Length - (i + 1) * BytesPerRow, BytesPerRow);
							}
						}
						else if (biHeight < 0) // top-down image
						{
							fileReader.Read(ret.ImageData, 0, BytesPerRow * absHeight); // load the whole image
						}

						uint redDiv = 0, greenDiv = 0, blueDiv = 0, alphaDiv = 0;
						if (doRed && redSize < 8) redDiv = 1u << redSize;
						if (doGreen && greenSize < 8) greenDiv = 1u << greenSize;
						if (doBlue && blueSize < 8) blueDiv = 1u << blueSize;
						if (doAlpha && alphaSize < 8) alphaDiv = 1u << alphaSize;

						// Use bit mask
						for (int i = 0; i < biWidth * absHeight; i++)
						{
							int start = i * 4;
							uint color = ((uint)ret.ImageData[start + 3] << 24) + ((uint)ret.ImageData[start + 2] << 16) + ((uint)ret.ImageData[start + 1] << 8) + (uint)ret.ImageData[start];

							if (doRed)
							{
								uint red = (color & bmRed) >> rshifta;
								if (redSize > 8) red >>= redSize - 8;
								if (redSize < 8)
								{
									red <<= 8 - redSize;
									red += red / redDiv;
								}
								ret.ImageData[start + 2] = (byte)red;
							}
							else ret.ImageData[start + 2] = 0;

							if (doGreen)
							{
								uint green = (color & bmGreen) >> gshifta;
								if (greenSize > 8) green >>= greenSize - 8;
								if (greenSize < 8)
								{
									green <<= 8 - greenSize;
									green += green / greenDiv;
								}
								ret.ImageData[start + 1] = (byte)green;
							}
							else ret.ImageData[start + 1] = 0;

							if (doBlue)
							{
								uint blue = (color & bmBlue) >> bshifta;
								if (blueSize > 8) blue >>= blueSize - 8;
								if (blueSize < 8)
								{
									blue <<= 8 - blueSize;
									blue += blue / blueDiv;
								}
								ret.ImageData[start] = (byte)blue;
							}
							else ret.ImageData[start] = 0;

							if (doAlpha)
							{
								uint alpha = (color & bmAlpha) >> ashifta;
								if (alphaSize > 8) alpha >>= alphaSize - 8;
								if (alphaSize < 8)
								{
									alpha <<= 8 - alphaSize;
									alpha += alpha / alphaDiv;
								}
								ret.ImageData[start + 3] = (byte)alpha;
							}
							else ret.ImageData[start + 3] = 0;
						}
						#endregion
					}

					return ret;
					#endregion
				}
				#endregion
			}
			finally
			{
				fileReader.Close();
			}

			throw new NotSupportedException();
		}

		public static void ToFile(string filename, Image8i image)
		{
			if (image.ChannelFormat == ChannelFormat.BGR||image.ChannelFormat == ChannelFormat.GRAY)
			{
				ToFile(filename, image.ConvertToRGB());
				return;
			}

			if (image.ChannelFormat == ChannelFormat.BGRA||image.ChannelFormat == ChannelFormat.GRAYAlpha)
			{
				ToFile(filename, image.ConvertToRGBA());
				return;
			}

			BinaryWriter fileWriter = new BinaryWriter(File.Open(filename, FileMode.Create));

			#region Header
			fileWriter.Write('B'); 		// Signature
			fileWriter.Write('M'); 		// Signature
			fileWriter.Write((uint)0); 	// Size of file //TODO add filesize
			fileWriter.Write((uint)0); 	// Reserved
			fileWriter.Write((uint)70); // Offset of image data
			#endregion

			#region Informationsblock
			fileWriter.Write((uint)40); 			// Size of info block
			fileWriter.Write((int)image.Width); 	// Image with
			fileWriter.Write((int)image.Height); 	// Image height (top down image)
			fileWriter.Write((ushort)1); 			// Planes (always one)

			if (image.ChannelFormat == ChannelFormat.RGB) fileWriter.Write((ushort)24); // 24 Bit
			else if (image.ChannelFormat == ChannelFormat.RGBA) fileWriter.Write((ushort)32); // 32 Bit

			fileWriter.Write((uint)0); // Compression (BI_RGB / uncompressed)
			fileWriter.Write((uint)0); // Size of image daza (can be zero)

			fileWriter.Write((int)0); // horizontal resolution of output device in pixel per meter 
			fileWriter.Write((int)0); // vertical resolution of output device in pixel per meter 

			fileWriter.Write((uint)0); // biClrUsed (zero, no palette)
			fileWriter.Write((uint)0); // biClrImportant (zero, no palette)
			#endregion

			#region RGB Quad
			fileWriter.Write((uint)0); // blue
			fileWriter.Write((uint)0); // green
			fileWriter.Write((uint)0); // red
			fileWriter.Write((uint)0); // reserved
			#endregion

			fileWriter.Write(image.GetImageDataWithGranularity(4));

			fileWriter.Close();
		}
	}
}