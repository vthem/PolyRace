using System.Collections;
using System.Collections.Generic;

using TSW;
using TSW.Algorithm.AStar;
using TSW.Struct;
using TSW.Unity;

using UnityEngine;

namespace LevelGen.Jobs
{
	public class RaceTrackFinder : Job
	{
		private readonly Dictionary<Int2, Chunk> _chunkByPosition = new Dictionary<Int2, Chunk>();
		private readonly Dictionary<Int2, Chunk> _chunkByPositionNoSeam = new Dictionary<Int2, Chunk>();
		private List<Int2> _pathLeftovers = new List<Int2>();
		private int _step = 0;
		private Int2 _lastValidPathBlock;
		private Vector3 _lastValidPathPoint;
		private int _pathIndex = 0;
		private EndChunkSelector _endChunkSelector;

		internal class EndChunkSelector
		{
			private int _currentChunkIndex;
			private readonly List<Chunk> _chunks;

			public EndChunkSelector(int startIndex, List<Chunk> chunks)
			{
				_currentChunkIndex = startIndex;
				_chunks = chunks;
			}

			public bool Update(int newIndex)
			{
				if (newIndex == _currentChunkIndex + 1)
				{
					int chunksOffsetIndex = _currentChunkIndex - _chunks[0].Index;
					if (chunksOffsetIndex + 1 < _chunks.Count)
					{
						++_currentChunkIndex;
						return true;
					}
				}
				return false;
			}

			public Int2 GetEndPosition()
			{
				Chunk chunk = GetEndChunk();
				return chunk.WorldBlockPosition(chunk.GetCenterBlock());
			}

			public Chunk GetEndChunk()
			{
				int lIndex = _currentChunkIndex - _chunks[0].Index;
				if (lIndex + 1 < _chunks.Count)
				{
					return _chunks[lIndex + 1];
				}
				return _chunks[lIndex];
			}
		}

		public RaceTrackFinder(LevelProfile level) : base(level)
		{
			Weight = 650f;
			_runType = RunType.RunInThread;
		}

		protected override IEnumerator RunByStep()
		{
			_step++;
			_totalStep = 1f;

			_chunkByPosition.Clear();
			_chunkByPositionNoSeam.Clear();
			foreach (Chunk chunk in _chunks)
			{
				_chunkByPosition[chunk.ChunkPosition] = chunk;
			}
			foreach (Chunk chunk in ChunksNoSeam())
			{
				_chunkByPositionNoSeam[chunk.ChunkPosition] = chunk;
			}

			int nBlock = _levelProfile.Terrain.NumberOfBlockXZ;
			Int2 start = new Int2(nBlock / 2, 3 * nBlock / 4);
			if (_chunks[0].Index > 0)
			{
				start = _pathLeftovers[_pathLeftovers.Count - 2];
			}

			Chunk startChunk = GetChunkByChunkPosition(_levelProfile.BlockToChunk(start));
			if (startChunk == null)
			{
				Log("Could not find start chunk at:" + start + " chunk:" + _levelProfile.BlockToChunk(start));
				throw new System.Exception("Could not find start chunk at:" + start + " chunk:" + _levelProfile.BlockToChunk(start));
			}
			Log("Scan start:" + start + " of " + startChunk.ChunkPosition + " index [" + startChunk.Index + "]" + " step:" + _step);

			_endChunkSelector = new EndChunkSelector(startChunk.Index, _chunks);

			Chunk endChunk = _endChunkSelector.GetEndChunk();
			Int2 end = _endChunkSelector.GetEndPosition();

			Log("Scan end:" + end + " of " + endChunk.ChunkPosition + " index [" + endChunk.Index + "]");

			PathFinder pathFinder = new PathFinder();
			List<Int2> path;
			int count = pathFinder.FindPath(start, end, GetWeight, Visiting, out path);
			Log("Scan count:" + count);
			if (count == 0)
			{
				throw new System.Exception("Fail to find race track path");
			}

			// Make the end of the race track
			endChunk = _endChunkSelector.GetEndChunk();
			if (endChunk.Index == _levelProfile.ShapeLength - 1)
			{
				for (int i = 0; i < path.Count; ++i)
				{
					Chunk chunk = GetChunkByChunkPosition(_levelProfile.BlockToChunk(path[i]));
					if (chunk != null && chunk.Index == endChunk.Index)
					{
						path = path.GetRange(0, i - 1);
						break;
					}
				}
				end = _endChunkSelector.GetEndPosition();
				path.Add(end);

				path = ReduceByAngle(path);
				path = ReduceByWeightDistanceAngle(path);

				path.Add(end);
			}
			else
			{
				// separate produced path into subpath (before the seam) and seam path (the path on
				// the two chunks that make the seam
				int seamIndex = ReverseIndexBeforeSeam(path);
				if (seamIndex == -1)
				{
					throw new System.Exception("Could not find seam last index");
				}
				List<Int2> subPath = path.GetRange(0, seamIndex + 1);

				// reduce the path produced by the A* algorithm (only on the subpath)
				subPath = ReduceByAngle(subPath);
				subPath = ReduceByWeightDistanceAngle(subPath);
				subPath = ReduceClose(subPath);

				// add two points of the seamPath. path = reduced(subPath) + twopoints(seamPath)
				List<Int2> seamPath = path.GetRange(seamIndex + 1, path.Count - seamIndex - 1);
				path = subPath;
				if (seamPath.Count < 4)
				{
					throw new System.Exception("Seam path is too small");
				}
				path.Add(seamPath[0]);
				path.Add(seamPath[3]);
			}

			// add first point if first chunk
			if (_chunks[0].Index == 0)
			{
				path.Insert(0, new Int2(nBlock / 2, nBlock / 2));
			}

			// add previous point at the seam
			if (_pathLeftovers.Count > 0)
			{
				path.Insert(0, _pathLeftovers[0]);
				path.Insert(0, _pathLeftovers[1]);
			}

			//			DrawPathPoints(path, Color.green, "final-path" + _step, 300f);

			CurvePath(path);
			_pathLeftovers = path.GetRange(path.Count - 4, 4);

			yield break;
		}

		private Vector3 GetStartPosition()
		{
			return _chunks[0].GetCenter() - new Vector3(0, 0, -_levelProfile.Terrain.ChunkSizeXZ / 4f);
		}

		private void CurvePath(List<Int2> blocks)
		{
			// DrawPathPoints(blocks, Color.red, "before-curve-" + _step, 150f);
			List<Vector3> path = FromBlocks(blocks);
			MakeCurve(path);
		}

		private List<Vector3> FromBlocks(List<Int2> blocks)
		{
			List<Vector3> path = new List<Vector3>();
			if (blocks.Count == 0)
			{
				return path;
			}
			Vector3 p;
			path.Capacity = blocks.Count;
			for (int i = 0; i < blocks.Count; ++i)
			{
				p = _levelProfile.Terrain.BlockToWorld(blocks[i], false);
				p.y = 0f;
				path.Add(p);
			}
			return path;
		}

		private int ReverseIndexBeforeSeam(List<Int2> path)
		{
			int lastChunkIndex = _chunks[_chunks.Count - 1].Index;
			for (int i = path.Count - 1; i >= 0; --i)
			{
				Chunk chunk = GetChunkByChunkPosition(_levelProfile.BlockToChunk(path[i]));
				if (chunk != null && chunk.Index < lastChunkIndex - 1)
				{
					return i;
				}
			}
			return -1;
		}

		private void MakeCurve(List<Vector3> path)
		{
			for (int i = 1; i < path.Count - 2; ++i)
			{
				MakeCurveSegment(path[i - 1], path[i], path[i + 1], path[i + 2]);
			}
		}

		private void Tangents(ref Vector3 p0, ref Vector3 p1, ref Vector3 p2, out Vector3 tanIn, out Vector3 tanOut)
		{
			Vector3 n1 = (p0 - p1).normalized;
			Vector3 n2 = (p2 - p1).normalized;
			Vector3 tan = (n1 + n2).normalized.Right(Vector3.up).normalized;
			tanIn = Mathf.Sign(Vector3.Dot(tan, n1)) * tan;
			tanOut = Mathf.Sign(Vector3.Dot(tan, n2)) * tan;
		}

		private void MakeCurveSegment(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
		{
			Vector3 tan1In, tan1Out, tan2In, tan2Out;
			Tangents(ref p0, ref p1, ref p2, out tan1In, out tan1Out);
			Tangents(ref p1, ref p2, ref p3, out tan2In, out tan2Out);
			float tanLength = _levelProfile._raceTrack._bezierTangentLength;
			Int2 pos = _levelProfile.WorldToBlock(p1);
			if (pos != _lastValidPathBlock)
			{
				if (AddPathToChunk(p1, pos))
				{
					_lastValidPathBlock = pos;
				}
			}
			foreach (Vector3 p in VectorUtils.BezierIterator(p1, p1 + tan1Out * tanLength, p2, p2 + tan2In * tanLength, .02f))
			{
				pos = _levelProfile.WorldToBlock(p);
				if (pos != _lastValidPathBlock)
				{
					if (AddPathToChunk(p, pos))
					{
						_lastValidPathBlock = pos;
					}
				}
			}
			//LevelDebug.DrawBezierPoint(p1, p1 + tan1Out * 20f, p2, p2 + tan2In * 20f, "bezier-ct-" + _step, Color.blue);
			//LevelDebug.DrawBezierSegment(p1, p1 + tan1Out * 20f, p2, p2 + tan2In * 20f, "bezier", Color.black);
		}

		private bool AddPathToChunk(Vector3 world, Int2 block)
		{
			Chunk chunk;
			if (!_chunkByPositionNoSeam.TryGetValue(_levelProfile.BlockToChunk(block), out chunk))
			{
				return false;
			}

			if (Mathf.Abs((_lastValidPathPoint - world).magnitude) > _levelProfile._raceTrack._minArrowDistance)
			{
				chunk.AddPath(world, chunk.LocalBlockPosition(block), _pathIndex++);
				_lastValidPathPoint = world;
			}

			bool walkable = chunk.IsWalkable(chunk.LocalBlockPosition(block));
			if (!walkable)
			{
				chunk.SetTunnelData(chunk.LocalBlockPosition(block));
				foreach (Int2 neighbor in Int2.Neighbors(block))
				{
					Chunk nchunk = GetChunkByChunkPosition(_levelProfile.BlockToChunk(neighbor));
					if (nchunk != null)
					{
						nchunk.SetTunnelData(chunk.LocalBlockPosition(neighbor));
					}
				}
			}
			return true;
		}

		private List<Int2> ReduceByWeightDistanceAngle(List<Int2> path)
		{
			LinkedList<Int2> nodes = new LinkedList<Int2>(path);
			if (nodes.First == null || nodes.First.Next == null)
			{
				return path;
			}
			LinkedListNode<Int2> cur = nodes.First.Next;
			int count = 0;
			while (true)
			{
				if (cur.Next == null)
				{
					if (count > 0 && nodes.First != null && nodes.First.Next != null)
					{
						count = 0;
						cur = nodes.First.Next;
						continue;
					}
					else
					{
						break;
					}
				}
				else
				{
					Int2 start = cur.Previous.Value;
					Int2 mid = cur.Value;
					Int2 end = cur.Next.Value;
					float weight = LineWeight(start, mid);
					weight += LineWeight(mid, end);
					if ((end - start).Magnitude < 50f && LineWeight(start, end) <= weight + _levelProfile._raceTrack._weightReduceTolerance)
					{
						cur = cur.Next;
						nodes.Remove(cur.Previous);
						count++;
					}
					else
					{
						cur = cur.Next;
					}
				}
			}
			return new List<Int2>(nodes);
		}

		private float LineWeight(Int2 s, Int2 e)
		{
			float weight = 0f;
			foreach (Int2 p in Int2.LineIterator(s, e))
			{
				weight += GetWeight(p);
			}
			return weight;
		}

		private List<Int2> ReduceClose(List<Int2> path)
		{
			int dx = 0, dz = 0;

			List<Int2> newPath = new List<Int2>();
			for (int i = 1; i < path.Count; ++i)
			{
				dx = Mathf.Abs(path[i].x - path[i - 1].x);
				dz = Mathf.Abs(path[i].z - path[i - 1].z);
				if (dx > 1 || dz > 1)
				{
					newPath.Add(path[i - 1]);
				}
			}
			newPath.Add(path[path.Count - 1]);
			return newPath;
		}

		private List<Int2> ReduceByAngle(List<Int2> path)
		{
			int dx = 0, dz = 0, ldx = 0, ldz = 0;

			List<Int2> newPath = new List<Int2>();
			for (int i = 1; i < path.Count; ++i)
			{
				dx = path[i].x - path[i - 1].x;
				dz = path[i].z - path[i - 1].z;
				if (dx != ldx || dz != ldz)
				{
					newPath.Add(path[i - 1]);
				}
				ldx = dx;
				ldz = dz;
			}
			newPath.Add(path[path.Count - 1]);

			return newPath;
		}

		private Chunk GetChunkByChunkPosition(Int2 pos)
		{
			Chunk chunk = null;
			_chunkByPosition.TryGetValue(pos, out chunk);
			return chunk;
		}

		private float GetWeight(Int2 wPos)
		{
			Chunk chunk = GetChunkByChunkPosition(_levelProfile.BlockToChunk(wPos));
			if (chunk == null)
			{
				return Chunk.WeightLimit;
			}
			float weight = chunk.GetWeight(chunk.LocalBlockPosition(wPos));
			return weight;
		}

		private bool Visiting(Int2 pos, ref Int2 endPos)
		{
			Chunk chunk = GetChunkByChunkPosition(_levelProfile.BlockToChunk(pos));
			if (chunk == null)
			{
				return false;
			}
			if (_endChunkSelector.Update(chunk.Index))
			{
				endPos = _endChunkSelector.GetEndPosition();
				return false;
			}
			return false;
		}

		private void DrawPathPoint(Int2 point, Color color, string tag, float height = 20f)
		{
			Vector3 p = _levelProfile.BlockToWorld(point);
			DrawLine.Add(p, p + Vector3.up * height, tag, color);
		}

		private void DrawPathPoints(List<Int2> path, Color color, string tag, float height = 20f)
		{
			for (int i = 0; i < path.Count; ++i)
			{
				Vector3 p = _levelProfile.BlockToWorld(path[i]);
				DrawLine.Add(p, p + Vector3.up * height, tag, color);
			}
		}
	}
}
