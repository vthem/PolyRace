using UnityEngine;

namespace TSW
{
	public class InputAxisTrigger
	{
		private bool _isDown = false;
		private readonly string _axisName;
		public enum Direction
		{
			Positive,
			Negave,
			Both
		}

		private readonly Direction _direction;

		public InputAxisTrigger(string axisName, Direction direction)
		{
			_axisName = axisName;
			_direction = direction;
		}

		public bool IsTrigger()
		{
			bool axisInUse = false;
			switch (_direction)
			{
				case Direction.Positive:
					axisInUse = Input.GetAxisRaw(_axisName) > 0f;
					break;
				case Direction.Negave:
					axisInUse = Input.GetAxisRaw(_axisName) < 0f;
					break;
				case Direction.Both:
					axisInUse = Input.GetAxisRaw(_axisName) != 0f;
					break;
			}
			if (axisInUse && !_isDown)
			{
				_isDown = true;
				return true;
			}
			if (!axisInUse)
			{
				_isDown = false;
			}
			return false;
		}
	}
}
