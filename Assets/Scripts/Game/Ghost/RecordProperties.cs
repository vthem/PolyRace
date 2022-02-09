namespace Game.Ghost
{
	[System.Serializable]
	public class RecordProperties
	{
		private readonly int _racerId;
		private readonly int _raceType;
		private readonly string _seedString;
		private readonly string _name;
		private string _playerName;

		public int RacerId => _racerId;
		public string SeedString => _seedString;
		public string Name => _name;
		public string PlayerName { get => _playerName; set => _playerName = value; }
		public int RaceType => _raceType;

		public RecordProperties(int raceId, string seedString, string name, string playerName, int raceType)
		{
			_racerId = raceId;
			_seedString = seedString;
			_name = name;
			_playerName = playerName;
			_raceType = raceType;
		}

		public override string ToString()
		{
			return string.Format("[RecordProperties: RacerId={0}, SeedString={1}, Name={2}, PlayerName={3}, RaceType={4}]", RacerId, SeedString, Name, PlayerName, RaceType);
		}
	}

	public static class RacerIdColor
	{
		public static int GetIntValue(int racerIndex, int colorIndex)
		{
			return colorIndex << 4 | racerIndex;
		}

		public static int GetColorIndex(int intValue)
		{
			return intValue >> 4 & 0xf;
		}

		public static int GetRacerIndex(int intValue)
		{
			return intValue & 0xf;
		}
	}
}
