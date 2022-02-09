using TSW.Struct;

// from: http://www.roguebasin.com/index.php?title=Bresenham%27s_Line_Algorithm

namespace TSW.Algorithm
{
	/// <summary>
	/// The Bresenham algorithm collection
	/// </summary>
	public static class Bresenhams
	{
		private static void Swap<T>(ref T lhs, ref T rhs) { T temp; temp = lhs; lhs = rhs; rhs = temp; }

		/// <summary>
		/// The plot function delegate
		/// </summary>
		/// <param name="x">The x co-ord being plotted</param>
		/// <param name="y">The y co-ord being plotted</param>
		/// <returns>True to continue, false to stop the algorithm</returns>
		public delegate bool PlotFunction(int x, int y);

		/// <summary>
		/// Plot the line from (x0, y0) to (x1, y10
		/// </summary>
		/// <param name="s">The start x</param>
		/// <param name="e">The end x</param>
		/// <param name="plot">The plotting function (if this returns false, the algorithm stops early)</param>
		public static Int2 Line(Int2 s, Int2 e, PlotFunction plot)
		{
			bool steep = System.Math.Abs(e.z - s.z) > System.Math.Abs(e.x - s.x);
			Int2 cur = e;
			if (steep)
			{
				Swap<int>(ref s.x, ref s.z);
				Swap<int>(ref e.x, ref e.z);
			}
			// WARNING: DON'T SWAP s and e. Some code need the function to plot from s to e
			//			if (s.x > e.x) {
			//				Swap<int>(ref s.x, ref e.x);
			//				Swap<int>(ref s.z, ref e.z);
			//			} 
			if (s.x > e.x)
			{
				int dX = (s.x - e.x);
				int dZ = System.Math.Abs(s.z - e.z);
				int err = (dX / 2);
				int zstep = (e.z > s.z ? 1 : -1);
				int z = s.z;

				for (int x = s.x; x >= e.x; --x)
				{
					if (steep)
					{
						cur.x = z;
						cur.z = x;
					}
					else
					{
						cur.x = x;
						cur.z = z;
					}
					if (!plot(cur.x, cur.z))
					{
						return cur;
					}
					err = err - dZ;
					if (err < 0)
					{
						z += zstep;
						err += dX;
					}
				}

			}
			else
			{
				int dX = (e.x - s.x);
				int dZ = System.Math.Abs(e.z - s.z);
				int err = (dX / 2);
				int zstep = (s.z < e.z ? 1 : -1);
				int z = s.z;
				for (int x = s.x; x <= e.x; ++x)
				{
					if (steep)
					{
						cur.x = z;
						cur.z = x;
					}
					else
					{
						cur.x = x;
						cur.z = z;
					}
					if (!plot(cur.x, cur.z))
					{
						return cur;
					}
					err = err - dZ;
					if (err < 0)
					{
						z += zstep;
						err += dX;
					}
				}
			}
			return cur;
		}
	}
}