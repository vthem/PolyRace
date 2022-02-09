namespace Game.Player
{
	// If you modify this class, the version number should be incremented in PlayerManager
	[System.Serializable]
	public class SerializableData
	{
		public int version;
		public int[] cameraView;
		public int utc;
	}
}
