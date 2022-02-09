using System;
using System.Collections;

using TSW.Design;

using UnityEngine;

namespace Game.Player
{
	public class PlayerManager : USingleton<PlayerManager>
	{
		private const int DataVersion = 1;
		private int[] _cameraView;

		public string DisplayName { get; set; }

		private string DataFile => TSW.Crypto.Hash(DisplayName) + ".player-data";

		/// <summary>
		/// Initialized when ready to be used
		/// </summary>
		public bool Initialized { get; private set; }
		public event Action OnInitialized;

		/// <summary>
		/// Write allowed if data has been loaded or created (for the first time)
		/// </summary>
		private bool _writeAllowed = false;

		protected override void SingletonCreate()
		{
			Initialized = false;
		}

		public void Initialize()
		{
			DisplayName = "OFFLINE";
			ValidatePlayerData();
			Initialized = true;

			if (OnInitialized != null)
			{
				OnInitialized();
			}
		}

		public void SetCameraView(Camera.CameraMode mode, int cameraView)
		{
			if (_cameraView[Camera.CameraManager.GetCameraModeIndex(mode)] != cameraView)
			{
				_cameraView[Camera.CameraManager.GetCameraModeIndex(mode)] = cameraView;
				Save("SetCameraView");
			}
		}

		public int GetCameraView(Camera.CameraMode mode)
		{
			return _cameraView[Camera.CameraManager.GetCameraModeIndex(mode)];
		}

		public void SetReplayCameraMode(Camera.CameraMode mode)
		{
			PlayerPrefs.SetInt(PlayerPrefsName.REPLAY_CAMERA_MODE, (int)mode);
		}

		public Camera.CameraMode GetReplayCameraMode()
		{
			return (Camera.CameraMode)PlayerPrefs.GetInt(PlayerPrefsName.REPLAY_CAMERA_MODE, (int)Camera.CameraMode.Spectator);
		}

		private static readonly UnlockReward.RewardId[] Ranks = new UnlockReward.RewardId[] {
			UnlockReward.RewardId.Adept,
			UnlockReward.RewardId.Skilled,
			UnlockReward.RewardId.Master,
			UnlockReward.RewardId.Grandmaster
		};

		public void SetLanguage(string code)
		{
			PlayerPrefs.SetString(PlayerPrefsName.LANGUAGE, code);
		}

		public string GetLanguage()
		{
			return PlayerPrefs.GetString(PlayerPrefsName.LANGUAGE, "en");
		}

		public SpeedSystemType GetSpeedUnit()
		{
			return (SpeedSystemType)PlayerPrefs.GetInt(PlayerPrefsName.SPEED_UNIT, 0);
		}

		public void SetSpeedUnit(SpeedSystemType systemType)
		{
			PlayerPrefs.SetInt(PlayerPrefsName.SPEED_UNIT, (int)systemType);
		}

		public void ClearData()
		{
			PlayerPrefs.DeleteKey(PlayerPrefsName.SPEED_UNIT);
			PlayerPrefs.DeleteKey(PlayerPrefsName.LANGUAGE);
			PlayerPrefs.DeleteKey(PlayerPrefsName.SFX_VOLUME);
			PlayerPrefs.DeleteKey(PlayerPrefsName.MUSIC_VOLUME);
		}

		public void SetLastPlayedMission(int missionIndex)
		{
			PlayerPrefs.SetInt(PlayerPrefsName.LAST_PLAYED_MISSION, missionIndex);
		}

		public int GetLastPlayedMission()
		{
			return PlayerPrefs.GetInt(PlayerPrefsName.LAST_PLAYED_MISSION, 1);
		}

		private IEnumerator Load(string userId)
		{
			Debug.Log("PlayerManager loading [" + userId + "]");
			DisplayName = userId;

			{
				Debug.Log("No existing data - creating data");
				_writeAllowed = true;
				ValidatePlayerData();
				Initialized = true;
				Save("FirstCreate");
			}

			if (OnInitialized != null)
			{
				OnInitialized();
			}
			Debug.Log("PlayerManager loaded [" + userId + "]");

			yield break;
		}

		private void Save(string reason)
		{
			if (!_writeAllowed)
			{
				return;
			}

			try
			{
				SerializableData sData = new SerializableData
				{
					cameraView = _cameraView,
					version = DataVersion,
					utc = DateTime.UtcNow.Second
				};

				byte[] data = TSW.ObjectSerializer.SerializeLZF(sData);
				Debug.Log("Saving PlayerData reason:" + reason);
			}
			catch (System.Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		private void ValidatePlayerData()
		{
			if (_cameraView == null || _cameraView.Length != System.Enum.GetValues(typeof(Camera.CameraMode)).Length)
			{
				_cameraView = new int[System.Enum.GetValues(typeof(Camera.CameraMode)).Length];
			}
		}
	}
}
