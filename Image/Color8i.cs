using System;

namespace Xevle.Imaging.Image
{
	public class Color8i
	{
		public byte R { get; private set; }

		public byte G { get; private set; }

		public byte B { get; private set; }

		public byte A { get; private set; }

		public Color8i(byte r=0, byte g=0, byte b=0, byte a = 0)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}
	}
}

