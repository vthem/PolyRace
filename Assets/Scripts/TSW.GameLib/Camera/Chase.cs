using UnityEngine;

namespace TSW
{
	namespace Camera
	{
		public class Chase : MonoBehaviour
		{
			// The target we are following
			public Transform _target;
			// The distance in the x-z plane to the target
			public float _distance0;
			public float _heightRatio;

			// How much we 
			public float _heightDamping;
			public float _rotationDamping;
			public float _recoilFactor = 0.1f;
			private Rigidbody _targetRigidbody;

			private void Start()
			{
				_targetRigidbody = _target.GetComponent<Rigidbody>();
			}

			private void FixedUpdate()
			{
				// Early out if we don't have a target
				if (!_target)
				{
					return;
				}
				float distance = _target.transform.InverseTransformDirection(_targetRigidbody.velocity).z * _recoilFactor + _distance0;
				float height = _distance0 * _heightRatio;

				// Calculate the current rotation angles
				float wantedRotationAngle = _target.eulerAngles.y;
				float wantedHeight = _target.position.y + height;

				float currentRotationAngle = transform.eulerAngles.y;
				float currentHeight = transform.position.y;

				// Damp the rotation around the y-axis
				currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, _rotationDamping * Time.deltaTime);

				// Damp the height
				currentHeight = Mathf.Lerp(currentHeight, wantedHeight, _heightDamping * Time.deltaTime);

				// Convert the angle into a rotation
				Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

				// Set the position of the camera on the x-z plane to:
				// distance meters behind the target
				transform.position = _target.position;
				transform.position -= currentRotation * Vector3.forward * distance;

				// Set the height of the camera
				transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

				// Always look at the target
				transform.LookAt(_target);
			}
		}
	}
}