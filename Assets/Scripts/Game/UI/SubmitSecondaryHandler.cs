using UnityEngine.EventSystems;

namespace Game.UI
{
	public interface ISubmitSecondaryHandler : IEventSystemHandler
	{
		void OnSecondarySubmit(BaseEventData eventData);
	}

	public static class SubmitSecondaryEvent
	{
		public static void Execute(ISubmitSecondaryHandler handler, BaseEventData eventData)
		{
			handler.OnSecondarySubmit(ExecuteEvents.ValidateEventData<BaseEventData>(eventData));
		}
	}
}