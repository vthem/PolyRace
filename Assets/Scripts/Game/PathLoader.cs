using System.Collections.Generic;

using LevelGen;

using UnityEngine;

namespace Game
{
	public class PathLoader : MonoBehaviour
	{
		public List<Transform> Path { get; private set; }

		public static PathLoader LoadLevel(Level level, string name)
		{
			GameObject obj = new GameObject("PathLoader_" + name);
			PathLoader pathLoader = obj.AddComponent<PathLoader>();
			pathLoader.Path = new List<Transform>();
			pathLoader._level = level;
			pathLoader.InvokeRepeating("UpdatePath", 0f, 1f);
			if (pathLoader.LoadNextIndex() == 0)
			{
				throw new System.Exception("Could not load path");
			}
			return pathLoader;
		}

		private int _lastChunkIndexLoaded = -1;
		private Level _level;

		private void UpdatePath()
		{
			while (LoadNextIndex() > 0) { }
		}

		private int LoadNextIndex()
		{
			int nextIndex = _lastChunkIndexLoaded + 1;
			int count = 0;
			GameObject chunkObj = _level.GetChunkDynamicContainer(nextIndex);
			if (chunkObj != null)
			{
				Transform arrowTransformContainer = chunkObj.transform.Find("Arrow");
				if (arrowTransformContainer != null)
				{
					foreach (Transform child in arrowTransformContainer)
					{
						count++;
						Path.Add(child);
					}
					_lastChunkIndexLoaded = nextIndex;
					return count;
				}
			}
			return count;
		}

	}
}