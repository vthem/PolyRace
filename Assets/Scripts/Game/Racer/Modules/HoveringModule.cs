using Reaktion;

using UnityEngine;

namespace Game.Racer.Modules
{
	public class HoveringModule : BaseModule
	{
		[SerializeField]
		private JitterMotion _hoverJitterMotion;

		public override void ModuleUpdate()
		{
			UpdateHeight();
		}

		public override void Enable()
		{
			base.Enable();
			UpdateHeight();
			_hoverJitterMotion.enabled = true;
		}

		public override void Disable()
		{
			base.Disable();
			_hoverJitterMotion.enabled = false;
		}

		private void UpdateHeight()
		{
			transform.position = new Vector3(
				transform.position.x,
				Controller.DataModule.TerrainHeight + CommonProperties.HoverHeight,
				transform.position.z);
		}
	}
}
