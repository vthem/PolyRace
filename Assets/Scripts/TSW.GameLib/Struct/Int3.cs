using System;

using UnityEngine;

namespace TSW.Struct
{

	[Serializable]
	public struct Int3
	{
		public int x, y, z;

		public int Magnitude => (int)Mathf.Sqrt(x * x + z * z);

		public Vector3 Normal { get { int m = Magnitude; return new Vector3(x / (float)m, z / (float)m); } }

		public Int3(int x, int y, int z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Int3(Int3 other)
		{
			x = other.x;
			y = other.y;
			z = other.z;
		}

		public Int3(Vector3 v)
		{
			x = Mathf.FloorToInt(v.x);
			y = Mathf.FloorToInt(v.y);
			z = Mathf.FloorToInt(v.x);
		}

		public override string ToString()
		{
			return x.ToString() + "," + y.ToString() + "," + z.ToString();
		}

		public override int GetHashCode()
		{
			return x ^ y ^ z;
		}

		public static Int3 operator +(Int3 a, Int3 b)
		{
			return new Int3(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		public static Int3 operator -(Int3 a, Int3 b)
		{
			return new Int3(a.x - b.x, a.y - b.y, a.z - b.z);
		}

		public static bool operator ==(Int3 a, Int3 b)
		{
			if ((object)a == null || (object)b == null)
			{
				return false;
			}
			return a.x == b.x && a.y == b.y && a.z == b.z;
		}

		public static bool operator !=(Int3 a, Int3 b)
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

			if (obj is Int3)
			{
				Int3 p = (Int3)obj;
				// Return true if the fields match:
				return (x == p.x) && (y == p.y) && (z == p.z);
			}
			return false;
		}
	}
}
