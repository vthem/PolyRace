using System.Collections.Generic;

using TSW.Struct;

using UnityEngine;

namespace TSW.Algorithm.AStar
{
	public class PathFinder
	{
		public delegate float GetWeight(Int2 pos);
		public delegate bool Visiting(Int2 pos, ref Int2 endPos);

		public float _d = 0.901f;
		public bool EndReached { get; private set; }

		private readonly Dictionary<Int2, Cell> _closeSet = new Dictionary<Int2, Cell>();
		private readonly PriorityQueueSet _openSet = new PriorityQueueSet();

		public int FindPath(Int2 sPos, Int2 ePos, GetWeight getWeight, Visiting visitingFunc, out List<Int2> path)
		{
			Cell start = new Cell(sPos, null, 0f, Heuristic(sPos, ePos));
			_openSet.Add(start);

			path = Path(start, ePos, getWeight, visitingFunc);
			return path.Count;
		}

		public List<Int2> GetPath(Int2 ePos)
		{
			Cell cur;
			if (!_closeSet.TryGetValue(ePos, out cur))
			{
				throw new System.Exception("Fail to find pos:" + ePos + " in closed set");
			}
			List<Int2> path = new List<Int2>();
			ReconstructPath(cur, path);
			return path;
		}

		private List<Int2> Path(Cell start, Int2 ePos, GetWeight getWeight, Visiting visitingFunc)
		{
			Cell cur = null;
			Cell neighborCell = null;
			float gScore = 0f;
			EndReached = false;
			int idx = 0;

			int nVisit = 0;
			while ((cur = _openSet.Get()) != null)
			{
				nVisit++;
				if (visitingFunc != null && visitingFunc(cur.position, ref ePos))
				{
					_openSet.Clear();
					_closeSet.Clear();
				}
				_closeSet.Add(cur.position, cur);
				//				Log("cur:" + cur);
				if (cur.position == ePos)
				{
					EndReached = true;
					break;
				}
				idx = 0;
				foreach (Int2 neighborPos in Int2.Neighbors(cur.position))
				{
					gScore = cur.gScore + _neighbourGScore[idx] + getWeight(neighborPos);
					neighborCell = null;
					if (_openSet.Contains(neighborPos, out neighborCell) && gScore < neighborCell.gScore)
					{
						_openSet.Remove(neighborPos);
					}
					if (_closeSet.TryGetValue(neighborPos, out neighborCell) && gScore < neighborCell.gScore)
					{
						_closeSet.Remove(neighborPos);
					}
					if (!_openSet.Contains(neighborPos, out neighborCell) && !_closeSet.TryGetValue(neighborPos, out neighborCell))
					{
						neighborCell = new Cell(neighborPos, cur, gScore, Heuristic(neighborPos, ePos));
						_openSet.Add(neighborCell);
						//						Log(" add:" + neighborCell);
					}
					idx++;
				}
			}

			List<Int2> path = new List<Int2>();
			if (cur != null)
			{
				ReconstructPath(cur, path);
			}
			return path;
		}

		private static int ReconstructPath(Cell cur, List<Int2> path)
		{
			//Log("End with cur:" + cur);
			while (cur.from != null)
			{
				path.Add(cur.position);
				cur = cur.from;
			}
			path.Add(cur.position);
			path.Reverse();
			//Log("AStar return");
			return path.Count;
		}

		/// <summary>
		/// 701
		/// 6x2
		/// 543
		/// </summary>
		private static readonly float[] _neighbourGScore = new float[] {
			1f, // 0
			1.41f, // 1
			1f, // 2
			1.41f, // 3
			1f, // 4
			1.41f, // 5
			1f, // 6
			1.41f // 7
		};

		private float Heuristic(Int2 cur, Int2 end)
		{
			int dx = Mathf.Abs(end.x - cur.x);
			int dz = Mathf.Abs(end.z - cur.z);
			return _d * Mathf.Sqrt(dx * dx + dz * dz);
		}

		private static void Log(string text)
		{
			Debug.Log("PathFinder/ " + text);
		}
	}
}
