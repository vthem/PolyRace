using System.Collections.Generic;

using TSW.Audio;

using DG.Tweening;

using UnityEngine.Audio;

namespace Game.Audio
{
	public class SnapshotManager
	{
		private readonly Dictionary<string, AudioMixerSnapshot> _snapshots = new Dictionary<string, AudioMixerSnapshot>();
		private string _currentSnapshot;

		public string CurrentSnapshot => _currentSnapshot;

		private Tweener _currentTransition;

		public void AddSnapshot(AudioMixer mixer, string name)
		{
			AudioMixerSnapshot snap = mixer.FindSnapshot(name);
			if (snap == null)
			{
				throw new System.Exception("Could not add snapshot " + name + " - not found");
			}
			_snapshots.Add(name, snap);
		}

		public void SetCurrent(string name)
		{
			_currentSnapshot = name;
		}

		public void TransitionTo(string to, float duration)
		{
			if (_currentTransition != null)
			{
				_currentTransition.Kill();
			}
			_currentTransition = SnapshotExtension.TransitionTo(Get(_currentSnapshot), Get(to), duration);
			_currentSnapshot = to;
		}

		private AudioMixerSnapshot Get(string name)
		{
			AudioMixerSnapshot snap;
			if (_snapshots.TryGetValue(name, out snap))
			{
				return snap;
			}
			throw new System.Exception("Snapshot " + name + " not found");
		}
	}
}
