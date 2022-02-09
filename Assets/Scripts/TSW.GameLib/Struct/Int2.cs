using System;
using System.Collections.Generic;

using UnityEngine;

namespace TSW.Struct
{

	[Serializable]
	public struct Int2
	{
		public int x, z;

		public float Magnitude => Mathf.Sqrt(x * x + z * z);

		public Vector2 Normalized { get { float m = Magnitude; return new Vector2(x / m, z / m); } }

		public Int2(int x, int z)
		{
			this.x = x;
			this.z = z;
		}

		public Int2(Int2 other)
		{
			x = other.x;
			z = other.z;
		}

		public Int2(Vector2 v)
		{
			x = Mathf.RoundToInt(v.x);
			z = Mathf.RoundToInt(v.y);
		}

		public void Swap()
		{
			int t = x;
			x = z;
			z = t;
		}

		public Int2 Left => new Int2(x - 1, z);
		public Int2 Right => new Int2(x + 1, z);
		public Int2 Down => new Int2(x, z - 1);
		public Int2 Up => new Int2(x, z + 1);

		public override string ToString()
		{
			return x.ToString() + "," + z.ToString();
		}

		public override int GetHashCode()
		{
			return x ^ z;
		}

		public static Int2 operator +(Int2 a, Int2 b)
		{
			return new Int2(a.x + b.x, a.z + b.z);
		}

		public static Int2 operator -(Int2 a, Int2 b)
		{
			return new Int2(a.x - b.x, a.z - b.z);
		}

		public static bool operator ==(Int2 a, Int2 b)
		{
			if ((object)a == null || (object)b == null)
			{
				return false;
			}
			return a.x == b.x && a.z == b.z;
		}

		public static bool operator !=(Int2 a, Int2 b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			// If parameter is null return false.
			if (obj == null)
			{
				return false;
			}

			if (obj is Int2)
			{
				Int2 p = (Int2)obj;
				// Return true if the fields match:
				return (x == p.x) && (z == p.z);
			}
			return false;
		}

		public static Int2 RotateCW(Int2 i)
		{
			i.RotateCW();
			return i;
		}

		public static Int2 RotateACW(Int2 i)
		{
			i.RotateACW();
			return i;
		}

		public void RotateCW()
		{
			int tmp = x;
			x = z;
			z = -tmp;
		}

		public void RotateACW()
		{
			int tmp = x;
			x = -z;
			z = tmp;
		}

		public static float Angle(Int2 a, Int2 m, Int2 b)
		{
			Vector2 va = new Vector2(a.x, a.z);
			Vector2 vm = new Vector2(m.x, m.z);
			Vector2 vb = new Vector2(b.x, b.z);
			return Vector2.Angle(va - vm, vb - vm);
		}

		public delegate int BinarySearchCompare(Int2 value);

		public static Int2 BinarySearch(Int2 start, Int2 end, BinarySearchCompare compare)
		{
			Int2 mid = (end - start);
			mid.x = Mathf.FloorToInt(mid.x / 2f);
			mid.z = Mathf.FloorToInt(mid.z / 2f);
			if (mid.Magnitude <= 1)
			{
				return start;
			}
			int r = compare(mid + start);
			if (r > 0)
			{
				return BinarySearch(mid + start, end, compare);
			}
			else if (r < 0)
			{
				return BinarySearch(start, mid + start, compare);
			}
			return end;
		}

		public static IEnumerable<Int2> SpiralIteration(Int2 start)
		{
			int dx = 0;
			int dz = 1;
			int t = 0;
			int i = 1;
			Int2 cur = new Int2();
			yield return start;
			while (true)
			{
				for (int j = 0; j < 2; j++)
				{
					for (int k = 0; k < i; k++)
					{
						cur.x += dx;
						cur.z += dz;
						yield return cur + start;
					}
					t = dx;
					dx = dz;
					dz = -t;
				}
				++i;
			}
		}

		public static IEnumerable<Int2> Neighbors(Int2 pos)
		{
			yield return new Int2(pos.x, pos.z + 1);
			yield return new Int2(pos.x + 1, pos.z + 1);
			yield return new Int2(pos.x + 1, pos.z);
			yield return new Int2(pos.x + 1, pos.z - 1);
			yield return new Int2(pos.x, pos.z - 1);
			yield return new Int2(pos.x - 1, pos.z - 1);
			yield return new Int2(pos.x - 1, pos.z);
			yield return new Int2(pos.x - 1, pos.z + 1);
		}

		public static void Neighbors(Int2 pos, Action<Int2> action)
		{
			pos.z += 1; // 0, 1
			action(pos);

			pos.x += 1; // 1, 1
			action(pos);

			pos.z -= 1; // 1, 0
			action(pos);

			pos.z -= 1; // 1, -1
			action(pos);

			pos.x -= 1; // 0, -1
			action(pos);

			pos.x -= 1; // -1, -1
			action(pos);

			pos.z += 1; // -1, 0
			action(pos);

			pos.z += 1; // -1, 1
			action(pos);
		}

		public static IEnumerable<Int2> ZFlipIteration(Int2 pos, int limit)
		{
			yield return pos;
			int offset = 1;
			while (limit > 0)
			{
				yield return new Int2(pos.x, pos.z + offset);
				yield return new Int2(pos.x, pos.z - offset);
				limit--;
				offset++;
			}
		}

		public static IEnumerable<Int2> XFlipIteration(Int2 pos, int limit)
		{
			yield return pos;
			int offset = 1;
			while (limit > 0)
			{
				yield return new Int2(pos.x + offset, pos.z);
				yield return new Int2(pos.x - offset, pos.z);
				limit--;
				offset++;
			}
		}

		public IEnumerable<Int2> Iteration(int xLen, int zLen)
		{
			Int2 pos = new Int2(this);
			int X = x + xLen;
			int Z = z + zLen;
			while (pos.x < X)
			{
				while (pos.z < Z)
				{
					yield return pos;
					pos.z += 1;
				}
				pos.z = z;
				pos.x += 1;
			}
		}

		public static IEnumerable<Int2> LineIterator(Int2 s, Int2 e)
		{
			int x0 = s.x, y0 = s.z, x1 = e.x, y1 = e.z;
			bool steep = System.Math.Abs(y1 - y0) > System.Math.Abs(x1 - x0);
			if (steep)
			{
				int t;
				t = x0; // swap x0 and y0
				x0 = y0;
				y0 = t;
				t = x1; // swap x1 and y1
				x1 = y1;
				y1 = t;
			}
			if (x0 > x1)
			{
				int t;
				t = x0; // swap x0 and x1
				x0 = x1;
				x1 = t;
				t = y0; // swap y0 and y1
				y0 = y1;
				y1 = t;
			}
			int dx = x1 - x0;
			int dy = System.Math.Abs(y1 - y0);
			int error = dx / 2;
			int ystep = (y0 < y1) ? 1 : -1;
			int y = y0;
			for (int x = x0; x <= x1; x++)
			{
				yield return new Int2((steep ? y : x), (steep ? x : y));
				error = error - dy;
				if (error < 0)
				{
					y += ystep;
					error += dx;
				}
			}
			yield break;
		}
	}
}
