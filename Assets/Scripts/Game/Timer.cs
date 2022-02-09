using System;
using System.Collections.Generic;

using TSW.Design;

using UnityEngine;

namespace Game
{
	public class Timer : USingleton<Timer>
	{
		private class Handle
		{
			public bool TimeScaleIndependent { get; private set; }
			public string Name { get; private set; }
			//public Action Callback { get; private set; }
			public float Duration { get; private set; }
			public bool Active { get; set; }
			public float Scale { get; set; }

			private class TimedCallback
			{
				public Action Callback { get; private set; }
				public float Time { get; private set; }

				public TimedCallback(Action callback, float time)
				{
					Callback = callback;
					Time = time;
				}
			}

			private readonly Stack<TimedCallback> _callbacks = new Stack<TimedCallback>();

			public Handle(string name, bool timeScaleIndependent, float duration, Action callback)
			{
				Name = name;
				TimeScaleIndependent = timeScaleIndependent;
				Duration = duration;
				_callbacks.Push(new TimedCallback(callback, 0));
				Active = true;
				Scale = 1.0f;
			}

			public void Reset(float duration)
			{
				Duration = duration;
			}

			public Action Update(float delta)
			{
				if (!Active)
				{
					return null;
				}
				if (TimeScaleIndependent)
				{
					Duration -= delta * Scale;
				}
				else
				{
					Duration -= delta * Time.timeScale * Scale;
				}
				Duration = Mathf.Max(0, Duration);
				return GetCallback();
			}

			public void Add(float value)
			{
				Duration += value;
			}

			public void AddCallback(float time, Action callback)
			{
				if (_callbacks.Count > 0 && time <= _callbacks.Peek().Time)
				{
					return;
				}
				_callbacks.Push(new TimedCallback(callback, time));
			}

			private Action GetCallback()
			{
				if (_callbacks.Count > 0 && Duration <= _callbacks.Peek().Time)
				{
					return _callbacks.Pop().Callback;
				}
				return null;
			}

			public bool IsValid()
			{
				return _callbacks.Count > 0;
			}
		}

		private readonly Dictionary<string, Handle> _handles = new Dictionary<string, Handle>();

		public void Arm(string name, bool timeScaleIndependent, float duration, Action callback)
		{
			if (!_handles.ContainsKey(name))
			{
				_handles.Add(name, new Handle(name, timeScaleIndependent, duration, callback));
			}
			else
			{
				_handles[name].Reset(duration);
			}
		}

		public void Cancel(string name)
		{
			_handles.Remove(name);
		}

		public float CurrentTime(string name)
		{
			Handle handle = null;
			if (_handles.TryGetValue(name, out handle))
			{
				return handle.Duration;
			}
			return 0f;
		}

		public void SetTimerState(string name, bool state)
		{
			Handle handle = null;
			if (_handles.TryGetValue(name, out handle))
			{
				handle.Active = state;
			}
		}

		public void SetTimerScale(string name, float scale)
		{
			Handle handle = null;
			if (_handles.TryGetValue(name, out handle))
			{
				handle.Scale = scale;
			}
		}

		public void Add(string name, float value)
		{
			Handle handle = null;
			if (_handles.TryGetValue(name, out handle))
			{
				handle.Add(value);
			}
		}

		public void AddCallback(string name, float time, Action callback)
		{
			Handle handle = null;
			if (_handles.TryGetValue(name, out handle))
			{
				handle.AddCallback(time, callback);
			}
		}

		protected override void SingletonCreate()
		{
			_lastUpdate = Time.realtimeSinceStartup;
		}

		private float _lastUpdate;

		private void Update()
		{
			float delta = Time.realtimeSinceStartup - _lastUpdate;
			_lastUpdate = Time.realtimeSinceStartup;
			List<Handle> toBeDeleted = new List<Handle>();
			List<Action> callbacks = new List<Action>();

			foreach (KeyValuePair<string, Handle> kv in _handles)
			{
				Action callback = kv.Value.Update(delta);
				if (null != callback)
				{
					callbacks.Add(callback);
				}
				if (!kv.Value.IsValid())
				{
					toBeDeleted.Add(kv.Value);
				}
			}
			foreach (Action callback in callbacks)
			{
				callback();
			}

			foreach (Handle handle in toBeDeleted)
			{
				_handles.Remove(handle.Name);
			}
		}
	}
}
