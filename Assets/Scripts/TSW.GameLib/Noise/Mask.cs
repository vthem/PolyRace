using UnityEngine;

namespace TSW.Noise
{
	public class Mask : Source
	{
		[SerializeField]
		private Source _mask;

		[SerializeField]
		private Source _source;

		[SerializeField]
		[Range(0f, 1f)]
		private float _threshold;

		[SerializeField]
		private bool _invert = false;

		public override float GetFloat(Vector3 xyz)
		{
			if (_invert)
			{
				if (_mask.GetFloat(xyz) > _threshold)
				{
					return _source.GetFloat(xyz);
				}
			}
			else
			{
				if (_mask.GetFloat(xyz) < _threshold)
				{
					return _source.GetFloat(xyz);
				}
			}
			return 0f;
		}

		public override void SetSeed(int seed)
		{
			_mask.SetSeed(seed);
			_source.SetSeed(seed * seed);
		}

		public override bool IsValid()
		{
			return !(_mask == null || _source == null);
		}
	}
}