using System;

namespace Xevle.Imaging.Image
{
	/// <summary>
	/// Image class for 8bit integer images
	/// </summary>
	public class Image8i: IImage
	{
		public ChannelFormat ChannelFormat { get; private set; }

		public uint Width { get; private set; }

		public uint Height { get; private set; }

		byte[] imageData;

		public Image8i(uint width, uint height, ChannelFormat format)
		{
			Width = width;
			Height = height;

			ChannelFormat = format;

			imageData = null;
	
			if (width * height > 0) imageData = new byte[width * height * Statics.GetMemoryUnitsPerPixel(format)];
		}
	}
}

