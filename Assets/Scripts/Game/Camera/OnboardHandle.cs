using Game.Racer;

using UnityEngine;

namespace Game.Camera
{
	public class OnboardHandle : RacerHandle
	{
		[SerializeField]
		private int _mountPointIndex = 0;

		protected override void HandleInitialize()
		{
			base.HandleInitialize();
			transform.SetParent(Controller.GetComponent<MountPoint>().GetMountPoint(_mountPointIndex));
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
		}
	}
}
