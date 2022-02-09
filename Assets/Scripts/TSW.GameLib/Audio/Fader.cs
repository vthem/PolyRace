using System;

using DG.Tweening;

using UnityEngine;

namespace TSW.Audio
{
	[RequireComponent(typeof(AudioSource))]
	public class Fader : MonoBehaviour
	{
		[SerializeField]
		private float _fadeInDuration = .5f;

		[SerializeField]
		private float _fadeOutDuration = .5f;

		private AudioSource _audioSource;
		private AudioSource AudioSource
		{
			get
			{
				if (!_audioSource)
				{
					_audioSource = GetComponent<AudioSource>();
				}
				return _audioSource;
			}
		}

		public void FadeIn()
		{
			if (_fadeInDuration > 0f)
			{
				AudioSource audioSource = AudioSource;
				float targetVolume = audioSource.volume;
				audioSource.volume = 0f;
				DOTween.To(() => audioSource.volume, (v) => audioSource.volume = v, targetVolume, _fadeInDuration);
			}
		}

		public void FadeOut(Action onFadeEnd)
		{
			if (_fadeOutDuration > 0f)
			{
				AudioSource audioSource = AudioSource;
				DOTween.To(() => audioSource.volume, (v) => audioSource.volume = v, 0f, _fadeOutDuration)
					.SetEase(Ease.InOutQuad)
					.OnComplete(() =>
					{
						onFadeEnd?.Invoke();
					});
			}
		}
	}
}
