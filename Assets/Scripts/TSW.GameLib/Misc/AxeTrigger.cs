namespace TSW
{
	public class AxeTrigger
	{
		private float _lastValue = 0f;
		private readonly float _threshold = 0f;

		public bool PassPositive { get; private set; }
		public bool PassNegative { get; private set; }

		public AxeTrigger(float threshold)
		{
			_threshold = threshold;
		}

		public void NewValue(float newValue)
		{
			PassPositive = PassNegative = false;
			if (newValue > _threshold && _lastValue <= _threshold)
			{
				PassPositive = true;
			}
			if (newValue < -_threshold && _lastValue >= -_threshold)
			{
				PassNegative = true;
			}
			_lastValue = newValue;
		}
	}
}
