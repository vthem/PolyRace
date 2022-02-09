using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
	public class HoverSetText : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField]
		private Text _text;

		[SerializeField]
		private string _locKey;

		[SerializeField]
		private string _defaultLocKey;

		public void OnDeselect(BaseEventData eventData)
		{
			ClearText();
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			SetText();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			ClearText();
		}

		public void OnSelect(BaseEventData eventData)
		{
			SetText();
		}

		private void SetText()
		{
			_text.text = TSW.Loca.DirtyLoca.GetTextValue(_locKey);
		}

		private void ClearText()
		{
			_text.text = TSW.Loca.DirtyLoca.GetTextValue(_defaultLocKey);
		}
	}
}