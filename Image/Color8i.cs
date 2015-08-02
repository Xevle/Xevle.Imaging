using System;

namespace Xevle.Imaging.Image
{
	/// <summary>
	/// Class for 8 bit color
	/// </summary>
	public class Color8i
	{
		#region Public properties
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
		#endregion

		#region Static color definitions
		public static Color8i Black = new Color8i(0, 0, 0);
		public static Color8i Blue = new Color8i(0, 0, 255);

		public static Color8i Brown = new Color8i(192, 128, 0);

		public static Color8i Cyan = new Color8i(0, 255, 255);

		public static Color8i DarkBlue = new Color8i(0, 0, 128);
		public static Color8i DarkBrown = new Color8i(128, 64, 0);
		public static Color8i DarkCyan = new Color8i(0,128,128);
		public static Color8i DarkGray = new Color8i(64, 64, 64);
		public static Color8i DarkGreen = new Color8i(0, 128, 0);
		public static Color8i DarkMagenta = new Color8i(128, 0, 128);
		public static Color8i DarkOrange = new Color8i(192, 96, 0);
		public static Color8i DarkPink = new Color8i(192, 0, 128);
		public static Color8i DarkRed = new Color8i(128,0,0);
		public static Color8i DarkViolet = new Color8i(128, 0, 192);
		public static Color8i DarkYellow = new Color8i(128,128,0);

		public static Color8i Gray = new Color8i(128, 128, 128);
		public static Color8i Green = new Color8i(0, 255, 0);

		public static Color8i LightBlue = new Color8i(128, 128, 255);
		public static Color8i LightBrown = new Color8i(255, 160, 0);
		public static Color8i LightCyan = new Color8i(128, 255, 255);
		public static Color8i LightGray = new Color8i(192, 192, 192);
		public static Color8i LightGreen = new Color8i(128, 255, 128);
		public static Color8i LightMagenta = new Color8i(255, 128, 255);
		public static Color8i LightOrange = new Color8i(255, 160, 64);
		public static Color8i LightPink = new Color8i(255, 64, 192);
		public static Color8i LightRed = new Color8i(255, 128, 128);
		public static Color8i LightViolet = new Color8i(192, 64, 255);
		public static Color8i LightYellow = new Color8i(255, 255, 128);

		public static Color8i Magenta = new Color8i(255, 0, 255);

		public static Color8i Orange = new Color8i(255, 128, 0);

		public static Color8i Pink = new Color8i(255, 0, 160);

		public static Color8i Red = new Color8i(255, 0, 0);

		public static Color8i Violet = new Color8i(160, 0, 255);

		public static Color8i White = new Color8i(255, 255, 255);

		public static Color8i Yellow = new Color8i(255, 255, 0);
		#endregion

		#region Constructor
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
		#endregion
	}
}

