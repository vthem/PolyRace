using UnityEngine;

namespace Game.Camera
{
	public class RacerHandle : Handle
	{
		private Racer.Controller _controller;
		protected Racer.Controller Controller => _controller;

		protected override void HandleInitialize()
		{
			base.HandleInitialize();
			try
			{
				_controller = Racer.Controller.GetActive("PlayerRacer");
			}
			catch (System.Exception)
			{
				Debug.LogWarning("could not find PlayerRacer");
			}
		}
	}
}
