using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class SwitcherButton : MonoBehaviour
	{
		[SerializeField]
		private ActiveSwitcher _switcher;

		[SerializeField]
		protected UIElement _UIElement;

		[SerializeField]
		private bool _setBackUI = false;

		private void Start()
		{
			GetComponent<Button>().onClick.AddListener(OnButtonPressed);
		}

		protected virtual void OnButtonPressed()
		{
			if (_setBackUI)
			{
				_switcher.Back = _switcher.Active;
			}
			_switcher.Switch(_UIElement);
		}
	}
}
