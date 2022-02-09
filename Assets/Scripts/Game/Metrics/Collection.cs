using System.Collections.Generic;

namespace Game.Metrics
{
	public class Collection
	{
		private readonly Dictionary<MetricType, Metric> _metrics = new Dictionary<MetricType, Metric>();
		public int _updateCount = 0;

		public int UpdateId => _updateCount;

		public void UpdateMetric(MetricType type, int value)
		{
			Metric metric;
			if (!_metrics.TryGetValue(type, out metric))
			{
				metric = Factory.Create(type);
				_metrics.Add(type, metric);
			}
			metric.Update(value);
		}

		public Metric GetMetric(MetricType id)
		{
			Metric metric;
			if (!_metrics.TryGetValue(id, out metric))
			{
				//                Debug.LogWarning("metric " + id + " not found in " + this.ToString());
			}
			return metric;
		}

		public void EndUpdate()
		{
			_updateCount++;
		}

		public int GetNumberOfImprove()
		{
			int count = 0;
			foreach (KeyValuePair<MetricType, Metric> keyValue in _metrics)
			{
				if (keyValue.Value.Improved)
				{
					count++;
				}
			}
			return count;
		}

		public override string ToString()
		{
			string s = string.Format("[MetricCollection: UpdateId={0}]", UpdateId);
			foreach (KeyValuePair<MetricType, Metric> keyValue in _metrics)
			{
				s += "\n" + keyValue.Value;
			}
			return s;
		}

		public byte[] SerializeToRaw()
		{
			SerializableCollection sc = new SerializableCollection
			{
				_metrics = new List<SerializableMetric>()
			};
			foreach (KeyValuePair<MetricType, Metric> keyValue in _metrics)
			{
				SerializableMetric sm = new SerializableMetric
				{
					_bestValue = keyValue.Value.BestValue,
					_diffValue = keyValue.Value.DiffValue,
					_value = keyValue.Value.Value,
					_id = keyValue.Value.Id
				};
				sc._metrics.Add(sm);
			}
			sc._updateCount = _updateCount;
			return TSW.ObjectSerializer.Serialize(sc);
		}

		public static Collection Deserialize(byte[] data)
		{
			Collection c = new Collection();
			SerializableCollection sc = TSW.ObjectSerializer.Deserialize<SerializableCollection>(data);
			c._updateCount = sc._updateCount;
			foreach (SerializableMetric sm in sc._metrics)
			{
				Metric metric = Factory.Create(sm._id);
				metric.SetData(sm._value, sm._bestValue, sm._diffValue);
				c._metrics.Add(sm._id, metric);
			}
			return c;
		}
	}
}