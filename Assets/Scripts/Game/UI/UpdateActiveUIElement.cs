using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI
{
	public class UpdateActiveUIElement : MonoBehaviour, ISelectHandler
	{
		public void OnSelect(BaseEventData eventData)
		{
			UIElement.Active = FindUIElementOnTransform(transform);
		}

		private UIElement FindUIElementOnTransform(Transform transform)
		{
			UIElement ui = transform.GetComponent<UIElement>();
			if (null != ui)
			{
				return ui;
			}
			if (null != transform.parent)
			{
				return FindUIElementOnTransform(transform.parent);
			}
			else
			{
				return null;
			}
		}
	}
}
