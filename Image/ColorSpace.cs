using System;

namespace Xevle.Imaging
{
	public class ColorSpace
	{
		/// <summary>
		/// RGB to HSL (Foley and VanDam)
		/// </summary>
		/// <param name="r">The red component.</param>
		/// <param name="g">The green component.</param>
		/// <param name="b">The blue component.</param>
		/// <param name="lit">Lit.</param>
		/// <param name="sat">Sat.</param>
		/// <param name="h">The height.</param>
		public static void RGB2HSL(byte r, byte g, byte b, out double lit, out double sat, out double h)
		{
			byte mx = System.Math.Max(System.Math.Max(r, g), b), mi = System.Math.Min(System.Math.Min(r, g), b);
			lit = mx + mi;

			double delta = mx - mi;
			if (delta == 0)
			{
				sat = h = 0;
				return;
			}

			if (lit <= 255) sat = 255 * delta / lit;
			else sat = 255 * delta / (510 - lit);

			if (r == mx) h = (g - b) * 60 / delta;
			else if (g == mx) h = 120 + (b - r) * 60 / delta;
			else h = 240 + (r - g) * 60 / delta;
			if (h < 0) h = h + 360;
		}

		/// <summary>
		/// Converts color from HSL to RGB coloar space (Foley and VanDam)
		/// </summary>
		/// <param name="r">The red component.</param>
		/// <param name="g">The green component.</param>
		/// <param name="b">The blue component.</param>
		/// <param name="lit">Lit.</param>
		/// <param name="sat">Sat.</param>
		/// <param name="h">.</param>
		public static void HSL2RGB(out byte r, out byte g, out byte b, double lit, double sat, double h)
		{
			if (sat == 0)
			{
				r = g = b = (byte)(lit / 2);
				return;
			}

			double m1, m2;
			if (lit <= 255) m2 = lit * (255 + sat) / 510;
			else m2 = lit * (255 - sat) / 510 + sat;
			m1 = lit - m2;

			double hue = h - 120;
			if (hue < 0) hue = hue + 360;
			if (hue < 60) b = (byte)(m1 + (m2 - m1) * hue / 60);
			else if (hue < 180) b = (byte)(m2);
			else if (hue < 240) b = (byte)(m1 + (m2 - m1) * (240 - hue) / 60);
			else b = (byte)(m1);
			hue = h;

			if (hue < 60) g = (byte)(m1 + (m2 - m1) * hue / 60);
			else if (hue < 180) g = (byte)(m2);
			else if (hue < 240) g = (byte)(m1 + (m2 - m1) * (240 - hue) / 60);
			else g = (byte)(m1);

			hue = h + 120;
			if (hue > 360) hue = hue - 360;
			if (hue < 60) r = (byte)(m1 + (m2 - m1) * hue / 60);
			else if (hue < 180) r = (byte)(m2);
			else if (hue < 240) r = (byte)(m1 + (m2 - m1) * (240 - hue) / 60);
			else r = (byte)(m1);
		}
	}
}

