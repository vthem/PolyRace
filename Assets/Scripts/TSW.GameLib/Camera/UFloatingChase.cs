using UnityEngine;

namespace TSW.Camera
{
	public class UFloatingChase : MonoBehaviour
	{
		public Transform _objectToFollow;
		public Vector3 _offsetLook;
		public Vector3 _offsetPosition;
		public Vector3 _speedFactor;
		public float _cameraSpeed;

		public void Initialize(Transform target)
		{
			_objectToFollow = target;
			transform.position = _objectToFollow.position + _objectToFollow.TransformDirection(_offsetPosition);
			transform.rotation = Quaternion.identity;
			transform.rotation = Quaternion.LookRotation(_objectToFollow.position - transform.position, transform.up);
		}

		private void FixedUpdate()
		{
			// Early out if we don't have a target
			if (!_objectToFollow)
			{
				return;
			}

			//float forwardSpeed = _objectToFollow.InverseTransformDirection(_objectToFollow.rigidbody.velocity).z;

			// first set the position of the camera
			Vector3 newOffsetPosition = _offsetPosition;
			//newOffsetPosition.z *= forwardSpeed;
			Vector3 targetPosition = _objectToFollow.position + _objectToFollow.TransformDirection(newOffsetPosition);
			transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * _cameraSpeed);

			// second set where the camera look
			Vector3 newOffsetLook = _offsetLook;
			//newOffsetLook.z *= forwardSpeed;
			Vector3 targetLookPosition = _objectToFollow.position + _objectToFollow.TransformDirection(newOffsetLook);
			transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetLookPosition - transform.position, transform.up), Time.deltaTime * _cameraSpeed);
		}
	}
}
