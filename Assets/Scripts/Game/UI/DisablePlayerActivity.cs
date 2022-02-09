using UnityEngine.EventSystems;

namespace Game.UI
{
	public class DisablePlayerActivity : UIBehaviour, IPointerClickHandler, IDeselectHandler
	{
		public void OnDeselect(BaseEventData eventData)
		{
			Input.InputManager.MouseActivityEventState = true;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			Input.InputManager.MouseActivityEventState = false;
		}
	}
}