using System.Collections.Generic;

using TSW.Struct;
using TSW.Unity;

using UnityEngine;

namespace LevelGen
{
	public class OverlappingChecker
	{
		private readonly HashSet<Int2> _set = new HashSet<Int2>();
		private readonly float _size = 1f;
		private readonly int _square = 0;

		public OverlappingChecker(float size, int square = 0)
		{
			_size = size;
			_square = square;
			if (_square == 1)
			{
				_square += 2;
			}
			if (_square > 0 && _square % 2 == 0)
			{
				_square += 1;
			}
		}

		public void Set(Vector3 position)
		{
			if (_square == 0)
			{
				_set.Add(Convert(position));
			}
			else
			{
				Int2 pos = Convert(position);
				Int2 bl = new Int2(pos.x - ((_square - 1) / 2), pos.z - ((_square - 1) / 2)); // bottom-left
				foreach (Int2 next in bl.Iteration(_square, _square))
				{
					_set.Add(next);
				}
			}
		}

		public void Set(Vector3 position, float xLen, float zLen)
		{
			Int2 pos = Convert(position);
			foreach (Int2 next in pos.Iteration(Convert(xLen), Convert(zLen)))
			{
				_set.Add(next);
			}
		}

		public void Clear()
		{
			_set.Clear();
		}

		public void DebugDraw(string tag, Color color)
		{
			foreach (Int2 pos in _set)
			{
				Vector3 v = Convert(pos);
				DrawLine.Add(v, v + Vector3.up * 100f, tag, color);
			}
		}

		public bool Check(Vector3 position)
		{
			return _set.Contains(Convert(position));
		}

		private Int2 Convert(Vector3 position)
		{
			return new Int2(Mathf.FloorToInt(position.x / _size), Mathf.FloorToInt(position.z / _size));
		}

		private Vector3 Convert(Int2 posision, float height = 0f)
		{
			return new Vector3(posision.x * _size, height, posision.z * _size);
		}

		private int Convert(float v)
		{
			return Mathf.FloorToInt(v / _size);
		}
	}
}
