using TSW.Struct;

namespace TSW.Algorithm.Rasterization
{
	public struct Edge
	{
		public Int2 begin;
		public Int2 end;

		public Edge(Int2 b, Int2 e)
		{
			if (b.z < e.z)
			{
				begin = b;
				end = e;
			}
			else
			{
				begin = e;
				end = b;
			}
		}
	}
}
