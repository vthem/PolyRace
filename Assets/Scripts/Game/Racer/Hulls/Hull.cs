using UnityEngine;

namespace Game.Racer.Hulls
{
	public abstract class Hull : MonoBehaviour
	{
		public virtual void Reset()
		{
			transform.eulerAngles = Vector3.zero;
		}


	}
}