using UnityEngine;

namespace TSW.FSM
{
	public class MonoStateMachine : MonoBehaviour
	{
		private StateMachine _stateMachine;

		private void Update()
		{
			_stateMachine.Update();
		}

		private void FixedUpdate()
		{
			_stateMachine.FixedUpdate();
		}

		private void OnGUI()
		{
			_stateMachine.UIUpdate();
		}

		private void OnDestroy()
		{
			if (_stateMachine != null)
			{
				StateMachine.DestroyStateMachine(gameObject.name);
			}
		}

		public static MonoStateMachine Instantiate(State state, string name)
		{
			GameObject obj = new GameObject(name);
			MonoStateMachine monoSM = obj.AddComponent<MonoStateMachine>();
			monoSM._stateMachine = StateMachine.CreateStateMachine(state, name);
			return monoSM;
		}
	}
}
