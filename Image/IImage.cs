using System;

namespace Xevle.Imaging.Image
{
	/// <summary>
	/// Interface for all image implementations
	/// </summary>
	public interface IImage
	{
		/// <summary>
		/// Gets the channel format.
		/// </summary>
		/// <value>The channel format.</value>
		ChannelFormat ChannelFormat { get; }

		/// <summary>
		/// Gets the color depth.
		/// </summary>
		/// <value>The color depth.</value>
		ColorDepth ColorDepth { get; }
	}
}

