using SysRandom = System.Random;

namespace LevelGen
{
	public class LevelIdentifier
	{

		public string SeedString { get; private set; }
		public LevelRegion LevelRegion { get; private set; }
		public LevelDifficulty LevelDifficulty { get; private set; }

		public LevelIdentifier(LevelRegion region, LevelDifficulty difficulty)
		{
			SysRandom random = new SysRandom(System.DateTime.Now.GetHashCode());
			LevelRegion = region;
			LevelDifficulty = difficulty;
			SeedString = RandLetter(random, region) + RandLetter(random) + RandDigit(random, difficulty) + RandDigit(random) + RandLetter(random) + RandLetter(random);
		}

		public LevelIdentifier(int day, LevelDifficulty minDifficulty)
		{
			SeedString = GenerateSeedString(day, minDifficulty);
			LevelRegion = ExtractRegionFromString(SeedString);
			LevelDifficulty = ExtractDifficultyFromString(SeedString);
		}

		public LevelIdentifier(string seedString)
		{
			SeedString = seedString;
			LevelRegion = ExtractRegionFromString(SeedString);
			LevelDifficulty = ExtractDifficultyFromString(SeedString);
		}

		public static LevelIdentifier Randomize()
		{
			SysRandom random = new SysRandom(System.DateTime.Now.GetHashCode());
			return new LevelIdentifier(GenerateSeedString(random, 0));
		}

		public static string GenerateSeedString(SysRandom random, LevelDifficulty minDifficulty)
		{
			return RandLetter(random) + RandLetter(random) + RandDigit(random, MinDifficultyDigit(minDifficulty)) + RandDigit(random) + RandLetter(random) + RandLetter(random);
		}

		public static string GenerateSeedString(int day, LevelDifficulty minDifficulty)
		{
			SysRandom random = new SysRandom(day.GetHashCode());
			return GenerateSeedString(random, minDifficulty);
		}

		private static string RandLetter(SysRandom random)
		{
			return ((char)random.Next(97, 123)).ToString();
		}

		private static string RandDigit(SysRandom random, int min = 0)
		{
			return random.Next(min, 10).ToString();
		}

		// difficulty
		// 0 => easy
		// 1 2 => normal
		// 3 4 5 6 => hard
		// 7 8 9 => extreme
		private static string RandDigit(SysRandom random, LevelDifficulty difficulty)
		{
			switch (difficulty)
			{
				case LevelDifficulty.Easy: // 0
					return "0";
				case LevelDifficulty.Normal: // 1 2
					return random.Next(1, 3).ToString();
				case LevelDifficulty.Hard: // 3 4 5 6
					return random.Next(3, 7).ToString();
				case LevelDifficulty.Extreme: // 7 8 9
					return random.Next(7, 10).ToString();
			}
			return "-";
		}

		private static int MinDifficultyDigit(LevelDifficulty difficulty)
		{
			switch (difficulty)
			{
				case LevelDifficulty.Easy: // 0
					return 0;
				case LevelDifficulty.Normal: // 1 2
					return 1;
				case LevelDifficulty.Hard: // 3 4 5 6
					return 3;
				case LevelDifficulty.Extreme: // 7 8 9
					return 7;
			}
			return 0;
		}

		private static string RandLetter(SysRandom random, LevelRegion region)
		{
			switch (region)
			{
				case LevelRegion.Continental:
					return ((char)random.Next(97, 109)).ToString();
				case LevelRegion.Desert:
					return ((char)random.Next(109, 116)).ToString();
				case LevelRegion.Arctic:
					return ((char)random.Next(116, 123)).ToString();
			}
			return "-";
		}

		public static LevelRegion ExtractRegionFromString(string str)
		{
			int ascii = str[0];
			if (ascii < 109)
			{
				return LevelRegion.Continental;
			}
			else if (ascii < 116)
			{
				return LevelRegion.Desert;
			}
			else
			{
				return LevelRegion.Arctic;
			}
		}

		// difficulty
		// 0 => easy
		// 1 2 => normal
		// 3 4 5 6 => hard
		// 7 8 9 => extreme
		public static LevelDifficulty ExtractDifficultyFromString(string str)
		{
			int digit = str[2] - 48;
			if (digit == 0)
			{
				return LevelDifficulty.Easy;
			}
			else if (digit > 0 && digit < 3)
			{
				return LevelDifficulty.Normal;
			}
			else if (digit >= 3 && digit < 7)
			{
				return LevelDifficulty.Hard;
			}
			return LevelDifficulty.Extreme;
		}

		public override string ToString()
		{
			return string.Format("[LevelIdentifier: SeedString={0}, Region={1}, Difficulty={2}]", SeedString, LevelRegion, LevelDifficulty);
		}
	}
}
