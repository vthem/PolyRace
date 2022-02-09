using UnityEngine;

using UnityInput = UnityEngine.Input;

namespace Game.Racer.Command
{
	public class TouchTwo : ICommand
	{
		private float _acceleration = 0.0f;
		private float _turn = 0.0f;
		private int _turnId = -1;

		public float Boost => 0f;
		public float Acceleration => _acceleration;

		public float Turn => _turn;

		public void Update()
		{
			_acceleration = _turn = 0.0f;
			foreach (Touch touch in UnityInput.touches)
			{
				switch (touch.phase)
				{
					case TouchPhase.Canceled:
					case TouchPhase.Ended:
						EndTouch(touch);
						break;
					case TouchPhase.Began:
					case TouchPhase.Moved:
					case TouchPhase.Stationary:
						UpdateTouch(touch);
						break;
				}
			}
		}

		private void UpdateTouch(Touch touch)
		{
			if (IsLeftScreen(touch))
			{
				if (_turnId == touch.fingerId)
				{
					_turn = (touch.position.x - (Screen.width / 4f)) / (Screen.width / 4f);
				}
				else if (_turnId == -1)
				{
					_turn = 0f;
					_turnId = touch.fingerId;
				}
			}
			else
			{
				_acceleration = 1f;
			}
		}

		private void EndTouch(Touch touch)
		{
			if (IsLeftScreen(touch))
			{
				if (_turnId == touch.fingerId)
				{
					_turn = 0f;
					_turnId = -1;
				}
			}
			else
			{
				_acceleration = 0f;
			}
		}

		private bool IsLeftScreen(Touch touch)
		{
			return touch.position.x < Screen.width / 2;
		}
	}
}

