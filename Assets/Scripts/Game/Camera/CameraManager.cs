using System;

using TSW;
using TSW.Messaging;

using UnityEngine;

namespace Game.Camera
{
	[System.Flags]
	public enum CameraMode
	{
		Default = 1,
		Chase = 2,
		Top = 4,
		Onboard = 8,
		Spectator = 16,
		Orbit = 32,
		MainMenu = 64
	}

	public enum CameraSceneMode
	{
		Default,
		Race,
		Replay,
		MainMenu
	}

	public class CameraManager : TSW.Design.USingleton<CameraManager>
	{
		public class CameraChangedEvent : Event<Handle> { }
		[SerializeField]
		private GameObject _defaultHandle;

		[SerializeField]
		private GameObject[] _chaseHandles;

		[SerializeField]
		private GameObject[] _spectatorHandles;

		[SerializeField]
		private GameObject[] _onboardHandles;

		[SerializeField]
		private GameObject[] _topHandles;

		[SerializeField]
		private GameObject[] _followOrbitHandles;

		[SerializeField]
		private GameObject _mainMenuHandle;

		[SerializeField]
		[BitMask(typeof(CameraMode))]
		private CameraMode _allowedDuringRace;

		[SerializeField]
		[BitMask(typeof(CameraMode))]
		private CameraMode _allowedDuringReplay;

		[SerializeField]
		private CameraMode _defaultRace;

		public delegate void CameraChangeHandler(Handle handle);
		public Handle CurrentHandle => _currentHandle;

		private CameraMode _currentMode = CameraMode.Default;
		private Handle _currentHandle;
		private CameraSceneMode _currentSceneMode = CameraSceneMode.Default;

		public void SetSceneMode(CameraSceneMode sceneMode)
		{
			switch (sceneMode)
			{
				case CameraSceneMode.Default:
					_currentMode = CameraMode.Default;
					SetCameraView(0);
					break;
				case CameraSceneMode.Race:
					_currentMode = _defaultRace;
					SetCameraView(Player.PlayerManager.Instance.GetCameraView(_currentMode));
					break;
				case CameraSceneMode.Replay:
					_currentMode = Player.PlayerManager.Instance.GetReplayCameraMode();
					SetCameraView(Player.PlayerManager.Instance.GetCameraView(_currentMode));
					break;
				case CameraSceneMode.MainMenu:
					_currentMode = CameraMode.MainMenu;
					SetCameraView(0);
					break;
			}
			_currentSceneMode = sceneMode;
		}

		public void SleepCameraTransform()
		{
			if (null != _currentHandle)
			{
				SleepCameraTransform sleep = _currentHandle.GetComponent<SleepCameraTransform>();
				if (null != sleep)
				{
					sleep.SleepTransform();
				}
				else
				{
					Debug.LogWarning("currentHandle does not have SmoothSleeper component");
				}
			}
			else
			{
				Debug.LogWarning("currentHandle is null");
			}
		}

		public void NextView(int offset)
		{
			SetCameraView(Player.PlayerManager.Instance.GetCameraView(_currentMode) + offset);
		}

		public void CameraCycle(int offset)
		{
			if (offset > Enum.GetValues(typeof(CameraMode)).Length)
			{
				// avoid infinite recursion
				return;
			}
			CameraMode newMode = ApplyOffset(_currentMode, offset);
			if (!IsCameraModeAllowed(newMode))
			{
				if (offset < 0)
				{
					--offset;
				}
				else
				{
					++offset;
				}
				CameraCycle(offset);
			}
			else
			{
				_currentMode = newMode;
				SetCameraView(Player.PlayerManager.Instance.GetCameraView(_currentMode));
				if (_currentSceneMode == CameraSceneMode.Replay)
				{
					Player.PlayerManager.Instance.SetReplayCameraMode(_currentMode);
				}
			}
		}

		private static CameraMode ApplyOffset(CameraMode mode, int offset)
		{
			CameraMode[] values = Enum.GetValues(typeof(CameraMode)) as CameraMode[];
			int newIndex = TSW.Math.Mod(GetCameraModeIndex(mode) + offset, values.Length);
			return values[newIndex];
		}

		public static int GetCameraModeIndex(CameraMode mode)
		{
			CameraMode[] values = Enum.GetValues(typeof(CameraMode)) as CameraMode[];
			int index = 0;
			for (; index < values.Length; ++index)
			{
				if (values[index] == mode)
				{
					break;
				}
			}
			return index;
		}

		private bool IsCameraModeAllowed(CameraMode mode)
		{
			bool result = true;
			switch (_currentSceneMode)
			{
				case CameraSceneMode.Default:
					break;
				case CameraSceneMode.Race:
					result = (_allowedDuringRace & mode) > 0;
					break;
				case CameraSceneMode.Replay:
					result = (_allowedDuringReplay & mode) > 0;
					break;
			}
			return result;
		}

		protected override void SingletonCreate()
		{
			DisableHandleGroup(_chaseHandles);
			DisableHandleGroup(_topHandles);
			DisableHandleGroup(_spectatorHandles);
			DisableHandleGroup(_onboardHandles);
			DisableHandleGroup(_followOrbitHandles);
			if (_mainMenuHandle != null)
			{
				_mainMenuHandle.SetActive(false);
			}
		}

		private static void DisableHandleGroup(GameObject[] group)
		{
			foreach (GameObject obj in group)
			{
				obj.SetActive(false);
			}
		}

		private void SetCameraView(int view)
		{
			GameObject[] objectHandles = null;
			switch (_currentMode)
			{
				case CameraMode.Default:
					SwitchHandle(_defaultHandle);
					return;
				case CameraMode.Chase:
					objectHandles = _chaseHandles;
					break;
				case CameraMode.Spectator:
					objectHandles = _spectatorHandles;
					break;
				case CameraMode.Onboard:
					objectHandles = _onboardHandles;
					break;
				case CameraMode.Top:
					objectHandles = _topHandles;
					break;
				case CameraMode.Orbit:
					objectHandles = _followOrbitHandles;
					break;
				case CameraMode.MainMenu:
					SwitchHandle(_mainMenuHandle);
					break;
			}
			if (objectHandles != null)
			{
				if (view < 0)
				{
					view = objectHandles.Length - 1;
				}
				else if (view >= objectHandles.Length)
				{
					view = 0;
				}
				SwitchHandle(objectHandles[view]);
				Player.PlayerManager.Instance.SetCameraView(_currentMode, view);
			}
		}

		private void SwitchHandle(GameObject newHandleObject)
		{
			if (_currentHandle != null)
			{
				_currentHandle.gameObject.SetActive(false);
			}
			newHandleObject.SetActive(true);
			_currentHandle = newHandleObject.GetComponent<Handle>();
			_currentHandle.Initialize();
			Dispatcher.FireEvent(new CameraChangedEvent().SetValue1(_currentHandle));
		}
	}
}
