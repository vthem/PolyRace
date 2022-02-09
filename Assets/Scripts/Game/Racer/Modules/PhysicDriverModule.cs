using TSW.Messaging;

using UnityEngine;

namespace Game.Racer.Modules
{
	public class PhysicDriverModule : BaseModule
	{
		private class HitPenalty
		{
			private float _start = -1f;
			private float _maxDuration = -1f;
			private readonly AnimationCurve _penaltyInterpolation;

			public HitPenalty(AnimationCurve cerp)
			{
				_penaltyInterpolation = cerp;
			}

			public void AddPenalty(float duration)
			{
				_start = Time.time;
				_maxDuration = duration;
			}

			public float GetPenalty()
			{
				if (_maxDuration <= 0f)
				{
					return 1f;
				}
				if (Time.time - _start > _maxDuration)
				{
					_maxDuration = -1f;
					return 1f;
				}
				return _penaltyInterpolation.Evaluate(Mathf.Clamp01((Time.time - _start) / _maxDuration));
			}

			public void Reset()
			{
				_start = -1f;
				_maxDuration = -1f;
			}
		}

		private HitPenalty _hitPenalty;

		protected override void ModuleInit()
		{
			_rigidbody.maxAngularVelocity = DynProperties.TurnSpeed;
			_rigidbody.inertiaTensorRotation = Quaternion.identity;
			_rigidbody.angularDrag = DynProperties.AngularDrag;
			_hitPenalty = new HitPenalty(CommonProperties.HitPenaltyInterpolation);
		}

		public void SlowStop()
		{
			_rigidbody.drag = CommonProperties.StopDrag;
		}

		public void FastStop()
		{
			_rigidbody.drag = CommonProperties.StopDrag * 2f;
		}

		public void ResumeSpeed()
		{
			_rigidbody.velocity = Controller.VelocityMeter.VelocityWorld;
		}

		public override void Enable()
		{
			base.Enable();
			_rigidbody.isKinematic = false;
		}

		public override void Disable()
		{
			base.Disable();
		}

		public void Freeze()
		{
			_rigidbody.isKinematic = true;
		}

		private bool _first = true;
		public override void ModuleUpdate()
		{
			if (_first)
			{
				_first = false;
			}
			float zSpeed = Controller.transform.InverseTransformDirection(_rigidbody.velocity).z;
			float xSpeed = Controller.transform.InverseTransformDirection(_rigidbody.velocity).x;

			float acceleration = DynProperties.Acceleration;
			float drag = DynProperties.Drag;
			if (Controller.EnergyModule.EnergyController.BoostState)
			{
				acceleration = DynProperties.BoostAcceleration;
				drag = DynProperties.BoostDrag;
			}
			acceleration *= _hitPenalty.GetPenalty();

			Vector3 dragAcceleration = Vector3.zero;
			dragAcceleration.z = -Mathf.Sign(zSpeed) * drag * zSpeed * zSpeed * (1 + Controller.DataModule.NormalizedAngularSpeed * Controller.DataModule.NormalizedAngularSpeed * DynProperties.TurnEfficiency);
			dragAcceleration = transform.TransformDirection(dragAcceleration);


			// apply the forces
			Vector3 torque = Controller.CommandModule.Turn * DynProperties.Torque * Vector3.up;
			_rigidbody.AddTorque(torque, ForceMode.Acceleration);

			float cmdThruttle = Controller.CommandModule.Accelerate;
			if (zSpeed <= 0 && cmdThruttle < 0)
			{
				cmdThruttle = 0f;
			}

			Vector3 forwardAcceleration = cmdThruttle * acceleration * transform.forward;
			_rigidbody.AddForce(dragAcceleration + forwardAcceleration, ForceMode.Acceleration);


			// handle grip + dodge
			float xSpeedDelta = -(xSpeed * DynProperties.Grip);
			Vector3 vChange = xSpeedDelta * transform.right;
			_rigidbody.AddForce(vChange, ForceMode.VelocityChange);
		}

		public void ResetHitPenalty()
		{
			_hitPenalty.Reset();
		}

		[EventHandler(typeof(CollisionModule.CollisionEvent))]
		public void OnCollision(CollisionModule.CollisionEvent evt)
		{
			if (enabled)
			{
				_hitPenalty.AddPenalty(CommonProperties.HitPenaltyDuration);
			}
		}
	}
}
