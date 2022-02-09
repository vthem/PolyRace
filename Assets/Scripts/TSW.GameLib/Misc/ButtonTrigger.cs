namespace TSW
{
	public class ButtonTrigger
	{
		private float _lastValue = 0f;
		private readonly float _threshold = 0f;

		public bool PassUp { get; private set; }
		public bool PassDown { get; private set; }

		public ButtonTrigger(float threshold)
		{
			_threshold = threshold;
		}

		public void NewValue(float newValue)
		{
			PassUp = PassDown = false;
			if (newValue > _threshold && _lastValue <= _threshold)
			{
				PassUp = true;
			}
			if (newValue < _threshold && _lastValue >= _threshold)
			{
				PassDown = true;
			}
			_lastValue = newValue;
		}
	}
}
