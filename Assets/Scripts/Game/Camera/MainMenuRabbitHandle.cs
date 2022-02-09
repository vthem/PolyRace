using TSW;
using TSW.Camera;

using UnityEngine;

namespace Game.Camera
{
	public class MainMenuRabbitHandle : Handle
	{
		private Follow _follow;
		private LookAt _lookAt;

		protected override void HandleInitialize()
		{
			base.HandleInitialize();
			GameObject obj = GameObject.FindWithTag("MainMenuRabbit");
			if (null == obj)
			{
				Debug.LogWarning("MainMenuRabbitHandle - Could not find object with tag MainMenuRabbit");
				return;
			}
			_follow = GetComponent<TSW.Follow>();
			_follow.Target = obj.transform;
			_follow.UpdatePosition();
			_follow.enabled = true;

			_lookAt = GetComponent<TSW.Camera.LookAt>();
			_lookAt.Target = obj.transform;
			_lookAt.UpdateLookAt();
			_lookAt.enabled = true;
		}

		protected override Transform GetCameraParentTransform()
		{
			return transform;
		}
	}
}