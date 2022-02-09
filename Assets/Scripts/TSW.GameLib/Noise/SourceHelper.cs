using UnityEngine;

namespace TSW.Noise
{
	public static class SourceHelper
	{
		public static float GetFloatExtended(this Source source, Vector3 xyz)
		{
			return source.GetFloat(xyz) * 2f - 1f;
		}

		public static float GetFloat(this Source source, int x, int y, int z)
		{
			return source.GetFloat(new Vector3(x, y, z));
		}
	}
}
