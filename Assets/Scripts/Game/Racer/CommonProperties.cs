using UnityEngine;

namespace Game.Racer
{
	public class CommonProperties : ScriptableObject
	{
		[SerializeField]
		private GameObject _groundSmokePrefab;

		[SerializeField]
		private float _hoverStabilizatorSpeed;

		[SerializeField]
		private float _hoverStabilizatorMaxSpeed;

		[SerializeField]
		private float _pitchStabilizatorSpeed;

		[SerializeField]
		private float _hoverHeight;

		[SerializeField]
		private Material _ghostDevMaterial;

		[SerializeField]
		private Material _ghostRemoteMaterial;

		[SerializeField]
		private Material _ghostSelfMaterial;

		[SerializeField]
		private float _stopDrag = 100f;

		[SerializeField]
		private LayerMask _groundLayer;

		[SerializeField]
		private LayerMask _terrainHeightLayer;

		[SerializeField]
		private float _hitPenaltyDuration = 1f;

		[SerializeField]
		private AnimationCurve _hitPenaltyInterpolation;

		[SerializeField]
		[Tooltip("The minimum distance required to update the last position. The distance used to measure the 4 points to get altitude")]
		private float _probeDistance = 50f;

		[SerializeField]
		private float _probeHeight = 150f;

		[SerializeField]
		private GameObject _hardExplosionPrefab;

		public float _dodgeAcceleration = 2000f;
		public float _dodgeDuration = .3f;

		public float _maxXVelocity = 15f;
		public float _maxRoll = 60f;
		public float _rollSpeed = 20f;
		public float _minTrailSpeed = 30f;

		public Material _defaultColor;

		public float HoverStabilizatorSpeed => _hoverStabilizatorSpeed;
		public float HoverStabilizatorMaxSpeed => _hoverStabilizatorMaxSpeed;
		public float PitchStabiltizatorSpeed => _pitchStabilizatorSpeed;
		public LayerMask GroundLayer => _groundLayer;
		public LayerMask TerrainHeightLayer => _terrainHeightLayer;
		public float StopDrag => _stopDrag;
		public float ProbeDistance => _probeDistance;
		public float ProbeHeight => _probeHeight;
		public float HoverHeight => _hoverHeight;

		public Material GetGhostMaterial(Ghost.GhostType ghostType)
		{
			switch (ghostType)
			{
				case Ghost.GhostType.Dev: return _ghostDevMaterial;
				case Ghost.GhostType.Remote: return _ghostRemoteMaterial;
				case Ghost.GhostType.Self: return _ghostSelfMaterial;
			}
			return null;
		}
		public AnimationCurve HitPenaltyInterpolation => _hitPenaltyInterpolation;
		public float HitPenaltyDuration => _hitPenaltyDuration;

		public GameObject InstantiateExplosionPrefab()
		{
			return GameObject.Instantiate(_hardExplosionPrefab);
		}

		public ParticleSystem InstantiateGroundSmokeParticleSystem()
		{
			GameObject obj = GameObject.Instantiate(_groundSmokePrefab);
			if (obj == null)
			{
				throw new System.Exception("Could not instantiate ground smoke prefab");
			}
			return obj.GetComponent<ParticleSystem>();
		}
	}
}
