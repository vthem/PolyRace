using System.Collections;
using System.Collections.Generic;

using TSW;
using TSW.Struct;

using UnityEngine;

using SysRand = System.Random;

namespace LevelGen.Jobs
{
	public class CloudObjectCreator : Job
	{
		private readonly TidyGameObjectDelegate _tidyGameObject;
		private readonly CloudProfile _cloudProfile;
		private readonly OverlappingChecker _overlapChecker;

		private struct ObjectToCreate
		{
			public Vector3 _position;
			public Vector3 _scale;
			public Vector3 _rotation;
			public GameObject _prefab;
		}

		public CloudObjectCreator(LevelProfile level, TidyGameObjectDelegate tidyGameObject, OverlappingChecker overlapChecker) : base(level)
		{
			Weight = 80f;
			_runType = RunType.RunInCoroutine;
			_tidyGameObject = tidyGameObject;
			_cloudProfile = level._cloud;
			_overlapChecker = overlapChecker;
		}

		protected override IEnumerator RunByStep()
		{
			if (!_cloudProfile._enable)
			{
				yield break;
			}
			List<ObjectToCreate> objToCreates = SetTotalStepRequired();
			foreach (ObjectToCreate obj in objToCreates)
			{
				CreateObject(obj);
				yield return null;
			}
			yield break;
		}

		private List<ObjectToCreate> SetTotalStepRequired()
		{
			List<ObjectToCreate> objToCreates = new List<ObjectToCreate>();
			foreach (Chunk chunk in ChunksNoSeam())
			{
				Rect surface = chunk.GetSurroundSurface();
				SysRand rand = new SysRand(chunk.Index + _levelProfile.SeedInt);
				foreach (Vector3 position in ObjectCreator.InSurfaceDistribution(surface, _levelProfile._cloud._step, _levelProfile.Terrain.ChunkSizeY))
				{
					if (_overlapChecker.Check(position))
					{
						continue;
					}
					else
					{
						_overlapChecker.Set(position);
					}
					ObjectToCreate objToCreate = new ObjectToCreate();
					Vector2 unitCircle = rand.InsideUnitCircle() * _cloudProfile._randomRadius;
					objToCreate._position = new Vector3(
						position.x + unitCircle.x,
						rand.Range(_levelProfile.Terrain.ChunkSizeY * _cloudProfile._minHeight, _levelProfile.Terrain.ChunkSizeY * _cloudProfile._maxHeight),
						position.z + unitCircle.y
					);
					objToCreate._prefab = GetPrefab(_cloudProfile._prefabs, rand);
					objToCreate._scale = rand.RangeVector3(_cloudProfile._minScale, _cloudProfile._maxScale);
					objToCreate._rotation = rand.RangeVector3(-_cloudProfile._rotation, _cloudProfile._rotation);
					objToCreates.Add(objToCreate);
				}
			}
			_totalStep = objToCreates.Count;
			return objToCreates;
		}

		private void CreateObject(ObjectToCreate objToCreate)
		{
			GameObject obj = GameObject.Instantiate(objToCreate._prefab);
			obj.transform.position = objToCreate._position;
			obj.name = CreateObjectName(objToCreate._prefab.name, obj.transform.position);
			obj.isStatic = true;
			obj.transform.localScale += objToCreate._scale;
			obj.transform.eulerAngles = objToCreate._rotation;
			_tidyGameObject(obj, LevelObjectType.Cloud, false);
		}

		private string CreateObjectName(string prefabName, Vector3 position)
		{
			return prefabName + "[" + Mathf.FloorToInt(position.x / _cloudProfile._step) + "," + Mathf.FloorToInt(position.z / _cloudProfile._step) + "]";
		}

		private static GameObject GetPrefab(PrefabGroup prefabGroup, SysRand rand)
		{
			int v = rand.Range(0, prefabGroup.Objects.Count);
			return prefabGroup.Objects[v];
		}
	}
}
