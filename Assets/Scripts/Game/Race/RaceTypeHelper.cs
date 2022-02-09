namespace Game.Race
{
	public static class RaceTypeHelper
	{
		public static bool IsEndless(this RaceType type)
		{
			return type == RaceType.Distance || type == RaceType.Endless;
		}
	}
}