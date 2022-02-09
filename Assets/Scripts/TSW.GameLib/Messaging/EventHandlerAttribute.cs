using System;
using System.Collections.Generic;

namespace TSW.Messaging
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class EventHandlerAttribute : System.Attribute
	{
		public List<Type> _types = new List<Type>();

		public EventHandlerAttribute(Type eventType)
		{
			_types.Add(eventType);
		}

		public EventHandlerAttribute(params Type[] eventTypes)
		{
			foreach (Type e in eventTypes)
			{
				_types.Add(e);
			}
		}
	}
}