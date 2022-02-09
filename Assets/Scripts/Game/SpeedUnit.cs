namespace Game
{
	public enum SpeedSystemType
	{
		International,
		Imperial
	}

	public static class SpeedUnit
	{
		public static float ConvertTo(SpeedSystemType systemType, float speed)
		{
			switch (systemType)
			{
				case SpeedSystemType.Imperial:
					return speed * 2.236f;
				case SpeedSystemType.International:
					return speed * 3.6f;
			}
			return speed;
		}

		public static string UnitString(SpeedSystemType systemType)
		{
			switch (systemType)
			{
				case SpeedSystemType.Imperial:
					return "mph";
				case SpeedSystemType.International:
					return "km/h";
			}
			return "m/s";
		}
	}
}