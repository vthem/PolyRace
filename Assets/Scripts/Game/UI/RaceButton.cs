using TSW.Messaging;

using UnityEngine;

using BDEvent = TSW.Messaging.Event;

namespace Game.UI
{
	public class RaceButton : MonoBehaviour
	{
		public class StartRaceEvent : BDEvent { }

		public void OnRaceButton()
		{
			Race.Setup.Swap();
			Dispatcher.FireEvent(new StartRaceEvent());
		}
	}
}
