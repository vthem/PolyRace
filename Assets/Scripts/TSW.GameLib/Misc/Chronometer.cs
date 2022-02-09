using System;

using UnityEngine;

namespace TSW
{
	public class Chronometer
	{
		private float _elapsedTime = 0f;
		private bool _running = false;
		private float _startTime;
		private float _pauseTime;
		private float _pauseDuration;
		private readonly Func<float> _timeProvider;


		public Chronometer(Func<float> timeProvider)
		{
			_timeProvider = timeProvider;
		}

		public float ElapsedTime
		{
			get
			{
				if (_running)
				{
					return _timeProvider.Invoke() - _startTime - _pauseDuration;
				}
				else
				{
					return _elapsedTime;
				}
			}
		}

		public void Start()
		{
			_startTime = _timeProvider.Invoke();
			_pauseDuration = 0f;
			_running = true;
		}

		public void Stop()
		{
			_elapsedTime = ElapsedTime;
			_running = false;
		}

		public void Pause()
		{
			_elapsedTime = ElapsedTime;
			_pauseTime = _timeProvider.Invoke();
			_running = false;
		}

		public void Resume()
		{
			_running = true;
			_pauseDuration += _timeProvider.Invoke() - _pauseTime;
		}

		public override string ToString()
		{
			return FormatTime(ElapsedTime);
		}

		public static string FormatTime(float time)
		{
			try
			{
				System.TimeSpan t = System.TimeSpan.FromSeconds(time);
				return string.Format("{0:D2}'{1:D2}''{2:D3}", t.Minutes, t.Seconds, t.Milliseconds);
			}
			catch (System.Exception)
			{
				Debug.LogWarning("Could not format time:" + time);
			}
			return "--'--''---";
		}

		public static string FormatTimeShort(float time)
		{
			try
			{
				System.TimeSpan t = System.TimeSpan.FromSeconds(time);
				return string.Format("{0:D2}'{1:D2}''", t.Minutes, t.Seconds);
			}
			catch (System.Exception)
			{
				Debug.LogWarning("Could not format time:" + time);
			}
			return "--'--''";
		}
	}
}
