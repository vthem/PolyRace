using System.Collections.Generic;

using TSW.Algorithm.MarchingCube;
using TSW.Noise;
using TSW.Struct;

using UnityEngine;

// that's for exporting to CSV


namespace LevelGen
{

	public sealed class Chunk
	{

		/// <summary>
		/// The position in world coordinate on plane x,z
		/// </summary>
		/// <value>The position.</value>
		public Vector3 Position { get; private set; }

		/// <summary>
		/// The coordinates of the chunk on plane x,z
		/// </summary>
		/// <value>The coordinates.</value>
		public Int2 ChunkPosition { get; private set; }

		/// <summary>
		/// The position in block (depends on block size) on plane x,z
		/// </summary>
		/// <value>The block position.</value>
		public Int2 BlockPosition => new Int2(Terrain.NumberOfBlockXZ * ChunkPosition.x, Terrain.NumberOfBlockXZ * ChunkPosition.z);

		/// <summary>
		/// Translate a local block position [0, nBlockXZ] in World block position
		/// </summary>
		/// <returns>The block position.</returns>
		/// <param name="local">Local.</param>
		public Int2 WorldBlockPosition(Int2 local)
		{
			return BlockPosition + local;
		}

		public Vector3 WorldPosition(Int2 worldBlock)
		{
			return new Vector3(worldBlock.x * Terrain.BlockSize, 0, worldBlock.z * Terrain.BlockSize);
		}

		/// <summary>
		/// Translate a world block position to local [0, nBlockXZ]
		/// </summary>
		/// <returns>The block position.</returns>
		/// <param name="world">World.</param>
		public Int2 LocalBlockPosition(Int2 world)
		{
			return Terrain.LocalPosition(world);
		}

		public const float WeightLimit = 10000000f;

		/// <summary>
		/// The _direction.
		///   0
		/// 3 x 1
		///   2
		/// 4 => not set
		/// </summary>
		private readonly int _inDirection = 4;
		public int InDirection => _inDirection;

		private readonly int _outDirection = 4;
		public int OutDirection => _outDirection;

		public static int NoDirection => 4;

		private readonly int _index;
		public int Index => _index;

		private Int2 _inPosition;
		public Int2 InPosition { get => _inPosition; set => _inPosition = value; }

		private Int2 _outPosition;
		public Int2 OutPosition { get => _outPosition; set => _outPosition = value; }

		private readonly int _nBorderBottom;
		private readonly int _nBorderTop;
		private readonly int _nScalarXZ;

		public enum Pattern
		{
			Default,
			Flat,
			Gradient,
			InverseGradient
		};

		private enum Type
		{
			Main,
			Side
		}

		private readonly Pattern _pattern;
		private readonly Type _type;

		public class PathData : System.IComparable<PathData>
		{
			public Vector3 _position;
			public int _index;

			public PathData(Vector3 position, int index)
			{
				_position = position;
				_index = index;
			}

			public int CompareTo(PathData other)
			{
				return _index.CompareTo(other._index);
			}
		}

		private readonly Dictionary<Int2, PathData> _pathDatas = new Dictionary<Int2, PathData>();

		/// <summary>
		/// Pointer to the neighboors of this chunks. Indexes:
		///  0
		/// 3 1
		///  2
		/// </summary>
		private readonly Chunk[] _neighboors = new Chunk[4];


		/// <summary>
		/// A list of side chunk
		/// </summary>
		private readonly List<Chunk> _sideChunks = new List<Chunk>();

		public TerrainProfile Terrain { get; private set; }
		public LevelProfile Level { get; private set; }

		/// <summary>
		/// Edges. If set, there is a border
		///  0
		/// 3 1
		///  2
		/// </summary>
		private readonly bool[] _edges = new bool[4];
		private float[] _heights;
		private float[] _weights;
		private bool[] _tunnelData;

		/// <summary>
		/// Contains all the data to create a mesh
		/// Used when building the mesh data
		/// </summary>
		private class ChunkMeshData
		{
			public int layer;
			public List<Vector3> vertices = new List<Vector3>();
			public List<int> triangles = new List<int>();
			public List<Vector3> normals = new List<Vector3>();
			public List<Color32> colors = new List<Color32>();

			public ChunkMeshData(int layer)
			{
				this.layer = layer;
			}
		}

		/// <summary>
		/// Contains all the final data to create the mesh
		/// </summary>
		private List<ChunkMeshData> _chunkMeshesData;

		private Chunk(Vector3 position, LevelProfile levelProfile, int index, bool[] edges, Pattern pattern) : this(position, levelProfile, 4, 4, index, pattern, Type.Side)
		{
			_edges = edges;
		}

		public Chunk(Vector3 position, LevelProfile levelProfile, int inDirection, int outDirection, int index, Pattern pattern) : this(position, levelProfile, inDirection, outDirection, index, pattern, Type.Main)
		{
		}

		private Chunk(Vector3 position, LevelProfile levelProfile, int inDirection, int outDirection, int index, Pattern pattern, Type type)
		{
			Terrain = levelProfile.Terrain;
			Level = levelProfile;
			Position = position;
			ChunkPosition = new Int2(Mathf.RoundToInt(position.x / Terrain.ChunkSizeXZ), Mathf.RoundToInt(position.z / Terrain.ChunkSizeXZ));
			_inDirection = inDirection;
			_outDirection = outDirection;
			_index = index;
			for (int i = 0; i < 4; ++i)
			{
				_edges[i] = true;
			}
			if (_inDirection != NoDirection)
			{
				_edges[_inDirection] = false;
			}
			if (_outDirection != NoDirection)
			{
				_edges[_outDirection] = false;
			}

			Int2[] offset = new Int2[] {
				new Int2(0, 1),
				new Int2(1, 0),
				new Int2(0, -1),
				new Int2(-1, 0)
			};

			_nScalarXZ = Terrain.NumberOfBlockXZ + 1;
			_nBorderBottom = Terrain.BorderSize / Terrain.BlockSize;
			_nBorderTop = _nScalarXZ - _nBorderBottom - 1;

			// Add side chunk according to edges
			_type = type;
			_pattern = pattern;
			if (_type == Type.Main)
			{
				for (int i = 0; i < 4; ++i)
				{
					if (_edges[i])
					{
						bool[] edges = new bool[4];
						for (int k = 0; k < edges.Length; ++k)
						{
							edges[k] = false;
						}
						edges[(i + 2) % 4] = true;
						Int2 chunkPosition = ChunkPosition + offset[i];
						AddSideChunk(new Chunk(new Vector3(chunkPosition.x * Terrain.ChunkSizeXZ, 0f, chunkPosition.z * Terrain.ChunkSizeXZ), levelProfile, _index * 1000 + i, edges, _pattern));
					}
				}
			}
		}

		public void CreateData()
		{
			TSW.Profiler.ProfilerEntry pe = TSW.Profiler.AddEntry("CreateData");
			// initialize output data of the chunk
			_heights = new float[_nScalarXZ * _nScalarXZ];
			if (_type == Type.Main)
			{
				_weights = new float[(_nScalarXZ - 1) * (_nScalarXZ - 1)];
			}

			// Create the scalar field used to create the mesh data
			Source heightNoise = Terrain.HeightDataLayer;
			int idx;
			int cx, cz;
			for (int z = 0; z < _nScalarXZ; ++z)
			{
				for (int x = 0; x < _nScalarXZ; ++x)
				{
					cx = (int)Position.x + x * Terrain.BlockSize;
					cz = (int)Position.z + z * Terrain.BlockSize;
					idx = z * _nScalarXZ + x;

					// height and color value
					ApplyPattern(x, z, () => heightNoise.GetFloat(cx, 0, cz), idx);
				}
			}

			TSW.Profiler.ProfilerEntry peWeightModifier = TSW.Profiler.AddEntry("WeightModifier");
			if (_type == Type.Main)
			{
				ChunkWeightModifier.ModifyWeightByEdgeDistance(_heights, _weights, this);
			}
			peWeightModifier.End();

			foreach (Chunk sideChunk in _sideChunks)
			{
				sideChunk.CreateData();
			}
			pe.End();
		}

		private void ApplyMainPattern(int x, int z, System.Func<float> perlin, int idx)
		{
			switch (_pattern)
			{
				case Pattern.Default:
					_heights[idx] = perlin();
					if (IsBorder(x, z) && _heights[idx] * Terrain.ChunkSizeY < Terrain.BorderHeight)
					{
						_heights[idx] = Terrain.BorderHeight / (float)Terrain.ChunkSizeY;
					}
					break;
				case Pattern.Flat:
					_heights[idx] = 0;
					if (IsBorder(x, z) /* && _heights[idx] * (float)Terrain.ChunkSizeY < Terrain.BorderHeight */)
					{
						_heights[idx] = Terrain.BorderHeight / (float)Terrain.ChunkSizeY;
					}
					break;
				case Pattern.Gradient:
					switch (_outDirection)
					{
						case 0:
							_heights[idx] = perlin() * z / Terrain.NumberOfBlockXZ; break;
						case 1:
							_heights[idx] = perlin() * x / Terrain.NumberOfBlockXZ; break;
						case 2:
							_heights[idx] = perlin() * (1 - (z / (float)(Terrain.NumberOfBlockXZ))); break;
						case 3:
							_heights[idx] = perlin() * (1 - (x / (float)(Terrain.NumberOfBlockXZ))); break;
					}
					if (IsBorder(x, z) && _heights[idx] * Terrain.ChunkSizeY < Terrain.BorderHeight)
					{
						_heights[idx] = Terrain.BorderHeight / (float)Terrain.ChunkSizeY;
					}
					break;
				case Pattern.InverseGradient:
					switch (_outDirection)
					{
						case 0:
							_heights[idx] = perlin() * (1 - (z / (float)(Terrain.NumberOfBlockXZ))); break;
						case 1:
							_heights[idx] = perlin() * (1 - (x / (float)(Terrain.NumberOfBlockXZ))); break;
						case 2:
							_heights[idx] = perlin() * z / Terrain.NumberOfBlockXZ; break;
						case 3:
							_heights[idx] = perlin() * x / Terrain.NumberOfBlockXZ; break;
					}
					if (IsBorder(x, z) && _heights[idx] * Terrain.ChunkSizeY < Terrain.BorderHeight)
					{
						_heights[idx] = Terrain.BorderHeight / (float)Terrain.ChunkSizeY;
					}
					break;
			}
		}

		private void ApplyPattern(int x, int z, System.Func<float> perlin, int idx)
		{
			if (Type.Main == _type)
			{
				ApplyMainPattern(x, z, perlin, idx);
			}
			else
			{
				if (IsBorder(x, z) && IsOnEdge(x, z))
				{
					ApplyMainPattern(x, z, perlin, idx);
				}
				else
				{
					_heights[idx] = -1f;
				}
			}
		}

		public void AddSideChunk(Chunk sideChunk)
		{
			_sideChunks.Add(sideChunk);
		}

		public void AddPath(Vector3 world, Int2 block, int index)
		{
			if (!_pathDatas.ContainsKey(block))
			{
				_pathDatas.Add(block, new PathData(world, index));
			}
		}

		public void AppendPathDatas(List<PathData> pathDatas)
		{
			pathDatas.AddRange(_pathDatas.Values);
		}

		public void CreateMeshData()
		{
			TSW.Profiler.ProfilerEntry pe = TSW.Profiler.AddEntry("CreateMeshData");

			// do the marching cube
			MeshCreator.SetTarget(.5f);
			MeshCreator.SetWindingOrder(0, 1, 2);

			int nBlockY = Terrain.NumberOfBlockY;
			int nBlockXZ = Terrain.NumberOfBlockXZ;
			float[] cube = new float[8];

			_chunkMeshesData = new List<ChunkMeshData>();
			ChunkMeshData[] chunkMeshDataLayer = new ChunkMeshData[2];
			chunkMeshDataLayer[0] = new ChunkMeshData(0);
			chunkMeshDataLayer[1] = new ChunkMeshData(1);
			const int maxVertices = 2500;
			Dictionary<Vector3, Vector3> vertexModifierCache = new Dictionary<Vector3, Vector3>();

			int idx = 0;
			int nCall = 0;
			int baseIdx;
			float value;
			int yStart;
			int yStop;
			Vector3 cubeLocalPosition;
			Vector3 cubeWorldPosition;
			Vector3 vertexPosition;
			Vector3[] outVertices = new Vector3[15];
			bool[] flatArray = new bool[5];
			for (int z = 0; z < nBlockXZ; ++z)
			{
				for (int x = 0; x < nBlockXZ; ++x)
				{
					idx = z * _nScalarXZ + x;

					// find the Y start (how high we start marching cube)
					yStart = 0;
					if (_tunnelData == null)
					{
						baseIdx = -1;
						value = 1f;

						idx = z * _nScalarXZ + x;
						if (_heights[idx] < value)
						{
							value = _heights[idx];
							baseIdx = idx;
						}
						idx = z * _nScalarXZ + x + 1;
						if (_heights[idx] < value)
						{
							value = _heights[idx];
							baseIdx = idx;
						}
						idx = (z + 1) * _nScalarXZ + x;
						if (_heights[idx] < value)
						{
							value = _heights[idx];
							baseIdx = idx;
						}
						idx = (z + 1) * _nScalarXZ + (x + 1);
						if (_heights[idx] < value)
						{
							value = _heights[idx];
							baseIdx = idx;
						}

						if (baseIdx != -1)
						{
							yStart = Mathf.FloorToInt(_heights[baseIdx] * nBlockY);
						}
					}

					// find the Y stop (how high we stop marching cube)
					baseIdx = -1;
					value = 0f;
					idx = z * _nScalarXZ + x;
					if (_heights[idx] > value)
					{
						value = _heights[idx];
						baseIdx = idx;
					}
					idx = z * _nScalarXZ + x + 1;
					if (_heights[idx] > value)
					{
						value = _heights[idx];
						baseIdx = idx;
					}
					idx = (z + 1) * _nScalarXZ + x;
					if (_heights[idx] > value)
					{
						value = _heights[idx];
						baseIdx = idx;
					}
					idx = (z + 1) * _nScalarXZ + (x + 1);
					if (_heights[idx] > value)
					{
						value = _heights[idx];
						baseIdx = idx;
					}
					yStop = 1;
					if (baseIdx != -1)
					{
						yStop = Mathf.Min(nBlockY, Mathf.FloorToInt(_heights[baseIdx] * nBlockY) + 1);
					}

					yStart = Mathf.Max(0, yStart); // when negative height
					for (int y = yStart; y < yStop; ++y)
					{

						// ground y
						if (y == 0)
						{
							// 0
							idx = z * _nScalarXZ + x;
							cube[0] = _heights[idx] * nBlockY >= y ? 1 : 0;

							// 1
							idx = z * _nScalarXZ + (x + 1);
							cube[1] = _heights[idx] * nBlockY >= y ? 1 : 0;

							// 2
							idx = z * _nScalarXZ + (x + 1);
							if (_tunnelData != null && _tunnelData[idx])
							{
								cube[2] = 0f;
							}
							else
							{
								cube[2] = _heights[idx] * nBlockY >= 1 ? 1 : 0;
							}

							// 3
							idx = z * _nScalarXZ + x;
							if (_tunnelData != null && _tunnelData[idx])
							{
								cube[3] = 0;
							}
							else
							{
								cube[3] = _heights[idx] * nBlockY >= 1 ? 1 : 0;
							}

							// 4
							idx = (z + 1) * _nScalarXZ + x;
							cube[4] = _heights[idx] * nBlockY >= y ? 1 : 0;

							// 5
							idx = (z + 1) * _nScalarXZ + (x + 1);
							cube[5] = _heights[idx] * nBlockY >= y ? 1 : 0;

							// 6
							idx = (z + 1) * _nScalarXZ + (x + 1);
							if (_tunnelData != null && _tunnelData[idx])
							{
								cube[6] = 0;
							}
							else
							{
								cube[6] = _heights[idx] * nBlockY >= 1 ? 1 : 0;
							}

							// 7
							idx = (z + 1) * _nScalarXZ + x;
							if (_tunnelData != null && _tunnelData[idx])
							{
								cube[7] = 0;
							}
							else
							{
								cube[7] = _heights[idx] * nBlockY >= 1 ? 1 : 0;
							}
						}
						else if (y == nBlockY - 1)
						{
							// 0
							idx = z * _nScalarXZ + x;
							cube[0] = _heights[idx] * nBlockY >= y ? 1 : 0;

							// 1
							idx = z * _nScalarXZ + (x + 1);
							cube[1] = _heights[idx] * nBlockY >= y ? 1 : 0;

							// 4
							idx = (z + 1) * _nScalarXZ + x;
							cube[4] = _heights[idx] * nBlockY >= y ? 1 : 0;

							// 5
							idx = (z + 1) * _nScalarXZ + (x + 1);
							cube[5] = _heights[idx] * nBlockY >= y ? 1 : 0;

							// 2, 3, 6, 7
							cube[2] = cube[3] = cube[6] = cube[7] = 0;
						}
						else if (_tunnelData != null)
						{
							for (int i = 0; i < 8; ++i)
							{
								idx = (z + _cubeOffset[i * 3]) * _nScalarXZ + x + _cubeOffset[i * 3 + 1];
								if (_tunnelData[idx] && y + _cubeOffset[i * 3 + 2] < Terrain.TunnelHeight)
								{
									cube[i] = 0;
								}
								else
								{
									cube[i] = _heights[idx] * nBlockY >= y + _cubeOffset[i * 3 + 2] ? 1 : 0;
								}
							}
						}
						else
						{
							for (int i = 0; i < 8; ++i)
							{
								idx = (z + _cubeOffset[i * 3]) * _nScalarXZ + x + _cubeOffset[i * 3 + 1];
								cube[i] = _heights[idx] * nBlockY >= y + _cubeOffset[i * 3 + 2] ? 1 : 0;
							}

						}

						nCall++;

						int verticeCount = 0;
						MeshCreator.MeshAppendSimple(cube, ref outVertices, ref verticeCount);

						// position of the cube in this chunk
						cubeLocalPosition = new Vector3(x, y, z) * Terrain.BlockSize;

						// position of the cube in the world
						cubeWorldPosition = cubeLocalPosition;
						cubeWorldPosition.x += Position.x;
						cubeWorldPosition.z += Position.z;

						for (int t = 0; t < verticeCount; t += 3)
						{
							Vector3 normal = Vector3.Cross(outVertices[t + 1] - outVertices[t], outVertices[t + 2] - outVertices[t]).normalized;
							// slope --> 1 : flat, slope --> 0 : vertical
							float slope = Mathf.Abs(Vector3.Dot(Vector3.up, normal));
							flatArray[t / 3] = slope > 0.8f;
						}

						// transform the vertices => scale, offset
						for (int k = 0; k < verticeCount; ++k)
						{
							// position of the vertex in the chunk
							vertexPosition = outVertices[k] * Terrain.BlockSize + cubeLocalPosition;
							// modify vertex position according to ground noise or height noise
							if (Terrain.EnableVertexModifiers)
							{
								Vector3 vOffset;
								if (!vertexModifierCache.TryGetValue(vertexPosition, out vOffset))
								{
									vOffset = Terrain.GetVertexOffset(outVertices[k] * Terrain.BlockSize + cubeWorldPosition);
									vertexModifierCache.Add(vertexPosition, vOffset);
								}
								outVertices[k] = vertexPosition + vOffset;
							}
							else
							{
								outVertices[k] = vertexPosition;
							}
						}

						int layer = 0;
						Color32 color;
						// work on triangles now
						for (int t = 0; t < verticeCount; t += 3)
						{
							vertexPosition = outVertices[t];
							vertexPosition.x += Position.x;
							vertexPosition.z += Position.z;
							Vector3 normal = Vector3.Cross(outVertices[t + 1] - outVertices[t], outVertices[t + 2] - outVertices[t]).normalized;
							layer = y;
							if (!flatArray[t / 3] && y == 0)
							{
								layer = 1;
							}
							if (layer > 1)
							{
								layer = 1;
							}
							color = Terrain.Colors.BlendColor(vertexPosition);
							int nExistingVertice = chunkMeshDataLayer[layer].vertices.Count;
							for (int i = 0; i < 3; ++i)
							{
								chunkMeshDataLayer[layer].colors.Add(color);
								chunkMeshDataLayer[layer].normals.Add(normal);
								chunkMeshDataLayer[layer].triangles.Add(nExistingVertice + i);
								chunkMeshDataLayer[layer].vertices.Add(outVertices[t + i]);
							}
							if (chunkMeshDataLayer[layer].vertices.Count > maxVertices)
							{
								_chunkMeshesData.Add(chunkMeshDataLayer[layer]);
								chunkMeshDataLayer[layer] = new ChunkMeshData(layer);
							}
						}
					}
				}
			}

			_chunkMeshesData.Add(chunkMeshDataLayer[0]);
			_chunkMeshesData.Add(chunkMeshDataLayer[1]);

			foreach (Chunk sideChunk in _sideChunks)
			{
				sideChunk.CreateMeshData();
			}
			pe.End();
		}

		public int GetNumberOfMesh()
		{
			int n = 0;
			if (null != _chunkMeshesData)
			{
				foreach (ChunkMeshData chunkMeshData in _chunkMeshesData)
				{
					if (chunkMeshData.vertices.Count != 0)
					{
						n++;
					}
				}
			}
			foreach (Chunk sideChunk in _sideChunks)
			{
				n += sideChunk.GetNumberOfMesh();
			}
			return n;
		}

		public IEnumerable<GameObject> CreateMesh()
		{
			// check some data before
			if (_chunkMeshesData == null)
			{
				Log("There is no data available to create the mesh");
				yield break;
			}

			int meshIndex = 0;
			foreach (ChunkMeshData chunkMeshData in _chunkMeshesData)
			{
				if (chunkMeshData.vertices.Count == 0)
				{
					Log("skip layer #" + chunkMeshData.layer + " there is no vertices");
					continue;
				}

				GameObject meshObject = new GameObject("Chunk_" + _index + "_" + chunkMeshData.layer + "_" + meshIndex++);
				meshObject.transform.position = new Vector3(Position.x, 0f, Position.z);
				if (chunkMeshData.layer == 0)
				{
					meshObject.layer = Terrain.GroundLayer;
				}
				else
				{
					meshObject.layer = Terrain.HeightLayer;
				}

				Mesh mesh = new Mesh
				{
					name = meshObject.name,
					vertices = chunkMeshData.vertices.ToArray(),
					triangles = chunkMeshData.triangles.ToArray()
				};

				//The diffuse shader wants uvs so just fill with a empty array, there not actually used
				mesh.uv = new Vector2[mesh.vertices.Length];
				mesh.colors32 = chunkMeshData.colors.ToArray();
				mesh.normals = chunkMeshData.normals.ToArray();

				meshObject.AddComponent<MeshFilter>().mesh = mesh;
				//				GameObject.Destroy(mesh);
				meshObject.AddComponent<MeshRenderer>();
				meshObject.AddComponent<MeshCleaner>();
				meshObject.GetComponent<Renderer>().sharedMaterial = Terrain.Material;
				meshObject.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
				meshObject.isStatic = true;
				meshObject.AddComponent<MeshCollider>().sharedMaterial = Resources.Load("Physics/Scenery") as PhysicMaterial;
				yield return meshObject;
			}

			foreach (Chunk sideChunk in _sideChunks)
			{
				IEnumerator<GameObject> objEnumerator = sideChunk.CreateMesh().GetEnumerator();
				while (objEnumerator.MoveNext())
				{
					yield return objEnumerator.Current;
				}
			}
		}

		public bool IsBorder(int x, int z)
		{
			return (z < _nBorderBottom && (x < _nBorderBottom || x > _nBorderTop)) // bottom left and right corner
				|| (z > _nBorderTop && (x < _nBorderBottom || x > _nBorderTop)) // up left and right corner
				|| (_edges[0] && z > _nBorderTop) // upper border
				|| (_edges[1] && x > _nBorderTop) // right border
				|| (_edges[2] && z < _nBorderBottom) // bottom border
				|| (_edges[3] && x < _nBorderBottom); // right border
		}

		public bool IsBorderOffset(int x, int z, int offset)
		{
			int bottom = _nBorderBottom + offset;
			int top = _nBorderTop - offset;
			return (z < bottom && (x < bottom || x > top)) // bottom left and right corner
				|| (z > top && (x < bottom || x > top)) // up left and right corner
					|| (_edges[0] && z > top) // upper border
					|| (_edges[1] && x > top) // right border
					|| (_edges[2] && z < bottom) // bottom border
					|| (_edges[3] && x < bottom); // right border
		}

		private bool IsOnEdge(int x, int z)
		{
			if (_edges[0] && z > _nBorderTop)
			{
				return true;
			}
			if (_edges[1] && x > _nBorderTop)
			{
				return true;
			}
			if (_edges[2] && z < _nBorderBottom)
			{
				return true;
			}
			if (_edges[3] && x < _nBorderBottom)
			{
				return true;
			}
			return false;
		}

		private void SetTunnelData(int idx)
		{
			if (_tunnelData == null)
			{
				_tunnelData = new bool[_nScalarXZ * _nScalarXZ];
			}
			_tunnelData[idx] = true;
		}

		public void SetTunnelData(Int2 pos)
		{
			int idx = pos.z * _nScalarXZ + pos.x;

			if (IsBorder(pos.x, pos.z))
			{
				return;
			}
			SetTunnelData(idx);
			SetTunnelData(idx + 1);
			SetTunnelData(idx + _nScalarXZ);
			SetTunnelData(idx + _nScalarXZ + 1);

			// right border
			if (pos.x == Terrain.NumberOfBlockXZ - 1 && _neighboors[1] != null)
			{
				idx = pos.z * _nScalarXZ;
				_neighboors[1].SetTunnelData(idx);
				_neighboors[1].SetTunnelData(idx + _nScalarXZ);
			}

			// top border
			if (pos.z == Terrain.NumberOfBlockXZ - 1 && _neighboors[0] != null)
			{
				idx = pos.x;
				_neighboors[0].SetTunnelData(idx);
				_neighboors[0].SetTunnelData(idx + 1);
			}

			// left border
			if (pos.x == 0 && _neighboors[3] != null)
			{
				idx = pos.z * _nScalarXZ + (_nScalarXZ - 1);
				_neighboors[3].SetTunnelData(idx);
				_neighboors[3].SetTunnelData(idx + _nScalarXZ);
			}

			// bottom border
			if (pos.z == 0 && _neighboors[2] != null)
			{
				idx = (_nScalarXZ - 1) * _nScalarXZ + pos.x;
				_neighboors[2].SetTunnelData(idx);
				_neighboors[2].SetTunnelData(idx + 1);
			}
		}

		public bool IsWalkable(Int2 pos)
		{
			if (pos.x < 0 || pos.z < 0 || pos.x >= Terrain.NumberOfBlockXZ || pos.z >= Terrain.NumberOfBlockXZ)
			{
				return false;
			}
			return GetMaxHeight(pos) < 1 / (float)Terrain.NumberOfBlockY;
		}

		public float GetWeight(Int2 pos)
		{
			if (pos.x < 0 || pos.z < 0 || pos.x >= Terrain.NumberOfBlockXZ || pos.z >= Terrain.NumberOfBlockXZ)
			{
				return float.MaxValue;
			}
			int idx = pos.z * (_nScalarXZ - 1) + pos.x;
			return _weights[idx];
		}

		/// <summary>
		/// Gets the center in local block position
		/// </summary>
		/// <returns>The center.</returns>
		public Int2 GetCenterBlock()
		{
			return new Int2((Terrain.NumberOfBlockXZ) / 2, (Terrain.NumberOfBlockXZ) / 2);
		}

		public Vector3 GetCenter()
		{
			return new Vector3(Terrain.ChunkSizeXZ / 2f, 0f, Terrain.ChunkSizeXZ / 2f) + Position;
		}

		public Vector3 GetCenterByRaycast()
		{
			Vector3 position = GetCenter();
			position.y = Terrain.ChunkSizeY;
			RaycastHit hit;
			if (Physics.Raycast(position, -Vector3.up, out hit))
			{
				return hit.point;
			}
			else
			{
				position.y = 0f;
			}
			return position;
		}

		public void SetNeighboor(Chunk neighboor)
		{
			if (neighboor.Position.x > Position.x)
			{
				_neighboors[1] = neighboor;
				neighboor._neighboors[3] = this;
			}
			else if (neighboor.Position.x < Position.x)
			{
				_neighboors[3] = neighboor;
				neighboor._neighboors[1] = this;
			}
			else if (neighboor.Position.z > Position.z)
			{
				_neighboors[0] = neighboor;
				neighboor._neighboors[2] = this;
			}
			else if (neighboor.Position.z < Position.z)
			{
				_neighboors[2] = neighboor;
				neighboor._neighboors[0] = this;
			}
		}

		private float GetMaxHeight(Int2 pos)
		{
			int idx = pos.z * _nScalarXZ + pos.x;
			float max = 0f;
			if (_tunnelData != null)
			{
				float cur = 0f;
				if (!_tunnelData[idx])
				{
					cur = _heights[idx];
					max = max > cur ? max : cur;
				}

				if (!_tunnelData[idx + 1])
				{
					cur = _heights[idx + 1];
					max = max > cur ? max : cur;
				}

				if (!_tunnelData[idx + _nScalarXZ + 1])
				{
					cur = _heights[idx + _nScalarXZ + 1];
					max = max > cur ? max : cur;
				}

				if (!_tunnelData[idx + _nScalarXZ])
				{
					cur = _heights[idx + _nScalarXZ];
					max = max > cur ? max : cur;
				}
			}
			else
			{
				float cur = _heights[idx];
				max = max > cur ? max : cur;

				cur = _heights[idx + 1];
				max = max > cur ? max : cur;

				cur = _heights[idx + _nScalarXZ + 1];
				max = max > cur ? max : cur;

				cur = _heights[idx + _nScalarXZ];
				max = max > cur ? max : cur;
			}
			return max;
		}

		public Rect GetSurroundSurface()
		{
			return new Rect(Position.x - Terrain.ChunkSizeXZ, Position.z - Terrain.ChunkSizeXZ, 3f * Terrain.ChunkSizeXZ, 3f * Terrain.ChunkSizeXZ);
		}

		public List<Rect> GetSurfaces()
		{
			List<Rect> surfaces = new List<Rect>();
			float edgeLength = Terrain.ChunkSizeXZ - 2f * Terrain.BorderSize;
			// center
			surfaces.Add(new Rect(Position.x + Terrain.BorderSize, Position.z + Terrain.BorderSize, edgeLength, edgeLength));

			// top edge
			if (!_edges[0])
			{
				surfaces.Add(new Rect(Position.x + Terrain.BorderSize, Position.z + Terrain.ChunkSizeXZ - Terrain.BorderSize, edgeLength, Terrain.BorderSize));
			}

			// right edge
			if (!_edges[1])
			{
				surfaces.Add(new Rect(Position.x + Terrain.ChunkSizeXZ - Terrain.BorderSize, Position.z + Terrain.BorderSize, Terrain.BorderSize, edgeLength));
			}

			// bottom edge
			if (!_edges[2])
			{
				surfaces.Add(new Rect(Position.x + Terrain.BorderSize, Position.z, edgeLength, Terrain.BorderSize));
			}

			// left edge
			if (!_edges[3])
			{
				surfaces.Add(new Rect(Position.x, Position.z + Terrain.BorderSize, Terrain.BorderSize, edgeLength));
			}
			return surfaces;
		}

		public override string ToString()
		{
			return string.Format("[Chunk: Position={0}, Coordinates={1}, BlockPosition={2}, InDirection={3}, OutDirection={4}, Indice={5}, InPosition={6}, OutPosition={7}]", Position, ChunkPosition, BlockPosition, InDirection, OutDirection, Index, InPosition, OutPosition);
		}

		private void Log(string text)
		{
			TSW.Log.Logger.Add("Chunk/ " + Position + " indice:" + _index + " - " + text);
		}

		/// <summary>
		/// Define x and z offset from a point to create a cube
		///
		///    7 +------+ 6
		///     /|     /|
		///  3 +-+----+ |
		///    | |  2 | |
		///    | +----+-+ 5
		///    |/ 4   |/
		///    +------+
		///   0        1
		/// </summary>
		private static readonly int[] _cubeOffset = new int[] {
		//  z, x, y,
			0, 0, 0,
			0, 1, 0,
			0, 1, 1,
			0, 0, 1,
			1, 0, 0,
			1, 1, 0,
			1, 1, 1,
			1, 0, 1
		};


	}
}