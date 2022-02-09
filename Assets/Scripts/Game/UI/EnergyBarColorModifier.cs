using TSW.Messaging;

namespace Game.UI
{
	public class EnergyBarColorModifier : ColorModifier
	{
		[EventHandler(typeof(Racer.EnergyController.ShieldActivatedEvent))]
		public void OnShieldActive(Event evt)
		{
			Fade(evt.GetType().Name);
		}
	}
}
