using TSW;

using UnityEngine;

namespace Game
{
	public class PauseState
	{
		private readonly UI.HUD _hudUI;
		private readonly bool _hudState;
		private readonly UI.ActiveSwitcher _mainUISwitcher;
		private readonly DistanceScore _distanceScore;
		private readonly Chronometer _chrono;
		private readonly UI.InRaceNotificationManager _noticeManager;

		public PauseState(UI.HUD HUDUI, UI.ActiveSwitcher mainUISwitcher, UI.InRaceNotificationManager noticeManager, DistanceScore distanceScore, Chronometer chrono)
		{
			_hudUI = HUDUI;
			_hudState = _hudUI.DisplayState;
			_mainUISwitcher = mainUISwitcher;
			_distanceScore = distanceScore;
			_chrono = chrono;
			_noticeManager = noticeManager;

			_mainUISwitcher.Switch("Pause_UIElement");
			if (_hudState)
			{
				_hudUI.AnimateDisplayState(false, null);
			}
			_distanceScore.PauseScoring();
			_chrono.Pause();
			_noticeManager.Hide();
			Audio.Music.Instance.TogglePause();
			Audio.SoundFx.Instance.TogglePause();
			Time.timeScale = 0f;
		}

		public void Resume()
		{
			_mainUISwitcher.HideActive(() =>
			{
				if (_hudState)
				{
					_hudUI.AnimateDisplayState(true, null);
				}

				_distanceScore.ResumeScoring();
				_chrono.Resume();
				_noticeManager.Display();
				Audio.Music.Instance.TogglePause();
				Audio.SoundFx.Instance.TogglePause();
				Clear();
			});
		}

		public void Clear()
		{
			Time.timeScale = 1f;
		}
	}
}
