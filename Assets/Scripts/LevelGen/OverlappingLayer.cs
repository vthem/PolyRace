using System.Collections.Generic;

using TSW.Struct;

namespace LevelGen
{
	public class OverlappingLayer
	{
		private readonly Dictionary<string, HashSet<Int2>> _layers = new Dictionary<string, HashSet<Int2>>();

		public int Count => _layers.Count;

		public bool Check(string layerName, Int2 gridPosition)
		{
			HashSet<Int2> layer = GetLayer(layerName);
			return layer.Contains(gridPosition);
		}

		public void Set(string layerName, Int2 gridPosition, int length)
		{
			HashSet<Int2> layer = GetLayer(layerName);
			Int2 cur;
			for (int x = gridPosition.x - length; x <= gridPosition.x + length; ++x)
			{
				for (int z = gridPosition.z - length; z <= gridPosition.z + length; ++z)
				{
					cur.x = x;
					cur.z = z;
					layer.Add(cur);
				}
			}
		}

		public void Clear()
		{
			_layers.Clear();
		}

		private HashSet<Int2> GetLayer(string layerName)
		{
			HashSet<Int2> layer = null;
			if (!_layers.TryGetValue(layerName, out layer))
			{
				layer = new HashSet<Int2>();
				_layers.Add(layerName, layer);
			}
			return layer;
		}

	}
}
