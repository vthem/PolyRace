using System.Collections;

using UnityEngine;

namespace Game
{
	public class StartLight : TSW.Design.USingleton<StartLight>
	{
		public Animator _animator00;
		public Animator _animator01;
		public Animator _animator10;
		public Animator _animator11;
		public Animator _animator20;
		public Animator _animator21;
		public float _waitTime = 1f;

		public delegate void CountDownDelegate();

		private CountDownDelegate _endDelegate;
		private CountDownDelegate _interDelegate;

		public void SetGreen()
		{
			SetState(2, 1);
		}

		public void SetRed()
		{
			SetState(1, 1);
		}

		public void SetOff()
		{
			SetState(0, 1);
		}

		public void Run()
		{
			StartCoroutine(CountDownRoutine());
		}

		public void StartCountDown(CountDownDelegate interDelegate, CountDownDelegate endDelegate)
		{
			_endDelegate = endDelegate;
			_interDelegate = interDelegate;
			StartCoroutine(CountDownRoutine());
		}

		private IEnumerator CountDownRoutine()
		{
			yield return new WaitForSeconds(_waitTime);

			SetState(1, 0);
			SetState(1, 1);
			SetState(1, 2);
			if (_interDelegate != null)
			{
				_interDelegate();
			}
			yield return new WaitForSeconds(_waitTime);

			SetState(0, 2);
			if (_interDelegate != null)
			{
				_interDelegate();
			}
			yield return new WaitForSeconds(_waitTime);

			SetState(0, 1);
			if (_interDelegate != null)
			{
				_interDelegate();
			}
			yield return new WaitForSeconds(_waitTime);

			SetState(2, 0);
			SetState(2, 1);
			SetState(2, 2);

			if (_endDelegate != null)
			{
				_endDelegate();
			}

			yield return new WaitForSeconds(_waitTime * 3f);
			SetState(0, 0);
			SetState(0, 1);
			SetState(0, 2);
		}

		private void SetState(int state, int index)
		{
			if (index == 0)
			{
				_animator00.SetInteger("State", state);
				_animator01.SetInteger("State", state);
			}
			else if (index == 1)
			{
				_animator10.SetInteger("State", state);
				_animator11.SetInteger("State", state);
			}
			else if (index == 2)
			{
				_animator20.SetInteger("State", state);
				_animator21.SetInteger("State", state);
			}
		}
	}
}
