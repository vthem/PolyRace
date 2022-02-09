using UnityEngine;

namespace TSW.Struct
{
	public struct Cube
	{
		public int x, y, z;

		private readonly int _size;
		public int Size => _size;

		public Cube(int x, int y, int z, int size)
		{
			this._size = size;
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public Cube(Cube other)
		{
			_size = other._size;
			x = other.x;
			y = other.y;
			z = other.z;
		}

		public Cube(Vector3 worldPoint, int size)
		{
			_size = size;
			x = Mathf.FloorToInt(worldPoint.x / _size);
			y = Mathf.FloorToInt(worldPoint.y / _size);
			z = Mathf.FloorToInt(worldPoint.z / _size);
		}

		public override string ToString()
		{
			return x.ToString() + "," + y.ToString() + "," + z.ToString();
		}

		public float GetMagnitude()
		{
			return Mathf.Sqrt(x * x + y * y + z * z);
		}

		public Cube GetNormalized()
		{
			float magn = GetMagnitude();
			return new Cube(Mathf.RoundToInt(x / magn), Mathf.RoundToInt(y / magn), Mathf.RoundToInt(z / magn), _size);
		}

		public void Normalize()
		{
			float magn = GetMagnitude();
			x = Mathf.RoundToInt(x / magn);
			y = Mathf.RoundToInt(y / magn);
			z = Mathf.RoundToInt(z / magn);
		}

		public Cube GetBottomLeft(int squareLength)
		{
			int radius = (squareLength - 1) / 2;
			return new Cube(x - radius, y - radius, z - radius, _size);
		}

		public Vector3 Vector3()
		{
			return new Vector3(
					_size * x + _size / 2.0f,
					_size * y + _size / 2.0f,
					_size * z + _size / 2.0f
			);
		}

		public Vector3 BottomLeftVector3()
		{
			return new Vector3(
					_size * x,
					_size * y,
					_size * z
			);
		}

		public Vector3 TopRightVector3()
		{
			return new Vector3(
					_size * x + _size,
					_size * y + _size,
					_size * z + _size
			);
		}

		public override bool Equals(object obj)
		{
			// If parameter is null return false.
			if (obj == null)
			{
				return false;
			}

			if (obj is Cube)
			{
				Cube p = (Cube)obj;
				// Return true if the fields match:
				return (x == p.x) && (y == p.y) && (z == p.z);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return x ^ y ^ z;
		}

		public static Cube operator +(Cube a, Cube b)
		{
			if (a._size != b._size)
			{
				throw new System.Exception("Cannot add two Cell with different size");
			}
			return new Cube(a.x + b.x, a.y + b.y, a.z + b.z, a._size);
		}

		public static Cube operator -(Cube a, Cube b)
		{
			if (a._size != b._size)
			{
				throw new System.Exception("Cannot substract two Cell with different size");
			}
			return new Cube(a.x - b.x, a.y - b.y, a.z - b.z, a._size);
		}

		public static Cube operator *(Cube a, int scalar)
		{
			return new Cube(a.x * scalar, a.y * scalar, a.z * scalar, a._size);
		}

		public static bool operator ==(Cube a, Cube b)
		{
			if ((object)a == null || (object)b == null)
			{
				return false;
			}
			return a.x == b.x && a.y == b.y && a.z == b.z && a._size == b._size;
		}

		public static bool operator !=(Cube a, Cube b)
		{
			return !(a == b);
		}
	}
}
