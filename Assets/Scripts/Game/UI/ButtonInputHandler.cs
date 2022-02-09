using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using GameInputManager = Game.Input.InputManager;

namespace Game.UI
{
	[RequireComponent(typeof(Button))]
	public class ButtonInputHandler : MonoBehaviour
	{
		private static readonly Dictionary<string, List<ButtonInputHandler>> _all = new Dictionary<string, List<ButtonInputHandler>>();

		[SerializeField]
		private string _actionName;

		[SerializeField]
		private int _priority = 0;

		[SerializeField]
		private GameObject _shortcutIconPrefab;
		private GameObject _shortcutIcon;

		private void Awake()
		{

		}

		private void OnEnable()
		{
			GetList().Add(this);
			CreateShortcutIcon();
			GameInputManager.RegisterInputPressed(GameInputManager.GetInputActionFromString(_actionName), InputHandler);
			StartCoroutine(EnableIconRoutine());
		}

		private IEnumerator EnableIconRoutine()
		{
			// we need a coroutine because the list of active ButtonInputHandler is not yet builded on OnEnable
			yield return null;
			SetIconState(HasHighestPriority());
		}

		private void OnDisable()
		{
			GameInputManager.UnregisterInputPressed(GameInputManager.GetInputActionFromString(_actionName), InputHandler);
			GetList().Remove(this);
		}

		private void InputHandler()
		{
			if (HasHighestPriority())
			{
				Button button = GetComponent<Button>();
				if (button.IsInteractable())
				{
					ExecuteEvents.Execute<ISubmitHandler>(button.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
					//                    GetComponent<Button>().onClick.Invoke();
				}
			}
		}

		private void CreateShortcutIcon()
		{
			if (_shortcutIcon != null)
			{
				return;
			}
			if (_shortcutIconPrefab == null)
			{
				return;
			}
			_shortcutIcon = GameObject.Instantiate(_shortcutIconPrefab);
			_shortcutIcon.transform.SetParent(transform);
			_shortcutIcon.transform.SetAsFirstSibling();
			_shortcutIcon.transform.localScale = Vector3.one;
			RectTransform rectTransform = _shortcutIcon.transform as RectTransform;
			rectTransform.anchoredPosition3D = new Vector3(20, 0, 0);
		}

		private void SetIconState(bool state)
		{
			if (_shortcutIcon != null)
			{
				_shortcutIcon.SetActive(state);
			}
		}

		private bool HasHighestPriority()
		{
			int maxPriority = 0;
			List<ButtonInputHandler> all = GetList();
			for (int i = 0; i < all.Count; ++i)
			{
				maxPriority = Mathf.Max(maxPriority, all[i]._priority);
			}
			return _priority >= maxPriority;
		}

		private List<ButtonInputHandler> GetList()
		{
			List<ButtonInputHandler> list;
			if (!_all.TryGetValue(_actionName, out list))
			{
				list = new List<ButtonInputHandler>();
				_all[_actionName] = list;
			}
			return list;
		}
	}
}
