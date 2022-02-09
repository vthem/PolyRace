using TSW;
using TSW.Struct;

using UnityEngine;

namespace LevelGen
{
	public sealed class LevelProfile : ScriptableObject
	{
		public TerrainProfile Terrain => _terrainProfile;

		[SerializeField]
		private TerrainProfile _terrainProfile;

		public RaceTrackProfile _raceTrack;

		public Scenery _scenery;

		public CloudProfile _cloud;

		public MountainProfile _mountain;

		public GameObject _gateGroundParticlePrefab;

		public GameObject _racerGroundParticlePrefab;

		public GameObject _airParticlePrefab;

		public GameObject _lightPrefab;

		public float _envLightIntensity;

		public Color _envLightColor;

		public Color _fogColor;

		public Material _skybox;

		public int SeedInt => SeedString.GetHashCode();

		public int ShapeLength { get; private set; }

		public string SeedString { get; private set; }

		public BuildOption _buildOption;

		[System.Serializable]
		public struct BuildOption
		{
			public bool _onlyTerrain;
		};

		public void Initialize(string seedString, int length)
		{
			try
			{
				Validate();
			}
			catch (System.Exception ex)
			{
				Debug.LogWarning("Fail to validate " + name + " message:" + ex.Message);
				return;
			}
			SeedString = seedString;
			ShapeLength = length;
			Terrain.Initialize(SeedInt);
			RenderSettings.fogColor = _fogColor;
			GameObject lightObj = GameObject.Find("Main_Light");
			if (null != lightObj)
			{
				lightObj.Destroy();
			}
			lightObj = GameObject.Instantiate(_lightPrefab);
			lightObj.name = "Main_Light";
			RenderSettings.skybox = _skybox;
			RenderSettings.ambientLight = _envLightColor;
			RenderSettings.ambientIntensity = _envLightIntensity;
			TSW.Log.Logger.Add("Initialized level profile with seed:" + SeedString + " / " + seedString);
		}

		public bool IsEndless()
		{
			return ShapeLength == -1;
		}

		public void Validate()
		{
			if (_terrainProfile == null)
			{
				throw new System.Exception("TerrainProfile is not set in " + name);
			}
			_terrainProfile.Validate();
			if (null == _lightPrefab)
			{
				Debug.LogWarning("LightPrefab is not set in " + name);
				return;
			}
		}

		public Int2 WorldToBlock(Vector3 point)
		{
			Int2 pos;
			pos.x = Mathf.FloorToInt((point.x / Terrain.BlockSize) - .5f);
			pos.z = Mathf.FloorToInt((point.z / Terrain.BlockSize) - .5f);
			return pos;
		}

		public Int2 WorldToChunk(Vector3 point)
		{
			return new Int2(Mathf.FloorToInt(point.x / Terrain.ChunkSizeXZ), Mathf.FloorToInt(point.z / Terrain.ChunkSizeXZ));
		}

		public Vector3 ChunkToWorld(Int2 chunkPos, float y = 0f)
		{
			return new Vector3(chunkPos.x * Terrain.ChunkSizeXZ, y, chunkPos.z * Terrain.ChunkSizeXZ);
		}

		public Vector3 BlockToWorld(Int2 block)
		{
			return new Vector3((block.x + .5f) * Terrain.BlockSize, 0f, (block.z + .5f) * Terrain.BlockSize);
		}

		public Int2 BlockToChunk(Int2 block)
		{
			block.x = Mathf.FloorToInt(block.x / (float)Terrain.NumberOfBlockXZ);
			block.z = Mathf.FloorToInt(block.z / (float)Terrain.NumberOfBlockXZ);
			return block;
		}
	}
}
