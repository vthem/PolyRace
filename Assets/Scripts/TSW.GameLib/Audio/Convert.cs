using UnityEngine;

namespace TSW.Audio
{
	public static class Convert
	{
		public static float ToLinear(float db)
		{
			return Mathf.Pow(10f, db / 20f);
		}

		public static float ToDecibel(float linear)
		{
			if (linear <= 0f)
			{
				return -80f;
			}
			if (linear >= 1f)
			{
				return 0f;
			}
			return 20f * Mathf.Log10(linear);
		}
	}
}
