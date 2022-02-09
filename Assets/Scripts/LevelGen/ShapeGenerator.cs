using System.Collections.Generic;

using TSW;
using TSW.Struct;

using UnityEngine;

using SysRandom = System.Random;

namespace LevelGen
{
	public class ShapeGenerator
	{
		private readonly int _maxLength;
		private readonly SysRandom _random;
		private int _lastOp = 0;
		private readonly int _seed;
		private Int2 _minBox;
		private Int2 _maxBox;

		public ShapeGenerator(int maxLength, int seed)
		{
			_maxLength = maxLength;
			_random = new SysRandom(seed);
			_seed = seed;
			_minBox = new Int2(0, 0);
			_maxBox = new Int2(0, 0);
		}

		public IEnumerable<LevelShapeCell> GetNextCell(int index, int length)
		{
			if (index == 0)
			{
				_lastOp = 0;
			}

			LevelShapeCell cur = new LevelShapeCell(0, 0, 0);
			if (index > 0)
			{
				int indexCount = 0;
				foreach (LevelShapeCell cell in GetNextCell(0, -1))
				{
					if (indexCount == index)
					{
						cur = cell;
						break;
					}
					indexCount++;
				}
			}
			yield return cur;

			++index;
			--length;

			while (length != 0)
			{
				cur = GetNextCell(index, cur);
				yield return cur;
				++index;
				--length;
			}
		}

		private static readonly Int2[] _directionOffset = new Int2[4] {
			new Int2(0, 1),
			new Int2(1, 0),
			new Int2(0, -1),
			new Int2(-1, 0)
		};

		private LevelShapeCell GetNextCell(int index, LevelShapeCell cur)
		{
			if (index == 1)
			{
				return NewCell(cur.Position.x, cur.Position.z + 1, cur.Direction);
			}
			else if (_maxLength != -1 && index > _maxLength - 2)
			{
				return NewCell(_directionOffset[cur.Direction] + cur.Position, cur.Direction);
			}
			// we don't do, two left, two right in a row
			List<int> randOp = new List<int>(); // left, straight / right
			for (int i = -1; i < 2; ++i)
			{
				if (_lastOp != 0 && i == _lastOp)
				{
					continue;
				}
				randOp.Add(i);
			}
			Reshuffle(randOp);
			Int2 newPos = cur.Position + _directionOffset[cur.Direction];
			for (int i = 0; i < randOp.Count; ++i)
			{
				int newDir = TSW.Math.Mod(cur.Direction + randOp[i], 4);
				//				Debug.Log("Test pos:" + newPos + " min:" + _minBox + " max:" + _maxBox);
				if (IsNewDirAllowed(newPos, newDir))
				{
					_lastOp = randOp[i];
					_minBox.x = Mathf.Min(newPos.x, _minBox.x);
					_minBox.z = Mathf.Min(newPos.z, _minBox.z);
					_maxBox.x = Mathf.Max(newPos.x, _maxBox.x);
					_maxBox.z = Mathf.Max(newPos.z, _maxBox.z);
					//					Debug.Log("=> new pos:" + newPos + " min:" + _minBox + " max:" + _maxBox);
					return NewCell(newPos, newDir);
				}
				else
				{
					//					Debug.Log("=> invalid");
				}
			}
			throw new System.Exception("Impossible path, index:" + index + " seed:" + _seed);
		}

		private LevelShapeCell NewCell(int x, int z, int dir)
		{
			return NewCell(new Int2(x, z), dir);
		}

		private LevelShapeCell NewCell(Int2 pos, int dir)
		{
			_minBox.x = Mathf.Min(pos.x, _minBox.x);
			_minBox.z = Mathf.Min(pos.z, _minBox.z);
			_maxBox.x = Mathf.Max(pos.x, _maxBox.x);
			_maxBox.z = Mathf.Max(pos.z, _maxBox.z);
			return new LevelShapeCell(pos, dir);
		}

		private bool IsNewDirAllowed(Int2 pos, int dir)
		{
			pos = _directionOffset[dir] + pos;
			return (pos.x > _maxBox.x || pos.x < _minBox.x) || (pos.z > _maxBox.z || pos.z < _minBox.z);
		}

		private void Reshuffle(List<int> values)
		{
			// Knuth shuffle algorithm :: courtesy of Wikipedia :)
			for (int t = 0; t < values.Count; t++)
			{
				int tmp = values[t];
				int newIdx = _random.Range(t, values.Count);
				values[t] = values[newIdx];
				values[newIdx] = tmp;
			}
		}
	}
}
