using TSW.Messaging;

using UnityEngine;

namespace Game.Racer.Modules
{
	public class EnergyModule : BaseModule
	{
		private EnergyController _energyController;
		public EnergyController EnergyController => _energyController;

		public override void ModuleUpdate()
		{
			_energyController.UpdateCapacity(Mathf.Abs(Controller.CommandModule.Turn), Controller.CommandModule.Boost);
		}

		protected override void ModuleInit()
		{
			base.ModuleInit();
			if (DynProperties != null)
			{
				_energyController = new EnergyController(DynProperties);
			}
		}

		[EventHandler(typeof(CollisionModule.CollisionEvent))]
		public void OnCollisionEvent(CollisionModule.CollisionEvent evt)
		{
			if (enabled)
			{
				_energyController.TriggerShield();
			}
		}
	}
}
