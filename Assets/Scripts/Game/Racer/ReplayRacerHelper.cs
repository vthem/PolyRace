using DG.Tweening;

using UnityEngine;

namespace Game.Racer
{
	public class ReplayRacerHelper : MonoBehaviour, IRacerHelper
	{
		private Controller _controller;

		private Controller Controller => _controller;

		private void Awake()
		{
			_controller = GetComponent<Controller>();
			_controller.Helper = this;
		}

		public void Ready(GameObject smokePrefab)
		{
			Controller.RegisterForEvent();
			Controller.DataModule.Enable();
			Controller.AudioModule.Enable();
			Controller.HoveringModule.Enable();
			Controller.CollisionModule.Enable();
			Controller.GroundSmokeModule.Enable(smokePrefab);
			Controller.ExplosionModule.Enable();
			Controller.HullModule.Enable();
		}

		public void Go()
		{
		}

		public void PassEndLine()
		{
			Controller.PhysicDriverModule.ResumeSpeed();
			Controller.PhysicDriverModule.FastStop();
			Controller.DataModule.SmoothZeroNormalizedAngularSpeed();
		}

		public void CrashTerrain()
		{
			Controller.HullModule.Disable();
			Controller.CollisionModule.Disable();
			transform.DOLocalMove(Controller.CollisionModule.GetCollisionPoint(), .1f);
			Controller.AudioModule.Disable();
			Controller.HoveringModule.Disable();
			Controller.GroundSmokeModule.Disable();
			Controller.DetachModule.Enable();
			Controller.BurnSmokeModule.Enable();
			Controller.ExplosionModule.Explode();
		}

		public void CrashGround()
		{
			Controller.HullModule.Disable();
			Controller.AudioModule.Disable();
			Controller.HoveringModule.Disable();
			Controller.PhysicDriverModule.ResumeSpeed();
			Controller.SlideModule.Enable();
			Controller.SlideModule.RegisterOnStop(() =>
			{
				Controller.GroundSmokeModule.Disable();
			});
			Controller.CollisionModule.RegisterOnCollision(
				() =>
				{
					Controller.DetachModule.Enable();
					Controller.BurnSmokeModule.Enable();
					Controller.SlideModule.RubOnGround(Controller.HullModule._terrainRollPitch);
				});
		}

		public void Overcharge(bool state)
		{
		}

		public Audio.MixerOption MixerOption => Audio.MixerOption.Player;
	}
}
