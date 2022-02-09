using UnityEngine;

namespace TSW.Struct
{
	public struct Rect3
	{
		public int x, y, z;
		private readonly int _sX, _sY, _sZ;
		public int SizeX => _sX;
		public int SizeY => _sY;
		public int SizeZ => _sZ;

		public Rect3(int x, int y, int z, int sX, int sY, int sZ)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this._sX = sX;
			this._sY = sY;
			this._sZ = sZ;
		}

		public Rect3(Rect3 other)
		{
			x = other.x;
			y = other.y;
			z = other.z;
			_sX = other.SizeX;
			_sY = other.SizeY;
			_sZ = other.SizeZ;
		}

		public Rect3(Vector3 worldPoint, Vector3 size)
		{
			_sX = Mathf.RoundToInt(size.x);
			_sY = Mathf.RoundToInt(size.y);
			_sZ = Mathf.RoundToInt(size.z);
			x = Mathf.FloorToInt(worldPoint.x / _sX);
			y = Mathf.FloorToInt(worldPoint.y / _sY);
			z = Mathf.FloorToInt(worldPoint.z / _sZ);
		}

		public Rect3(Vector3 worldPoint, int sX, int sY, int sZ)
		{
			this._sX = sX;
			this._sY = sY;
			this._sZ = sZ;
			x = Mathf.FloorToInt(worldPoint.x / _sX);
			y = Mathf.FloorToInt(worldPoint.y / _sY);
			z = Mathf.FloorToInt(worldPoint.z / _sZ);
		}

		public override string ToString()
		{
			return x.ToString() + "," + y.ToString() + "," + z.ToString();
		}

		public Vector3 Vector3()
		{
			return new Vector3(
					_sX * x + _sX / 2.0f,
					_sY * y + _sY / 2.0f,
					_sZ * z + _sZ / 2.0f
			);
		}

		public Vector3 LowerCorner()
		{
			return new Vector3(
					_sX * x,
					_sY * y,
					_sZ * z
			);
		}

		public Vector3 UpperCorner()
		{
			return new Vector3(
					_sX * x + _sX,
					_sY * y + _sY,
					_sZ * z + _sZ
			);
		}

		public override int GetHashCode()
		{
			return x ^ y ^ z;
		}

		public static Rect3 operator +(Rect3 a, Rect3 b)
		{
			if (a._sX != b._sX ||
					a._sY != b._sY ||
					a._sZ != b._sZ)
			{
				throw new System.Exception("Cannot add two Cell with different size");
			}
			return new Rect3(a.x + b.x, a.y + b.y, a.z + b.z, a._sX, a._sY, a._sZ);
		}

		public static Rect3 operator -(Rect3 a, Rect3 b)
		{
			if (a._sX != b._sX ||
					a._sY != b._sY ||
					a._sZ != b._sZ)
			{
				throw new System.Exception("Cannot substract two Cell with different size");
			}
			return new Rect3(a.x - b.x, a.y - b.y, a.z - b.z, a._sX, a._sY, a._sZ);
		}

		public static bool operator ==(Rect3 a, Rect3 b)
		{
			if ((object)a == null || (object)b == null)
			{
				return false;
			}
			return a.x == b.x && a.y == b.y && a.z == b.z
					&& a._sX == b._sX && a._sY == b._sY && a._sZ == b._sZ;
		}

		public static bool operator !=(Rect3 a, Rect3 b)
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

			if (obj is Rect3)
			{
				Rect3 p = (Rect3)obj;
				// Return true if the fields match:
				return (x == p.x) && (y == p.y) && (z == p.z)
					&& (_sX == p._sX) && (_sY == p._sY) && (_sZ == p._sZ);
			}
			return false;
		}
	}
}
