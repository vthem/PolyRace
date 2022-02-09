using UnityEngine;

namespace TSW.Camera
{

	// from http://wiki.unity3d.com/index.php/MouseOrbitImproved

	public class MouseOrbit : MonoBehaviour
	{
		public Transform _target;
		public float _distance = 5.0f;
		public float _xSpeed = 120.0f;
		public float _ySpeed = 120.0f;

		public float _yMinLimit = -20f;
		public float _yMaxLimit = 80f;

		public float _distanceMin = .5f;
		public float _distanceMax = 15f;
		private float _x = 0.0f;
		private float _y = 0.0f;

		// Use this for initialization
		private void Start()
		{
			Init();
		}

		private void Init()
		{
			Vector3 angles = transform.eulerAngles;
			_x = angles.y;
			_y = angles.x;
			if (_target != null)
			{
				_distance = (_target.position - transform.position).magnitude;
			}
		}

		public void UpdateTarget(Transform target)
		{
			_target = target;
			transform.LookAt(_target);
			Init();
		}

		private void LateUpdate()
		{
			if (_target)
			{
				if (Input.GetKeyDown(KeyCode.V))
				{
					Init();
				}
				_x += Input.GetAxis("Mouse X") * _xSpeed * _distance * 0.02f;
				_y -= Input.GetAxis("Mouse Y") * _ySpeed * 0.02f;

				_y = ClampAngle(_y, _yMinLimit, _yMaxLimit);

				Quaternion rotation = Quaternion.Euler(_y, _x, 0);

				_distance = Mathf.Clamp(_distance - Input.GetAxis("Mouse ScrollWheel") * 5f, _distanceMin, _distanceMax);
				Vector3 negDistance = new Vector3(0.0f, 0.0f, -_distance);
				Vector3 position = rotation * negDistance + _target.position;

				transform.rotation = rotation;
				transform.position = position;
			}
		}

		public static float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360F)
			{
				angle += 360F;
			}
			if (angle > 360F)
			{
				angle -= 360F;
			}
			return Mathf.Clamp(angle, min, max);
		}
	}
}
