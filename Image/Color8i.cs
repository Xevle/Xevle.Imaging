using System;

namespace Xevle.Imaging.Image
{
	/// <summary>
	/// Class for 8 bit color
	/// </summary>
	public class Color8i
	{
		/// <summary>
		/// Gets the r.
		/// </summary>
		/// <value>The r.</value>
		public byte R { get; private set; }

		/// <summary>
		/// Gets the g.
		/// </summary>
		/// <value>The g.</value>
		public byte G { get; private set; }

		/// <summary>
		/// Gets the b.
		/// </summary>
		/// <value>The b.</value>
		public byte B { get; private set; }

		/// <summary>
		/// Gets a.
		/// </summary>
		/// <value>A.</value>
		public byte A { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Xevle.Imaging.Image.Color8i"/> class.
		/// </summary>
		/// <param name="r">The red component.</param>
		/// <param name="g">The green component.</param>
		/// <param name="b">The blue component.</param>
		/// <param name="a">The alpha component.</param>
		public Color8i(byte r = 0, byte g = 0, byte b = 0, byte a = 0)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}
	}
}

