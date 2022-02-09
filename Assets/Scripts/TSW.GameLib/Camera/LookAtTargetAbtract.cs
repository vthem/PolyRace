using UnityEngine;

namespace TSW.Camera
{
	public abstract class LookAtTargetAbstract : MonoBehaviour
	{
		public abstract Transform GetLookAtTargetTransform();
		public abstract float GetLookAtTargetYRotationSpeed();
	}
}
