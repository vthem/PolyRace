using System.Collections;
using System.Collections.Generic;

using TSW;
using TSW.Struct;

using UnityEngine;

using SysRand = System.Random;


namespace LevelGen.Jobs
{
	public class MountainObjectCreator : Job
	{
		private readonly TidyGameObjectDelegate _tidyGameObject;
		private readonly OverlappingChecker _overlapChecker;
		private readonly MountainProfile _mountainProfile;

		private struct ObjectToCreate
		{
			public Vector3 _position;
			public float _yScale;
			public Vector3 _rotation;
			public Mesh _mesh;
		}

		private static readonly float[] _rotation = new float[] {
			0f,
			90f,
			180f,
			270f
		};

		public MountainObjectCreator(LevelProfile level, TidyGameObjectDelegate tidyGameObject, OverlappingChecker overlapChecker) : base(level)
		{
			Weight = 80f;
			_runType = RunType.RunInCoroutine;
			_tidyGameObject = tidyGameObject;
			_overlapChecker = overlapChecker;
			_mountainProfile = level._mountain;
		}

		protected override IEnumerator RunByStep()
		{
			if (!_mountainProfile._enable)
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
				SysRand rand = new SysRand(chunk.Index + _levelProfile.SeedInt);
				Int2.Neighbors(chunk.ChunkPosition, (pos) =>
				{
					ObjectToCreate objToCreate = new ObjectToCreate();
					Vector3 position = _levelProfile.ChunkToWorld(pos);
					position.x += _levelProfile.Terrain.ChunkSizeXZ / 2f;
					position.z += _levelProfile.Terrain.ChunkSizeXZ / 2f;
					if (_overlapChecker.Check(position))
					{
						return;
					}
					else
					{
						_overlapChecker.Set(position);
					}
					objToCreate._mesh = _mountainProfile._meshes[rand.Range(0, _mountainProfile._meshes.Length)];
					objToCreate._position = position;
					objToCreate._rotation = RandRotation(rand);
					objToCreate._yScale = rand.Range(_mountainProfile.yMinScale, _mountainProfile.yMaxScale);
					objToCreates.Add(objToCreate);
				});
			}
			_totalStep = objToCreates.Count;
			return objToCreates;
		}

		private void CreateObject(ObjectToCreate objToCreate)
		{
			GameObject obj = new GameObject(objToCreate._mesh.name);
			obj.AddComponent<MeshFilter>().mesh = objToCreate._mesh;
			obj.AddComponent<MeshRenderer>().sharedMaterial = _mountainProfile._material;
			obj.GetComponent<MeshRenderer>().receiveShadows = false;
			obj.transform.position = objToCreate._position;
			obj.transform.localScale = new Vector3(_mountainProfile._xScale, objToCreate._yScale, _mountainProfile._zScale);
			obj.transform.eulerAngles = objToCreate._rotation;
			obj.isStatic = true;
			_tidyGameObject(obj, LevelObjectType.Mountain, false);
		}

		private static Vector3 RandRotation(SysRand rand)
		{
			return new Vector3(0f, _rotation[rand.Range(0, 4)], 0f);
		}
	}
}
