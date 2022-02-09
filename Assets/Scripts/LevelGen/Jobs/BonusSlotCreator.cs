using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace LevelGen.Jobs
{
	public class BonusSlotCreator : Job
	{
		private readonly TidyGameObjectDelegate _addChildObject;
		private readonly OverlappingChecker _groundOverlapChecker;
		private readonly RaceTrackProfile _trackProfile;
		private float _currentDistance = 0f;
		private int _bonusId = 0;

		public BonusSlotCreator(LevelProfile level, TidyGameObjectDelegate addChildObject, OverlappingChecker groundOverlapChecker) : base(level)
		{
			Weight = 40f;
			_runType = RunType.RunInCoroutine;
			_addChildObject = addChildObject;
			_groundOverlapChecker = groundOverlapChecker;
			_trackProfile = _levelProfile._raceTrack;
		}

		protected override IEnumerator RunByStep()
		{
			if (_chunks[0].Index == 0)
			{
				_bonusId = 0;
				_currentDistance = 0f;
			}
			_totalStep = 1f;
			yield return null;

			List<Chunk.PathData> pathDatas = new List<Chunk.PathData>();
			foreach (Chunk chunk in ChunksNoSeam())
			{
				chunk.AppendPathDatas(pathDatas);
			}
			_totalStep = pathDatas.Count + 1;

			pathDatas.Sort();

			float baseDistance = _trackProfile._averageSpeed * _trackProfile._bonusTimer / 3.6f;
			for (int i = 0; i < pathDatas.Count - 1; ++i)
			{
				_currentDistance += _trackProfile._minArrowDistance;
				if (_currentDistance < baseDistance)
				{
					continue;
				}

				Vector3 position = pathDatas[i]._position;
				if (UnderCavern(position))
				{
					continue;
				}

				CreateBonusSlot(position);
				_currentDistance = 0f;
			}

			yield break;
		}

		private bool UnderCavern(Vector3 position)
		{
			return Physics.Raycast(position, Vector3.up, _levelProfile.Terrain.HeightLayer);
		}

		private void CreateBonusSlot(Vector3 position)
		{
			if (_levelProfile.Terrain.GetGroundHeight(ref position))
			{
				GameObject bonus = GameObject.Instantiate(_trackProfile._bonusSlotPrefab);
				bonus.name = BonusSlotName(++_bonusId);
				bonus.transform.position = position;
				_groundOverlapChecker.Set(position);
				_addChildObject(bonus, LevelObjectType.Bonus, true);
			}
			else
			{
				TSW.Log.Logger.Add("Fail to create arrow at position:" + position);
			}
		}

		public static string BonusSlotName(int id)
		{
			return "BonusSlot[" + id.ToString("D2") + "]";
		}
	}
}
