using TSW.Struct;

using UnityEngine;

namespace LevelGen
{
	[System.Serializable]
	public struct SceneryPrefabGroup
	{
		public PrefabGroup _prefabGroup;
		public AnimationCurve _probability;
		public float _minScale;
		public float _maxScale;
		public Vector3 _rotation;
	}

	public sealed class Scenery : ScriptableObject
	{
		public bool _enable;

		public SceneryPrefabGroup[] _prefabGroups;

		public float _randomRadius = 5f;

		public float _step = 20f;

		public float _probability = .3f;

		public GameObject _sideArrowPrefab;
	}
}
