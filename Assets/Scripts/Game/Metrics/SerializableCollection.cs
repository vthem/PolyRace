using System.Collections.Generic;

namespace Game.Metrics
{
	[System.Serializable]
	internal class SerializableCollection
	{
		public List<SerializableMetric> _metrics;
		public int _updateCount;
	}
}