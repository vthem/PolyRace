using System.Collections;

using UnityEngine;

namespace LevelGen.Jobs
{
	public class ChunkObjectCreator : Job
	{
		private readonly int _nChunkToAdd;

		public ChunkObjectCreator(LevelProfile level, int nChunkToAdd) : base(level)
		{
			_nChunkToAdd = nChunkToAdd;
			Weight = 6f;
			_runType = RunType.RunInCoroutine;
		}

		protected override IEnumerator RunByStep()
		{
			int n = _nChunkToAdd;
			_totalStep = n;

			int index = 0;
			if (_chunks.Count > 0)
			{
				index = _chunks[_chunks.Count - 1].Index + 1;
			}
			int count = n - _chunks.Count;
			if (count > 0)
			{
				ShapeGenerator shapeGanerator = new ShapeGenerator(_levelProfile.ShapeLength, _levelProfile.SeedInt);
				foreach (LevelShapeCell cell in shapeGanerator.GetNextCell(index, count))
				{
					if (index == 0)
					{
						AddChunk(cell, Chunk.NoDirection, cell.Direction, Chunk.Pattern.Flat);
					}
					else
					{
						Chunk lastChunk = _chunks[_chunks.Count - 1];
						if (index == 1)
						{
							AddChunk(cell, LevelShapeCell.ReverseDirection(lastChunk.OutDirection), cell.Direction, Chunk.Pattern.Gradient);
						}
						else if (index == _levelProfile.ShapeLength - 2)
						{
							AddChunk(cell, LevelShapeCell.ReverseDirection(lastChunk.OutDirection), cell.Direction, Chunk.Pattern.InverseGradient);

						}
						else if (index == _levelProfile.ShapeLength - 1)
						{
							AddChunk(cell, LevelShapeCell.ReverseDirection(lastChunk.OutDirection), Chunk.NoDirection, Chunk.Pattern.Flat);

						}
						else
						{
							AddChunk(cell, LevelShapeCell.ReverseDirection(lastChunk.OutDirection), cell.Direction, Chunk.Pattern.Default);
						}
					}
					yield return null;
					index++;
				}
			}
		}

		private void AddChunk(LevelShapeCell cell, int inDirection, int outDirection, Chunk.Pattern pattern)
		{
			int index = 0;
			if (_chunks.Count > 0)
			{
				index = _chunks[_chunks.Count - 1].Index + 1;
			}
			Chunk chunk = new Chunk(
				CellToVector3(cell),
				_levelProfile,
				inDirection,
				outDirection,
				index,
				pattern);
			_chunks.Add(chunk);
			if (_chunks.Count > 1)
			{
				_chunks[_chunks.Count - 2].SetNeighboor(_chunks[_chunks.Count - 1]);
			}
			Log("Added chunk: " + chunk);
		}

		private Vector3 CellToVector3(LevelShapeCell cell)
		{
			return new Vector3(cell.Position.x * _levelProfile.Terrain.ChunkSizeXZ, 0f, cell.Position.z * _levelProfile.Terrain.ChunkSizeXZ);
		}
	}
}
