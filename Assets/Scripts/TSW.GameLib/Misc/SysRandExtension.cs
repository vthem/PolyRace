using UnityEngine;

using SysRand = System.Random;

namespace TSW
{
	public static class SysRandExtension
	{
		public static Vector2 InsideUnitCircle(this SysRand rand)
		{
			return new Vector2(-(float)rand.NextDouble(), (float)rand.NextDouble());
		}

		public static float Range(this SysRand rand, float min, float max)
		{
			return (max - min) * (float)rand.NextDouble() + min;
		}

		public static int Range(this SysRand rand, int min, int max)
		{
			return rand.Next(min, max);
		}

		public static Vector3 RangeVector3(this SysRand rand, float min, float max)
		{
			float v = rand.Range(min, max);
			return new Vector3(v, v, v);
		}

		public static Vector3 RangeVector3(this SysRand rand, Vector3 min, Vector3 max)
		{
			Vector3 v;
			v.x = rand.Range(min.x, max.x);
			v.y = rand.Range(min.y, max.y);
			v.z = rand.Range(min.z, max.z);
			return v;
		}
	}
}
