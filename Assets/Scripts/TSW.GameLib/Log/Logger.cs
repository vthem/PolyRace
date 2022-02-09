using System;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;

namespace TSW.Log
{
	public class Logger : TSW.Design.Singleton<Logger>
	{
		private IWriter _writer = null;
		private readonly Queue<string> _logQueue = new Queue<string>();
		private readonly AutoResetEvent _logQueueEvent = new AutoResetEvent(false);
		private readonly object _writerLocker = new object();
		private HashSet<string> _filters = new HashSet<string>();
		private bool _ready = false;

		public const string EnableOptionKey = "LOG_ENABLED";

		public static void Add(string text)
		{
			if (!Instance._ready)
			{
				return;
			}
			if (Instance.IsFiltered(text))
			{
				return;
			}
			text = DateTime.Now.ToString("yyyy:MM:dd H:mm:ss.fff ") + " " + text;
			Instance.Enqueue(text);
		}

		public static void Disable()
		{
			if (!Instance._ready)
			{
				return;
			}
			Instance.InnerDisable();
		}

		public static void Enable()
		{
			Instance.InnerEnable();
		}

		public static void ClearFilter()
		{
			Instance._filters = new HashSet<string>();
		}

		public static void AddFilter(string filter)
		{
			Instance._filters.Add(filter);
		}

		public static void HandleLog(string logString, string stackTrace, LogType logType)
		{
			if (!Instance._ready)
			{
				return;
			}
			Add(logString);
			Add(stackTrace);
		}

		protected override void SingletonCreate()
		{
			UnityEngine.Debug.Log("Create Logger");
		}

		private void Consumer()
		{
			while (true)
			{
				try
				{
					string log = Dequeue();
					if (log != null)
					{
						UnityEngine.Debug.Log("log::" + log);
						Write(log);
					}
					else
					{
						UnityEngine.Debug.Log("-- end log file --");
						return;
					}

				}
				catch (Exception)
				{
				}
			}
		}

		private void Enqueue(string log)
		{
			Write(log);
		}

		private bool IsFiltered(string log)
		{
			int idx = log.IndexOf('/');
			if (idx > 0)
			{
				string s = log.Substring(0, idx);
				return _filters.Contains(s);
			}
			return false;
		}

		private string Dequeue()
		{
			_logQueueEvent.WaitOne();
			lock (_logQueue)
			{
				return _logQueue.Dequeue();
			}
		}

		private void Write(string log)
		{
			lock (_writerLocker)
			{
				if (Instance._writer != null)
				{
					Instance._writer.Log(log);
				}
			}
		}

		private void InnerDisable()
		{
			if (!_ready)
			{
				return;
			}
			UnityEngine.Debug.Log("disabling logs");

			OnQuitNotifier.Instance.OnQuit -= InnerDisable;

			UnityEngine.Debug.Log("disabled logs");
		}

		private void InnerEnable()
		{
			if (_ready)
			{
				return;
			}
			_writer = new DefaultWriter();

			OnQuitNotifier.Instance.OnQuit += InnerDisable;

			_ready = true;
		}
	}
}