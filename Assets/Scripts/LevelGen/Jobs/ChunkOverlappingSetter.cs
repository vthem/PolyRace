using System.Collections;

using UnityEngine;

namespace LevelGen.Jobs
{
	public class ChunkOverlappingSetter : Job
	{
		private readonly OverlappingChecker _overlappingChecker;

		public ChunkOverlappingSetter(LevelProfile level, OverlappingChecker overlappingChecker) : base(level)
		{
			Weight = 1f;
			_runType = RunType.RunInCoroutine;
			_overlappingChecker = overlappingChecker;
		}

		protected override IEnumerator RunByStep()
		{
			_totalStep = 1;
			int index = _chunks[0].Index;
			int length = _chunks.Count;
			if (_levelProfile.ShapeLength == -1)
			{
				length += length;
			}
			else
			{
				int toEnd = _levelProfile.ShapeLength - _chunks[_chunks.Count - 1].Index - 1;
				length += Mathf.Min(toEnd, length);
			}
			ShapeGenerator shapeGenerator = new ShapeGenerator(_levelProfile.ShapeLength, _levelProfile.SeedInt);
			foreach (LevelShapeCell cell in shapeGenerator.GetNextCell(index, length))
			{
				_overlappingChecker.Set(_levelProfile.ChunkToWorld(cell.Position));
			}
			yield break;
		}
	}
}
