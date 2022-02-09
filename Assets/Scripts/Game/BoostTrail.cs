using UnityEngine;

namespace Game
{
	public class BoostTrail : MonoBehaviour
	{
		[SerializeField]
		private Material _boostMaterial;
		private Material _defaultMaterial;
		private TrailRenderer _trail;

		private void Start()
		{
			_trail = GetComponent<TrailRenderer>();
			_defaultMaterial = _trail.sharedMaterial;
		}

		public void ActiveBoost()
		{
			_trail.sharedMaterial = _boostMaterial;
		}

		public void DisableBoost()
		{
			_trail.sharedMaterial = _defaultMaterial;
		}

		public bool Emit
		{
			get => _trail.emitting;
			set => _trail.emitting = value;
		}
	}
}