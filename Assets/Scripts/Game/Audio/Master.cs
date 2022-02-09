using TSW.Design;

using UnityEngine;
using UnityEngine.Audio;

namespace Game.Audio
{
	public class Master : USingleton<Master>
	{
		[SerializeField]
		private AudioMixerGroup _masterMixerGroup;

		[SerializeField]
		private AudioMixerGroup _soundFxMixerGroup;

		[SerializeField]
		private AudioMixerGroup _musicMixerGroup;

		private AudioMixer GetAudioMixer(Game.Options.AudioMixerType mixerType)
		{
			switch (mixerType)
			{
				case Options.AudioMixerType.Master:
					return _masterMixerGroup.audioMixer;
				case Options.AudioMixerType.Music:
					return _musicMixerGroup.audioMixer;
				case Options.AudioMixerType.SoundFx:
					return _soundFxMixerGroup.audioMixer;
			}
			return null;
		}

		public float GetVolume(Game.Options.AudioMixerType mixerType)
		{
			float v;
			GetAudioMixer(mixerType).GetFloat("MasterVolume", out v);
			return TSW.Audio.Convert.ToLinear(v);
		}

		public void SetVolume(Game.Options.AudioMixerType mixerType, float volume)
		{
			GetAudioMixer(mixerType).SetFloat("MasterVolume", TSW.Audio.Convert.ToDecibel(volume));
		}

		private void Start()
		{
			foreach (Options.AudioMixerType type in System.Enum.GetValues(typeof(Options.AudioMixerType)))
			{
				SetVolume(type, Game.Options.GetVolume(type));
			}
		}
	}
}