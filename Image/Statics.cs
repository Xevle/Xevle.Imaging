using System;

namespace Xevle.Imaging.Image
{
	/// <summary>
	/// Class for static function
	/// </summary>
	public static class Statics
	{
		/// <summary>
		/// Align the specified x and granularity.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="granularity">Granularity.</param>
		public static uint Align(uint x, uint granularity)
		{
			if (granularity == 1) return x;
			if (granularity == 0) return x;
			return ((x + (granularity - 1)) / granularity) * granularity;
		}
	}
}

