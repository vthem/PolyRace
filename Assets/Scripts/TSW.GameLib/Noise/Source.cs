using UnityEngine;

namespace TSW.Noise
{
	public class Source : ScriptableObject
	{
		public virtual float GetFloat(Vector3 xyz)
		{
			return 0f;
		}

		public virtual void SetSeed(int x)
		{
		}

		public virtual bool IsValid()
		{
			return false;
		}
	}
}
