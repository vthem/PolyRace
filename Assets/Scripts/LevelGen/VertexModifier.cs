using TSW.Noise;

using UnityEngine;

namespace LevelGen
{

	public class VertexModifier : ScriptableObject
	{
		[SerializeField]
		private Source _noise;

		[SerializeField]
		private AnimationCurve _yScale;

		[SerializeField]
		private AnimationCurve _xzScale;

		[SerializeField]
		private bool _enable = true;
		private int _seedBase = 123;
		public void SetSeed(int seed)
		{
			_seedBase = seed;
		}

		public Vector3 GetVectorOffset(Vector3 v, float scaleValue)
		{
			if (_enable == false)
			{
				return Vector3.zero;
			}
			float x = 0f, y = 0f, z = 0f;

			int seed = _seedBase;
			_noise.SetSeed(seed);
			x = _noise.GetFloatExtended(v) * _xzScale.Evaluate(scaleValue);

			seed *= -_seedBase;
			_noise.SetSeed(seed);
			z = _noise.GetFloatExtended(v) * _xzScale.Evaluate(scaleValue);

			seed *= -_seedBase;
			_noise.SetSeed(seed);
			y = _noise.GetFloat(v) * _yScale.Evaluate(scaleValue);

			return new Vector3(x, y, z);
		}

		public void Validate()
		{
			if (null == _noise)
			{
				throw new System.Exception("Source is not set in " + name);
			}
			if (!_noise.IsValid())
			{
				throw new System.Exception("Source " + _noise.name + " is not valid in " + name);
			}
		}
	}
}
