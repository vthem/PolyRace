using System;

namespace Game.Ghost
{
	public class Player
	{
		/// <summary>
		/// The ghost record data
		/// </summary>
		private Record _record;

		/// <summary>
		/// The index of the current Frame in the record
		/// </summary>
		private int _readIndex = -1;

		public Action<FrameProperties> ApplyFrameFunc { get; set; }

		public void Rewind()
		{
			_readIndex = -1;
		}

		private Action _endAction;
		public Action EndAction { set => _endAction = value; }

		public void CallEndActionOneTime()
		{
			if (_endAction != null)
			{
				_endAction();
				_endAction = null;
			}
		}

		public bool SetData(string data)
		{
			_record = Record.Deserialize(data);
			//			_record.PrepareForReplay();
			//			TSW.Log.Logger.Add("Set data in player (serialized)" + _record.ToString());
			if (_record.Count <= 2)
			{
				return false;
			}
			return true;
		}

		public void SetData(Record record)
		{
			//			TSW.Log.Logger.Add("Set data in player (not serialized)" + record.ToString());
			_record = record;
			//			_record.PrepareForReplay();
		}

		public void UpdateTarget(float time)
		{
			ApplyFrameFunc(GetFrame(time));
		}

		public bool IsOver(float time)
		{
			//			TSW.Log.Logger.Add("time:" + time + " last:" + _record[_record.Count-1].Timestamp);
			return time >= _record[_record.Count - 1].Timestamp;
		}

		public FrameProperties GetFrame(float time)
		{
			if (_readIndex == -1)
			{
				_readIndex = 0;
			}
			if (time < _record[0].Timestamp)
			{
				//				TSW.Log.Logger.Add("relative time before first frame rt:" + time + " frame:" + _record[0].Timestamp);
				return _record[0].Properties;
			}
			if (time >= _record[_record.Count - 1].Timestamp)
			{
				//				TSW.Log.Logger.Add("relative time after last frame rt:" + time + " frame:" + _record[_record.Count-1].Timestamp);
				return _record[_record.Count - 1].Properties;
			}

			if (time < _record[_readIndex].Timestamp)
			{
				_readIndex = 0;
			}

			for (int i = _readIndex; i < _record.Count - 1; ++i)
			{
				if (time >= _record[i].Timestamp && time < _record[i + 1].Timestamp)
				{
					_readIndex = i;
					break;
				}
			}

			float frameDuration = _record[_readIndex + 1].Timestamp - _record[_readIndex].Timestamp;
			float frameOffset = time - _record[_readIndex].Timestamp;
			//TSW.Log.Logger.Add("Lerp index:" + _readIndex + " rt:" + time + " duration:" + frameDuration + " offset:" + frameOffset / frameDuration);
			//TSW.Log.Logger.Add("Lerp from:" + _record[_readIndex]);
			//TSW.Log.Logger.Add("Lerp to:" + _record[_readIndex + 1]);
			//TSW.Log.Logger.Add("Lerp =:" + _record[_readIndex].Properties.Lerp(_record[_readIndex + 1].Properties, frameOffset / frameDuration));
			return _record[_readIndex].Properties.Lerp(_record[_readIndex + 1].Properties, frameOffset / frameDuration);
		}

		public void DebugFrames(float delta)
		{
			for (float t = 0f; !IsOver(t); t += delta)
			{
				GetFrame(t);
			}
		}


		public EndRaceType GetEndRaceType()
		{
			return _record.EndRaceType;
		}

		public RecordProperties GetRecordProperties()
		{
			return _record.Properties;
		}

		public override string ToString()
		{
			return string.Format("[Player record:" + _record.ToString() + "]");
		}
	}
}
