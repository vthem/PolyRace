using DG.Tweening;

using UnityEngine;
using UnityEngine.Audio;

namespace TSW.Audio
{
	public class SnapshotExtension : MonoBehaviour
	{
		public static Tweener TransitionTo(AudioMixerSnapshot from, AudioMixerSnapshot to, float duration)
		{
			float current = 0f;
			DG.Tweening.Core.DOSetter<float> setAction = (v) =>
			{
				AudioMixerSnapshot[] snaps = { from, to };
				float[] weights = { 1f - v, v };
				from.audioMixer.TransitionToSnapshots(snaps, weights, 0f);
				current = v;
			};
			return DOTween.To(() => current, setAction, 1f, duration).SetUpdate(true).SetEase(Ease.InOutCubic);
		}
	}
}
