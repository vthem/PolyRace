using UnityEngine;

namespace TSW.Noise
{
	public class Highest : Source
	{
		[SerializeField]
		private Source[] _sources;

		public override float GetFloat(Vector3 xyz)
		{
			float sample = 0f;
			foreach (Source src in _sources)
			{
				if (src == null)
				{
					continue;
				}
				sample = Mathf.Max(src.GetFloat(xyz), sample);
			}
			return Mathf.Clamp01(sample);
		}

		public override void SetSeed(int seed)
		{
			int i = 1;
			foreach (Source src in _sources)
			{
				src.SetSeed(seed * i);
				++i;
			}
		}

		public override bool IsValid()
		{
			return !(_sources == null || _sources.Length == 0);
		}
	}
}