using System.Collections;
using System.Collections.Generic;

using TSW;
using TSW.Algorithm;

using UnityEngine;

using SysRand = System.Random;

namespace LevelGen.Jobs
{
	public class SceneryObjectCreator : Job
	{
		private readonly TidyGameObjectDelegate _tidyGameObject;
		private readonly OverlappingChecker _overlapingChecker;

		public SceneryObjectCreator(LevelProfile level, TidyGameObjectDelegate tidyGameObject, OverlappingChecker overlapingChecker) : base(level)
		{
			Weight = 80f;
			_runType = RunType.RunInCoroutine;
			_tidyGameObject = tidyGameObject;
			_overlapingChecker = overlapingChecker;
		}

		protected override IEnumerator RunByStep()
		{
			if (!_levelProfile._scenery._enable)
			{
				yield break;
			}
			SetTotalStepRequired();
			foreach (Chunk chunk in ChunksNoSeam())
			{
				Rect surface = ObjectCreator.CombineSurface(chunk.GetSurfaces());
				SysRand rand = new SysRand(_levelProfile.SeedInt + chunk.Index);
				Vector3 createPosition;
				foreach (Vector3 position in ObjectCreator.InSurfaceDistribution(surface, _levelProfile._scenery._step, _levelProfile.Terrain.ChunkSizeY))
				{
					yield return null;
					Vector2 unitCircle = rand.InsideUnitCircle() * _levelProfile._scenery._randomRadius;
					createPosition = position;
					createPosition.x += unitCircle.x;
					createPosition.z += unitCircle.y;
					if (rand.Range(0f, 1f) > _levelProfile._scenery._probability)
					{
						continue;
					}
					if (!_levelProfile.Terrain.GetTerrainHeight(ref createPosition))
					{
						continue;
					}
					if (_overlapingChecker.Check(createPosition))
					{
						continue;
					}
					int prefabGroupIndex = GetPrefabGroupIndex(createPosition.y / _levelProfile.Terrain.ChunkSizeY, rand);
					CreateObject(createPosition, prefabGroupIndex, rand);
				}
			}
			yield break;
		}

		private void SetTotalStepRequired()
		{
			_totalStep = 0;
			foreach (Chunk chunk in ChunksNoSeam())
			{
				Rect surface = ObjectCreator.CombineSurface(chunk.GetSurfaces());
				IEnumerator<Vector3> enumerator = ObjectCreator.InSurfaceDistribution(surface, _levelProfile._scenery._step, _levelProfile.Terrain.ChunkSizeY).GetEnumerator();
				while (enumerator.MoveNext())
				{
					_totalStep++;
				}
			}
		}

		private int GetPrefabGroupIndex(float heightNormalized, SysRand rand)
		{
			List<float> probArray = new List<float>();
			float total = 0f;
			foreach (SceneryPrefabGroup prefabGroup in _levelProfile._scenery._prefabGroups)
			{
				float proba = prefabGroup._probability.Evaluate(heightNormalized);
				total += proba;
				probArray.Add(proba);
			}
			return Probability.GetProbabilityIndex(probArray, rand.Range(0f, total));
		}

		private void CreateObject(Vector3 position, int prefabGroupIndex, SysRand rand)
		{
			SceneryPrefabGroup sceneryPrefabGroup = _levelProfile._scenery._prefabGroups[prefabGroupIndex];
			GameObject prefab = sceneryPrefabGroup._prefabGroup.Objects[rand.Range(0, sceneryPrefabGroup._prefabGroup.Objects.Count)];
			GameObject obj = GameObject.Instantiate(prefab, position, Quaternion.identity);
			obj.name = CreateObjectName(prefab.name, position);
			obj.transform.localScale += rand.RangeVector3(sceneryPrefabGroup._minScale, sceneryPrefabGroup._maxScale);
			obj.transform.eulerAngles = rand.RangeVector3(-sceneryPrefabGroup._rotation, sceneryPrefabGroup._rotation);
			obj.isStatic = true;
			_tidyGameObject(obj, LevelObjectType.Obstacle, false);
		}

		private string CreateObjectName(string prefabName, Vector3 position)
		{
			return prefabName + "[" + Mathf.FloorToInt(position.x / _levelProfile._scenery._step) + "," + Mathf.FloorToInt(position.z / _levelProfile._scenery._step) + "]";
		}
	}
}
