using System;
using System.Collections.Generic;

using UnityEngine;

namespace Game.Audio
{
	public class Loop
	{
		private readonly List<AudioSource> _audioSources = new List<AudioSource>();

		public Loop(string identifier, Transform parent, MixerOption mixerOption, Func<string, Func<float>> getCommandSelector = null)
		{
			_audioSources.Add(SoundFx.Instance.PlayLoop(identifier, parent, mixerOption, getCommandSelector));
			int count = 1;
			AudioSource source;
			try
			{
				while ((source = SoundFx.Instance.PlayLoop(identifier + " " + count, parent, mixerOption, getCommandSelector)) != null)
				{
					count++;
					_audioSources.Add(source);
				}
			}
			catch
			{
			}
		}

		public void End()
		{
			foreach (AudioSource source in _audioSources)
			{
				SoundFx.Instance.EndLoop(source);
			}
		}
	}
}
