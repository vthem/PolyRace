using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
	public class OnClickSetAsBackFocus : MonoBehaviour, IPointerClickHandler, ISubmitHandler
	{
		[SerializeField]
		private UIElement _targetUIElement;

		public UIElement TargetUIElement { get => _targetUIElement; set => _targetUIElement = value; }

		public void OnPointerClick(PointerEventData eventData)
		{
			SetBackFocus();
		}

		public void OnSubmit(BaseEventData eventData)
		{
			SetBackFocus();
		}

		private void SetBackFocus()
		{
			if (_targetUIElement == null)
			{
				return;
			}
			//            Debug.Log("Set BackFocus on " + _targetUIElement.name + " to " + gameObject.name);
			_targetUIElement.BackTargetFocus = GetComponent<Selectable>();
		}
	}
}