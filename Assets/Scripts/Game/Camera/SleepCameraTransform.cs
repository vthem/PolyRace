using TSW;

using UnityEngine;

namespace Game.Camera
{
	[RequireComponent(typeof(VelocityMeter))]
	[RequireComponent(typeof(Rigidbody))]
	public class SleepCameraTransform : MonoBehaviour
	{
		public void SleepTransform()
		{
			VelocityMeter velocityMeter = GetComponent<VelocityMeter>();
			Rigidbody rigidbody = GetComponent<Rigidbody>();
			if (null == rigidbody)
			{
				Debug.LogWarning("There is no rigidbody. SmoothSleep cannot be used");
				return;
			}
			rigidbody.velocity = velocityMeter.VelocityWorld;
			rigidbody.angularVelocity = velocityMeter.AngularVelocityWorld;
			rigidbody.isKinematic = false;
			Follow follow = GetComponent<TSW.Follow>();
			if (null != follow)
			{
				follow.enabled = false;
			}
		}
	}
}
