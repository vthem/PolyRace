using System.Collections.Generic;

using UnityEngine;

using BLogger = TSW.Log.Logger;

namespace TSW.Messaging
{
	public class ScanForHandler : MonoBehaviour
	{
		[SerializeField]
		private bool _recursive = false;
		private readonly List<Dispatcher.EventHandler> _handlers = new List<Dispatcher.EventHandler>();

		private void Awake()
		{
			AutoRegister(transform);
		}

		private void AutoRegister(Transform target, int depth = 0)
		{
			Component[] components = target.GetComponents<Component>();
			foreach (Component component in components)
			{
				List<Dispatcher.EventHandler> handlers = Dispatcher.AddHandler(component);
				foreach (Dispatcher.EventHandler handler in handlers)
				{
					_handlers.Add(handler);
				}
			}
			if (_recursive && target.childCount > 0)
			{
				depth++;
				foreach (Transform child in target)
				{
					AutoRegister(child, depth);
				}
			}
		}

		private void OnDestroy()
		{
			foreach (Dispatcher.EventHandler handler in _handlers)
			{
				Dispatcher.Instance.RemoveHandler(handler);
			}
		}

		private static void Log(string text)
		{
			BLogger.Add("EventQueue/ " + text);
		}
	}
}
