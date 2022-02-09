using System;
using System.Collections.Generic;

using TSW.Messaging;

namespace TSW.FSM
{
	public class State
	{
		private readonly Dictionary<Type, Type> _stateTransitions = new Dictionary<Type, Type>();
		private StateMachine _stateMachine;
		public StateMachine StateMachine { get => _stateMachine; set => _stateMachine = value; }

		/// <summary>
		/// Adds the state transition. Replace the State if the transition
		/// already exist
		/// </summary>
		/// <typeparam name="T">Transition class</typeparam>
		/// <typeparam name="S">State class</typeparam>
		protected void AddStateTransition<T, S>()
		{
			if (_stateTransitions.ContainsKey(typeof(T)))
			{
				_stateTransitions[typeof(T)] = typeof(S);
			}
			else
			{
				_stateTransitions.Add(typeof(T), typeof(S));
			}
		}

		public virtual void OnUpdate()
		{
		}

		public virtual void OnFixedUpdate()
		{
		}

		public virtual void OnUIUpdate()
		{
		}

		public virtual void OnEnter(Event evt)
		{
		}

		public virtual void OnExit(Event evt)
		{
		}

		public Type GetStateByTransition<T>()
		{
			Type type = null;
			_stateTransitions.TryGetValue(typeof(T), out type);
			return type;
		}

		public State GetStateByTransition(Event evt)
		{
			Type type = null;
			_stateTransitions.TryGetValue(evt.GetType(), out type);
			if (type == null)
			{
				return null;
			}
			return System.Activator.CreateInstance(type) as State;
		}
	}

	public sealed class FinalState : State
	{
	}
}