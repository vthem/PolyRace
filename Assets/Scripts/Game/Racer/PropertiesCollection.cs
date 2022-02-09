using System.Collections.Generic;

using UnityEngine;

namespace Game.Racer
{
	public class PropertiesCollection : ScriptableObject
	{
		[SerializeField]
		private List<Properties> _propertiesCollection;

		[SerializeField]
		private CommonProperties _commonProperties;

		public int Count => _propertiesCollection.Count;

		public CommonProperties Common => _commonProperties;

		public Properties GetProperties(int id)
		{
			if (id >= _propertiesCollection.Count)
			{
				Debug.LogError("Racer id:" + id + " does not exist");
				return null;
			}
			return _propertiesCollection[id];
		}

		public DynamicProperties GetDynamicProperties(int id)
		{
			if (id >= _propertiesCollection.Count)
			{
				Debug.LogError("Racer id:" + id + " does not exist");
				return null;
			}
			return new DynamicProperties(_propertiesCollection[id], new DynamicPropertyPoints());
		}

		public static PropertiesCollection GetAsset()
		{
			return Resources.Load("Prefabs/Racer/PropertiesCollection") as PropertiesCollection;
		}
	}
}
