using UnityEngine;

namespace TSW
{
	public class RigidbodyPause : MonoBehaviour
	{
		protected Vector3 _velocity;
		protected Vector3 _angularVelocity;
		private Rigidbody _rigidbody;

		private void Start()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		public void Pause()
		{
			_velocity = _rigidbody.velocity;
			_angularVelocity = _rigidbody.angularVelocity;
			_rigidbody.Sleep();
			_rigidbody.isKinematic = true;
		}

		public void Resume()
		{
			_rigidbody.isKinematic = false;
			_rigidbody.velocity = _velocity;
			_rigidbody.angularVelocity = _angularVelocity;
			_rigidbody.WakeUp();

		}
	}
}
