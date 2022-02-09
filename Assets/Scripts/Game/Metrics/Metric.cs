namespace Game.Metrics
{
	[System.Serializable]
	internal struct SerializableMetric
	{
		public int _value;
		public int _bestValue;
		public int _diffValue;
		public MetricType _id;
	}

	public class Metric
	{
		private int _value = 0;
		public int Value => _value;

		private int _bestValue = 0;
		public int BestValue => _bestValue;

		private int _diffValue = 0;
		public int DiffValue => _diffValue;

		private readonly MetricType _id;
		public MetricType Id => _id;

		public enum ResultType
		{
			Improve,
			Worse,
			Equal
		}

		public bool Improved => _resultType == ResultType.Improve;

		private ResultType _resultType = ResultType.Equal;
		public ResultType Result { get => _resultType; set => _resultType = value; }

		public enum CompareType
		{
			LowerBetter,
			HigherBetter
		}

		private readonly CompareType _type;
		private readonly IProgress _progress;
		public IProgress Progress => _progress;

		private bool _firstUpdate = true;

		public Metric(MetricType id, CompareType type, IProgress progress)
		{
			_id = id;
			_type = type;
			switch (_type)
			{
				case CompareType.LowerBetter:
					_bestValue = int.MaxValue;
					break;
				case CompareType.HigherBetter:
					_bestValue = int.MinValue;
					break;
			}
			_progress = progress;
		}

		public void SetData(int value, int bestValue, int diffValue)
		{
			_value = value;
			_bestValue = bestValue;
			_diffValue = diffValue;
		}

		public void Update(int newValue)
		{
			_value = newValue;
			if (_firstUpdate)
			{
				_firstUpdate = false;
				_diffValue = 0;
			}
			else
			{
				_diffValue = _value - _bestValue;
			}
			switch (_type)
			{
				case CompareType.LowerBetter:
					if (_value < _bestValue)
					{
						_bestValue = _value;
						_resultType = ResultType.Improve;
					}
					else if (_value > _bestValue)
					{
						_resultType = ResultType.Worse;
					}
					else
					{
						_resultType = ResultType.Equal;
					}
					break;
				case CompareType.HigherBetter:
					if (_value > _bestValue)
					{
						_bestValue = _value;
						_resultType = ResultType.Improve;
					}
					else if (_value < _bestValue)
					{
						_resultType = ResultType.Worse;
					}
					else
					{
						_resultType = ResultType.Equal;
					}
					break;
			}
		}

		public float GetCompletionRatio(Metric challenge)
		{
			return _progress.ComputeProgress(challenge.BestValue, BestValue);
		}

		public string ValueToString(IMetricFormatter formatter)
		{
			return formatter.FormatValue(Value);
		}

		public string BestValueToString(IMetricFormatter formatter)
		{
			return formatter.FormatBestValue(BestValue);
		}

		public string DiffValueToString(IMetricFormatter formatter)
		{
			return formatter.FormatDiffValue(DiffValue, _type);
		}

		public override string ToString()
		{
			return string.Format("[Metric: Value={0}, BestValue={1}, DiffValue={2}, Id={3}, Improved={4}]", Value, BestValue, DiffValue, Id, Improved);
		}
	}
}
