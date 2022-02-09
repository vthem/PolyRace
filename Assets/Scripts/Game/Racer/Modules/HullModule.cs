using TSW.Messaging;

using UnityEngine;

namespace Game.Racer.Modules
{
	public class HullModule : BaseModule
	{
		public Animation _animation;
		public Transform _angularVelocityRoll;
		public Transform _terrainRollPitch;
		public Transform _rollDodge;
		public BoostTrail[] _boostTrails;
		private float _angularSpeed;
		public override void ModuleUpdate()
		{
			float targetAngularSpeed = Controller.DataModule.NormalizedAngularSpeed;
			float speed = Controller.DataModule.Speed;
			_angularSpeed = Mathf.Lerp(_angularSpeed, targetAngularSpeed, CommonProperties._rollSpeed * Time.deltaTime);
			//            float anglePercent = Mathf.Clamp((Controller.VelocityMeter.Velocity.x / CommonProperties._maxXVelocity) - _angularSpeed, -1f, 1f);
			float anglePercent = Mathf.Clamp(-_angularSpeed, -1f, 1f);
			_angularVelocityRoll.localEulerAngles = new Vector3(_angularVelocityRoll.localRotation.eulerAngles.x, _angularVelocityRoll.localRotation.eulerAngles.y, anglePercent * CommonProperties._maxRoll);
			_terrainRollPitch.rotation = Quaternion.LookRotation(Controller.DataModule.TerrainForward, Controller.DataModule.TerrainUpward);
			SetAnimationState("", (_angularSpeed + 1f) / 2f); // [0..1]

			for (int i = 0; i < _boostTrails.Length; ++i)
			{
				_boostTrails[i].Emit = speed > CommonProperties._minTrailSpeed;
			}
		}

		protected override void ModuleInit()
		{
			base.ModuleInit();
		}

		protected void SetAnimationState(string flapName, float value)
		{
			if (_animation != null)
			{
				_animation[_animation.clip.name].enabled = true;
				_animation[_animation.clip.name].weight = 1f;
				_animation[_animation.clip.name].normalizedTime = value;
				_animation.Sample();
				_animation[_animation.clip.name].enabled = false;
			}
		}

		[EventHandler(typeof(EnergyController.BoostActivatedEvent))]
		public void OnBoostActivated(EnergyController.BoostActivatedEvent evt)
		{
			if (enabled)
			{
				for (int i = 0; i < _boostTrails.Length; ++i)
				{
					_boostTrails[i].ActiveBoost();
				}
			}
		}

		[EventHandler(typeof(EnergyController.BoostDeactivatedEvent))]
		public void OnBoostDeactivated(EnergyController.BoostDeactivatedEvent evt)
		{
			if (enabled)
			{
				for (int i = 0; i < _boostTrails.Length; ++i)
				{
					_boostTrails[i].DisableBoost();
				}
			}
		}
	}
}