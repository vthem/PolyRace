using UnityEngine;

namespace TSW.Camera
{
	public class LookAt : MonoBehaviour
	{
		[SerializeField]
		protected Transform _target;
		public Transform Target { set => _target = value; }

		[SerializeField]
		private float _speed = 1f;

		[SerializeField]
		protected Vector3 _offset;
		private Vector3 _velocity = Vector3.zero;

		public void UpdateLookAt()
		{
			transform.forward = ComputeTargetNormal();
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
				transform.forward = Vector3.SmoothDamp(transform.forward, ComputeTargetNormal(), ref _velocity, _speed);
			}
		}

		protected virtual Vector3 ComputeTargetNormal()
		{
			return ((_target.TransformPoint(_offset)) - transform.position).normalized;
		}
	}
}