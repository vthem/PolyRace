using UnityEngine;

namespace Game.Audio
{
	public class RandomizeAudioClip : MonoBehaviour
	{
		[SerializeField]
		private AudioClip[] _audioClips;

		public void Randomize()
		{
			AudioSource audioSource = GetComponent<AudioSource>();
			int idx = Random.Range(0, _audioClips.Length);
			audioSource.clip = _audioClips[idx];
		}
	}
}
