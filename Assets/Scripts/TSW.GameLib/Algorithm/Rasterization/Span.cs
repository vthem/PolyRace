
namespace TSW.Algorithm.Rasterization
{
	public struct Span
	{
		public int X1, X2;

		public Span(int x1, int x2)
		{
			if (x1 < x2)
			{
				X1 = x1;
				X2 = x2;
			}
			else
			{
				X1 = x2;
				X2 = x1;
			}
		}
	}
}