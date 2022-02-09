using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;

using BLogger = TSW.Log.Logger;

namespace TSW.Messaging
{
	/// <summary>
	/// The dispatcher stores all the Listener objects. When an Event is pushed,
	/// the dispatcher look which object is listening to the event and it calls
	/// the method of listener corresponding to the event.
	/// </summary>
	public class Dispatcher : TSW.Design.Singleton<Dispatcher>
	{
		private readonly Dictionary<System.Type, List<EventHandler>> _handlerByEventType = new Dictionary<System.Type, List<EventHandler>>();

		public struct EventHandler
		{
			public MethodInfo Method { get; private set; }
			public object Target { get; private set; }
			public System.Type EventType { get; private set; }
			public EventHandler(MethodInfo method, System.Type eventType, object target) : this()
			{
				Method = method;
				EventType = eventType;
				Target = target;
			}

			public override bool Equals(object obj)
			{
				if (obj is EventHandler)
				{
					return ((EventHandler)obj).Method == Method;
				}
				return base.Equals(obj);
			}

			public override int GetHashCode()
			{
				return Method.GetHashCode();
			}

			public override string ToString()
			{
				return "[EventHandler: " + EventType.Name + " => " + Target.GetType().Name + "." + Method.Name + "]";
			}
		}

		/// <summary>
		/// Push the specified event.
		/// </summary>
		/// <param name="evt">Evt.</param>
		public static void FireEvent(Event evt)
		{
			Log("Fire event:" + evt.GetType().Name);
			Instance.CallEventMethod(evt);
		}

		public static List<EventHandler> GetHandlers(Component component)
		{
			List<EventHandler> handlers = new List<EventHandler>();
			MethodInfo[] methods = component.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod);
			foreach (MethodInfo method in methods)
			{
				foreach (object attr in method.GetCustomAttributes(true))
				{
					EventHandlerAttribute eventHandlerAttribute = attr as EventHandlerAttribute;
					if (eventHandlerAttribute != null)
					{
						foreach (Type eventType in eventHandlerAttribute._types)
						{
							try
							{
								handlers.Add(new EventHandler(method, eventType, component));
							}
							catch (System.Exception ex)
							{
								Debug.LogWarning("Could not use method " + method.Name + " on object " + component.name + " as EventHandler - " + ex.Message);
							}
						}
						break;
					}
				}
			}
			//			Log("found: " + handlers.Count + " on component:" + component.GetType().Name);
			return handlers;
		}

		private void AddHandler(EventHandler handler)
		{
			Log("Add: " + handler);
			if (!_handlerByEventType.ContainsKey(handler.EventType))
			{
				_handlerByEventType[handler.EventType] = new List<EventHandler>();
			}
			_handlerByEventType[handler.EventType].Add(handler);
		}

		public static void AddHandler<T>(Action<T> method) where T : Event
		{
			Instance.AddHandler(new EventHandler(method.Method, typeof(T), method.Target));
		}

		public static void AddHandler(System.Type eventType, Action<Event> method)
		{
			Instance.AddHandler(new EventHandler(method.Method, eventType, method.Target));
		}

		public static List<EventHandler> AddHandler(Component component)
		{
			List<EventHandler> handlers = GetHandlers(component);
			foreach (EventHandler handler in handlers)
			{
				Instance.AddHandler(handler);
			}
			return handlers;
		}

		public static void RemoveHandler<EventType>(Action<Event> action)
		{
			Instance.RemoveHandler(new EventHandler(action.Method, typeof(EventType), action.Target));
		}

		public static void RemoveHandler(Component component)
		{
			List<EventHandler> handlers = GetHandlers(component);
			foreach (EventHandler handler in handlers)
			{
				Instance.RemoveHandler(handler);
			}
		}

		public void RemoveHandler(EventHandler handler)
		{
			if (!_handlerByEventType.ContainsKey(handler.EventType))
			{
				Log("Remove not found: " + handler);
				return;
			}
			Log("Remove: " + handler);
			_handlerByEventType[handler.EventType].Remove(handler);
		}

		private void CallEventMethod(Event evt)
		{
			if (_handlerByEventType.ContainsKey(evt.GetType()))
			{
				List<EventHandler> handlers = _handlerByEventType[evt.GetType()];
				foreach (EventHandler handler in handlers)
				{
					Log("Exec: " + handler);
					handler.Method.Invoke(handler.Target, new Event[] { evt });
				}
			}
			else
			{
				Log("No handler found for event:" + evt.GetType().Name);
			}
		}

		public override string ToString()
		{
			string s = string.Empty;
			foreach (KeyValuePair<System.Type, List<EventHandler>> dictData in _handlerByEventType)
			{
				s += "\nEvent:" + dictData.Key.Name + ":";
				foreach (EventHandler handler in dictData.Value)
				{
					s += "\n\t" + handler;
				}
			}
			return string.Format("[Dispatcher]" + s);
		}

		private static void Log(string text)
		{
			BLogger.Add("EventQueue/ " + text);
		}
	}
}

