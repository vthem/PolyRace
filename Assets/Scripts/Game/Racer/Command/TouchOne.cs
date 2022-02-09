using UnityEngine;

using UnityInput = UnityEngine.Input;

namespace Game.Racer.Command
{
	public class TouchOne : ICommand
	{
		private float _acceleration = 1.0f;
		private float _turn = 0.0f;

		public float Boost => 0f;
		public float Acceleration => _acceleration;

		public float Turn => _turn;

		public void Reset()
		{
			_turn = 0.0f;
			_acceleration = 1.0f;
		}

		public void Update()
		{
			foreach (Touch touch in UnityInput.touches)
			{
				switch (touch.phase)
				{
					case TouchPhase.Canceled:
					case TouchPhase.Ended:
						_turn = 0f;
						break;
					case TouchPhase.Began:
					case TouchPhase.Moved:
					case TouchPhase.Stationary:
						SetTurn(touch);
						break;
				}
			}
		}

		public void SetTurn(Touch touch)
		{
			if (touch.position.x < Screen.width / 2)
			{
				_turn = -1f;
			}
			else
			{
				_turn = 1f;
			}
		}
	}
}

