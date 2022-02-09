using System;
using System.Collections.Generic;

using TSW;

using UnityEngine;

namespace Game.Ghost
{
	public class Synchronizer : MonoBehaviour
	{
		private Recorder _recorder;
		private readonly Dictionary<string, Player> _players = new Dictionary<string, Player>();
		private Func<FrameProperties> _getFrameFunc;
		private float _maxInterval;
		private bool _firstLoop = false;
		private RecordProperties _recordProperties;
		private readonly Chronometer _chrono = new Chronometer(() => { return Time.fixedTime; });

		public int PlayerCount => _players.Count;

		public static Synchronizer CreateSynchronizer(float maxInterval, RecordProperties recordProperties)
		{
			GameObject obj = new GameObject("GhostSynchronizer");
			Synchronizer sync = obj.AddComponent<Synchronizer>();
			sync.enabled = false;
			sync._maxInterval = maxInterval;
			sync._recordProperties = recordProperties;
			return sync;
		}

		public static Synchronizer CreateSynchronizer()
		{
			GameObject obj = new GameObject("GhostSynchronizer");
			Synchronizer sync = obj.AddComponent<Synchronizer>();
			sync.enabled = false;
			return sync;
		}

		public static void DestroySynchronizer(Synchronizer synchronizer)
		{
			GameObject.Destroy(synchronizer.gameObject);
		}

		public void SetProvider(Func<FrameProperties> func)
		{
			_getFrameFunc = func;
		}

		public void SetEndRaceType(EndRaceType endRaceType)
		{
			Log("Set end race type:" + endRaceType);
			_recorder.SetEndRaceType(endRaceType);
		}

		public static Player CreatePlayer(Record record)
		{
			Player player = new Player();
			player.SetData(record);
			return player;
		}

		public Player GetPlayer(string name)
		{
			Player player = null;
			_players.TryGetValue(name, out player);
			return player;
		}

		public Player GetReplay()
		{
			if (_recorder == null)
			{
				return null;
			}
			Player player = new Player();
			player.SetData(_recorder.Record);
			return player;
		}

		public void AddModifier(string name, Player player, Action<FrameProperties> applyFrameFunc)
		{
			player.ApplyFrameFunc = applyFrameFunc;
			_players[name] = player;
		}

		private void OnEnable()
		{
			_firstLoop = true;
		}

		private void FixedUpdate()
		{
			if (enabled)
			{
				if (_firstLoop)
				{
					_firstLoop = false;
					if (_getFrameFunc != null)
					{
						_recorder = new Recorder(_recordProperties)
						{
							MaxInterval = _maxInterval
						};
					}
					else
					{
						_recorder = null;
					}
					_chrono.Start();
				}
				float time = _chrono.ElapsedTime;
				if (_recorder != null)
				{
					_recorder.AddFrame(_getFrameFunc(), time);
				}
				time += Time.fixedDeltaTime; // player need a small offset for full synchronization
				foreach (KeyValuePair<string, Player> kv in _players)
				{
					Player player = kv.Value;
					if (!player.IsOver(time))
					{
						player.UpdateTarget(time);
					}
					else
					{
						player.CallEndActionOneTime();
					}
				}
			}
		}

		public string GetGhostData()
		{
			if (_recorder != null)
			{
				return _recorder.Record.SerializeToString();
			}
			throw new System.Exception("There was no recorder. There is no data available.");
		}

		public Record GetGhost()
		{
			if (_recorder != null)
			{
				return _recorder.Record;
			}
			throw new System.Exception("There was no recorder. There is no data available.");
		}

		public override string ToString()
		{
			string s = "Synchronizer Info:" + "\n";
			s += "Player count:" + _players.Count + "\n";
			foreach (KeyValuePair<string, Player> kv in _players)
			{
				s += "[" + kv.Key + "] " + kv.Value.ToString() + "\n";
			}
			if (_recorder != null)
			{
				s += "Recorder:" + _recorder + "\n";
			}
			else
			{
				s += "Recorder: null\n";
			}
			s += "Time:" + _chrono;
			return s;
		}

		private static void Log(string text)
		{
			TSW.Log.Logger.Add("Synchro/ " + text);
		}
	}
}
