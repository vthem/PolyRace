using System.Collections.Generic;

using UnityEngine;

namespace TSW.Algorithm.MarchingCube
{
	public struct MeshInfo
	{
		public Vector3 offset;
		public Vector3[] cubeOffset;
		public float[] cube;
		public Color32 color;
		public int darkenHorizontal;
		public float scale;
		public float[] noises;
		public List<Vector3> outVertices;
		public List<int> outIndices;
		public List<Vector3> outNormals;
		public List<Color32> outColors;
	}

	public struct MeshInfoSimple
	{
		public float[] cube;
		public Vector3[] outVertices;
	}
}
