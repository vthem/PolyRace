using UnityEngine;

namespace Game.Racer
{
	public class Properties : ScriptableObject, IPropertiesGetter
	{
		[SerializeField]
		private GameObject _prefab;
		public GameObject Prefab => _prefab;

		[SerializeField]
		private string _hovercraftName;
		public string HovercraftName => _hovercraftName;

		[SerializeField]
		private int _id;
		public int Id => _id;

		[SerializeField]
		private float _speed;

		[SerializeField]
		private float _boostSpeed;

		[SerializeField]
		private float _acceleration;

		[SerializeField]
		private float _boostAcceleration;

		[SerializeField]
		private float _turnSpeed;

		[SerializeField]
		private float _turnEfficiency;

		[SerializeField]
		private float _angularDrag;

		[SerializeField]
		private float _grip;

		[SerializeField]
		private float _shieldDelay;

		[SerializeField]
		private float _brake;

		public float ShieldRegenRate => ShieldCapacity / ShieldDelay;
		public float ShieldCapacity => 1;
		public float ShieldCost => ShieldCapacity / ShieldCount;
		public float ShieldCount => 5;
		public float ShieldDelay => _shieldDelay;
		public float Torque => (_turnSpeed * Mathf.Deg2Rad + 0.4f) * 5f;
		public float TurnSpeed => _turnSpeed * Mathf.Deg2Rad;
		public float TurnEfficiency => _turnEfficiency;
		public float Acceleration => _acceleration;
		public float BoostAcceleration => _boostAcceleration;
		public float Speed => _speed / 3.6f;
		public float BoostSpeed => _boostSpeed / 3.6f;
		public float Drag => _acceleration / (Speed * Speed);
		public float BoostDrag => _boostAcceleration / (BoostSpeed * BoostSpeed);
		public float Grip => _grip;
		public float AngularDrag => _angularDrag;
		public float Brake => _brake;
	}
}
