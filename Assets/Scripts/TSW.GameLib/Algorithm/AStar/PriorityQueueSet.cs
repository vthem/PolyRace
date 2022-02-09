using System;
using System.Collections.Generic;

using TSW.Struct;

namespace TSW.Algorithm.AStar
{
	public class PriorityQueue<T> where T : IComparable<T>
	{
		private readonly List<T> data;

		public PriorityQueue()
		{
			this.data = new List<T>();
		}

		public void Enqueue(T item)
		{
			data.Add(item);
			int ci = data.Count - 1; // child index; start at end
			while (ci > 0)
			{
				int pi = (ci - 1) / 2; // parent index
				if (data[ci].CompareTo(data[pi]) >= 0)
				{
					break; // child item is larger than (or equal) parent so we're done
				}

				T tmp = data[ci]; data[ci] = data[pi]; data[pi] = tmp;
				ci = pi;
			}
		}

		public T Dequeue()
		{
			// assumes pq is not empty; up to calling code
			int li = data.Count - 1; // last index (before removal)
			T frontItem = data[0];   // fetch the front
			data[0] = data[li];
			data.RemoveAt(li);

			--li; // last index (after removal)
			int pi = 0; // parent index. start at front of pq
			while (true)
			{
				int ci = pi * 2 + 1; // left child index of parent
				if (ci > li)
				{
					break;  // no children so done
				}

				int rc = ci + 1;     // right child
				if (rc <= li && data[rc].CompareTo(data[ci]) < 0) // if there is a rc (ci + 1), and it is smaller than left child, use the rc instead
				{
					ci = rc;
				}

				if (data[pi].CompareTo(data[ci]) <= 0)
				{
					break; // parent is smaller than (or equal to) smallest child so done
				}

				T tmp = data[pi]; data[pi] = data[ci]; data[ci] = tmp; // swap parent and child
				pi = ci;
			}
			return frontItem;
		}

		public T Peek()
		{
			T frontItem = data[0];
			return frontItem;
		}

		public int Count()
		{
			return data.Count;
		}

		public override string ToString()
		{
			string s = "";
			for (int i = 0; i < data.Count; ++i)
			{
				s += data[i].ToString() + " ";
			}

			s += "count = " + data.Count;
			return s;
		}
	}

	public sealed class PriorityQueueSet
	{
		private PriorityQueue<Cell> _queue = new PriorityQueue<Cell>();
		private Dictionary<Int2, Cell> _dictionary = new Dictionary<Int2, Cell>();

		public void Add(Cell cell)
		{
			_queue.Enqueue(cell);
			_dictionary.Add(cell.position, cell);
		}

		public Cell Get()
		{
			if (_queue.Count() == 0)
			{
				return null;
			}
			Cell cell = _queue.Dequeue();
			if (_dictionary.ContainsKey(cell.position))
			{
				_dictionary.Remove(cell.position);
			}
			else
			{
				return Get();
			}
			return cell;
		}

		public bool Contains(Int2 position, out Cell cell)
		{
			return _dictionary.TryGetValue(position, out cell);
		}

		public void Remove(Int2 position)
		{
			_dictionary.Remove(position);
		}

		public void Clear()
		{
			_queue = new PriorityQueue<Cell>();
			_dictionary = new Dictionary<Int2, Cell>();
		}
	}
}