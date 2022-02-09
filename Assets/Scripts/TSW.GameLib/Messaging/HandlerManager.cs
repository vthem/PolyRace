using System.Collections.Generic;

using UnityEngine;

using BLogger = TSW.Log.Logger;

namespace TSW.Messaging
{
	public class HandlerManager : MonoBehaviour
	{
		[SerializeField]
		private Transform[] _autoRegister;
		private readonly List<Dispatcher.EventHandler> _handlers = new List<Dispatcher.EventHandler>();

		private void Start()
		{
			foreach (Transform child in _autoRegister)
			{
				AutoRegister(child);
			}
		}

		private void AutoRegister(Transform target, int depth = 0)
		{
			//			string tab = "";
			//			for (int i = 0; i < depth; ++i) {
			//				tab += ".";
			//			}
			//			Log("AutoRegister Test Transform:" + tab + target.name);
			Component[] components = target.GetComponents<Component>();
			foreach (Component component in components)
			{
				List<Dispatcher.EventHandler> handlers = Dispatcher.AddHandler(component);
				foreach (Dispatcher.EventHandler handler in handlers)
				{
					_handlers.Add(handler);
				}
			}
			if (target.childCount > 0)
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
