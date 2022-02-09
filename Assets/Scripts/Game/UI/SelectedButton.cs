using System.Reflection;

using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class SelectedButton : MonoBehaviour
	{
		public Color _selectedColor;
		public ActiveSwitcher _activeSwitcher;
		public UIElement _uiElement;
		private Color _defaultColor;
		private readonly MethodInfo _methodInfo;
		private readonly Component _component;
		private bool _initialized = false;

		public void SetAsSelected()
		{
			Initialize();
			Image image = GetComponent<Image>();
			image.color = _selectedColor;
		}

		public void SetAsNotSelected()
		{
			Initialize();
			Image image = GetComponent<Image>();
			image.color = _defaultColor;
		}

		private void Awake()
		{
			Initialize();
		}

		private void Initialize()
		{
			if (!_initialized)
			{
				_defaultColor = GetComponent<Image>().color;
				if (_activeSwitcher != null)
				{
					_activeSwitcher.AfterSwitch += OnActiveChanged;
				}
				_initialized = true;
			}
		}

		private void OnActiveChanged(ActiveSwitcher obj)
		{
			if (_activeSwitcher.Active == _uiElement)
			{
				SetAsSelected();
			}
			else
			{
				SetAsNotSelected();
			}
		}
	}
}
