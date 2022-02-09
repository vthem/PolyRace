using System;
using System.Collections;

using UnityEngine;

namespace Game.Audio
{
	[RequireComponent(typeof(AudioSource))]
	public class DelayPlay : MonoBehaviour
	{
		[SerializeField]
		private float _delay = .5f;


		public void PlayDelayed(Action onPlay)
		{
			StartCoroutine(InnerPlayDelayed(onPlay));
		}

		private IEnumerator InnerPlayDelayed(Action onPlay)
		{
			yield return new WaitForSeconds(_delay);
			onPlay();
			GetComponent<AudioSource>().Play();
		}
	}
}
