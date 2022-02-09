using System;
using System.Collections.Generic;

namespace Game.Ghost
{
	[System.Serializable]
	public class SerializableRecord
	{
		public List<byte[]> _frames;
		public byte[] _properties;
		public string _framePropertyTypeName;
		public int _endRaceType;
	}

	public class Record
	{
		private readonly List<Frame> _frames;
		public Frame this[int i] => _frames[i];
		public int Count => _frames.Count;

		private readonly RecordProperties _properties;
		public RecordProperties Properties => _properties;
		public EndRaceType EndRaceType { get; set; }

		public Record(RecordProperties properties)
		{
			_properties = properties;
			_frames = new List<Frame>();
		}

		private Record(SerializableRecord sRecord)
		{
			_properties = TSW.ObjectSerializer.Deserialize<RecordProperties>(sRecord._properties);
			_frames = new List<Frame>();
			for (int i = 0; i < sRecord._frames.Count; ++i)
			{
				Frame frame = new Frame(Type.GetType(sRecord._framePropertyTypeName));
				frame.Deserialize(sRecord._frames[i]);
				_frames.Add(frame);
			}
			EndRaceType = (EndRaceType)sRecord._endRaceType;
		}

		public void AddFrame(Frame frame)
		{
			_frames.Add(frame);
		}

		public void PrepareForReplay()
		{
			for (int i = 1; i < _frames.Count; ++i)
			{
				_frames[i].PreviousFrame(_frames[i - 1]);
				TSW.Log.Logger.Add("PrepareForReplay: " + _frames[i]);
			}
		}

		public string SerializeToString()
		{
			return TSW.ObjectSerializer.SerializeLZFBase64(Serialize());
		}

		public byte[] SerializeToRaw()
		{
			return TSW.ObjectSerializer.Serialize(Serialize());
		}

		private SerializableRecord Serialize()
		{
			SerializableRecord sRecord = new SerializableRecord();
			if (_frames.Count == 0)
			{
				throw new System.Exception("There is no frame to record");
			}
			sRecord._frames = new List<byte[]>();
			sRecord._properties = TSW.ObjectSerializer.Serialize(_properties);
			sRecord._framePropertyTypeName = _frames[0].Properties.GetType().FullName;
			sRecord._endRaceType = (int)EndRaceType;
			for (int i = 0; i < _frames.Count; ++i)
			{
				sRecord._frames.Add(_frames[i].Serialize());
			}
			return sRecord;
		}

		public static Record Deserialize(string data)
		{
			SerializableRecord sRecord = TSW.ObjectSerializer.DeserializeLZFBase64<SerializableRecord>(data);
			Record record = new Record(sRecord);
			return record;
		}

		public static Record Deserialize(byte[] data)
		{
			SerializableRecord sRecord = TSW.ObjectSerializer.Deserialize<SerializableRecord>(data);
			Record record = new Record(sRecord);
			return record;
		}

		public override string ToString()
		{
			return "[Record frame number:" + Count + " last timestamp:" + _frames[_frames.Count - 1].Timestamp + " properties:" + _properties.ToString() + "]";
		}
	}
}
