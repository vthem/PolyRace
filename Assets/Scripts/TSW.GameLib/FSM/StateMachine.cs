namespace TSW.FSM
{
	using Messaging;

	public sealed class StartEvent : Event { }

	public class StateMachine
	{
		private State _current;
		private readonly State _start;
		private readonly string _name;

		public static StateMachine CreateStateMachine(State state, string name)
		{
			return Manager.Instance.Create(state, name);
		}

		public static void PostEvent(Event evt)
		{
			Manager.Instance.PostEvent(evt);
		}

		public static void DestroyStateMachine(string name)
		{
			Manager.Instance.Remove(name);
		}

		public StateMachine(State state, string name)
		{
			_current = state;
			_start = state;
			_name = name;
			_current.StateMachine = this;
			Log.Logger.Add("FSM/ " + _name + " => Starting by state: '" + _current.GetType().Name + "'");
		}

		public void Handle(Event evt)
		{
			if (null == _current)
			{
				Log.Logger.Add("FSM/ " + _name + " => No state to handle event: '" + evt.GetType().Name + "'");
				return;
			}
			if (evt is StartEvent && _start == _current)
			{
				Log.Logger.Add("FSM/ " + _name + " => '" + _current.GetType().Name + "'.OnEnter()");
				_current.OnEnter(evt);
				return;
			}
			State nextState = _current.GetStateByTransition(evt);
			if (null == nextState)
			{
				Log.Logger.Add("FSM/ " + _name + " => Discard event: '" + evt.GetType().Name + "'");
				return;
			}
			nextState.StateMachine = this;

			string exitName = _current.GetType().Name;
			_current.OnExit(evt);
			_current = nextState;
			string enterName = _current.GetType().Name;
			Log.Logger.Add("FSM/ " + _name + " " + evt.GetType().Name + " => Exit:" + exitName + " Enter:" + enterName);
			_current.OnEnter(evt);

		}

		public void Update()
		{
			if (_current != null)
			{
				_current.OnUpdate();
			}
		}

		public void FixedUpdate()
		{
			if (_current != null)
			{
				_current.OnFixedUpdate();
			}
		}

		public void UIUpdate()
		{
			if (_current != null)
			{
				_current.OnUIUpdate();
			}
		}
	}
}