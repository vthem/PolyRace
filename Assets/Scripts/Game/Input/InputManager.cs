using System.Collections.Generic;
using System.Linq;

using TSW.Design;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

using GameInputAction = Game.Input.InputAction;
using UnityInputAction = UnityEngine.InputSystem.InputAction;

namespace Game.Input
{
	public class InputManager : USingleton<InputManager>
	{
		public static event System.Action OnMouseActivityChanged;
		public static bool IsMouseActive => Instance._isMouseActive;
		public static bool MouseActivityEventState { get; set; } = true;

		public static bool GetBool(GameInputAction inputAction)
		{
			return Instance._GetBool(inputAction);
		}

		public static float GetFloat(GameInputAction inputAction)
		{
			return Instance._GetFloat(inputAction);
		}

		public static bool PressAnyKey()
		{
			return Instance._PressAnyKey();
		}

		public static GameInputAction GetInputActionFromString(string actionName)
		{
			if (actionName == "UICancel")
			{
				return InputAction.UICancel;
			}
			else if (actionName == "UISubmitSecondary")
			{
				return InputAction.UISubmitSecondary;
			}
			else if (actionName == "UISubmitPrimary")
			{
				return InputAction.UISubmitPrimary;
			}
			else if (actionName == "QuickRestart")
			{
				return InputAction.QuickRestart;
			}
			throw new System.Exception($"unmapped action name {actionName}");
		}

		public static void RegisterInputPressed(GameInputAction inputAction, System.Action callback)
		{
			Instance?._RegisterInputPressed(inputAction, callback);
		}

		public static void UnregisterInputPressed(GameInputAction inputAction, System.Action callback)
		{
			Instance?._UnregisterInputPressed(inputAction, callback);
		}

		private class InputActionCallbackContext
		{
			public GameInputAction inputAction;
			public UnityInputAction unityInputAction;
			public System.Action callback;

			public InputActionCallbackContext(GameInputAction inputAction,
				UnityInputAction unityInputAction,
				System.Action callback)
			{
				this.inputAction = inputAction;
				this.unityInputAction = unityInputAction;
				this.callback = callback;
			}
		}

		private readonly Dictionary<GameInputAction, UnityInputAction> _unityActionDict = new Dictionary<GameInputAction, UnityInputAction>();
		private readonly GameInputAction[] _actionUsingCallbackArray = {
			GameInputAction.NextCameraType,
			GameInputAction.PreviousCameraType,
			GameInputAction.NextCameraView,
			GameInputAction.PreviousCameraView,
			GameInputAction.PauseToggle,
			GameInputAction.QuickRestart,
			GameInputAction.UICancel,
			GameInputAction.UISubmitSecondary
		};
		private readonly List<InputActionCallbackContext> _inputPressed = new List<InputActionCallbackContext>();
		private InputDevice _lastUsedDevice;
		private bool _isMouseActive = false;

		protected override void SingletonCreate()
		{
			base.SingletonCreate();
			PlayerInput playerInput = GetComponent<PlayerInput>();

			foreach (GameInputAction action in System.Enum.GetValues(typeof(GameInputAction)))
			{
				_unityActionDict[action] = playerInput.actions.FindAction(GetInputActionPath(action), true);
			}

			foreach (GameInputAction inputAction in _actionUsingCallbackArray)
			{
				if (_unityActionDict.TryGetValue(inputAction, out UnityInputAction action))
				{
					action.performed += OnPerformed;
				}
			}
			InputSystem.onEvent += OnInputSystemEvent;
		}

		private bool _GetBool(GameInputAction inputAction)
		{
			return _GetFloat(inputAction) > 0f;
		}

		private string GetInputActionPath(GameInputAction inputAction)
		{
			switch (inputAction)
			{
				case GameInputAction.NextCameraType: return "Player/NextCameraType";
				case GameInputAction.PreviousCameraType: return "Player/PreviousCameraType";
				case GameInputAction.NextCameraView: return "Player/NextCameraView";
				case GameInputAction.PreviousCameraView: return "Player/PreviousCameraview";
				case GameInputAction.Boost: return "Player/Boost";
				case GameInputAction.Accelerate: return "Player/Accelerate";
				case GameInputAction.Brake: return "Player/Brake";
				case GameInputAction.Steer: return "Player/Steer";
				case GameInputAction.UICancel: return "UI/Cancel";
				case GameInputAction.UISubmitPrimary: return "UI/Click";
				case GameInputAction.UISubmitSecondary: return "UI/SubmitSecondary";
				case GameInputAction.QuickRestart: return "Player/QuickRestart";
				case GameInputAction.PauseToggle: return "Player/PauseToggle";
			}
			Debug.LogError($"path not found for {inputAction}");
			return null;
		}

		private float _GetFloat(GameInputAction inputAction)
		{
			if (_unityActionDict.TryGetValue(inputAction, out UnityInputAction action))
			{
				return action.ReadValue<float>();
			}
			Debug.LogError($"reading float for {inputAction} is not implemented");
			return 0f;
		}

		private bool _PressAnyKey()
		{
			if (Keyboard.current != null && Keyboard.current.anyKey.isPressed)
			{
				return true;
			}
			if (Mouse.current != null && (Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed))
			{
				return true;
			}
			if (Gamepad.current != null & Gamepad.current.IsPressed())
			{
				return true;
			}
			return false;
		}

		private void _RegisterInputPressed(GameInputAction inputAction, System.Action callback)
		{
			if (!_actionUsingCallbackArray.Contains(inputAction))
			{
				Debug.LogError($"{inputAction} does not support callback");
				return;
			}
			InputActionCallbackContext ctx = null;
			ctx = _inputPressed.Find((c) => c.inputAction == inputAction);

			if (null == ctx)
			{
				if (_unityActionDict.TryGetValue(inputAction, out UnityInputAction action))
				{
					ctx = new InputActionCallbackContext(inputAction, action, callback);
					_inputPressed.Add(ctx);
				}
			}
			else
			{
				ctx.callback += callback;
			}
		}

		private void _UnregisterInputPressed(GameInputAction inputAction, System.Action callback)
		{
			InputActionCallbackContext ctx = _inputPressed.Find((c) => c.inputAction == inputAction);
			if (ctx != null)
			{
				ctx.callback -= callback;
			}
		}

		private void OnPerformed(UnityInputAction.CallbackContext obj)
		{
			foreach (InputActionCallbackContext ip in _inputPressed)
			{
				if (ip.unityInputAction == obj.action)
				{
					ip.callback?.Invoke();
				}
			}
		}

		private void OnInputSystemEvent(InputEventPtr eventPtr, InputDevice device)
		{
			if (_lastUsedDevice == device)
			{
				return;
			}

			if (device == Mouse.current)
			{
				if (!_isMouseActive)
				{
					_isMouseActive = true;
					if (MouseActivityEventState)
					{
						OnMouseActivityChanged?.Invoke();
					}
				}
			}
			else
			{
				if (_isMouseActive)
				{
					_isMouseActive = false;
					if (MouseActivityEventState)
					{
						OnMouseActivityChanged?.Invoke();
					}
				}
			}
			_lastUsedDevice = device;
		}

		private void OnDestroy()
		{
			InputSystem.onEvent -= OnInputSystemEvent;
			foreach (GameInputAction inputAction in _actionUsingCallbackArray)
			{
				if (_unityActionDict.TryGetValue(inputAction, out UnityInputAction action))
				{
					action.performed -= OnPerformed;
				}
				_inputPressed.Clear();
			}
			Shut();
		}
	}
}
