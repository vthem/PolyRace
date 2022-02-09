using System;

using TSW;

using DG.Tweening;

using UnityEngine;

namespace Game.Racer.Modules
{
	public class DataModule : BaseModule
	{
		private float _normalizedAngularSpeed = 0f;
		private float _normalizedAcceleration;
		private float _normalizedSpeed = 0f;
		private float _maxSpeed;
		private Vector3 _lastPosition;
		private float _maxAcceleration;

		public float NormalizedAngularSpeed => _normalizedAngularSpeed;
		public float NormalizedAcceleration => _normalizedAcceleration;
		public float NormalizedSpeed => _normalizedSpeed;
		public float MaxSpeed => _maxSpeed;
		public float Speed => Controller.VelocityMeter.Velocity.z;
		public Vector3 LastValidPosition { get; private set; }
		public float TerrainHeight { get; private set; }
		public Vector3 TerrainForward { get; private set; }
		public Vector3 TerrainUpward { get; private set; }

		public bool GhostDriven { get; set; }

		public override void Enable()
		{
			base.Enable();
		}

		public override void ModuleUpdate()
		{
			Controller.VelocityMeter.ExternalUpdate();
			if (!GhostDriven)
			{
				_normalizedAngularSpeed = Mathf.Clamp(Controller.VelocityMeter.AngularVelocity.y, -DynProperties.TurnSpeed, DynProperties.TurnSpeed) / DynProperties.TurnSpeed;
			}
			_normalizedAcceleration = Mathf.Clamp(Controller.VelocityMeter.Acceleration.z / _maxAcceleration, -1f, 1f);
			_normalizedSpeed = Controller.VelocityMeter.Velocity.z / DynProperties.BoostSpeed;
			_maxSpeed = Mathf.Max(Controller.VelocityMeter.Velocity.z, _maxSpeed);
			if (Mathf.Abs((transform.position - _lastPosition).magnitude) > CommonProperties.ProbeDistance)
			{
				LastValidPosition = _lastPosition;
				_lastPosition = transform.position;
			}
			UpdateTerrainHeight();
			UpdateTerrainNormal();
		}

		public void SetValue(float normalizedAngularSpeed)
		{
			_normalizedAngularSpeed = normalizedAngularSpeed;
		}

		public void SmoothZeroNormalizedAngularSpeed()
		{
			DOTween.To(() => _normalizedAngularSpeed, (v) => _normalizedAngularSpeed = v, 0f, .5f);
		}

		public Func<float> GetAudioSourceParameterValue(string identifier)
		{
			if (identifier == "speed")
			{
				return () => _normalizedSpeed;
			}
			else if (identifier == "rotation")
			{
				return () => _normalizedAngularSpeed;
			}
			else if (identifier == "acceleration")
			{
				return () => NormalizedAcceleration;
			}
			return () => 0f;
		}

		protected override void ModuleInit()
		{
			base.ModuleInit();

			GhostDriven = false;
			LastValidPosition = Controller.transform.position;
			UpdateTerrainHeight();
			UpdateTerrainNormal();
			_maxAcceleration = Config.GetAsset().MaxAcceleration;
		}

		private static readonly Vector3[] _probes = new Vector3[] {
			new Vector3(1, 0, 1),
			new Vector3(1, 0, -1),
			new Vector3(-1, 0, 1),
			new Vector3(-1, 0, -1),
			new Vector3(0, 0, 0)
		};

		private void UpdateTerrainHeight()
		{
			RaycastHit hit;
			Vector3 raycastFrom = transform.position;
			raycastFrom.y = CommonProperties.ProbeHeight;
			float count = 0f;
			float sum = 0f;
			foreach (Vector3 p in _probes)
			{
				if (Physics.Raycast(raycastFrom + p * 5f, -Vector3.up, out hit, CommonProperties.ProbeHeight, CommonProperties.GroundLayer))
				{
					sum += hit.point.y;
					count += 1f;
				}
			}
			if (count > 0f)
			{
				TerrainHeight = sum / count;
			}
		}

		private void UpdateTerrainNormal()
		{
			Vector3 front = transform.position + transform.forward * CommonProperties.ProbeDistance;
			Vector3 back = transform.position - transform.forward * CommonProperties.ProbeDistance;
			Vector3 right = transform.position + transform.right * CommonProperties.ProbeDistance;
			Vector3 left = transform.position - transform.right * CommonProperties.ProbeDistance;
			Vector3 frontPoint, backPoint, rightPoint, leftPoint;
			if (GetGroundPoint(front, back, out frontPoint) && GetGroundPoint(back, front, out backPoint))
			{
				Vector3 forwardDir = (frontPoint - backPoint).normalized;
				if (GetGroundPoint(left, right, out leftPoint) && GetGroundPoint(right, left, out rightPoint))
				{
					Vector3 sideDir = (leftPoint - rightPoint).normalized;
					TerrainUpward = -Vector3.Cross(forwardDir, sideDir).normalized;
					TerrainForward = forwardDir;
				}
			}
		}

		private bool GetGroundPoint(Vector3 from, Vector3 to, out Vector3 pos)
		{
			from.y = CommonProperties.ProbeHeight;
			to.y = CommonProperties.ProbeHeight;
			foreach (Vector3 p in VectorUtils.Range(from, to, 5f))
			{
				RaycastHit hit;
				if (Physics.Raycast(p, -Vector3.up, out hit, CommonProperties.ProbeHeight, CommonProperties.GroundLayer))
				{
					pos = hit.point;
					return true;
				}
			}
			pos = Vector3.zero;
			return false;
		}
	}
}
