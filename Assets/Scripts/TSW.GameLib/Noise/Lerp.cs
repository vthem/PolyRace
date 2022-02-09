using UnityEngine;

namespace TSW.Noise
{
	public class Lerp : Source
	{
		[SerializeField]
		private Source _time;

		[SerializeField]
		private Source _from;

		[SerializeField]
		private Source _to;

		public override float GetFloat(Vector3 xyz)
		{
			return Mathf.Lerp(_from.GetFloat(xyz), _to.GetFloat(xyz), _time.GetFloat(xyz));
		}

		public override void SetSeed(int seed)
		{
			_from.SetSeed(seed);
			_to.SetSeed(seed * seed);
			_time.SetSeed(seed * seed * seed);
		}

		public override bool IsValid()
		{
			return !(_time == null || _from == null || _to == null);
		}
	}
}