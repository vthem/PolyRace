using UnityEngine;

namespace Game.Camera
{
	public class Handle : MonoBehaviour
	{
		[SerializeField]
		private GameObject _cameraPrefab;

		protected UnityEngine.Camera ActiveCamera { get; private set; }

		public void Initialize()
		{
			GameObject camera = CreateCamera(_cameraPrefab);
			ActiveCamera = camera.GetComponent<UnityEngine.Camera>();
			HandleInitialize();
		}

		protected GameObject CreateCamera(GameObject prefab)
		{
			GameObject camera = GameObject.Instantiate(prefab);
			camera.transform.SetParent(GetCameraParentTransform());
			camera.transform.localPosition = Vector3.zero;
			camera.transform.localRotation = Quaternion.identity;
			return camera;
		}

		protected virtual void HandleInitialize()
		{
		}

		protected virtual Transform GetCameraParentTransform()
		{
			return transform;
		}

		protected virtual void OnDisable()
		{
			if (ActiveCamera != null)
			{
				GameObject.Destroy(ActiveCamera.gameObject);
				ActiveCamera = null;
			}
		}
	}
}
