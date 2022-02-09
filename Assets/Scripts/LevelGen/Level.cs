using System.Collections.Generic;

using TSW;
using TSW.Struct;

using UnityEngine;

// threadings

namespace LevelGen
{
	public sealed class Level
	{
		private Transform _rootContainer;
		public Transform RootContainer => _rootContainer;

		private readonly Transform _staticContainer;
		private readonly Transform _dynamicContainer;
		private readonly List<Chunk> _chunks = new List<Chunk>();
		private readonly Dictionary<Int2, Chunk> _chunksByPosition = new Dictionary<Int2, Chunk>();
		private readonly Jobs.RaceTrackObjectCreator _raceTrackObjectCreator;
		private readonly Jobs.BonusSlotCreator _bonusSlotCreator;
		private readonly Jobs.RaceTrackFinder _raceTrackFinder;
		private Jobs.JobQueue _jobQueue;
		private readonly OverlappingChecker _cloudOverlappingChecker;
		private readonly OverlappingChecker _groundOverlappingChecker;
		private readonly OverlappingChecker _chunkOverlappingChecker;
		private readonly List<GameObject> _reUseObjects = new List<GameObject>();

		public Jobs.JobQueue JobQueue => _jobQueue;

		public List<Chunk> Chunks => _chunks;

		public LevelProfile Profile { get; private set; }

		public int LoadedCount { get; set; }

		public void AddReUseObject(GameObject obj)
		{
			_reUseObjects.Add(obj);
		}

		public Level(LevelProfile profile)
		{
			Profile = profile;

			TSW.Log.Logger.Add("New level with seed:" + profile.SeedString);

			_raceTrackFinder = new Jobs.RaceTrackFinder(Profile);

			_rootContainer = new GameObject("Level").transform;
			_dynamicContainer = _rootContainer.Find("Dynamic");
			if (_dynamicContainer == null)
			{
				_dynamicContainer = new GameObject("Dynamic").transform;
				_dynamicContainer.SetParent(_rootContainer);
			}
			_staticContainer = _rootContainer.Find("Static");
			if (_staticContainer == null)
			{
				_staticContainer = new GameObject("Static").transform;
				_staticContainer.SetParent(_rootContainer);
			}

			_groundOverlappingChecker = new OverlappingChecker(Profile._raceTrack._arrowOverlappingSize, Profile._raceTrack._arrowOverlappingSquare);
			_raceTrackObjectCreator = new Jobs.RaceTrackObjectCreator(Profile, AddChildObject, _groundOverlappingChecker);
			_bonusSlotCreator = new Jobs.BonusSlotCreator(Profile, AddChildObject, _groundOverlappingChecker);
			_cloudOverlappingChecker = new OverlappingChecker(Profile._cloud._step);
			_chunkOverlappingChecker = new OverlappingChecker(Profile.Terrain.ChunkSizeXZ);
		}

		public void Clear()
		{
			if (_rootContainer != null)
			{
				_groundOverlappingChecker.Clear();
				_chunkOverlappingChecker.Clear();
				_cloudOverlappingChecker.Clear();
				if (_jobQueue != null)
				{
					_jobQueue.Stop();
				}
				GameObject.DestroyImmediate(_rootContainer.gameObject);
			}
		}

		public void Create(int n, bool asynchronous = false)
		{
			Log("Create n:" + n + " LoadedCount:" + LoadedCount + " existing chunks:" + _chunks.Count);
			List<Chunk> chunks = new List<Chunk>();
			if (_chunks.Count > 1)
			{
				chunks.Add(_chunks[_chunks.Count - 2]);
				chunks.Add(_chunks[_chunks.Count - 1]);
			}
			if (Profile.ShapeLength != -1)
			{
				n = Mathf.Min(Profile.ShapeLength - LoadedCount + chunks.Count, n);
				Log("Limited level length. Reduce n:" + n);
			}
			if (asynchronous)
			{
				GetCreateJobQueue(n).RunAsync(chunks, 2, (chks) => { FinalizeJob(chks); });
			}
			else
			{
				GetCreateJobQueue(n).Run(chunks, 2);
				FinalizeJob(chunks);
			}
		}

		public void Reload()
		{
			for (int i = 0; i < _reUseObjects.Count; ++i)
			{
				_reUseObjects[i].SetActive(true);
			}
			_reUseObjects.Clear();
		}

		public Chunk GetChunkByPosition(Int2 pos)
		{
			Chunk chunk;
			_chunksByPosition.TryGetValue(pos, out chunk);
			return chunk;
		}

		public Chunk GetChunkByIndex(int index)
		{

			if (index == -1)
			{
				index = _chunks.Count - 1;
			}
			return _chunks[index];
		}

		public GameObject GetChunkDynamicContainer(int index)
		{
			if (null == _rootContainer)
			{
				Debug.LogWarning("Could not find chunk container for index:" + index + " - no root container");
				return null;
			}
			if (index >= _chunks.Count)
			{
				return null;
			}
			Transform chunkContainer = _dynamicContainer.Find(_chunks[index].ChunkPosition.ToString());
			if (null == chunkContainer)
			{
				//Debug.Log("Could not find chunk container for index:" + index + " - chunk not found");
				return null;
			}
			return chunkContainer.gameObject;
		}

		public GameObject GetChunkStaticContainer(int index)
		{
			if (null == _rootContainer)
			{
				Debug.LogWarning("Could not find chunk container for index:" + index + " - no root container");
				return null;
			}
			if (index >= _chunks.Count)
			{
				return null;
			}
			Transform chunkContainer = _staticContainer.Find(_chunks[index].ChunkPosition.ToString());
			if (null == chunkContainer)
			{
				//Debug.Log("Could not find chunk container for index:" + index + " - chunk not found");
				return null;
			}
			return chunkContainer.gameObject;
		}

		public List<Chunk.PathData> GetChunkPathData(int startIndex)
		{
			List<Chunk.PathData> pathDatas = new List<Chunk.PathData>();
			if (startIndex >= _chunks.Count)
			{
				return pathDatas;
			}
			for (int i = startIndex; i < _chunks.Count; ++i)
			{
				_chunks[i].AppendPathDatas(pathDatas);
			}
			pathDatas.Sort();
			return pathDatas;
		}

		private void FinalizeJob(List<Chunk> chunks)
		{
			foreach (Chunk chunk in chunks)
			{
				if (!_chunksByPosition.ContainsKey(chunk.ChunkPosition))
				{
					Log("Add chunk " + chunk + " to chunk created list and dictionary");
					_chunksByPosition.Add(chunk.ChunkPosition, chunk);
					_chunks.Add(chunk);
					LoadedCount += 1;
				}
			}
		}

		private Jobs.JobQueue GetCreateJobQueue(int n)
		{
			_jobQueue = new Jobs.JobQueue("Create");
			_jobQueue.Add(new Jobs.ChunkObjectCreator(Profile, n));
			_jobQueue.Add(new Jobs.ChunkOverlappingSetter(Profile, _chunkOverlappingChecker));
			_jobQueue.Add(new Jobs.ChunkDataCreator(Profile));
			if (!Profile._buildOption._onlyTerrain)
			{
				_jobQueue.Add(_raceTrackFinder);
			}
			_jobQueue.Add(new Jobs.ChunkMeshModeler(Profile));
			_jobQueue.Add(new Jobs.ChunkMeshCreator(Profile, AddChildObject));
			if (!Profile._buildOption._onlyTerrain)
			{
				_jobQueue.Add(_raceTrackObjectCreator);
				_jobQueue.Add(_bonusSlotCreator);
				_jobQueue.Add(new Jobs.SceneryObjectCreator(Profile, AddChildObject, _groundOverlappingChecker));
				_jobQueue.Add(new Jobs.SideArrowObjectCreator(Profile, AddChildObject));
				_jobQueue.Add(new Jobs.CloudObjectCreator(Profile, AddChildObject, _cloudOverlappingChecker));
				_jobQueue.Add(new Jobs.MountainObjectCreator(Profile, AddChildObject, _chunkOverlappingChecker));
			}
			_jobQueue.Add(new Jobs.Combiner(Profile, _staticContainer));
			return _jobQueue;
		}

		private string ChunkCoordinateName(Vector3 pos)
		{
			return Mathf.FloorToInt(pos.x / Profile.Terrain.ChunkSizeXZ) + "," + Mathf.FloorToInt(pos.z / Profile.Terrain.ChunkSizeXZ);
		}

		private Vector3 ChunkSnap(Vector3 pos)
		{
			pos.x = Mathf.FloorToInt(pos.x / Profile.Terrain.ChunkSizeXZ) * Profile.Terrain.ChunkSizeXZ;
			pos.y = Mathf.FloorToInt(pos.y / Profile.Terrain.ChunkSizeY) * Profile.Terrain.ChunkSizeY;
			pos.z = Mathf.FloorToInt(pos.z / Profile.Terrain.ChunkSizeXZ) * Profile.Terrain.ChunkSizeXZ;
			return pos;
		}

		public void AddChildObject(GameObject obj, LevelObjectType type, bool isDynamic = false)
		{
			if (null == _rootContainer)
			{
				_rootContainer = new GameObject("Level").transform;
			}
			Transform batchContainer = null;
			if (isDynamic)
			{
				batchContainer = _dynamicContainer;
			}
			else
			{
				batchContainer = _staticContainer;
			}

			string coordinateName = ChunkCoordinateName(obj.transform.position);
			Transform container = batchContainer.Find(coordinateName);
			if (container == null)
			{
				container = new GameObject(coordinateName).transform;
				container.SetParent(batchContainer);
				container.position = ChunkSnap(obj.transform.position);
			}
			Transform typeContainer = container.Find(type.ToString());
			if (typeContainer == null)
			{
				typeContainer = new GameObject(type.ToString()).transform;
				typeContainer.SetParent(container);
				typeContainer.localPosition = Vector3.zero;
			}
			Transform existing = typeContainer.Find(obj.name);
			if (existing != null)
			{
				Log("deleting existing obj:" + obj.name);
				existing.gameObject.Destroy();
			}
			obj.transform.SetParent(typeContainer);
		}

		private static void Log(string text)
		{
			TSW.Log.Logger.Add("Level/ " + text);
		}
	}
}
