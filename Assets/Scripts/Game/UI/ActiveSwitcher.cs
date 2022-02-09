using System;
using System.Collections;

using UnityEngine;

namespace Game.UI
{
	public class ActiveSwitcher : MonoBehaviour
	{
		[SerializeField]
		private UIElement[] _UIElements;

		public UIElement Active { get; private set; }
		public UIElement Back { get; set; }

		public event Action<ActiveSwitcher> AfterSwitch;
		public event Action<ActiveSwitcher, UIElement> BeforeSwitch;

		public void Switch(string uiName, Action onComplete = null)
		{
			UIElement ui = FindByName(uiName);
			if (ui != null)
			{
				Switch(ui, onComplete);
			}
			else
			{
				Debug.LogWarning("UIElement " + uiName + " not found in ActiveSwitcher " + name);
			}
		}

		public void Switch(UIElement ui, Action onComplete = null)
		{
			if (ui == null)
			{
				Debug.LogWarning("Fail to switch UI; UIElement ui is null");
				return;
			}
			if (BeforeSwitch != null)
			{
				BeforeSwitch(this, ui);
			}

			Action displayNew = () =>
			{
				ui.AnimateDisplayState(true, () =>
				{
					UpdateActive(ui);
					if (onComplete != null)
					{
						onComplete();
					}
				});
			};
			if (Active != null && Active != ui)
			{
				Active.AnimateDisplayState(false, displayNew);
			}
			else
			{
				displayNew();
			}
		}

		public IEnumerator SwitchRoutine(string uiName)
		{
			UIElement ui = FindByName(uiName);
			if (ui != null)
			{
				yield return StartCoroutine(SwitchRoutine(ui));
			}
			else
			{
				Debug.LogWarning("UIElement " + name + " not found in ActiveSwitcher " + name);
			}
		}

		public IEnumerator SwitchRoutine(UIElement ui)
		{
			if (ui == null)
			{
				Debug.LogWarning("Fail to switch UI; UIElement ui is null");
				yield break;
			}
			if (Active != null && Active != ui)
			{
				if (BeforeSwitch != null)
				{
					BeforeSwitch(this, ui);
				}
				yield return Active.StartCoroutine(Active.AnimateDisplayState(false));
				yield return ui.StartCoroutine(ui.AnimateDisplayState(true));
				UpdateActive(ui);
			}
			else
			{
				yield return ui.StartCoroutine(ui.AnimateDisplayState(true));
				UpdateActive(ui);
			}
		}

		public IEnumerator HideActive()
		{
			if (Active != null)
			{
				yield return Active.StartCoroutine(Active.AnimateDisplayState(false));
				UpdateActive(null);
			}
		}

		public void HideActive(Action onComplete)
		{
			if (Active != null)
			{
				Active.AnimateDisplayState(false, () =>
				{
					UpdateActive(null);
					if (onComplete != null)
					{
						onComplete();
					}
				});
			}
		}

		private UIElement FindByName(string uiName)
		{
			for (int i = 0; i < _UIElements.Length; ++i)
			{
				if (_UIElements[i].name == uiName)
				{
					return _UIElements[i];
				}
			}
			return null;
		}

		private void UpdateActive(UIElement ui)
		{
			Active = ui;
			if (AfterSwitch != null)
			{
				AfterSwitch(this);
			}
		}
	}
}
