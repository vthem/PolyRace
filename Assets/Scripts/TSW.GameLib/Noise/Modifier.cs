using UnityEngine;

namespace TSW.Noise
{
	public class Modifier : Source
	{
		[SerializeField]
		private Source _source;

		[SerializeField]
		private AnimationCurve _curve;

		public override float GetFloat(Vector3 xyz)
		{

			return _curve.Evaluate(_source.GetFloat(xyz));
		}

		public override void SetSeed(int seed)
		{
			_source.SetSeed(seed);
		}

		public override bool IsValid()
		{
			return !(_source == null);
		}
	}
}