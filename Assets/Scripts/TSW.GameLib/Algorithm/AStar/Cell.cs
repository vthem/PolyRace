using System;

using TSW.Struct;

namespace TSW.Algorithm.AStar
{

	public sealed class Cell : IComparable<Cell>
	{
		public Int2 position;
		public float gScore;
		public float fScore => gScore + heuristic;
		public float heuristic;
		public Cell from;

		public Cell(Int2 key, Cell from, float gScore, float heuristic)
		{
			this.position = key;
			this.gScore = gScore;
			this.heuristic = heuristic;
			this.from = from;
		}

		public override int GetHashCode()
		{
			return position.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("[Cell] {0} f:{1} g:{2} h:{3} from:{4}", position, fScore, gScore, heuristic, from == null ? "no" : from.position.ToString());
		}

		public static bool operator ==(Cell a, Cell b)
		{
			if ((object)a == null && (object)b == null)
			{
				return true;
			}
			if ((object)a == null || (object)b == null)
			{
				return false;
			}
			return a.position == b.position;
		}

		public static bool operator !=(Cell a, Cell b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Cell))
			{
				return false;
			}
			Cell b = (Cell)obj;
			return position == b.position;
		}

		public int CompareTo(Cell other)
		{
			if (fScore > other.fScore)
			{
				return 1;
			}
			else if (fScore < other.fScore)
			{
				return -1;
			}
			return 0;
		}
	}
}