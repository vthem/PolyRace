using System.Collections;

using TSW.Audio;

using UnityEngine;
using UnityEngine.Audio;

namespace Game.Audio
{
	public class Music : TSW.Design.USingleton<Music>
	{
		[System.Serializable]
		public class RaceMusicClip
		{
			public AudioClip _intro;
			public AudioClip _race;
		}

		[SerializeField]
		private RaceMusicClip[] _raceClips;

		[SerializeField]
		private AudioSource _introSource;

		[SerializeField]
		private AudioSource _raceSource;

		[SerializeField]
		private AudioMixerGroup _musicAudioMixer;
		private const string NoMusicAudioSnapshot = "NoMusic";
		private const string RaceAudioSnapshot = "Race";
		private const string MenuAudioSnapshot = "Menu";
		private SnapshotManager _snapshotManager;
		private AudioSource _menuSource;
		private double _introStartTimestamp;
		private double _introLeft;
		private MusicPlayState _playState;

		private enum MusicPlayState
		{
			PlayMenu,
			PlayRace,
			PlayMenuRacePaused
		}

		private void Start()
		{
			_menuSource = GetComponent<AudioSource>();
			_menuSource.Play();
			GetComponent<Fader>().FadeIn();
			_playState = MusicPlayState.PlayMenu;
			_snapshotManager = new SnapshotManager();
			_snapshotManager.AddSnapshot(_musicAudioMixer.audioMixer, NoMusicAudioSnapshot);
			_snapshotManager.AddSnapshot(_musicAudioMixer.audioMixer, RaceAudioSnapshot);
			_snapshotManager.AddSnapshot(_musicAudioMixer.audioMixer, MenuAudioSnapshot);
			_snapshotManager.SetCurrent(MenuAudioSnapshot);
			_snapshotManager.TransitionTo(MenuAudioSnapshot, 0.1f);
		}

		private Coroutine _playCoroutine;
		public void PlayRandom(int seed)
		{
			System.Random rand = new System.Random(seed);
			if (_playCoroutine != null)
			{
				StopCoroutine(_playCoroutine);
			}
			_playCoroutine = StartCoroutine(Play(rand.Next(0, _raceClips.Length)));
		}

		public void PlayMenu()
		{
			if (_playState == MusicPlayState.PlayRace)
			{
				_raceSource.Stop();
				_introLeft = _introSource.clip.length - (AudioSettings.dspTime - _introStartTimestamp);
				if (_introLeft > 0.0)
				{
					_introSource.Stop();
				}
				_snapshotManager.TransitionTo(MenuAudioSnapshot, .2f);
			}
			_playState = MusicPlayState.PlayMenu;
		}

		private IEnumerator Play(int id)
		{
			if (id >= _raceClips.Length)
			{
				yield break;
			}
			_snapshotManager.TransitionTo(NoMusicAudioSnapshot, 0.2f);
			yield return new WaitForSeconds(0.2f);

			AudioClip intro = _raceClips[id]._intro;
			AudioClip race = _raceClips[id]._race;
			_introSource.clip = intro;
			_raceSource.clip = race;
			_introSource.Play();
			_introLeft = 0.0;
			_introStartTimestamp = AudioSettings.dspTime;
			_raceSource.Stop();
			_raceSource.PlayScheduled(_introStartTimestamp + intro.length);
			_snapshotManager.TransitionTo(RaceAudioSnapshot, .2f);
			_playState = MusicPlayState.PlayRace;
		}

		public void MuteMusic()
		{
			_snapshotManager.TransitionTo(NoMusicAudioSnapshot, 1f);
		}


		public void TogglePause()
		{
			if (_playState == MusicPlayState.PlayRace)
			{
				_playState = MusicPlayState.PlayMenuRacePaused;
				_raceSource.Pause();
				_introLeft = _introSource.clip.length - (AudioSettings.dspTime - _introStartTimestamp);
				if (_introLeft > 0.0)
				{
					_introSource.Pause();
				}
				_snapshotManager.TransitionTo(MenuAudioSnapshot, .2f);
			}
			else if (_playState == MusicPlayState.PlayMenuRacePaused)
			{
				_playState = MusicPlayState.PlayRace;
				if (_introLeft > 0.0)
				{
					_raceSource.SetScheduledStartTime(AudioSettings.dspTime + _introLeft);
					_introStartTimestamp = AudioSettings.dspTime - _introLeft;
					_introSource.UnPause();
				}
				_raceSource.UnPause();
				_snapshotManager.TransitionTo(RaceAudioSnapshot, .2f);
			}
		}
	}
}
