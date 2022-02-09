using UnityEngine;

namespace Game.Racer
{
	public class PlayerRacerHelper : MonoBehaviour, IRacerHelper
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
			Controller.AudioModule.Enable();
			Controller.HoveringModule.Enable();
			Controller.GateDetection.Enable();
			Controller.GroundSmokeModule.Enable(smokePrefab);
			Controller.ExplosionModule.Enable();
			Controller.HullModule.Enable();
		}

		public void Go()
		{
			Controller.CollisionModule.Enable();
			Controller.CommandModule.Enable();
			Controller.DataModule.Enable();
			Controller.EnergyModule.Enable();
			Controller.PhysicDriverModule.Enable();
		}

		public void PassEndLine()
		{
			Controller.CommandModule.Disable();
			Controller.GateDetection.Disable();
			Controller.EnergyModule.Disable();
			Controller.PhysicDriverModule.Disable();
			Controller.PhysicDriverModule.FastStop();
		}

		public void CrashTerrain()
		{
			Controller.CommandModule.Disable();
			Controller.HullModule.Disable();
			Controller.PhysicDriverModule.Disable();
			Controller.PhysicDriverModule.Freeze();
			Controller.CollisionModule.Disable();
			Controller.transform.position = (Controller.transform.position + Controller.CollisionModule.GetCollisionPoint()) / 2f;
			Controller.AudioModule.Disable();
			Controller.HoveringModule.Disable();
			Controller.EnergyModule.Disable();
			Controller.GroundSmokeModule.Disable();
			Controller.DetachModule.Enable();
			Controller.BurnSmokeModule.Enable();
			Controller.ExplosionModule.Explode();
		}

		public void CrashGround()
		{
			Controller.CommandModule.Disable();
			Controller.HullModule.Disable();
			Controller.PhysicDriverModule.Disable();
			Controller.AudioModule.Disable();
			Controller.HoveringModule.Disable();
			Controller.EnergyModule.Disable();
			Controller.SlideModule.Enable();
			Controller.SlideModule.RegisterOnStop(() =>
			{
				Controller.GroundSmokeModule.Disable();
			});
			Controller.CollisionModule.RegisterOnCollision(
			() =>
			{
				Controller.DetachModule.Enable();
				Controller.PhysicDriverModule.SlowStop();
				Controller.BurnSmokeModule.Enable();
				Controller.SlideModule.RubOnGround(Controller.HullModule._terrainRollPitch);
			});
		}

		public Audio.MixerOption MixerOption => Audio.MixerOption.Player;
	}
}
