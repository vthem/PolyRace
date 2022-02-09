using UnityEngine;

namespace TSW.Audio
{
	[RequireComponent(typeof(AudioSource))]
	public class VolumeModifier : FloatModifier
	{
		private AudioSource _audioSource;

		protected override void OnStart()
		{
			_audioSource = GetComponent<AudioSource>();
		}

		protected override void UpdateValue(float value)
		{
			_audioSource.volume = value;
		}

		protected override float GetValue()
		{
			return _audioSource.volume;
		}
	}
}
