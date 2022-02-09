using TSW.Struct;

namespace LevelGen
{
	public struct LevelShapeCell
	{
		public Int2 Position { get; private set; }

		public int Direction { get; private set; }

		public static int ReverseDirection(int direction)
		{
			return (direction + 2) % 4;
		}

		public LevelShapeCell(int x, int z, int direction) : this()
		{
			Position = new Int2(x, z);
			Direction = direction;
		}

		public LevelShapeCell(Int2 position, int direction) : this()
		{
			Position = position;
			Direction = direction;
		}

		public override string ToString()
		{
			return string.Format("[Cell: Position={0}, Direction={1}]", Position, Direction);
		}
	}
}
