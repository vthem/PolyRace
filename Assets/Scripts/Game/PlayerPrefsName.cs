namespace Game
{
	public class PlayerPrefsName
	{
		public const string ALLOW_GA = "ALLOW_GA";
		public const string LAST_PLAYER = "LAST_PLAYER";
		public const string ALLOW_PARTICLE_EFFECT = "ALLOW_PARTICLE_EFFECT";
		public const string ALLOW_CELL_SHADING = "ALLOW_CELL_SHADING";
		public const string SFX_VOLUME = "SFX_VOLUME";
		public const string MUSIC_VOLUME = "MUSIC_VOLUME";
		public const string MASTER_VOLUME = "MASTER_VOLUME";
		public const string LANGUAGE = "LANG";
		public const string MISSION_COMPLETION = "MISSION_COMPLETION";
		public const string SPEED_UNIT = "SPEED_UNIT";
		public const string REPLAY_CAMERA_MODE = "REPLAY_CAMERA_MODE";
		public const string LAST_PLAYED_MISSION = "LAST_PLAYED_MISSION";


		public static string BuildPlayerStorageName(string userName)
		{
			return "USER_DATA_" + userName;
		}
	}
}
