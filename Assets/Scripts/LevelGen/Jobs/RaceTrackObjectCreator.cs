using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace LevelGen.Jobs
{
	public class RaceTrackObjectCreator : Job
	{
		private int _arrowCount = 0;
		private bool _firstPass = true;
		private readonly TidyGameObjectDelegate _addChildObject;
		private readonly OverlappingChecker _groundOverlapChecker;
		private Transform _lastArrow;

		public RaceTrackObjectCreator(LevelProfile level, TidyGameObjectDelegate addChildObject, OverlappingChecker groundOverlapChecker) : base(level)
		{
			Weight = 40f;
			_runType = RunType.RunInCoroutine;
			_addChildObject = addChildObject;
			_groundOverlapChecker = groundOverlapChecker;
		}

		protected override IEnumerator RunByStep()
		{
			_totalStep = 1f;
			bool lastBatch = _chunks[_chunks.Count - 1].Index == _levelProfile.ShapeLength - 1;

			List<Chunk.PathData> pathDatas = new List<Chunk.PathData>();
			foreach (Chunk chunk in ChunksNoSeam())
			{
				chunk.AppendPathDatas(pathDatas);
			}
			_totalStep = pathDatas.Count + 1;
			yield return null;
			pathDatas.Sort();

			List<Transform> arrows = new List<Transform>();
			float scaleModifier = 1f;
			int scaleReduceFrom = 10;
			for (int i = 0; i < pathDatas.Count - 1; ++i)
			{
				_groundOverlapChecker.Set(pathDatas[i]._position);
				if (i > pathDatas.Count - scaleReduceFrom && lastBatch)
				{
					scaleModifier = (pathDatas.Count - i) / (float)scaleReduceFrom;
				}
				CreateArrow(pathDatas[i]._position, arrows, scaleModifier);
				yield return null;
			}

			if (_firstPass)
			{
				AddStartArch();
			}
			if (lastBatch)
			{
				SetLastArrowForward(AddEndArch());
			}
			_firstPass = false;
			yield break;
		}

		private void AddStartArch()
		{
			Vector3 position = _chunks[0].GetCenterByRaycast();
			GameObject gate = GameObject.Instantiate(_levelProfile._raceTrack._startArchPrefab);
			_levelProfile.Terrain.GetGroundHeight(ref position);
			gate.transform.position = position;
			gate.name = "Start";
			SetGatePivotHeight(gate.transform, "Left_Pivot");
			SetGatePivotHeight(gate.transform, "Right_Pivot");
			_addChildObject(gate, LevelObjectType.Gate, true);
			_groundOverlapChecker.Set(_chunks[0].Position, _levelProfile.Terrain.ChunkSizeXZ, 3 * _levelProfile.Terrain.ChunkSizeXZ / 4f);
		}

		private Transform AddEndArch()
		{
			Vector3 position = _chunks[_chunks.Count - 1].GetCenterByRaycast();
			GameObject gate = GameObject.Instantiate(_levelProfile._raceTrack._endArchPrefab);
			_levelProfile.Terrain.GetGroundHeight(ref position);
			gate.transform.position = position;
			gate.name = "Finish";
			switch (_chunks[_chunks.Count - 1].InDirection)
			{
				case 0:
					gate.transform.eulerAngles = new Vector3(0, 180, 0);
					break;
				case 1:
					gate.transform.eulerAngles = new Vector3(0, -90, 0);
					break;
				case 2:
					gate.transform.eulerAngles = new Vector3(0, 0, 0);
					break;
				case 3:
					gate.transform.eulerAngles = new Vector3(0, 90, 0);
					break;
			}

			_addChildObject(gate, LevelObjectType.Gate, true);
			_groundOverlapChecker.Set(position);
			Chunk chunk = _chunks[_chunks.Count - 1];
			float chunkSizeXZ = _levelProfile.Terrain.ChunkSizeXZ;
			Vector3 start = chunk.Position;
			if (chunk.InDirection == 0)
			{
				_groundOverlapChecker.Set(start, chunkSizeXZ, 3 * chunkSizeXZ / 4f);
			}
			else
			{
				start.z += chunkSizeXZ / 4;
				_groundOverlapChecker.Set(start, chunkSizeXZ, 3 * chunkSizeXZ / 4f);
			}
			SetGatePivotHeight(gate.transform, "Left_Pivot");
			SetGatePivotHeight(gate.transform, "Right_Pivot");
			return gate.transform;
		}

		private void SetLastArrowForward(Transform endArch)
		{
			if (_lastArrow != null)
			{
				_lastArrow.transform.forward = (endArch.position - _lastArrow.transform.position).normalized;
			}
		}

		private static readonly Vector3 _minScale = new Vector3(.1f, .1f, .1f);

		private void CreateArrow(Vector3 position, List<Transform> arrows, float scaleModifier)
		{
			if (_levelProfile.Terrain.GetGroundHeight(ref position))
			{
				GameObject arrow = GameObject.Instantiate(_levelProfile._raceTrack._arrowPrefab);
				arrow.name = _arrowCount.ToString("D5");
				arrow.tag = "Arrow";
				arrow.transform.position = position;
				arrow.transform.localScale = Vector3.Lerp(_minScale, arrow.transform.localScale, scaleModifier);
				_groundOverlapChecker.Set(position);
				_addChildObject(arrow, LevelObjectType.Arrow, true);
				arrows.Add(arrow.transform);
				_arrowCount++;
				if (_lastArrow != null)
				{
					_lastArrow.transform.forward = (arrow.transform.position - _lastArrow.transform.position).normalized;
				}
				_lastArrow = arrow.transform;
			}
			else
			{
				TSW.Log.Logger.Add("Fail to create arrow at position:" + position);
			}
		}

		private void SetGatePivotHeight(Transform gate, string childName)
		{
			Transform pivot = gate.Find(childName);
			if (null == pivot)
			{
				Debug.LogWarning("Could not set pivot height - pivot not found - " + childName);
				return;
			}

			Vector3 position = pivot.transform.position;
			float height = pivot.transform.localPosition.y;
			if (!_levelProfile.Terrain.GetGroundHeight(ref position))
			{
				Debug.LogWarning("Could not set pivot height - could not get terrain height");
				return;
			}
			position.y += height;
			pivot.transform.position = position;
		}
	}
}
