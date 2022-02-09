using System;
using System.IO;

namespace Game.Ghost
{

	public class Frame
	{
		private float _timestamp;
		public float Timestamp => _timestamp;

		private readonly FrameProperties _properties;
		public FrameProperties Properties => _properties;

		public Frame(FrameProperties properties, float timestamp)
		{
			_properties = properties;
			_timestamp = timestamp;
		}

		public Frame(Type framePropertiesType)
		{
			_properties = System.Activator.CreateInstance(framePropertiesType) as FrameProperties;
		}

		public void PreviousFrame(Frame previous)
		{
			if (_properties != null)
			{
				_properties.PreviousFrameProperties(previous.Properties, _timestamp - previous.Timestamp);
			}
		}

		public override string ToString()
		{
			return string.Format("[Frame: Timestamp={0}, Properties={1}]", Timestamp, Properties);
		}

		public byte[] Serialize()
		{
			MemoryStream mem = new MemoryStream();
			byte[] data = BitConverter.GetBytes(_timestamp);
			mem.Write(data, 0, data.Length);
			data = _properties.Serialize();
			mem.Write(data, 0, data.Length);
			return mem.ToArray();
		}

		public void Deserialize(byte[] data)
		{
			_timestamp = BitConverter.ToSingle(data, 0);
			_properties.Deserialize(data, sizeof(float));
		}
	}
}
