namespace Game.Race
{
	public static class Identifier
	{
		public static string BuildString(LevelGen.LevelIdentifier levelId, Race.RaceType raceType)
		{
			string s = levelId.SeedString;
			switch (raceType)
			{
				case RaceType.Distance:
					s += "-D";
					break;
				case RaceType.TimeAttack:
					s += "-T";
					break;
				case RaceType.Endless:
					s += "-E";
					break;
			}
			return s;
		}
	}
}
