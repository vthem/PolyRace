using System.Collections;
using System.Collections.Generic;

using TSW.Messaging;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using BDEvent = TSW.Messaging.Event;

namespace Game.UI
{
	public interface INavigationOption
	{
		// possible buttons:
		// return new string[] { "Resume_Button", "Restart_Button", "Options_Button", "Quit_Button", "MainMenu_Button", "ExitOptions_Button", "ExitLeaderboard_Button", "DisplaySummary_Button"};
		string[] GetActiveButton();
		string GetFocusButton();
	}

	public class Navigation : UIElement
	{
		public class RestartRaceEvent : BDEvent { }
		public class ResumeRaceEvent : BDEvent { }
		public class DisplayMainMenuEvent : BDEvent { }

		[SerializeField]
		private ActiveSwitcher _activeSwitcher;

		public void OnResumeButton()
		{
			Dispatcher.FireEvent(new ResumeRaceEvent());
		}

		public void OnRestartButton()
		{
			Dispatcher.FireEvent(new RestartRaceEvent());
		}

		public void OnCancelButton()
		{
			Race.Setup.Restore();
			_activeSwitcher.Switch(_activeSwitcher.Back);
		}

		public void OnBackButton()
		{
			_activeSwitcher.Switch(_activeSwitcher.Back);
		}

		public void OnMainMenuButton()
		{
			Dispatcher.FireEvent(new DisplayMainMenuEvent());
		}

		public override void SetFocus()
		{
			SmartFocus();
		}

		private void SmartFocus()
		{
			if (EventSystem.current.currentSelectedGameObject == null || !EventSystem.current.currentSelectedGameObject.activeInHierarchy)
			{
				Transform layout = transform.Find("Layout_Group");
				Selectable focusButton = null;
				if (_activeSwitcher.Active != null)
				{
					INavigationOption navOption = _activeSwitcher.Active as INavigationOption;
					if (navOption != null)
					{
						Transform buttonTransform = layout.Find(navOption.GetFocusButton());
						if (buttonTransform != null)
						{
							focusButton = buttonTransform.GetComponent<Selectable>();
						}
					}
				}

				if (focusButton != null && SetFocus(focusButton))
				{
					return;
				}
				// set focus on the first button available
				foreach (Transform child in layout)
				{
					Selectable button = child.GetComponent<UnityEngine.UI.Selectable>();
					if (button == null)
					{
						continue;
					}
					if (button.gameObject.activeInHierarchy)
					{
						SetFocus(button);
						break;
					}
				}
			}
			else
			{
				//                Debug.Log("SmartFocus, already selected");
			}
		}

		protected override IEnumerator OnHide()
		{
			StartCoroutine(_activeSwitcher.HideActive());
			yield return null;
		}

		public void AdjustSize()
		{
			Transform layout = transform.Find("Layout_Group");
			if (layout == null)
			{
				Debug.LogWarning("Layout_Group not found as child of " + name);
				return;
			}
			HorizontalLayoutGroup hozLayoutGroup = layout.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>();
			float width = hozLayoutGroup.padding.right + hozLayoutGroup.padding.left;
			int activeCount = 0;
			foreach (RectTransform child in (layout as RectTransform))
			{
				if (child.gameObject.activeSelf)
				{
					activeCount += 1;
					width += LayoutUtility.GetMinWidth(child);
				}
			}
			width += (activeCount - 1) * hozLayoutGroup.spacing;
			RectTransform myRect = transform as RectTransform;
			myRect.sizeDelta = new Vector2(width, myRect.sizeDelta.y);
		}

		public override void Initialize()
		{
			base.Initialize();
			_activeSwitcher.BeforeSwitch += BeforeSwitch;
			_activeSwitcher.AfterSwitch += AfterSwitch;
			Game.Input.InputManager.OnMouseActivityChanged += OnPlayerInputChanged;
		}

		private void BeforeSwitch(ActiveSwitcher switcher, UIElement newUI)
		{
			if (newUI == null)
			{
				AnimateDisplayState(false, null);
				return;
			}
			if (NeedReload(newUI as INavigationOption))
			{
				StartCoroutine(AnimateHide());
			}
			GetComponent<CanvasGroup>().interactable = false;
		}

		private void AfterSwitch(ActiveSwitcher switcher)
		{
			if (switcher.Active == null)
			{
				if (DisplayState)
				{
					AnimateDisplayState(false, null);
				}
				return;
			}

			INavigationOption navOption = switcher.Active as INavigationOption;
			if (NeedReload(navOption))
			{
				UpdateEnabledButtons(navOption);
			}

			if (!DisplayState)
			{
				AnimateDisplayState(true, null);
				GetComponent<CanvasGroup>().interactable = true;
			}
			else
			{
				StartCoroutine(AnimateDisplay());
				GetComponent<CanvasGroup>().interactable = true;
				SmartFocus();
			}
		}

		private void OnPlayerInputChanged()
		{
			if (!Input.InputManager.IsMouseActive && DisplayState)
			{
				SmartFocus();
			}
		}

		private bool NeedReload(INavigationOption navOption)
		{
			Transform layout = transform.Find("Layout_Group");
			if (navOption == null)
			{
				Debug.LogWarning("UIElement " + _activeSwitcher.Active.name + " does not implement INavigationOption");
			}
			else
			{
				List<string> activeButtons = new List<string>(navOption.GetActiveButton());
				foreach (Transform button in layout)
				{
					if (button.gameObject.activeSelf && !activeButtons.Contains(button.name)
						|| !button.gameObject.activeSelf && activeButtons.Contains(button.name))
					{
						return true;
					}
				}
			}
			return false;
		}

		private void UpdateEnabledButtons(INavigationOption navOption)
		{
			Transform layout = transform.Find("Layout_Group");
			if (navOption == null)
			{
				return;
			}
			else
			{
				List<string> activeButtons = new List<string>(navOption.GetActiveButton());
				foreach (Transform button in layout)
				{
					button.gameObject.SetActive(activeButtons.Contains(button.name));
				}
			}
		}

		private void Update()
		{
			AdjustSize();
		}
	}
}
