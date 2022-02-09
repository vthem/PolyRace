using System.Collections;
using System.Collections.Generic;

using TSW;

using UnityEngine;

namespace LevelGen.Jobs
{
	public class SideArrowObjectCreator : Job
	{
		private struct PositionKey
		{
			public int _in;
			public int _out;
			public int _edge;

			public PositionKey(int i, int o, int edge)
			{
				_in = i;
				_out = o;
				_edge = edge;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is PositionKey))
				{
					return false;
				}
				PositionKey other = (PositionKey)obj;
				return other._in == _in && other._out == _out && other._edge == _edge;
			}

			public override int GetHashCode()
			{
				return _in * 100 + _out * 10 + _edge;
			}

			public override string ToString()
			{
				return string.Format("[PositionKey] in:{0} out:{1} edge:{2}", _in, _out, _edge);
			}
		}

		private struct PositionValue
		{
			public int _start;
			public int _end;

			public PositionValue(int start, int end)
			{
				_start = start;
				_end = end;
			}

			public override string ToString()
			{
				return string.Format("[PositionValue] start:{0} end:{1}", _start, _end);
			}
		}

		private static readonly Dictionary<PositionKey, PositionValue> _positionDictionary = new Dictionary<PositionKey, PositionValue>() {
			{ new PositionKey(0, 1, 2), new PositionValue(0, 3) }, // 1
			{ new PositionKey(0, 1, 3), new PositionValue(1, 0) }, // 2
			{ new PositionKey(0, 2, 1), new PositionValue(2, 3) }, // 3
			{ new PositionKey(0, 2, 3), new PositionValue(1, 0) }, // 4
			{ new PositionKey(0, 3, 1), new PositionValue(2, 3) }, // 5
			{ new PositionKey(0, 3, 2), new PositionValue(3, 0) }, // 6
			{ new PositionKey(1, 2, 0), new PositionValue(2, 1) }, // 7
			{ new PositionKey(1, 2, 3), new PositionValue(1, 0) }, // 8
			{ new PositionKey(1, 0, 2), new PositionValue(3, 0) }, // 9
			{ new PositionKey(1, 0, 3), new PositionValue(0, 1) }, // 10
			{ new PositionKey(1, 3, 0), new PositionValue(2, 1) }, // 11
			{ new PositionKey(1, 3, 2), new PositionValue(3, 0) }, // 12
			{ new PositionKey(2, 0, 1), new PositionValue(3, 2) }, // 13
			{ new PositionKey(2, 0, 3), new PositionValue(0, 1) }, // 14
			{ new PositionKey(2, 1, 0), new PositionValue(1, 2) }, // 15
			{ new PositionKey(2, 1, 3), new PositionValue(0, 1) }, // 16
			{ new PositionKey(2, 3, 0), new PositionValue(2, 1) }, // 17
			{ new PositionKey(2, 3, 1), new PositionValue(3, 2) }, // 18
			{ new PositionKey(3, 0, 1), new PositionValue(3, 2) }, // 19
			{ new PositionKey(3, 0, 2), new PositionValue(0, 3) }, // 20
			{ new PositionKey(3, 1, 0), new PositionValue(1, 2) }, // 21
			{ new PositionKey(3, 1, 2), new PositionValue(0, 3) }, // 22
			{ new PositionKey(3, 2, 0), new PositionValue(1, 2) }, // 23
			{ new PositionKey(3, 2, 1), new PositionValue(2, 3) }  // 24
		};
		private static readonly Vector2[] _positionCoordinate = new Vector2[] {
			new Vector2(0, 0),
			new Vector2(0, 1),
			new Vector2(1, 1),
			new Vector2(1, 0)
		};
		private static readonly Vector2[] _positionBorderMargin = new Vector2[] {
			new Vector2(1, 1),
			new Vector2(1, -1),
			new Vector2(-1, -1),
			new Vector2(-1, 1)
		};
		private readonly TidyGameObjectDelegate _tidyGameObject;

		public SideArrowObjectCreator(LevelProfile level, TidyGameObjectDelegate tidyGameObject) : base(level)
		{
			Weight = 80f;
			_runType = RunType.RunInCoroutine;
			_tidyGameObject = tidyGameObject;
		}

		protected override IEnumerator RunByStep()
		{
			if (!_levelProfile._scenery._enable)
			{
				yield break;
			}
			SetTotalStepRequired();
			float midBorder = _levelProfile.Terrain.BorderSize / 2f;
			foreach (Chunk chunk in ChunksNoSeam())
			{
				for (int e = 0; e < 4; ++e)
				{
					PositionValue pv;
					if (_positionDictionary.TryGetValue(new PositionKey(chunk.InDirection, chunk.OutDirection, e), out pv))
					{

						yield return null;
						Vector3 wStart = chunk.Position;
						wStart.x += midBorder * _positionBorderMargin[pv._start].x;
						wStart.z += midBorder * _positionBorderMargin[pv._start].y;
						wStart.x += _levelProfile.Terrain.ChunkSizeXZ * _positionCoordinate[pv._start].x;
						wStart.z += _levelProfile.Terrain.ChunkSizeXZ * _positionCoordinate[pv._start].y;
						Vector3 wEnd = chunk.Position;
						wEnd.x += midBorder * _positionBorderMargin[pv._end].x;
						wEnd.z += midBorder * _positionBorderMargin[pv._end].y;
						wEnd.x += _levelProfile.Terrain.ChunkSizeXZ * _positionCoordinate[pv._end].x;
						wEnd.z += _levelProfile.Terrain.ChunkSizeXZ * _positionCoordinate[pv._end].y;
						//						Log ("chunk:" + chunk.Index + " pos:" + chunk.WorldPosition() + " found keyValue:" + pv + " start:" + wStart + " end:" + wEnd);
						float step = Mathf.Abs((wEnd - wStart).magnitude) / 4f;
						foreach (Vector3 position in VectorUtils.Range(wStart, wEnd, step))
						{
							Vector3 validPosition = position;
							if (_levelProfile.Terrain.GetTerrainHeight(ref validPosition))
							{
								//								DrawLine.Add(validPosition, validPosition + Vector3.up * 200f, "pole", Color.green);
								CreateObject(validPosition, GetForward(wStart, wEnd));
							}
						}
					}
				}
			}
			yield break;
		}

		private Vector3 GetForward(Vector3 start, Vector3 end)
		{
			start.y = 0f;
			end.y = 0;
			return (end - start).normalized;
		}

		private void SetTotalStepRequired()
		{
			_totalStep = 0;
			foreach (Chunk chunk in ChunksNoSeam())
			{
				if (chunk.InDirection != 4 && chunk.OutDirection != 4)
				{
					_totalStep += 4;
				}
			}
		}

		private void CreateObject(Vector3 position, Vector3 forward)
		{
			GameObject obj = GameObject.Instantiate(_levelProfile._scenery._sideArrowPrefab);
			obj.name = CreateObjectName(_levelProfile._scenery._sideArrowPrefab.name, position);
			obj.transform.position = position;
			obj.transform.forward = forward;
			obj.isStatic = true;
			_tidyGameObject(obj, LevelObjectType.Scenery, true);
		}

		private string CreateObjectName(string prefabName, Vector3 position)
		{
			return prefabName + "[" + Mathf.FloorToInt(position.x) + "," + Mathf.FloorToInt(position.z) + "]";
		}
	}
}
