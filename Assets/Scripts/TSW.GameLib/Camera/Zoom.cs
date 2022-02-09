using UnityEngine;

namespace TSW.Camera
{
	public class Zoom : MonoBehaviour
	{
		[SerializeField]
		private float _minDistance = 1f;
		public float MinDistance => _minDistance;

		[SerializeField]
		private float _fovMinDistance = 120f;

		[SerializeField]
		private float _maxDistance = 100f;
		public float MaxDistance => _maxDistance;

		[SerializeField]
		private float _fovMaxDistance = 3f;

		[SerializeField]
		private Transform _target;
		public Transform Target { set { _target = value; enabled = _target != null; } }

		[SerializeField]
		private float _zoomSpeed = 0.1f;

		[SerializeField]
		private UnityEngine.Camera _activeCamera;
		public UnityEngine.Camera ActiveCamera { set => _activeCamera = value; }

		private void Start()
		{
			if (null == _activeCamera)
			{
				_activeCamera = GetComponent<UnityEngine.Camera>();
			}
		}

		public void UpdateZoom()
		{
			_activeCamera.fieldOfView = ComputeTargetFov();
		}

		private void FixedUpdate()
		{
			if (_target == null)
			{
				Debug.Log("The target is not set in " + name);
				enabled = false;
			}
			else
			{
				_activeCamera.fieldOfView = Mathf.Lerp(_activeCamera.fieldOfView, ComputeTargetFov(), _zoomSpeed);
			}
		}

		private float ComputeTargetFov()
		{
			float distance = (transform.position - _target.position).magnitude;
			distance = Mathf.Clamp(distance, _minDistance, _maxDistance);
			return Mathf.Lerp(_fovMinDistance, _fovMaxDistance, (distance - _minDistance) / _maxDistance);
		}
	}
}
