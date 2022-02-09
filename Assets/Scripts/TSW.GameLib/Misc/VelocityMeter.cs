using UnityEngine;

namespace TSW
{
	public class VelocityMeter : MonoBehaviour
	{
		private enum UpdateType
		{
			FixedUpdate,
			External
		}

		[SerializeField]
		private Vector3 _velocity;
		[SerializeField]
		private Vector3 _minVelocity;
		[SerializeField]
		private Vector3 _maxVelocity;
		[SerializeField]
		private Vector3 _angularVelocity;
		[SerializeField]
		private Vector3 _minAngularVelocity;
		[SerializeField]
		private Vector3 _maxAngularVelocity;
		[SerializeField]
		private Vector3 _acceleration;
		[SerializeField]
		private Vector3 _minAcceleration;
		[SerializeField]
		private Vector3 _maxAcceleration;
		[SerializeField]
		private Vector3 _angularAcceleration;
		[SerializeField]
		private Vector3 _minAngularAcceleration;
		[SerializeField]
		private Vector3 _maxAngularAcceleration;
		[SerializeField]
		private UpdateType _updateType = UpdateType.FixedUpdate;

		public Vector3 Velocity => _velocity;
		public Vector3 VelocityWorld => transform.TransformVector(_velocity);
		public Vector3 AngularVelocity => _angularVelocity;
		public Vector3 AngularVelocityWorld => transform.TransformVector(_angularVelocity);
		public Vector3 Acceleration => _acceleration;

		private Vector3 _lastPosition;
		private Vector3 _lastEulerAngles;
		private Vector3 _lastVelocity;
		private Vector3 _lastAngularVelocity;

		private void Start()
		{
			_lastPosition = transform.position;
			_lastEulerAngles = transform.eulerAngles;
		}

		private void FixedUpdate()
		{
			if (_updateType == UpdateType.FixedUpdate)
			{
				InternalUpdate();
			}
		}

		public void ExternalUpdate()
		{
			if (_updateType == UpdateType.External)
			{
				InternalUpdate();
			}
		}

		private void InternalUpdate()
		{
			_velocity = transform.InverseTransformVector((transform.position - _lastPosition)) / Time.fixedDeltaTime;
			//            DebugGraph.Log("_velocity" + gameObject.name, _velocity);
			_acceleration = (_velocity - _lastVelocity) / Time.fixedDeltaTime;
			Vector3 deltaAngle = new Vector3(
				-Mathf.DeltaAngle(transform.eulerAngles.x, _lastEulerAngles.x),
				-Mathf.DeltaAngle(transform.eulerAngles.y, _lastEulerAngles.y),
				-Mathf.DeltaAngle(transform.eulerAngles.z, _lastEulerAngles.z));
			_angularVelocity = (2f * Mathf.PI * deltaAngle) / (360.0f * Time.fixedDeltaTime);

			_angularAcceleration = (_angularVelocity - _lastAngularVelocity) / Time.fixedDeltaTime;

			_lastVelocity = _velocity;
			_lastAngularVelocity = _angularVelocity;
			_lastPosition = transform.position;
			_lastEulerAngles = transform.eulerAngles;

			_maxAcceleration = Vector3.Max(_acceleration, _maxAcceleration);
			_minAcceleration = Vector3.Min(_acceleration, _minAcceleration);
			_maxAngularAcceleration = Vector3.Max(_angularAcceleration, _maxAngularAcceleration);
			_minAngularAcceleration = Vector3.Min(_angularAcceleration, _minAngularAcceleration);
			_maxVelocity = Vector3.Max(_velocity, _maxVelocity);
			_minVelocity = Vector3.Min(_velocity, _minVelocity);
			_maxAngularVelocity = Vector3.Max(_angularVelocity, _maxAngularVelocity);
			_minAngularVelocity = Vector3.Min(_angularVelocity, _minAngularVelocity);
		}
	}
}
