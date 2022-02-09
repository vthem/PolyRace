using UnityEngine;

namespace LevelGen
{
	public class MountainProfile : ScriptableObject
	{
		public bool _enable;
		public Mesh[] _meshes;
		public float _xScale;
		public float _zScale;
		public float yMinScale = 0f;
		public float yMaxScale = 0f;
		public Material _material;

	}
}
