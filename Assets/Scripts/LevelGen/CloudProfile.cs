using TSW.Struct;

using UnityEngine;

namespace LevelGen
{
	public class CloudProfile : ScriptableObject
	{
		public bool _enable;

		public PrefabGroup _prefabs;

		[Tooltip("The minimal height of the object in percentage of ChunkSizeY")]
		public float _minHeight = 1;

		[Tooltip("The minimal height of the object in percentage of ChunkSizeY")]
		public float _maxHeight = 1;

		public float _step;

		public float _minScale;

		public float _maxScale;

		public Vector3 _rotation;

		public float _randomRadius;
	}
}
