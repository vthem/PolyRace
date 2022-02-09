using System.Collections.Generic;

using Event = TSW.Messaging.Event;

namespace TSW.FSM
{
	internal class Manager : TSW.Design.Singleton<Manager>
	{
		private readonly Dictionary<string, StateMachine> _stateMachines = new Dictionary<string, StateMachine>();
		private readonly Queue<Event> _eventQueue = new Queue<Event>();
		private Event _current;

		public StateMachine Create(State state, string name)
		{
			StateMachine sm = new StateMachine(state, name);
			_stateMachines[name] = sm;
			return sm;
		}

		public void Remove(string name)
		{
			_stateMachines.Remove(name);
		}

		public void PostEvent(Event evt)
		{
			if (_current == null)
			{
				UnityEngine.Debug.Log("FSM/ => Process: '" + evt.GetType().Name + "'");
				_current = evt;
				foreach (KeyValuePair<string, StateMachine> kv in _stateMachines)
				{
					kv.Value.Handle(evt);
				}
				_current = null;
				if (_eventQueue.Count > 0)
				{
					PostEvent(_eventQueue.Dequeue());
				}
			}
			else
			{
				UnityEngine.Debug.Log("FSM/ => Queue: '" + evt.GetType().Name + "'");
				_eventQueue.Enqueue(evt);
			}
		}
	}
}
