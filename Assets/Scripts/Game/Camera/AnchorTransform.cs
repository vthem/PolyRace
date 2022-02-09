using UnityEngine;

namespace Game.Camera
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(UnityEngine.Camera))]
	public class AnchorTransform : MonoBehaviour
	{
		[SerializeField]
		private RectTransform _target;
		private UnityEngine.Camera _camera;

		private void Start()
		{
			_camera = GetComponent<UnityEngine.Camera>();
		}

		// Update is called once per frame
		private void Update()
		{
			if (_target != null)
			{
				_target.position = _camera.ViewportToWorldPoint(new Vector3(1, 1, _camera.farClipPlane / 2f));
			}
		}
	}
}