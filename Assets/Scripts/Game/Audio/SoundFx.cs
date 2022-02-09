using System;

using TSW;
using TSW.Audio;
using TSW.Design;

using UnityEngine;
using UnityEngine.Audio;

namespace Game.Audio
{
	public class SoundFx : USingleton<SoundFx>
	{
		[SerializeField]
		private string _soundLocation = "Audio/SoundFx/";

		[SerializeField]
		private AudioMixerGroup _masterSoundFxMixerGroup;

		[SerializeField]
		private float _exitCavernDuration = 5f;

		[SerializeField]
		private float _defaultDuration = .2f;
		private const string ReverbAudioSnapshot = "Reverb";
		private const string DefaultAudioSnapshot = "Default";
		private const string PauseAudioSnapshot = "Pause";
		private SnapshotManager _snapshotManager;
		private bool _insideCavern = false;
		private bool _inPause = false;

		public AudioSource Play(string id)
		{
			return Play(id, null);
		}

		public AudioSource Play(string id, Transform parent, MixerOption mixerOption = MixerOption.Default)
		{
			return Play(GetAudioSource(id), parent, mixerOption);
		}

		public AudioSource Play(AudioSource audioSource, Transform parent, MixerOption mixerOption = MixerOption.Default)
		{
			if (_masterSoundFxMixerGroup == null)
			{
				Debug.LogWarning("No sound can be played - MasterSoundFxMixerGroup not set in SoundFX object");
				return null;
			}
			AudioMixerGroup mixerGroup = audioSource.outputAudioMixerGroup;
			if (mixerOption != MixerOption.Default)
			{
				AudioMixerGroup[] mixers = _masterSoundFxMixerGroup.audioMixer.FindMatchingGroups(mixerOption + "/" + mixerGroup.name);
				if (mixers.Length == 0)
				{
					throw new System.Exception("Could not find mixer group name:" + mixerOption + "/" + mixerGroup.name + " in " + _masterSoundFxMixerGroup.audioMixer.name);
				}
				mixerGroup = _masterSoundFxMixerGroup.audioMixer.FindMatchingGroups(mixerOption + "/" + mixerGroup.name)[0];
			}
			audioSource.outputAudioMixerGroup = mixerGroup;
			if (null != parent)
			{
				audioSource.transform.SetParent(parent);
			}
			audioSource.transform.localPosition = Vector3.zero;
			Randomize(audioSource);
			SetAutoDestroy(audioSource);
			Play(audioSource);
			return audioSource;
		}

		public AudioSource PlayLoop(string identifier, Transform parent, MixerOption mixerOption, Func<string, Func<float>> getCommandSelector = null)
		{
			GameObject audioObj = GetAudioGameObject(identifier);
			AudioSource audioSource = audioObj.GetComponent<AudioSource>();
			Play(audioSource, parent, mixerOption);
			FloatModifier[] modifiers = audioSource.GetComponents<FloatModifier>();
			if (getCommandSelector != null)
			{
				foreach (FloatModifier modifier in modifiers)
				{
					modifier.SetGetCommand(getCommandSelector);
				}
			}
			return audioSource;
		}

		public void EndLoop(AudioSource audioSource)
		{
			if (null == audioSource)
			{
				return;
			}
			audioSource.transform.SetParent(null, true);
			foreach (FloatModifier modifier in audioSource.GetComponents<FloatModifier>())
			{
				modifier.enabled = false;
			}
			Fader fader = audioSource.GetComponent<Fader>();
			if (fader)
			{
				fader.FadeOut(() =>
				{
					GameObject.Destroy(audioSource.gameObject);
				});
			}
			else
			{
				GameObject.Destroy(audioSource.gameObject);
			}
		}

		public GameObject GetAudioGameObject(string id)
		{
			string fullName = _soundLocation + id;
			UnityEngine.Object resourceObj = Resources.Load(fullName);
			if (null == resourceObj)
			{
				throw new System.Exception("Audio SoundFx not found:" + fullName);
			}
			GameObject obj = GameObject.Instantiate(resourceObj) as GameObject;
			if (null == obj)
			{
				throw new System.Exception("Could not instantiate " + fullName);
			}
			obj.name = id + "_SoundFx";

			return obj;
		}

		public void ToggleCavern()
		{
			if (_insideCavern)
			{
				_snapshotManager.TransitionTo(DefaultAudioSnapshot, _exitCavernDuration);
			}
			else
			{
				_snapshotManager.TransitionTo(ReverbAudioSnapshot, _defaultDuration);
			}
			_insideCavern = !_insideCavern;
		}

		public void TogglePause()
		{
			if (_inPause && _insideCavern)
			{
				_snapshotManager.TransitionTo(ReverbAudioSnapshot, _defaultDuration);
			}
			else if (_inPause && !_insideCavern)
			{
				_snapshotManager.TransitionTo(DefaultAudioSnapshot, _defaultDuration);
			}
			else
			{
				_snapshotManager.TransitionTo(PauseAudioSnapshot, _defaultDuration);
			}
			_inPause = !_inPause;
		}

		public void ApplyDefault()
		{
			_snapshotManager.TransitionTo(DefaultAudioSnapshot, _defaultDuration);
			_inPause = _insideCavern = false;
		}

		private AudioSource GetAudioSource(string id)
		{
			return GetAudioGameObject(id).GetComponent<AudioSource>();
		}

		private static void Randomize(AudioSource audioSource)
		{
			RandomizeAudioClip randomize = audioSource.GetComponent<RandomizeAudioClip>();
			if (null != randomize)
			{
				randomize.Randomize();
			}
		}

		private static void SetAutoDestroy(AudioSource audioSource)
		{
			if (!audioSource.loop)
			{
				GameObject.Destroy(audioSource.gameObject, audioSource.clip.length * 2f);
			}
		}

		private static void Play(AudioSource audioSource)
		{
			Action fadeIn = () =>
			{
				Fader fader = audioSource.GetComponent<Fader>();
				if (null != fader)
				{
					fader.FadeIn();
				}
			};
			DelayPlay delay = audioSource.GetComponent<DelayPlay>();
			if (null != delay)
			{
				delay.PlayDelayed(fadeIn);
			}
			else
			{
				fadeIn();
				audioSource.Play();
			}
		}

		private void Start()
		{
			if (_masterSoundFxMixerGroup == null)
			{
				Debug.LogWarning("No sound can be played - MasterSoundFxMixerGroup not set in SoundFX object");
				return;
			}
			_snapshotManager = new SnapshotManager();
			_snapshotManager.AddSnapshot(_masterSoundFxMixerGroup.audioMixer, DefaultAudioSnapshot);
			_snapshotManager.AddSnapshot(_masterSoundFxMixerGroup.audioMixer, ReverbAudioSnapshot);
			_snapshotManager.AddSnapshot(_masterSoundFxMixerGroup.audioMixer, PauseAudioSnapshot);
			_snapshotManager.SetCurrent(DefaultAudioSnapshot);
		}
	}
}
