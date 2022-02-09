using TSW.Noise;
using TSW.Struct;

using UnityEngine;

// that's for exporting to CSV

// that's for Stopwatch

namespace LevelGen
{

	/// <summary>
	/// Contains the world data configuration and informate shared between the multiple components.
	/// </summary>
	public sealed class TerrainProfile : ScriptableObject
	{
		[SerializeField]
		private string _groundLayerName = "TerrainGround";
		public int GroundLayer => LayerMask.NameToLayer(_groundLayerName);

		[SerializeField]
		private string _heightLayerName = "TerrainHeight";
		public int HeightLayer => LayerMask.NameToLayer(_heightLayerName);

		#region Geometry
		[System.Serializable]
		private struct Geometry
		{
			public int _chunkSizeXZ;
			public int _chunkSizeY;
			public int _borderSize;
			public int _borderHeight;
			public int _blockSize;
			public int _tunnelHeight;
			public int _tunnelWidth;
			public int _numberOfGroundBlock;
			public Source _heightDataLayer;
			public int _colliderLayer;
			public AnimationCurve _weightModifier;
		}

		[SerializeField]
		private Geometry _geometry;
		private int _numberOfBlockXZ;
		private int _numberOfBlockY;

		/// <summary>
		/// Size of a block in world size
		/// </summary>
		/// <value>Size of a block in world size.</value>
		public int BlockSize => _geometry._blockSize;

		/// <summary>
		/// The size of the border in world unit
		/// </summary>
		/// <value>he size of the border (in world unit).</value>
		public int BorderSize => _geometry._borderSize;

		public int BorderHeight => _geometry._borderHeight;

		/// <summary>
		/// The number of block on XZ plane
		/// </summary>
		/// <value>The number of block on XZ plane.</value>
		public int NumberOfBlockXZ => _numberOfBlockXZ;
		public int ChunkSizeXZ => _geometry._chunkSizeXZ;

		/// <summary>
		/// The size of the chunk on Y axis
		/// </summary>
		/// <value>The size of the chunk on Y axis.</value>
		public int ChunkSizeY => _geometry._chunkSizeY;
		public int NumberOfGroundBlock => _geometry._numberOfGroundBlock;

		/// <summary>
		/// The height of the _tunnel in block
		/// </summary>
		public int TunnelHeight => _geometry._tunnelHeight;

		/// <summary>
		/// Gets the width of the tunnel in block
		/// </summary>
		public int TunnelWidth => _geometry._tunnelWidth;

		/// <summary>
		/// Number of block on Y axis
		/// </summary>
		/// <value>The number of block y.</value>
		public int NumberOfBlockY => _numberOfBlockY;

		/// <summary>
		/// How the output of the weight layer is modified
		/// </summary>
		public AnimationCurve WeightModifier => _geometry._weightModifier;

		/// <summary>
		/// The Noise used to build the hills of the terrain
		/// </summary>
		public Source HeightDataLayer => _geometry._heightDataLayer;


		/// <summary>
		/// Return the maximum height of the ground
		/// </summary>
		/// <returns>The ground max y.</returns>
		public float GetGroundMaxY()
		{
			return BlockSize / 2f;
		}
		#endregion

		#region Terrain Vertex Offset Modifier
		[SerializeField]
		private bool _enableVertexModifiers = true;
		public bool EnableVertexModifiers => _enableVertexModifiers;

		[SerializeField]
		private VertexModifier[] _vertexModifers;

		public Vector3 GetVertexOffset(Vector3 xyz)
		{
			Vector3 offset = Vector3.zero;
			foreach (VertexModifier modifier in _vertexModifers)
			{
				offset += modifier.GetVectorOffset(xyz, xyz.y / ChunkSizeY) * BlockSize;
			}
			return offset;
		}
		#endregion

		[SerializeField]
		private ColorProfile _colorProfile;
		public ColorProfile Colors => _colorProfile;


		[SerializeField]
		private Material _material;
		public Material Material => _material;


		public void Validate()
		{
			if (_material == null)
			{
				throw new System.Exception("Material is not set in " + name);
			}
			if (_geometry._heightDataLayer == null)
			{
				throw new System.Exception("HeightDataLayer is not set in " + name);
			}
			if (!_geometry._heightDataLayer.IsValid())
			{
				throw new System.Exception("HeightDataLayer is not valid in " + name);
			}
			if (_vertexModifers == null)
			{
				throw new System.Exception("VertexModifier is not set in " + name);
			}
			foreach (VertexModifier modifier in _vertexModifers)
			{
				modifier.Validate();
			}
		}

		/// <summary>
		/// Transform a world block position into a local block position
		/// </summary>
		/// <returns>The position.</returns>
		/// <param name="wPos">W position.</param>
		public Int2 LocalPosition(Int2 wPos)
		{
			int n = NumberOfBlockXZ;
			wPos.x = wPos.x - Mathf.FloorToInt(wPos.x / (float)n) * n;
			wPos.z = wPos.z - Mathf.FloorToInt(wPos.z / (float)n) * n;
			return wPos;
		}

		public Vector3 WorldPosition(Int2 wPos)
		{
			return new Vector3((wPos.x + .5f) * BlockSize, .5f * BlockSize, (wPos.z + .5f) * BlockSize);
		}

		public void Initialize(int seed)
		{
			HeightDataLayer.SetSeed(seed);
			foreach (VertexModifier modifier in _vertexModifers)
			{
				modifier.SetSeed(seed);
			}
			if (_geometry._chunkSizeY < _geometry._blockSize)
			{
				_numberOfBlockY = 1;
			}
			else
			{
				_numberOfBlockY = Mathf.RoundToInt(_geometry._chunkSizeY / (float)_geometry._blockSize);
			}
			_numberOfBlockXZ = ChunkSizeXZ / BlockSize;
			if (HeightLayer == -1)
			{
				throw new System.Exception("Please define a layer named " + _heightLayerName);
			}
			if (GroundLayer == -1)
			{
				throw new System.Exception("Please define a layer named TerrainGround " + _groundLayerName);
			}
		}

		public Vector3 BlockToWorld(Int2 block, bool offset = true)
		{
			Vector3 position = new Vector3((block.x + .5f) * BlockSize, 0f, (block.z + .5f) * BlockSize);
			if (offset && EnableVertexModifiers)
			{
				position = position + GetVertexOffset(position);
			}
			return position;
		}

		public bool GetGroundHeight(ref Vector3 position)
		{
			RaycastHit hit;
			position.y = ChunkSizeY;
			bool success = Physics.Raycast(position, -Vector3.up, out hit, ChunkSizeY * 2f, 1 << GroundLayer);
			if (success)
			{
				position = hit.point;
			}
			return success;
		}

		public bool GetTerrainHeight(ref Vector3 position)
		{
			RaycastHit hit;
			position.y = ChunkSizeY;
			int layerMask = 1 << GroundLayer | 1 << HeightLayer;
			bool success = Physics.Raycast(position, -Vector3.up, out hit, ChunkSizeY * 2f, layerMask);
			if (success)
			{
				position = hit.point;
			}
			return success;
		}

		public bool GetGroundInfo(ref Vector3 position, out Vector3 normal)
		{
			RaycastHit hit;
			position.y = ChunkSizeY;
			bool success = Physics.Raycast(position, -Vector3.up, out hit, ChunkSizeY, 1 << GroundLayer);
			if (success)
			{
				position = hit.point;
				normal = hit.normal;
			}
			normal = Vector3.up;
			return success;
		}

		public void SetSeed(int seed)
		{

		}

		public override string ToString()
		{
			string s = "";
			s += "WorldDataContext:";
			s += "\n - ChunkSizeXZ: " + _geometry._chunkSizeXZ;
			s += "\n - ChunkSizeY: " + _geometry._chunkSizeY;
			s += "\n - BlockSize: " + _geometry._blockSize;
			s += "\n - BorderSize: " + _geometry._borderSize;
			s += "\n - HeightLayer: " + _geometry._heightDataLayer;
			return s;
		}
	}
}