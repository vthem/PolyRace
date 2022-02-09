using UnityEngine;

namespace Game
{
	public static class Options
	{
		public enum AudioMixerType
		{
			Master,
			Music,
			SoundFx
		}

		public static void EnableLog()
		{
			PlayerPrefs.SetInt(TSW.Log.Logger.EnableOptionKey, 1);
			PlayerPrefs.Save();
		}

		public static void DisableLog()
		{
			PlayerPrefs.SetInt(TSW.Log.Logger.EnableOptionKey, 0);
			PlayerPrefs.Save();
		}

		public static bool IsLogEnabled()
		{
			return PlayerPrefs.GetInt(TSW.Log.Logger.EnableOptionKey, 0) == 1;
		}

		public static bool IsParticleEnabled()
		{
			return PlayerPrefs.GetInt(PlayerPrefsName.ALLOW_PARTICLE_EFFECT, 1) == 1;
		}

		public static bool IsCellShadingEnabled()
		{
			return PlayerPrefs.GetInt(PlayerPrefsName.ALLOW_CELL_SHADING, 1) == 1;
		}

		public static void SaveVolume(AudioMixerType sourceType, float volume)
		{
			switch (sourceType)
			{
				case AudioMixerType.Master:
					PlayerPrefs.SetFloat(PlayerPrefsName.MASTER_VOLUME, volume);
					break;
				case AudioMixerType.Music:
					PlayerPrefs.SetFloat(PlayerPrefsName.MUSIC_VOLUME, volume);
					break;
				case AudioMixerType.SoundFx:
					PlayerPrefs.SetFloat(PlayerPrefsName.SFX_VOLUME, volume);
					break;
			}
			PlayerPrefs.Save();
		}

		public static float GetVolume(AudioMixerType mixerType)
		{
			switch (mixerType)
			{
				case AudioMixerType.Master:
					return PlayerPrefs.GetFloat(PlayerPrefsName.MASTER_VOLUME, 1f);
				case AudioMixerType.Music:
					return PlayerPrefs.GetFloat(PlayerPrefsName.MUSIC_VOLUME, 1f);
				case AudioMixerType.SoundFx:
					return PlayerPrefs.GetFloat(PlayerPrefsName.SFX_VOLUME, 1f);
			}
			return 0f;
		}
	}
}
