namespace Game.Player
{
	[System.Serializable]
	public class SerializableUpgradePoints
	{
		public int pointsAvailable;
		public Racer.DynamicPropertyPoints points = new Racer.DynamicPropertyPoints();
	}

	public static class SerializableUpgradePointsHelper
	{
		public static string ToString(SerializableUpgradePoints data)
		{
			string s = "points available: " + data.pointsAvailable + "\n";
			s += "dynamic prop points: " + Racer.DPPointsHelper.ToString(data.points);
			return s;
		}
	}
}
