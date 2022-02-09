using TSW.Messaging;

namespace Game.UI
{
	public class BonusColorModifier : ColorModifier
	{
		protected override void Awake()
		{
			Dispatcher.AddHandler(typeof(Bonus.Controller.BonusEvent), OnDistanceBonusEvent);
		}

		public void OnDistanceBonusEvent(TSW.Messaging.Event evt)
		{
			Fade(evt.GetType().Name);
		}
	}
}
