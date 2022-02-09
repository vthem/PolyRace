using UnityEngine;

namespace Game.Camera
{
	public class ChaseHandle : RacerHandle
	{
		[SerializeField]
		private GameObject _HUDCameraPrefab;

		[SerializeField]
		private Canvas _HUDCanvas;
		private TSW.Follow _follow;
		private RacerLookAt _lookAt;
		private UnityEngine.Camera _HUDCamera;

		protected override void HandleInitialize()
		{
			base.HandleInitialize();
			GameObject camera = CreateCamera(_HUDCameraPrefab);
			_HUDCamera = camera.GetComponent<UnityEngine.Camera>();
			_HUDCanvas.worldCamera = _HUDCamera;
			if (Controller != null)
			{
				_follow = GetComponent<TSW.Follow>();
				_follow.Target = Controller.transform;
				_follow.UpdatePosition();

				_lookAt = GetComponent<RacerLookAt>();
				_lookAt.SetRacerController(Controller);
				_lookAt.UpdateLookAt();
			}
			else
			{
				Debug.Log("Controller not set in ChaseHandle");
			}
		}

		protected override Transform GetCameraParentTransform()
		{
			return transform.Find("CameraContainer");
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (_HUDCamera != null)
			{
				GameObject.Destroy(_HUDCamera.gameObject);
				_HUDCamera = null;
			}
		}
	}
}
