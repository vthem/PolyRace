namespace Game.Ghost
{
	public class Recorder
	{
		private readonly Record _record;
		public Record Record => _record;

		private float _maxInterval = 0.1f;
		private bool _active = true;

		public float MaxInterval { get => _maxInterval; set => _maxInterval = value; }
		public bool Active { get => _active; set => _active = true; }

		public Recorder(RecordProperties recordProperties)
		{
			_record = new Record(recordProperties);
		}

		public void AddFrame(FrameProperties frameProperties, float relativeTime)
		{
			if (_active)
			{
				if (_record.Count == 0 || (_record.Count >= 1 && relativeTime >= _record[_record.Count - 1].Timestamp + _maxInterval))
				{
					Frame frame = new Frame(frameProperties, relativeTime);
					//					TSW.Log.Logger.Add("record frame:" + frame);
					_record.AddFrame(frame);
				}
			}
		}

		public void SetEndRaceType(EndRaceType endRaceType)
		{
			_record.EndRaceType = endRaceType;
		}

		public override string ToString()
		{
			return string.Format("[Recorder: Record={0}, MaxInterval={1}, Active={2}]", Record, MaxInterval, Active);
		}
	}
}
