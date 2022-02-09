using UnityEngine;

namespace Game.Audio
{
	[RequireComponent(typeof(AudioSource))]
	public class RandomizePitch : MonoBehaviour
	{
		[SerializeField]
		private float _offset = .1f;

		private void Awake()
		{
			AudioSource src = GetComponent<AudioSource>();
			src.pitch = Random.Range(src.pitch - _offset, src.pitch + _offset);
		}
	}
}
