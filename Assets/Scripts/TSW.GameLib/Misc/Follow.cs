using UnityEngine;

namespace TSW
{
	public class Follow : MonoBehaviour
	{
		[SerializeField]
		private Transform _target;
		public Transform Target { set => _target = value; }

		[SerializeField]
		public Vector3 _relativePosition;
		public Vector3 RelativePosition { get => _relativePosition; set => _relativePosition = value; }

		[SerializeField]
		private Vector3 _speed3;

		//		public float _speed = 1f;



		private Vector3 _velocity = Vector3.zero;

		public void UpdatePosition()
		{
			transform.position = ComputeTargetPosition();
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
				// transform.position = Vector3.SmoothDamp(transform.position, ComputeTargetPosition(), ref _velocity, _speed);

				// compute the position of the camera in target local space
				Vector3 pos = _target.InverseTransformPoint(transform.position);
				pos.x = Mathf.SmoothDamp(pos.x, _relativePosition.x, ref _velocity.x, _speed3.x);
				pos.y = Mathf.SmoothDamp(pos.y, _relativePosition.y, ref _velocity.y, _speed3.y);
				pos.z = Mathf.SmoothDamp(pos.z, _relativePosition.z, ref _velocity.z, _speed3.z);
				transform.position = _target.TransformPoint(pos);
			}
		}

		private Vector3 ComputeTargetPosition()
		{
			return _target.position + _target.TransformDirection(_relativePosition);
		}
	}
}
