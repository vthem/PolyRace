namespace TSW
{
	public class Math
	{
		public static int Mod(int value, int mod)
		{
			int r = value % mod;
			return r < 0 ? r + mod : r;
		}
	}
}
